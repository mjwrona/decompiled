// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime.StageInstance
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class StageInstance : GraphNodeInstance<Stage>
  {
    public StageInstance()
    {
    }

    public StageInstance(string name)
      : this(name, TaskResult.Succeeded)
    {
    }

    public StageInstance(string name, int attempt)
      : this(name, attempt, (Stage) null, TaskResult.Succeeded)
    {
    }

    public StageInstance(Stage stage)
      : this(stage, 1)
    {
    }

    public StageInstance(Stage stage, int attempt)
      : this(stage.Name, attempt, stage, TaskResult.Succeeded)
    {
    }

    public StageInstance(string name, TaskResult result)
      : this(name, 1, (Stage) null, result)
    {
    }

    public StageInstance(string name, int attempt, Stage definition, TaskResult result)
      : base(name, attempt, definition, result)
    {
    }

    public static implicit operator StageInstance(string name) => new StageInstance(name);
  }
}
