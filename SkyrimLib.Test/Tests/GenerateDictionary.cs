using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SkyrimLib.Test.Tests
{
    public static class GenerateDictionary
    {
        private sealed class IndentedStringBuilder
        {
            private readonly StringBuilder _stringBuilder;
            private string _currentIndentation;

            public IndentedStringBuilder() => this._stringBuilder = new StringBuilder();

            public void Indent() => this._currentIndentation += "    ";

            public void Outdent()
            {
                if (this._currentIndentation.Length > 0) this._currentIndentation = this._currentIndentation.Substring(4);
            }

            public void AppendLine(string value) => this._stringBuilder.AppendLine(this._currentIndentation + value);

            public void AppendLine(string format, params object[] args) => this._stringBuilder.AppendLine(this._currentIndentation + string.Format(format, args));

            public void AppendLine() => this._stringBuilder.AppendLine();

            public override string ToString() => this._stringBuilder.ToString();

            public void Clear()
            {
                this._currentIndentation = string.Empty;
                this._stringBuilder.Clear();
            }
        }

        public class Node
        {
            public string Name { get; }
            public HashSet<string> Children { get; }

            public Node(string name, IEnumerable<string> children)
            {
                this.Name = name;
                this.Children = new HashSet<string>(children);
            }
        }
        
        private static unsafe string TypeToString(uint type)
        {
            var t = &type;
            return Encoding.GetEncoding(1252).GetString((byte*)t, 4);
        }
        
        private static readonly HashSet<string> Records = new HashSet<string>();

        private static void ProcessRecord(IndentedStringBuilder sb, Record record, IRecordOrGroup parent)
        {
            var name = parent == null ? TypeToString(record.Type) : $"{TypeToString(parent.Type)}_{TypeToString(record.Type)}";
            if (Records.Contains(name)) return;
            Records.Add(name);
            sb.AppendLine($"var {name.ToLower()} = new Node(\"{name.ToUpper()}\", new []{{");
            sb.Indent();
            var fields = new HashSet<string>();
            foreach (var child in record.Fields)
            {
                var cName = TypeToString(child.Type).ToUpper();
                if(fields.Contains(cName)) continue;
                fields.Add(cName);
                sb.AppendLine($"\"{cName}\",");
            }
            sb.Outdent();
            sb.AppendLine("});");
        }

        private static void Process(IndentedStringBuilder sb, IRecordOrGroup child, IRecordOrGroup parent)
        {
            switch (child)
            {
                case Record r:
                    ProcessRecord(sb, r, parent);
                    break;
                case Group g:
                {
                    foreach (var c in g.Children)
                    {
                        Process(sb, c, child);
                    }

                    break;
                }
            }
        }
        
        public static void Run(ModFile m, TextWriter writer)
        {
            var sb = new IndentedStringBuilder();
            foreach (var child in m.Children)
            {
                Process(sb, child, null);
            }
            writer.Write(sb);
        }
    }
}