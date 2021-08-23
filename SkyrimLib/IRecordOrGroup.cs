using System;

namespace SkyrimLib
{
    public interface IRecordOrGroup: IDisposable
    {
        Signature Type { get; }
        uint Size { get; }

        void Write(IWriter writer);
    }
}