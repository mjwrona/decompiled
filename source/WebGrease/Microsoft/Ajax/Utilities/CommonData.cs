// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.CommonData
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Text.RegularExpressions;

namespace Microsoft.Ajax.Utilities
{
  internal static class CommonData
  {
    private static Regex s_replacementToken;
    private static Regex s_decimalFormat;

    public static Regex ReplacementToken
    {
      get
      {
        if (CommonData.s_replacementToken == null)
          CommonData.s_replacementToken = new Regex("%(?<token>[\\w\\.-]+)(?:\\:(?<fallback>\\w*))?%", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        return CommonData.s_replacementToken;
      }
    }

    public static Regex DecimalFormat
    {
      get
      {
        if (CommonData.s_decimalFormat == null)
          CommonData.s_decimalFormat = new Regex("^\\s*(?:\\+|(?<neg>\\-))?0*(?<mag>(?<sig>\\d*[1-9])(?<zer>0*))?(\\.(?<man>\\d*[1-9])?0*)?(?<exp>E\\+?(?<eng>\\-?)0*(?<pow>[1-9]\\d*))?$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);
        return CommonData.s_decimalFormat;
      }
    }
  }
}
