// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmReleaseEnvironmentsController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "environments")]
  [ClientGroupByResource("releases")]
  public class RmReleaseEnvironmentsController : ReleaseManagementProjectControllerBase
  {
    public static bool TryGetEnvironmentUpdateData(
      HttpActionContext actionExecutingContext,
      out Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironmentUpdateMetadata environmentUpdateData)
    {
      environmentUpdateData = (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironmentUpdateMetadata) null;
      return actionExecutingContext != null && actionExecutingContext.ActionArguments != null && actionExecutingContext.ActionArguments.TryGetValue<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironmentUpdateMetadata>(nameof (environmentUpdateData), out environmentUpdateData) && environmentUpdateData != null;
    }

    [HttpPatch]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.None)]
    [ClientExample("PATCH__UpdateReleaseEnvironment.json", "Start deployment on an environment", null, null)]
    public virtual Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment UpdateReleaseEnvironment(
      int releaseId,
      int environmentId,
      [FromBody] Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironmentUpdateMetadata environmentUpdateData)
    {
      environmentUpdateData.Status = environmentUpdateData != null ? environmentUpdateData.Status.ConvertToNewEnvironmentStatusWithUpdatedValue() : throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.EmptyBodyIsNotAllowedInPatchRequest);
      return this.UpdateReleaseEnvironmentImplementation(releaseId, environmentId, environmentUpdateData);
    }

    protected Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment UpdateReleaseEnvironmentImplementation(
      int releaseId,
      int environmentId,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironmentUpdateMetadata environmentUpdateData)
    {
      if (environmentUpdateData == null)
        throw new InvalidRequestException(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.EmptyBodyIsNotAllowedInPatchRequest);
      using (PerformanceTelemetryService.Measure(this.TfsRequestContext, "Service", "RmReleaseEnvironmentsController.UpdateReleaseEnvironment", 50, true))
      {
        ReleasesService service = this.TfsRequestContext.GetService<ReleasesService>();
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = service.GetRelease(this.TfsRequestContext, this.ProjectId, releaseId);
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment = release.GetEnvironment(environmentId);
        if (environment == null)
        {
          IList<int> environmentIdList = release.GetEnvironmentIdList();
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.EnvironmentIdIsInvalid, (object) environmentId, (object) string.Join(",", environmentIdList.Select<int, string>((Func<int, string>) (id => id.ToString((IFormatProvider) CultureInfo.CurrentCulture))).ToArray<string>())));
        }
        if (environmentUpdateData != null && environmentUpdateData.Status != EnvironmentStatus.Undefined)
        {
          int definitionEnvironmentId = environment.DefinitionEnvironmentId;
          string folderPath = ReleaseManagementSecurityProcessor.GetFolderPath(this.TfsRequestContext, this.ProjectId, release.ReleaseDefinitionId);
          if (!this.TfsRequestContext.HasPermission(this.ProjectId, folderPath, release.ReleaseDefinitionId, definitionEnvironmentId, ReleaseManagementSecurityPermissions.ManageDeployments))
          {
            if (this.TfsRequestContext.IsFeatureEnabled("WebAccess.ReleaseManagement.ReleasesHub.AllowRedeployAndCancelForQueueReleasesPermission") && (RmReleaseEnvironmentsController.IsCanceledOperation(environmentUpdateData) || RmReleaseEnvironmentsController.IsRedeployOperation(environmentUpdateData, environment)))
            {
              if (!this.TfsRequestContext.HasPermission(this.ProjectId, folderPath, release.ReleaseDefinitionId, ReleaseManagementSecurityPermissions.CreateReleases))
                this.ThrowUnauthorizedException(ReleaseManagementSecurityPermissions.CreateReleases);
            }
            else
              this.ThrowUnauthorizedException(ReleaseManagementSecurityPermissions.ManageDeployments);
          }
          if (!environmentUpdateData.Variables.IsNullOrEmpty<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>() && !this.TfsRequestContext.HasPermission(this.ProjectId, folderPath, release.ReleaseDefinitionId, definitionEnvironmentId, ReleaseManagementSecurityPermissions.EditReleaseEnvironment))
            this.ThrowUnauthorizedException(ReleaseManagementSecurityPermissions.EditReleaseEnvironment);
        }
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment = service.UpdateReleaseEnvironment(this.TfsRequestContext, this.ProjectId, release, environment, environmentUpdateData.FromContract()).GetReleaseEnvironmentById(environmentId);
        if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.ReturnOnlyLatestDeploymentData"))
          releaseEnvironment = releaseEnvironment.RemoveNonLatestDeploymentAttemptData();
        return this.LatestToIncoming(releaseEnvironment.ToContract(release, this.TfsRequestContext, this.ProjectId));
      }
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ReleaseManagementSecurityPermission("releaseId", ReleaseManagementSecurityArgumentType.ReleaseId, ReleaseManagementSecurityPermissions.ViewReleases)]
    public Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment GetReleaseEnvironment(
      int releaseId,
      int environmentId)
    {
      using (PerformanceTelemetryService.Measure(this.TfsRequestContext, "Service", "RmReleaseEnvironmentsController.GetReleaseEnvironment", 50, true))
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release release = this.TfsRequestContext.GetService<ReleasesService>().GetRelease(this.TfsRequestContext, this.ProjectId, releaseId);
        if (release.GetEnvironment(environmentId) == null)
        {
          IList<int> environmentIdList = release.GetEnvironmentIdList();
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties.Resources.EnvironmentIdIsInvalid, (object) environmentId, (object) string.Join(",", environmentIdList.Select<int, string>((Func<int, string>) (id => id.ToString((IFormatProvider) CultureInfo.CurrentCulture))).ToArray<string>())));
        }
        Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment incoming = this.LatestToIncoming(release.GetReleaseEnvironmentById(environmentId).ToContract(release, this.TfsRequestContext, this.ProjectId));
        this.TfsRequestContext.SetSecuredObject<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>(incoming);
        return incoming;
      }
    }

    protected virtual Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment LatestToIncoming(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment)
    {
      if (environment == null)
        return environment;
      environment.ToNoPhasesFormat();
      if (environment.Status == EnvironmentStatus.PartiallySucceeded)
        environment.Status = EnvironmentStatus.Rejected;
      if (environment.DeploySteps != null)
      {
        foreach (DeploymentAttempt deployStep in environment.DeploySteps)
        {
          if (deployStep.Status == Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus.PartiallySucceeded)
            deployStep.Status = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.DeploymentStatus.Failed;
        }
      }
      environment.HandleCancelingStateBackCompatibility();
      environment.HandleGateCanceledStateBackCompatibility();
      return environment;
    }

    private static bool IsCanceledOperation(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironmentUpdateMetadata environmentUpdateData)
    {
      return environmentUpdateData.Status == EnvironmentStatus.Canceled;
    }

    private static bool IsRedeployOperation(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironmentUpdateMetadata environmentUpdateData,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment releaseEnvironment)
    {
      if (environmentUpdateData.Status != EnvironmentStatus.InProgress)
        return false;
      return releaseEnvironment.Status == ReleaseEnvironmentStatus.Rejected || releaseEnvironment.Status == ReleaseEnvironmentStatus.Canceled || releaseEnvironment.Status == ReleaseEnvironmentStatus.PartiallySucceeded || releaseEnvironment.Status == ReleaseEnvironmentStatus.Succeeded;
    }

    private void ThrowUnauthorizedException(ReleaseManagementSecurityPermissions permission) => throw new AccessCheckException(new ResourceAccessException(this.TfsRequestContext.GetUserId().ToString(), permission).Message);
  }
}
