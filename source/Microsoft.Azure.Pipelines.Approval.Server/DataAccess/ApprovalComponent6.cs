// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.DataAccess.ApprovalComponent6
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Approval.Server.DataAccess
{
  internal class ApprovalComponent6 : ApprovalComponent5
  {
    public override IList<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> QueryApprovals(
      Guid projectId,
      IList<Guid> approvalIds,
      IList<Guid> approverIds,
      ApprovalStatus approvalStatus,
      int rowsCount)
    {
      using (new ApprovalSqlComponentBase.SqlMethodScope((ApprovalSqlComponentBase) this, method: nameof (QueryApprovals)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_QueryApprovals_v2");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindGuidTable("@approvalIds", (IEnumerable<Guid>) approvalIds);
        this.BindGuidTable("@userIds", (IEnumerable<Guid>) approverIds);
        this.BindInt("@approvalStatus", (int) approvalStatus);
        this.BindInt("@rowsCount", rowsCount);
        return this.FetchApprovals();
      }
    }
  }
}
