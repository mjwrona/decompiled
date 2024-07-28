// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HttpStreams.ServerResponseUnderflowException
// Assembly: Microsoft.VisualStudio.Services.HttpStreams, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08EEF7AF-2ADD-4A01-B7DB-5972BBFA47F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.HttpStreams.dll

namespace Microsoft.VisualStudio.Services.HttpStreams
{
  public class ServerResponseUnderflowException : UnexpectedServerResponseException
  {
    public ServerResponseUnderflowException(int expectedLength, int actualLength)
      : this(Resources.Error_ServerSentTooFewBytes((object) expectedLength, (object) actualLength))
    {
      this.ExpectedLength = expectedLength;
      this.ActualLength = actualLength;
    }

    private ServerResponseUnderflowException(string message)
      : base(message)
    {
    }

    public int ExpectedLength { get; private set; }

    public int ActualLength { get; private set; }
  }
}
