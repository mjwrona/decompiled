// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TeamFoundationVersion
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation
{
  public static class TeamFoundationVersion
  {
    private const string MinimumTeamFoundationVersion = "1.0";
    private const string CurrentTeamFoundationVersion = "1.0";
    internal static Version minimumVersion = TeamFoundationVersion.CreateVersion("1.0");
    internal static Version currentVersion = TeamFoundationVersion.CreateVersion("1.0");

    internal static Version CreateVersion(string s)
    {
      Version version = new Version(s);
      if (version.Revision == -1)
        version = version.Build != -1 ? new Version(version.Major, version.Minor, version.Build, 0) : new Version(version.Major, version.Minor, 0, 0);
      return version;
    }

    public static Version MinimumContractVersion => TeamFoundationVersion.minimumVersion;

    public static Version CurrentContractVersion => TeamFoundationVersion.currentVersion;

    public static bool Validate(string version)
    {
      try
      {
        return TeamFoundationVersion.Validate(TeamFoundationVersion.CreateVersion(version));
      }
      catch
      {
        return false;
      }
    }

    public static bool Validate(Version version) => version >= TeamFoundationVersion.minimumVersion && version <= TeamFoundationVersion.currentVersion;

    public static string ContractVersionRange => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}..{1}", (object) TeamFoundationVersion.minimumVersion.ToString(), (object) TeamFoundationVersion.currentVersion.ToString());
  }
}
