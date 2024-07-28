// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnectionSqlComponent6
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  internal class ExternalConnectionSqlComponent6 : ExternalConnectionSqlComponent5
  {
    protected override ExternalConnectionSqlComponent.ExternalConnectionDatasetBinder GetExternalConnectionDatasetBinder() => (ExternalConnectionSqlComponent.ExternalConnectionDatasetBinder) new ExternalConnectionSqlComponent6.ExternalConnectionDatasetBinder3();

    public override void UpdateConnectionMetadata(
      Guid projectId,
      Guid connectionId,
      string connectionMetadata)
    {
      this.PrepareStoredProcedure("prc_SaveExternalConnectionMetadata");
      this.BindProjectId(projectId);
      this.BindGuid("@connectionId", connectionId);
      this.BindString("@connectionMetadata", connectionMetadata, 2147483646, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    protected class ExternalConnectionDatasetBinder3 : 
      ExternalConnectionSqlComponent2.ExternalConnectionDatasetBinder2
    {
      protected SqlColumnBinder m_connectionMetadata = new SqlColumnBinder("ConnectionMetadata");

      public override ExternalConnectionDataset Bind(IDataReader reader)
      {
        ExternalConnectionDataset connectionDataset = base.Bind(reader);
        connectionDataset.ConnectionMetadata = this.m_connectionMetadata.GetString(reader, true);
        return connectionDataset;
      }
    }
  }
}
