// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.ESCollectionAnyResultsExistAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1A24B0F-DA40-425D-8B54-1865D1FC90B8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers
{
  public class ESCollectionAnyResultsExistAnalyzer : IAnalyzer
  {
    public List<ActionData> Analyze(
      List<HealthData> dataList,
      Dictionary<DataType, ProviderContext> contextDataSet,
      out string result)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzer: {0}.", (object) nameof (ESCollectionAnyResultsExistAnalyzer))));
      try
      {
        ESTermQueryData esTermQueryData = (ESTermQueryData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.ESTermQuerydata));
        if (esTermQueryData != null)
        {
          int searchHitsCount = ((ESResponseObject) esTermQueryData.GetESQueryResponse()).SearchHitsCount;
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzing dataType: {0}.", (object) esTermQueryData.DataType)));
          if (searchHitsCount > 0)
            stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Some files have been found to be indexed for the collection. DocCount: {0}", (object) searchHitsCount)));
          else
            stringBuilder.Append("No docs found to be indexed for the collection.");
        }
        return new List<ActionData>();
      }
      finally
      {
        result = stringBuilder.ToString();
        Tracer.PublishClientTraceMessage("Health Manager", "HealthManagerAnalyzer", Level.Info, result);
        stringBuilder.Clear();
      }
    }

    public HashSet<DataType> GetDataTypes() => new HashSet<DataType>()
    {
      DataType.ESTermQuerydata
    };
  }
}
