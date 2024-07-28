// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.Controllers.ApprovalProjectApiController
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Approval.Server.Controllers
{
  public abstract class ApprovalProjectApiController : TfsProjectApiController
  {
    public override string TraceArea => "PipelinePolicy.Approval";

    public override string ActivityLogArea => "ApprovalService";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      ApprovalExceptionMapper.Map(exceptionMap);
    }

    protected IApprovalService ApprovalService => this.TfsRequestContext.GetService<IApprovalService>();

    [HttpGet]
    [ClientExample("GET__Approval.json", null, null, null)]
    public Microsoft.Azure.Pipelines.Approval.WebApi.Approval GetApproval(
      Guid approvalId,
      [FromUri(Name = "$expand")] ApprovalDetailsExpandParameter expand = ApprovalDetailsExpandParameter.None)
    {
      return this.ApprovalService.GetApproval(this.TfsRequestContext, this.ProjectId, approvalId, expand);
    }

    [HttpPatch]
    [ClientExample("PATCH__UpdateApproval.json", null, null, null)]
    public IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> UpdateApprovals(
      [FromBody] IEnumerable<ApprovalUpdateParameters> updateParameters)
    {
      return this.ApprovalService.UpdateApprovals(this.TfsRequestContext, this.ProjectId, updateParameters);
    }

    protected IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> QueryApprovalsExtended(
      string approvalIds,
      ApprovalDetailsExpandParameter expand,
      string assignedTo,
      ApprovalStatus state,
      int top)
    {
      List<Guid> guidList = new List<Guid>();
      if (!string.IsNullOrWhiteSpace(approvalIds))
      {
        string str = approvalIds;
        char[] chArray = new char[1]{ ',' };
        foreach (string input in str.Split(chArray))
        {
          Guid result;
          if (!Guid.TryParse(input, out result))
            throw new ArgumentException("approvalIds contains invalid approvalId: " + input, approvalIds);
          guidList.Add(result);
        }
      }
      List<string> stringList;
      if (!string.IsNullOrWhiteSpace(assignedTo))
        stringList = ((IEnumerable<string>) assignedTo.Split(',')).ToList<string>();
      else
        stringList = new List<string>();
      return this.ApprovalService.QueryApprovalsExtended(this.TfsRequestContext, this.ProjectId, new ApprovalsQueryParameters()
      {
        ApprovalIds = guidList,
        AssignedTo = stringList,
        ApproverStatus = state,
        Top = top
      }, expand);
    }
  }
}
