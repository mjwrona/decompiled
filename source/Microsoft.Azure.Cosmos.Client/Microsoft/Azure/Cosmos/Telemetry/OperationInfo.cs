// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Telemetry.OperationInfo
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Cosmos.Telemetry
{
  [Serializable]
  internal sealed class OperationInfo
  {
    [JsonProperty(PropertyName = "regionsContacted")]
    internal string RegionsContacted { get; }

    [JsonProperty(PropertyName = "greaterThan1Kb")]
    internal bool? GreaterThan1Kb { get; set; }

    [JsonProperty(PropertyName = "databaseName")]
    private string DatabaseName { get; }

    [JsonProperty(PropertyName = "containerName")]
    private string ContainerName { get; }

    [JsonProperty(PropertyName = "operation")]
    internal string Operation { get; }

    [JsonProperty(PropertyName = "resource")]
    internal string Resource { get; }

    [JsonProperty(PropertyName = "consistency")]
    internal string Consistency { get; }

    [JsonProperty(PropertyName = "statusCode")]
    public int? StatusCode { get; }

    [JsonProperty(PropertyName = "subStatusCode")]
    public string SubStatusCode { get; }

    [JsonProperty(PropertyName = "metricInfo")]
    internal MetricInfo MetricInfo { get; set; }

    internal OperationInfo(string metricsName, string unitName) => this.MetricInfo = new MetricInfo(metricsName, unitName);

    internal OperationInfo(
      string regionsContacted,
      long? responseSizeInBytes,
      string consistency,
      string databaseName,
      string containerName,
      OperationType? operation,
      ResourceType? resource,
      int? statusCode,
      string subStatusCode)
    {
      this.RegionsContacted = regionsContacted;
      if (responseSizeInBytes.HasValue)
      {
        long? nullable = responseSizeInBytes;
        long num = 1024;
        this.GreaterThan1Kb = new bool?(nullable.GetValueOrDefault() > num & nullable.HasValue);
      }
      this.Consistency = consistency;
      this.DatabaseName = databaseName;
      this.ContainerName = containerName;
      this.Operation = operation.HasValue ? operation.GetValueOrDefault().ToOperationTypeString() : (string) null;
      this.Resource = resource.HasValue ? resource.GetValueOrDefault().ToResourceTypeString() : (string) null;
      this.StatusCode = statusCode;
      this.SubStatusCode = subStatusCode;
    }

    public OperationInfo(
      string regionsContacted,
      bool? greaterThan1Kb,
      string databaseName,
      string containerName,
      string operation,
      string resource,
      string consistency,
      int? statusCode,
      string subStatusCode,
      MetricInfo metricInfo)
    {
      this.RegionsContacted = regionsContacted;
      this.GreaterThan1Kb = greaterThan1Kb;
      this.DatabaseName = databaseName;
      this.ContainerName = containerName;
      this.Operation = operation;
      this.Resource = resource;
      this.Consistency = consistency;
      this.StatusCode = statusCode;
      this.SubStatusCode = subStatusCode;
      this.MetricInfo = metricInfo;
    }

    public OperationInfo Copy() => new OperationInfo(this.RegionsContacted, this.GreaterThan1Kb, this.DatabaseName, this.ContainerName, this.Operation, this.Resource, this.Consistency, this.StatusCode, this.SubStatusCode, (MetricInfo) null);

    public override int GetHashCode()
    {
      int num1 = (3 * 7 ^ (this.RegionsContacted == null ? 0 : this.RegionsContacted.GetHashCode())) * 7;
      bool? greaterThan1Kb = this.GreaterThan1Kb;
      int num2;
      if (greaterThan1Kb.HasValue)
      {
        greaterThan1Kb = this.GreaterThan1Kb;
        num2 = greaterThan1Kb.GetHashCode();
      }
      else
        num2 = 0;
      int num3 = ((((((num1 ^ num2) * 7 ^ (this.Consistency == null ? 0 : this.Consistency.GetHashCode())) * 7 ^ (this.DatabaseName == null ? 0 : this.DatabaseName.GetHashCode())) * 7 ^ (this.ContainerName == null ? 0 : this.ContainerName.GetHashCode())) * 7 ^ (this.Operation == null ? 0 : this.Operation.GetHashCode())) * 7 ^ (this.Resource == null ? 0 : this.Resource.GetHashCode())) * 7;
      int? statusCode = this.StatusCode;
      int num4;
      if (statusCode.HasValue)
      {
        statusCode = this.StatusCode;
        num4 = statusCode.GetHashCode();
      }
      else
        num4 = 0;
      return (num3 ^ num4) * 7 ^ (this.SubStatusCode == null ? 0 : this.SubStatusCode.GetHashCode());
    }

    public override bool Equals(object obj)
    {
      if (!(obj is OperationInfo operationInfo) || (this.RegionsContacted != null || operationInfo.RegionsContacted != null) && (this.RegionsContacted == null || operationInfo.RegionsContacted == null || !this.RegionsContacted.Equals(operationInfo.RegionsContacted)) || (this.GreaterThan1Kb.HasValue || operationInfo.GreaterThan1Kb.HasValue) && (!this.GreaterThan1Kb.HasValue || !operationInfo.GreaterThan1Kb.HasValue || !this.GreaterThan1Kb.Equals((object) operationInfo.GreaterThan1Kb)) || (this.Consistency != null || operationInfo.Consistency != null) && (this.Consistency == null || operationInfo.Consistency == null || !this.Consistency.Equals(operationInfo.Consistency)) || (this.DatabaseName != null || operationInfo.DatabaseName != null) && (this.DatabaseName == null || operationInfo.DatabaseName == null || !this.DatabaseName.Equals(operationInfo.DatabaseName)) || (this.ContainerName != null || operationInfo.ContainerName != null) && (this.ContainerName == null || operationInfo.ContainerName == null || !this.ContainerName.Equals(operationInfo.ContainerName)) || (this.Operation != null || operationInfo.Operation != null) && (this.Operation == null || operationInfo.Operation == null || !this.Operation.Equals(operationInfo.Operation)) || (this.Resource != null || operationInfo.Resource != null) && (this.Resource == null || operationInfo.Resource == null || !this.Resource.Equals(operationInfo.Resource)) || (this.StatusCode.HasValue || operationInfo.StatusCode.HasValue) && (!this.StatusCode.HasValue || !operationInfo.StatusCode.HasValue || !this.StatusCode.Equals((object) operationInfo.StatusCode)))
        return false;
      if (this.SubStatusCode == null && operationInfo.SubStatusCode == null)
        return true;
      return this.SubStatusCode != null && operationInfo.SubStatusCode != null && this.SubStatusCode.Equals(operationInfo.SubStatusCode);
    }

    internal void SetAggregators(LongConcurrentHistogram histogram, double adjustment = 1.0) => this.MetricInfo.SetAggregators(histogram, adjustment);
  }
}
