// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.TransportRequestStats
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal sealed class TransportRequestStats
  {
    private const string RequestStageCreated = "Created";
    private const string RequestStageChannelAcquisitionStarted = "ChannelAcquisitionStarted";
    private const string RequestStagePipelined = "Pipelined";
    private const string RequestStageSent = "Transit Time";
    private const string RequestStageReceived = "Received";
    private const string RequestStageCompleted = "Completed";
    private const string RequestStageFailed = "Failed";
    private readonly ValueStopwatch stopwatch;
    private readonly DateTime requestCreatedTime;
    private TimeSpan? channelAcquisitionStartedTime;
    private TimeSpan? requestPipelinedTime;
    private TimeSpan? requestSentTime;
    private TimeSpan? requestReceivedTime;
    private TimeSpan? requestCompletedTime;
    private TimeSpan? requestFailedTime;

    public TransportRequestStats()
    {
      this.CurrentStage = TransportRequestStats.RequestStage.Created;
      this.requestCreatedTime = DateTime.UtcNow;
      this.stopwatch = ValueStopwatch.StartNew();
    }

    public TransportRequestStats.RequestStage CurrentStage { get; private set; }

    public long? RequestSizeInBytes { get; set; }

    public long? RequestBodySizeInBytes { get; set; }

    public long? ResponseMetadataSizeInBytes { get; set; }

    public long? ResponseBodySizeInBytes { get; set; }

    public int? NumberOfInflightRequestsToEndpoint { get; set; }

    public int? NumberOfOpenConnectionsToEndpoint { get; set; }

    public bool? RequestWaitingForConnectionInitialization { get; set; }

    public int? NumberOfInflightRequestsInConnection { get; set; }

    public DateTime? ConnectionLastSendAttemptTime { get; set; }

    public DateTime? ConnectionLastSendTime { get; set; }

    public DateTime? ConnectionLastReceiveTime { get; set; }

    public void RecordState(TransportRequestStats.RequestStage requestStage)
    {
      TimeSpan elapsed = this.stopwatch.Elapsed;
      switch (requestStage)
      {
        case TransportRequestStats.RequestStage.ChannelAcquisitionStarted:
          this.channelAcquisitionStartedTime = new TimeSpan?(elapsed);
          this.CurrentStage = TransportRequestStats.RequestStage.ChannelAcquisitionStarted;
          break;
        case TransportRequestStats.RequestStage.Pipelined:
          this.requestPipelinedTime = new TimeSpan?(elapsed);
          this.CurrentStage = TransportRequestStats.RequestStage.Pipelined;
          break;
        case TransportRequestStats.RequestStage.Sent:
          this.requestSentTime = new TimeSpan?(elapsed);
          this.CurrentStage = TransportRequestStats.RequestStage.Sent;
          break;
        case TransportRequestStats.RequestStage.Received:
          this.requestReceivedTime = new TimeSpan?(elapsed);
          this.CurrentStage = TransportRequestStats.RequestStage.Received;
          break;
        case TransportRequestStats.RequestStage.Completed:
          this.requestCompletedTime = new TimeSpan?(elapsed);
          this.CurrentStage = TransportRequestStats.RequestStage.Completed;
          break;
        case TransportRequestStats.RequestStage.Failed:
          this.requestFailedTime = new TimeSpan?(elapsed);
          this.CurrentStage = TransportRequestStats.RequestStage.Failed;
          break;
        default:
          throw new InvalidOperationException(string.Format("No transition to {0}", (object) requestStage));
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      this.AppendJsonString(stringBuilder);
      return stringBuilder.ToString();
    }

    public void AppendJsonString(StringBuilder stringBuilder)
    {
      stringBuilder.Append("{\"requestTimeline\":[");
      TransportRequestStats.AppendRequestStats(stringBuilder, "Created", this.requestCreatedTime, TimeSpan.Zero, this.channelAcquisitionStartedTime, this.requestFailedTime);
      if (this.channelAcquisitionStartedTime.HasValue)
      {
        stringBuilder.Append(",");
        TransportRequestStats.AppendRequestStats(stringBuilder, "ChannelAcquisitionStarted", this.requestCreatedTime, this.channelAcquisitionStartedTime.Value, this.requestPipelinedTime, this.requestFailedTime);
      }
      if (this.requestPipelinedTime.HasValue)
      {
        stringBuilder.Append(",");
        TransportRequestStats.AppendRequestStats(stringBuilder, "Pipelined", this.requestCreatedTime, this.requestPipelinedTime.Value, this.requestSentTime, this.requestFailedTime);
      }
      if (this.requestSentTime.HasValue)
      {
        stringBuilder.Append(",");
        TransportRequestStats.AppendRequestStats(stringBuilder, "Transit Time", this.requestCreatedTime, this.requestSentTime.Value, this.requestReceivedTime, this.requestFailedTime);
      }
      if (this.requestReceivedTime.HasValue)
      {
        stringBuilder.Append(",");
        TransportRequestStats.AppendRequestStats(stringBuilder, "Received", this.requestCreatedTime, this.requestReceivedTime.Value, this.requestCompletedTime, this.requestFailedTime);
      }
      if (this.requestCompletedTime.HasValue)
      {
        stringBuilder.Append(",");
        TransportRequestStats.AppendRequestStats(stringBuilder, "Completed", this.requestCreatedTime, this.requestCompletedTime.Value, this.requestCompletedTime, this.requestFailedTime);
      }
      if (this.requestFailedTime.HasValue)
      {
        stringBuilder.Append(",");
        TransportRequestStats.AppendRequestStats(stringBuilder, "Failed", this.requestCreatedTime, this.requestFailedTime.Value, this.requestFailedTime, this.requestFailedTime);
      }
      stringBuilder.Append("]");
      this.AppendServiceEndpointStats(stringBuilder);
      this.AppendConnectionStats(stringBuilder);
      long? nullable = this.RequestSizeInBytes;
      if (nullable.HasValue)
      {
        stringBuilder.Append(",\"requestSizeInBytes\":");
        StringBuilder stringBuilder1 = stringBuilder;
        nullable = this.RequestSizeInBytes;
        string str = nullable.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder1.Append(str);
      }
      nullable = this.RequestBodySizeInBytes;
      if (nullable.HasValue)
      {
        stringBuilder.Append(",\"requestBodySizeInBytes\":");
        StringBuilder stringBuilder2 = stringBuilder;
        nullable = this.RequestBodySizeInBytes;
        string str = nullable.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder2.Append(str);
      }
      nullable = this.ResponseMetadataSizeInBytes;
      if (nullable.HasValue)
      {
        stringBuilder.Append(",\"responseMetadataSizeInBytes\":");
        StringBuilder stringBuilder3 = stringBuilder;
        nullable = this.ResponseMetadataSizeInBytes;
        string str = nullable.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder3.Append(str);
      }
      nullable = this.ResponseBodySizeInBytes;
      if (nullable.HasValue)
      {
        stringBuilder.Append(",\"responseBodySizeInBytes\":");
        StringBuilder stringBuilder4 = stringBuilder;
        nullable = this.ResponseBodySizeInBytes;
        string str = nullable.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder4.Append(str);
      }
      stringBuilder.Append("}");
    }

    private void AppendServiceEndpointStats(StringBuilder stringBuilder)
    {
      stringBuilder.Append(",\"serviceEndpointStats\":");
      stringBuilder.Append("{");
      if (this.NumberOfInflightRequestsToEndpoint.HasValue)
      {
        stringBuilder.Append("\"inflightRequests\":");
        stringBuilder.Append(this.NumberOfInflightRequestsToEndpoint.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      int? connectionsToEndpoint = this.NumberOfOpenConnectionsToEndpoint;
      if (connectionsToEndpoint.HasValue)
      {
        stringBuilder.Append(",\"openConnections\":");
        StringBuilder stringBuilder1 = stringBuilder;
        connectionsToEndpoint = this.NumberOfOpenConnectionsToEndpoint;
        string str = connectionsToEndpoint.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder1.Append(str);
      }
      stringBuilder.Append("}");
    }

    private void AppendConnectionStats(StringBuilder stringBuilder)
    {
      stringBuilder.Append(",\"connectionStats\":");
      stringBuilder.Append("{");
      if (this.RequestWaitingForConnectionInitialization.HasValue)
      {
        stringBuilder.Append("\"waitforConnectionInit\":\"");
        stringBuilder.Append(this.RequestWaitingForConnectionInitialization.Value.ToString());
        stringBuilder.Append("\"");
      }
      int? requestsInConnection = this.NumberOfInflightRequestsInConnection;
      if (requestsInConnection.HasValue)
      {
        stringBuilder.Append(",\"callsPendingReceive\":");
        StringBuilder stringBuilder1 = stringBuilder;
        requestsInConnection = this.NumberOfInflightRequestsInConnection;
        string str = requestsInConnection.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder1.Append(str);
      }
      DateTime? nullable = this.ConnectionLastSendAttemptTime;
      if (nullable.HasValue)
      {
        stringBuilder.Append(",\"lastSendAttempt\":\"");
        StringBuilder stringBuilder2 = stringBuilder;
        nullable = this.ConnectionLastSendAttemptTime;
        string str = nullable.Value.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder2.Append(str);
        stringBuilder.Append("\"");
      }
      nullable = this.ConnectionLastSendTime;
      if (nullable.HasValue)
      {
        stringBuilder.Append(",\"lastSend\":\"");
        StringBuilder stringBuilder3 = stringBuilder;
        nullable = this.ConnectionLastSendTime;
        string str = nullable.Value.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder3.Append(str);
        stringBuilder.Append("\"");
      }
      nullable = this.ConnectionLastReceiveTime;
      if (nullable.HasValue)
      {
        stringBuilder.Append(",\"lastReceive\":\"");
        StringBuilder stringBuilder4 = stringBuilder;
        nullable = this.ConnectionLastReceiveTime;
        string str = nullable.Value.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture);
        stringBuilder4.Append(str);
        stringBuilder.Append("\"");
      }
      stringBuilder.Append("}");
    }

    private static void AppendRequestStats(
      StringBuilder stringBuilder,
      string eventName,
      DateTime requestStartTime,
      TimeSpan startTime,
      TimeSpan? endTime,
      TimeSpan? failedTime)
    {
      stringBuilder.Append("{\"event\": \"");
      stringBuilder.Append(eventName);
      stringBuilder.Append("\", \"startTimeUtc\": \"");
      stringBuilder.Append((requestStartTime + startTime).ToString("o", (IFormatProvider) CultureInfo.InvariantCulture));
      stringBuilder.Append("\", \"durationInMs\": ");
      TimeSpan? nullable = endTime ?? failedTime;
      if (nullable.HasValue)
        stringBuilder.Append((nullable.Value - startTime).TotalMilliseconds.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      else
        stringBuilder.Append("\"Not Set\"");
      stringBuilder.Append("}");
    }

    public enum RequestStage
    {
      Created,
      ChannelAcquisitionStarted,
      Pipelined,
      Sent,
      Received,
      Completed,
      Failed,
    }
  }
}
