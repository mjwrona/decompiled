// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.MultiDomainInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [Serializable]
  public class MultiDomainInfo : IMultiDomainInfo
  {
    [JsonConstructor]
    public MultiDomainInfo(
      IDomainId domainId,
      bool isDefault,
      string region,
      string redundancyType)
      : this(domainId, isDefault, MultiDomainInfo.TryParseRegionType(region), MultiDomainInfo.TryParseRedundancyType(redundancyType))
    {
    }

    public MultiDomainInfo(
      IDomainId domainId,
      bool isDefault,
      AzureGeoRegion region,
      AzureStorageGeoRedundancyType redundancyType)
    {
      this.DomainId = domainId;
      this.IsDefault = isDefault;
      this.Region = region;
      this.RedundancyType = redundancyType;
    }

    [JsonConverter(typeof (DomainIdJsonConverter))]
    public virtual IDomainId DomainId { get; set; }

    public virtual bool IsDefault { get; set; }

    [JsonConverter(typeof (TypeConverter<AzureGeoRegion>))]
    public virtual AzureGeoRegion Region { get; set; }

    [JsonConverter(typeof (TypeConverter<AzureStorageGeoRedundancyType>))]
    public AzureStorageGeoRedundancyType RedundancyType { get; set; }

    private static AzureStorageGeoRedundancyType TryParseRedundancyType(string redundancy)
    {
      AzureStorageGeoRedundancyType result;
      if (!Enum.TryParse<AzureStorageGeoRedundancyType>(redundancy, true, out result))
        throw new InvalidEnumArgumentException("Cannot deserialize string: " + redundancy + " to type: AzureStorageGeoRedundancyType");
      return result;
    }

    private static AzureGeoRegion TryParseRegionType(string region)
    {
      AzureGeoRegion result;
      if (!Enum.TryParse<AzureGeoRegion>(region, true, out result))
        throw new InvalidEnumArgumentException("Cannot deserialize string: " + region + " to type: AzureGeoRegion");
      return result;
    }
  }
}
