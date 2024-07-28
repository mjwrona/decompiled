// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.GenericHealthAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1A24B0F-DA40-425D-8B54-1865D1FC90B8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers
{
  public class GenericHealthAnalyzer : IAnalyzer
  {
    public List<ActionData> Analyze(
      List<HealthData> dataList,
      Dictionary<DataType, ProviderContext> contextDataSet,
      out string result)
    {
      StringBuilder stringBuilder = new StringBuilder();
      result = stringBuilder.ToString();
      return new List<ActionData>();
    }

    public HashSet<DataType> GetDataTypes() => new HashSet<DataType>()
    {
      DataType.CollectionIndexingUnitData,
      DataType.ProjectIndexingUnitData,
      DataType.RepoIndexingUnitData,
      DataType.ReindexingStatusData,
      DataType.IndexingUnitChangeEventStateData
    };
  }
}
