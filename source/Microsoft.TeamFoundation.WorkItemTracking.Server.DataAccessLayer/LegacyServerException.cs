// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.LegacyServerException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class LegacyServerException : TeamFoundationServiceException
  {
    public LegacyServerException() => this.SetDefaultEventId();

    public LegacyServerException(string message)
      : base(message)
    {
      this.SetDefaultEventId();
    }

    public LegacyServerException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.SetDefaultEventId();
    }

    protected LegacyServerException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.SetDefaultEventId();
    }

    public LegacyServerException(string message, int id)
      : base(message, id)
    {
      this.SetDefaultEventId();
    }

    public LegacyServerException(string message, int id, Exception innerException)
      : base(message, id, innerException)
    {
      this.SetDefaultEventId();
    }

    public LegacyServerException(string message, int id, bool log)
      : base(message, id, log)
    {
      this.SetDefaultEventId();
    }

    public LegacyServerException(string message, int id, bool log, Exception innerException)
      : base(message, id, log, innerException)
    {
      this.SetDefaultEventId();
    }

    public int ErrorId => this.ErrorCode;

    protected virtual void SetEventId() => this.SetDefaultEventId();

    private void SetDefaultEventId() => this.EventId = TeamFoundationEventId.WitBaseEventId;
  }
}
