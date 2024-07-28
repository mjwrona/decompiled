// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.KeyValueSyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class KeyValueSyntax : SyntaxNode
  {
    private KeySyntax? _key;
    private SyntaxToken? _equalToken;
    private ValueSyntax? _value;
    private SyntaxToken? _endOfLineToken;

    public KeyValueSyntax()
      : base(SyntaxKind.KeyValue)
    {
    }

    public KeyValueSyntax(string key, ValueSyntax value)
      : this()
    {
      this.Key = key != null ? new KeySyntax(key) : throw new ArgumentNullException(nameof (key));
      this.Value = value ?? throw new ArgumentNullException(nameof (value));
      this.EqualToken = SyntaxFactory.Token(TokenKind.Equal);
      this.EqualToken.AddLeadingWhitespace<SyntaxToken>();
      this.EqualToken.AddTrailingWhitespace<SyntaxToken>();
      this.EndOfLineToken = SyntaxFactory.NewLine();
    }

    public KeyValueSyntax(KeySyntax key, ValueSyntax value)
      : this()
    {
      this.Key = key ?? throw new ArgumentNullException(nameof (key));
      this.Value = value ?? throw new ArgumentNullException(nameof (value));
      this.EqualToken = SyntaxFactory.Token(TokenKind.Equal);
      this.EqualToken.AddLeadingWhitespace<SyntaxToken>();
      this.EqualToken.AddTrailingWhitespace<SyntaxToken>();
      this.EndOfLineToken = SyntaxFactory.NewLine();
    }

    public KeySyntax? Key
    {
      get => this._key;
      set => this.ParentToThis<KeySyntax>(ref this._key, value);
    }

    public SyntaxToken? EqualToken
    {
      get => this._equalToken;
      set => this.ParentToThis<SyntaxToken>(ref this._equalToken, value, TokenKind.Equal);
    }

    public ValueSyntax? Value
    {
      get => this._value;
      set => this.ParentToThis<ValueSyntax>(ref this._value, value);
    }

    public SyntaxToken? EndOfLineToken
    {
      get => this._endOfLineToken;
      set => this.ParentToThis<SyntaxToken>(ref this._endOfLineToken, value, TokenKind.NewLine, TokenKind.Eof);
    }

    public override int ChildrenCount => 4;

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    protected override SyntaxNode? GetChildImpl(int index)
    {
      switch (index)
      {
        case 0:
          return (SyntaxNode) this.Key;
        case 1:
          return (SyntaxNode) this.EqualToken;
        case 2:
          return (SyntaxNode) this.Value;
        default:
          return (SyntaxNode) this.EndOfLineToken;
      }
    }

    protected override string ToDebuggerDisplay() => string.Format("{0}: {1} = {2}", (object) base.ToDebuggerDisplay(), (object) this.Key, (object) this.Value);
  }
}
