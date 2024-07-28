// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.NoViableAltException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using System;

namespace Antlr4.Runtime
{
  [Serializable]
  internal class NoViableAltException : RecognitionException
  {
    private const long serialVersionUID = 5096000008992867052;
    [Nullable]
    private readonly ATNConfigSet deadEndConfigs;
    [NotNull]
    private readonly IToken startToken;

    public NoViableAltException(Parser recognizer)
      : this((IRecognizer) recognizer, (ITokenStream) recognizer.InputStream, recognizer.CurrentToken, recognizer.CurrentToken, (ATNConfigSet) null, recognizer.RuleContext)
    {
    }

    public NoViableAltException(
      IRecognizer recognizer,
      ITokenStream input,
      IToken startToken,
      IToken offendingToken,
      ATNConfigSet deadEndConfigs,
      ParserRuleContext ctx)
      : base(recognizer, (IIntStream) input, ctx)
    {
      this.deadEndConfigs = deadEndConfigs;
      this.startToken = startToken;
      this.OffendingToken = offendingToken;
    }

    public virtual IToken StartToken => this.startToken;

    [Nullable]
    public virtual ATNConfigSet DeadEndConfigs => this.deadEndConfigs;
  }
}
