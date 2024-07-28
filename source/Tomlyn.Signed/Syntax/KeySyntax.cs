// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.KeySyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class KeySyntax : ValueSyntax
  {
    private BareKeyOrStringValueSyntax? _key;

    public KeySyntax()
      : base(SyntaxKind.Key)
    {
      SyntaxList<DottedKeyItemSyntax> syntaxList = new SyntaxList<DottedKeyItemSyntax>();
      syntaxList.Parent = (SyntaxNode) this;
      this.DotKeys = syntaxList;
    }

    public KeySyntax(string key)
      : this()
    {
      this.Key = BareKeySyntax.IsBareKey(key) ? (BareKeyOrStringValueSyntax) new BareKeySyntax(key) : (BareKeyOrStringValueSyntax) new StringValueSyntax(key);
    }

    public KeySyntax(string key, string dotKey1)
      : this()
    {
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      if (dotKey1 == null)
        throw new ArgumentNullException(nameof (dotKey1));
      this.Key = BareKeySyntax.IsBareKey(key) ? (BareKeyOrStringValueSyntax) new BareKeySyntax(key) : (BareKeyOrStringValueSyntax) new StringValueSyntax(key);
      this.DotKeys.Add(new DottedKeyItemSyntax(dotKey1));
    }

    public BareKeyOrStringValueSyntax? Key
    {
      get => this._key;
      set => this.ParentToThis<BareKeyOrStringValueSyntax>(ref this._key, value);
    }

    public SyntaxList<DottedKeyItemSyntax> DotKeys { get; }

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    public override int ChildrenCount => 2;

    protected override SyntaxNode? GetChildImpl(int index) => index == 0 ? (SyntaxNode) this.Key : (SyntaxNode) this.DotKeys;

    protected override string ToDebuggerDisplay() => base.ToDebuggerDisplay() + ": " + this.ToString();
  }
}
