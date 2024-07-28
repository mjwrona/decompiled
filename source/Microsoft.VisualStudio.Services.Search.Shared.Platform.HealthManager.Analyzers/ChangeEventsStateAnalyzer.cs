// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.ChangeEventsStateAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1A24B0F-DA40-425D-8B54-1865D1FC90B8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
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
  public class ChangeEventsStateAnalyzer : IAnalyzer
  {
    public List<ActionData> Analyze(
      List<HealthData> dataList,
      Dictionary<DataType, ProviderContext> contextDataSet,
      out string result)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzer: {0}.", (object) nameof (ChangeEventsStateAnalyzer))));
      try
      {
        List<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>> iuceStateData = ((IndexingUnitChangeEventStateData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.IndexingUnitChangeEventStateData)))?.GetIUCEStateData();
        if (iuceStateData != null && iuceStateData.Any<Tuple<string, IndexingUnitChangeEventState, int, TimeSpan>>())
        {
          stringBuilder.Append("Following is the state of events for all entities:");
          foreach (Tuple<string, IndexingUnitChangeEventState, int, TimeSpan> tuple in iuceStateData)
            stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Operation:{0}, State:{1}, Count:{2}, MaxTimeInMinutes: {3}.\n", (object) tuple.Item1, (object) tuple.Item2, (object) tuple.Item3, (object) tuple.Item4)));
        }
        else
          stringBuilder.Append("No change events found.");
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
      DataType.IndexingUnitChangeEventStateData
    };
  }
}
