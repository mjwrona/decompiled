// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.SendJobResponse
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class SendJobResponse
  {
    private IDictionary<string, string> m_variables;
    private JobEventsConfig m_jobEventsConfig;

    [JsonConstructor]
    public SendJobResponse()
    {
    }

    [DataMember]
    public JobEventsConfig Events
    {
      get
      {
        if (this.m_jobEventsConfig == null)
          this.m_jobEventsConfig = new JobEventsConfig();
        return this.m_jobEventsConfig;
      }
      private set => this.m_jobEventsConfig = value;
    }

    [DataMember]
    public IDictionary<string, string> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = (IDictionary<string, string>) new Dictionary<string, string>();
        return this.m_variables;
      }
    }

    public SendJobResponse(IDictionary<string, string> variables, JobEventsConfig eventsConfig)
    {
      this.m_variables = variables;
      this.m_jobEventsConfig = eventsConfig;
    }
  }
}
