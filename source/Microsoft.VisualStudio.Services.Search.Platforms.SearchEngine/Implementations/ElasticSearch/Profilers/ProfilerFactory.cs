// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.Profilers.ProfilerFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.Profilers
{
  internal class ProfilerFactory
  {
    private readonly int m_profileProbability;
    private readonly IEntityType m_entityType;

    internal ProfilerFactory(int profileProbability, IEntityType entityType)
    {
      this.m_profileProbability = profileProbability;
      this.m_entityType = entityType;
    }

    internal IProfiler GetProfiler(ProfilerType profilterKey) => profilterKey != ProfilerType.BasicProfiler && profilterKey == ProfilerType.CodeRepoScopedProfiler ? (IProfiler) new CodeRepoScopedProfiler(this.m_profileProbability, this.m_entityType) : (IProfiler) new BasicProfiler(this.m_profileProbability, this.m_entityType);
  }
}
