// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.ESDuplicateFilesDetectionAnalyzer
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
  public class ESDuplicateFilesDetectionAnalyzer : IAnalyzer
  {
    public List<ActionData> Analyze(
      List<HealthData> dataList,
      Dictionary<DataType, ProviderContext> contextDataSet,
      out string result)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzer: {0}.", (object) nameof (ESDuplicateFilesDetectionAnalyzer))));
      try
      {
        ESTermQueryData esTermQueryData = (ESTermQueryData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.ESTermQuerydata));
        if (esTermQueryData != null)
        {
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzing dataType: {0}.", (object) "ESTermQuerydata")));
          ESResponseObject esQueryResponse = (ESResponseObject) esTermQueryData.GetESQueryResponse();
          int searchHitsCount = esQueryResponse.SearchHitsCount;
          switch (searchHitsCount)
          {
            case 0:
              stringBuilder.Append("ES returned zero results. Hence not proceeding with duplicacy detection");
              break;
            case 1:
              stringBuilder.Append("Only one result was returned from ES. Hence concluding there can't be duplication in this case");
              break;
            default:
              if (searchHitsCount > 1)
              {
                int count = esQueryResponse.HitData.Count;
                stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("ResultsCount {0} being analyzed for duplication.", (object) count)));
                if (esQueryResponse.HitData != null)
                {
                  List<Tuple<string, string>> hitData = esQueryResponse.HitData;
                  bool flag = false;
                  for (int index1 = 0; index1 < count && !flag; ++index1)
                  {
                    Tuple<string, string> tuple1 = hitData[index1];
                    for (int index2 = index1 + 1; index2 < count; ++index2)
                    {
                      Tuple<string, string> tuple2 = hitData[index2];
                      if (tuple1.Item1.Equals(tuple2.Item1) && !tuple1.Item2.Equals(tuple2.Item2))
                      {
                        stringBuilder.Append("Detected duplication.");
                        stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Found same documentId:{0} in routingIds {1} and {2}", (object) tuple1.Item1, (object) tuple1.Item2, (object) tuple2.Item2)));
                        flag = true;
                        break;
                      }
                    }
                  }
                  if (!flag)
                  {
                    stringBuilder.Append("No duplication found in the analysis of ES results");
                    break;
                  }
                  break;
                }
                break;
              }
              break;
          }
        }
      }
      finally
      {
        result = stringBuilder.ToString();
        Tracer.PublishClientTraceMessage("Health Manager", "HealthManagerAnalyzer", Level.Info, result);
        stringBuilder.Clear();
      }
      return new List<ActionData>();
    }

    public HashSet<DataType> GetDataTypes() => new HashSet<DataType>()
    {
      DataType.ESTermQuerydata
    };
  }
}
