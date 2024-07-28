// Decompiled with JetBrains decompiler
// Type: Pegasus.Common.ParseResult`1
// Assembly: Pegasus.Common, Version=4.0.14.0, Culture=neutral, PublicKeyToken=28c69b6c6d100f4a
// MVID: 081C50C0-D236-41F7-86F9-E3F2168B7118
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Pegasus.Common.dll

using System;

namespace Pegasus.Common
{
  public class ParseResult<T> : IParseResult<T>, IEquatable<ParseResult<T>>
  {
    public ParseResult(Cursor startCursor, Cursor endCursor, T value)
    {
      this.StartCursor = startCursor;
      this.EndCursor = endCursor;
      this.Value = value;
    }

    public Cursor EndCursor { get; }

    public Cursor StartCursor { get; }

    public T Value { get; }

    public static bool operator !=(ParseResult<T> left, ParseResult<T> right) => !object.Equals((object) left, (object) right);

    public static bool operator ==(ParseResult<T> left, ParseResult<T> right) => object.Equals((object) left, (object) right);

    public override bool Equals(object obj) => this.Equals(obj as ParseResult<T>);

    public bool Equals(ParseResult<T> other) => (object) other != null && this.StartCursor == other.StartCursor && this.EndCursor == other.EndCursor && object.Equals((object) this.Value, (object) other.Value);

    public override int GetHashCode() => ((1374496523 * -626349353 + this.StartCursor.GetHashCode()) * -626349353 + this.EndCursor.GetHashCode()) * -626349353 + ((object) this.Value == null ? 0 : this.Value.GetHashCode());
  }
}
