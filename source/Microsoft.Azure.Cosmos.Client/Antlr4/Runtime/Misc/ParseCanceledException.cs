// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Misc.ParseCanceledException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Antlr4.Runtime.Misc
{
  [Serializable]
  internal class ParseCanceledException : OperationCanceledException
  {
    public ParseCanceledException()
    {
    }

    public ParseCanceledException(string message)
      : base(message)
    {
    }

    public ParseCanceledException(Exception cause)
      : base("The parse operation was cancelled.", cause)
    {
    }

    public ParseCanceledException(string message, Exception cause)
      : base(message, cause)
    {
    }
  }
}
