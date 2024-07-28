// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.JsonSerializer
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Platform;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal static class JsonSerializer
  {
    private static readonly UTF8Encoding TransmissionEncoding = new UTF8Encoding(false);

    internal static string CompressionType => "gzip";

    internal static byte[] Serialize(IEnumerable<ITelemetry> telemetryItems, bool compress = true)
    {
      MemoryStream memoryStream = new MemoryStream();
      using (Stream stream = compress ? JsonSerializer.CreateCompressedStream((Stream) memoryStream) : (Stream) memoryStream)
      {
        using (StreamWriter streamWriter = new StreamWriter(stream, (Encoding) JsonSerializer.TransmissionEncoding))
          JsonSerializer.SeializeToStream(telemetryItems, (TextWriter) streamWriter);
      }
      return memoryStream.ToArray();
    }

    internal static byte[] Serialize(ITelemetry telemetryItem, bool compress = true) => JsonSerializer.Serialize((IEnumerable<ITelemetry>) new ITelemetry[1]
    {
      telemetryItem
    }, compress);

    internal static string SerializeAsString(IEnumerable<ITelemetry> telemetryItems)
    {
      StringBuilder sb = new StringBuilder();
      using (StringWriter streamWriter = new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture))
      {
        JsonSerializer.SeializeToStream(telemetryItems, (TextWriter) streamWriter);
        return sb.ToString();
      }
    }

    internal static string SerializeAsString(ITelemetry telemetry) => JsonSerializer.SerializeAsString((IEnumerable<ITelemetry>) new ITelemetry[1]
    {
      telemetry
    });

    private static void ConvertExceptionTree(
      Exception exception,
      ExceptionDetails parentExceptionDetails,
      List<ExceptionDetails> exceptions)
    {
      if (exception == null)
        exception = new Exception(Utils.PopulateRequiredStringValue((string) null, "message", typeof (ExceptionTelemetry).FullName));
      ExceptionDetails exceptionDetails = PlatformSingleton.Current.GetExceptionDetails(exception, parentExceptionDetails);
      exceptions.Add(exceptionDetails);
      if (exception is AggregateException aggregateException)
      {
        foreach (Exception innerException in aggregateException.InnerExceptions)
          JsonSerializer.ConvertExceptionTree(innerException, exceptionDetails, exceptions);
      }
      else
      {
        if (exception.InnerException == null)
          return;
        JsonSerializer.ConvertExceptionTree(exception.InnerException, exceptionDetails, exceptions);
      }
    }

    private static void SerializeExceptions(
      IEnumerable<ExceptionDetails> exceptions,
      IJsonWriter writer)
    {
      int num1 = 0;
      foreach (ExceptionDetails exception in exceptions)
      {
        if (num1++ != 0)
          writer.WriteComma();
        writer.WriteStartObject();
        writer.WriteProperty("id", new int?(exception.id));
        if (exception.outerId != 0)
          writer.WriteProperty("outerId", new int?(exception.outerId));
        writer.WriteProperty("typeName", Utils.PopulateRequiredStringValue(exception.typeName, "typeName", typeof (ExceptionTelemetry).FullName));
        writer.WriteProperty("message", Utils.PopulateRequiredStringValue(exception.message, "message", typeof (ExceptionTelemetry).FullName));
        if (exception.hasFullStack)
          writer.WriteProperty("hasFullStack", new bool?(exception.hasFullStack));
        writer.WriteProperty("stack", exception.stack);
        if (exception.parsedStack.Count > 0)
        {
          writer.WritePropertyName("parsedStack");
          writer.WriteStartArray();
          int num2 = 0;
          foreach (StackFrame parsed in (IEnumerable<StackFrame>) exception.parsedStack)
          {
            if (num2++ != 0)
              writer.WriteComma();
            writer.WriteStartObject();
            IJsonWriter writer1 = writer;
            JsonSerializer.SerializeStackFrame(parsed, writer1);
            writer.WriteEndObject();
          }
          writer.WriteEndArray();
        }
        writer.WriteEndObject();
      }
    }

    private static void SerializeStackFrame(StackFrame frame, IJsonWriter writer)
    {
      writer.WriteProperty("level", new int?(frame.level));
      writer.WriteProperty("method", Utils.PopulateRequiredStringValue(frame.method, "StackFrameMethod", typeof (ExceptionTelemetry).FullName));
      writer.WriteProperty("assembly", frame.assembly);
      writer.WriteProperty("fileName", frame.fileName);
      if (frame.line == 0)
        return;
      writer.WriteProperty("line", new int?(frame.line));
    }

    private static Stream CreateCompressedStream(Stream stream) => (Stream) new GZipStream(stream, CompressionMode.Compress);

    private static void SerializeTelemetryItem(ITelemetry telemetryItem, JsonWriter jsonWriter)
    {
      switch (telemetryItem)
      {
        case EventTelemetry _:
          JsonSerializer.SerializeEventTelemetry(telemetryItem as EventTelemetry, jsonWriter);
          break;
        case ExceptionTelemetry _:
          JsonSerializer.SerializeExceptionTelemetry(telemetryItem as ExceptionTelemetry, jsonWriter);
          break;
        case MetricTelemetry _:
          JsonSerializer.SerializeMetricTelemetry(telemetryItem as MetricTelemetry, jsonWriter);
          break;
        case PageViewTelemetry _:
          JsonSerializer.SerializePageViewTelemetry(telemetryItem as PageViewTelemetry, jsonWriter);
          break;
        case RemoteDependencyTelemetry _:
          JsonSerializer.SerializeRemoteDependencyTelemetry(telemetryItem as RemoteDependencyTelemetry, jsonWriter);
          break;
        case RequestTelemetry _:
          JsonSerializer.SerializeRequestTelemetry(telemetryItem as RequestTelemetry, jsonWriter);
          break;
        case SessionStateTelemetry _:
          JsonSerializer.SerializeSessionStateTelemetry(telemetryItem as SessionStateTelemetry, jsonWriter);
          break;
        case TraceTelemetry _:
          JsonSerializer.SerializeTraceTelemetry(telemetryItem as TraceTelemetry, jsonWriter);
          break;
        case PerformanceCounterTelemetry _:
          JsonSerializer.SerializePerformanceCounter(telemetryItem as PerformanceCounterTelemetry, jsonWriter);
          break;
        default:
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unknown telemtry type: {0}", new object[1]
          {
            (object) telemetryItem.GetType()
          });
          CoreEventSource.Log.LogVerbose(message);
          break;
      }
    }

    private static void SeializeToStream(
      IEnumerable<ITelemetry> telemetryItems,
      TextWriter streamWriter)
    {
      JsonWriter jsonWriter1 = new JsonWriter(streamWriter);
      int num = 0;
      foreach (ITelemetry telemetryItem in telemetryItems)
      {
        if (num++ > 0)
          streamWriter.Write(Environment.NewLine);
        JsonWriter jsonWriter2 = jsonWriter1;
        JsonSerializer.SerializeTelemetryItem(telemetryItem, jsonWriter2);
      }
    }

    private static void SerializeEventTelemetry(EventTelemetry eventTelemetry, JsonWriter writer)
    {
      writer.WriteStartObject();
      eventTelemetry.WriteTelemetryName((IJsonWriter) writer, string.Empty);
      eventTelemetry.WriteEnvelopeProperties((IJsonWriter) writer);
      writer.WritePropertyName("data");
      writer.WriteStartObject();
      writer.WriteProperty("baseType", eventTelemetry.BaseType);
      writer.WritePropertyName("baseData");
      writer.WriteStartObject();
      writer.WriteProperty("ver", new int?(eventTelemetry.Data.ver));
      writer.WriteProperty("name", eventTelemetry.Data.name);
      writer.WriteProperty("measurements", eventTelemetry.Data.measurements);
      writer.WriteProperty("properties", eventTelemetry.Data.properties);
      writer.WriteEndObject();
      writer.WriteEndObject();
      writer.WriteEndObject();
    }

    private static void SerializeExceptionTelemetry(
      ExceptionTelemetry exceptionTelemetry,
      JsonWriter writer)
    {
      writer.WriteStartObject();
      exceptionTelemetry.WriteTelemetryName((IJsonWriter) writer, "Exception");
      exceptionTelemetry.WriteEnvelopeProperties((IJsonWriter) writer);
      writer.WritePropertyName("data");
      writer.WriteStartObject();
      writer.WriteProperty("baseType", exceptionTelemetry.BaseType);
      writer.WritePropertyName("baseData");
      writer.WriteStartObject();
      writer.WriteProperty("ver", new int?(exceptionTelemetry.Data.ver));
      writer.WriteProperty("handledAt", Utils.PopulateRequiredStringValue(exceptionTelemetry.Data.handledAt, "handledAt", typeof (ExceptionTelemetry).FullName));
      writer.WriteProperty("properties", exceptionTelemetry.Data.properties);
      writer.WriteProperty("measurements", exceptionTelemetry.Data.measurements);
      writer.WritePropertyName("exceptions");
      writer.WriteStartArray();
      JsonSerializer.SerializeExceptions((IEnumerable<ExceptionDetails>) exceptionTelemetry.Exceptions, (IJsonWriter) writer);
      writer.WriteEndArray();
      if (exceptionTelemetry.Data.severityLevel.HasValue)
        writer.WriteProperty("severityLevel", exceptionTelemetry.Data.severityLevel.Value.ToString());
      writer.WriteEndObject();
      writer.WriteEndObject();
      writer.WriteEndObject();
    }

    private static void SerializeMetricTelemetry(MetricTelemetry metricTelemetry, JsonWriter writer)
    {
      writer.WriteStartObject();
      metricTelemetry.WriteTelemetryName((IJsonWriter) writer, "Metric");
      metricTelemetry.WriteEnvelopeProperties((IJsonWriter) writer);
      writer.WritePropertyName("data");
      writer.WriteStartObject();
      writer.WriteProperty("baseType", metricTelemetry.BaseType);
      writer.WritePropertyName("baseData");
      writer.WriteStartObject();
      writer.WriteProperty("ver", new int?(metricTelemetry.Data.ver));
      writer.WritePropertyName("metrics");
      writer.WriteStartArray();
      writer.WriteStartObject();
      writer.WriteProperty("name", metricTelemetry.Metric.name);
      writer.WriteProperty("kind", metricTelemetry.Metric.kind.ToString());
      writer.WriteProperty("value", new double?(metricTelemetry.Metric.value));
      writer.WriteProperty("count", metricTelemetry.Metric.count);
      writer.WriteProperty("min", metricTelemetry.Metric.min);
      writer.WriteProperty("max", metricTelemetry.Metric.max);
      writer.WriteProperty("stdDev", metricTelemetry.Metric.stdDev);
      writer.WriteEndObject();
      writer.WriteEndArray();
      writer.WriteProperty("properties", metricTelemetry.Data.properties);
      writer.WriteEndObject();
      writer.WriteEndObject();
      writer.WriteEndObject();
    }

    private static void SerializePageViewTelemetry(
      PageViewTelemetry pageViewTelemetry,
      JsonWriter writer)
    {
      writer.WriteStartObject();
      pageViewTelemetry.WriteTelemetryName((IJsonWriter) writer, "PageView");
      pageViewTelemetry.WriteEnvelopeProperties((IJsonWriter) writer);
      writer.WritePropertyName("data");
      writer.WriteStartObject();
      writer.WriteProperty("baseType", pageViewTelemetry.BaseType);
      writer.WritePropertyName("baseData");
      writer.WriteStartObject();
      writer.WriteProperty("ver", new int?(pageViewTelemetry.Data.ver));
      writer.WriteProperty("name", pageViewTelemetry.Data.name);
      writer.WriteProperty("url", pageViewTelemetry.Data.url);
      writer.WriteProperty("duration", pageViewTelemetry.Data.duration);
      writer.WriteProperty("measurements", pageViewTelemetry.Data.measurements);
      writer.WriteProperty("properties", pageViewTelemetry.Data.properties);
      writer.WriteEndObject();
      writer.WriteEndObject();
      writer.WriteEndObject();
    }

    private static void SerializeRemoteDependencyTelemetry(
      RemoteDependencyTelemetry remoteDependencyTelemetry,
      JsonWriter writer)
    {
      writer.WriteStartObject();
      remoteDependencyTelemetry.WriteTelemetryName((IJsonWriter) writer, "RemoteDependency");
      remoteDependencyTelemetry.WriteEnvelopeProperties((IJsonWriter) writer);
      writer.WritePropertyName("data");
      writer.WriteStartObject();
      writer.WriteProperty("baseType", remoteDependencyTelemetry.BaseType);
      writer.WritePropertyName("baseData");
      writer.WriteStartObject();
      writer.WriteProperty("ver", new int?(remoteDependencyTelemetry.Data.ver));
      writer.WriteProperty("name", remoteDependencyTelemetry.Data.name);
      writer.WriteProperty("commandName", remoteDependencyTelemetry.Data.commandName);
      writer.WriteProperty("kind", new int?((int) remoteDependencyTelemetry.Data.kind));
      writer.WriteProperty("value", new double?(remoteDependencyTelemetry.Data.value));
      writer.WriteProperty("count", remoteDependencyTelemetry.Data.count);
      writer.WriteProperty("dependencyKind", new int?((int) remoteDependencyTelemetry.Data.dependencyKind));
      writer.WriteProperty("success", remoteDependencyTelemetry.Data.success);
      writer.WriteProperty("async", remoteDependencyTelemetry.Data.async);
      writer.WriteProperty("dependencySource", new int?((int) remoteDependencyTelemetry.Data.dependencySource));
      writer.WriteProperty("properties", remoteDependencyTelemetry.Data.properties);
      writer.WriteEndObject();
      writer.WriteEndObject();
      writer.WriteEndObject();
    }

    private static void SerializeRequestTelemetry(
      RequestTelemetry requestTelemetry,
      JsonWriter jsonWriter)
    {
      jsonWriter.WriteStartObject();
      requestTelemetry.WriteTelemetryName((IJsonWriter) jsonWriter, "Request");
      requestTelemetry.WriteEnvelopeProperties((IJsonWriter) jsonWriter);
      jsonWriter.WritePropertyName("data");
      jsonWriter.WriteStartObject();
      jsonWriter.WriteProperty("baseType", requestTelemetry.BaseType);
      jsonWriter.WritePropertyName("baseData");
      jsonWriter.WriteStartObject();
      jsonWriter.WriteProperty("ver", new int?(requestTelemetry.Data.ver));
      jsonWriter.WriteProperty("id", requestTelemetry.Data.id);
      jsonWriter.WriteProperty("name", requestTelemetry.Data.name);
      jsonWriter.WriteProperty("startTime", new DateTimeOffset?(requestTelemetry.Timestamp));
      jsonWriter.WriteProperty("duration", new TimeSpan?(requestTelemetry.Duration));
      jsonWriter.WriteProperty("success", new bool?(requestTelemetry.Data.success));
      jsonWriter.WriteProperty("responseCode", requestTelemetry.Data.responseCode);
      jsonWriter.WriteProperty("url", requestTelemetry.Data.url);
      jsonWriter.WriteProperty("measurements", requestTelemetry.Data.measurements);
      jsonWriter.WriteProperty("httpMethod", requestTelemetry.Data.httpMethod);
      jsonWriter.WriteProperty("properties", requestTelemetry.Data.properties);
      jsonWriter.WriteEndObject();
      jsonWriter.WriteEndObject();
      jsonWriter.WriteEndObject();
    }

    private static void SerializeSessionStateTelemetry(
      SessionStateTelemetry sessionStateTelemetry,
      JsonWriter jsonWriter)
    {
      jsonWriter.WriteStartObject();
      sessionStateTelemetry.WriteEnvelopeProperties((IJsonWriter) jsonWriter);
      sessionStateTelemetry.WriteTelemetryName((IJsonWriter) jsonWriter, "SessionState");
      jsonWriter.WritePropertyName("data");
      jsonWriter.WriteStartObject();
      jsonWriter.WriteProperty("baseType", typeof (SessionStateData).Name);
      jsonWriter.WritePropertyName("baseData");
      jsonWriter.WriteStartObject();
      jsonWriter.WriteProperty("ver", new int?(2));
      jsonWriter.WriteProperty("state", sessionStateTelemetry.State.ToString());
      jsonWriter.WriteEndObject();
      jsonWriter.WriteEndObject();
      jsonWriter.WriteEndObject();
    }

    private static void SerializeTraceTelemetry(TraceTelemetry traceTelemetry, JsonWriter writer)
    {
      writer.WriteStartObject();
      traceTelemetry.WriteTelemetryName((IJsonWriter) writer, "Message");
      traceTelemetry.WriteEnvelopeProperties((IJsonWriter) writer);
      writer.WritePropertyName("data");
      writer.WriteStartObject();
      writer.WriteProperty("baseType", traceTelemetry.BaseType);
      writer.WritePropertyName("baseData");
      writer.WriteStartObject();
      writer.WriteProperty("ver", new int?(traceTelemetry.Data.ver));
      writer.WriteProperty("message", traceTelemetry.Message);
      if (traceTelemetry.SeverityLevel.HasValue)
        writer.WriteProperty("severityLevel", traceTelemetry.SeverityLevel.Value.ToString());
      writer.WriteProperty("properties", traceTelemetry.Properties);
      writer.WriteEndObject();
      writer.WriteEndObject();
      writer.WriteEndObject();
    }

    private static void SerializePerformanceCounter(
      PerformanceCounterTelemetry performanceCounter,
      JsonWriter writer)
    {
      writer.WriteStartObject();
      performanceCounter.WriteTelemetryName((IJsonWriter) writer, "PerformanceCounter");
      performanceCounter.WriteEnvelopeProperties((IJsonWriter) writer);
      writer.WritePropertyName("data");
      writer.WriteStartObject();
      writer.WriteProperty("baseType", performanceCounter.BaseType);
      writer.WritePropertyName("baseData");
      writer.WriteStartObject();
      writer.WriteProperty("ver", new int?(performanceCounter.Data.ver));
      writer.WriteProperty("categoryName", performanceCounter.Data.categoryName);
      writer.WriteProperty("counterName", performanceCounter.Data.counterName);
      writer.WriteProperty("instanceName", performanceCounter.Data.instanceName);
      writer.WriteProperty("value", new double?(performanceCounter.Data.value));
      writer.WriteProperty("properties", performanceCounter.Data.properties);
      writer.WriteEndObject();
      writer.WriteEndObject();
      writer.WriteEndObject();
    }
  }
}
