using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CodeGen;
using CodeGen.Building;

namespace SkyrimLib.Generator
{
    internal static class Program
    {
        private static readonly HashSet<string> Records = new HashSet<string>();

        private static readonly HashSet<(string main, string sub)>
            SubRecords = new HashSet<(string main, string sub)>();

        private static string Title(this string v)
        {
            return char.ToUpper(v[0]) + v.Substring(1);
        }

        public sealed class FileFormat
        {
            public string From;
            public int Size;
            public string To;
            public string Description;
            public bool LengthPrefixed;
            public string LengthType;
            public bool NullTerminated;
            public string Read;
            public string Write;
        }

        internal static class FileFormats
        {
            public static readonly Dictionary<string, FileFormat> Formats = new Dictionary<string, FileFormat>();

            public static void Parse(string filename)
            {
                var doc = XDocument.Load(filename);
                var readPrefix = doc.Root.Attribute("readPrefix").Value;
                var writePrefix = doc.Root.Attribute("writePrefix").Value;
                foreach (var record in doc.Root.Elements("type"))
                {
                    var f = new FileFormat
                    {
                        From = record.Attribute("from").Value,
                        To = record.Attribute("to").Value,
                        Description = record.Attribute("description")?.Value
                    };

                    if (!f.From.EndsWith("string", StringComparison.Ordinal))
                    {
                        f.Size = int.Parse(record.Attribute("size").Value);
                    }
                    else
                    {
                        f.LengthPrefixed = bool.Parse(record.Attribute("lengthPrefixed").Value);
                        f.NullTerminated = bool.Parse(record.Attribute("nullTerminated").Value);
                        if (f.LengthPrefixed) f.LengthType = record.Attribute("lengthType").Value;
                    }

                    //Console.WriteLine(f.@from);
                    var suffix = record.Attribute("suffix").Value;
                    f.Read = readPrefix + (string.IsNullOrEmpty(suffix) ? "" : suffix);
                    f.Write = writePrefix + (string.IsNullOrEmpty(suffix) ? "" : suffix);

                    Formats.Add(f.From, f);
                }
            }
        }

        private sealed class RecordBuilder
        {
            private readonly Class _genClass;
            private readonly Method _constructorRead;
            private readonly Method _constructorEmpty;
            private readonly Method _getSubRecordsForWriting;
            private readonly string _name;
            private readonly Base _base;

            private RecordBuilder(string name, string description)
            {
                this._base = new Base
                {
                    Comment =
                        "ReSharper disable RedundantUsingDirective\nReSharper disable InconsistentNaming\nReSharper disable UseObjectOrCollectionInitializer\nReSharper disable RedundantCast"
                };
                this._base.Using.Add("System.Collections.Generic");
                this._base.Using.Add("System.Linq");
                var ns = new Namespace("SkyrimLib");
                this._base.Namespaces.Add(ns);
                this._name = name;
                Records.Add(name);
                this._genClass = new Class(name) {Modifiers = Modifiers.Public | Modifiers.Sealed};
                var s1 = new Field("Signature", "FieldType")
                {
                    Value = $"Signature.FromString(\"{name}\")",
                    Modifiers = Modifiers.Static | Modifiers.Public | Modifiers.ReadOnly
                };
                this._genClass.Members.Add(s1);

                this._genClass.Extends.Add("Record");

                this._constructorRead = new Method(null, name)
                {
                    Parameters = "IReader headerReader, IReader dataReader",
                    ConstructorInvocation = "base(headerReader, dataReader)",
                    Modifiers = Modifiers.Internal,
                    Body = new Builder()
                };
                this._genClass.Members.Add(this._constructorRead);

                this._constructorEmpty = new Method(null, name)
                {
                    ConstructorInvocation = "base(FieldType)",
                    Modifiers = Modifiers.Public
                };
                this._genClass.Members.Add(this._constructorEmpty);

                this._getSubRecordsForWriting = new Method("IReadOnlyList<SubRecord>", "GetSubRecordsForWriting")
                {
                    Modifiers = Modifiers.Override | Modifiers.Protected, Body = new Builder()
                };
                this._getSubRecordsForWriting.Body.AppendLine("var ret = new List<SubRecord>();");
                this._genClass.Members.Add(this._getSubRecordsForWriting);

                ns.Types.Add(this._genClass);
            }

