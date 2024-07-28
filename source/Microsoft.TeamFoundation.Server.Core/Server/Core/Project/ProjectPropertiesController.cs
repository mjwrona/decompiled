// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Project.ProjectPropertiesController
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core.Project
{
  [VersionedApiControllerCustomName("core", "properties", 1)]
  [ClientGroupByResource("projects")]
  public class ProjectPropertiesController : ServerCoreApiController
  {
    private static readonly IReadOnlyCollection<string> s_allProperties = (IReadOnlyCollection<string>) new string[1]
    {
      "*"
    };

    [HttpGet]
    [ClientExample("GET__projects__project__properties.json", "Get all team project properties", null, null)]
    [ClientExample("GET__projects__project__properties_keys-_propertyName_,_wildcard_.json", "Get specific team project properties", null, null)]
    [PublicProjectRequestRestrictions(false, true, "projectid", null)]
    public IEnumerable<ProjectProperty> GetProjectProperties(Guid projectId, [ClientParameterAsIEnumerable(typeof (string), ',')] string keys = null)
    {
      IEnumerable<string> values;
      bool result;
      if (((!this.Request.Headers.TryGetValues("X-TFS-BypassProjectPropertyCache", out values) ? 0 : (bool.TryParse(values.FirstOrDefault<string>(), out result) ? 1 : 0)) & (result ? 1 : 0)) != 0)
        this.TfsRequestContext.Items.Add("BypassPropertyCache", (object) true);
      IProjectService service = this.TfsRequestContext.GetService<IProjectService>();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId1 = projectId;
      IReadOnlyCollection<string> projectPropertyFilters;
      if (!string.IsNullOrEmpty(keys))
        projectPropertyFilters = (IReadOnlyCollection<string>) keys.Split(',');
      else
        projectPropertyFilters = ProjectPropertiesController.s_allProperties;
      IEnumerable<ProjectProperty> projectProperties = service.GetProjectProperties(tfsRequestContext, projectId1, (IEnumerable<string>) projectPropertyFilters).Where<ProjectProperty>((Func<ProjectProperty, bool>) (property => !string.Equals(property.Name, "Commerce.PipelineBootstrapConfigurations", StringComparison.OrdinalIgnoreCase)));
      foreach (ProjectProperty projectProperty in projectProperties)
        projectProperty.ProjectId = projectId;
      return projectProperties;
    }

    [HttpPatch]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    [ClientEnforceBodyParameterToComeBeforeQueryParameters(false)]
    [ClientExample("PATCH__projects__project__properties.json", "Create or update a team project property", null, null)]
    [ClientExample("PATCH__projects__project__properties2.json", "Delete a team project property", null, null)]
    public void SetProjectProperties(
      Guid projectId,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<IDictionary<string, object>> patchDocument)
    {
      this.TfsRequestContext.GetService<IProjectService>().SetProjectProperties(this.TfsRequestContext, projectId, (IEnumerable<ProjectProperty>) patchDocument.ToProjectProperties());
    }
  }
}
