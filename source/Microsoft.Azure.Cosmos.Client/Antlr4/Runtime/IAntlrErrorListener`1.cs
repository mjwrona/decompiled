// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.IAntlrErrorListener`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.IO;

namespace Antlr4.Runtime
{
  internal interface IAntlrErrorListener<in TSymbol>
  {
    void SyntaxError(
      TextWriter output,
      IRecognizer recognizer,
      TSymbol offendingSymbol,
      int line,
      int charPositionInLine,
      string msg,
      RecognitionException e);
  }
}
