// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.SyntaxNode
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections.Generic;
using System.IO;
using Tomlyn.Helpers;


#nullable enable
namespace Tomlyn.Syntax
{
  public abstract class SyntaxNode : SyntaxNodeBase
  {
    protected SyntaxNode(SyntaxKind kind) => this.Kind = kind;

    public SyntaxKind Kind { get; }

    public List<SyntaxTrivia>? LeadingTrivia { get; set; }

    public List<SyntaxTrivia>? TrailingTrivia { get; set; }

    public abstract int ChildrenCount { get; }

    public SyntaxNode? GetChild(int index)
    {
      if (index < 0)
        throw ThrowHelper.GetIndexNegativeArgumentOutOfRangeException();
      return index <= this.ChildrenCount ? this.GetChildImpl(index) : throw ThrowHelper.GetIndexArgumentOutOfRangeException(this.ChildrenCount);
    }

    protected abstract SyntaxNode? GetChildImpl(int index);

    public override string ToString()
    {
      StringWriter writer = new StringWriter();
      this.WriteTo((TextWriter) writer);
      return writer.ToString();
    }

    public void WriteTo(TextWriter writer)
    {
      if (writer == null)
        throw new ArgumentNullException(nameof (writer));
      this.WriteToInternal(writer);
    }

    private void WriteToInternal(TextWriter writer)
    {
      SyntaxNode.WriteTriviaTo(this.LeadingTrivia, writer);
      if (this is SyntaxToken syntaxToken)
      {
        writer.Write(syntaxToken.TokenKind.ToText() ?? syntaxToken.Text);
      }
      else
      {
        int childrenCount = this.ChildrenCount;
        for (int index = 0; index < childrenCount; ++index)
          this.GetChild(index)?.WriteToInternal(writer);
      }
      SyntaxNode.WriteTriviaTo(this.TrailingTrivia, writer);
    }

    private static void WriteTriviaTo(List<SyntaxTrivia>? trivias, TextWriter writer)
    {
      if (trivias == null)
        return;
      foreach (SyntaxTrivia trivia in trivias)
      {
        if (trivia != null)
          writer.Write(trivia.Text);
      }
    }

    protected void ParentToThis<TSyntaxNode>(ref TSyntaxNode? set, TSyntaxNode? node) where TSyntaxNode : SyntaxNode
    {
      if (node?.Parent != null)
        throw ThrowHelper.GetExpectingNoParentException();
      if ((object) set != null)
        set.Parent = (SyntaxNode) null;
      if ((object) node != null)
        node.Parent = this;
      set = node;
    }

    protected void ParentToThis<TSyntaxNode>(
      ref TSyntaxNode? set,
      TSyntaxNode? node,
      TokenKind expectedKind)
      where TSyntaxNode : SyntaxToken
    {
      ref TSyntaxNode local1 = ref set;
      TSyntaxNode node1 = node;
      // ISSUE: variable of a boxed type
      __Boxed<TSyntaxNode> local2 = (object) node;
      int num = local2 != null ? (local2.TokenKind == expectedKind ? 1 : 0) : 0;
      int expectedMessage = (int) expectedKind;
      this.ParentToThis<TSyntaxNode, TokenKind>(ref local1, node1, num != 0, (TokenKind) expectedMessage);
    }

    protected void ParentToThis<TSyntaxNode, TExpected>(
      ref TSyntaxNode? set,
      TSyntaxNode? node,
      bool expectedKindSuccess,
      TExpected expectedMessage)
      where TSyntaxNode : SyntaxToken
    {
      if ((object) node != null && !expectedKindSuccess)
        throw new InvalidOperationException(string.Format("Unexpected node kind `{0}` while expecting `{1}`", (object) node.TokenKind, (object) expectedMessage));
      this.ParentToThis<TSyntaxNode>(ref set, node);
    }

    protected void ParentToThis<TSyntaxNode>(
      ref TSyntaxNode? set,
      TSyntaxNode? node,
      TokenKind expectedKind1,
      TokenKind expectedKind2)
      where TSyntaxNode : SyntaxToken
    {
      ref TSyntaxNode local1 = ref set;
      TSyntaxNode node1 = node;
      int num;
      if ((object) node == null || node.TokenKind != expectedKind1)
      {
        // ISSUE: variable of a boxed type
        __Boxed<TSyntaxNode> local2 = (object) node;
        num = local2 != null ? (local2.TokenKind == expectedKind2 ? 1 : 0) : 0;
      }
      else
        num = 1;
      SyntaxNode.ExpectedTuple2<TokenKind, TokenKind> expectedMessage = new SyntaxNode.ExpectedTuple2<TokenKind, TokenKind>(expectedKind1, expectedKind2);
      this.ParentToThis<TSyntaxNode, SyntaxNode.ExpectedTuple2<TokenKind, TokenKind>>(ref local1, node1, num != 0, expectedMessage);
    }

    private readonly struct ExpectedTuple2<T1, T2>
    {
      public readonly T1 Value1;
      public readonly T2 Value2;

      public ExpectedTuple2(T1 value1, T2 value2)
      {
        this.Value1 = value1;
        this.Value2 = value2;
      }

      public override string ToString() => string.Format("`{0}` or `{1}`", (object) this.Value1, (object) this.Value2);
    }
  }
}
