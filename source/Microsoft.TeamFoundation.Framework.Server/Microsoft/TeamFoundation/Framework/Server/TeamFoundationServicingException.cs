// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationServicingException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class TeamFoundationServicingException : TeamFoundationServiceException
  {
    public TeamFoundationServicingException()
    {
      this.LogException = true;
      this.LogLevel = EventLogEntryType.Error;
      this.EventId = TeamFoundationEventId.ServicingError;
    }

    public TeamFoundationServicingException(string message)
      : base(message)
    {
      this.LogException = true;
      this.LogLevel = EventLogEntryType.Error;
      this.EventId = TeamFoundationEventId.ServicingError;
    }

    public TeamFoundationServicingException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.LogException = true;
      this.LogLevel = EventLogEntryType.Error;
      this.EventId = TeamFoundationEventId.ServicingError;
    }

    protected TeamFoundationServicingException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
