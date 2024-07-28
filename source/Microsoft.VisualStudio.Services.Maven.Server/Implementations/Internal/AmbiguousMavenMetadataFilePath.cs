// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.Internal.AmbiguousMavenMetadataFilePath
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations.Internal
{
  public class AmbiguousMavenMetadataFilePath : 
    MavenMetadataFilePath,
    IMavenArtifactIdLevelMetadataFilePath,
    IMavenMetadataFilePath,
    IMavenFilePath,
    IMavenGroupIdLevelMetadataFilePath
  {
    private readonly IMavenArtifactIdLevelMetadataFilePath artifactIdLevelMetadataFilePath;
    private readonly IMavenGroupIdLevelMetadataFilePath groupIdLevelMetdataFilePath;

    string IMavenArtifactIdLevelMetadataFilePath.GroupId => this.artifactIdLevelMetadataFilePath.GroupId;

    string IMavenArtifactIdLevelMetadataFilePath.ArtifactId => this.artifactIdLevelMetadataFilePath.ArtifactId;

    MavenPackageName IMavenArtifactIdLevelMetadataFilePath.PackageName => this.artifactIdLevelMetadataFilePath.PackageName;

    string IMavenGroupIdLevelMetadataFilePath.GroupId => this.groupIdLevelMetdataFilePath.GroupId;

    public AmbiguousMavenMetadataFilePath(
      IMavenArtifactIdLevelMetadataFilePath artifactIdLevelMetadataFilePath,
      IMavenGroupIdLevelMetadataFilePath groupIdLevelMetdataFilePath)
      : base(artifactIdLevelMetadataFilePath?.FileName)
    {
      ArgumentUtility.CheckForNull<IMavenArtifactIdLevelMetadataFilePath>(artifactIdLevelMetadataFilePath, nameof (artifactIdLevelMetadataFilePath));
      ArgumentUtility.CheckForNull<IMavenGroupIdLevelMetadataFilePath>(groupIdLevelMetdataFilePath, nameof (groupIdLevelMetdataFilePath));
      if (!VssStringComparer.FilePath.Equals(artifactIdLevelMetadataFilePath.FileName, groupIdLevelMetdataFilePath.FileName))
        throw new ArgumentException(Resources.Error_ArtifactIdLevelAndGroupIdLevelMetadataFilePathMustHaveSameName((object) artifactIdLevelMetadataFilePath.FileName, (object) groupIdLevelMetdataFilePath.FileName));
      this.artifactIdLevelMetadataFilePath = artifactIdLevelMetadataFilePath;
      this.groupIdLevelMetdataFilePath = groupIdLevelMetdataFilePath;
    }

    public override string FullName => this.OriginalPath ?? this.groupIdLevelMetdataFilePath.FullName;
  }
}