            internal static RecordBuilder Begin(string type, string description)
            {
                return new RecordBuilder(type, description);
            }

            private static string GetLength(string name, FileFormat format)
            {
                if (name == null) name = "Value";
                if (format.To != "string") return $"{format.Size}";

                var prefix = string.Empty;
                if (format.LengthPrefixed)
                    switch (format.LengthType)
                    {
                        case "byte":
                            prefix = "1 + ";
                            break;
                        case "uint16":
                            prefix = "2 + ";
                            break;
                        case "uint32":
                            prefix = "4 + ";
                            break;
                        case "uint64":
                            prefix = "8 + ";
                            break;
                    }

                var suffix = string.Empty;
                if (format.NullTerminated) suffix = " + 1";
                return $"{prefix}this.{name}.Length{suffix}";
            }

            internal void AddSubRecord(string name, string description, bool required, FileFormat format)
            {
                SubRecords.Add((this._name, $"{this._name}_{name}"));
                var p = new Field($"{this._name}_{name}", name) {Modifiers = Modifiers.Public};
                if (required)
                {
                    p.Modifiers |= Modifiers.ReadOnly;
                    p.Value = $"new {this._name}_{name}()";
                }

                this._genClass.Members.Add(p);

                if (required)
                {
                    this._constructorRead.Body.AppendLine(
                        $"this.{name} = this.Fields.First(x => x.Type == {this._name}_{name}.FieldType) as {this._name}_{name};");
                    this._getSubRecordsForWriting.Body.AppendLine($"ret.Add(this.{name});");
                }
                else
                {
                    this._constructorRead.Body.AppendLine(
                        $"this.{name} = this.Fields.FirstOrDefault(x => x.Type == {this._name}_{name}.FieldType) as {this._name}_{name};");
                    this._getSubRecordsForWriting.Body.AppendLine($"if(this.{name} != null) ret.Add(this.{name});");
                }

                this.AddSubRecordNoField(name, description, required, format);
            }

            private void AddSubRecordNoField(string name, string description, bool required, FileFormat format)
            {
                SubRecords.Add((this._name, $"{this._name}_{name}"));
                var c = new Class($"{this._name}_{name}") {Modifiers = Modifiers.Public | Modifiers.Sealed};
                c.Extends.Add("SubRecord");
                var s1 = new Field("Signature", "FieldType")
                {
                    Value = $"Signature.FromString(\"{name}\")",
                    Modifiers = Modifiers.Static | Modifiers.Public | Modifiers.ReadOnly
                };
                c.Members.Add(s1);

                var f = new Field(format.To, "Value") {Modifiers = Modifiers.Public};
                c.Members.Add(f);

                var baseCon = new Method(null, c.Name)
                {
                    Modifiers = Modifiers.Internal,
                    Parameters = "IReader headerReader, IReader dataReader, uint overrideDataSize = 0",
                    ConstructorInvocation = "base(headerReader, dataReader, overrideDataSize)",
                    Body = new Builder()
                };
                baseCon.Body.AppendLine($"this.Value = dataReader.{format.Read}(0);");
                c.Members.Add(baseCon);

                var emtpyCon = new Method(null, c.Name)
                {
                    ConstructorInvocation = "base(FieldType)",
                    Modifiers = required ? Modifiers.Internal : Modifiers.Public
                };
                c.Members.Add(emtpyCon);

                if (!required)
                {
                    var valueConstructor = new Method(null, c.Name)
                    {
                        Modifiers = Modifiers.Public,
                        ConstructorInvocation = "base(FieldType)",
                        Parameters = $"{format.To} value",
                        Body = new Builder()
                    };
                    valueConstructor.Body.AppendLine("this.Value = value;");
                    c.Members.Add(valueConstructor);
                }

                var writer = new Method("void", "WriteData")
                {
                    Modifiers = Modifiers.Override | Modifiers.Protected,
                    Parameters = "IWriter writer",
                    Body = new Builder()
                };
                writer.Body.AppendLine($"writer.{format.Write}(this.Value);");
                c.Members.Add(writer);

                var length = new Method("ushort", "DataLength")
                {
                    Modifiers = Modifiers.Override | Modifiers.Protected, Body = new Builder()
                };
                length.Body.AppendLine($"return (ushort) ({GetLength(null, format)});");
                c.Members.Add(length);

                var bse = new Base
                {
                    Comment =
                        "ReSharper disable RedundantUsingDirective\nReSharper disable InconsistentNaming\nReSharper disable UseObjectOrCollectionInitializer\nReSharper disable RedundantCast"
                };
                bse.Using.Add("System.Collections.Generic");
                bse.Using.Add("System.Linq");
                var ns = new Namespace("SkyrimLib");
                bse.Namespaces.Add(ns);
                ns.Types.Add(c);
                File.WriteAllText($"../../../../SkyrimLib/Generated/{this._name}_{name}.cs", bse.BuildCode(true));
            }

