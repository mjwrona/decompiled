// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TfsCustomField
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  public class TfsCustomField
  {
    [XmlElement("DefaultValue")]
    public string DefaultValue { get; set; }

    [XmlElement("IcmFieldName")]
    public string IcmFieldName { get; set; }

    [XmlElement("TfsFieldName")]
    public string TfsFieldName { get; set; }

    [XmlElement("CustomActionType")]
    public CustomFieldCustomActionType CustomActionType { get; set; }

    [XmlElement("AllowedValues")]
    public string AllowedValues { get; set; }

    [XmlElement("Type")]
    public TfsCustomFieldType Type { get; set; }

    public static void ThrowIfInvalid(TfsCustomField customField)
    {
      ArgumentCheck.ThrowIfNull((object) customField, nameof (customField), nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsCustomField.cs");
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(customField.IcmFieldName, "customField.IcmFieldName", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsCustomField.cs");
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(customField.TfsFieldName, "customField.TfsFieldName", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsCustomField.cs");
      if (customField.Type == TfsCustomFieldType.Custom && customField.CustomActionType == CustomFieldCustomActionType.Invalid)
        throw new ArgumentException("Custom field of type Custom must specify a custom action type.");
    }
  }
}
