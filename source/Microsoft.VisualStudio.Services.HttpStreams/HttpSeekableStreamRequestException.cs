// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HttpStreams.HttpSeekableStreamRequestException
// Assembly: Microsoft.VisualStudio.Services.HttpStreams, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08EEF7AF-2ADD-4A01-B7DB-5972BBFA47F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.HttpStreams.dll

using System;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.HttpStreams
{
  [Serializable]
  public class HttpSeekableStreamRequestException : Exception
  {
    public HttpSeekableStreamRequestException()
    {
    }

    public HttpSeekableStreamRequestException(string message, HttpStatusCode statusCode)
      : base(message)
    {
      this.StatusCode = statusCode;
    }

    protected HttpSeekableStreamRequestException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public HttpStatusCode StatusCode { get; private set; }
  }
}
