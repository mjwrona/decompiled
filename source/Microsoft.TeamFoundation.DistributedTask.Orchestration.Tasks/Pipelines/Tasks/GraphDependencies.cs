// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.GraphDependencies
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [DataContract]
  public class GraphDependencies
  {
    [DataMember(Name = "Satisfied", EmitDefaultValue = false)]
    private HashSet<string> m_satisfied;
    [DataMember(Name = "Unsatisfied", EmitDefaultValue = false)]
    private HashSet<string> m_unsatisfied;

    public ISet<string> Satisfied
    {
      get
      {
        if (this.m_satisfied == null)
          this.m_satisfied = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (ISet<string>) this.m_satisfied;
      }
    }

    public ISet<string> Unsatisfied
    {
      get
      {
        if (this.m_unsatisfied == null)
          this.m_unsatisfied = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (ISet<string>) this.m_unsatisfied;
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      HashSet<string> satisfied = this.m_satisfied;
      // ISSUE: explicit non-virtual call
      if ((satisfied != null ? (__nonvirtual (satisfied.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_satisfied = (HashSet<string>) null;
      HashSet<string> unsatisfied = this.m_unsatisfied;
      // ISSUE: explicit non-virtual call
      if ((unsatisfied != null ? (__nonvirtual (unsatisfied.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_unsatisfied = (HashSet<string>) null;
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      if (this.m_satisfied != null)
        this.m_satisfied = new HashSet<string>((IEnumerable<string>) this.m_satisfied, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (this.m_unsatisfied == null)
        return;
      this.m_unsatisfied = new HashSet<string>((IEnumerable<string>) this.m_unsatisfied, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
