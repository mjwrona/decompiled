// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.WebApi.Commerce.RestApiResponseStatusModel
// Assembly: Microsoft.VisualStudio.Services.Gallery.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EE9D0AAA-B110-4AD6-813B-50FA04AC401A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Gallery.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.WebApi.Commerce
{
  [DataContract]
  public class RestApiResponseStatusModel
  {
    [JsonProperty(Required = Required.Always)]
    [DataMember]
    public string OperationId { get; set; }

    [JsonProperty(Required = Required.Always)]
    [DataMember]
    public RestApiResponseStatus Status { get; set; }

    [JsonProperty(Required = Required.Always)]
    [DataMember]
    public string StatusMessage { get; set; }

    [JsonProperty(Required = Required.Always)]
    [DataMember]
    public int PercentageCompleted { get; set; }

    [JsonProperty(Required = Required.Default)]
    [DataMember]
    public JToken OperationDetails { get; set; }
  }
}
