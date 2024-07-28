// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.MavenArtifactMetadata
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models.Xml
{
  [XmlRoot("metadata")]
  public class MavenArtifactMetadata : MavenXmlMetadataBase<MavenArtifactMetadataVersioning>
  {
    public MavenArtifactMetadata() => this.Versioning = new MavenArtifactMetadataVersioning();

    public MavenArtifactMetadata(
      MavenPackageName packageName,
      IEnumerable<IMavenMetadataEntry> entries)
      : this()
    {
      ArgumentUtility.CheckForNull<MavenPackageName>(packageName, nameof (packageName));
      ArgumentUtility.CheckForNull<IEnumerable<IMavenMetadataEntry>>(entries, nameof (entries));
      this.GroupId = packageName.GroupId;
      this.ArtifactId = packageName.ArtifactId;
      this.AddVersions((IEnumerable<MavenPackageVersion>) entries.Select<IMavenMetadataEntry, MavenPackageVersion>((Func<IMavenMetadataEntry, MavenPackageVersion>) (entry => entry.PackageIdentity.Version)).OrderBy<MavenPackageVersion, string>((Func<MavenPackageVersion, string>) (entry => entry.SortableVersion), (IComparer<string>) StringComparer.Ordinal));
      if (!entries.Any<IMavenMetadataEntry>())
        return;
      this.SetLastUpdated(entries.Last<IMavenMetadataEntry>());
    }

    private void AddVersions(IEnumerable<MavenPackageVersion> packageVersions)
    {
      foreach (MavenPackageVersion packageVersion in packageVersions)
        this.AddVersion(packageVersion);
    }

    private void AddVersion(MavenPackageVersion version)
    {
      string displayVersion = version?.DisplayVersion;
      if (string.IsNullOrWhiteSpace(displayVersion))
        return;
      if (!this.Versioning.Versions.Contains(displayVersion))
        this.Versioning.Versions.Add(displayVersion);
      this.Versioning.Latest = this.Versioning.Versions.LastOrDefault<string>();
      if (!version.Parser.IsRelease || MavenIdentityUtility.IsSnapshotVersion(version) || !(this.Versioning.Latest == displayVersion) && !string.IsNullOrWhiteSpace(this.Versioning.Release))
        return;
      this.Versioning.Release = displayVersion;
    }

    private void SetLastUpdated(IMavenMetadataEntry entry)
    {
      this.Version = string.IsNullOrWhiteSpace(this.Versioning.Release) ? this.Versioning.Latest : this.Versioning.Release;
      this.Versioning.SetLastUpdated(entry.ModifiedDate);
    }
  }
}
