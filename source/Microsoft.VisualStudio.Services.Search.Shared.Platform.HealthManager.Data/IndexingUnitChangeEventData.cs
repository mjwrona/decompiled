// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.IndexingUnitChangeEventData
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CDF1FDEE-D833-4D57-965E-D6F8F75FE22F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data
{
  public class IndexingUnitChangeEventData : HealthData
  {
    private readonly IEnumerable<IndexingUnitChangeEventDetails> m_indexingUnitChangeEventData;

    public IndexingUnitChangeEventData(
      IEnumerable<IndexingUnitChangeEventDetails> indexingUnitChangeEventData,
      DataType dataType)
      : base(dataType)
    {
      this.m_indexingUnitChangeEventData = indexingUnitChangeEventData;
    }

    public IEnumerable<IndexingUnitChangeEventDetails> GetIUCEData() => this.m_indexingUnitChangeEventData;
  }
}
