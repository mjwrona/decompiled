// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants.ArtifactTraceabilityPropertyKeys
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Constants
{
  public static class ArtifactTraceabilityPropertyKeys
  {
    public static readonly string ProjectId = "projectId";
    public static readonly string BuildNumber = "buildNumber";
    public static readonly string ConnectionId = "connectionId";
    public static readonly string OrgName = "orgName";
    public static readonly string RepoName = "repoName";
    public static readonly string ArtifactName = "artifactName";
    public static readonly string Pattern = "pattern";
    public static readonly string ContainerRegistry = nameof (ContainerRegistry);
    public static readonly string ContainerRepository = nameof (ContainerRepository);
    public static readonly string ContainerDigest = nameof (ContainerDigest);
    public static readonly string ContainerLocation = nameof (ContainerLocation);
    public static readonly string ContainerUri = nameof (ContainerUri);
  }
}
