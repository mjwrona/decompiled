// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.Profilers.CodeRepoScopedProfiler
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.Profilers
{
  internal class CodeRepoScopedProfiler : IProfiler
  {
    private readonly int m_profileProbability;
    private readonly IEntityType m_entityType;

    internal CodeRepoScopedProfiler(int profilerProbability, IEntityType entityType)
    {
      this.m_profileProbability = profilerProbability;
      this.m_entityType = entityType;
    }

    public bool ShouldProfile(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request)
    {
      IEnumerable<string> source;
      if (this.m_entityType.Name == CodeEntityType.GetInstance().Name && request.SearchFilters.TryGetValue("RepositoryFilters", out source) && source.Count<string>() == 1)
      {
        string configValue = requestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/ScopedProfilerRepo", TeamFoundationHostType.Deployment, true, "");
        if ((string.IsNullOrWhiteSpace(configValue) || configValue.Equals(source.First<string>(), StringComparison.OrdinalIgnoreCase)) && this.m_profileProbability > 0 && new Random().Next(100) < this.m_profileProbability)
          return true;
      }
      return false;
    }
  }
}
