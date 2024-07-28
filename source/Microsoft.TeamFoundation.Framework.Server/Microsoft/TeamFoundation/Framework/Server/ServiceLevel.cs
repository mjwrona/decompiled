// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceLevel
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class ServiceLevel : IComparable<ServiceLevel>, IEquatable<ServiceLevel>
  {
    public const string RtmMilestone = "RTM";
    public const string SP1Milestone = "SP1";
    public const string TfsDev11MajorVersion = "Dev11";
    public const string TfsDev12MajorVersion = "Dev12";
    public const string TfsDev14MajorVersion = "Dev14";
    public const string TfsDev15MajorVersion = "Dev15";
    public const string TfsDev16MajorVersion = "Dev16";
    public const string TfsDev17MajorVersion = "Dev17";
    public const string TfsDev18MajorVersion = "Dev18";
    public const string TfsDev19MajorVersion = "Dev19";
    public const string TfsDev20MajorVersion = "Dev20";
    public const string CurrentMajorVersion = "Dev19";
    public static readonly char[] ServiceLevelSeparator = new char[1]
    {
      ';'
    };
    private const string c_maxMajorVersion = "DEV20";
    internal static readonly string[] s_majorVersions = new string[13]
    {
      "TFS2005",
      "TFS2008",
      "TFS2010",
      "DEV11",
      "DEV12",
      "DEV13",
      "DEV14",
      "DEV15",
      "DEV16",
      "DEV17",
      "DEV18",
      "DEV19",
      "DEV20"
    };
    private static readonly string[] s_legacyMilestones = new string[12]
    {
      "CTP1",
      "CTP2",
      "CTP3",
      "BETA1",
      "BETA2RC",
      "BETA2RC2",
      "BETA2",
      "BETA3",
      "RC1",
      "RTM",
      "SP1BETA1",
      "SP1"
    };
    private const string c_tfs2005MajorVersion = "Tfs2005";
    private const string c_tfs2008MajorVersion = "Tfs2008";
    private const string c_tfs2010MajorVersion = "Tfs2010";
    private const string c_testPatchPrefix = "[TEST]";
    private static readonly Regex s_milestoneRegex = new Regex("^M\\d{1,3}(-((QU\\d{1,3})|(PART\\d{1,2})))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex s_deltaOperationRegex = new Regex("^(?:To|From)(?<majorVersion>Dev\\d{2})(?<milestone>M\\d{1,3}(-((QU\\d{1,3})|(Part\\d{1,2})))*)(\\.(?<patchNumber>\\d{1,3}))*(?:FinalConfiguration|Deployment|Organization|Collection)*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public ServiceLevel(string majorVersion, string milestone, int patchNumber = 0, bool isTestPatch = false)
    {
      ServiceLevel.ValidateMajorVersion(majorVersion);
      ServiceLevel.ValidateMilestone(milestone);
      ArgumentUtility.CheckForOutOfRange(patchNumber, nameof (patchNumber), 0);
      this.MajorVersion = majorVersion;
      this.Milestone = milestone;
      this.PatchNumber = patchNumber;
      this.IsTestPatch = isTestPatch;
    }

    public ServiceLevel(string serviceLevel)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceLevel, nameof (serviceLevel));
      string str = serviceLevel;
      if (str.StartsWith("[TEST]", StringComparison.OrdinalIgnoreCase))
      {
        this.IsTestPatch = true;
        str = str.Substring("[TEST]".Length);
      }
      string[] strArray = str.Split(new char[1]{ '.' }, StringSplitOptions.None);
      this.MajorVersion = strArray.Length >= 2 ? strArray[0] : throw new ArgumentException(FrameworkResources.ServiceLevelIsInvalid((object) serviceLevel), nameof (serviceLevel));
      this.Milestone = strArray[1];
      if (this.MajorVersion.Equals("Tfs2010", StringComparison.OrdinalIgnoreCase) || this.MajorVersion.Equals("Tfs2008", StringComparison.OrdinalIgnoreCase))
      {
        if (strArray.Length > 4)
          throw new ArgumentException(FrameworkResources.ServiceLevelIsInvalid((object) serviceLevel), nameof (serviceLevel));
        if (strArray.Length == 4)
        {
          int result;
          if (!strArray[3].StartsWith("P#", StringComparison.OrdinalIgnoreCase) || !int.TryParse(strArray[3].Substring(2), out result))
            throw new ArgumentException(FrameworkResources.ServiceLevelIsInvalid((object) serviceLevel), nameof (serviceLevel));
          this.PatchNumber = result;
        }
      }
      else
      {
        if (strArray.Length > 3)
          throw new ArgumentException(FrameworkResources.ServiceLevelIsInvalid((object) serviceLevel), nameof (serviceLevel));
        int result = 0;
        if (strArray.Length == 3)
        {
          if (!int.TryParse(strArray[2], out result))
            throw new ArgumentException(FrameworkResources.ServiceLevelIsInvalid((object) serviceLevel), nameof (serviceLevel));
          this.PatchNumber = result;
        }
      }
      try
      {
        ServiceLevel.ValidateMajorVersion(this.MajorVersion);
        ServiceLevel.ValidateMilestone(this.Milestone);
        ArgumentUtility.CheckForOutOfRange(this.PatchNumber, "patchNumber", 0);
      }
      catch (Exception ex)
      {
        throw new ArgumentException(FrameworkResources.ServiceLevelIsInvalid((object) serviceLevel), nameof (serviceLevel), ex);
      }
    }

    public string MajorVersion { get; private set; }

    public string Milestone { get; private set; }

    public int PatchNumber { get; set; }

    public bool IsTestPatch { get; private set; }

    public bool IsDev10 => ServiceLevel.CompareMajorVersions(this.MajorVersion, "Tfs2010") == 0;

    public bool IsDev11 => ServiceLevel.CompareMajorVersions(this.MajorVersion, "Dev11") == 0;

    public bool IsDev12 => ServiceLevel.CompareMajorVersions(this.MajorVersion, "Dev12") == 0;

    public bool IsDev14 => ServiceLevel.CompareMajorVersions(this.MajorVersion, "Dev14") == 0;

    public bool IsDev15 => ServiceLevel.CompareMajorVersions(this.MajorVersion, "Dev15") == 0;

    public bool IsDev16 => ServiceLevel.CompareMajorVersions(this.MajorVersion, "Dev16") == 0;

    public bool IsDev17 => ServiceLevel.CompareMajorVersions(this.MajorVersion, "Dev17") == 0;

    public bool IsDev18 => ServiceLevel.CompareMajorVersions(this.MajorVersion, "Dev18") == 0;

    public bool IsDev19 => ServiceLevel.CompareMajorVersions(this.MajorVersion, "Dev19") == 0;

    public bool IsDev20 => ServiceLevel.CompareMajorVersions(this.MajorVersion, "Dev20") == 0;

    public static ServiceLevel Tfs2010Rtm => new ServiceLevel("Tfs2010", "RTM");

    public static ServiceLevel Tfs2010SP1 => new ServiceLevel("Tfs2010", "SP1");

    public static ServiceLevel MaxServiceLevel => new ServiceLevel("DEV20", "M999-Part99", 9999);

    public static ServiceLevel Dev14StartingServiceLevel => new ServiceLevel("Dev14", "M62");

    public override string ToString()
    {
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) this.MajorVersion, (object) this.Milestone);
      if (this.PatchNumber > 0)
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) str, (object) this.PatchNumber);
      if (this.IsTestPatch)
        str = "[TEST]" + str;
      return str;
    }

    public override int GetHashCode() => this.MajorVersion.ToUpperInvariant().GetHashCode() ^ this.Milestone.ToUpperInvariant().GetHashCode() ^ this.PatchNumber;

    public int CompareTo(ServiceLevel other)
    {
      if ((object) other == null)
        return 1;
      int num = ServiceLevel.CompareMajorVersions(this.MajorVersion, other.MajorVersion);
      if (num == 0)
        num = ServiceLevel.CompareMilestones(this.Milestone, other.Milestone);
      if (num == 0)
        num = this.PatchNumber.CompareTo(other.PatchNumber);
      if (num == 0 && this.IsTestPatch != other.IsTestPatch)
        num = this.IsTestPatch ? -1 : 1;
      return num;
    }

    public bool Equals(ServiceLevel other) => this.CompareTo(other) == 0;

    public override bool Equals(object obj) => this.Equals(obj as ServiceLevel);

    public static int Compare(ServiceLevel serviceLevel1, ServiceLevel serviceLevel2)
    {
      if ((object) serviceLevel1 != null)
        return serviceLevel1.CompareTo(serviceLevel2);
      return (object) serviceLevel2 == null ? 0 : -1;
    }

    public static int Compare(
      IVssRequestContext requestContext,
      string operationPrefix,
      ServiceLevel serviceLevel2)
    {
      ServiceLevel serviceLevel1;
      if (ServiceLevel.CreateServiceLevelMap(requestContext, requestContext.ServiceHost.ServiceHostInternal().ServiceLevel).TryGetValue(operationPrefix, out serviceLevel1))
        return ServiceLevel.Compare(serviceLevel1, serviceLevel2);
      throw new ArgumentNullException(nameof (operationPrefix));
    }

    public static int CompareMajorVersions(string version1, string version2)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(version1, nameof (version1));
      ArgumentUtility.CheckStringForNullOrEmpty(version2, nameof (version2));
      int num1 = Array.IndexOf<string>(ServiceLevel.s_majorVersions, version1.ToUpperInvariant());
      int num2 = Array.IndexOf<string>(ServiceLevel.s_majorVersions, version2.ToUpperInvariant());
      if (num1 == -1)
        throw new ArgumentException(FrameworkResources.MajorVersionIsInvalid((object) version1), "Version1");
      return num2 != -1 ? num1.CompareTo(num2) : throw new ArgumentException(FrameworkResources.MajorVersionIsInvalid((object) version2), "Version2");
    }

    public static int CompareMilestones(string milestone1, string milestone2)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(milestone1, nameof (milestone1));
      ArgumentUtility.CheckStringForNullOrEmpty(milestone2, nameof (milestone2));
      int num1;
      if (milestone1.Length >= 2 && milestone1[0] == 'M' && char.IsNumber(milestone1[1]))
      {
        int length = 1;
        if (milestone1.Length >= 3 && char.IsNumber(milestone1[2]))
          length = 2;
        if (length == 2 && milestone1.Length >= 4 && char.IsNumber(milestone1[3]))
          length = 3;
        num1 = int.Parse(milestone1.Substring(1, length)) + 100;
      }
      else
        num1 = Array.IndexOf<string>(ServiceLevel.s_legacyMilestones, milestone1.ToUpperInvariant());
      if (num1 == -1)
        throw new ArgumentException(FrameworkResources.MilestoneIsInvalid((object) milestone1), nameof (milestone1));
      int num2;
      if (milestone2.Length >= 2 && milestone2[0] == 'M' && char.IsNumber(milestone2[1]))
      {
        int length = 1;
        if (milestone2.Length >= 3 && char.IsNumber(milestone2[2]))
          length = 2;
        if (length == 2 && milestone2.Length >= 4 && char.IsNumber(milestone2[3]))
          length = 3;
        num2 = int.Parse(milestone2.Substring(1, length)) + 100;
      }
      else
        num2 = Array.IndexOf<string>(ServiceLevel.s_legacyMilestones, milestone2.ToUpperInvariant());
      int num3 = num2 != -1 ? num1.CompareTo(num2) : throw new ArgumentException(FrameworkResources.MilestoneIsInvalid((object) milestone2), nameof (milestone2));
      if (num3 == 0)
        num3 = string.Compare(milestone1, milestone2, StringComparison.OrdinalIgnoreCase);
      return num3;
    }

    public static bool operator <(ServiceLevel serviceLevel1, ServiceLevel serviceLevel2) => ServiceLevel.Compare(serviceLevel1, serviceLevel2) < 0;

    public static bool operator >(ServiceLevel serviceLevel1, ServiceLevel serviceLevel2) => ServiceLevel.Compare(serviceLevel1, serviceLevel2) > 0;

    public static bool operator <=(ServiceLevel serviceLevel1, ServiceLevel serviceLevel2) => ServiceLevel.Compare(serviceLevel1, serviceLevel2) <= 0;

    public static bool operator >=(ServiceLevel serviceLevel1, ServiceLevel serviceLevel2) => ServiceLevel.Compare(serviceLevel1, serviceLevel2) >= 0;

    public static bool operator ==(ServiceLevel serviceLevel1, ServiceLevel serviceLevel2) => ServiceLevel.Compare(serviceLevel1, serviceLevel2) == 0;

    public static bool operator !=(ServiceLevel serviceLevel1, ServiceLevel serviceLevel2) => ServiceLevel.Compare(serviceLevel1, serviceLevel2) != 0;

    private static void ValidateMajorVersion(string majorVersion)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(majorVersion, nameof (majorVersion));
      if (!((IEnumerable<string>) ServiceLevel.s_majorVersions).Contains<string>(majorVersion, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        throw new ArgumentException(FrameworkResources.MajorVersionIsInvalid((object) majorVersion), nameof (majorVersion));
    }

    internal static bool IsValidMilestone(string milestone) => !string.IsNullOrEmpty(milestone) && (milestone[0] != 'M' ? ((IEnumerable<string>) ServiceLevel.s_legacyMilestones).Contains<string>(milestone, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : ServiceLevel.s_milestoneRegex.IsMatch(milestone));

    internal static void ValidateMilestone(string milestone)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(milestone, nameof (milestone));
      if (!ServiceLevel.IsValidMilestone(milestone))
        throw new ArgumentException(FrameworkResources.MilestoneIsInvalid((object) milestone), nameof (milestone));
    }

    public static bool TryGetServiceLevelFromOperation(
      string operationName,
      string operationPrefix,
      out ServiceLevel level)
    {
      level = (ServiceLevel) null;
      string input = operationName;
      int num;
      if (!string.IsNullOrEmpty(operationPrefix) && (num = input.IndexOf(operationPrefix, StringComparison.OrdinalIgnoreCase)) > -1)
        input = input.Substring(num + operationPrefix.Length);
      Match match = ServiceLevel.s_deltaOperationRegex.Match(input);
      if (!match.Success)
        return false;
      string majorVersion = match.Groups["majorVersion"].Value;
      string milestone = match.Groups["milestone"].Value;
      int patchNumber = 0;
      if (!string.IsNullOrEmpty(match.Groups["patchNumber"].Value))
        patchNumber = int.Parse(match.Groups["patchNumber"].Value);
      level = new ServiceLevel(majorVersion, milestone, patchNumber);
      return true;
    }

    public static string UpdateServiceLevel(
      IVssRequestContext requestContext,
      string serviceLevels,
      string area,
      string newLevel)
    {
      Dictionary<string, ServiceLevel> serviceLevelMap = ServiceLevel.CreateServiceLevelMap(requestContext, serviceLevels);
      serviceLevelMap[area] = new ServiceLevel(newLevel);
      return ServiceLevel.CreateServiceLevelString(requestContext, serviceLevelMap);
    }

    public static bool ContainsPatchMilestone(
      IVssRequestContext requestContext,
      string serviceLevels)
    {
      bool flag = false;
      foreach (ServiceLevel serviceLevel in ServiceLevel.CreateServiceLevelMap(requestContext, serviceLevels).Values)
      {
        if (serviceLevel.PatchNumber > 0)
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    public static Dictionary<string, ServiceLevel> CreateServiceLevelMap(
      IVssRequestContext requestContext,
      string serviceLevels)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return ServiceLevel.CreateServiceLevelMap(vssRequestContext.GetService<IVssRegistryService>().GetValue(vssRequestContext, (RegistryQuery) FrameworkServerConstants.ServicingAreas, ServicingOperationPrefixes.VisualStudioServices), serviceLevels);
    }

    public static Dictionary<string, ServiceLevel> CreateServiceLevelMap(
      string serviceAreas,
      string serviceLevels)
    {
      Dictionary<string, ServiceLevel> serviceLevelMap = new Dictionary<string, ServiceLevel>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (string.IsNullOrEmpty(serviceAreas) || string.IsNullOrEmpty(serviceLevels))
        return serviceLevelMap;
      string[] strArray1 = serviceAreas.Split(ServiceLevel.ServiceLevelSeparator, StringSplitOptions.None);
      string[] strArray2 = serviceLevels != null ? serviceLevels.Split(ServiceLevel.ServiceLevelSeparator, StringSplitOptions.None) : new string[strArray1.Length];
      for (int index = 0; index < strArray1.Length; ++index)
      {
        string serviceLevel = strArray2.Length < index + 1 ? ServiceLevel.Dev14StartingServiceLevel.ToString() : strArray2[index];
        serviceLevelMap.Add(strArray1[index], new ServiceLevel(serviceLevel));
      }
      return serviceLevelMap;
    }

    public static string CreateServiceLevelString(
      IVssRequestContext requestContext,
      Dictionary<string, ServiceLevel> serviceLevelMap)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return ServiceLevel.CreateServiceLevelString(vssRequestContext.GetService<IVssRegistryService>().GetValue(vssRequestContext, (RegistryQuery) FrameworkServerConstants.ServicingAreas, ServicingOperationPrefixes.VisualStudioServices), serviceLevelMap);
    }

    public static bool AreServiceLevelsEquivalent(
      IVssRequestContext requestContext,
      string x,
      string y)
    {
      Dictionary<string, ServiceLevel> serviceLevelMap = ServiceLevel.CreateServiceLevelMap(requestContext, x);
      Dictionary<string, ServiceLevel> mapY = ServiceLevel.CreateServiceLevelMap(requestContext, y);
      Func<KeyValuePair<string, ServiceLevel>, bool> predicate = (Func<KeyValuePair<string, ServiceLevel>, bool>) (pair => !object.Equals((object) pair.Value, (object) mapY[pair.Key]));
      return !serviceLevelMap.Any<KeyValuePair<string, ServiceLevel>>(predicate);
    }

    public static bool IsServiceLevelLessThanOrEqual(
      IVssRequestContext requestContext,
      string x,
      string y)
    {
      Dictionary<string, ServiceLevel> serviceLevelMap = ServiceLevel.CreateServiceLevelMap(requestContext, x);
      Dictionary<string, ServiceLevel> mapY = ServiceLevel.CreateServiceLevelMap(requestContext, y);
      Func<KeyValuePair<string, ServiceLevel>, bool> predicate = (Func<KeyValuePair<string, ServiceLevel>, bool>) (pair => pair.Value > mapY[pair.Key]);
      return !serviceLevelMap.Any<KeyValuePair<string, ServiceLevel>>(predicate);
    }

    public static bool IsServiceLevelLessThan(
      IVssRequestContext requestContext,
      string x,
      string y)
    {
      Dictionary<string, ServiceLevel> serviceLevelMap = ServiceLevel.CreateServiceLevelMap(requestContext, x);
      Dictionary<string, ServiceLevel> mapY = ServiceLevel.CreateServiceLevelMap(requestContext, y);
      Func<KeyValuePair<string, ServiceLevel>, bool> predicate = (Func<KeyValuePair<string, ServiceLevel>, bool>) (pair => pair.Value < mapY[pair.Key]);
      return serviceLevelMap.All<KeyValuePair<string, ServiceLevel>>(predicate);
    }

    public ServiceLevel GetPreviousServiceLevel()
    {
      if (!(this >= new ServiceLevel("Dev14.M65")))
        return (ServiceLevel) null;
      string majorVersion = this.MajorVersion;
      if (this.IsDev15 && this.MilestoneNumber == 96)
        majorVersion = "Dev14";
      else if (this.IsDev16 && this.MilestoneNumber == 118)
        majorVersion = "Dev15";
      else if (this.IsDev17 && this.MilestoneNumber == 133)
        majorVersion = "Dev16";
      else if (this.IsDev18 && this.MilestoneNumber == 159)
        majorVersion = "Dev17";
      else if (this.IsDev19 && this.MilestoneNumber == 204)
        majorVersion = "Dev18";
      else if (this.IsDev20)
        throw new NotSupportedException("Dev19+ service levels are not supported.");
      return new ServiceLevel(majorVersion, string.Format("M{0}", (object) (this.MilestoneNumber - 1)));
    }

    internal static string CreateServiceLevelString(
      string serviceAreas,
      Dictionary<string, ServiceLevel> serviceLevelMap)
    {
      string[] strArray = serviceAreas.Split(ServiceLevel.ServiceLevelSeparator, StringSplitOptions.None);
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (!serviceLevelMap.ContainsKey(strArray[index]))
          throw new ArgumentException(FrameworkResources.ServiceVersionAreaNotRegistered((object) strArray[index]));
        string str = serviceLevelMap[strArray[index]] == (ServiceLevel) null ? string.Empty : serviceLevelMap[strArray[index]].ToString();
        stringBuilder.Append(str);
        if (index < strArray.Length - 1)
          stringBuilder.Append(";");
      }
      return stringBuilder.ToString();
    }

    internal int MilestoneNumber
    {
      get
      {
        string s = this.Milestone.Length >= 3 ? this.Milestone.Substring(1) : throw new InvalidOperationException(string.Format("Don't know how to interpret current milestone string ({0})", (object) this.Milestone));
        int length = s.IndexOf("-Part", StringComparison.Ordinal);
        if (length > 0)
          s = s.Substring(0, length);
        int result;
        if (!int.TryParse(s, out result))
          throw new InvalidOperationException(string.Format("Don't know how to interpret current milestone string ({0})", (object) this.Milestone));
        return result;
      }
    }

    internal static bool ContainsMajorVersion(string text) => ((IEnumerable<string>) ServiceLevel.s_majorVersions).Any<string>((Func<string, bool>) (v => text.IndexOf(v, StringComparison.InvariantCultureIgnoreCase) != -1));
  }
}
