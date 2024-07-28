// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.PendingProcessAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1A24B0F-DA40-425D-8B54-1865D1FC90B8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers
{
  public class PendingProcessAnalyzer : IAnalyzer
  {
    public List<ActionData> Analyze(
      List<HealthData> dataList,
      Dictionary<DataType, ProviderContext> contextDataSet,
      out string result)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzer: {0}.", (object) nameof (PendingProcessAnalyzer))));
      try
      {
        List<KeyValuePair<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, TeamFoundationJobQueueEntry>> jobQueueData = ((JobQueueData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.JobQueueData)))?.GetJobQueueData();
        if (jobQueueData != null && jobQueueData.Any<KeyValuePair<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, TeamFoundationJobQueueEntry>>())
        {
          int num = 0;
          stringBuilder.Append("Additional details:");
          foreach (KeyValuePair<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, TeamFoundationJobQueueEntry> keyValuePair in jobQueueData)
          {
            if (keyValuePair.Value != null)
            {
              num += keyValuePair.Value.State == TeamFoundationJobState.QueuedScheduled ? 1 : 0;
              stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Entry: {0}, IndexingUnit: {1} \n", (object) keyValuePair.Value, (object) keyValuePair.Key)));
            }
          }
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("{0} jobs pending processing", (object) num)));
        }
        else
          stringBuilder.Append("No job entries found in the job queue.");
        return new List<ActionData>();
      }
      finally
      {
        result = stringBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTraceMessage("Health Manager", "HealthManagerAnalyzer", Level.Info, result);
        stringBuilder.Clear();
      }
    }

    public HashSet<DataType> GetDataTypes() => new HashSet<DataType>()
    {
      DataType.JobQueueData
    };
  }
}
