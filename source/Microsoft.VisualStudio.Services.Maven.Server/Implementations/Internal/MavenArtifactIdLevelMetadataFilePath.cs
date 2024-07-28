// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.Internal.MavenArtifactIdLevelMetadataFilePath
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Models;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations.Internal
{
  public class MavenArtifactIdLevelMetadataFilePath : 
    MavenMetadataFilePath,
    IMavenArtifactIdLevelMetadataFilePath,
    IMavenMetadataFilePath,
    IMavenFilePath
  {
    public string GroupId => this.PackageName.GroupId;

    public string ArtifactId => this.PackageName.ArtifactId;

    public MavenPackageName PackageName { get; }

    public MavenArtifactIdLevelMetadataFilePath(string groupId, string artifactId, string fileName = "maven-metadata.xml")
      : this(new MavenPackageName(groupId, artifactId), fileName)
    {
    }

    public MavenArtifactIdLevelMetadataFilePath(MavenPackageName packageName, string fileName = "maven-metadata.xml")
      : base(fileName)
    {
      ArgumentUtility.CheckForNull<MavenPackageName>(packageName, nameof (packageName));
      this.PackageName = packageName;
    }

    public override string FullName
    {
      get
      {
        string originalPath = this.OriginalPath;
        if (originalPath != null)
          return originalPath;
        return this.BuildPath(this.GroupId, this.ArtifactId, this.FileName);
      }
    }
  }
}
