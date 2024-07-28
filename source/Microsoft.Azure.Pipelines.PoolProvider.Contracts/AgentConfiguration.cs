// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.PoolProvider.Contracts.AgentConfiguration
// Assembly: Microsoft.Azure.Pipelines.PoolProvider.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2F8171A-7EDF-4EAC-B6BB-DAF285412F1E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.PoolProvider.Contracts.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.PoolProvider.Contracts
{
  [DataContract]
  public class AgentConfiguration
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false, Name = "AgentSettings")]
    private Dictionary<string, string> m_settings;
    [DataMember(IsRequired = true, EmitDefaultValue = false, Name = "AgentDownloadUrls")]
    private Dictionary<string, string> m_downloadUrls;

    public Dictionary<string, string> AgentSettings
    {
      get
      {
        if (this.m_settings == null)
          this.m_settings = new Dictionary<string, string>();
        return this.m_settings;
      }
      set => this.m_settings = value;
    }

    [DataMember]
    public AgentCredentialData AgentCredentials { get; set; }

    [DataMember]
    public string AgentVersion { get; set; }

    [DataMember]
    public string MinimumAgentVersion { get; set; }

    public Dictionary<string, string> AgentDownloadUrls
    {
      get
      {
        if (this.m_downloadUrls == null)
          this.m_downloadUrls = new Dictionary<string, string>();
        return this.m_downloadUrls;
      }
      set => this.m_downloadUrls = value;
    }
  }
}
