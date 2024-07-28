// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.TfsGitArtifact
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public class TfsGitArtifact : RepositoryArtifact
  {
    private readonly string m_uniqueSourceIdentifier = "{{connection}}:{{projectId}}:{{repoName}}";

    public override string Name => "TfsGit";

    public override string Type => "TfsGit";

    public override string DisplayName => "TfsGit";

    public override string UniqueSourceIdentifier => this.m_uniqueSourceIdentifier;
  }
}
