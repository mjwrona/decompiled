// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmApprovalsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "approvals")]
  public class RmApprovalsController : ReleaseManagementProjectControllerBase
  {
    protected const int DefaultTop = 50;
    protected const int MaxAllowedTop = 100;

    [HttpPatch]
    [ClientLocationId("9328E074-59FB-465A-89D9-B09C82EE5109")]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    [ClientExample("PATCH__UpdateStatusOfAnApprovalFromPendingToApproved.json", "Approve a release", null, null)]
    public virtual ReleaseApproval UpdateReleaseApproval(int approvalId, [FromBody] ReleaseApproval approval) => this.UpdateApproval(approvalId, approval, this.ProjectId);

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [HttpGet]
    [ClientLocationId("B47C6458-E73B-47CB-A770-4DF1E8813A91")]
    [ClientResponseType(typeof (IPagedList<ReleaseApproval>), null, null)]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public virtual HttpResponseMessage GetApprovals(
      string assignedToFilter = null,
      ApprovalStatus statusFilter = ApprovalStatus.Pending,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string releaseIdsFilter = null,
      ApprovalType typeFilter = ApprovalType.Undefined,
      int top = 50,
      int continuationToken = 0,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder queryOrder = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder.Descending,
      bool includeMyGroupApprovals = false)
    {
      top = top > 100 || top < 0 ? 100 : top;
      List<ReleaseApproval> list = this.GetFilteredApprovalsWithHistory(this.GetCompatibleParameters(assignedToFilter, statusFilter, releaseIdsFilter, typeFilter, top + 1, continuationToken, queryOrder, includeMyGroupApprovals)).ToList<ReleaseApproval>();
      if (queryOrder == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder.Ascending)
        list.Sort((Comparison<ReleaseApproval>) ((a1, a2) => a1.Id.CompareTo(a2.Id)));
      else
        list.Sort((Comparison<ReleaseApproval>) ((a1, a2) => a2.Id.CompareTo(a1.Id)));
      HttpResponseMessage responseMessage = (HttpResponseMessage) null;
      IDictionary<int, string> folderPaths = ReleaseManagementSecurityProcessor.GetFolderPaths(this.TfsRequestContext, this.ProjectId, (IList<int>) list.Select<ReleaseApproval, int>((Func<ReleaseApproval, int>) (a => a.ReleaseDefinitionReference.Id)).ToList<int>());
      IEnumerable<ReleaseApproval> releaseApprovals = ReleaseManagementSecurityProcessor.FilterComponents<ReleaseApproval>(this.TfsRequestContext, (IEnumerable<ReleaseApproval>) list, (Func<ReleaseApproval, ReleaseManagementSecurityInfo>) (a => ReleaseManagementSecurityProcessor.GetSecurityInfo(this.ProjectId, folderPaths[a.ReleaseDefinitionReference.Id], a.ReleaseDefinitionReference.Id, ReleaseManagementSecurityPermissions.ViewReleases)), true).Take<ReleaseApproval>(top);
      try
      {
        responseMessage = this.Request.CreateResponse<IEnumerable<ReleaseApproval>>(HttpStatusCode.OK, releaseApprovals);
        if (list.Count > top)
          RmApprovalsController.SetContinuationToken(responseMessage, list[top]);
        return responseMessage;
      }
      catch (Exception ex)
      {
        responseMessage?.Dispose();
        this.TfsRequestContext.TraceException(1961024, "ReleaseManagementService", "Service", ex);
        throw;
      }
    }

    [HttpGet]
    [ClientInternalUseOnly(false)]
    [ClientLocationId("9328E074-59FB-465A-89D9-B09C82EE5109")]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public virtual ReleaseApproval GetApproval(int approvalId, bool includeHistory = false) => this.GetReleaseApproval(approvalId, includeHistory);

    [HttpGet]
    [ClientInternalUseOnly(false)]
    [ClientLocationId("250C7158-852E-4130-A00F-A0CCE9B72D05")]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public virtual ReleaseApproval GetApprovalHistory(int approvalStepId) => this.GetHistoryForApproval((IEnumerable<int>) new List<int>()
    {
      approvalStepId
    });

    protected virtual ApprovalQueryParameters GetCompatibleParameters(
      string assignedToFilter,
      ApprovalStatus statusFilter,
      string releaseIdsFilter,
      ApprovalType typeFilter,
      int top,
      int continuationToken,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder queryOrder,
      bool includeMyGroupApprovals)
    {
      return ApprovalQueryParameters.GetQueryParameters(this.TfsRequestContext, assignedToFilter, statusFilter, releaseIdsFilter, typeFilter, top, continuationToken, queryOrder, includeMyGroupApprovals, true);
    }

    protected ReleaseApproval UpdateApproval(
      int approvalId,
      ReleaseApproval approval,
      Guid projectId)
    {
      if (approval == null)
        throw new InvalidRequestException(Resources.ApprovalNotProvidedInMethodBody);
      ApprovalsService service = this.GetService<ApprovalsService>();
      approval.Id = approvalId;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId1 = this.ProjectId;
      List<ReleaseApproval> approvals = new List<ReleaseApproval>();
      approvals.Add(approval);
      IList<DeploymentAuthorizationInfo> authorizationInfoList = this.DeploymentAuthorizationInfoList;
      IEnumerable<ReleaseEnvironmentStep> releaseEnvironmentSteps = service.UpdateApprovalsStatus(tfsRequestContext, projectId1, (IEnumerable<ReleaseApproval>) approvals, authorizationInfoList);
      return releaseEnvironmentSteps.First<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.Id == approvalId)).ToReleaseApprovalContract(this.TfsRequestContext, releaseEnvironmentSteps, projectId);
    }

    protected ReleaseApproval GetReleaseApproval(int approvalId, bool includeHistory)
    {
      IEnumerable<ReleaseEnvironmentStep> approval = this.GetService<ApprovalsService>().GetApproval(this.TfsRequestContext, this.ProjectId, approvalId, includeHistory);
      List<ReleaseApproval> releaseApprovalList = new List<ReleaseApproval>()
      {
        approval.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.Id == approvalId)).Single<ReleaseEnvironmentStep>().ToReleaseApprovalContract(this.TfsRequestContext, approval, this.ProjectId)
      };
      IDictionary<int, string> folderPaths = ReleaseManagementSecurityProcessor.GetFolderPaths(this.TfsRequestContext, this.ProjectId, (IList<int>) releaseApprovalList.Select<ReleaseApproval, int>((Func<ReleaseApproval, int>) (a => a.ReleaseDefinitionReference.Id)).ToList<int>());
      return ReleaseManagementSecurityProcessor.FilterComponents<ReleaseApproval>(this.TfsRequestContext, (IEnumerable<ReleaseApproval>) releaseApprovalList, (Func<ReleaseApproval, ReleaseManagementSecurityInfo>) (a => ReleaseManagementSecurityProcessor.GetSecurityInfo(this.ProjectId, folderPaths[a.ReleaseDefinitionReference.Id], a.ReleaseDefinitionReference.Id, ReleaseManagementSecurityPermissions.ViewReleases)), false).SingleOrDefault<ReleaseApproval>();
    }

    protected IEnumerable<ReleaseApproval> GetFilteredApprovalsWithHistory(
      ApprovalQueryParameters approvalQueryParameters)
    {
      ApprovalsService service = this.GetService<ApprovalsService>();
      List<ReleaseEnvironmentStep> list = service.GetApprovals(this.TfsRequestContext, this.ProjectId, approvalQueryParameters).ToList<ReleaseEnvironmentStep>();
      List<ReleaseApproval> approvalsWithHistory = new List<ReleaseApproval>();
      IEnumerable<ReleaseEnvironmentStep> approvalHistory = service.GetApprovalHistory(this.TfsRequestContext, this.ProjectId, list.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step => step.HasHistory())).Select<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (s => s.Id)));
      approvalsWithHistory.AddRange(list.ToReleaseApprovalContract(this.TfsRequestContext, approvalHistory, this.ProjectId));
      return (IEnumerable<ReleaseApproval>) approvalsWithHistory;
    }

    protected ReleaseApproval GetHistoryForApproval(IEnumerable<int> approvalStepIds)
    {
      List<ReleaseEnvironmentStep> list1 = this.GetService<ApprovalsService>().GetApprovalHistory(this.TfsRequestContext, this.ProjectId, approvalStepIds).ToList<ReleaseEnvironmentStep>();
      List<ReleaseEnvironmentStep> list2 = list1.Where<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (s => s.Status != ReleaseEnvironmentStepStatus.Reassigned)).ToList<ReleaseEnvironmentStep>();
      IDictionary<int, string> folderPaths = ReleaseManagementSecurityProcessor.GetFolderPaths(this.TfsRequestContext, this.ProjectId, (IList<int>) list2.Select<ReleaseEnvironmentStep, int>((Func<ReleaseEnvironmentStep, int>) (a => a.ReleaseDefinitionId)).ToList<int>());
      IList<ReleaseEnvironmentStep> source = ReleaseManagementSecurityProcessor.FilterComponents<ReleaseEnvironmentStep>(this.TfsRequestContext, (IEnumerable<ReleaseEnvironmentStep>) list2, (Func<ReleaseEnvironmentStep, ReleaseManagementSecurityInfo>) (a => ReleaseManagementSecurityProcessor.GetSecurityInfo(this.ProjectId, folderPaths[a.ReleaseDefinitionId], a.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.ViewReleases)), false);
      return !source.Any<ReleaseEnvironmentStep>() ? new ReleaseApproval() : source.First<ReleaseEnvironmentStep>().ToReleaseApprovalContract(this.TfsRequestContext, (IEnumerable<ReleaseEnvironmentStep>) list1, this.ProjectId);
    }

    private static void SetContinuationToken(
      HttpResponseMessage responseMessage,
      ReleaseApproval nextApproval)
    {
      if (nextApproval == null)
        return;
      int id = nextApproval.Id;
      responseMessage.Headers.Add("X-MS-ContinuationToken", id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }
  }
}
