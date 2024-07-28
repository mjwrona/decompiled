// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ProjectInfoExtensions
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Types
{
  public static class ProjectInfoExtensions
  {
    public static void PopulateProperties(
      this ProjectInfo project,
      IVssRequestContext requestContext,
      params string[] filters)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(project, nameof (project));
      IProjectService service = requestContext.GetService<IProjectService>();
      project.Properties = (IList<ProjectProperty>) service.GetProjectProperties(requestContext, project.Id, (IEnumerable<string>) filters).ToList<ProjectProperty>();
    }
  }
}
