// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.Pattern.TokenTagToken
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System;

namespace Antlr4.Runtime.Tree.Pattern
{
  [Serializable]
  internal class TokenTagToken : CommonToken
  {
    [NotNull]
    private readonly string tokenName;
    [Nullable]
    private readonly string label;

    public TokenTagToken(string tokenName, int type)
      : this(tokenName, type, (string) null)
    {
    }

    public TokenTagToken(string tokenName, int type, string label)
      : base(type)
    {
      this.tokenName = tokenName;
      this.label = label;
    }

    [NotNull]
    public string TokenName => this.tokenName;

    [Nullable]
    public string Label => this.label;

    public override string Text
    {
      get
      {
        if (this.label == null)
          return "<" + this.tokenName + ">";
        return "<" + this.label + ":" + this.tokenName + ">";
      }
    }

    public override string ToString() => this.tokenName + ":" + this.Type.ToString();
  }
}
