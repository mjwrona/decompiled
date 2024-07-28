// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.LookAheadBuffer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.IO;

namespace YamlDotNet.Core
{
  [Serializable]
  public class LookAheadBuffer : ILookAheadBuffer
  {
    private readonly TextReader input;
    private readonly char[] buffer;
    private int firstIndex;
    private int count;
    private bool endOfInput;

    public LookAheadBuffer(TextReader input, int capacity)
    {
      if (input == null)
        throw new ArgumentNullException(nameof (input));
      if (capacity < 1)
        throw new ArgumentOutOfRangeException(nameof (capacity), "The capacity must be positive.");
      this.input = input;
      this.buffer = new char[capacity];
    }

    public bool EndOfInput => this.endOfInput && this.count == 0;

    private int GetIndexForOffset(int offset)
    {
      int indexForOffset = this.firstIndex + offset;
      if (indexForOffset >= this.buffer.Length)
        indexForOffset -= this.buffer.Length;
      return indexForOffset;
    }

    public char Peek(int offset)
    {
      if (offset < 0 || offset >= this.buffer.Length)
        throw new ArgumentOutOfRangeException(nameof (offset), "The offset must be betwwen zero and the capacity of the buffer.");
      this.Cache(offset);
      return offset < this.count ? this.buffer[this.GetIndexForOffset(offset)] : char.MinValue;
    }

    public void Cache(int length)
    {
      for (; length >= this.count; ++this.count)
      {
        int num = this.input.Read();
        if (num >= 0)
        {
          this.buffer[this.GetIndexForOffset(this.count)] = (char) num;
        }
        else
        {
          this.endOfInput = true;
          break;
        }
      }
    }

    public void Skip(int length)
    {
      this.firstIndex = length >= 1 && length <= this.count ? this.GetIndexForOffset(length) : throw new ArgumentOutOfRangeException(nameof (length), "The length must be between 1 and the number of characters in the buffer. Use the Peek() and / or Cache() methods to fill the buffer.");
      this.count -= length;
    }
  }
}