            internal void AddSubRecordList(string name, string description, FileFormat format)
            {
                SubRecords.Add((this._name, $"{this._name}_{name}"));
                /* Property */
                var p = new Field($"{this._name}_{name}", name)
                {
                    Modifiers = Modifiers.Public | Modifiers.ReadOnly, Value = $"new {this._name}_{name}()"
                };
                this._genClass.Members.Add(p);

                this._constructorRead.Body.AppendLine(
                    $"this.{name} = this.Fields.First(x => x.Type == {this._name}_{name}.FieldType) as {this._name}_{name};");
                this._getSubRecordsForWriting.Body.AppendLine(
                    $"if(this.{name}.Values.Count > 0) ret.Add(this.{name});");

                /* Backing class */
                var c = new Class($"{this._name}_{name}") {Modifiers = Modifiers.Public | Modifiers.Sealed};
                c.Extends.Add("SubRecord");
                var s1 = new Field("Signature", "FieldType")
                {
                    Value = $"Signature.FromString(\"{name}\")",
                    Modifiers = Modifiers.Static | Modifiers.Public | Modifiers.ReadOnly
                };
                c.Members.Add(s1);

                var f = new Field($"List<{format.To}>", "Values")
                {
                    Modifiers = Modifiers.Public | Modifiers.ReadOnly, Value = $"new List<{format.To}>()"
                };
                c.Members.Add(f);

                var baseCon = new Method(null, c.Name)
                {
                    Modifiers = Modifiers.Internal,
                    Parameters = "IReader headerReader, IReader dataReader, uint overrideDataSize = 0",
                    ConstructorInvocation = "base(headerReader, dataReader, overrideDataSize)",
                    Body = new Builder()
                };
                baseCon.Body.AppendLine("var idx = 0;");
                baseCon.Body.AppendLine($"while (dataReader.Length > idx + {format.Size}){{");
                baseCon.Body.EnterBlock();
                baseCon.Body.AppendLine($"this.Values.Add(dataReader.{format.Read}(idx));");
                baseCon.Body.AppendLine($"idx += {format.Size};");
                baseCon.Body.LeaveBlock();
                baseCon.Body.AppendLine("}");
                c.Members.Add(baseCon);

                var emtpyCon = new Method(null, c.Name)
                {
                    ConstructorInvocation = "base(FieldType)", Modifiers = Modifiers.Internal
                };
                c.Members.Add(emtpyCon);

                var writer = new Method("void", "WriteData")
                {
                    Modifiers = Modifiers.Override | Modifiers.Protected,
                    Parameters = "IWriter writer",
                    Body = new Builder()
                };
                writer.Body.AppendLine($"foreach(var value in this.Values) writer.{format.Write}(value);");
                c.Members.Add(writer);

                var length = new Method("ushort", "DataLength")
                {
                    Modifiers = Modifiers.Override | Modifiers.Protected, Body = new Builder()
                };
                length.Body.AppendLine($"return (ushort) (this.Values.Count * {format.Size});");
                c.Members.Add(length);

                var bse = new Base
                {
                    Comment =
                        "ReSharper disable RedundantUsingDirective\nReSharper disable InconsistentNaming\nReSharper disable UseObjectOrCollectionInitializer\nReSharper disable RedundantCast"
                };
                bse.Using.Add("System.Collections.Generic");
                bse.Using.Add("System.Linq");
                var ns = new Namespace("SkyrimLib");
                bse.Namespaces.Add(ns);
                ns.Types.Add(c);
                File.WriteAllText($"../../../../SkyrimLib/Generated/{this._name}_{name}.cs", bse.BuildCode(true));
            }

