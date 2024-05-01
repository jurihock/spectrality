using System;

namespace Spectrality.Models;

public sealed class Deque<T>
{
  public int Size { get; private init; }
  public int Capacity { get; private init; }
  public int Cursor { get; private set; }
  public T[] Data { get; private init; }

  public T this[int offset] => Data[Cursor + offset];

  public Deque(int size)
  {
    Size = size;
    Capacity = size * 2;
    Cursor = 0;
    Data = new T[Capacity];
  }

  public void PopFrontPushBack(T value)
  {
    Cursor++;

    if (Cursor >= Size)
    {
      Array.Copy(Data, Size, Data, 0, Size);
      Cursor = 0;
    }

    Data[Cursor + Size] = value;
  }
}
