// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.BrowserFilterEntry
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class BrowserFilterEntry
  {
    private TimeSpan RegexMatchTimeout = TimeSpan.FromSeconds(30.0);

    public string Browser { get; set; }

    public string Exception { get; set; }

    public int? MajorVersion { get; set; }

    public double? MinorVersion { get; set; }

    public bool? Below { get; set; }

    public bool RegexMatch { get; set; }

    private int CompareVersions(int major, double minor)
    {
      if (this.MajorVersion.HasValue)
      {
        if (major != this.MajorVersion.Value)
          return major <= this.MajorVersion.Value ? -1 : 1;
        if (this.MinorVersion.HasValue && minor != this.MinorVersion.Value)
          return minor <= this.MinorVersion.Value ? -1 : 1;
      }
      return 0;
    }

    public bool IsMatch(
      string browserName,
      int majorVersion,
      double minorVersion,
      string userAgent)
    {
      if (this.RegexMatch)
        return Regex.IsMatch(userAgent, this.Browser, RegexOptions.None, this.RegexMatchTimeout);
      if (!string.IsNullOrEmpty(this.Exception) && userAgent != null && userAgent.Contains(this.Exception) || !StringComparer.OrdinalIgnoreCase.Equals(browserName, this.Browser))
        return false;
      if (this.MajorVersion.HasValue)
      {
        if (majorVersion == 0 && minorVersion == 0.0)
          return false;
        int num = this.CompareVersions(majorVersion, minorVersion);
        if (this.Below.HasValue)
        {
          if (this.Below.Value)
          {
            if (num > 0)
              return false;
          }
          else if (num < 0)
            return false;
        }
        else if (num != 0)
          return false;
      }
      return true;
    }

    public override string ToString()
    {
      string str1 = this.Browser;
      if (!this.RegexMatch && this.MajorVersion.HasValue)
      {
        str1 = str1 + "/" + this.MajorVersion.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
        double? minorVersion = this.MinorVersion;
        if (minorVersion.HasValue)
        {
          string str2 = str1;
          minorVersion = this.MinorVersion;
          string str3 = minorVersion.Value.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
          str1 = str2 + str3;
        }
        if (this.Below.HasValue)
          str1 = !this.Below.Value ? str1 + "+" : str1 + "-";
      }
      return str1;
    }
  }
}
