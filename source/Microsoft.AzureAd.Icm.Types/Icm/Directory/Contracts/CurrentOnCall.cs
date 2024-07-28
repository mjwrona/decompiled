// Decompiled with JetBrains decompiler
// Type: Microsoft.Icm.Directory.Contracts.CurrentOnCall
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Collections.Generic;

namespace Microsoft.Icm.Directory.Contracts
{
  public class CurrentOnCall
  {
    public CurrentOnCall() => this.ShiftCurrentOnCalls = (IList<ShiftOnCall>) new List<ShiftOnCall>();

    public long TeamId { get; set; }

    public string TimeZoneId { get; set; }

    public string TeamName { get; set; }

    public string TeamEmailAddress { get; set; }

    public long? ReferencedTeamId { get; set; }

    public IList<ShiftOnCall> ShiftCurrentOnCalls { get; set; }
  }
}
