// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.PoolProvider.Contracts.AgentCredentialData
// Assembly: Microsoft.Azure.Pipelines.PoolProvider.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2F8171A-7EDF-4EAC-B6BB-DAF285412F1E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.PoolProvider.Contracts.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.PoolProvider.Contracts
{
  [DataContract]
  public class AgentCredentialData
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false, Name = "Data")]
    private Dictionary<string, string> m_data;

    [DataMember]
    public string Scheme { get; set; }

    public Dictionary<string, string> Data
    {
      get
      {
        if (this.m_data == null)
          this.m_data = new Dictionary<string, string>();
        return this.m_data;
      }
    }
  }
}
