// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.PreAggregateUsage
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  public class PreAggregateUsage
  {
    public const byte CurrentVersion = 0;

    public PreAggregateUsage()
    {
    }

    public PreAggregateUsage(
      string monitoringAccount,
      string metricNamespace,
      string metricName,
      int dailyUsageCount,
      Dictionary<string, string> dimensionAliasMapping)
    {
      this.MonitoringAccount = monitoringAccount;
      this.MetricNamespace = metricNamespace;
      this.MetricName = metricName;
      this.DailyUsageCount = dailyUsageCount;
      this.DimensionAliasMapping = dimensionAliasMapping;
    }

    public string MonitoringAccount { get; private set; }

    public string MetricNamespace { get; private set; }

    public string MetricName { get; private set; }

    public int DailyUsageCount { get; private set; }

    public Dictionary<string, string> DimensionAliasMapping { get; private set; }

    public void Deserialize(BinaryReader reader)
    {
      int num1 = reader.ReadInt32();
      if (num1 > 0)
        throw new ArgumentException(string.Format("The serialization version for PreAggregateUsage is higher than the supported version, so upgrading SDK is required.CurrentVersion: {0}, VersionInResponse: {1}.", (object) (byte) 0, (object) num1));
      this.MonitoringAccount = reader.ReadString();
      this.MetricNamespace = reader.ReadString();
      this.MetricName = reader.ReadString();
      this.DailyUsageCount = reader.ReadInt32();
      int num2 = reader.ReadInt32();
      if (num2 > 0)
      {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        for (int index = 0; index < num2; ++index)
        {
          string key = reader.ReadString();
          string str = reader.ReadString();
          if (!dictionary.ContainsKey(key))
            dictionary.Add(key, str);
        }
        this.DimensionAliasMapping = dictionary;
      }
      else
        this.DimensionAliasMapping = (Dictionary<string, string>) null;
    }
  }
}
