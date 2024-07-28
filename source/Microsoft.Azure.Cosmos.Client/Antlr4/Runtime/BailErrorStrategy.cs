// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.BailErrorStrategy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using System;

namespace Antlr4.Runtime
{
  internal class BailErrorStrategy : DefaultErrorStrategy
  {
    public override void Recover(Parser recognizer, RecognitionException e)
    {
      for (ParserRuleContext parserRuleContext = recognizer.Context; parserRuleContext != null; parserRuleContext = (ParserRuleContext) parserRuleContext.Parent)
        parserRuleContext.exception = e;
      throw new ParseCanceledException((Exception) e);
    }

    public override IToken RecoverInline(Parser recognizer)
    {
      InputMismatchException cause = new InputMismatchException(recognizer);
      for (ParserRuleContext parserRuleContext = recognizer.Context; parserRuleContext != null; parserRuleContext = (ParserRuleContext) parserRuleContext.Parent)
        parserRuleContext.exception = (RecognitionException) cause;
      throw new ParseCanceledException((Exception) cause);
    }

    public override void Sync(Parser recognizer)
    {
    }
  }
}
