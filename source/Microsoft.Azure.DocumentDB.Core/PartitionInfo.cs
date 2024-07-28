// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.PartitionInfo
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal sealed class PartitionInfo : Resource
  {
    [JsonProperty(PropertyName = "resourceType")]
    public string ResourceType
    {
      get => this.GetValue<string>("resourceType");
      set => this.SetValue("resourceType", (object) value);
    }

    [JsonProperty(PropertyName = "serviceIndex")]
    public int ServiceIndex
    {
      get => this.GetValue<int>("serviceIndex");
      set => this.SetValue("serviceIndex", (object) value);
    }

    [JsonProperty(PropertyName = "partitionIndex")]
    public int PartitionIndex
    {
      get => this.GetValue<int>("partitionIndex");
      set => this.SetValue("partitionIndex", (object) value);
    }

    public override bool Equals(object obj) => obj is PartitionInfo partitionInfo && partitionInfo.ResourceType == this.ResourceType && partitionInfo.PartitionIndex == this.PartitionIndex && partitionInfo.ServiceIndex == this.ServiceIndex;

    public override int GetHashCode() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}@{1}@{2}", (object) this.ResourceType, (object) this.PartitionIndex, (object) this.ServiceIndex).GetHashCode();
  }
}
