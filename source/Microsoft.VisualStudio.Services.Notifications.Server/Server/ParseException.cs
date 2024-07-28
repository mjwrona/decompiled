// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ParseException
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [Serializable]
  public class ParseException : TeamFoundationServiceException, ISerializable
  {
    public ParseException() => this.EventId = TeamFoundationEventId.ParseErrorException;

    public ParseException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.EventId = TeamFoundationEventId.ParseErrorException;
    }

    protected ParseException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.EventId = TeamFoundationEventId.ParseErrorException;
    }

    public ParseException(string message)
      : base(message)
    {
      this.EventId = TeamFoundationEventId.ParseErrorException;
    }
  }
}
