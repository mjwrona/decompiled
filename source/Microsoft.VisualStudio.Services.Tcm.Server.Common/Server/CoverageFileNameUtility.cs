// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageFileNameUtility
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Globalization;
using System.IO;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public static class CoverageFileNameUtility
  {
    public static string GetCoverageFileName(
      string buildNumber,
      BuildConfiguration buildConfig,
      bool isBuildMigrated = false)
    {
      int num = isBuildMigrated ? buildConfig.OldBuildConfigurationId : buildConfig.BuildConfigurationId;
      return CoverageFileNameUtility.SanitizeCoverageFileName(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}.coverage", (object) buildNumber, (object) buildConfig.BuildFlavor, (object) buildConfig.BuildPlatform, (object) num.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)));
    }

    private static string SanitizeCoverageFileName(string inputName)
    {
      if (string.IsNullOrEmpty(inputName))
        return inputName;
      string str = inputName;
      char newChar = '_';
      foreach (char invalidFileNameChar in Path.GetInvalidFileNameChars())
        str = str.Replace(invalidFileNameChar, newChar);
      return str;
    }
  }
}
