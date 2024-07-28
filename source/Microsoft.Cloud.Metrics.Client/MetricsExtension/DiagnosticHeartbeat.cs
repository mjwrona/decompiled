// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsExtension.DiagnosticHeartbeat
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Metrics.Etw;
using System;

namespace Microsoft.Cloud.Metrics.Client.MetricsExtension
{
  internal sealed class DiagnosticHeartbeat : IDiagnosticHeartbeat
  {
    public int AggregatedMetricsDroppedCount { get; private set; }

    public int EtwEventsDroppedCount { get; private set; }

    public int EtwEventsLostCount { get; private set; }

    public string InstanceName { get; private set; }

    public bool IsNearingAggregationQueueLimit { get; private set; }

    public bool IsNearingEtwQueueLimit { get; private set; }

    public int UptimeInSec { get; private set; }

    public static unsafe IDiagnosticHeartbeat FromEtwEvent(NativeMethods.EventRecord* etwMetricData)
    {
      DiagnosticHeartbeat diagnosticHeartbeat = new DiagnosticHeartbeat();
      IntPtr userData = etwMetricData->UserData;
      diagnosticHeartbeat.InstanceName = EtwPayloadManipulationUtils.ReadString(ref userData);
      int num1 = *(int*) (void*) userData;
      IntPtr ptr1 = EtwPayloadManipulationUtils.Shift(userData, 4);
      diagnosticHeartbeat.UptimeInSec = num1;
      int num2 = *(int*) (void*) ptr1;
      IntPtr ptr2 = EtwPayloadManipulationUtils.Shift(ptr1, 4);
      diagnosticHeartbeat.EtwEventsDroppedCount = num2;
      int num3 = *(int*) (void*) ptr2;
      IntPtr ptr3 = EtwPayloadManipulationUtils.Shift(ptr2, 4);
      diagnosticHeartbeat.EtwEventsLostCount = num3;
      int num4 = *(int*) (void*) ptr3;
      IntPtr ptr4 = EtwPayloadManipulationUtils.Shift(ptr3, 4);
      diagnosticHeartbeat.AggregatedMetricsDroppedCount = num4;
      byte num5 = *(byte*) (void*) ptr4;
      IntPtr num6 = EtwPayloadManipulationUtils.Shift(ptr4, 1);
      diagnosticHeartbeat.IsNearingEtwQueueLimit = num5 > (byte) 0;
      diagnosticHeartbeat.IsNearingAggregationQueueLimit = *(byte*) (void*) num6 > (byte) 0;
      return (IDiagnosticHeartbeat) diagnosticHeartbeat;
    }
  }
}
