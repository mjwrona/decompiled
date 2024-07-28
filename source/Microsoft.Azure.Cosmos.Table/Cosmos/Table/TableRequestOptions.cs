// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.TableRequestOptions
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class TableRequestOptions
  {
    public static readonly TimeSpan MaxMaximumExecutionTime = TimeSpan.FromDays(24.0);
    private TablePayloadFormat? payloadFormat;
    private TimeSpan? maximumExecutionTime;
    internal static TableRequestOptions BaseDefaultRequestOptions = new TableRequestOptions()
    {
      RetryPolicy = (IRetryPolicy) new NoRetry(),
      ServerTimeout = new TimeSpan?(),
      MaximumExecutionTime = new TimeSpan?(),
      PayloadFormat = new TablePayloadFormat?(TablePayloadFormat.Json),
      PropertyResolver = (Func<string, string, string, string, EdmType>) null,
      ProjectSystemProperties = new bool?(true),
      TableQueryMaxItemCount = new int?(1000),
      TableQueryMaxDegreeOfParallelism = new int?(-1),
      TableQueryEnableScan = new bool?(false),
      TableQueryContinuationTokenLimitInKb = new int?(),
      TableQueryMaxBufferedItemCount = new int?(-1)
    };

    public TableRequestOptions()
    {
    }

    public TableRequestOptions(TableRequestOptions other)
    {
      if (other == null)
        return;
      this.ServerTimeout = other.ServerTimeout;
      this.RetryPolicy = other.RetryPolicy;
      this.MaximumExecutionTime = other.MaximumExecutionTime;
      this.OperationExpiryTime = other.OperationExpiryTime;
      this.PayloadFormat = other.PayloadFormat;
      this.PropertyResolver = other.PropertyResolver;
      this.ProjectSystemProperties = other.ProjectSystemProperties;
      this.LocationMode = other.LocationMode;
      this.SessionToken = other.SessionToken;
      this.TableQueryMaxItemCount = other.TableQueryMaxItemCount;
      this.TableQueryEnableScan = other.TableQueryEnableScan;
      this.TableQueryMaxDegreeOfParallelism = other.TableQueryMaxDegreeOfParallelism;
      this.TableQueryContinuationTokenLimitInKb = other.TableQueryContinuationTokenLimitInKb;
      this.TableQueryMaxBufferedItemCount = other.TableQueryMaxBufferedItemCount;
      this.ConsistencyLevel = other.ConsistencyLevel;
    }

    internal static TableRequestOptions ApplyDefaults(
      TableRequestOptions requestOptions,
      CloudTableClient serviceClient)
    {
      TableRequestOptions tableRequestOptions1 = new TableRequestOptions(requestOptions);
      bool? nullable1;
      if (serviceClient.IsPremiumEndpoint())
      {
        tableRequestOptions1.TableQueryMaxItemCount = tableRequestOptions1.TableQueryMaxItemCount ?? serviceClient.DefaultRequestOptions.TableQueryMaxItemCount ?? TableRequestOptions.BaseDefaultRequestOptions.TableQueryMaxItemCount;
        TableRequestOptions tableRequestOptions2 = tableRequestOptions1;
        int? nullable2 = tableRequestOptions1.TableQueryMaxDegreeOfParallelism;
        int? nullable3 = nullable2 ?? serviceClient.DefaultRequestOptions.TableQueryMaxDegreeOfParallelism ?? TableRequestOptions.BaseDefaultRequestOptions.TableQueryMaxDegreeOfParallelism;
        tableRequestOptions2.TableQueryMaxDegreeOfParallelism = nullable3;
        TableRequestOptions tableRequestOptions3 = tableRequestOptions1;
        nullable1 = tableRequestOptions1.TableQueryEnableScan;
        bool? nullable4 = nullable1 ?? serviceClient.DefaultRequestOptions.TableQueryEnableScan ?? TableRequestOptions.BaseDefaultRequestOptions.TableQueryEnableScan;
        tableRequestOptions3.TableQueryEnableScan = nullable4;
        TableRequestOptions tableRequestOptions4 = tableRequestOptions1;
        nullable2 = tableRequestOptions1.TableQueryContinuationTokenLimitInKb;
        int? nullable5 = nullable2 ?? serviceClient.DefaultRequestOptions.TableQueryContinuationTokenLimitInKb ?? TableRequestOptions.BaseDefaultRequestOptions.TableQueryContinuationTokenLimitInKb;
        tableRequestOptions4.TableQueryContinuationTokenLimitInKb = nullable5;
        TableRequestOptions tableRequestOptions5 = tableRequestOptions1;
        nullable2 = tableRequestOptions1.TableQueryMaxBufferedItemCount;
        int? nullable6 = nullable2 ?? serviceClient.DefaultRequestOptions.TableQueryMaxBufferedItemCount ?? TableRequestOptions.BaseDefaultRequestOptions.TableQueryMaxBufferedItemCount;
        tableRequestOptions5.TableQueryMaxBufferedItemCount = nullable6;
        tableRequestOptions1.ConsistencyLevel = tableRequestOptions1.ConsistencyLevel ?? serviceClient.DefaultRequestOptions.ConsistencyLevel ?? TableRequestOptions.BaseDefaultRequestOptions.ConsistencyLevel;
      }
      else
        tableRequestOptions1.LocationMode = tableRequestOptions1.LocationMode ?? serviceClient.DefaultRequestOptions.LocationMode ?? TableRequestOptions.BaseDefaultRequestOptions.LocationMode;
      tableRequestOptions1.RetryPolicy = tableRequestOptions1.RetryPolicy ?? serviceClient.DefaultRequestOptions.RetryPolicy ?? TableRequestOptions.BaseDefaultRequestOptions.RetryPolicy;
      TableRequestOptions tableRequestOptions6 = tableRequestOptions1;
      TimeSpan? nullable7 = tableRequestOptions1.ServerTimeout;
      TimeSpan? nullable8 = nullable7 ?? serviceClient.DefaultRequestOptions.ServerTimeout ?? TableRequestOptions.BaseDefaultRequestOptions.ServerTimeout;
      tableRequestOptions6.ServerTimeout = nullable8;
      TableRequestOptions tableRequestOptions7 = tableRequestOptions1;
      nullable7 = tableRequestOptions1.MaximumExecutionTime;
      TimeSpan? nullable9 = nullable7 ?? serviceClient.DefaultRequestOptions.MaximumExecutionTime ?? TableRequestOptions.BaseDefaultRequestOptions.MaximumExecutionTime;
      tableRequestOptions7.MaximumExecutionTime = nullable9;
      tableRequestOptions1.PayloadFormat = tableRequestOptions1.PayloadFormat ?? serviceClient.DefaultRequestOptions.PayloadFormat ?? TableRequestOptions.BaseDefaultRequestOptions.PayloadFormat;
      if (!tableRequestOptions1.OperationExpiryTime.HasValue)
      {
        nullable7 = tableRequestOptions1.MaximumExecutionTime;
        if (nullable7.HasValue)
        {
          TableRequestOptions tableRequestOptions8 = tableRequestOptions1;
          DateTime utcNow = DateTime.UtcNow;
          nullable7 = tableRequestOptions1.MaximumExecutionTime;
          TimeSpan timeSpan = nullable7.Value;
          DateTime? nullable10 = new DateTime?(utcNow + timeSpan);
          tableRequestOptions8.OperationExpiryTime = nullable10;
        }
      }
      tableRequestOptions1.PropertyResolver = tableRequestOptions1.PropertyResolver ?? serviceClient.DefaultRequestOptions.PropertyResolver ?? TableRequestOptions.BaseDefaultRequestOptions.PropertyResolver;
      TableRequestOptions tableRequestOptions9 = tableRequestOptions1;
      nullable1 = tableRequestOptions1.ProjectSystemProperties;
      bool? nullable11 = nullable1 ?? serviceClient.DefaultRequestOptions.ProjectSystemProperties ?? TableRequestOptions.BaseDefaultRequestOptions.ProjectSystemProperties;
      tableRequestOptions9.ProjectSystemProperties = nullable11;
      return tableRequestOptions1;
    }

    internal DateTime? OperationExpiryTime { get; set; }

    public IRetryPolicy RetryPolicy { get; set; }

    public bool? ProjectSystemProperties { get; set; }

    public Microsoft.Azure.Cosmos.Table.LocationMode? LocationMode { get; set; }

    public TimeSpan? ServerTimeout { get; set; }

    public TimeSpan? MaximumExecutionTime
    {
      get => this.maximumExecutionTime;
      set
      {
        if (value.HasValue)
          CommonUtility.AssertInBounds<TimeSpan>(nameof (MaximumExecutionTime), value.Value, TimeSpan.Zero, TableRequestOptions.MaxMaximumExecutionTime);
        this.maximumExecutionTime = value;
      }
    }

    public TablePayloadFormat? PayloadFormat
    {
      get => this.payloadFormat;
      set
      {
        if (!value.HasValue)
          return;
        this.payloadFormat = new TablePayloadFormat?(value.Value);
      }
    }

    public Func<string, string, string, string, EdmType> PropertyResolver { get; set; }

    public string SessionToken { get; set; }

    public int? TableQueryMaxItemCount { get; set; }

    public bool? TableQueryEnableScan { get; set; }

    public int? TableQueryMaxDegreeOfParallelism { get; set; }

    public int? TableQueryContinuationTokenLimitInKb { get; set; }

    public int? TableQueryMaxBufferedItemCount { get; set; }

    public Microsoft.Azure.Cosmos.ConsistencyLevel? ConsistencyLevel { get; set; }
  }
}
