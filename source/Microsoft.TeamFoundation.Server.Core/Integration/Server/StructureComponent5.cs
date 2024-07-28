// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.StructureComponent5
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal class StructureComponent5 : StructureComponent4
  {
    internal override void CreateInitialNodes(
      TeamFoundationIdentity identity,
      Guid projectId,
      string nodes,
      DateTime timeStamp,
      out int nodeSeqId)
    {
      this.PrepareStoredProcedure("prc_css_create_initial_nodes");
      this.BindGuid("@project_id", projectId);
      this.BindString("@nodes", nodes, 0, false, SqlDbType.NVarChar);
      this.BindGuid("@user_id", identity.TeamFoundationId);
      this.BindDateTime("@time_stamp", timeStamp);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new Microsoft.TeamFoundation.Server.Core.SequenceIdColumns());
        nodeSeqId = resultCollection.GetCurrent<int>().Items.FirstOrDefault<int>();
      }
    }

    internal override void DeleteNodes(
      Guid projectId,
      string userName,
      DateTime timeStamp,
      out int nodeSeqId)
    {
      this.PrepareStoredProcedure("prc_css_delete_nodes");
      this.BindGuid("@project_id", projectId);
      this.BindString("@user_name", userName, 256, false, SqlDbType.NVarChar);
      this.BindDateTime("@time_stamp", timeStamp);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new Microsoft.TeamFoundation.Server.Core.SequenceIdColumns());
        nodeSeqId = resultCollection.GetCurrent<int>().Items.FirstOrDefault<int>();
      }
    }
  }
}
