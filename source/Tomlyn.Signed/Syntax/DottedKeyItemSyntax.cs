// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.DottedKeyItemSyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class DottedKeyItemSyntax : ValueSyntax
  {
    private SyntaxToken? _dot;
    private BareKeyOrStringValueSyntax? _key;

    public DottedKeyItemSyntax()
      : base(SyntaxKind.DottedKeyItem)
    {
    }

    public DottedKeyItemSyntax(string key)
      : this()
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      this.Dot = SyntaxFactory.Token(TokenKind.Dot);
      this.Key = BareKeySyntax.IsBareKey(key) ? (BareKeyOrStringValueSyntax) new BareKeySyntax(key) : (BareKeyOrStringValueSyntax) new StringValueSyntax(key);
    }

    public SyntaxToken? Dot
    {
      get => this._dot;
      set => this.ParentToThis<SyntaxToken>(ref this._dot, value, TokenKind.Dot);
    }

    public BareKeyOrStringValueSyntax? Key
    {
      get => this._key;
      set => this.ParentToThis<BareKeyOrStringValueSyntax>(ref this._key, value);
    }

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    public override int ChildrenCount => 2;

    protected override SyntaxNode? GetChildImpl(int index) => index != 0 ? (SyntaxNode) this.Key : (SyntaxNode) this.Dot;

    protected override string ToDebuggerDisplay() => base.ToDebuggerDisplay() + ": " + this.ToString();
  }
}
