using System;
using System.Diagnostics;

namespace SkyrimLib
{
    public readonly struct ReadonlyArrayWrapper<T>
    {
        private readonly T[] _array;
        private readonly int _offset;
        internal int Length { get; }

        internal ReadOnlySpan<T> Span => this._array.AsSpan(this._offset, this.Length);
        
        public ReadonlyArrayWrapper(T[] array): this(array, 0, array.Length)
        {
        }

        public ReadonlyArrayWrapper(T[] array, int offset): this(array, offset, array.Length - offset)
        {
        }

        internal ReadonlyArrayWrapper(T[] array, int offset, int length)
        {
            Debug.Assert(offset + length <= array.Length);
            this._array = array;
            this._offset = offset;
            this.Length = length;
        }

        public ReadonlyArrayWrapper(ReadonlyArrayWrapper<T> other): this(other._array, other._offset, other.Length)
        {
        }
        
        public ReadonlyArrayWrapper(ReadonlyArrayWrapper<T> other, int offset): this(other._array, other._offset + offset, other.Length - offset)
        {
        }

        internal ReadonlyArrayWrapper(ReadonlyArrayWrapper<T> other, int offset, int length): this(other._array, other._offset + offset, length)
        {
        }

        public ReadonlyArrayWrapper<T> Slice(int offset) => new ReadonlyArrayWrapper<T>(this._array, this._offset + offset, this.Length - offset);
        public ReadonlyArrayWrapper<T> Slice(int offset, int length) => new ReadonlyArrayWrapper<T>(this._array, this._offset + offset, length);

        public static implicit operator ReadOnlySpan<T>(ReadonlyArrayWrapper<T> other)
        {
            return other.Span;
        }
    }
}