// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskVariableRestrictions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskVariableRestrictions
  {
    private List<string> m_allowed;

    public TaskVariableRestrictions()
    {
    }

    private TaskVariableRestrictions(TaskVariableRestrictions restrictionsToBeCloned) => this.m_allowed = new List<string>((IEnumerable<string>) restrictionsToBeCloned.Allowed);

    [DataMember(EmitDefaultValue = false)]
    public IList<string> Allowed
    {
      get
      {
        if (this.m_allowed == null)
          this.m_allowed = new List<string>();
        return (IList<string>) this.m_allowed;
      }
    }

    internal TaskVariableRestrictions Clone() => new TaskVariableRestrictions(this);
  }
}
