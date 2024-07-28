// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.AzureOperationDetails
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling
{
  [DataContract]
  public class AzureOperationDetails
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Status { get; set; }

    [DataMember]
    public DateTime? StartTime { get; set; }

    [DataMember]
    public DateTime? EndTime { get; set; }

    public TimeSpan? Duration => !this.StartTime.HasValue || !this.EndTime.HasValue ? new TimeSpan?() : new TimeSpan?(this.EndTime.Value - this.StartTime.Value);

    public AzureOperationDetails()
    {
    }

    public AzureOperationDetails(string responseJson)
      : this(JObject.Parse(responseJson))
    {
    }

    public AzureOperationDetails(JObject response)
    {
      JToken jtoken1;
      DateTime result1;
      if (response.TryGetValue("startTime", out jtoken1) && DateTime.TryParse(jtoken1.ToObject<string>(), out result1))
        this.StartTime = new DateTime?(result1);
      JToken jtoken2;
      DateTime result2;
      if (response.TryGetValue("endTime", out jtoken2) && DateTime.TryParse(jtoken2.ToObject<string>(), out result2))
        this.EndTime = new DateTime?(result2);
      JToken jtoken3;
      if (response.TryGetValue("status", out jtoken3))
        this.Status = jtoken3.ToObject<string>();
      JToken jtoken4;
      if (!response.TryGetValue("name", out jtoken4))
        return;
      this.Name = jtoken4.ToObject<string>();
    }

    public override string ToString() => string.Format("[AzureOperationDetails] startTime: {0}, endTime: {1}, status: {2}, name: {3}", (object) this.StartTime, (object) this.EndTime, (object) this.Status, (object) this.Name);
  }
}
