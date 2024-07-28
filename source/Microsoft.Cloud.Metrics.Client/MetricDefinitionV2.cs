// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricDefinitionV2
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client
{
  public sealed class MetricDefinitionV2 : IEquatable<MetricDefinitionV2>
  {
    private int hashCode;

    public MetricDefinitionV2(
      string monitoringAccount,
      string metricNamespace,
      string metricName,
      IEnumerable<string> dimensionNames)
    {
      this.MonitoringAccount = monitoringAccount;
      this.MetricNamespace = metricNamespace;
      this.MetricName = metricName;
      this.DimensionNames = new SortedSet<string>((IComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (string dimensionName in dimensionNames)
        this.DimensionNames.Add(dimensionName);
      this.hashCode = 524287;
      int hashCode1 = StringComparer.OrdinalIgnoreCase.GetHashCode(monitoringAccount);
      this.hashCode = (this.hashCode << hashCode1 | this.hashCode >>> 32 - hashCode1) ^ hashCode1;
      this.hashCode = this.hashCode << hashCode1 | this.hashCode >>> 32 - hashCode1;
      int hashCode2 = StringComparer.OrdinalIgnoreCase.GetHashCode(metricNamespace);
      this.hashCode = (this.hashCode << hashCode2 | this.hashCode >>> 32 - hashCode2) ^ hashCode2;
      this.hashCode = this.hashCode << hashCode2 | this.hashCode >>> 32 - hashCode2;
      int hashCode3 = StringComparer.OrdinalIgnoreCase.GetHashCode(metricName);
      this.hashCode = (this.hashCode << hashCode3 | this.hashCode >>> 32 - hashCode3) ^ hashCode3;
      this.hashCode = this.hashCode << hashCode3 | this.hashCode >>> 32 - hashCode3;
      foreach (string dimensionName in this.DimensionNames)
      {
        int num = !string.IsNullOrWhiteSpace(dimensionName) ? StringComparer.OrdinalIgnoreCase.GetHashCode(dimensionName) : throw new ArgumentException("Dimension names cannot be null or empty strings.", nameof (dimensionNames));
        this.hashCode = (this.hashCode << num | this.hashCode >>> 32 - num) ^ num;
        this.hashCode = this.hashCode << num | this.hashCode >>> 32 - num;
      }
    }

    public string MonitoringAccount { get; private set; }

    public string MetricNamespace { get; private set; }

    public string MetricName { get; private set; }

    public SortedSet<string> DimensionNames { get; private set; }

    public override int GetHashCode() => this.hashCode;

    public override bool Equals(object obj) => this.Equals(obj as MetricDefinitionV2);

    public bool Equals(MetricDefinitionV2 other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return this.hashCode == other.hashCode && this.DimensionNames.Count == other.DimensionNames.Count && this.MonitoringAccount.Equals(other.MonitoringAccount) && this.MonitoringAccount.Equals(other.MetricNamespace) && this.MonitoringAccount.Equals(other.MetricName) && this.DimensionNames.SetEquals((IEnumerable<string>) other.DimensionNames);
    }
  }
}
