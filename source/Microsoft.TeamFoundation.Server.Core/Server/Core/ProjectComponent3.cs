// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectComponent3
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectComponent3 : ProjectComponent2
  {
    internal override ProjectOperation ReserveProject(
      Guid pendingProjectGuid,
      string projectName,
      Guid userIdentity)
    {
      this.PrepareStoredProcedure("prc_css_reserve_project");
      this.BindGuid("@pending_project_id", pendingProjectGuid);
      this.BindProjectName("@project_name", projectName);
      this.ExecuteNonQuery();
      return (ProjectOperation) null;
    }

    internal override ProjectOperation DeleteReservedProject(
      Guid pendingProjectGuid,
      Guid userIdentity)
    {
      this.PrepareStoredProcedure("prc_css_delete_reserved_project");
      this.BindGuid("@pending_project_id", pendingProjectGuid);
      this.ExecuteNonQuery();
      return (ProjectOperation) null;
    }

    internal override ProjectInfo CreateProject(
      TeamFoundationIdentity requestingIdentity,
      Guid projectId,
      string projectName,
      Guid adminGroupId,
      string nodes,
      DateTime timeStamp,
      Guid pendingProjectGuid,
      out int nodeSeqId)
    {
      this.PrepareStoredProcedure("prc_css_create_project");
      this.BindGuid("@project_id", projectId);
      this.BindProjectName("@project_name", projectName);
      this.BindGuid("@admin_id", adminGroupId);
      this.BindString("@nodes", nodes, 0, false, SqlDbType.NVarChar);
      this.BindGuid("@user_id", requestingIdentity.TeamFoundationId);
      this.BindDateTime("@time_stamp", timeStamp);
      this.BindGuid("@pending_project_id", pendingProjectGuid);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SequenceIdColumns());
        nodeSeqId = resultCollection.GetCurrent<int>().Items.FirstOrDefault<int>();
      }
      return new ProjectInfo(projectId, projectName, ProjectState.New);
    }
  }
}
