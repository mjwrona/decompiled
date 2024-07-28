// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.RepositoryBranchReferences
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class RepositoryBranchReferences
  {
    private BuildRepository m_repository;

    public int Id { get; set; }

    public string Identifier { get; set; }

    public string Type { get; set; }

    public BranchReference[] Branches { get; set; }

    public BuildRepository Repository => this.m_repository ?? (this.m_repository = this.SummonBuildRepository());

    internal string RepositoryString { get; set; }

    private BuildRepository SummonBuildRepository() => string.IsNullOrWhiteSpace(this.RepositoryString) ? (BuildRepository) null : JsonUtility.FromString<BuildRepository>(this.RepositoryString);
  }
}
