// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.TfsFieldDefaultInfo
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [Serializable]
  public class TfsFieldDefaultInfo
  {
    [XmlElement("TfsFieldName")]
    public string TfsFieldName { get; set; }

    [XmlElement("Type")]
    public TfsCustomFieldType Type { get; set; }

    [XmlElement("DefaultValues")]
    public string DefaultValues { get; set; }

    public static void ThrowIfInvalid(TfsFieldDefaultInfo info)
    {
      ArgumentCheck.ThrowIfNull((object) info, nameof (info), nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsFieldDefaultInfo.cs");
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(info.TfsFieldName, "info.TfsFieldName", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsFieldDefaultInfo.cs");
      ArgumentCheck.ThrowIfNullEmptyOrWhiteSpace(info.DefaultValues, "info.DefaultValues", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsFieldDefaultInfo.cs");
      ArgumentCheck.ThrowIfEqualTo<TfsCustomFieldType>(info.Type, TfsCustomFieldType.Custom, "Default values of type custom are not expected.", nameof (ThrowIfInvalid), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\Connectors\\TfsFieldDefaultInfo.cs");
    }
  }
}
