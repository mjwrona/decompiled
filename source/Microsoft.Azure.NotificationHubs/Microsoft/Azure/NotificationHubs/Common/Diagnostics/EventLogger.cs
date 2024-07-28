// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.Diagnostics.EventLogger
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Common.Diagnostics
{
  internal sealed class EventLogger
  {
    private const int MaxEventLogsInPT = 5;
    [SecurityCritical]
    private static int logCountForPT;
    private static bool canLogEvent = true;
    private DiagnosticTrace diagnosticTrace;
    [SecurityCritical]
    private string eventLogSourceName;
    private bool isInPartialTrust;

    private EventLogger() => this.isInPartialTrust = EventLogger.IsInPartialTrust();

    [Obsolete("For System.Runtime.dll use only. Call FxTrace.EventLog instead")]
    public EventLogger(string eventLogSourceName, DiagnosticTrace diagnosticTrace)
    {
      try
      {
        this.diagnosticTrace = diagnosticTrace;
        if (!EventLogger.canLogEvent)
          return;
        this.SafeSetLogSourceName(eventLogSourceName);
      }
      catch (SecurityException ex)
      {
        EventLogger.canLogEvent = false;
      }
    }

    [SecurityCritical]
    public static EventLogger UnsafeCreateEventLogger(
      string eventLogSourceName,
      DiagnosticTrace diagnosticTrace)
    {
      EventLogger eventLogger = new EventLogger();
      eventLogger.SetLogSourceName(eventLogSourceName, diagnosticTrace);
      return eventLogger;
    }

    [SecurityCritical]
    public void UnsafeLogEvent(
      TraceEventType type,
      ushort eventLogCategory,
      uint eventId,
      bool shouldTrace,
      params string[] values)
    {
      if (EventLogger.logCountForPT >= 5)
        return;
      try
      {
        int num1 = 0;
        string[] logValues = new string[values.Length + 2];
        for (int index = 0; index < values.Length; ++index)
        {
          string eventLogParameter = values[index];
          string str = string.IsNullOrEmpty(eventLogParameter) ? string.Empty : EventLogger.NormalizeEventLogParameter(eventLogParameter);
          logValues[index] = str;
          num1 += str.Length + 1;
        }
        string str1 = EventLogger.NormalizeEventLogParameter(EventLogger.UnsafeGetProcessName());
        string[] strArray1 = logValues;
        strArray1[strArray1.Length - 2] = str1;
        int num2 = num1 + (str1.Length + 1);
        string str2 = EventLogger.UnsafeGetProcessId().ToString((IFormatProvider) CultureInfo.InvariantCulture);
        string[] strArray2 = logValues;
        strArray2[strArray2.Length - 1] = str2;
        if (num2 + (str2.Length + 1) > 25600)
        {
          int length = 25600 / logValues.Length - 1;
          for (int index = 0; index < logValues.Length; ++index)
          {
            if (logValues[index].Length > length)
              logValues[index] = logValues[index].Substring(0, length);
          }
        }
        SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
        byte[] numArray1 = new byte[user.BinaryLength];
        user.GetBinaryForm(numArray1, 0);
        IntPtr[] numArray2 = new IntPtr[logValues.Length];
        GCHandle stringsRootHandle = new GCHandle();
        GCHandle[] gcHandleArray = (GCHandle[]) null;
        try
        {
          stringsRootHandle = GCHandle.Alloc((object) numArray2, GCHandleType.Pinned);
          gcHandleArray = new GCHandle[logValues.Length];
          for (int index = 0; index < logValues.Length; ++index)
          {
            gcHandleArray[index] = GCHandle.Alloc((object) logValues[index], GCHandleType.Pinned);
            numArray2[index] = gcHandleArray[index].AddrOfPinnedObject();
          }
          this.UnsafeWriteEventLog(type, eventLogCategory, eventId, logValues, numArray1, stringsRootHandle);
        }
        finally
        {
          if (stringsRootHandle.AddrOfPinnedObject() != IntPtr.Zero)
            stringsRootHandle.Free();
          if (gcHandleArray != null)
          {
            foreach (GCHandle gcHandle in gcHandleArray)
              gcHandle.Free();
          }
        }
        if (shouldTrace)
        {
          if (this.diagnosticTrace != null)
          {
            if (!TraceCore.TraceCodeEventLogCriticalIsEnabled(this.diagnosticTrace) && !TraceCore.TraceCodeEventLogVerboseIsEnabled(this.diagnosticTrace) && !TraceCore.TraceCodeEventLogInfoIsEnabled(this.diagnosticTrace) && !TraceCore.TraceCodeEventLogWarningIsEnabled(this.diagnosticTrace))
            {
              if (!TraceCore.TraceCodeEventLogErrorIsEnabled(this.diagnosticTrace))
                goto label_39;
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>(logValues.Length + 4);
            dictionary["CategoryID.Name"] = "EventLogCategory";
            dictionary["CategoryID.Value"] = eventLogCategory.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            dictionary["InstanceID.Name"] = "EventId";
            dictionary["InstanceID.Value"] = eventId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            for (int index = 0; index < values.Length; ++index)
              dictionary.Add("Value" + index.ToString((IFormatProvider) CultureInfo.InvariantCulture), values[index] == null ? string.Empty : DiagnosticTrace.XmlEncode(values[index]));
            TraceRecord traceRecord = (TraceRecord) new DictionaryTraceRecord((IDictionary) dictionary);
            switch (type)
            {
              case TraceEventType.Critical:
                TraceCore.TraceCodeEventLogCritical(this.diagnosticTrace, traceRecord);
                break;
              case TraceEventType.Error:
                TraceCore.TraceCodeEventLogError(this.diagnosticTrace, traceRecord);
                break;
              case TraceEventType.Warning:
                TraceCore.TraceCodeEventLogWarning(this.diagnosticTrace, traceRecord);
                break;
              case TraceEventType.Information:
                TraceCore.TraceCodeEventLogInfo(this.diagnosticTrace, traceRecord);
                break;
              case TraceEventType.Verbose:
                TraceCore.TraceCodeEventLogVerbose(this.diagnosticTrace, traceRecord);
                break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        if (Fx.IsFatal(ex))
          throw;
      }
label_39:
      if (!this.isInPartialTrust)
        return;
      ++EventLogger.logCountForPT;
    }

    public void LogEvent(
      TraceEventType type,
      ushort eventLogCategory,
      uint eventId,
      bool shouldTrace,
      params string[] values)
    {
      if (!EventLogger.canLogEvent)
        return;
      try
      {
        this.SafeLogEvent(type, eventLogCategory, eventId, shouldTrace, values);
      }
      catch (SecurityException ex)
      {
        EventLogger.canLogEvent = false;
        if (!shouldTrace || this.diagnosticTrace == null || !TraceCore.HandledExceptionIsEnabled(this.diagnosticTrace))
          return;
        TraceCore.HandledException(this.diagnosticTrace, (Exception) ex);
      }
    }

    public void LogEvent(
      TraceEventType type,
      ushort eventLogCategory,
      uint eventId,
      params string[] values)
    {
      this.LogEvent(type, eventLogCategory, eventId, true, values);
    }

    private static EventLogEntryType EventLogEntryTypeFromEventType(TraceEventType type)
    {
      EventLogEntryType eventLogEntryType = EventLogEntryType.Information;
      switch (type)
      {
        case TraceEventType.Critical:
        case TraceEventType.Error:
          eventLogEntryType = EventLogEntryType.Error;
          break;
        case TraceEventType.Warning:
          eventLogEntryType = EventLogEntryType.Warning;
          break;
      }
      return eventLogEntryType;
    }

    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    private void SafeLogEvent(
      TraceEventType type,
      ushort eventLogCategory,
      uint eventId,
      bool shouldTrace,
      params string[] values)
    {
      this.UnsafeLogEvent(type, eventLogCategory, eventId, shouldTrace, values);
    }

    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    private void SafeSetLogSourceName(string updatedEventLogSourceName) => this.eventLogSourceName = updatedEventLogSourceName;

    [SecurityCritical]
    private void SetLogSourceName(
      string updatedEventLogSourceName,
      DiagnosticTrace updatedDiagnosticTrace)
    {
      this.eventLogSourceName = updatedEventLogSourceName;
      this.diagnosticTrace = updatedDiagnosticTrace;
    }

    private static bool IsInPartialTrust()
    {
      try
      {
        using (Process currentProcess = Process.GetCurrentProcess())
          return string.IsNullOrEmpty(currentProcess.ProcessName);
      }
      catch (SecurityException ex)
      {
        return true;
      }
    }

    [SecurityCritical]
    [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
    private void UnsafeWriteEventLog(
      TraceEventType type,
      ushort eventLogCategory,
      uint eventId,
      string[] logValues,
      byte[] sidBA,
      GCHandle stringsRootHandle)
    {
      using (SafeEventLogWriteHandle eventLogWriteHandle = SafeEventLogWriteHandle.RegisterEventSource((string) null, this.eventLogSourceName))
      {
        if (eventLogWriteHandle == null)
          return;
        HandleRef strings = new HandleRef((object) eventLogWriteHandle, stringsRootHandle.AddrOfPinnedObject());
        Microsoft.Azure.NotificationHubs.Common.Interop.UnsafeNativeMethods.ReportEvent((SafeHandle) eventLogWriteHandle, (ushort) EventLogger.EventLogEntryTypeFromEventType(type), eventLogCategory, eventId, sidBA, (ushort) logValues.Length, 0U, strings, (byte[]) null);
      }
    }

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.NoInlining)]
    [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
    private static string UnsafeGetProcessName()
    {
      using (Process currentProcess = Process.GetCurrentProcess())
        return currentProcess.ProcessName;
    }

    [SecurityCritical]
    [MethodImpl(MethodImplOptions.NoInlining)]
    [SecurityPermission(SecurityAction.Assert, UnmanagedCode = true)]
    private static int UnsafeGetProcessId()
    {
      using (Process currentProcess = Process.GetCurrentProcess())
        return currentProcess.Id;
    }

    private static string NormalizeEventLogParameter(string eventLogParameter)
    {
      if (eventLogParameter.IndexOf('%') < 0)
        return eventLogParameter;
      StringBuilder stringBuilder = (StringBuilder) null;
      int length = eventLogParameter.Length;
      for (int index1 = 0; index1 < length; ++index1)
      {
        char ch = eventLogParameter[index1];
        if (ch != '%')
          stringBuilder?.Append(ch);
        else if (index1 + 1 >= length)
          stringBuilder?.Append(ch);
        else if (eventLogParameter[index1 + 1] < '0' || eventLogParameter[index1 + 1] > '9')
        {
          stringBuilder?.Append(ch);
        }
        else
        {
          if (stringBuilder == null)
          {
            stringBuilder = new StringBuilder(length + 2);
            for (int index2 = 0; index2 < index1; ++index2)
              stringBuilder.Append(eventLogParameter[index2]);
          }
          stringBuilder.Append(ch);
          stringBuilder.Append(' ');
        }
      }
      return stringBuilder == null ? eventLogParameter : stringBuilder.ToString();
    }
  }
}
