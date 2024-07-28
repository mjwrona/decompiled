// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.FailedFilesAnalyzer
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
  public class FailedFilesAnalyzer : IAnalyzer
  {
    public List<ActionData> Analyze(
      List<HealthData> dataList,
      Dictionary<DataType, ProviderContext> contextDataSet,
      out string result)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzer: {0}.", (object) nameof (FailedFilesAnalyzer))));
      try
      {
        FailedFilesCountData failedFilesCountData = (FailedFilesCountData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.FailedFilesCountData));
        IDictionary<int, int> countOfFailedRecords = failedFilesCountData?.GetCountOfFailedRecords();
        if (countOfFailedRecords != null)
        {
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzing dataType: {0}.", (object) failedFilesCountData.DataType)));
          if (countOfFailedRecords.Any<KeyValuePair<int, int>>())
          {
            foreach (KeyValuePair<int, int> keyValuePair in (IEnumerable<KeyValuePair<int, int>>) countOfFailedRecords)
              stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("IndexingUnitId: {0} CountOfFailures: {1}", (object) keyValuePair.Key, (object) keyValuePair.Value)));
          }
          else
            stringBuilder.Append("No entry found.");
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
      DataType.FailedFilesCountData
    };
  }
}
