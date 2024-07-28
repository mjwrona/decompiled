// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseArtifactTypeService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ReleaseArtifactTypeService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public IEnumerable<ProjectReference> GetReleaseProjects(
      IVssRequestContext requestContext,
      string artifactType,
      string artifactSourceId)
    {
      List<ProjectReference> releaseProjects = new List<ProjectReference>();
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (artifactType.IsNullOrEmpty<char>())
        throw new ArgumentNullException(nameof (artifactType));
      string[] sourceIdFilter = new string[1]
      {
        artifactSourceId
      };
      Func<ReleaseDefinitionSqlComponent, IEnumerable<ReleaseDefinition>> action = (Func<ReleaseDefinitionSqlComponent, IEnumerable<ReleaseDefinition>>) (component => component.ListReleaseDefinitions(Guid.Empty, string.Empty, (IEnumerable<string>) sourceIdFilter, artifactType));
      List<ReleaseDefinition> list = requestContext.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, IEnumerable<ReleaseDefinition>>(action).ToList<ReleaseDefinition>().Where<ReleaseDefinition>((Func<ReleaseDefinition, bool>) (r => requestContext.HasPermission(r.ProjectId, r.Path, r.Id, ReleaseManagementSecurityPermissions.ViewReleaseDefinition, true))).ToList<ReleaseDefinition>();
      if (list.Count > 0)
      {
        foreach (ProjectInfo project1 in requestContext.GetService<IProjectService>().GetProjects(requestContext, ProjectState.WellFormed))
        {
          ProjectInfo project = project1;
          if (list.Any<ReleaseDefinition>((Func<ReleaseDefinition, bool>) (r => r.ProjectId.Equals(project.Id))))
            releaseProjects.Add(new ProjectReference()
            {
              Id = project.Id,
              Name = project.Name
            });
        }
      }
      return (IEnumerable<ProjectReference>) releaseProjects;
    }
  }
}
