// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AzureSessionTerminatedException
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
  public class AzureSessionTerminatedException : SqlAzureException
  {
    private SqlAzureSessionResource m_sessionResource;

    public AzureSessionTerminatedException(SqlException sqlException)
      : base(FrameworkResources.ErrorWhileProcessingResults(), sqlException)
    {
      this.LogException = true;
      this.ReportException = false;
      this.LogLevel = EventLogEntryType.Warning;
      this.EventId = TeamFoundationEventId.SqlAzureSessionTerminated;
    }

    public AzureSessionTerminatedException(
      IVssRequestContext requestContext,
      SqlException sqlException,
      SqlError sqlError)
      : this(sqlException)
    {
      switch (sqlError.Number)
      {
        case 40549:
          this.m_sessionResource = SqlAzureSessionResource.TimeLimit;
          break;
        case 40550:
          this.m_sessionResource = SqlAzureSessionResource.Locks;
          break;
        case 40551:
          this.m_sessionResource = SqlAzureSessionResource.TempDB;
          break;
        case 40552:
          this.m_sessionResource = SqlAzureSessionResource.Logspace;
          break;
        case 40553:
          this.m_sessionResource = SqlAzureSessionResource.Memory;
          break;
      }
    }

    protected AzureSessionTerminatedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.m_sessionResource = (SqlAzureSessionResource) info.GetValue(nameof (Resource), typeof (SqlAzureSessionResource));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("Resource", (object) this.m_sessionResource, typeof (SqlAzureSessionResource));
    }

    public SqlAzureSessionResource Resource => this.m_sessionResource;
  }
}
