// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.EuiiUtility
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class EuiiUtility
  {
    private static readonly Regex s_emailRegex = new Regex("[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private const string c_emailPattern = "[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}";
    private static readonly Regex c_emailMaskRegex = new Regex("(?!^|.@|(?<=@)|.$)[^@\\.\\s]", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private const string c_emailMaskPattern = "(?!^|.@|(?<=@)|.$)[^@\\.\\s]";
    private static readonly Regex s_ipAddressRegex = new Regex("\\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\b", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private const string c_ipAddressPattern = "\\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\b";
    private const string c_euiiMask = "******";

    public static bool ContainsEmail(string message, bool assertOnDetection = true) => message.Contains("@") && EuiiUtility.PatternMatch(message, EuiiUtility.s_emailRegex, assertOnDetection);

    public static bool ContainsIpAddress(string message, bool assertOnDetection = false) => EuiiUtility.PatternMatch(message, EuiiUtility.s_ipAddressRegex, assertOnDetection);

    public static string MaskEmail(string message)
    {
      if (!message.Contains("@"))
        return message;
      MatchCollection matchCollection = EuiiUtility.s_emailRegex.Matches(message);
      string str1 = message;
      foreach (object obj in matchCollection)
      {
        string str2 = obj.ToString();
        string newValue = EuiiUtility.c_emailMaskRegex.Replace(str2, "*");
        str1 = str1.Replace(str2, newValue);
      }
      return str1;
    }

    private static bool PatternMatch(string message, Regex pattern, bool assertOnDetection)
    {
      Match match = pattern.Match(message);
      if (match.Success & assertOnDetection)
        throw new EUIILeakException(pattern.Replace(message, "******"));
      return match.Success;
    }
  }
}
