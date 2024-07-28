// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingServiceException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public abstract class WorkItemTrackingServiceException : TeamFoundationServiceException
  {
    public WorkItemTrackingServiceException() => this.SetDefaultEventId();

    public WorkItemTrackingServiceException(string message)
      : base(message)
    {
      this.SetDefaultEventId();
    }

    public WorkItemTrackingServiceException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.SetDefaultEventId();
    }

    protected WorkItemTrackingServiceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.SetDefaultEventId();
    }

    public WorkItemTrackingServiceException(string message, int id)
      : base(message, id)
    {
      this.SetDefaultEventId();
    }

    public WorkItemTrackingServiceException(string message, int id, Exception innerException)
      : base(message, id, innerException)
    {
      this.SetDefaultEventId();
    }

    public WorkItemTrackingServiceException(string message, int id, bool log)
      : base(message, id, log)
    {
      this.SetDefaultEventId();
    }

    public WorkItemTrackingServiceException(
      string message,
      int id,
      bool log,
      Exception innerException)
      : base(message, id, log, innerException)
    {
      this.SetDefaultEventId();
    }

    protected virtual void SetEventId() => this.SetDefaultEventId();

    private void SetDefaultEventId() => this.EventId = TeamFoundationEventId.WitBaseEventId;
  }
}
