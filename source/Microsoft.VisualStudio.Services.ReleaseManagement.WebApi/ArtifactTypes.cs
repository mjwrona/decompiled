// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ArtifactTypes
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  [GenerateAllConstants(null)]
  public static class ArtifactTypes
  {
    public const string BuildArtifactType = "Build";
    public const string GitArtifactType = "Git";
    public const string GitHubArtifactType = "GitHub";
    public const string GitHubReleaseArtifactType = "GitHubRelease";
    public const string JenkinsArtifactType = "Jenkins";
    public const string TfvcArtifactType = "TFVC";
    public const string CircleCIArtifactType = "CircleCI";
    public const string NugetArtifactType = "Nuget";
    public const string TfsOnPremArtifactType = "Team Build (external)";
    public const string ExternalGitArtifactType = "ExternalGit";
    public const string ExternalTfsBuildArtifactType = "ExternalTFSBuild";
    public const string SvnArtifactType = "Svn";
    public const string ExternalTfsXamlBuildArtifactType = "ExternalTfsXamlBuild";
    public const string PackageManagementArtifactType = "PackageManagement";
    public const string DockerHubArtifactType = "DockerHub";
    public const string AzureContainerRepositoryArtifactType = "AzureContainerRepository";
    public const string GitHubEnterpriseArtifactType = "GitHubEnterprise";
  }
}
