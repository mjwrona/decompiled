// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Rest.AzureDeploymentsController
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Build.Server.Rest
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Build", ResourceName = "Deployments")]
  public class AzureDeploymentsController : BuildApiController
  {
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static AzureDeploymentsController()
    {
      AzureDeploymentsController.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
      AzureDeploymentsController.s_httpExceptions.Add(typeof (UriFormatException), HttpStatusCode.BadRequest);
      AzureDeploymentsController.s_httpExceptions.Add(typeof (HttpRequestException), HttpStatusCode.BadRequest);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) AzureDeploymentsController.s_httpExceptions;

    [HttpGet]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.Build.WebApi.BuildDeployment>), null, null)]
    public HttpResponseMessage GetDeployments(
      string projectName = "*",
      string requestedFor = null,
      string definitionName = "*",
      string environmentName = null,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647)
    {
      TeamFoundationDeploymentService service = this.TfsRequestContext.GetService<TeamFoundationDeploymentService>();
      BuildDeploymentSpec spec = new BuildDeploymentSpec();
      if (string.IsNullOrEmpty(projectName))
        ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      if (string.IsNullOrEmpty(definitionName))
        ArgumentUtility.CheckStringForNullOrEmpty(definitionName, "definition");
      if (!string.IsNullOrEmpty(requestedFor))
        spec.RequestedFor = requestedFor;
      if (!string.IsNullOrEmpty(environmentName))
        spec.EnvironmentName = environmentName;
      if (top != int.MaxValue)
        spec.MaxDeployments = skip + top;
      spec.TeamProject = projectName;
      spec.DefinitionPath = BuildPath.Root(projectName, definitionName);
      spec.QueryOrder = BuildQueryOrder.FinishTimeDescending;
      try
      {
        IEnumerable<Microsoft.TeamFoundation.Build.Server.BuildDeployment> source = service.QueryDeployments(this.TfsRequestContext, spec).Skip<Microsoft.TeamFoundation.Build.Server.BuildDeployment>(skip);
        if (top != int.MaxValue)
          source = source.Take<Microsoft.TeamFoundation.Build.Server.BuildDeployment>(top);
        return this.GenerateResponse<Microsoft.TeamFoundation.Build.WebApi.BuildDeployment>(source.Select<Microsoft.TeamFoundation.Build.Server.BuildDeployment, Microsoft.TeamFoundation.Build.WebApi.BuildDeployment>((Func<Microsoft.TeamFoundation.Build.Server.BuildDeployment, Microsoft.TeamFoundation.Build.WebApi.BuildDeployment>) (x => this.ConvertDetailToDataContract(x))));
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
      }
      return this.GenerateResponse<Microsoft.TeamFoundation.Build.WebApi.BuildDeployment>((IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildDeployment>) new List<Microsoft.TeamFoundation.Build.WebApi.BuildDeployment>());
    }

    private Microsoft.TeamFoundation.Build.WebApi.BuildDeployment ConvertDetailToDataContract(
      Microsoft.TeamFoundation.Build.Server.BuildDeployment buildDeployment)
    {
      if (buildDeployment == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildDeployment) null;
      Microsoft.TeamFoundation.Build.WebApi.BuildDeployment dataContract = new Microsoft.TeamFoundation.Build.WebApi.BuildDeployment();
      Microsoft.TeamFoundation.Build.WebApi.BuildSummary buildSummary = new Microsoft.TeamFoundation.Build.WebApi.BuildSummary();
      if (buildDeployment.Deployment != null)
      {
        Microsoft.TeamFoundation.Build.Server.BuildSummary deployment = buildDeployment.Deployment;
        buildSummary.FinishTime = deployment.FinishTime;
        buildSummary.KeepForever = deployment.KeepForever;
        buildSummary.Quality = deployment.Quality;
        buildSummary.Reason = (Microsoft.TeamFoundation.Build.WebApi.BuildReason) deployment.Reason;
        buildSummary.StartTime = deployment.StartTime;
        buildSummary.Status = (Microsoft.TeamFoundation.Build.WebApi.BuildStatus) deployment.Status;
        ShallowReference shallowReference = new ShallowReference();
        ArtifactId artifactId = LinkingUtilities.DecodeUri(deployment.Uri);
        shallowReference.Id = int.Parse(artifactId.ToolSpecificId);
        shallowReference.Name = deployment.Number;
        shallowReference.Url = this.Url.RestLink(this.TfsRequestContext, BuildResourceIds.Builds, (object) new
        {
          buildId = int.Parse(artifactId.ToolSpecificId)
        });
        buildSummary.Build = shallowReference;
        buildSummary.RequestedFor = this.GetOwnerIdentity(this.TfsRequestContext, this.Url, deployment.RequestedFor.First<RequestedForDisplayInformation>().TeamFoundationId);
      }
      if (buildDeployment.Source != null)
      {
        ShallowReference shallowReference = new ShallowReference();
        ArtifactId artifactId = LinkingUtilities.DecodeUri(buildDeployment.Source.Uri);
        shallowReference.Id = int.Parse(artifactId.ToolSpecificId);
        shallowReference.Name = buildDeployment.Source.Number;
        shallowReference.Url = this.Url.RestLink(this.TfsRequestContext, BuildResourceIds.Builds, (object) new
        {
          buildId = int.Parse(artifactId.ToolSpecificId)
        });
        dataContract.SourceBuild = shallowReference;
      }
      dataContract.Deployment = buildSummary;
      return dataContract;
    }

    private IdentityRef GetOwnerIdentity(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      Guid ownerId)
    {
      TeamFoundationIdentity[] foundationIdentityArray = requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, new Guid[1]
      {
        ownerId
      }, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
      return foundationIdentityArray.Length == 1 && foundationIdentityArray[0] != null ? foundationIdentityArray[0].ToIdentityRef(requestContext) : (IdentityRef) null;
    }
  }
}
