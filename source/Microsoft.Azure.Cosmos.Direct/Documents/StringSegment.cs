// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.StringSegment
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;

namespace Microsoft.Azure.Documents
{
  internal readonly struct StringSegment
  {
    private readonly string value;

    public StringSegment(string value)
    {
      this.value = value;
      this.Start = 0;
      this.Length = value != null ? value.Length : 0;
    }

    public StringSegment(string value, int start, int length)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      if (start < 0 || start >= value.Length && value.Length > 0)
        throw new ArgumentException(nameof (start));
      if (length < 0 || start + length > value.Length)
        throw new ArgumentException(nameof (length));
      this.value = value;
      this.Start = start;
      this.Length = length;
    }

    private int Start { get; }

    public int Length { get; }

    public static implicit operator StringSegment(string b) => new StringSegment(b);

    public bool IsNullOrEmpty() => string.IsNullOrEmpty(this.value) || this.Length == 0;

    public int Compare(string other, StringComparison comparison) => string.Compare(this.value, this.Start, other, 0, Math.Max(this.Length, other.Length), comparison);

    public int Compare(StringSegment other, StringComparison comparison) => string.Compare(this.value, this.Start, other.value, other.Start, Math.Max(this.Length, other.Length), comparison);

    public bool Equals(string other, StringComparison comparison) => this.Compare(other, comparison) == 0;

    public StringSegment Substring(int start, int length)
    {
      if (length == 0)
        return new StringSegment(string.Empty);
      if (start > this.Length)
        throw new ArgumentException(nameof (start));
      if (start + length > this.Length)
        throw new ArgumentException(nameof (length));
      return new StringSegment(this.value, start + this.Start, length);
    }

    public int LastIndexOf(char segment)
    {
      if (this.IsNullOrEmpty())
        return -1;
      int num = this.value.LastIndexOf(segment, this.Start + this.Length - 1);
      return num >= 0 ? num - this.Start : num;
    }

    public StringSegment Trim(char[] trimChars) => this.TrimStart(trimChars).TrimEnd(trimChars);

    public StringSegment TrimStart(char[] trimChars)
    {
      if (this.Length == 0)
        return new StringSegment(string.Empty, 0, 0);
      int start = this.Start;
      int length;
      for (length = this.Length; length > 0 && this.value.IndexOfAny(trimChars, start, 1) == start; --length)
        ++start;
      return new StringSegment(this.value, start, length);
    }

    public StringSegment TrimEnd(char[] trimChars)
    {
      if (this.Length == 0)
        return new StringSegment(string.Empty, 0, 0);
      int length = this.Length;
      for (int startIndex = this.Start + this.Length - 1; length > 0 && this.value.LastIndexOfAny(trimChars, startIndex, 1) == startIndex; --startIndex)
        --length;
      return new StringSegment(this.value, this.Start, length);
    }

    public string GetString()
    {
      if (this.Length == 0)
        return string.Empty;
      return this.Length == this.value.Length ? this.value : this.value.Substring(this.Start, this.Length);
    }
  }
}
