// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmReleases2Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "releases", ResourceVersion = 2)]
  public class RmReleases2Controller : RmReleasesController
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [HttpGet]
    [ClientLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases, true)]
    public override Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release GetRelease(
      int releaseId,
      [ClientIgnore] bool includeAllApprovals = true,
      ApprovalFilters approvalFilters = ApprovalFilters.All,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string propertyFilters = null,
      [FromUri(Name = "$expand")] SingleReleaseExpands expands = SingleReleaseExpands.Tasks,
      [FromUri(Name = "$topGateRecords")] int topGateRecords = 5)
    {
      using (ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RmReleases2Controller.GetRelease", 1971003, 3, true))
      {
        this.PublishGetReleaseCustomerIntelligenceData(this.TfsRequestContext, releaseId);
        ReleasesService service = this.TfsRequestContext.GetService<ReleasesService>();
        IList<string> stringList = ServerModelUtility.ToStringList(propertyFilters);
        IVssRequestContext tfsRequestContext1 = this.TfsRequestContext;
        Guid projectId1 = this.ProjectId;
        int releaseId1 = releaseId;
        IList<string> propertyFilters1 = stringList;
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = service.GetRelease(tfsRequestContext1, projectId1, releaseId1, propertyFilters: (IEnumerable<string>) propertyFilters1);
        if (!includeAllApprovals)
          approvalFilters = ApprovalFilters.ManualApprovals | ApprovalFilters.AutomatedApprovals;
        topGateRecords = topGateRecords > 10 || topGateRecords <= 0 ? 10 : topGateRecords;
        bool flag = (expands & SingleReleaseExpands.Tasks) == SingleReleaseExpands.Tasks;
        IVssRequestContext tfsRequestContext2 = this.TfsRequestContext;
        Guid projectId2 = this.ProjectId;
        int num1 = flag ? 1 : 0;
        int num2 = (int) approvalFilters;
        int numberOfGateRecords = topGateRecords;
        return this.LatestToIncoming(release.ToContract(tfsRequestContext2, projectId2, num1 != 0, (ApprovalFilters) num2, numberOfGateRecords));
      }
    }

    [HttpPut]
    [ClientLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ManageReleases)]
    [ClientExample("PUT__UpdateRelease.json", "Update the release", null, null)]
    public override Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release UpdateRelease(
      int releaseId,
      [FromBody] Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release)
    {
      if (release == null)
        throw new InvalidRequestException(Resources.NoRequestBodySpecifiedInPostMethod);
      ReleasesService service = this.TfsRequestContext.GetService<ReleasesService>();
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release1 = service.GetRelease(this.TfsRequestContext, this.ProjectId, releaseId);
      release = this.IncomingToLatest(release, release1);
      return this.LatestToIncoming(service.UpdateRelease(this.TfsRequestContext, this.ProjectId, releaseId, release, release1).ToContract(this.TfsRequestContext, this.ProjectId, true, ApprovalFilters.All));
    }

    [HttpPatch]
    [ClientLocationId("A166FDE7-27AD-408E-BA75-703C2CC9D500")]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.None)]
    [ClientExample("PATCH__AbandonAnActiveRelease.json", "Abandoning an active release", null, null)]
    public override Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release UpdateReleaseResource(
      int releaseId,
      [FromBody] Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseUpdateMetadata releaseUpdateMetadata)
    {
      return this.UpdateReleaseResourceImplementation(releaseId, releaseUpdateMetadata);
    }

    protected override Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release LatestToIncoming(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (release.Environments != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) release.Environments)
        {
          environment.ToNoPhasesFormat();
          environment.HandleCancelingStateBackCompatibility();
          environment.HandleGateCanceledStateBackCompatibility();
          if (environment.Status == EnvironmentStatus.PartiallySucceeded)
            environment.Status = EnvironmentStatus.Rejected;
          if (environment.DeploySteps != null)
          {
            foreach (DeploymentAttempt deployStep in environment.DeploySteps)
            {
              if (deployStep.Status == DeploymentStatus.PartiallySucceeded)
                deployStep.Status = DeploymentStatus.Failed;
              deployStep.HandleTaskStatus();
            }
          }
        }
      }
      if (release.Reason == ReleaseReason.PullRequest)
        release.Reason = ReleaseReason.ContinuousIntegration;
      return release;
    }
  }
}
