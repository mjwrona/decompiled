// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Health.BuildNumber
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Framework.Server.Health
{
  public struct BuildNumber : IComparable<BuildNumber>
  {
    public const string NotApplicable = "N/A";
    public readonly double MilestonePatch;
    public readonly int BuildDate;
    public readonly int BuildIndex;

    public BuildNumber(double milestonePatch, int buildDate, int buildIndex)
    {
      this.MilestonePatch = milestonePatch;
      this.BuildDate = buildDate;
      this.BuildIndex = buildIndex;
    }

    public static BuildNumber Default => new BuildNumber(999.0, 0, 0);

    public static bool TryParse(string input, out BuildNumber buildNumber)
    {
      if (input == null)
      {
        buildNumber = BuildNumber.Default;
        return false;
      }
      if (string.Equals(input, "N/A", StringComparison.OrdinalIgnoreCase))
      {
        buildNumber = new BuildNumber(0.0, 0, 0);
        return true;
      }
      Match match1 = Regex.Match(input, ".+M(\\d{3}\\.\\d+).+");
      if (!match1.Success)
        match1 = Regex.Match(input, ".+M(\\d{3}).+");
      double milestonePatch = match1.Success ? double.Parse(match1.Groups[1].Value) : (double) TeamFoundationSqlResourceComponent.CurrentServiceLevel.MilestoneNumber;
      Match match2 = Regex.Match(input, ".+(\\d{8})\\.(\\d+).*");
      if (match2.Success)
      {
        buildNumber = new BuildNumber(milestonePatch, int.Parse(match2.Groups[1].Value), int.Parse(match2.Groups[2].Value));
        return true;
      }
      Match match3 = Regex.Match(input, ".+(\\d{4})-(\\d{2})-(\\d{2})_(\\d{2})-(\\d{2})-(\\d{2}).*");
      if (match3.Success)
      {
        string s1 = match3.Groups[1].Value + match3.Groups[2].Value + match3.Groups[3].Value;
        string s2 = match3.Groups[4].Value + match3.Groups[5].Value + match3.Groups[6].Value;
        buildNumber = new BuildNumber(milestonePatch, int.Parse(s1), int.Parse(s2));
        return true;
      }
      buildNumber = BuildNumber.Default;
      return false;
    }

    private static int Compare(BuildNumber b1, BuildNumber b2)
    {
      if (b1.MilestonePatch == b2.MilestonePatch)
      {
        if (b1.BuildDate == b2.BuildDate)
        {
          if (b1.BuildIndex == b2.BuildIndex)
            return 0;
          return b1.BuildIndex < b2.BuildIndex ? -1 : 1;
        }
        return b1.BuildDate < b2.BuildDate ? -1 : 1;
      }
      return b1.MilestonePatch < b2.MilestonePatch ? -1 : 1;
    }

    public override bool Equals(object obj) => obj is BuildNumber b2 && BuildNumber.Compare(this, b2) == 0;

    public override int GetHashCode() => this.ToString().GetHashCode();

    public int CompareTo(BuildNumber other) => BuildNumber.Compare(this, other);

    public static bool operator <(BuildNumber b1, BuildNumber b2) => BuildNumber.Compare(b1, b2) < 0;

    public static bool operator <=(BuildNumber b1, BuildNumber b2) => BuildNumber.Compare(b1, b2) <= 0;

    public static bool operator >(BuildNumber b1, BuildNumber b2) => BuildNumber.Compare(b1, b2) > 0;

    public static bool operator >=(BuildNumber b1, BuildNumber b2) => BuildNumber.Compare(b1, b2) >= 0;

    public static bool operator ==(BuildNumber b1, BuildNumber b2) => BuildNumber.Compare(b1, b2) == 0;

    public static bool operator !=(BuildNumber b1, BuildNumber b2) => BuildNumber.Compare(b1, b2) != 0;

    public override string ToString() => string.Format("{0}_{1}.{2}", (object) this.MilestonePatch, (object) this.BuildDate, (object) this.BuildIndex);
  }
}
