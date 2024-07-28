// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.EventCounts
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi
{
  [DataContract]
  public class EventCounts
  {
    [DataMember]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public long WebPageViews;
    [DataMember]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public long InstallCount;
    [DataMember]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public long WebDownloadCount;
    [DataMember]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int UninstallCount;
    [DataMember]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int BuyCount;
    [DataMember]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int TryCount;
    [DataMember]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int ConnectedInstallCount;
    [DataMember]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int ConnectedBuyCount;
    [DataMember]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public float AverageRating;
  }
}
