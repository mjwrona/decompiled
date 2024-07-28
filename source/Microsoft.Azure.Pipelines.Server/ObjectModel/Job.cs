// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.Job
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public class Job
  {
    public Job(Guid JobId) => this.Id = JobId;

    public Guid Id { get; private set; }

    public string Name { get; set; }

    public int Attempt { get; set; }

    public JobState? State { get; set; }

    public JobResult? Result { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? FinishTime { get; set; }

    public IReadOnlyDictionary<string, object> Links { get; set; }
  }
}
