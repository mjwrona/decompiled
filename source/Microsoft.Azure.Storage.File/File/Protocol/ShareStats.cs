// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.ShareStats
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using System;
using System.Globalization;
using System.Xml.Linq;

namespace Microsoft.Azure.Storage.File.Protocol
{
  public sealed class ShareStats
  {
    private const string ShareStatsName = "ShareStats";
    private const string ShareUsageBytes = "ShareUsageBytes";

    private ShareStats()
    {
    }

    public int Usage { get; private set; }

    public long UsageInBytes { get; private set; }

    internal static ShareStats FromServiceXml(XDocument shareStatsDocument)
    {
      long num1 = long.Parse(shareStatsDocument.Element((XName) nameof (ShareStats)).Element((XName) "ShareUsageBytes").Value, (IFormatProvider) CultureInfo.InvariantCulture);
      int num2 = (int) Math.Ceiling((double) num1 / 1073741824.0);
      return new ShareStats()
      {
        Usage = num2,
        UsageInBytes = num1
      };
    }
  }
}
