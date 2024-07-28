// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectInfoExtensions
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class ProjectInfoExtensions
  {
    public static IEnumerable<ProjectInfo> PopulateProperties(
      this IEnumerable<ProjectInfo> projects,
      IVssRequestContext requestContext,
      params string[] filters)
    {
      ArgumentUtility.CheckForNull<IEnumerable<ProjectInfo>>(projects, nameof (projects));
      List<ProjectInfo> projectInfoList = new List<ProjectInfo>(projects);
      if (projectInfoList.Count > 0)
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IReadOnlyDictionary<Guid, IEnumerable<ProjectProperty>> projectProperties = requestContext.GetService<PlatformProjectService>().GetProjectProperties(requestContext, projects.Select<ProjectInfo, Guid>(ProjectInfoExtensions.\u003C\u003EO.\u003C0\u003E__ValidateAndGetProjectId ?? (ProjectInfoExtensions.\u003C\u003EO.\u003C0\u003E__ValidateAndGetProjectId = new Func<ProjectInfo, Guid>(ProjectInfoExtensions.ValidateAndGetProjectId))), (IEnumerable<string>) filters);
        if (projectProperties != null)
        {
          foreach (ProjectInfo projectInfo in projectInfoList)
            projectInfo.Properties = (IList<ProjectProperty>) projectProperties[projectInfo.Id].ToList<ProjectProperty>();
        }
        else
        {
          foreach (ProjectInfo projectInfo in projectInfoList)
            projectInfo.Properties = (IList<ProjectProperty>) new List<ProjectProperty>();
        }
      }
      return (IEnumerable<ProjectInfo>) projectInfoList;
    }

    private static Guid ValidateAndGetProjectId(ProjectInfo project)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(project, nameof (project));
      return project.Id;
    }
  }
}
