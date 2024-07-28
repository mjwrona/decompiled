// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.PersistedStage
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public class PersistedStage
  {
    [DataMember(EmitDefaultValue = false)]
    public long Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int DefinitionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PersistedStageGroup Group { get; set; }

    public PersistedStageReference GetPersistedStageReference()
    {
      PersistedStageReference persistedStageReference1 = new PersistedStageReference();
      persistedStageReference1.Id = this.Id;
      persistedStageReference1.Name = (ExpressionValue<string>) this.Name;
      persistedStageReference1.DefinitionId = this.DefinitionId;
      PersistedStageReference persistedStageReference2 = persistedStageReference1;
      if (this.Group != null)
      {
        persistedStageReference2.GroupPath = this.Group.Path;
        persistedStageReference2.BuildId = this.Group.BuildId;
      }
      return persistedStageReference2;
    }
  }
}
