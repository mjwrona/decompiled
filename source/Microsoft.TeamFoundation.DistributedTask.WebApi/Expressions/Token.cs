// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.Token
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  internal sealed class Token
  {
    public Token(TokenKind kind, char rawValue, int index, object parsedValue = null)
      : this(kind, rawValue.ToString(), index, parsedValue)
    {
    }

    public Token(TokenKind kind, string rawValue, int index, object parsedValue = null)
    {
      this.Kind = kind;
      this.RawValue = rawValue;
      this.Index = index;
      this.ParsedValue = parsedValue;
    }

    public TokenKind Kind { get; }

    public string RawValue { get; }

    public int Index { get; }

    public object ParsedValue { get; }
  }
}
