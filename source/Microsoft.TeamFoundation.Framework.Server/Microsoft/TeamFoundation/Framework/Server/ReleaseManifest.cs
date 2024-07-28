// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ReleaseManifest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ReleaseManifest
  {
    private List<ReleaseInfo> m_releases = new List<ReleaseInfo>();
    private const string c_firstManifestRelease = "Tfs2010";

    [XmlArray("Releases")]
    [XmlArrayItem("Release")]
    public List<ReleaseInfo> Releases => this.m_releases;

    [XmlIgnore]
    public ServiceLevel CurrentServiceLevel => this.CurrentRelease?.CurrentServiceLevel;

    [XmlIgnore]
    public ReleaseInfo CurrentRelease
    {
      get
      {
        int currentReleaseIndex = this.GetCurrentReleaseIndex();
        return currentReleaseIndex >= 0 ? this.Releases[currentReleaseIndex] : (ReleaseInfo) null;
      }
    }

    public ReleaseInfo FindRelease(ServiceLevel serviceLevel) => this.GetRelease(serviceLevel);

    public UpdatePackage FindUpdatePackage(ServiceLevel serviceLevel)
    {
      ReleaseInfo releaseInfo = (ReleaseInfo) null;
      return this.FindUpdatePackage(serviceLevel, out releaseInfo);
    }

    public UpdatePackage FindUpdatePackage(ServiceLevel serviceLevel, out ReleaseInfo releaseInfo)
    {
      ArgumentUtility.CheckForNull<ServiceLevel>(serviceLevel, nameof (serviceLevel));
      UpdatePackage updatePackage = (UpdatePackage) null;
      releaseInfo = this.Releases.Find((Predicate<ReleaseInfo>) (release => string.Equals(release.Milestone, serviceLevel.Milestone, StringComparison.OrdinalIgnoreCase) && string.Equals(release.Version, serviceLevel.MajorVersion, StringComparison.OrdinalIgnoreCase)));
      if (releaseInfo != null)
        updatePackage = releaseInfo.UpdatePackages.Find((Predicate<UpdatePackage>) (up => up.PatchNumber == serviceLevel.PatchNumber));
      return updatePackage;
    }

    public string GetRtmMilestone(string majorVersion)
    {
      if (ServiceLevel.CompareMajorVersions(majorVersion, "Tfs2010") < 0)
        return "RTM";
      return this.Releases.Find((Predicate<ReleaseInfo>) (release => string.Equals(release.Version, majorVersion, StringComparison.OrdinalIgnoreCase) && release.IsRtm))?.Milestone;
    }

    internal string ToXml()
    {
      StringWriter stringWriter1 = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      XmlSerializer xmlSerializer = new XmlSerializer(typeof (ReleaseManifest));
      XmlSerializerNamespaces serializerNamespaces = new XmlSerializerNamespaces();
      serializerNamespaces.Add("", "");
      StringWriter stringWriter2 = stringWriter1;
      XmlSerializerNamespaces namespaces = serializerNamespaces;
      xmlSerializer.Serialize((TextWriter) stringWriter2, (object) this, namespaces);
      return stringWriter1.ToString();
    }

    public static ReleaseManifest LoadFrom(Stream releaseManifestStream)
    {
      ArgumentUtility.CheckForNull<Stream>(releaseManifestStream, nameof (releaseManifestStream));
      ReleaseManifest releaseManifest = (ReleaseManifest) new XmlSerializer(typeof (ReleaseManifest)).Deserialize(releaseManifestStream);
      foreach (ReleaseInfo release in releaseManifest.Releases)
      {
        for (int index = 0; index < release.UpdatePackages.Count; ++index)
          release.UpdatePackages[index].PatchNumber = index;
      }
      return releaseManifest;
    }

    public static ReleaseManifest LoadFrom(string releaseManifestFile)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(releaseManifestFile, nameof (releaseManifestFile));
      using (Stream releaseManifestStream = (Stream) File.OpenRead(releaseManifestFile))
        return ReleaseManifest.LoadFrom(releaseManifestStream);
    }

    private int GetCurrentReleaseIndex()
    {
      for (int index = this.Releases.Count - 1; index >= 0; --index)
      {
        ReleaseInfo release = this.Releases[index];
        if (release.UpdatePackages.Count > 0 && !release.UpdatePackages[0].Removed)
          return index;
      }
      return -1;
    }

    private ReleaseInfo GetRelease(ServiceLevel serviceLevel) => this.Releases.FirstOrDefault<ReleaseInfo>((Func<ReleaseInfo, bool>) (release => string.Equals(release.Version, serviceLevel.MajorVersion, StringComparison.OrdinalIgnoreCase) && string.Equals(release.Milestone, serviceLevel.Milestone, StringComparison.OrdinalIgnoreCase)));
  }
}
