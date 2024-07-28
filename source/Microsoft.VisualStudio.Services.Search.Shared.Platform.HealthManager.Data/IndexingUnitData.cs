// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.IndexingUnitData
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CDF1FDEE-D833-4D57-965E-D6F8F75FE22F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data
{
  public class IndexingUnitData : HealthData
  {
    private readonly IEnumerable<IndexingUnit> m_indexingUnits;

    public IndexingUnitData(IEnumerable<IndexingUnit> indexingUnits, DataType dataType)
      : base(dataType)
    {
      this.m_indexingUnits = indexingUnits;
    }

    public IEnumerable<IndexingUnit> GetIndexingUnitData() => this.m_indexingUnits;
  }
}
