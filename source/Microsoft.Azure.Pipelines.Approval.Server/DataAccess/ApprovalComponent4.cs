// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.DataAccess.ApprovalComponent4
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Approval.Server.DataAccess
{
  internal class ApprovalComponent4 : ApprovalComponent3
  {
    public override void DeleteApprovals(Guid projectId, IList<Guid> approvalIds)
    {
      List<Guid> list = approvalIds.Distinct<Guid>().ToList<Guid>();
      using (new ApprovalSqlComponentBase.SqlMethodScope((ApprovalSqlComponentBase) this, method: nameof (DeleteApprovals)))
      {
        this.PrepareStoredProcedure("PipelinePolicy.prc_DeleteApprovals");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "PipelinePolicy", true));
        this.BindGuidTable("@approvalIds", (IEnumerable<Guid>) list);
        this.ExecuteNonQuery();
      }
    }
  }
}
