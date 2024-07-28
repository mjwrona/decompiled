// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RequestCanceledException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class RequestCanceledException : TeamFoundationServiceException
  {
    public RequestCanceledException()
      : base(FrameworkResources.RequestCanceledError())
    {
      this.LogException = true;
      this.EventId = TeamFoundationEventId.RequestCanceled;
      this.LogLevel = EventLogEntryType.Warning;
    }

    public RequestCanceledException(string message)
      : base(message)
    {
    }

    public RequestCanceledException(string message, HttpStatusCode httpStatusCode)
      : base(message, httpStatusCode)
    {
    }

    public RequestCanceledException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public RequestCanceledException(
      string message,
      HttpStatusCode httpStatusCode,
      Exception innerException)
      : base(message, httpStatusCode, innerException)
    {
    }

    protected RequestCanceledException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
