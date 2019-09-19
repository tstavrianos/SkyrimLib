using System;

namespace SkyrimLib
{
    public interface IRecordOrGroup: IDisposable
    {
        uint Type { get; }
        uint Size { get; }

        void Write(IWriter writer);
    }
}