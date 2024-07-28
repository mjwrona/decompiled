// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.Keychain.Logging.DiagnosticsLogging
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Client.AccountManagement.Logging;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Client.Keychain.Logging
{
  public static class DiagnosticsLogging
  {
    public const string EventTypeProperty = "EventType";
    public const string DiagnosticsLoggingEventType = "DiagnosticLoggingEvent";
    public const string MessagePropertyName = "Message";
    public const string ExceptionPropertyName = "Exception";
    public const string SeverityPropertyName = "Severity";

    public static void LogInformationEvent(
      ILogger logger,
      string eventName,
      string message,
      IDictionary<string, object> additionalMetadata = null,
      Exception exception = null)
    {
      DiagnosticsLogging.LogEvent(logger, DiagnosticsLoggingMessageSeverity.Information, eventName, message, additionalMetadata, exception);
    }

    public static void LogErrorEvent(
      ILogger logger,
      string eventName,
      string message,
      IDictionary<string, object> additionalMetadata = null,
      Exception exception = null)
    {
      DiagnosticsLogging.LogEvent(logger, DiagnosticsLoggingMessageSeverity.Error, eventName, message, additionalMetadata, exception);
    }

    private static void LogEvent(
      ILogger logger,
      DiagnosticsLoggingMessageSeverity severity,
      string eventName,
      string message,
      IDictionary<string, object> additionalMetadata = null,
      Exception e = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(eventName, nameof (eventName));
      ArgumentUtility.CheckForNull<string>(message, nameof (message));
      Dictionary<string, object> properties = additionalMetadata != null ? new Dictionary<string, object>(additionalMetadata) : new Dictionary<string, object>();
      properties.Add("EventType", (object) "DiagnosticLoggingEvent");
      properties.Add("Message", (object) message);
      properties.Add("Exception", (object) e);
      properties.Add("Severity", (object) severity);
      logger.LogEvent(eventName, (IDictionary<string, object>) properties);
    }
  }
}
