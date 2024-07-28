// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssRestApiVersionsRegistry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class VssRestApiVersionsRegistry
  {
    private static readonly Version c_defaultLatestVersion = new Version(7, 2);
    private static readonly Version c_defaultLatestReleasedVersion = new Version(7, 1);
    private static Dictionary<VssRestApiVersion, Version> s_apiVersionMap = new Dictionary<VssRestApiVersion, Version>()
    {
      [VssRestApiVersion.v1_0] = new Version(1, 0),
      [VssRestApiVersion.v2_0] = new Version(2, 0),
      [VssRestApiVersion.v2_1] = new Version(2, 1),
      [VssRestApiVersion.v2_2] = new Version(2, 2),
      [VssRestApiVersion.v2_3] = new Version(2, 3),
      [VssRestApiVersion.v3_0] = new Version(3, 0),
      [VssRestApiVersion.v3_1] = new Version(3, 1),
      [VssRestApiVersion.v3_2] = new Version(3, 2),
      [VssRestApiVersion.v4_0] = new Version(4, 0),
      [VssRestApiVersion.v4_1] = new Version(4, 1),
      [VssRestApiVersion.v5_0] = new Version(5, 0),
      [VssRestApiVersion.v5_1] = new Version(5, 1),
      [VssRestApiVersion.v5_2] = new Version(5, 2),
      [VssRestApiVersion.v6_0] = new Version(6, 0),
      [VssRestApiVersion.v6_1] = new Version(6, 1),
      [VssRestApiVersion.v7_0] = new Version(7, 0),
      [VssRestApiVersion.v7_1] = new Version(7, 1),
      [VssRestApiVersion.v7_2] = new Version(7, 2)
    };

    public static Version ToVersion(this VssRestApiVersion version)
    {
      Version version1;
      if (VssRestApiVersionsRegistry.s_apiVersionMap.TryGetValue(version, out version1))
        return version1;
      throw new ArgumentException(string.Format("Invalid VssRestApiVersions value: {0}.", (object) version));
    }

    public static Version GetLatestVersion() => VssRestApiVersionsRegistry.c_defaultLatestVersion;

    public static Version GetLatestReleasedVersion() => VssRestApiVersionsRegistry.c_defaultLatestReleasedVersion;

    public static bool IsPreview(Version version) => version.CompareTo(VssRestApiVersionsRegistry.GetLatestReleasedVersion()) > 0;

    public static Version GetLatestReleasedVersion(Version minVersion, Version maxVersion = null)
    {
      Version latestReleasedVersion = VssRestApiVersionsRegistry.GetLatestReleasedVersion();
      if (minVersion > latestReleasedVersion)
        return new Version(0, 0);
      return !(maxVersion == (Version) null) && !(latestReleasedVersion <= maxVersion) ? maxVersion : latestReleasedVersion;
    }
  }
}
