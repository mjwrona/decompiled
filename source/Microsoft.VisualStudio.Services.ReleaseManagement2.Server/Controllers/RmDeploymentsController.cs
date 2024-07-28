// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmDeploymentsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
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
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "deployments")]
  public class RmDeploymentsController : ReleaseManagementProjectControllerBase
  {
    protected const int DefaultTop = 50;
    protected const int MaxAllowedTop = 100;
    protected const int MaxAllowedTopForReporting = 500;

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional for user")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This class should be refactored into smaller classes.")]
    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientResponseType(typeof (IPagedList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>), null, null)]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    [ClientExample("GET__ListDeploymentsForAGivenDefinitionId.json", "Get all deployments for a given definition Id", null, null)]
    [ClientExample("GET__ListDeployments.json", "Get all deployments in the project", null, null)]
    public virtual HttpResponseMessage GetDeployments(
      int definitionId = 0,
      int definitionEnvironmentId = 0,
      string createdBy = null,
      DateTime? minModifiedTime = null,
      DateTime? maxModifiedTime = null,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus deploymentStatus = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus.Undefined,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus operationStatus = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Undefined,
      bool latestAttemptsOnly = false,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder queryOrder = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder.Descending,
      [FromUri(Name = "$top")] int top = 50,
      int continuationToken = 0,
      string createdFor = null,
      DateTime? minStartedTime = null,
      DateTime? maxStartedTime = null,
      string sourceBranch = null)
    {
      using (ReleaseManagementTimer.Create(this.TfsRequestContext, "Service", "RmDeploymentsController.GetDeployments", 1900004, 8, true))
      {
        DeploymentsService service = this.TfsRequestContext.GetService<DeploymentsService>();
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder releaseQueryOrder = queryOrder == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder.Ascending ? Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder.IdAscending : Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder.IdDescending;
        top = top > 100 || top < 0 ? 100 : top;
        if (this.TfsRequestContext.IsFeatureEnabled("WebAccess.ReleaseManagement.DevOpsReporting"))
          top = top > 500 || top < 0 ? 500 : top;
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus deploymentOperationStatus = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus) operationStatus;
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus deploymentStatus1 = (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus) deploymentStatus;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        Guid projectId = this.ProjectId;
        int definitionId1 = definitionId;
        int definitionEnvironmentId1 = definitionEnvironmentId;
        string createdBy1 = createdBy;
        int operationStatus1 = (int) deploymentOperationStatus;
        int num1 = (int) deploymentStatus1;
        int num2 = latestAttemptsOnly ? 1 : 0;
        int queryOrder1 = (int) releaseQueryOrder;
        int top1 = top + 1;
        int deploymentContinuationToken = continuationToken;
        DateTime? minModifiedTime1 = minModifiedTime;
        DateTime? maxModifiedTime1 = maxModifiedTime;
        string createdFor1 = createdFor;
        DateTime? nullable1 = minStartedTime;
        DateTime? nullable2 = maxStartedTime;
        string branchName = sourceBranch;
        DateTime? minStartedTime1 = nullable1;
        DateTime? maxStartedTime1 = nullable2;
        List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> list = this.LatestToIncoming((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>) service.ListDeployments(tfsRequestContext, projectId, definitionId1, definitionEnvironmentId1, 0, createdBy1, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentOperationStatus) operationStatus1, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeploymentStatus) num1, num2 != 0, (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.ReleaseQueryOrder) queryOrder1, top1, deploymentContinuationToken, minModifiedTime1, maxModifiedTime1, createdFor: createdFor1, branchName: branchName, minStartedTime: minStartedTime1, maxStartedTime: maxStartedTime1).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Deployment>().ToContract(this.TfsRequestContext, this.ProjectId)).ToList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>();
        if (queryOrder == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseQueryOrder.Ascending)
          list.Sort((Comparison<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>) ((r1, r2) => r1.Id.CompareTo(r2.Id)));
        else
          list.Sort((Comparison<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>) ((r1, r2) => r2.Id.CompareTo(r1.Id)));
        IDictionary<int, string> folderPaths = ReleaseManagementSecurityProcessor.GetFolderPaths(this.TfsRequestContext, this.ProjectId, (IList<int>) list.Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment, int>) (d => d.ReleaseDefinitionReference.Id)).ToList<int>());
        IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> source = ReleaseManagementSecurityProcessor.FilterComponents<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>(this.TfsRequestContext, (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>) list, (Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment, ReleaseManagementSecurityInfo>) (deployment => ReleaseManagementSecurityProcessor.GetSecurityInfo(this.ProjectId, folderPaths[deployment.ReleaseDefinitionReference.Id], deployment.ReleaseDefinitionReference.Id, ReleaseManagementSecurityPermissions.ViewReleases)), true);
        HttpResponseMessage responseMessage = (HttpResponseMessage) null;
        try
        {
          responseMessage = this.Request.CreateResponse<IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>>(HttpStatusCode.OK, source.Take<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>(top));
          if (list.Count > top && list[top] != null)
            ReleaseManagementProjectControllerBase.SetContinuationToken(responseMessage, list[top].Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          return responseMessage;
        }
        catch (Exception ex)
        {
          responseMessage?.Dispose();
          this.TfsRequestContext.TraceException(1900004, "ReleaseManagementService", "Service", ex);
          throw;
        }
      }
    }

    [HttpPost]
    [PublicProjectRequestRestrictions]
    [ClientInternalUseOnly(false)]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> GetDeploymentsForMultipleEnvironments(
      [FromBody] DeploymentQueryParameters queryParameters)
    {
      if (queryParameters == null)
        throw new InvalidRequestException(Resources.NoRequestBodySpecifiedInPostMethod);
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> contract = this.TfsRequestContext.GetService<DeploymentsService>().GetDeploymentsForMultipleEnvironments(this.TfsRequestContext, this.ProjectId, queryParameters).ToContract(this.TfsRequestContext, this.ProjectId);
      IDictionary<int, string> folderPaths = ReleaseManagementSecurityProcessor.GetFolderPaths(this.TfsRequestContext, this.ProjectId, (IList<int>) contract.Select<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment, int>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment, int>) (d => d.ReleaseDefinitionReference.Id)).ToList<int>());
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> deployments = ReleaseManagementSecurityProcessor.FilterComponents<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>(this.TfsRequestContext, (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>) contract, (Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment, ReleaseManagementSecurityInfo>) (deployment => ReleaseManagementSecurityProcessor.GetSecurityInfo(this.ProjectId, folderPaths[deployment.ReleaseDefinitionReference.Id], deployment.ReleaseDefinitionReference.Id, ReleaseManagementSecurityPermissions.ViewReleases)), true);
      RmDeploymentsController.FilterDeploymentsData(deployments, queryParameters.Expands);
      return this.LatestToIncoming((IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>) deployments);
    }

    protected virtual IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> LatestToIncoming(
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> deployments)
    {
      if (deployments == null)
        return deployments;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment deployment in deployments)
      {
        if (deployment.OperationStatus == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Cancelling)
          deployment.OperationStatus = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentOperationStatus.Canceled;
      }
      return deployments;
    }

    private static void FilterDeploymentsData(
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment> deployments,
      DeploymentExpands expands)
    {
      bool flag1 = expands == DeploymentExpands.All || (expands & DeploymentExpands.Approvals) == DeploymentExpands.Approvals;
      bool flag2 = expands == DeploymentExpands.All || (expands & DeploymentExpands.Artifacts) == DeploymentExpands.Artifacts;
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment deployment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Deployment>) deployments)
      {
        if (!flag1)
        {
          deployment.PreDeployApprovals = (List<ReleaseApproval>) null;
          deployment.PostDeployApprovals = (List<ReleaseApproval>) null;
        }
        if (!flag2 && deployment.Release != null)
          deployment.Release.Artifacts = (IList<Artifact>) null;
        DateTime? scheduledDeploymentTime = deployment.ScheduledDeploymentTime;
        DateTime dateTime = new DateTime();
        if ((scheduledDeploymentTime.HasValue ? (scheduledDeploymentTime.HasValue ? (scheduledDeploymentTime.GetValueOrDefault() == dateTime ? 1 : 0) : 1) : 0) != 0)
          deployment.ScheduledDeploymentTime = new DateTime?();
      }
    }
  }
}
