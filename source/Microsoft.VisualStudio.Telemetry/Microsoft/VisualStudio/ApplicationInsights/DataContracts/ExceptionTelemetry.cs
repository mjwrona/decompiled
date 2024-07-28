// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.DataContracts.ExceptionTelemetry
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Platform;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.ApplicationInsights.DataContracts
{
  public sealed class ExceptionTelemetry : ITelemetry, ISupportProperties
  {
    internal const string TelemetryName = "Exception";
    internal readonly string BaseType = typeof (ExceptionData).Name;
    internal readonly ExceptionData Data;
    private readonly TelemetryContext context;
    private Exception exception;

    public ExceptionTelemetry()
    {
      this.Data = new ExceptionData();
      this.context = new TelemetryContext(this.Data.properties, (IDictionary<string, string>) new Dictionary<string, string>());
      this.HandledAt = ExceptionHandledAt.Unhandled;
    }

    public ExceptionTelemetry(Exception exception)
      : this()
    {
      if (exception == null)
        exception = new Exception(Utils.PopulateRequiredStringValue((string) null, "message", typeof (ExceptionTelemetry).FullName));
      this.Exception = exception;
    }

    public DateTimeOffset Timestamp { get; set; }

    public string Sequence { get; set; }

    public TelemetryContext Context => this.context;

    public ExceptionHandledAt HandledAt
    {
      get => this.ValidateExceptionHandledAt(this.Data.handledAt);
      set => this.Data.handledAt = value.ToString();
    }

    public Exception Exception
    {
      get => this.exception;
      set
      {
        this.exception = value;
        this.UpdateExceptions(value);
      }
    }

    public IDictionary<string, double> Metrics => this.Data.measurements;

    public IDictionary<string, string> Properties => this.Data.properties;

    public Microsoft.VisualStudio.ApplicationInsights.DataContracts.SeverityLevel? SeverityLevel
    {
      get => this.Data.severityLevel.TranslateSeverityLevel();
      set => this.Data.severityLevel = value.TranslateSeverityLevel();
    }

    internal IList<ExceptionDetails> Exceptions => this.Data.exceptions;

    void ITelemetry.Sanitize()
    {
      this.Properties.SanitizeProperties();
      this.Metrics.SanitizeMeasurements();
    }

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
          ExceptionTelemetry.ConvertExceptionTree(innerException, exceptionDetails, exceptions);
      }
      else
      {
        if (exception.InnerException == null)
          return;
        ExceptionTelemetry.ConvertExceptionTree(exception.InnerException, exceptionDetails, exceptions);
      }
    }

    private void UpdateExceptions(Exception exception)
    {
      List<ExceptionDetails> exceptions = new List<ExceptionDetails>();
      ExceptionTelemetry.ConvertExceptionTree(exception, (ExceptionDetails) null, exceptions);
      if (exceptions.Count > 10)
      {
        InnerExceptionCountExceededException exceededException = new InnerExceptionCountExceededException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The number of inner exceptions was {0} which is larger than {1}, the maximum number allowed during transmission. All but the first {1} have been dropped.", new object[2]
        {
          (object) exceptions.Count,
          (object) 10
        }));
        exceptions.RemoveRange(10, exceptions.Count - 10);
        exceptions.Add(PlatformSingleton.Current.GetExceptionDetails((Exception) exceededException, exceptions[0]));
      }
      this.Data.exceptions = (IList<ExceptionDetails>) exceptions;
    }

    private ExceptionHandledAt ValidateExceptionHandledAt(string value)
    {
      ExceptionHandledAt exceptionHandledAt = ExceptionHandledAt.Unhandled;
      if (Enum.IsDefined(typeof (ExceptionHandledAt), (object) value))
        exceptionHandledAt = (ExceptionHandledAt) Enum.Parse(typeof (ExceptionHandledAt), value);
      return exceptionHandledAt;
    }
  }
}
