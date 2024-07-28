// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.DsIcm.ShiftContactInfo
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;

namespace Microsoft.AzureAd.Icm.Types.DsIcm
{
  public class ShiftContactInfo : IEquatable<ShiftContactInfo>
  {
    public int Position { get; set; }

    public long ContactId { get; set; }

    public SubstitutionInfo SubstitutionInfo { get; set; }

    public bool Equals(ShiftContactInfo other)
    {
      ArgumentCheck.ThrowIfNull((object) other, nameof (other), nameof (Equals), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\DsIcm\\ShiftContactInfo.cs");
      return this.Position == other.Position && this.ContactId == other.ContactId && this.SubstitutionInfo == other.SubstitutionInfo;
    }
  }
}
