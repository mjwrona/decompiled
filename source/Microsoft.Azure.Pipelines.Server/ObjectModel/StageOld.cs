// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.StageOld
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using System;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public class StageOld
  {
    public StageOld(Guid projectId, int runId, Guid stageId)
    {
      this.ProjectId = projectId;
      this.RunId = runId;
      this.Id = stageId;
    }

    public Guid ProjectId { get; private set; }

    public int RunId { get; private set; }

    public Guid Id { get; private set; }

    public string Name { get; set; }

    public string DisplayName { get; set; }

    public StageState State { get; set; }

    public StageResult? Result { get; set; }
  }
}
