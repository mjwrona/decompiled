// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Core.Mark
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;

namespace YamlDotNet.Core
{
  [Serializable]
  public class Mark : IEquatable<Mark>, IComparable<Mark>, IComparable
  {
    public static readonly Mark Empty = new Mark();

    public int Index { get; private set; }

    public int Line { get; private set; }

    public int Column { get; private set; }

    public Mark()
    {
      this.Line = 1;
      this.Column = 1;
    }

    public Mark(int index, int line, int column)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException(nameof (index), "Index must be greater than or equal to zero.");
      if (line < 1)
        throw new ArgumentOutOfRangeException(nameof (line), "Line must be greater than or equal to 1.");
      if (column < 1)
        throw new ArgumentOutOfRangeException(nameof (column), "Column must be greater than or equal to 1.");
      this.Index = index;
      this.Line = line;
      this.Column = column;
    }

    public override string ToString() => string.Format("Line: {0}, Col: {1}, Idx: {2}", (object) this.Line, (object) this.Column, (object) this.Index);

    public override bool Equals(object obj) => this.Equals(obj as Mark);

    public bool Equals(Mark other) => other != null && this.Index == other.Index && this.Line == other.Line && this.Column == other.Column;

    public override int GetHashCode()
    {
      int num = this.Index;
      int hashCode1 = num.GetHashCode();
      num = this.Line;
      int hashCode2 = num.GetHashCode();
      num = this.Column;
      int hashCode3 = num.GetHashCode();
      int h2 = HashCode.CombineHashCodes(hashCode2, hashCode3);
      return HashCode.CombineHashCodes(hashCode1, h2);
    }

    public int CompareTo(object obj) => obj != null ? this.CompareTo(obj as Mark) : throw new ArgumentNullException(nameof (obj));

    public int CompareTo(Mark other)
    {
      int num = other != null ? this.Line.CompareTo(other.Line) : throw new ArgumentNullException(nameof (other));
      if (num == 0)
        num = this.Column.CompareTo(other.Column);
      return num;
    }
  }
}
