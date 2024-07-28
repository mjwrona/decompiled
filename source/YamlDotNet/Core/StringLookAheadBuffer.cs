// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.StringLookAheadBuffer
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;

namespace YamlDotNet.Core
{
  [Serializable]
  internal class StringLookAheadBuffer : ILookAheadBuffer
  {
    private readonly string value;

    public int Position { get; private set; }

    public StringLookAheadBuffer(string value) => this.value = value;

    public int Length => this.value.Length;

    public bool EndOfInput => this.IsOutside(this.Position);

    public char Peek(int offset)
    {
      int index = this.Position + offset;
      return !this.IsOutside(index) ? this.value[index] : char.MinValue;
    }

    private bool IsOutside(int index) => index >= this.value.Length;

    public void Skip(int length)
    {
      if (length < 0)
        throw new ArgumentOutOfRangeException(nameof (length), "The length must be positive.");
      this.Position += length;
    }
  }
}
