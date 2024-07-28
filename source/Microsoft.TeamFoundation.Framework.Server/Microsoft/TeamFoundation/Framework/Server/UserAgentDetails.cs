// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UserAgentDetails
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class UserAgentDetails : IEquatable<UserAgentDetails>
  {
    private static readonly Dictionary<string, UserAgentDetails.ExeNameEntry> s_exeNames = ((IEnumerable<UserAgentDetails.ExeNameEntry>) new UserAgentDetails.ExeNameEntry[24]
    {
      new UserAgentDetails.ExeNameEntry("devenv.exe", UserAgentType.VisualStudio, (string) null),
      new UserAgentDetails.ExeNameEntry("wdexpress.exe", UserAgentType.VisualStudio, "Express"),
      new UserAgentDetails.ExeNameEntry("vpdexpress.exe", UserAgentType.VisualStudio, "Express"),
      new UserAgentDetails.ExeNameEntry("vwdexpress.exe", UserAgentType.VisualStudio, "Express"),
      new UserAgentDetails.ExeNameEntry("vswinexpress.exe", UserAgentType.VisualStudio, "Express"),
      new UserAgentDetails.ExeNameEntry("tf.exe", UserAgentType.TfExe, (string) null),
      new UserAgentDetails.ExeNameEntry("tfpt.exe", UserAgentType.TfPt, (string) null),
      new UserAgentDetails.ExeNameEntry("excel.exe", UserAgentType.Office, (string) null),
      new UserAgentDetails.ExeNameEntry("msbuild.exe", UserAgentType.BuildAgent, (string) null),
      new UserAgentDetails.ExeNameEntry("tfsbuildservicehost.exe", UserAgentType.BuildAgent, (string) null),
      new UserAgentDetails.ExeNameEntry("vsoagent.exe", UserAgentType.BuildAgent, (string) null),
      new UserAgentDetails.ExeNameEntry("BuildNotification.exe", UserAgentType.BuildNotification, (string) null),
      new UserAgentDetails.ExeNameEntry("BuildNotificationApp.exe", UserAgentType.BuildNotification, (string) null),
      new UserAgentDetails.ExeNameEntry("blend.exe", UserAgentType.Blend, (string) null),
      new UserAgentDetails.ExeNameEntry("mstest.exe", UserAgentType.MSTest, (string) null),
      new UserAgentDetails.ExeNameEntry("mtm.exe", UserAgentType.MTM, (string) null),
      new UserAgentDetails.ExeNameEntry("TfsCommandRunnerSvr.exe", UserAgentType.TfsShellPlugin, (string) null),
      new UserAgentDetails.ExeNameEntry("TfsComProviderSvr.exe", UserAgentType.TfsShellPlugin, (string) null),
      new UserAgentDetails.ExeNameEntry("TFsMsscciSvr.exe", UserAgentType.TfsMsscci, (string) null),
      new UserAgentDetails.ExeNameEntry("TfsConfig.exe", UserAgentType.TfsAdmin, (string) null),
      new UserAgentDetails.ExeNameEntry("TfsMgmt.exe", UserAgentType.TfsAdmin, (string) null),
      new UserAgentDetails.ExeNameEntry("tfssecurity.exe", UserAgentType.TfsSecurity, (string) null),
      new UserAgentDetails.ExeNameEntry("witadmin.exe", UserAgentType.WitAdmin, (string) null),
      new UserAgentDetails.ExeNameEntry("tfsbuild.exe", UserAgentType.TfsBuild, (string) null)
    }).ToDictionary<UserAgentDetails.ExeNameEntry, string>((Func<UserAgentDetails.ExeNameEntry, string>) (x => x.ExeName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly UserAgentDetails.UserAgentMatcher[] s_userAgentMatchers = new UserAgentDetails.UserAgentMatcher[31]
    {
      new UserAgentDetails.UserAgentMatcher(UserAgentType.VisualStudio, "^Team Foundation \\((?<exeName>[\\w.]+), (?<build>(?<majorVersion>\\d+)[\\d.]+)(, (?<skuName>\\w+))?.*"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.VisualStudio, "^VSServices/(?<build>(?<majorVersion>\\d+)\\.[0-9.]+) \\((?<exeName>[\\w.]+)( ,(?<skuName>\\w+),)?.*"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.UnknownOMClient, "^Team Foundation \\(.*, (?<build>(?<majorVersion>\\d+)\\.[0-9.]+), (?<skuName>\\w+),.*"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.UnknownOMClient, "^VSServices/(?<majorVersion>\\d+).(?<build>[0-9.]+) \\(.*, ,(?<skuName>\\w+),.*"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.Spartan, ".*Edge/(?<majorVersion>[0-9.]+).*"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.ChromeMobile, ".*Android.*(Chrome|CrMo|CriOS)/(?<majorVersion>\\d+).*"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.ChromeMobile, ".*(iPhone|IPad|IPod).*CriOS/(?<majorVersion>\\d+).*"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.SafariMobile, ".*(IPhone|IPad|IPod).*(KHTML, like Gecko).*Version/(?<majorVersion>\\d+)[0-9.]+.*"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.InternetExplorerMobile, ".*MSIE (?<majorVersion>\\d+).*Windows Phone.*"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.OperaMobile, ".*Opera Mini/(?<majorVersion>\\d+).*$"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.OperaMobile, ".*Opera Mobi.*Opera (?<majorVersion>\\d+)[0-9.]*$"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.OperaMobile, "^Opera.*Opera Mobi.*Version/(?<majorVersion>\\d+)[0-9.]*$"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.FirefoxMobile, "Mozilla.*(Mobile|Tablet).*rv:[0-9.]+.*Gecko/\\d+.*Firefox/(?<majorVersion>\\d+)[0-9.]*$"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.Opera, "Opera[ ]+(?<majorVersion>\\d+)[0-9.]+$"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.Opera, "Opera.*Version/(?<majorVersion>\\d+)"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.Opera, "^Opera/(?<majorVersion>\\d+)"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.Chrome, "Chrome/(?<majorVersion>\\d+)"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.Firefox, "Firefox/(?<majorVersion>\\d+).*"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.InternetExplorer, "Mozilla.*Trident.*rv:(?<majorVersion>\\d+).*"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.InternetExplorer, "MSIE.*Trident/(?<majorVersion>\\d+)", new Func<string, string>(UserAgentDetails.TridentVersionToInternetExplorerVersion)),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.InternetExplorer, "MSIE\\s*(?<majorVersion>\\d+)"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.Safari, "Version/(?<majorVersion>\\d+).*Safari/(?<build>[0-9.]+)"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.Git, "^git/(?<build>(?<majorVersion>[\\d]+(\\.\\d+)?)[^ ]*)?"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.TeeEclipse, "^Team[ ]+Explorer[ ]+Everywhere,.*\\(Plugin (?<build>(?<majorVersion>[\\d]+)[\\d.]+)"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.TeeEclipse, "^TeamExplorerEverywherePlugin/(?<build>(?<majorVersion>[\\d]+)[\\d.]+)"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.TeeEclipse, "^Team Explorer Everywhere/(?<build>(?<majorVersion>[\\d]+)[\\d.]+)"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.TeeCommandLine, "^Team[ ]+Explorer[ ]+Everywhere,.*\\(CLC (?<build>(?<majorVersion>[\\d]+)[\\d.]+)"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.TeeCommandLine, "^TeamExplorerEverywhereCLC/(?<build>(?<majorVersion>[\\d]+)[\\d.]+)"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.JavaSDK, "^Team[ ]+Explorer[ ]+Everywhere,.*\\(SDK (?<build>(?<majorVersion>[\\d]+)[\\d.]+)"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.JavaSDK, "^TeamExplorerEverywhereSDK/(?<build>(?<majorVersion>[\\d]+)[\\d.]+)"),
      new UserAgentDetails.UserAgentMatcher(UserAgentType.JavaSDK, "^TFSSDKForJava/(?<build>(?<majorVersion>[\\d]+)[\\d.]+)")
    };
    private readonly UserAgentType m_userAgentType;
    private readonly string m_majorVersion;
    private readonly string m_build;
    private readonly string m_skuName;

    public UserAgentDetails(string userAgent)
    {
      if (userAgent == null)
        return;
      try
      {
        foreach (UserAgentDetails.UserAgentMatcher userAgentMatcher in UserAgentDetails.s_userAgentMatchers)
        {
          Match match = userAgentMatcher.RegularExpression.Match(userAgent);
          if (match.Success)
          {
            string key = match.Groups["exeName"].Value;
            this.m_userAgentType = userAgentMatcher.UserAgentType;
            this.m_majorVersion = match.Groups["majorVersion"].Value;
            this.m_build = match.Groups["build"].Value;
            this.m_skuName = match.Groups["skuName"].Value;
            if (!string.IsNullOrEmpty(key))
            {
              UserAgentDetails.ExeNameEntry exeNameEntry;
              if (UserAgentDetails.s_exeNames.TryGetValue(key, out exeNameEntry))
              {
                this.m_userAgentType = exeNameEntry.UserAgentType;
                if (!string.IsNullOrEmpty(exeNameEntry.SkuName))
                  this.m_skuName = exeNameEntry.SkuName;
              }
              else
                this.m_userAgentType = UserAgentType.UnknownOMClient;
            }
            if (userAgentMatcher.Transform == null)
              break;
            this.m_majorVersion = userAgentMatcher.Transform(this.m_majorVersion);
            break;
          }
        }
      }
      catch
      {
      }
    }

    public bool IsBrowser
    {
      get
      {
        switch (this.m_userAgentType)
        {
          case UserAgentType.Chrome:
          case UserAgentType.ChromeMobile:
          case UserAgentType.Firefox:
          case UserAgentType.FirefoxMobile:
          case UserAgentType.InternetExplorer:
          case UserAgentType.InternetExplorerMobile:
          case UserAgentType.Safari:
          case UserAgentType.SafariMobile:
          case UserAgentType.Opera:
          case UserAgentType.OperaMobile:
          case UserAgentType.Spartan:
            return true;
          default:
            return false;
        }
      }
    }

    public UserAgentType UserAgentType => this.m_userAgentType;

    public string MajorVersion => this.m_majorVersion;

    public string Build => this.m_build;

    public string ClientSku => this.m_skuName;

    public override bool Equals(object obj) => obj is UserAgentDetails other && this.Equals(other);

    public bool Equals(UserAgentDetails other) => this.GetHashCode() == other.GetHashCode() && this.UserAgentType == other.UserAgentType && string.Equals(this.MajorVersion, other.MajorVersion, StringComparison.OrdinalIgnoreCase) && string.Equals(this.ClientSku, other.ClientSku, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode()
    {
      int userAgentType = (int) this.UserAgentType;
      if (!string.IsNullOrEmpty(this.MajorVersion))
        userAgentType ^= StringComparer.OrdinalIgnoreCase.GetHashCode(this.MajorVersion) << 11;
      if (!string.IsNullOrEmpty(this.ClientSku))
        userAgentType ^= StringComparer.OrdinalIgnoreCase.GetHashCode(this.ClientSku) << 13;
      return userAgentType;
    }

    public override string ToString() => string.Format("type={0} majorVersion={1} build={2} sku={3}", (object) this.m_userAgentType, (object) this.m_majorVersion, (object) this.m_build, (object) this.m_skuName);

    private static string TridentVersionToInternetExplorerVersion(string version)
    {
      int result;
      if (int.TryParse(version, out result))
        version = (result + 4).ToString();
      return version;
    }

    private class UserAgentMatcher
    {
      public UserAgentMatcher(UserAgentType userAgentType, string regexPattern)
        : this(userAgentType, regexPattern, (Func<string, string>) null)
      {
      }

      public UserAgentMatcher(
        UserAgentType userAgentType,
        string regexPattern,
        Func<string, string> transform)
      {
        this.UserAgentType = userAgentType;
        this.RegularExpression = new Regex(regexPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        this.Transform = transform;
      }

      public UserAgentType UserAgentType { get; private set; }

      public Regex RegularExpression { get; private set; }

      public Func<string, string> Transform { get; private set; }
    }

    private struct ExeNameEntry
    {
      public string ExeName;
      public UserAgentType UserAgentType;
      public string SkuName;

      public ExeNameEntry(string exeName, UserAgentType userAgentType, string skuName)
      {
        this.ExeName = exeName;
        this.UserAgentType = userAgentType;
        this.SkuName = skuName;
      }
    }
  }
}
