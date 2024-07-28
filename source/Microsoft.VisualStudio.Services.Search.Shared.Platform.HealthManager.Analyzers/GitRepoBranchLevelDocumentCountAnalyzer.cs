// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.GitRepoBranchLevelDocumentCountAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1A24B0F-DA40-425D-8B54-1865D1FC90B8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
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
  public class GitRepoBranchLevelDocumentCountAnalyzer : IAnalyzer
  {
    public List<ActionData> Analyze(
      List<HealthData> dataList,
      Dictionary<DataType, ProviderContext> contextDataSet,
      out string result)
    {
      if (dataList.IsNullOrEmpty<HealthData>())
        throw new ArgumentException("dataList cannot be null.", nameof (dataList));
      if (contextDataSet.IsNullOrEmpty<KeyValuePair<DataType, ProviderContext>>())
        throw new ArgumentException("contextDataSet cannont be null.", nameof (contextDataSet));
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzer: {0}. ", (object) nameof (GitRepoBranchLevelDocumentCountAnalyzer))));
      try
      {
        GitRepoBranchLevelDocumentCountData documentCountData = (GitRepoBranchLevelDocumentCountData) dataList.SingleOrDefault<HealthData>((Func<HealthData, bool>) (x => x.DataType == DataType.GitRepoBranchLevelDocCount));
        if (documentCountData != null)
        {
          stringBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Analyzing dataType: {0}. ", (object) documentCountData.DataType)));
          IDictionary<string, long> levelDocumentCount = documentCountData.GetBranchLevelDocumentCount();
          stringBuilder.Append("Branch Level Document Count is present in ClientTrace.");
          foreach (KeyValuePair<string, long> keyValuePair in (IEnumerable<KeyValuePair<string, long>>) levelDocumentCount)
          {
            ClientTraceData properties = new ClientTraceData();
            properties.Add(keyValuePair.Key, (object) keyValuePair.Value);
            Tracer.PublishClientTrace("Health Manager", "HealthManagerAnalyzer", properties, true);
          }
          return new List<ActionData>();
        }
        throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("No data found corresponding to data type {0}", (object) DataType.GitRepoBranchLevelDocCount)));
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
      DataType.GitRepoBranchLevelDocCount
    };
  }
}
