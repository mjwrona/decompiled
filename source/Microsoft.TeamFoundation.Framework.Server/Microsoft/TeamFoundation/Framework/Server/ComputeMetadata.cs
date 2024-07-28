// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ComputeMetadata
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ComputeMetadata
  {
    [JsonProperty(PropertyName = "azEnvironment")]
    public string AzEnvironment { get; set; }

    [JsonProperty(PropertyName = "location")]
    public string Location { get; set; }

    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "offer")]
    public string Offer { get; set; }

    [JsonProperty(PropertyName = "osType")]
    public string OsType { get; set; }

    [JsonProperty(PropertyName = "resourceGroupName")]
    public string ResourceGroupName { get; set; }

    [JsonProperty(PropertyName = "sku")]
    public string Sku { get; set; }

    [JsonProperty(PropertyName = "subscriptionId")]
    public string SubscriptionId { get; set; }

    [JsonProperty(PropertyName = "vmSize")]
    public string VmSize { get; set; }

    [JsonProperty(PropertyName = "zone")]
    public string Zone { get; set; }

    public static ComputeMetadata Parse(string input)
    {
      try
      {
        return JsonUtilities.Deserialize<ComputeMetadata>(input);
      }
      catch (Exception ex)
      {
        throw new FormatException("Could not parse compute metadata", ex);
      }
    }
  }
}
