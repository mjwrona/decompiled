// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TfsCustomFieldMap
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  public class TfsCustomFieldMap
  {
    [XmlArray("CustomFields")]
    [XmlArrayItem("CustomField")]
    public List<TfsCustomField> Fields { get; set; }

    [XmlElement("ContainerName")]
    public string ContainerName { get; set; }

    [XmlElement("ContainerPublicId", IsNullable = true)]
    public Guid? ContainerPublicId { get; set; }

    [XmlElement("ContainerType")]
    public IncidentCustomFieldContainerType ContainerType { get; set; }

    public static void ThrowIfNotValid(TfsCustomFieldMap map)
    {
      ArgumentCheck.ThrowIfNull((object) map, nameof (map), nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsCustomFieldMap.cs");
      ArgumentCheck.ThrowIfCollectionNullOrEmpty<TfsCustomField>((IEnumerable<TfsCustomField>) map.Fields, "map.Fields", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsCustomFieldMap.cs");
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(map.ContainerName, "map.ContainerName", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsCustomFieldMap.cs");
      ArgumentCheck.ThrowIfEqualTo<Guid>(map.ContainerPublicId, Guid.Empty, "map.ContainerPublicId", nameof (ThrowIfNotValid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsCustomFieldMap.cs");
      foreach (TfsCustomField field in map.Fields)
        TfsCustomField.ThrowIfInvalid(field);
    }
  }
}
