// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.MetadataDbStampExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public static class MetadataDbStampExtensions
  {
    private const string c_requestContextStampKey = "WorkItemTracking/MetadataDbStamps";

    public static MetadataDBStamps MetadataDbStamps(this IVssRequestContext requestContext)
    {
      MetadataDBStamps metadataTableTimestamps;
      if (!requestContext.Items.TryGetValue<MetadataDBStamps>("WorkItemTracking/MetadataDbStamps", out metadataTableTimestamps))
      {
        metadataTableTimestamps = requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().GetMetadataTableTimestamps(requestContext);
        requestContext.Items["WorkItemTracking/MetadataDbStamps"] = (object) metadataTableTimestamps;
      }
      return metadataTableTimestamps;
    }

    public static MetadataDBStamps MetadataDbStamps(
      this IVssRequestContext requestContext,
      IEnumerable<MetadataTable> tableNames)
    {
      return requestContext.MetadataDbStamps().SubSet(tableNames);
    }

    public static void UpdateMetadataDbStampForTable(
      this IVssRequestContext requestContext,
      MetadataTable table,
      long timestamp)
    {
      requestContext.Items["WorkItemTracking/MetadataDbStamps"] = (object) new MetadataDBStamps((IDictionary<MetadataTable, long>) new Dictionary<MetadataTable, long>((IDictionary<MetadataTable, long>) requestContext.MetadataDbStamps())
      {
        [table] = timestamp
      });
    }

    public static void ResetMetadataDbStamps(this IVssRequestContext requestContext) => requestContext.Items.Remove("WorkItemTracking/MetadataDbStamps");

    public static int Compare(this MetadataDBStamps stampsA, MetadataDBStamps stampsB)
    {
      int num1 = stampsA == null ? 1 : (stampsA.Count == 0 ? 1 : 0);
      bool flag = stampsB == null || stampsB.Count == 0;
      if (num1 != 0)
        return !flag ? -1 : 0;
      if (flag)
        return 1;
      if (stampsA.Count != stampsB.Count)
        return -1;
      foreach (KeyValuePair<MetadataTable, long> keyValuePair in (ReadOnlyDictionary<MetadataTable, long>) stampsA)
      {
        long num2;
        if (!stampsB.TryGetValue(keyValuePair.Key, out num2))
          num2 = 0L;
        if (keyValuePair.Value != num2)
          return keyValuePair.Value > num2 ? 1 : -1;
      }
      return 0;
    }

    public static bool IsFresh(this MetadataDBStamps oldStamps, MetadataDBStamps currentDbStamps) => oldStamps.Compare(currentDbStamps) >= 0;

    public static bool AreClientStampsFresh(
      this MetadataDBStamps clientStamps,
      MetadataDBStamps currentDbStamps)
    {
      long num;
      return clientStamps.All<KeyValuePair<MetadataTable, long>>((Func<KeyValuePair<MetadataTable, long>, bool>) (s => currentDbStamps.TryGetValue(s.Key, out num) && s.Value >= num));
    }

    public static MetadataDBStamps SubSet(
      this MetadataDBStamps stamps,
      IEnumerable<MetadataTable> tableNames)
    {
      return new MetadataDBStamps((IDictionary<MetadataTable, long>) tableNames.Distinct<MetadataTable>().ToDictionary<MetadataTable, MetadataTable, long>((Func<MetadataTable, MetadataTable>) (t => t), (Func<MetadataTable, long>) (t =>
      {
        long num;
        if (!stamps.TryGetValue(t, out num))
          num = 0L;
        return num;
      })));
    }
  }
}
