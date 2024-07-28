// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.SyntaxFactory
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;


#nullable enable
namespace Tomlyn.Syntax
{
  public static class SyntaxFactory
  {
    public static SyntaxTrivia Whitespace() => new SyntaxTrivia(TokenKind.Whitespaces, " ");

    public static SyntaxTrivia NewLineTrivia() => new SyntaxTrivia(TokenKind.NewLine, "\n");

    public static SyntaxTrivia Comment(string comment) => comment != null ? new SyntaxTrivia(TokenKind.Comment, "# " + comment) : throw new ArgumentNullException(nameof (comment));

    public static SyntaxToken NewLine() => new SyntaxToken(TokenKind.NewLine, "\n");

    public static SyntaxToken Token(TokenKind kind)
    {
      string text = kind != TokenKind.NewLine && kind.IsToken() ? kind.ToText() : throw new ArgumentOutOfRangeException(string.Format("The token kind `{0}` is not supported for a plain token without a predefined value", (object) kind));
      return new SyntaxToken(kind, text);
    }
  }
}
