// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.ComposedAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1A24B0F-DA40-425D-8B54-1865D1FC90B8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers
{
  public class ComposedAnalyzer : IAnalyzer
  {
    private readonly List<IAnalyzer> m_analyzers;

    public ComposedAnalyzer(List<IAnalyzer> analyzers) => this.m_analyzers = analyzers;

    public List<ActionData> Analyze(
      List<HealthData> dataList,
      Dictionary<DataType, ProviderContext> contextDataSet,
      out string result)
    {
      List<ActionData> actionDataList = new List<ActionData>();
      StringBuilder stringBuilder = new StringBuilder();
      foreach (IAnalyzer analyzer in this.m_analyzers)
      {
        string result1;
        actionDataList.AddRange((IEnumerable<ActionData>) analyzer.Analyze(dataList, contextDataSet, out result1));
        stringBuilder.Append(result1);
      }
      result = stringBuilder.ToString();
      return actionDataList;
    }

    public HashSet<DataType> GetDataTypes()
    {
      HashSet<DataType> dataTypes = new HashSet<DataType>();
      foreach (IAnalyzer analyzer in this.m_analyzers)
        dataTypes.UnionWith((IEnumerable<DataType>) analyzer.GetDataTypes());
      return dataTypes;
    }
  }
}
