// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.JobQueueData
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CDF1FDEE-D833-4D57-965E-D6F8F75FE22F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data
{
  public class JobQueueData : HealthData
  {
    private readonly List<KeyValuePair<IndexingUnit, TeamFoundationJobQueueEntry>> m_jobQueueData;

    public JobQueueData(
      List<KeyValuePair<IndexingUnit, TeamFoundationJobQueueEntry>> jobQueueData,
      DataType dataType)
      : base(dataType)
    {
      this.m_jobQueueData = jobQueueData;
    }

    public List<KeyValuePair<IndexingUnit, TeamFoundationJobQueueEntry>> GetJobQueueData() => this.m_jobQueueData;
  }
}
