// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.EventLogger
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using Microsoft.Azure.NotificationHubs.Common.Diagnostics;
using Microsoft.Azure.NotificationHubs.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal class EventLogger
  {
    private DiagnosticTrace diagnosticTrace;
    private string eventLogSourceName;

    [Obsolete("For SMDiagnostics.dll use only. Call DiagnosticUtility.EventLog instead")]
    internal EventLogger(string eventLogSourceName, object diagnosticTrace)
    {
      this.eventLogSourceName = eventLogSourceName;
      this.diagnosticTrace = (DiagnosticTrace) diagnosticTrace;
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

    internal void LogEvent(
      TraceEventType type,
      EventLogCategory category,
      EventLogEventId eventId,
      bool shouldTrace,
      params string[] values)
    {
      try
      {
        int num1 = 0;
        string[] strArray1 = new string[values.Length + 2];
        for (int index = 0; index < values.Length; ++index)
        {
          string str1 = values[index];
          string str2 = string.IsNullOrEmpty(str1) ? string.Empty : EventLogger.NormalizeEventLogParameter(str1);
          strArray1[index] = str2;
          num1 += str2.Length + 1;
        }
        string str3 = EventLogger.NormalizeEventLogParameter(DiagnosticTrace.ProcessName);
        string[] strArray2 = strArray1;
        strArray2[strArray2.Length - 2] = str3;
        int num2 = num1 + (str3.Length + 1);
        string str4 = DiagnosticTrace.ProcessId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        string[] strArray3 = strArray1;
        strArray3[strArray3.Length - 1] = str4;
        if (num2 + (str4.Length + 1) > 25600)
        {
          int length = 25600 / strArray1.Length - 1;
          for (int index = 0; index < strArray1.Length; ++index)
          {
            if (strArray1[index].Length > length)
              strArray1[index] = strArray1[index].Substring(0, length);
          }
        }
        using (SafeEventLogWriteHandle eventLogWriteHandle = SafeEventLogWriteHandle.RegisterEventSource((string) null, this.eventLogSourceName))
        {
          if (eventLogWriteHandle != null)
          {
            SecurityIdentifier user = WindowsIdentity.GetCurrent().User;
            byte[] numArray1 = new byte[user.BinaryLength];
            user.GetBinaryForm(numArray1, 0);
            IntPtr[] numArray2 = new IntPtr[strArray1.Length];
            GCHandle gcHandle1 = GCHandle.Alloc((object) numArray2, GCHandleType.Pinned);
            GCHandle[] gcHandleArray = (GCHandle[]) null;
            try
            {
              gcHandleArray = new GCHandle[strArray1.Length];
              for (int index = 0; index < strArray1.Length; ++index)
              {
                gcHandleArray[index] = GCHandle.Alloc((object) strArray1[index], GCHandleType.Pinned);
                numArray2[index] = gcHandleArray[index].AddrOfPinnedObject();
              }
              HandleRef strings = new HandleRef((object) eventLogWriteHandle, gcHandle1.AddrOfPinnedObject());
              NativeMethods.ReportEvent((SafeHandle) eventLogWriteHandle, (ushort) EventLogger.EventLogEntryTypeFromEventType(type), (ushort) category, (uint) eventId, numArray1, (ushort) strArray1.Length, 0U, strings, (byte[]) null);
            }
            finally
            {
              if (gcHandle1.AddrOfPinnedObject() != IntPtr.Zero)
                gcHandle1.Free();
              if (gcHandleArray != null)
              {
                foreach (GCHandle gcHandle2 in gcHandleArray)
                  gcHandle2.Free();
              }
            }
          }
        }
        if (!shouldTrace || this.diagnosticTrace == null)
          return;
        Dictionary<string, string> dictionary = new Dictionary<string, string>(strArray1.Length + 4);
        dictionary["CategoryID.Name"] = category.ToString();
        dictionary["CategoryID.Value"] = ((uint) category).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        dictionary["InstanceID.Name"] = eventId.ToString();
        dictionary["InstanceID.Value"] = ((uint) eventId).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        for (int index = 0; index < values.Length; ++index)
          dictionary.Add("Value" + index.ToString((IFormatProvider) CultureInfo.InvariantCulture), values[index] == null ? string.Empty : DiagnosticTrace.XmlEncode(values[index]));
        this.diagnosticTrace.TraceEvent(type, TraceCode.EventLog, Microsoft.Azure.NotificationHubs.SR.GetString(Resources.TraceCodeEventLog), (TraceRecord) new DictionaryTraceRecord((IDictionary) dictionary), (Exception) null, (object) null);
      }
      catch (Exception ex)
      {
        if (!Fx.IsFatal(ex))
          return;
        throw;
      }
    }

    internal void LogEvent(
      TraceEventType type,
      EventLogCategory category,
      EventLogEventId eventId,
      params string[] values)
    {
      this.LogEvent(type, category, eventId, true, values);
    }

    internal static string NormalizeEventLogParameter(string param)
    {
      if (param.IndexOf('%') < 0)
        return param;
      StringBuilder stringBuilder = (StringBuilder) null;
      int length = param.Length;
      for (int index1 = 0; index1 < length; ++index1)
      {
        char ch = param[index1];
        if (ch != '%')
          stringBuilder?.Append(ch);
        else if (index1 + 1 >= length)
          stringBuilder?.Append(ch);
        else if (param[index1 + 1] < '0' || param[index1 + 1] > '9')
        {
          stringBuilder?.Append(ch);
        }
        else
        {
          if (stringBuilder == null)
          {
            stringBuilder = new StringBuilder(length + 2);
            for (int index2 = 0; index2 < index1; ++index2)
              stringBuilder.Append(param[index2]);
          }
          stringBuilder.Append(ch);
          stringBuilder.Append(' ');
        }
      }
      return stringBuilder == null ? param : stringBuilder.ToString();
    }
  }
}
