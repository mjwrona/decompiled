// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.JobOption
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class JobOption : ICloneable
  {
    private Dictionary<string, string> m_data;
    [DataMember(Name = "Data", EmitDefaultValue = false)]
    private IDictionary<string, string> m_serializedData;

    public JobOption()
    {
    }

    private JobOption(JobOption optionToClone)
    {
      this.Id = optionToClone.Id;
      if (optionToClone.m_data == null)
        return;
      this.m_data = new Dictionary<string, string>((IDictionary<string, string>) optionToClone.m_data, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember]
    public Guid Id { get; set; }

    public IDictionary<string, string> Data
    {
      get
      {
        if (this.m_data == null)
          this.m_data = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (IDictionary<string, string>) this.m_data;
      }
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      if (this.m_serializedData != null && this.m_serializedData.Count > 0)
        this.m_data = new Dictionary<string, string>(this.m_serializedData, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_serializedData = (IDictionary<string, string>) null;
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => this.m_serializedData = this.Data.Count > 0 ? this.Data : (IDictionary<string, string>) null;

    object ICloneable.Clone() => (object) this.Clone();

    public JobOption Clone() => new JobOption(this);
  }
}
