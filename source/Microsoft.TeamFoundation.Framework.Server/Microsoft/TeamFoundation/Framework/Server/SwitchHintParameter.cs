// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SwitchHintParameter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SwitchHintParameter
  {
    private SwitchHintParameter(string switchType, string accountName)
    {
      this.SwitchType = switchType;
      this.AccountName = accountName;
    }

    public string SwitchType { get; }

    public string AccountName { get; }

    public override string ToString() => this.AccountName == null ? this.SwitchType : this.SwitchType + "," + this.AccountName;

    private static bool IsSwitchTypeValid(char switchType) => Array.IndexOf<char>(new char[2]
    {
      'P',
      'W'
    }, switchType) >= 0;

    public static SwitchHintParameter FromString(string switchHintStr)
    {
      if (string.IsNullOrEmpty(switchHintStr))
        return (SwitchHintParameter) null;
      if (switchHintStr.Length > 1 && switchHintStr[1] == '%')
        switchHintStr = HttpUtility.UrlDecode(switchHintStr);
      if (!SwitchHintParameter.IsSwitchTypeValid(switchHintStr[0]))
        return (SwitchHintParameter) null;
      string switchType = switchHintStr[0].ToString();
      string str = (string) null;
      if (switchHintStr.Length > 3 && switchHintStr[1] == ',')
        str = switchHintStr.Substring(2);
      string accountName = str;
      return new SwitchHintParameter(switchType, accountName);
    }

    public static SwitchHintParameter PersonalWithAccountName(string accountName) => new SwitchHintParameter('P'.ToString(), accountName);

    public bool ShouldSwitchToPersonalAccount() => 'P'.ToString().Equals(this.SwitchType);

    public bool ShouldSwitchToWorkAccount() => 'W'.ToString().Equals(this.SwitchType);
  }
}
