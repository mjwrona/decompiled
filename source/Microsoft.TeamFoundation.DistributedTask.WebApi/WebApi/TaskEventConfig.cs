// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskEventConfig
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [ClientIgnore]
  [DataContract]
  public class TaskEventConfig
  {
    [DataMember(Name = "Enabled")]
    private JValue m_enabled;
    [DataMember(Name = "Timeout")]
    private string m_timeout;

    [JsonConstructor]
    public TaskEventConfig()
    {
    }

    public TaskEventConfig(string timeout, bool enabled = true)
    {
      this.m_timeout = timeout;
      this.m_enabled = new JValue(enabled);
    }

    public string Timeout
    {
      get
      {
        if (this.m_timeout == null)
          this.m_timeout = TimeSpan.Zero.ToString();
        return this.m_timeout;
      }
    }

    internal JValue Enabled
    {
      get
      {
        if (this.m_enabled == null)
          this.m_enabled = new JValue(true);
        return this.m_enabled;
      }
      set => this.m_enabled = value;
    }
  }
}
