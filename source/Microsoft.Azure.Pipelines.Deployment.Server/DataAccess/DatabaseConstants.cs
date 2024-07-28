// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.DatabaseConstants
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal static class DatabaseConstants
  {
    public const int ArtifactTypeColumnLength = 256;
    public const int ArtifactAliasColumnLength = 256;
    public const int ArtifactVersionIdColumnLength = 256;
    public const int ArtifactVersionNameColumnLength = 256;
    public const int ArtifactNameColumnLength = 256;
    public const int UniqueResourceIdentifierColumnLength = 2048;
    public const int RepoIdColumnLength = 1024;
    public const int RepoNameColumnLength = 1024;
    public const int RepoBranchColumnLength = 1024;
    public const int CommitIdColumnLength = 256;
    public const int RepoPropertiesColumnLength = 2048;
    public const int ArtifactVersionPropertiesColumnLength = 2048;
    public const int JobNameColumnLength = 1024;
    public const int JobIdColumnLength = 520;
  }
}
