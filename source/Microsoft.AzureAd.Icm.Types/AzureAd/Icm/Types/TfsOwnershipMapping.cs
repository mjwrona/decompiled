// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TfsOwnershipMapping
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  public class TfsOwnershipMapping
  {
    private const StringSplitOptions Sso = StringSplitOptions.RemoveEmptyEntries;
    private static readonly char[] Sep = new char[1]{ ',' };

    [XmlAttribute]
    public string PublicId { get; set; }

    [XmlElement("Query")]
    public TfsQuery Query { get; set; }

    [XmlArray("Ownership")]
    [XmlArrayItem("Field")]
    public List<TfsFieldMap> Fields { get; set; }

    [XmlElement("RoutingIdFields")]
    public string RoutingIdFieldsText { get; set; }

    [XmlIgnore]
    public ICollection<string> RoutingIdFields => TfsOwnershipMapping.SplitCsv(this.RoutingIdFieldsText);

    [XmlElement("CorrelationIdFields")]
    public string CorrelationIdFieldsText { get; set; }

    [XmlIgnore]
    public ICollection<string> CorrelationIdFields => TfsOwnershipMapping.SplitCsv(this.CorrelationIdFieldsText);

    [XmlElement("TrackingTeam")]
    public string TrackingTeam { get; set; }

    public void ThrowIfInvalid()
    {
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(this.PublicId, "publicId", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsOwnershipMapping.cs");
      if (this.Query != null)
        this.Query.ThrowIfInvalid();
      if (this.TrackingTeam == null)
        return;
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(this.TrackingTeam, "trackingTeam", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsOwnershipMapping.cs");
    }

    private static ICollection<string> SplitCsv(string text) => text != null ? (ICollection<string>) text.Split(TfsOwnershipMapping.Sep, StringSplitOptions.RemoveEmptyEntries) : (ICollection<string>) null;
  }
}
