// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.SyntaxVisitor
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System.Collections.Generic;


#nullable enable
namespace Tomlyn.Syntax
{
  public abstract class SyntaxVisitor
  {
    public virtual void Visit(SyntaxList list) => this.DefaultVisit((SyntaxNode) list);

    public virtual void Visit(DocumentSyntax document) => this.DefaultVisit((SyntaxNode) document);

    public virtual void Visit(KeyValueSyntax keyValue) => this.DefaultVisit((SyntaxNode) keyValue);

    public virtual void Visit(StringValueSyntax stringValue) => this.DefaultVisit((SyntaxNode) stringValue);

    public virtual void Visit(IntegerValueSyntax integerValue) => this.DefaultVisit((SyntaxNode) integerValue);

    public virtual void Visit(BooleanValueSyntax boolValue) => this.DefaultVisit((SyntaxNode) boolValue);

    public virtual void Visit(FloatValueSyntax floatValue) => this.DefaultVisit((SyntaxNode) floatValue);

    public virtual void Visit(TableSyntax table) => this.DefaultVisit((SyntaxNode) table);

    public virtual void Visit(TableArraySyntax table) => this.DefaultVisit((SyntaxNode) table);

    public virtual void Visit(SyntaxToken token) => this.DefaultVisit((SyntaxNode) token);

    public virtual void Visit(SyntaxTrivia trivia)
    {
    }

    public virtual void Visit(BareKeySyntax bareKey) => this.DefaultVisit((SyntaxNode) bareKey);

    public virtual void Visit(KeySyntax key) => this.DefaultVisit((SyntaxNode) key);

    public virtual void Visit(DateTimeValueSyntax dateTime) => this.DefaultVisit((SyntaxNode) dateTime);

    public virtual void Visit(ArraySyntax array) => this.DefaultVisit((SyntaxNode) array);

    public virtual void Visit(InlineTableItemSyntax inlineTableItem) => this.DefaultVisit((SyntaxNode) inlineTableItem);

    public virtual void Visit(ArrayItemSyntax arrayItem) => this.DefaultVisit((SyntaxNode) arrayItem);

    public virtual void Visit(DottedKeyItemSyntax dottedKeyItem) => this.DefaultVisit((SyntaxNode) dottedKeyItem);

    public virtual void Visit(InlineTableSyntax inlineTable) => this.DefaultVisit((SyntaxNode) inlineTable);

    public virtual void DefaultVisit(SyntaxNode? node)
    {
      if (node == null)
        return;
      this.VisitTrivias(node.LeadingTrivia);
      for (int index = 0; index < node.ChildrenCount; ++index)
        node.GetChild(index)?.Accept(this);
      this.VisitTrivias(node.TrailingTrivia);
    }

    private void VisitTrivias(List<SyntaxTrivia>? trivias)
    {
      if (trivias == null)
        return;
      foreach (SyntaxTrivia trivia in trivias)
        trivia?.Accept(this);
    }
  }
}
