// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.MavenSnapshotMetadata
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Metadata;
using Microsoft.VisualStudio.Services.Maven.Server.Models.Xml.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models.Xml
{
  [XmlRoot("metadata")]
  public class MavenSnapshotMetadata : MavenXmlMetadataBase<MavenSnapshotMetadataVersioning>
  {
    public MavenSnapshotMetadata() => this.Versioning = new MavenSnapshotMetadataVersioning();

    public MavenSnapshotMetadata(MavenPackageIdentity identity, IMavenMetadataEntry entry)
      : this()
    {
      this.GroupId = identity.Name.GroupId;
      this.ArtifactId = identity.Name.ArtifactId;
      this.Version = identity.Version.DisplayVersion;
      if (entry == null)
        return;
      MavenSnapshotMetadataFiles<MavenPackageFileNew> source1 = new MavenSnapshotMetadataFiles<MavenPackageFileNew>(entry.PackageIdentity.Name, (IEnumerable<MavenPackageFileNew>) entry.PackageFiles, (Func<MavenPackageFileNew, string>) (x => x.Path));
      DateTime dateTime;
      if (source1.Any<KeyValuePair<DateTime, SortedDictionary<int, List<MavenSnapshotMetadataFile>>>>())
      {
        KeyValuePair<DateTime, SortedDictionary<int, List<MavenSnapshotMetadataFile>>> keyValuePair1 = source1.Last<KeyValuePair<DateTime, SortedDictionary<int, List<MavenSnapshotMetadataFile>>>>();
        KeyValuePair<int, List<MavenSnapshotMetadataFile>> keyValuePair2 = keyValuePair1.Value.Last<KeyValuePair<int, List<MavenSnapshotMetadataFile>>>();
        int key = keyValuePair2.Key;
        List<MavenSnapshotMetadataFile> source2 = keyValuePair2.Value;
        if (keyValuePair1.Key > DateTime.MinValue)
        {
          foreach (MavenSnapshotMetadataFile file in source2.Where<MavenSnapshotMetadataFile>((Func<MavenSnapshotMetadataFile, bool>) (x => !string.IsNullOrWhiteSpace(x.Value))))
            this.Versioning.Versions.Add(new MavenSnapshotMetadataVersions(file));
          MavenSnapshotMetadataVersioning versioning = this.Versioning;
          MavenSnapshotMetadataLatest snapshotMetadataLatest = new MavenSnapshotMetadataLatest();
          snapshotMetadataLatest.BuildNumber = key.ToString();
          dateTime = keyValuePair1.Key;
          snapshotMetadataLatest.Timestamp = dateTime.ToString("yyyyMMdd.HHmmss");
          versioning.Latest = snapshotMetadataLatest;
        }
      }
      MavenSnapshotMetadataVersioning versioning1 = this.Versioning;
      dateTime = entry.ModifiedDate;
      string str = dateTime.ToString("yyyyMMddHHmmss");
      versioning1.LastUpdated = str;
    }
  }
}
