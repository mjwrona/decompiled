// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.ArrayItemSyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class ArrayItemSyntax : SyntaxNode
  {
    private ValueSyntax? _value;
    private SyntaxToken? _comma;

    public ArrayItemSyntax()
      : base(SyntaxKind.ArrayItem)
    {
    }

    public ValueSyntax? Value
    {
      get => this._value;
      set => this.ParentToThis<ValueSyntax>(ref this._value, value);
    }

    public SyntaxToken? Comma
    {
      get => this._comma;
      set => this.ParentToThis<SyntaxToken>(ref this._comma, value, TokenKind.Comma);
    }

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    public override int ChildrenCount => 2;

    protected override SyntaxNode? GetChildImpl(int index) => index != 0 ? (SyntaxNode) this.Comma : (SyntaxNode) this.Value;
  }
}