            internal void AddSubRecordListStruct(string name, string description,
                IEnumerable<(string name, string type, string description, bool primary, FileFormat format)> fields)
            {
                var primary = fields.First(x => x.primary);
                foreach (var field in fields.Where(field => !field.primary))
                {
                    this._getSubRecordsForWriting.Body.AppendLine(
                        $"while(this.{field.name}.Count > this.{primary.name}.Count) this.{field.name}.RemoveAt(this.{field.name}.Count - 1);");
                    this._getSubRecordsForWriting.Body.AppendLine(
                        $"while(this.{field.name}.Count < this.{primary.name}.Count) this.{field.name}.Add(new {this._name}_{field.name}());");
                }

                this._getSubRecordsForWriting.Body.AppendLine($"for(var i = 0; i < this.{primary.name}.Count; i++){{");
                this._getSubRecordsForWriting.Body.EnterBlock();

                foreach (var field in fields)
                {
                    var f = new Field($"List<{this._name}_{field.name}>", field.name)
                    {
                        Modifiers = Modifiers.ReadOnly | Modifiers.Public,
                        Value = $"new List<{this._name}_{field.name}>()"
                    };
                    this._genClass.Members.Add(f);
                    this._constructorRead.Body.AppendLine(
                        $"this.{field.name}.AddRange(this.Fields.Where(x => x.Type == {this._name}_{field.name}.FieldType).Cast<{this._name}_{field.name}>());");
                    this.AddSubRecordNoField(field.name, field.description, false, field.format);
                    this._getSubRecordsForWriting.Body.AppendLine($"ret.Add(this.{field.name}[i]);");
                }

                this._getSubRecordsForWriting.Body.LeaveBlock();
                this._getSubRecordsForWriting.Body.AppendLine("}");
            }

