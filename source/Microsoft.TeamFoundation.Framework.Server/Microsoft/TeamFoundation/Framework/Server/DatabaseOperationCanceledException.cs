// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseOperationCanceledException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class DatabaseOperationCanceledException : TeamFoundationServiceException
  {
    public DatabaseOperationCanceledException(SqlException innerException)
      : base(FrameworkResources.DatabaseOperationCanceledException(), (Exception) innerException)
    {
      this.LogException = false;
      this.LogLevel = EventLogEntryType.Information;
      this.EventId = TeamFoundationEventId.ProcessTerminatedByUserAction;
    }

    protected DatabaseOperationCanceledException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.EventId = TeamFoundationEventId.ProcessTerminatedByUserAction;
    }
  }
}
