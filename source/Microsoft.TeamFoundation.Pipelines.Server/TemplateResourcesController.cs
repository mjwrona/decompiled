// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.TemplateResourcesController
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "pipelines", ResourceName = "resources")]
  public class TemplateResourcesController : TfsProjectApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (CreatedResources), null, null)]
    [ClientLocationId("43201899-7690-4870-9c79-ab69605f21ed")]
    public async Task<CreatedResources> CreateResources(
      [FromBody] IDictionary<string, ResourceCreationParameter> creationParameters)
    {
      TemplateResourcesController resourcesController = this;
      // ISSUE: explicit non-virtual call
      ArgumentUtility.CheckForNull<ProjectInfo>(__nonvirtual (resourcesController.ProjectInfo), "ProjectInfo");
      IVssRequestContext vssRequestContext = resourcesController.TfsRequestContext.To(TeamFoundationHostType.ProjectCollection);
      IDictionary<string, JObject> resourcesAsync = await vssRequestContext.GetService<IResourceCreationService>().CreateResourcesAsync(vssRequestContext, resourcesController.ProjectId, creationParameters);
      return new CreatedResources()
      {
        Resources = resourcesAsync
      };
    }
  }
}
