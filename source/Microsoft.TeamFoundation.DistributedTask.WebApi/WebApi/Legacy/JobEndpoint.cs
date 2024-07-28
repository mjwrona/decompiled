// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy.JobEndpoint
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy
{
  [DataContract]
  [JsonConverter(typeof (LegacyJsonConverter<JobEndpoint>))]
  public sealed class JobEndpoint
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Data")]
    private Dictionary<string, string> m_data;

    public JobEndpoint()
    {
    }

    private JobEndpoint(JobEndpoint endpointToClone)
    {
      this.Id = endpointToClone.Id;
      this.Name = endpointToClone.Name;
      this.Type = endpointToClone.Type;
      this.Authorization = endpointToClone.Authorization;
      this.Url = endpointToClone.Url;
      if (endpointToClone.m_data == null)
        return;
      this.m_data = new Dictionary<string, string>((IDictionary<string, string>) endpointToClone.m_data, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember(EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Authorization { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    public IDictionary<string, string> Data
    {
      get
      {
        if (this.m_data == null)
          this.m_data = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, string>) this.m_data;
      }
    }

    public JobEndpoint Clone() => new JobEndpoint(this);
  }
}
