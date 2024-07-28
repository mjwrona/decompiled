// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TfsProfileConfig
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [XmlRoot("TfsProfile")]
  [Serializable]
  public class TfsProfileConfig
  {
    [XmlElement("ConnectorInfo")]
    public TfsProfileConfig.IcmConnectorInfo ConnectorInfo { get; set; }

    [XmlElement(ElementName = "HistoryFieldsToTrack")]
    public string HistoryFieldsToTrackText { get; set; }

    [XmlElement("BlackHoleMasterService", IsNullable = true)]
    public Guid? BlackHoleMasterService { get; set; }

    public bool BlackHoleMasterServiceSpecified => this.BlackHoleMasterService.HasValue;

    [XmlElement("RoutingIdFields")]
    public string RoutingIdFieldsText { get; set; }

    [XmlElement("CorrelationIdFields")]
    public string CorrelationIdFieldsText { get; set; }

    [XmlArray("TeamOwnershipFieldsMapping")]
    [XmlArrayItem("Mapping")]
    public List<TfsOwnershipMapping> TeamOwnershipFieldsMapping { get; set; }

    [XmlArray("EnvironmentMap")]
    [XmlArrayItem("Map")]
    public List<TfsProfileConfig.IcmTfsValueMap> EnvironmentMap { get; set; }

    [XmlArray("EnvironmentClusterMap")]
    [XmlArrayItem("Map")]
    public List<TfsProfileConfig.DefaultValueMap> EnvironmentClusterMap { get; set; }

    [XmlArray("HowFixedMap")]
    [XmlArrayItem("Map")]
    public List<TfsProfileConfig.IcmTfsValueMap> HowFixedMap { get; set; }

    [XmlArray("SourceMap")]
    [XmlArrayItem("Map")]
    public List<TfsProfileConfig.IcmTfsValueMap> SourceMap { get; set; }

    [XmlArray("IncidentTypeMap")]
    [XmlArrayItem("Map")]
    public List<TfsProfileConfig.IcmTfsValueMap> IncidentTypeMap { get; set; }

    [XmlArray("IncidentSubTypeMap")]
    [XmlArrayItem("Map")]
    public List<TfsProfileConfig.IcmTfsValueMap> IncidentSubTypeMap { get; set; }

    [XmlElement("PlugInInfo")]
    public TfsProfileConfig.PlugInInterfaceInfo PlugInInfo { get; set; }

    [XmlArray("CustomFieldMap", IsNullable = true)]
    [XmlArrayItem("Map")]
    public List<TfsCustomFieldMap> CustomFieldMap { get; set; }

    [XmlArray("ServicesMap")]
    [XmlArrayItem("Map")]
    public List<TfsProfileConfig.IcmTfsValueMap> ServicesMap { get; set; }

    [XmlArray("DefaultValuesInfo", IsNullable = true)]
    [XmlArrayItem("FieldInfo")]
    public List<TfsFieldDefaultInfo> DefaultValuesInfo { get; set; }

    public static TfsProfileConfig Deserialize(TextReader textReader, string source)
    {
      TfsProfileConfig tfsProfileConfig = XmlSerializerSimple.Deserialize<TfsProfileConfig>(textReader);
      if (tfsProfileConfig == null)
        throw new InvalidOperationException("Could not deserialize profile from: " + source);
      tfsProfileConfig.ThrowIfInvalid();
      return tfsProfileConfig;
    }

    public static string Serialize(TfsProfileConfig config) => XmlSerializerSimple.Serialize<TfsProfileConfig>(config);

    public void ThrowIfInvalid()
    {
      ArgumentCheck.ThrowIfNull((object) this.ConnectorInfo, "connectorInfo", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsProfileConfig.cs");
      ArgumentCheck.ThrowIfNull((object) this.TeamOwnershipFieldsMapping, "teamOwnershipFieldsMapping", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsProfileConfig.cs");
      this.ConnectorInfo.ThrowIfInvalid();
      foreach (TfsOwnershipMapping ownershipMapping in this.TeamOwnershipFieldsMapping)
        ownershipMapping.ThrowIfInvalid();
      foreach (TfsProfileConfig.IcmTfsValueMap environment in this.EnvironmentMap)
        environment.ThrowIfInvalid();
      foreach (TfsProfileConfig.DefaultValueMap environmentCluster in this.EnvironmentClusterMap)
        environmentCluster.ThrowIfInvalid();
      foreach (TfsProfileConfig.IcmTfsValueMap howFixed in this.HowFixedMap)
        howFixed.ThrowIfInvalid();
      foreach (TfsProfileConfig.IcmTfsValueMap source in this.SourceMap)
        source.ThrowIfInvalid();
      foreach (TfsProfileConfig.IcmTfsValueMap incidentType in this.IncidentTypeMap)
        incidentType.ThrowIfInvalid();
      foreach (TfsProfileConfig.IcmTfsValueMap incidentSubType in this.IncidentSubTypeMap)
        incidentSubType.ThrowIfInvalid();
      foreach (TfsProfileConfig.IcmTfsValueMap services in this.ServicesMap)
        services.ThrowIfInvalid();
      if (this.CustomFieldMap != null)
      {
        foreach (TfsCustomFieldMap customField in this.CustomFieldMap)
          TfsCustomFieldMap.ThrowIfNotValid(customField);
      }
      if (this.DefaultValuesInfo == null)
        return;
      foreach (TfsFieldDefaultInfo info in this.DefaultValuesInfo)
        TfsFieldDefaultInfo.ThrowIfInvalid(info);
    }

    public class DefaultValueMap
    {
      [XmlAttribute]
      public string Value { get; set; }

      [XmlAttribute]
      public string Default { get; set; }

      public void ThrowIfInvalid()
      {
        ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(this.Value, "value", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsProfileConfig.cs");
        ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(this.Default, "default", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsProfileConfig.cs");
      }
    }

    public class IcmTfsValueMap
    {
      [XmlAttribute("IcmValue")]
      public string IcmValue { get; set; }

      [XmlAttribute("TfsValue")]
      public string TfsValue { get; set; }

      [XmlAttribute("DefaultTfsValue")]
      public bool DefaultTfsValue { get; set; }

      [XmlAttribute("DefaultIcmValue")]
      public bool DefaultIcmValue { get; set; }

      public void ThrowIfInvalid()
      {
        ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(this.IcmValue, "icmValue", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsProfileConfig.cs");
        ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(this.TfsValue, "tfsValue", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsProfileConfig.cs");
      }
    }

    public class IcmConnectorInfo
    {
      [XmlAttribute]
      public Guid Id { get; set; }

      public Guid TenantId { get; set; }

      public string TenantName { get; set; }

      public void ThrowIfInvalid()
      {
        ArgumentCheck.ThrowIfEqualTo<Guid>(this.Id, Guid.Empty, "connectorId", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsProfileConfig.cs");
        ArgumentCheck.ThrowIfEqualTo<Guid>(this.TenantId, Guid.Empty, "tenantId", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsProfileConfig.cs");
        ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(this.TenantName, "tenantName", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsProfileConfig.cs");
      }
    }

    public class PlugInInterfaceInfo
    {
      [XmlElement(ElementName = "GetIncidentMethodName")]
      public string GetIncidentMethodNameText { get; set; }

      [XmlIgnore]
      public ConversionMethod GetIncidentMethodName => EnumTextMapperSimple<ConversionMethod>.GetEnumValue(this.GetIncidentMethodNameText);

      [XmlElement(ElementName = "UpdateTfsItemMethodName")]
      public string UpdateTfsItemMethodNameText { get; set; }

      [XmlIgnore]
      public ConversionMethod UpdateTfsItemMethodName => EnumTextMapperSimple<ConversionMethod>.GetEnumValue(this.UpdateTfsItemMethodNameText);
    }
  }
}
