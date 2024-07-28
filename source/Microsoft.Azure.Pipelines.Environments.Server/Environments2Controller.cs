// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Environments.Server.Controllers.Environments2Controller
// Assembly: Microsoft.Azure.Pipelines.Environments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A0C9A0D-816B-442F-8D21-CE0F4EA438AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Environments.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.Converters;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Environments.Server.Controllers
{
  [ControllerApiVersion(5.2)]
  [VersionedApiControllerCustomName(Area = "environments", ResourceName = "environments")]
  public sealed class Environments2Controller : EnvironmentsProjectApiController
  {
    [HttpPost]
    public EnvironmentInstance AddEnvironment(
      [FromBody] EnvironmentCreateParameter environmentCreateParameter)
    {
      ArgumentUtility.CheckForNull<EnvironmentCreateParameter>(environmentCreateParameter, nameof (environmentCreateParameter));
      return this.TfsRequestContext.GetService<IEnvironmentService>().AddEnvironment(this.TfsRequestContext, this.ProjectId, environmentCreateParameter.ToEnvironment());
    }

    [HttpGet]
    public EnvironmentInstance GetEnvironmentById(int environmentId, [ClientQueryParameter] EnvironmentExpands expands = EnvironmentExpands.None)
    {
      bool includeResourceReferences = (expands & EnvironmentExpands.ResourceReferences) == EnvironmentExpands.ResourceReferences;
      return this.TfsRequestContext.GetService<IEnvironmentService>().GetEnvironmentById(this.TfsRequestContext, this.ProjectId, environmentId, includeResourceReferences: includeResourceReferences);
    }

    [HttpGet]
    [ClientResponseType(typeof (IEnumerable<EnvironmentInstance>), null, null)]
    public HttpResponseMessage GetEnvironments([ClientQueryParameter] string name = null, [ClientQueryParameter] string continuationToken = "", [FromUri(Name = "$top")] int top = 50)
    {
      if (top < 0 || top > 1000)
        top = 1000;
      IPagedList<EnvironmentInstance> environments = this.TfsRequestContext.GetService<IEnvironmentService>().GetEnvironments(this.TfsRequestContext, this.ProjectId, name, continuationToken, maxEnvironmentsCount: top);
      HttpResponseMessage response = this.Request.CreateResponse<IPagedList<EnvironmentInstance>>(HttpStatusCode.OK, environments);
      if (!string.IsNullOrWhiteSpace(environments.ContinuationToken))
        EnvironmentsProjectApiController.SetContinuationToken(response, environments.ContinuationToken);
      return response;
    }

    [HttpPatch]
    public EnvironmentInstance UpdateEnvironment(
      int environmentId,
      EnvironmentUpdateParameter environmentUpdateParameter)
    {
      ArgumentUtility.CheckForNull<EnvironmentUpdateParameter>(environmentUpdateParameter, nameof (environmentUpdateParameter));
      return this.TfsRequestContext.GetService<IEnvironmentService>().UpdateEnvironment(this.TfsRequestContext, this.ProjectId, environmentId, environmentUpdateParameter.ToEnvironment());
    }

    [HttpDelete]
    public void DeleteEnvironment(int environmentId) => this.TfsRequestContext.GetService<IEnvironmentService>().DeleteEnvironment(this.TfsRequestContext, this.ProjectId, environmentId);
  }
}
