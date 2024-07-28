// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.IndexingUnitChangeEventStateData
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CDF1FDEE-D833-4D57-965E-D6F8F75FE22F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data
{
  public class IndexingUnitChangeEventStateData : HealthData
  {
    private readonly List<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>> m_indexingUnitChangeEventsStateData;

    public IndexingUnitChangeEventStateData(
      List<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>> indexingUnitChangeEventsStateData,
      DataType dataType)
      : base(dataType)
    {
      this.m_indexingUnitChangeEventsStateData = indexingUnitChangeEventsStateData;
    }

    public List<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>> GetIUCEStateData() => this.m_indexingUnitChangeEventsStateData;

    public int GetTotalChangeEventsCount()
    {
      int changeEventsCount = 0;
      foreach (Tuple<string, IndexingUnitChangeEventState, int, TimeSpan> tuple in this.m_indexingUnitChangeEventsStateData)
        changeEventsCount += tuple.Item3;
      return changeEventsCount;
    }
  }
}
