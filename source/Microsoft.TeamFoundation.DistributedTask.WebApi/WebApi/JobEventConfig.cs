// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.JobEventConfig
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class JobEventConfig
  {
    private string m_timeout;

    public JobEventConfig(string timeout) => this.m_timeout = timeout;

    [DataMember(Name = "Timeout")]
    public string Timeout
    {
      get
      {
        if (this.m_timeout == null)
          this.m_timeout = string.Empty;
        return this.m_timeout;
      }
    }
  }
}
