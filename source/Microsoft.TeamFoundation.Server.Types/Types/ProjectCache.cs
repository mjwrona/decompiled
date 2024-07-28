// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ProjectCache
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Types
{
  public class ProjectCache
  {
    private readonly VssMemoryCacheList<Guid, ProjectInfo> m_projects;
    private readonly IVssMemoryCacheGrouping<Guid, ProjectInfo, string> m_nameMapping;
    private readonly IVssMemoryCacheGrouping<Guid, ProjectInfo, ProjectState> m_stateMapping;

    public ProjectCache(
      IVssRequestContext requestContext,
      IVssCachePerformanceProvider cacheService,
      IEnumerable<ProjectInfo> projects)
    {
      this.m_projects = new VssMemoryCacheList<Guid, ProjectInfo>(cacheService);
      this.m_nameMapping = VssMemoryCacheGroupingFactory.Create<Guid, ProjectInfo, string>(requestContext, this.m_projects, (Func<Guid, ProjectInfo, IEnumerable<string>>) ((K, V) => (IEnumerable<string>) V.KnownNames), (IEqualityComparer<string>) TFStringComparer.TeamProjectCollectionName);
      this.m_stateMapping = VssMemoryCacheGroupingFactory.Create<Guid, ProjectInfo, ProjectState>(requestContext, this.m_projects, (Func<Guid, ProjectInfo, IEnumerable<ProjectState>>) ((K, V) => (IEnumerable<ProjectState>) new ProjectState[1]
      {
        V.State
      }));
      foreach (ProjectInfo project in projects)
        this.m_projects.Add(project.Id, project, false);
    }

    public void Clear() => this.m_projects.Clear();

    public bool Update(ProjectInfo projectInfo) => this.m_projects.Add(projectInfo.Id, projectInfo.Clone(), true);

    public bool Remove(Guid projectId) => this.m_projects.Remove(projectId);

    public bool TryGetValue(Guid projectId, out ProjectInfo projectInfo)
    {
      ProjectInfo projectInfo1;
      if (this.m_projects.TryGetValue(projectId, out projectInfo1))
      {
        projectInfo = projectInfo1.Clone();
        return true;
      }
      projectInfo = (ProjectInfo) null;
      return false;
    }

    public bool TryGetValue(string projectName, out ProjectInfo projectInfo)
    {
      projectInfo = (ProjectInfo) null;
      IEnumerable<Guid> keys;
      if (this.m_nameMapping.TryGetKeys(projectName, out keys))
      {
        long num = 0;
        ProjectInfo projectInfo1 = (ProjectInfo) null;
        foreach (Guid projectId in keys)
        {
          if (this.TryGetValue(projectId, out projectInfo1) && projectInfo1.Revision >= num)
          {
            num = projectInfo1.Revision;
            projectInfo = projectInfo1;
          }
        }
      }
      return projectInfo != null;
    }

    public IEnumerable<ProjectInfo> GetValues(ProjectState projectState)
    {
      HashSet<Guid> guidSet = new HashSet<Guid>();
      Array array;
      if (projectState != ProjectState.All)
        array = (Array) new ProjectState[1]{ projectState };
      else
        array = Enum.GetValues(typeof (ProjectState));
      foreach (ProjectState groupingKey in array)
      {
        IEnumerable<Guid> keys;
        if (this.m_stateMapping.TryGetKeys(groupingKey, out keys))
          guidSet.UnionWith(keys);
      }
      foreach (Guid projectId in guidSet)
      {
        ProjectInfo projectInfo;
        if (this.TryGetValue(projectId, out projectInfo))
          yield return projectInfo;
      }
    }
  }
}
