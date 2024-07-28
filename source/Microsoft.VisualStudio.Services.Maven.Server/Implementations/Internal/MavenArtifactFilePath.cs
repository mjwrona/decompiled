// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.Internal.MavenArtifactFilePath
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Maven.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations.Internal
{
  public class MavenArtifactFilePath : 
    MavenFilePath,
    IMavenArtifactFilePath,
    IMavenFilePath,
    IMavenFullyQualifiedFilePath,
    IPackageFileName
  {
    public string GroupId { get; }

    public string ArtifactId { get; }

    public string Version { get; }

    public bool UsesSnapshotLiteralVersion { get; private set; }

    public MavenPackageName PackageName { get; }

    public MavenPackageVersion PackageVersion { get; }

    public MavenArtifactFilePath(
      string groupId,
      string artifactId,
      string version,
      string fileName)
      : base(fileName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(groupId, nameof (groupId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(version, nameof (version));
      this.ParseFileName(fileName, artifactId, version);
      this.GroupId = groupId;
      this.ArtifactId = artifactId;
      this.Version = version;
      this.PackageName = new MavenPackageName(groupId, artifactId);
      this.PackageVersion = new MavenPackageVersion(version);
    }

    public override string FullName
    {
      get
      {
        string originalPath = this.OriginalPath;
        if (originalPath != null)
          return originalPath;
        return this.BuildPath(this.GroupId, this.ArtifactId, this.Version, this.FileName);
      }
    }

    string IPackageFileName.Path => this.FullName;

    private void ParseFileName(string fileName, string artifactId, string version)
    {
      if (!fileName.StartsWith(artifactId, MavenFilePath.FileNameStringComparison))
        throw new MavenInvalidFilenameException(Resources.Error_FileNameDoesNotStartWithArtifactId((object) fileName, (object) artifactId), fileName);
      string str1 = fileName.Substring(artifactId.Length);
      string str2 = "-" + version;
      if (MavenIdentityUtility.IsSnapshotVersion(version))
      {
        if (!str1.StartsWith(str2, MavenFilePath.FileNameStringComparison))
          return;
        this.UsesSnapshotLiteralVersion = true;
      }
      else if (!str1.StartsWith(str2, MavenFilePath.FileNameStringComparison))
        throw new MavenInvalidFilenameException(Resources.Error_FileNameMustSpecifyVersionAfterArtifactId((object) fileName), fileName);
    }
  }
}
