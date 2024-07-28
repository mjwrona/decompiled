// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.InputMismatchException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Antlr4.Runtime
{
  [Serializable]
  internal class InputMismatchException : RecognitionException
  {
    private const long serialVersionUID = 1532568338707443067;

    public InputMismatchException(Parser recognizer)
      : base((IRecognizer) recognizer, recognizer.InputStream, recognizer.RuleContext)
    {
      this.OffendingToken = recognizer.CurrentToken;
    }
  }
}
