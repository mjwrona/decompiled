// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Approval.Server.IApprovalService
// Assembly: Microsoft.Azure.Pipelines.Approval.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03DD9218-2C79-49A0-9EA7-F497B1327A4F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Approval.Server.dll

using Microsoft.Azure.Pipelines.Approval.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Approval.Server
{
  [DefaultServiceImplementation(typeof (ApprovalService))]
  public interface IApprovalService : IVssFrameworkService
  {
    IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> CreateApprovals(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ApprovalRequest> createParameters);

    Microsoft.Azure.Pipelines.Approval.WebApi.Approval GetApproval(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid approvalId,
      ApprovalDetailsExpandParameter expand = ApprovalDetailsExpandParameter.None);

    IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> UpdateApprovals(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ApprovalUpdateParameters> updateParameters,
      Guid? userId = null);

    IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> QueryApprovals(
      IVssRequestContext requestContext,
      Guid projectId,
      ApprovalsQueryParameters queryParameters,
      ApprovalDetailsExpandParameter expand = ApprovalDetailsExpandParameter.None);

    IEnumerable<Microsoft.Azure.Pipelines.Approval.WebApi.Approval> QueryApprovalsExtended(
      IVssRequestContext requestContext,
      Guid projectId,
      ApprovalsQueryParameters queryParameters,
      ApprovalDetailsExpandParameter expand = ApprovalDetailsExpandParameter.None);

    void DeleteApprovals(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Guid> approvalIds);
  }
}
