// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentSession
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgentSession
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "SystemCapabilities")]
    private IDictionary<string, string> m_systemCapabilities;

    public TaskAgentSession()
    {
    }

    public TaskAgentSession(string ownerName, TaskAgentReference agent)
    {
      this.Agent = agent;
      this.OwnerName = ownerName;
    }

    public TaskAgentSession(
      string ownerName,
      TaskAgentReference agent,
      IDictionary<string, string> systemCapabilities)
    {
      this.Agent = agent;
      this.OwnerName = ownerName;
      foreach (KeyValuePair<string, string> systemCapability in (IEnumerable<KeyValuePair<string, string>>) systemCapabilities)
      {
        if (systemCapability.Value != null)
          this.SystemCapabilities.Add(systemCapability.Key, systemCapability.Value);
      }
    }

    [DataMember]
    public Guid SessionId { get; internal set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskAgentSessionKey EncryptionKey { get; internal set; }

    [DataMember]
    public string OwnerName { get; set; }

    [DataMember]
    public TaskAgentReference Agent { get; set; }

    public IDictionary<string, string> SystemCapabilities
    {
      get
      {
        if (this.m_systemCapabilities == null)
          this.m_systemCapabilities = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_systemCapabilities;
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      IDictionary<string, string> systemCapabilities = this.m_systemCapabilities;
      if ((systemCapabilities != null ? (systemCapabilities.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_systemCapabilities = (IDictionary<string, string>) null;
    }
  }
}
