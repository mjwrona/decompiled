// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.OperationEvent
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;

namespace Microsoft.VisualStudio.Telemetry
{
  public class OperationEvent : TelemetryEvent
  {
    private const string OperationPropertyPrefixName = "DataModel.Action.";
    private const string ResultPropertyName = "DataModel.Action.Result";
    private const string ResultSummaryPropertyName = "DataModel.Action.ResultSummary";
    private const string StageTypePropertyName = "DataModel.Action.Type";
    private const string StartTimePropertyName = "DataModel.Action.StartTime";
    private const string EndTimePropertyName = "DataModel.Action.EndTime";
    private const string DurationPropertyName = "DataModel.Action.DurationInMilliseconds";
    private const string PostStartEventPropertyName = "DataModel.Action.PostStartEvent";
    private TelemetryResult result;
    private string resultSummary;
    private OperationStageType stageType;
    private double? duration;
    private long? startTime;
    private long? endTime;

    public TelemetryResult Result
    {
      get => this.result;
      private set
      {
        this.result = value;
        this.ReservedProperties.AddPrefixed("Reserved.DataModel.Action.Result", (object) TelemetryResultStrings.GetString(value));
      }
    }

    public string ResultSummary
    {
      get => this.resultSummary;
      private set
      {
        this.resultSummary = value;
        this.ReservedProperties.AddPrefixed("Reserved.DataModel.Action.ResultSummary", (object) value);
      }
    }

    public OperationStageType StageType
    {
      get => this.stageType;
      internal set
      {
        this.stageType = value;
        this.ReservedProperties.AddPrefixed("Reserved.DataModel.Action.Type", (object) this.GetOperationStageTypeName(value));
      }
    }

    public Guid? StartEndPairId
    {
      get
      {
        Guid? nullable = new Guid?();
        return this.StageType != OperationStageType.Atomic ? new Guid?(this.Correlation.Id) : nullable;
      }
    }

    public double? Duration
    {
      get => this.duration;
      private set
      {
        this.duration = value;
        this.ReservedProperties.AddPrefixed("Reserved.DataModel.Action.DurationInMilliseconds", (object) value);
      }
    }

    public long? StartTime
    {
      get => this.startTime;
      private set
      {
        this.startTime = value;
        if (value.HasValue)
          this.ReservedProperties.AddPrefixed("Reserved.DataModel.Action.StartTime", (object) new DateTime(value.Value, DateTimeKind.Utc).ToString("O"));
        else
          this.ReservedProperties.RemovePrefixed("Reserved.DataModel.Action.StartTime");
      }
    }

    public long? EndTime
    {
      get => this.endTime;
      private set
      {
        this.endTime = value;
        if (value.HasValue)
          this.ReservedProperties.AddPrefixed("Reserved.DataModel.Action.EndTime", (object) new DateTime(value.Value, DateTimeKind.Utc).ToString("O"));
        else
          this.ReservedProperties.RemovePrefixed("Reserved.DataModel.Action.EndTime");
      }
    }

    public string ProductName => this.ReservedProperties.GetPrefixed("Reserved.DataModel.ProductName").ToString();

    public string FeatureName => this.ReservedProperties.GetPrefixed("Reserved.DataModel.FeatureName").ToString();

    public string EntityName => this.ReservedProperties.GetPrefixed("Reserved.DataModel.EntityName").ToString();

    public OperationEvent(string eventName, TelemetryResult result, string resultSummary = null)
      : this(eventName, OperationStageType.Atomic, result, resultSummary)
    {
    }

    internal OperationEvent(
      string eventName,
      OperationStageType stageType,
      TelemetryResult result,
      string resultSummary = null)
      : this(eventName, DataModelEventType.Operation, stageType, result, resultSummary)
    {
    }

    internal OperationEvent(
      string eventName,
      DataModelEventType eventType,
      OperationStageType stageType,
      TelemetryResult result,
      string resultSummary = null)
      : base(eventName, TelemetrySeverity.Normal, eventType)
    {
      if (eventType != DataModelEventType.UserTask && eventType != DataModelEventType.Operation)
        throw new ArgumentException("Expect DataModelEventType UserTask or Operation only.", nameof (eventType));
      DataModelEventNameHelper.SetProductFeatureEntityName(this);
      this.StageType = stageType;
      this.SetResultProperties(result, resultSummary);
    }

    public void Correlate(TelemetryEventCorrelation correlation, string description)
    {
      description.RequiresArgumentNotNullAndNotWhiteSpace(nameof (description));
      this.CorrelateWithDescription(correlation, description);
    }

    internal void SetResultProperties(TelemetryResult result, string resultSummary)
    {
      this.Result = result;
      this.ResultSummary = resultSummary;
    }

    internal void SetTimeProperties(
      DateTime startTime,
      DateTime endTime,
      double durationInMilliseconds)
    {
      this.StartTime = new long?(startTime.Ticks);
      this.EndTime = new long?(endTime.Ticks);
      this.Duration = new double?(durationInMilliseconds);
    }

    internal void SetPostStartEventProperty(bool postStartEvent) => this.ReservedProperties.AddPrefixed("Reserved.DataModel.Action.PostStartEvent", (object) postStartEvent);

    private string GetOperationStageTypeName(OperationStageType operationType) => operationType.ToString();
  }
}
