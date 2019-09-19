using System;
using System.Diagnostics;

namespace SkyrimLib
{
    public readonly struct ReadonlyArrayWrapper<T>
    {
        internal readonly T[] Array;
        private readonly int _offset;
        internal readonly bool Rented;
        public int Length { get; }

        public ReadOnlySpan<T> Span => this.Array.AsSpan(this._offset, this.Length);
        
        public ReadonlyArrayWrapper(T[] array, bool rented = false): this(array, 0, array.Length, rented)
        {
        }

        public ReadonlyArrayWrapper(T[] array, int offset, bool rented = false): this(array, offset, array.Length - offset, rented)
        {
        }

        public ReadonlyArrayWrapper(T[] array, int offset, int length, bool rented = false)
        {
            Debug.Assert(offset + length <= array.Length);
            this.Array = array;
            this._offset = offset;
            this.Length = length;
            this.Rented = rented;
        }

        public ReadonlyArrayWrapper(ReadonlyArrayWrapper<T> other): this(other.Array, other._offset, other.Length)
        {
        }
        
        public ReadonlyArrayWrapper(ReadonlyArrayWrapper<T> other, int offset): this(other.Array, other._offset + offset, other.Length - offset)
        {
        }

        public ReadonlyArrayWrapper(ReadonlyArrayWrapper<T> other, int offset, int length): this(other.Array, other._offset + offset, length)
        {
        }

        public ReadonlyArrayWrapper<T> Slice(int offset) => new ReadonlyArrayWrapper<T>(this.Array, this._offset + offset, this.Length - offset);
        public ReadonlyArrayWrapper<T> Slice(int offset, int length) => new ReadonlyArrayWrapper<T>(this.Array, this._offset + offset, length);

        public static implicit operator ReadOnlySpan<T>(ReadonlyArrayWrapper<T> other)
        {
            return other.Span;
        }
    }
}