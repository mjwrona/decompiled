// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildQuality
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal class BuildQuality
  {
    private static Dictionary<string, string> s_wellKnownBuildQualities;
    private const string cWellKnownBuildQualityPrefix = "EDA8CA26_B64A_44c1_A1D5_42451DD97486_";

    public int Id { get; set; }

    public string Name { get; set; }

    internal string DatabaseName => BuildQuality.TryConvertBuildQualityToResId(this.Name);

    internal static Dictionary<string, string> WellKnownBuildQualities
    {
      get
      {
        if (BuildQuality.s_wellKnownBuildQualities == null)
        {
          BuildQuality.s_wellKnownBuildQualities = new Dictionary<string, string>(8, (IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
          BuildQuality.s_wellKnownBuildQualities.Add(ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_InitialTestPassed(), "EDA8CA26_B64A_44c1_A1D5_42451DD97486_InitialTestPassed");
          BuildQuality.s_wellKnownBuildQualities.Add(ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_LabTestPassed(), "EDA8CA26_B64A_44c1_A1D5_42451DD97486_LabTestPassed");
          BuildQuality.s_wellKnownBuildQualities.Add(ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForDeployment(), "EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForDeployment");
          BuildQuality.s_wellKnownBuildQualities.Add(ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForInitialTest(), "EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForInitialTest");
          BuildQuality.s_wellKnownBuildQualities.Add(ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_Rejected(), "EDA8CA26_B64A_44c1_A1D5_42451DD97486_Rejected");
          BuildQuality.s_wellKnownBuildQualities.Add(ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_Released(), "EDA8CA26_B64A_44c1_A1D5_42451DD97486_Released");
          BuildQuality.s_wellKnownBuildQualities.Add(ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_UATPassed(), "EDA8CA26_B64A_44c1_A1D5_42451DD97486_UATPassed");
          BuildQuality.s_wellKnownBuildQualities.Add(ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_UnderInvestigation(), "EDA8CA26_B64A_44c1_A1D5_42451DD97486_UnderInvestigation");
        }
        return BuildQuality.s_wellKnownBuildQualities;
      }
    }

    internal static string TryConvertResIdToBuildQuality(string quality)
    {
      if (string.IsNullOrEmpty(quality) || !TFStringComparer.BuildQuality.StartsWith(quality, "EDA8CA26_B64A_44c1_A1D5_42451DD97486_") || quality == null)
        return quality;
      switch (quality.Length)
      {
        case 45:
          switch (quality[39])
          {
            case 'j':
              if (quality == "EDA8CA26_B64A_44c1_A1D5_42451DD97486_Rejected")
                return ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_Rejected();
              break;
            case 'l':
              if (quality == "EDA8CA26_B64A_44c1_A1D5_42451DD97486_Released")
                return ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_Released();
              break;
          }
          break;
        case 46:
          if (quality == "EDA8CA26_B64A_44c1_A1D5_42451DD97486_UATPassed")
            return ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_UATPassed();
          break;
        case 50:
          if (quality == "EDA8CA26_B64A_44c1_A1D5_42451DD97486_LabTestPassed")
            return ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_LabTestPassed();
          break;
        case 54:
          if (quality == "EDA8CA26_B64A_44c1_A1D5_42451DD97486_InitialTestPassed")
            return ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_InitialTestPassed();
          break;
        case 55:
          switch (quality[37])
          {
            case 'R':
              if (quality == "EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForDeployment")
                return ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForDeployment();
              break;
            case 'U':
              if (quality == "EDA8CA26_B64A_44c1_A1D5_42451DD97486_UnderInvestigation")
                return ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_UnderInvestigation();
              break;
          }
          break;
        case 56:
          if (quality == "EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForInitialTest")
            return ResourceStrings.EDA8CA26_B64A_44c1_A1D5_42451DD97486_ReadyForInitialTest();
          break;
      }
      return quality;
    }

    internal static string TryConvertBuildQualityToResId(string quality)
    {
      string str;
      return !string.IsNullOrEmpty(quality) && BuildQuality.WellKnownBuildQualities.TryGetValue(quality, out str) ? str : quality;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildQuality Id={0} Name={1}]", (object) this.Id, (object) this.Name);
  }
}
