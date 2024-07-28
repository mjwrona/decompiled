// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.Controllers.ApprovalController
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Approval.Server.Controllers
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "approval", ResourceName = "approvals", ResourceVersion = 1)]
  public class ApprovalController : ApprovalProjectApiController
  {
    [HttpGet]
    [ActionName("Query")]
    [ClientExample("GET__QueryApproval.json", null, null, null)]
    public IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> QueryApprovals(
      [ClientParameterAsIEnumerable(typeof (Guid), ',')] string approvalIds = null,
      [FromUri(Name = "$expand")] ApprovalDetailsExpandParameter expand = ApprovalDetailsExpandParameter.None,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string userIds = null,
      ApprovalStatus state = ApprovalStatus.All,
      int top = 250)
    {
      return this.QueryApprovalsExtended(approvalIds, expand, userIds, state, top);
    }
  }
}
