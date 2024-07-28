// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Tree.Pattern.RuleTagToken
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System;

namespace Antlr4.Runtime.Tree.Pattern
{
  internal class RuleTagToken : IToken
  {
    private readonly string ruleName;
    private readonly int bypassTokenType;
    private readonly string label;

    public RuleTagToken(string ruleName, int bypassTokenType)
      : this(ruleName, bypassTokenType, (string) null)
    {
    }

    public RuleTagToken(string ruleName, int bypassTokenType, string label)
    {
      this.ruleName = !string.IsNullOrEmpty(ruleName) ? ruleName : throw new ArgumentException("ruleName cannot be null or empty.");
      this.bypassTokenType = bypassTokenType;
      this.label = label;
    }

    [NotNull]
    public string RuleName => this.ruleName;

    [Nullable]
    public string Label => this.label;

    public virtual int Channel => 0;

    public virtual string Text
    {
      get
      {
        if (this.label == null)
          return "<" + this.ruleName + ">";
        return "<" + this.label + ":" + this.ruleName + ">";
      }
    }

    public virtual int Type => this.bypassTokenType;

    public virtual int Line => 0;

    public virtual int Column => -1;

    public virtual int TokenIndex => -1;

    public virtual int StartIndex => -1;

    public virtual int StopIndex => -1;

    public virtual ITokenSource TokenSource => (ITokenSource) null;

    public virtual ICharStream InputStream => (ICharStream) null;

    public override string ToString() => this.ruleName + ":" + this.bypassTokenType.ToString();
  }
}
