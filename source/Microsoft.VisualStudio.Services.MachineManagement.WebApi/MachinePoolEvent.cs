// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePoolEvent
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachinePoolEvent
  {
    private IDictionary<string, object> m_content;

    internal MachinePoolEvent() => this.EventId = Guid.NewGuid();

    public MachinePoolEvent(Guid id, string name)
    {
      this.EventId = id;
      this.EventName = name;
    }

    [DataMember(IsRequired = true)]
    public Guid EventId { get; internal set; }

    [DataMember(IsRequired = true)]
    public string EventName { get; internal set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public string InstanceName { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public DateTime? CreatedOn { get; set; }

    [DataMember(IsRequired = false)]
    public IDictionary<string, object> Content
    {
      get
      {
        if (this.m_content == null)
          this.m_content = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_content;
      }
      internal set => this.m_content = value;
    }

    [IgnoreDataMember]
    internal string PoolType { get; set; }

    [IgnoreDataMember]
    internal string PoolName { get; set; }
  }
}
