// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Project.ProjectsPropertiesController
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core.Project
{
  [VersionedApiControllerCustomName("core", "projectsproperties", 1)]
  [ClientGroupByResource("projects")]
  [ClientInternalUseOnly(true)]
  public class ProjectsPropertiesController : ServerCoreApiController
  {
    private static readonly IReadOnlyCollection<string> s_allProperties = (IReadOnlyCollection<string>) new string[1]
    {
      "*"
    };

    [HttpGet]
    public IEnumerable<ProjectProperties> GetProjectsProperties(
      [ClientParameterAsIEnumerable(typeof (Guid), ',')] string projectIds,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string properties = null)
    {
      IEnumerable<string> values;
      bool result;
      if (((!this.Request.Headers.TryGetValues("X-TFS-BypassProjectPropertyCache", out values) ? 0 : (bool.TryParse(values.FirstOrDefault<string>(), out result) ? 1 : 0)) & (result ? 1 : 0)) != 0)
        this.TfsRequestContext.Items.Add("BypassPropertyCache", (object) true);
      IProjectService service = this.TfsRequestContext.GetService<IProjectService>();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      List<Guid> idsAsGuidList = this.ParseIdsAsGuidList(projectIds);
      IReadOnlyCollection<string> projectPropertyFilters;
      if (!string.IsNullOrEmpty(properties))
        projectPropertyFilters = (IReadOnlyCollection<string>) properties.Split(',');
      else
        projectPropertyFilters = ProjectsPropertiesController.s_allProperties;
      return service.GetProjectsProperties(tfsRequestContext, (IEnumerable<Guid>) idsAsGuidList, (IEnumerable<string>) projectPropertyFilters);
    }

    private List<Guid> ParseIdsAsGuidList(string projectIds)
    {
      List<Guid> idsAsGuidList = new List<Guid>();
      string[] strArray = projectIds.Split(',');
      List<string> stringList = new List<string>();
      foreach (string input in strArray)
      {
        Guid result;
        if (Guid.TryParse(input, out result))
          idsAsGuidList.Add(result);
        else
          stringList.Add(input);
        if (stringList.Any<string>())
          throw new ArgumentException(string.Format("{0}, projectIds: {1}", (object) TFCommonResources.EntityModel_BadGuidFormat(), (object) string.Join(", ", (IEnumerable<string>) stringList)));
      }
      return idsAsGuidList;
    }
  }
}
