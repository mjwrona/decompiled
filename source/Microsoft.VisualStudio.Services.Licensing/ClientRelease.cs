// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ClientRelease
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class ClientRelease
  {
    private const string s_VSReleaseRegistryPrefix = "/Service/Licensing/VisualStudioRelease/";
    private const string s_area = "Licensing";
    private const string s_layer = "ClientRelease";

    public ClientRelease(
      IVssRequestContext requestContext,
      string name,
      Version minVersion,
      Version maxVersion,
      string buildLab,
      ClientReleaseType releaseType,
      DateTimeOffset? expirationDate = null)
    {
      this.Initialize(requestContext, name, minVersion, maxVersion, buildLab, releaseType, expirationDate);
    }

    public ClientRelease(
      IVssRequestContext requestContext,
      string name,
      string minVersionValue,
      string maxVersionValue,
      string buildLabValue,
      string releaseTypeValue,
      DateTimeOffset? expirationDate = null)
    {
      Version minVersion;
      Version maxVersion;
      ClientReleaseType releaseType;
      this.ParseSettings(minVersionValue, maxVersionValue, releaseTypeValue, out minVersion, out maxVersion, out releaseType);
      this.Initialize(requestContext, name, minVersion, maxVersion, buildLabValue, releaseType, expirationDate);
    }

    private void Initialize(
      IVssRequestContext requestContext,
      string name,
      Version minVersion,
      Version maxVersion,
      string buildLab,
      ClientReleaseType releaseType,
      DateTimeOffset? expirationDate)
    {
      this.ValidateSettings(requestContext, name, minVersion, maxVersion, buildLab, releaseType, expirationDate);
      this.MinVersion = minVersion;
      this.MaxVersion = maxVersion;
      this.BuildLab = buildLab;
      this.ReleaseType = releaseType;
      this.RegistryPath = name;
      if (releaseType == ClientReleaseType.Preview && expirationDate.HasValue)
        this.ExpirationDate = DateTimeOffset.Parse(expirationDate.ToString());
      name.LastIndexOf('/');
      this.Name = name;
      this.RegistryPath = "/Service/Licensing/VisualStudioRelease/" + name;
    }

    private void ParseSettings(
      string minVersionValue,
      string maxVersionValue,
      string releaseTypeValue,
      out Version minVersion,
      out Version maxVersion,
      out ClientReleaseType releaseType)
    {
      List<string> stringList = new List<string>();
      if (string.IsNullOrEmpty(minVersionValue))
        stringList.Add("MinVersion should have some value.");
      if (string.IsNullOrEmpty(maxVersionValue))
        stringList.Add("MaxVersion should have some value.");
      if (string.IsNullOrEmpty(releaseTypeValue))
        stringList.Add("ReleaseType should have some value.");
      if (!Version.TryParse(minVersionValue, out minVersion))
        stringList.Add("Invalid MinVersion format (" + minVersionValue + ").");
      if (!Version.TryParse(maxVersionValue, out maxVersion))
        stringList.Add("Invalid MaxVersion format (" + maxVersionValue + ").");
      if (!Enum.TryParse<ClientReleaseType>(releaseTypeValue, true, out releaseType))
        stringList.Add("ReleaseType(" + releaseTypeValue + ") must be one of the valid VisualStudioReleaseType values.");
      if (stringList.Any<string>())
        throw new LicensingInvalidSettingsException(string.Join(" ", (IEnumerable<string>) stringList));
    }

    private void ValidateSettings(
      IVssRequestContext requestContext,
      string registryPath,
      Version minVersion,
      Version maxVersion,
      string buildLab,
      ClientReleaseType releaseType,
      DateTimeOffset? expirationDate)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(registryPath, nameof (registryPath));
      if (string.IsNullOrEmpty(buildLab))
        throw new LicensingInvalidSettingsException("BuildLab should have some value.");
      if (!(minVersion > maxVersion))
        return;
      Version version = minVersion;
      minVersion = maxVersion;
      maxVersion = version;
      requestContext.Trace(1031647, TraceLevel.Error, "Licensing", nameof (ClientRelease), string.Format("MinVersion({0}) is greater than MaxVersion({1}). ", (object) minVersion, (object) maxVersion) + "This has been corrected automatically but should be fixed in the configuration.");
    }

    public string Name { get; private set; }

    public Version MinVersion { get; private set; }

    public Version MaxVersion { get; private set; }

    public string BuildLab { get; private set; }

    public ClientReleaseType ReleaseType { get; private set; }

    public string RegistryPath { get; private set; }

    public DateTimeOffset ExpirationDate { get; private set; }

    public override bool Equals(object obj)
    {
      ClientRelease release = obj as ClientRelease;
      return obj != null && release != null && this.Equals(release);
    }

    public bool Equals(ClientRelease release) => release != null && this.Name == release.Name && this.ReleaseType == release.ReleaseType && this.MinVersion == release.MinVersion && this.MaxVersion == release.MaxVersion && LicensingComparers.RightNameComparer.Equals(this.BuildLab, release.BuildLab);

    public override int GetHashCode() => 23 * (23 * (23 * this.ReleaseType.GetHashCode() + (this.MinVersion == (Version) null ? 0 : this.MinVersion.GetHashCode())) + (this.MaxVersion == (Version) null ? 0 : this.MaxVersion.GetHashCode())) + (this.BuildLab == null ? 0 : this.BuildLab.GetHashCode());

    public bool IsInRange(Version version, string buildLab)
    {
      ArgumentUtility.CheckForNull<Version>(version, nameof (version));
      return (LicensingComparers.RightNameComparer.Equals(buildLab, this.BuildLab) || string.Equals(this.BuildLab, "*", StringComparison.Ordinal)) && version.CompareTo(this.MinVersion) >= 0 && version.CompareTo(this.MaxVersion) <= 0;
    }
  }
}