            internal void AddSubRecordStruct(string name, string description, bool required,
                IEnumerable<(string name, string type, string description, bool primary, FileFormat format)> fields)
            {
                SubRecords.Add((this._name, $"{this._name}_{name}"));
                var p = new Field($"{this._name}_{name}", name) {Modifiers = Modifiers.Public};
                if (required)
                {
                    p.Modifiers |= Modifiers.ReadOnly;
                    p.Value = $"new {this._name}_{name}()";
                }

                this._genClass.Members.Add(p);

                if (required)
                {
                    this._constructorRead.Body.AppendLine(
                        $"this.{name} = this.Fields.First(x => x.Type == {this._name}_{name}.FieldType) as {this._name}_{name};");
                    this._getSubRecordsForWriting.Body.AppendLine($"ret.Add(this.{name});");
                }
                else
                {
                    this._constructorRead.Body.AppendLine(
                        $"this.{name} = this.Fields.FirstOrDefault(x => x.Type == {this._name}_{name}.FieldType) as {this._name}_{name};");
                    this._getSubRecordsForWriting.Body.AppendLine($"if(this.{name} != null) ret.Add(this.{name});");
                }

                var c = new Class($"{this._name}_{name}") {Modifiers = Modifiers.Public | Modifiers.Sealed};
                c.Extends.Add("SubRecord");
                var s1 = new Field("Signature", "FieldType")
                {
                    Value = $"Signature.FromString(\"{name}\")",
                    Modifiers = Modifiers.Static | Modifiers.Public | Modifiers.ReadOnly
                };
                c.Members.Add(s1);

                var baseCon = new Method(null, c.Name)
                {
                    Modifiers = Modifiers.Internal,
                    Parameters = "IReader headerReader, IReader dataReader, uint overrideDataSize = 0",
                    ConstructorInvocation = "base(headerReader, dataReader, overrideDataSize)",
                    Body = new Builder()
                };
                baseCon.Body.AppendLine("var idx = 0;");
                c.Members.Add(baseCon);

                var emtpyCon = new Method(null, c.Name)
                {
                    ConstructorInvocation = "base(FieldType)",
                    Modifiers = required ? Modifiers.Internal : Modifiers.Public
                };
                c.Members.Add(emtpyCon);
                var writer = new Method("void", "WriteData")
                {
                    Modifiers = Modifiers.Override | Modifiers.Protected,
                    Parameters = "IWriter writer",
                    Body = new Builder()
                };
                c.Members.Add(writer);
                var length = new Method("ushort", "DataLength")
                {
                    Modifiers = Modifiers.Override | Modifiers.Protected, Body = new Builder()
                };
                c.Members.Add(length);
                var lengths = new List<string>();
                foreach (var field in fields)
                {
                    var f = new Field(field.format.To, field.name) {Modifiers = Modifiers.Public};
                    c.Members.Add(f);

                    baseCon.Body.AppendLine($"this.{field.name} = dataReader.{field.format.Read}(idx);");
                    baseCon.Body.AppendLine($"idx += {GetLength(field.name, field.format)};");

                    writer.Body.AppendLine($"writer.{field.format.Write}(this.{field.name});");
                    lengths.Add($"{GetLength(null, field.format)}");
                }

                length.Body.AppendLine("return (ushort) (" + string.Join(" + ", lengths) + ");");

                var bse = new Base
                {
                    Comment =
                        "ReSharper disable RedundantUsingDirective\nReSharper disable InconsistentNaming\nReSharper disable UseObjectOrCollectionInitializer\nReSharper disable RedundantCast"
                };
                bse.Using.Add("System.Collections.Generic");
                bse.Using.Add("System.Linq");
                var ns = new Namespace("SkyrimLib");
                bse.Namespaces.Add(ns);
                ns.Types.Add(c);
                File.WriteAllText($"../../../../SkyrimLib/Generated/{this._name}_{name}.cs", bse.BuildCode(true));
            }

            internal void End()
            {
                this._constructorRead.Body.AppendLine("this.Fields.Clear();");
                this._getSubRecordsForWriting.Body.AppendLine("return ret;");

                File.WriteAllText($"../../../../SkyrimLib/Generated/{this._name}.cs", this._base.BuildCode(true));
            }
        }

        private static void ParseSubRecord([NotNull] XElement subRecord, RecordBuilder r)
        {
            var type = subRecord.Attribute("type").Value;
            var description = subRecord.Attribute("description")?.Value;
            bool.TryParse(subRecord.Attribute("required")?.Value, out var required);
            var name = subRecord.Attribute("name").Value;
            if (FileFormats.Formats.TryGetValue(type, out var format))
                r.AddSubRecord(name, description, required, format);
            else
                switch (type)
                {
                    case "list":
                    {
                        var subType = subRecord.Attribute("subType").Value;
                        if (FileFormats.Formats.TryGetValue(subType, out var subTypeFormat))
                        {
                            r.AddSubRecordList(name, description, subTypeFormat);
                        }
                        else if (subType == "struct")
                        {
                            var fields =
                                new List<(string name, string type, string description, bool primary, FileFormat format)
                                >();
                            foreach (var field in subRecord.Elements("Fields").First().Elements("Field"))
                            {
                                var fName = field.Attribute("name").Value;
                                var fType = field.Attribute("type").Value;
                                var fDescription = field.Attribute("description")?.Value;
                                bool.TryParse(field.Attribute("primary")?.Value, out var fPrimary);
                                fields.Add((fName, fType, fDescription, fPrimary, FileFormats.Formats[fType]));
                            }

                            r.AddSubRecordListStruct(name, description, fields);
                        }

                        break;
                    }
                    case "struct":
                    {
                        var fields =
                            new List<(string name, string type, string description, bool primary, FileFormat format)>();
                        foreach (var field in subRecord.Elements("Fields").First().Elements("Field"))
                        {
                            var fName = field.Attribute("name").Value;
                            var fType = field.Attribute("type").Value;
                            var fDescription = field.Attribute("description")?.Value;
                            bool.TryParse(field.Attribute("primary")?.Value, out var fPrimary);
                            fields.Add((fName, fType, fDescription, fPrimary, FileFormats.Formats[fType]));
                        }

                        r.AddSubRecordStruct(name, description, required, fields);
                        break;
                    }
                }
        }

