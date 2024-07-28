// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmApprovals3Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(4.0)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "approvals", ResourceVersion = 1)]
  public class RmApprovals3Controller : RmApprovals2Controller
  {
    [HttpPatch]
    [ClientInternalUseOnly(false)]
    [ClientLocationId("c957584a-82aa-4131-8222-6d47f78bfa7a")]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public IEnumerable<ReleaseApproval> UpdateReleaseApprovals(
      [FromBody] IEnumerable<ReleaseApproval> approvals)
    {
      return this.UpdateApprovals(this.ProjectId, approvals);
    }

    protected IEnumerable<ReleaseApproval> UpdateApprovals(
      Guid projectId,
      IEnumerable<ReleaseApproval> approvals)
    {
      IEnumerable<ReleaseApproval> source1 = approvals != null && approvals.Any<ReleaseApproval>() ? approvals.Where<ReleaseApproval>((Func<ReleaseApproval, bool>) (approval => approval != null)) : throw new InvalidRequestException(Resources.ApprovalListNotProvidedInMethodBody);
      if (!source1.Any<ReleaseApproval>())
        throw new InvalidRequestException(Resources.ApprovalNotProvidedInMethodBody);
      IEnumerable<ReleaseApproval> releaseApprovals = source1.GroupBy<ReleaseApproval, int>((Func<ReleaseApproval, int>) (s => s.Id)).Select<IGrouping<int, ReleaseApproval>, ReleaseApproval>((Func<IGrouping<int, ReleaseApproval>, ReleaseApproval>) (g => g.First<ReleaseApproval>()));
      IEnumerable<ReleaseEnvironmentStep> updatedApprovals = this.GetService<ApprovalsService>().UpdateApprovalsStatus(this.TfsRequestContext, projectId, releaseApprovals, this.DeploymentAuthorizationInfoList);
      List<ReleaseEnvironmentStep> source2 = new List<ReleaseEnvironmentStep>();
      foreach (ReleaseEnvironmentStep releaseEnvironmentStep in updatedApprovals)
      {
        ReleaseEnvironmentStep approval = releaseEnvironmentStep;
        if (releaseApprovals.Any<ReleaseApproval>((Func<ReleaseApproval, bool>) (a => a.Id == approval.Id)))
          source2.Add(approval);
      }
      return (IEnumerable<ReleaseApproval>) source2.Select<ReleaseEnvironmentStep, ReleaseApproval>((Func<ReleaseEnvironmentStep, ReleaseApproval>) (u => u.ToReleaseApprovalContract(this.TfsRequestContext, updatedApprovals, projectId))).ToList<ReleaseApproval>();
    }
  }
}
