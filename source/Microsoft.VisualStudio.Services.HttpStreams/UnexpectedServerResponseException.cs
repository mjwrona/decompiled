// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HttpStreams.UnexpectedServerResponseException
// Assembly: Microsoft.VisualStudio.Services.HttpStreams, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08EEF7AF-2ADD-4A01-B7DB-5972BBFA47F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.HttpStreams.dll

using System.IO;

namespace Microsoft.VisualStudio.Services.HttpStreams
{
  public class UnexpectedServerResponseException : IOException
  {
    public UnexpectedServerResponseException(string message)
      : base(message)
    {
    }
  }
}
