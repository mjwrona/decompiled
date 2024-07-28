// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.ReleaseManagementProjectControllerBase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.License;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [StakeholderLicenseHandler]
  [HandlePropertySelectors]
  [ReleaseManagementSecurity]
  [TraceFilter]
  public abstract class ReleaseManagementProjectControllerBase : TfsProjectApiController
  {
    protected const string ContinuationTokenHeaderName = "x-ms-continuationtoken";
    protected const string DeploymentAuthorizationHeaderName = "x-rm-deploymentauthorization";
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Vssf require this")]
    [StaticSafe]
    public static readonly IDictionary<Type, HttpStatusCode> HttpExceptionsMap = (IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (InvalidRequestException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ReleaseDefinitionNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ReleaseDefinitionDisabledException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (DeletedReleaseDefinitionNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ReleaseDefinitionAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (QueueAlreadyExistsException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (QueueNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ArtifactAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (ArtifactDefinitionDoesNotExistException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ReleaseManagementObjectNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (AuthenticationException),
        HttpStatusCode.Unauthorized
      },
      {
        typeof (ReleaseManagementServiceException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (ReleaseManagementUnauthorizedException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (ReleaseAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (ApprovalUpdateException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ReleaseWebHookException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ReleaseNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ReleaseEnvironmentNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ReleaseStepNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ReleaseDefinitionEnvironmentTemplateAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (ReleaseDefinitionEnvironmentTemplateNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (DeletedDefinitionEnvironmentTemplateNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ReleaseDefinitionDeletionNotAllowedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ReleaseDeletionNotAllowedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (UnauthorizedAccessException),
        HttpStatusCode.Unauthorized
      },
      {
        typeof (InvalidReleaseStatusUpdateException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (QueueReleaseNotAllowedException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (ReleaseManagementExternalServiceException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TaskGroupMissingException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ExecuteServiceEndpointRequestFailedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArtifactVersionUnavailableException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MissingLicenseException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (TaskAgentQueueNotFoundException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TaskDefinitionNotFoundException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ManualInterventionNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ReleaseDefinitionEnvironmentNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (FolderExistsException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidReleaseEnvironmentStatusUpdateException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (AccessDeniedException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (InvalidMultiConfigException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TeamFoundationValidationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (MetaTaskDefinitionNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ActionDeniedBySubscriberException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (FailedToResolveEndpointVariableException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ReleaseDefinitionRevisionNotFoundException),
        HttpStatusCode.NotFound
      }
    };
    private IList<DeploymentAuthorizationInfo> deploymentAuthorizationInfoList;

    public override string ActivityLogArea => "ReleaseManagement";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => ReleaseManagementProjectControllerBase.HttpExceptionsMap;

    protected static void SetContinuationToken(
      HttpResponseMessage responseMessage,
      string tokenValue)
    {
      if (responseMessage == null)
        throw new ArgumentNullException(nameof (responseMessage));
      if (string.IsNullOrWhiteSpace(tokenValue))
        return;
      responseMessage.Headers.Add("x-ms-continuationtoken", tokenValue);
    }

    public IList<DeploymentAuthorizationInfo> DeploymentAuthorizationInfoList => this.deploymentAuthorizationInfoList ?? (this.deploymentAuthorizationInfoList = (IList<DeploymentAuthorizationInfo>) new List<DeploymentAuthorizationInfo>());

    protected T GetService<T>() where T : class, IVssFrameworkService => this.TfsRequestContext.GetService<T>();

    [NonAction]
    public override Task<HttpResponseMessage> ExecuteAsync(
      HttpControllerContext controllerContext,
      CancellationToken cancellationToken)
    {
      if (controllerContext == null)
        throw new ArgumentNullException(nameof (controllerContext));
      if (controllerContext.Request.Headers.Contains("x-rm-deploymentauthorization"))
      {
        string json = controllerContext.Request.Headers.GetValues("x-rm-deploymentauthorization").FirstOrDefault<string>();
        List<DeploymentAuthorizationInfo> authorizationInfoList;
        if (json != null && JsonUtilities.TryDeserialize<List<DeploymentAuthorizationInfo>>(json, out authorizationInfoList) && authorizationInfoList.Any<DeploymentAuthorizationInfo>())
          this.DeploymentAuthorizationInfoList.AddRange<DeploymentAuthorizationInfo, IList<DeploymentAuthorizationInfo>>((IEnumerable<DeploymentAuthorizationInfo>) authorizationInfoList);
      }
      return base.ExecuteAsync(controllerContext, cancellationToken);
    }
  }
}