        private static void ParseRecord([NotNull] XElement record)
        {
            var type = record.Attribute("type").Value;
            var description = record.Attribute("description").Value;

            var c = RecordBuilder.Begin(type, description);

            foreach (var field in record.Elements("SubRecords").First().Elements("SubRecord")) ParseSubRecord(field, c);

            c.End();
        }

        private static void GenerateRegistry()
        {
            var bse = new Base
            {
                Comment =
                    "ReSharper disable RedundantUsingDirective\nReSharper disable InconsistentNaming\nReSharper disable UseObjectOrCollectionInitializer\nReSharper disable RedundantCast"
            };
            bse.Using.Add("System");
            bse.Using.Add("System.Collections.Generic");

            var ns = new Namespace("SkyrimLib");
            bse.Namespaces.Add(ns);

            var c = new Class("Registry");
            ns.Types.Add(c);
            c.Modifiers = Modifiers.Internal | Modifiers.Static;
            var f1 = new Field("IReadOnlyDictionary<Signature, Func<IReader, IReader, Record>>", "ParsedRecords")
            {
                Modifiers = Modifiers.Internal | Modifiers.Static | Modifiers.ReadOnly
            };
            c.Members.Add(f1);
            var f2 = new Field(
                "IReadOnlyDictionary<(Signature record, Signature subrecord), Func<IReader, IReader, uint, SubRecord>>",
                "ParsedSubRecords") {Modifiers = Modifiers.Internal | Modifiers.Static | Modifiers.ReadOnly};
            c.Members.Add(f2);

            var m = new Method(null, "Registry") {Modifiers = Modifiers.Static, Body = new Builder()};
            m.Body.AppendLine("ParsedRecords = new Dictionary<Signature, Func<IReader, IReader, Record>> {");
            m.Body.EnterBlock();
            foreach (var r in Records)
                m.Body.AppendLine(
                    $"{{{r.Title()}.FieldType, (headerReader, dataReader) => new {r.Title()}(headerReader, dataReader)}},");
            m.Body.LeaveBlock();
            m.Body.AppendLine("};");
            m.Body.AppendLine(
                "ParsedSubRecords = new Dictionary<(Signature record, Signature subrecord), Func<IReader, IReader, uint, SubRecord>> {");
            m.Body.EnterBlock();
            foreach (var sr in SubRecords)
                m.Body.AppendLine(
                    $"{{({sr.main}.FieldType, {sr.sub}.FieldType), (headerReader, dataReader, size) => new {sr.sub}(headerReader, dataReader, size)}},");
            m.Body.LeaveBlock();
            m.Body.AppendLine("};");
            c.Members.Add(m);

            File.WriteAllText("../../../../SkyrimLib/Generated/Registry.cs", bse.BuildCode(true));
        }

        private static void Main(string[] args)
        {
            FileFormats.Parse("resources/FileFormats.xml");
            var doc = XDocument.Load("resources/SSE_Records.xml");

            if (doc.Root != null)
                foreach (var record in doc.Root.Elements("Record"))
                    ParseRecord(record);

            GenerateRegistry();
        }
    }
}