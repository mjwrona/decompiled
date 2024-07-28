// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmApprovals2Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "approvals", ResourceVersion = 2)]
  public class RmApprovals2Controller : RmApprovalsController
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientLocationId("B47C6458-E73B-47CB-A770-4DF1E8813A91")]
    [ClientResponseType(typeof (IPagedList<ReleaseApproval>), null, null)]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    [ClientExample("GET__ListAllPendingApprovals.json", "Pending for all users", null, null)]
    [ClientExample("GET__ListPendingApprovalsForASpecificUser.json", "Pending for a specific user", null, null)]
    [ClientExample("GET__ListPendingApprovalsForASpecificARelease.json", "Pending for a specific release", null, null)]
    public override HttpResponseMessage GetApprovals(
      string assignedToFilter = null,
      ApprovalStatus statusFilter = ApprovalStatus.Pending,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string releaseIdsFilter = null,
      ApprovalType typeFilter = ApprovalType.Undefined,
      int top = 50,
      int continuationToken = 0,
      ReleaseQueryOrder queryOrder = ReleaseQueryOrder.Descending,
      bool includeMyGroupApprovals = false)
    {
      return base.GetApprovals(assignedToFilter, statusFilter, releaseIdsFilter, typeFilter, top, continuationToken, queryOrder, includeMyGroupApprovals);
    }

    protected override ApprovalQueryParameters GetCompatibleParameters(
      string assignedToFilter,
      ApprovalStatus statusFilter,
      string releaseIdsFilter,
      ApprovalType typeFilter,
      int top,
      int continuationToken,
      ReleaseQueryOrder queryOrder,
      bool includeMyGroupApprovals)
    {
      return ApprovalQueryParameters.GetQueryParameters(this.TfsRequestContext, assignedToFilter, statusFilter, releaseIdsFilter, typeFilter, top, continuationToken, queryOrder, includeMyGroupApprovals);
    }
  }
}
