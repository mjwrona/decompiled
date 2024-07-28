// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.AlertSourceInfo
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class AlertSourceInfo : IEquatable<AlertSourceInfo>
  {
    [DataMember]
    public Guid SourceId { get; set; }

    [DataMember]
    public string Origin { get; set; }

    [DataMember]
    public string CreatedBy { get; set; }

    [DataMember]
    public DateTime CreateDate { get; set; }

    [DataMember]
    public string IncidentId { get; set; }

    [DataMember]
    public DateTime ModifiedDate { get; set; }

    [DataMember]
    public int? Revision { get; set; }

    public static void ThrowIfNotValid(AlertSourceInfo source)
    {
      ArgumentCheck.ThrowIfNull((object) source, nameof (source), nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceInfo.cs");
      TypeUtility.ValidateStringParameter(source.IncidentId, "source.IncidentId", 1, 128);
      TypeUtility.ValidateStringParameter(source.CreatedBy, "source.CreatedBy", 1, 128);
      TypeUtility.ValidateStringParameter(source.Origin, "source.Origin", 1, 128);
      ArgumentCheck.ThrowIfNotInRangeInclusive<DateTime>(source.CreateDate, IcmConstants.Dates.MinDate, IcmConstants.Dates.MaxDate, "source.CreateDate", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceInfo.cs");
      ArgumentCheck.ThrowIfNotInRangeInclusive<DateTime>(source.ModifiedDate, IcmConstants.Dates.MinDate, IcmConstants.Dates.MaxDate, "source.ModifiedDate", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceInfo.cs");
      ArgumentCheck.ThrowIfEqualTo<Guid>(source.SourceId, Guid.Empty, "source.SourceId", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Incidents\\AlertSourceInfo.cs");
    }

    public bool Equals(AlertSourceInfo other)
    {
      if (other == null)
        return false;
      DateTime dateTime = this.CreateDate;
      if (dateTime.Equals(other.CreateDate) && string.Equals(this.CreatedBy, other.CreatedBy, StringComparison.OrdinalIgnoreCase) && string.Equals(this.IncidentId, other.IncidentId, StringComparison.OrdinalIgnoreCase))
      {
        dateTime = this.ModifiedDate;
        if (dateTime.Equals(other.ModifiedDate) && string.Equals(this.Origin, other.Origin, StringComparison.OrdinalIgnoreCase) && this.Revision.Equals((object) other.Revision))
          return this.SourceId.Equals(other.SourceId);
      }
      return false;
    }
  }
}
