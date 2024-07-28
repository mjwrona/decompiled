// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PersistedStageReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PersistedStageReference : ResourceReference
  {
    public PersistedStageReference()
    {
    }

    private PersistedStageReference(PersistedStageReference referenceToCopy)
      : base((ResourceReference) referenceToCopy)
    {
      this.Id = referenceToCopy.Id;
      this.DefinitionId = referenceToCopy.DefinitionId;
      this.GroupPath = referenceToCopy.GroupPath;
      this.BuildId = referenceToCopy.BuildId;
    }

    [DataMember(EmitDefaultValue = false)]
    public long Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int DefinitionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string GroupPath { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int BuildId { get; set; }

    public PersistedStageReference Clone() => new PersistedStageReference(this);

    public override string ToString() => base.ToString() ?? string.Format("[DefinitionId: {0}, StageId: {1}, StageName: {2}]", (object) this.DefinitionId, (object) this.Id, (object) this.Name);
  }
}
