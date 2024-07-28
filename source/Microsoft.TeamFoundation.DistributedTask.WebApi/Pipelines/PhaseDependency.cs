// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PhaseDependency
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PhaseDependency
  {
    [JsonConstructor]
    public PhaseDependency()
    {
    }

    private PhaseDependency(PhaseDependency dependencyToCopy)
    {
      this.Scope = dependencyToCopy.Scope;
      this.Event = dependencyToCopy.Event;
    }

    [DataMember]
    public string Scope { get; set; }

    [DataMember]
    public string Event { get; set; }

    public static implicit operator PhaseDependency(Phase dependency) => PhaseDependency.PhaseCompleted(dependency.Name);

    public static PhaseDependency PhaseCompleted(string name) => new PhaseDependency()
    {
      Scope = name,
      Event = "Completed"
    };

    internal PhaseDependency Clone() => new PhaseDependency(this);
  }
}
