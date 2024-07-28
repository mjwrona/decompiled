// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Builds4r4Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "builds", ResourceVersion = 4)]
  public class Builds4r4Controller : Builds4Controller
  {
    [HttpPost]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.Build.WebApi.Build), null, null)]
    public override HttpResponseMessage QueueBuild(
      [FromBody] Microsoft.TeamFoundation.Build.WebApi.Build build,
      [FromUri] bool ignoreWarnings = false,
      [FromUri] string checkInTicket = null,
      [FromUri] int? sourceBuildId = null,
      [FromUri] int? definitionId = null)
    {
      return this.QueueBuildInternal(build, ignoreWarnings, checkInTicket);
    }

    protected HttpResponseMessage QueueBuildInternal(
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      bool ignoreWarnings = false,
      string checkInTicket = null,
      int? sourceBuildId = null)
    {
      this.CheckRequestContent((object) build);
      this.ValidateBuild(build);
      BuildData serverBuild = (BuildData) null;
      BuildRequestValidationFlags validationFlags = ignoreWarnings ? BuildRequestValidationFlags.None : BuildRequestValidationFlags.WarningsAsErrors;
      serverBuild = build.ToBuildServerBuildData(this.TfsRequestContext);
      if (this.TfsRequestContext.IsFeatureEnabled("WebAccess.Build2.DoNotAllowUsersToSpoofRequestedFor") && this.TfsRequestContext.IsUserContext)
        serverBuild.RequestedFor = this.TfsRequestContext.GetUserId();
      HashSet<string> internalRuntimeVariables = new HashSet<string>();
      serverBuild.Parameters = BuildRequestHelper.SanitizeDiagnosticsParameters(serverBuild.Parameters, out internalRuntimeVariables);
      IEnumerable<IBuildRequestValidator> validators = BuildRequestValidatorProvider.GetValidators(new BuildRequestValidationOptions()
      {
        RequireOnlineAgent = true,
        WarnIfNoMatchingAgent = true,
        ValidateSourceVersionFormat = true,
        InternalRuntimeVariables = internalRuntimeVariables
      });
      Microsoft.TeamFoundation.Build.WebApi.Build webApiBuild = this.BuildService.QueueBuild(this.TfsRequestContext, serverBuild, validators, validationFlags, checkInTicket, sourceBuildId, nameof (QueueBuildInternal), "D:\\a\\_work\\1\\s\\Tfs\\Service\\Build2\\Web\\Server\\Controllers\\4.0\\Builds4Controller.cs").ToWebApiBuild(this.TfsRequestContext, this.GetApiResourceVersion().ApiVersion);
      if (webApiBuild != null)
        this.TfsRequestContext.TraceInfo(12030196, "TriggeredBuilds", "Builds4Controller: Queued manual build {0} requested for {1} with definition {2} using branch {3} version {4}", (object) webApiBuild.Id, (object) webApiBuild.RequestedFor?.DisplayName, (object) webApiBuild.Definition?.Id, (object) webApiBuild.SourceBranch, (object) webApiBuild.SourceVersion);
      else
        this.TfsRequestContext.TraceInfo(12030196, "TriggeredBuilds", "Builds4Controller: Manual call to queue a build requested for {0} with definition {1} using branch {2} did not actually queue a build", (object) build.RequestedFor?.DisplayName, (object) build.Definition?.Id, (object) serverBuild.SourceBranch);
      this.TraceQueueBuildValidationResults(serverBuild);
      if (serverBuild.ValidationResults.Any<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, bool>) (vr => vr.Result == Microsoft.TeamFoundation.Build2.Server.ValidationResult.Error)))
        throw new BuildRequestValidationFailedException(Resources.BuildRequestValidationFailed(), serverBuild.ValidationResults.Select<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>) (vr => BuildRequestValidationResultExtensions.ToWebApiBuildRequestValidationResult(vr, serverBuild.ToSecuredObject()))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>());
      if (!ignoreWarnings && serverBuild.ValidationResults.Any<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, bool>) (vr => vr.Result == Microsoft.TeamFoundation.Build2.Server.ValidationResult.Warning && !vr.Ignorable)))
        throw new BuildRequestValidationWarningException(Resources.BuildRequestValidationFailed(), serverBuild.ValidationResults.Select<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>((Func<Microsoft.TeamFoundation.Build2.Server.BuildRequestValidationResult, Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>) (vr => BuildRequestValidationResultExtensions.ToWebApiBuildRequestValidationResult(vr, serverBuild.ToSecuredObject()))).ToList<Microsoft.TeamFoundation.Build.WebApi.BuildRequestValidationResult>());
      return this.Request.CreateResponse<Microsoft.TeamFoundation.Build.WebApi.Build>(HttpStatusCode.OK, this.FixResource(webApiBuild));
    }
  }
}
