// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseFullException
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
  public class DatabaseFullException : TeamFoundationServiceException
  {
    public DatabaseFullException()
      : this(FrameworkResources.DatabaseFullException())
    {
    }

    public DatabaseFullException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(FrameworkResources.DatabaseFullExceptionWithDetails((object) sqlError.Server, (object) sqlError.Number, (object) sqlError.Message))
    {
    }

    public DatabaseFullException(string msg)
      : base(msg)
    {
      this.LogException = true;
      this.EventId = TeamFoundationEventId.DatabaseFullException;
      this.ReportException = false;
      this.LogLevel = EventLogEntryType.Error;
    }

    protected DatabaseFullException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.EventId = TeamFoundationEventId.DatabaseFullException;
    }

    public string CleanMessage { get; private set; } = FrameworkResources.DatabaseFullException();
  }
}
