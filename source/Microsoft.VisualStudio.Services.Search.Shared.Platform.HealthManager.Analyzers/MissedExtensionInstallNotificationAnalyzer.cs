// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.MissedExtensionInstallNotificationAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1A24B0F-DA40-425D-8B54-1865D1FC90B8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
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
  public class MissedExtensionInstallNotificationAnalyzer : IAnalyzer
  {
    public List<ActionData> Analyze(
      List<HealthData> dataList,
      Dictionary<DataType, ProviderContext> contextDataSet,
      out string result)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzer: {0}.", (object) nameof (MissedExtensionInstallNotificationAnalyzer))));
      try
      {
        IndexingUnitData indexingUnitData = (IndexingUnitData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.CollectionIndexingUnitData));
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> list = indexingUnitData != null ? indexingUnitData.GetIndexingUnitData().ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>() : (List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) null;
        if (list != null)
        {
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzing dataType: {0}.", (object) indexingUnitData.DataType)));
          if (list.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
          {
            if (list.Count == 1)
            {
              stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Entry found as expected. Details-")) + FormattableString.Invariant(FormattableStringFactory.Create("IndexIndex: {0}", (object) list[0].Properties?.IndexIndices[0]?.IndexName)) + FormattableString.Invariant(FormattableStringFactory.Create("QueryIndex: {0}", (object) list[0].Properties?.QueryIndices[0]?.IndexName)) + FormattableString.Invariant(FormattableStringFactory.Create("QueryRouting: {0}", (object) list[0].Properties?.QueryIndices[0]?.Routing)));
            }
            else
            {
              stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Unexpectedly more entries found. Count:{0}", (object) list.Count)));
              foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in list)
                stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("IndexIndex: {0}", (object) indexingUnit.Properties?.IndexIndices[0]?.IndexName)) + FormattableString.Invariant(FormattableStringFactory.Create("QueryIndex: {0}", (object) indexingUnit.Properties?.QueryIndices[0]?.IndexName)) + FormattableString.Invariant(FormattableStringFactory.Create("QueryRouting: {0}", (object) indexingUnit.Properties?.QueryIndices[0]?.Routing)));
            }
          }
          else
            stringBuilder.Append("No entry found.");
        }
        IndexingUnitChangeEventData unitChangeEventData = (IndexingUnitChangeEventData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.IndexingUnitChangeEventsData));
        IEnumerable<IndexingUnitChangeEventDetails> iuceData = unitChangeEventData?.GetIUCEData();
        if (iuceData != null)
        {
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzing dataType: {0}.", (object) unitChangeEventData.DataType)));
          if (iuceData.Any<IndexingUnitChangeEventDetails>())
          {
            stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("One or more entries found as expected. Count:{0}", (object) iuceData.Count<IndexingUnitChangeEventDetails>())));
            foreach (IndexingUnitChangeEventDetails changeEventDetails in iuceData)
              stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Id:{0}", (object) changeEventDetails.IndexingUnitChangeEvent.Id)) + FormattableString.Invariant(FormattableStringFactory.Create("ChangeType:{0}", (object) changeEventDetails.IndexingUnitChangeEvent.ChangeType)) + FormattableString.Invariant(FormattableStringFactory.Create("State:{0}", (object) changeEventDetails.IndexingUnitChangeEvent.State)) + FormattableString.Invariant(FormattableStringFactory.Create("AttemptCount:{0}", (object) changeEventDetails.IndexingUnitChangeEvent.AttemptCount)) + FormattableString.Invariant(FormattableStringFactory.Create("AssociatedJobId:{0}", (object) changeEventDetails.AssociatedJobId)));
          }
          else
            stringBuilder.Append("No entry found.");
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
      DataType.CollectionIndexingUnitData,
      DataType.IndexingUnitChangeEventsData
    };
  }
}
