// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.Api.TeamOnCallSubstitution
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Collections.Generic;

namespace Microsoft.AzureAd.Icm.Types.Api
{
  public class TeamOnCallSubstitution
  {
    private static readonly IList<OnCallSubstitutionSlot> EmptySlotList = (IList<OnCallSubstitutionSlot>) new List<OnCallSubstitutionSlot>();

    public TeamOnCallSubstitution() => this.SubstitutionSlots = TeamOnCallSubstitution.EmptySlotList;

    public long TeamId { get; set; }

    public IList<OnCallSubstitutionSlot> SubstitutionSlots { get; set; }
  }
}
