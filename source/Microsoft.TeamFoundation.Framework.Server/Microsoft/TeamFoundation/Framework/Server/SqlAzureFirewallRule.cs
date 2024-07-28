// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlAzureFirewallRule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class SqlAzureFirewallRule
  {
    public string Name { get; set; }

    public string StartIpAddress { get; set; }

    public string EndIpAddress { get; set; }

    public DateTime CreateDate { get; set; }

    public DateTime ModifyDate { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Name: {0}, StartIpAddress: {1}, EndIpAddress: {2}, CreateDate: {3}, ModifyDate: {4}", (object) this.Name, (object) this.StartIpAddress, (object) this.EndIpAddress, (object) this.CreateDate, (object) this.ModifyDate);
  }
}
