// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.LabCompatibilityUtils
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System;

namespace Microsoft.TeamFoundation.Build.Common
{
  internal static class LabCompatibilityUtils
  {
    internal static LabCompatibilityResult CheckCompatibility(
      Version preferredVersion,
      Version givenVersion)
    {
      if (preferredVersion == (Version) null)
        throw new ArgumentNullException(nameof (preferredVersion));
      if (givenVersion == (Version) null)
        throw new ArgumentNullException(nameof (givenVersion));
      TeamFoundationTrace.Verbose("CheckCompatibility: Preferred {0}; Given {1}", (object) preferredVersion, (object) givenVersion);
      LabCompatibilityResult compatibilityResult = LabCompatibilityResult.OK;
      if (preferredVersion.Major != givenVersion.Major)
        compatibilityResult = LabCompatibilityResult.Fail;
      else if (preferredVersion.Minor != givenVersion.Minor)
        compatibilityResult = LabCompatibilityResult.WarnUpgrade;
      else if (preferredVersion.Build > givenVersion.Build)
        compatibilityResult = LabCompatibilityResult.InformUpgrade;
      TeamFoundationTrace.Verbose("CheckCompatibility: Returning {0}", (object) compatibilityResult);
      return compatibilityResult;
    }
  }
}
