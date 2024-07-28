// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.TextPosition
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;


#nullable enable
namespace Tomlyn.Syntax
{
  public struct TextPosition : IEquatable<TextPosition>
  {
    public static readonly TextPosition Eof = new TextPosition(-1, -1, -1);

    public TextPosition(int offset, int line, int column)
    {
      this.Offset = offset;
      this.Column = column;
      this.Line = line;
    }

    public int Offset { get; set; }

    public int Column { get; set; }

    public int Line { get; set; }

    public override string ToString() => string.Format("({0},{1})", (object) (this.Line + 1), (object) (this.Column + 1));

    public bool Equals(TextPosition other) => this.Offset == other.Offset && this.Column == other.Column && this.Line == other.Line;

    public override bool Equals(object? obj) => obj != null && obj is TextPosition other && this.Equals(other);

    public override int GetHashCode() => this.Offset;

    public static bool operator ==(TextPosition left, TextPosition right) => left.Equals(right);

    public static bool operator !=(TextPosition left, TextPosition right) => !left.Equals(right);
  }
}
