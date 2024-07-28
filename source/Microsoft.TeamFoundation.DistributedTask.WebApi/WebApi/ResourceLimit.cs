// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ResourceLimit
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class ResourceLimit
  {
    [DataMember(Name = "ResourceLimitsData", EmitDefaultValue = false)]
    private IDictionary<string, string> m_resourceLimitsData;

    public ResourceLimit(Guid hostId, string parallelismTag, bool isHosted)
    {
      this.HostId = hostId;
      this.ParallelismTag = parallelismTag;
      this.IsHosted = isHosted;
    }

    [DataMember]
    public Guid HostId { get; set; }

    [DataMember]
    public string ParallelismTag { get; set; }

    [DataMember]
    public bool IsHosted { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? TotalCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? TotalMinutes { get; set; }

    [DataMember]
    public bool IsPremium { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool FailedToReachAllProviders { get; set; }

    public IDictionary<string, string> Data
    {
      get
      {
        if (this.m_resourceLimitsData == null)
          this.m_resourceLimitsData = (IDictionary<string, string>) new Dictionary<string, string>();
        return this.m_resourceLimitsData;
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      IDictionary<string, string> resourceLimitsData = this.m_resourceLimitsData;
      if ((resourceLimitsData != null ? (resourceLimitsData.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_resourceLimitsData = (IDictionary<string, string>) null;
    }
  }
}
