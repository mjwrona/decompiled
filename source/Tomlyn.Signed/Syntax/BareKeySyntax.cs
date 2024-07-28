// Decompiled with JetBrains decompiler
// Type: Tomlyn.Syntax.BareKeySyntax
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using Tomlyn.Helpers;
using Tomlyn.Model;
using Tomlyn.Text;


#nullable enable
namespace Tomlyn.Syntax
{
  public sealed class BareKeySyntax : BareKeyOrStringValueSyntax
  {
    private SyntaxToken? _key;

    public BareKeySyntax()
      : base(SyntaxKind.BasicKey)
    {
    }

    public BareKeySyntax(string name)
      : this()
    {
      this.Key = BareKeySyntax.IsBareKey(name) ? new SyntaxToken(TokenKind.BasicKey, name) : throw new ArgumentOutOfRangeException("The key `" + name + "` does not contain valid characters [A-Za-z0-9_\\-]");
    }

    public SyntaxToken? Key
    {
      get => this._key;
      set => this.ParentToThis<SyntaxToken>(ref this._key, value, TokenKind.BasicKey);
    }

    public override int ChildrenCount => 1;

    public override void Accept(SyntaxVisitor visitor) => visitor.Visit(this);

    protected override SyntaxNode? GetChildImpl(int index) => (SyntaxNode) this.Key;

    public static bool IsBareKey(string name)
    {
      switch (name)
      {
        case null:
          throw new ArgumentNullException(nameof (name));
        case "":
          return false;
        default:
          if (!string.IsNullOrWhiteSpace(name))
          {
            foreach (int c in name)
            {
              if (!CharHelper.IsKeyContinue((char32) c))
                return false;
            }
            return true;
          }
          goto case "";
      }
    }

    protected override string ToDebuggerDisplay() => base.ToDebuggerDisplay() + ": " + (this.Key != null ? TomlFormatHelper.ToString(this.Key.ToString(), TomlPropertyDisplayKind.Default) : string.Empty);
  }
}
