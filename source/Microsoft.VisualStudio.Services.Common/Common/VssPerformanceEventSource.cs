// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.VssPerformanceEventSource
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Diagnostics.Tracing;

namespace Microsoft.VisualStudio.Services.Common
{
  public sealed class VssPerformanceEventSource : EventSource
  {
    public static VssPerformanceEventSource Log = new VssPerformanceEventSource();

    [NonEvent]
    public unsafe void WriteEvent(int eventId, Guid u1, Guid u2, string st)
    {
      if (!this.IsEnabled())
        return;
      st = st ?? string.Empty;
      EventSource.EventData* data = stackalloc EventSource.EventData[3];
      data->Size = sizeof (Guid);
      data->DataPointer = (IntPtr) (void*) &u1;
      data[1].Size = sizeof (Guid);
      data[1].DataPointer = (IntPtr) (void*) &u2;
      data[2].Size = (st.Length + 1) * 2;
      fixed (char* chPtr = st)
      {
        data[2].DataPointer = (IntPtr) (void*) chPtr;
        this.WriteEventCore(eventId, 3, data);
      }
    }

    [NonEvent]
    public unsafe void WriteEvent(int eventId, Guid u1, Guid u2, string st, long duration)
    {
      if (!this.IsEnabled())
        return;
      st = st ?? string.Empty;
      EventSource.EventData* data = stackalloc EventSource.EventData[4];
      data->Size = sizeof (Guid);
      data->DataPointer = (IntPtr) (void*) &u1;
      data[1].Size = sizeof (Guid);
      data[1].DataPointer = (IntPtr) (void*) &u2;
      data[2].Size = (st.Length + 1) * 2;
      data[3].Size = 8;
      data[3].DataPointer = (IntPtr) (void*) &duration;
      fixed (char* chPtr = st)
      {
        data[2].DataPointer = (IntPtr) (void*) chPtr;
        this.WriteEventCore(eventId, 4, data);
      }
    }

    [NonEvent]
    public unsafe void WriteEvent(int eventId, Guid u, string st)
    {
      if (!this.IsEnabled())
        return;
      st = st ?? string.Empty;
      EventSource.EventData* data = stackalloc EventSource.EventData[2];
      data->DataPointer = (IntPtr) (void*) &u;
      data->Size = sizeof (Guid);
      data[1].Size = (st.Length + 1) * 2;
      fixed (char* chPtr = st)
      {
        data[1].DataPointer = (IntPtr) (void*) chPtr;
        this.WriteEventCore(eventId, 2, data);
      }
    }

    [NonEvent]
    public unsafe void WriteEvent(int eventId, Guid u, string st, long duration)
    {
      if (!this.IsEnabled())
        return;
      st = st ?? string.Empty;
      EventSource.EventData* data = stackalloc EventSource.EventData[3];
      data->DataPointer = (IntPtr) (void*) &u;
      data->Size = sizeof (Guid);
      data[1].Size = (st.Length + 1) * 2;
      data[2].Size = 8;
      data[2].DataPointer = (IntPtr) (void*) &duration;
      fixed (char* chPtr = st)
      {
        data[1].DataPointer = (IntPtr) (void*) chPtr;
        this.WriteEventCore(eventId, 3, data);
      }
    }

    [NonEvent]
    public unsafe void WriteEvent(int eventId, Guid u)
    {
      if (!this.IsEnabled())
        return;
      this.WriteEventCore(eventId, 1, &new EventSource.EventData()
      {
        DataPointer = (IntPtr) (void*) &u,
        Size = sizeof (Guid)
      });
    }

    [NonEvent]
    public unsafe void WriteEvent(int eventId, Guid u, long duration)
    {
      if (!this.IsEnabled())
        return;
      EventSource.EventData* data = stackalloc EventSource.EventData[2];
      data->DataPointer = (IntPtr) (void*) &u;
      data->Size = sizeof (Guid);
      data[1].DataPointer = (IntPtr) (void*) &duration;
      data[1].Size = 8;
      this.WriteEventCore(eventId, 2, data);
    }

    [NonEvent]
    public unsafe void WriteEvent(
      int eventId,
      Guid u1,
      string st1,
      DateTime dt1,
      DateTime dt2,
      Guid u2)
    {
      if (!this.IsEnabled())
        return;
      st1 = st1 ?? string.Empty;
      long fileTimeUtc1 = dt1.ToFileTimeUtc();
      long fileTimeUtc2 = dt2.ToFileTimeUtc();
      EventSource.EventData* data = stackalloc EventSource.EventData[5];
      data->DataPointer = (IntPtr) (void*) &u1;
      data->Size = sizeof (Guid);
      data[1].Size = (st1.Length + 1) * 2;
      data[2].DataPointer = (IntPtr) (void*) &fileTimeUtc1;
      data[2].Size = 8;
      data[3].DataPointer = (IntPtr) (void*) &fileTimeUtc2;
      data[3].Size = 8;
      data[4].DataPointer = (IntPtr) (void*) &u2;
      data[4].Size = sizeof (Guid);
      fixed (char* chPtr = st1)
      {
        data[1].DataPointer = (IntPtr) (void*) chPtr;
        this.WriteEventCore(eventId, 5, data);
      }
    }

    [NonEvent]
    public unsafe void WriteEvent(
      int eventId,
      Guid u1,
      string st1,
      string st2,
      string st3,
      Guid u2,
      long duration)
    {
      if (!this.IsEnabled())
        return;
      st1 = st1 ?? string.Empty;
      st2 = st2 ?? string.Empty;
      st3 = st3 ?? string.Empty;
      EventSource.EventData* data = stackalloc EventSource.EventData[6];
      data->DataPointer = (IntPtr) (void*) &u1;
      data->Size = sizeof (Guid);
      data[1].Size = (st1.Length + 1) * 2;
      data[2].Size = (st2.Length + 1) * 2;
      data[3].Size = (st3.Length + 1) * 2;
      data[4].DataPointer = (IntPtr) (void*) &u2;
      data[4].Size = sizeof (Guid);
      data[5].DataPointer = (IntPtr) (void*) &duration;
      data[5].Size = 8;
      fixed (char* chPtr1 = st1)
        fixed (char* chPtr2 = st2)
          fixed (char* chPtr3 = st3)
          {
            data[1].DataPointer = (IntPtr) (void*) chPtr1;
            data[2].DataPointer = (IntPtr) (void*) chPtr2;
            data[3].DataPointer = (IntPtr) (void*) chPtr3;
            this.WriteEventCore(eventId, 6, data);
          }
    }

    [NonEvent]
    public unsafe void WriteEvent(
      int eventId,
      Guid uniqueIdentifier,
      string st1,
      string st2,
      string st3)
    {
      if (!this.IsEnabled())
        return;
      st1 = st1 ?? string.Empty;
      st2 = st2 ?? string.Empty;
      st3 = st3 ?? string.Empty;
      EventSource.EventData* data = stackalloc EventSource.EventData[4];
      data->DataPointer = (IntPtr) (void*) &uniqueIdentifier;
      data->Size = sizeof (Guid);
      data[1].Size = (st1.Length + 1) * 2;
      data[2].Size = (st2.Length + 1) * 2;
      data[3].Size = (st3.Length + 1) * 2;
      fixed (char* chPtr1 = st1)
        fixed (char* chPtr2 = st2)
          fixed (char* chPtr3 = st3)
          {
            data[1].DataPointer = (IntPtr) (void*) chPtr1;
            data[2].DataPointer = (IntPtr) (void*) chPtr2;
            data[3].DataPointer = (IntPtr) (void*) chPtr3;
            this.WriteEventCore(eventId, 4, data);
          }
    }

    [NonEvent]
    public unsafe void WriteEvent(
      int eventId,
      Guid uniqueIdentifier,
      string st1,
      string st2,
      string st3,
      long duration)
    {
      if (!this.IsEnabled())
        return;
      st1 = st1 ?? string.Empty;
      st2 = st2 ?? string.Empty;
      st3 = st3 ?? string.Empty;
      EventSource.EventData* data = stackalloc EventSource.EventData[5];
      data->DataPointer = (IntPtr) (void*) &uniqueIdentifier;
      data->Size = sizeof (Guid);
      data[1].Size = (st1.Length + 1) * 2;
      data[2].Size = (st2.Length + 1) * 2;
      data[3].Size = (st3.Length + 1) * 2;
      data[4].DataPointer = (IntPtr) (void*) &duration;
      data[4].Size = 8;
      fixed (char* chPtr1 = st1)
        fixed (char* chPtr2 = st2)
          fixed (char* chPtr3 = st3)
          {
            data[1].DataPointer = (IntPtr) (void*) chPtr1;
            data[2].DataPointer = (IntPtr) (void*) chPtr2;
            data[3].DataPointer = (IntPtr) (void*) chPtr3;
            this.WriteEventCore(eventId, 5, data);
          }
    }

    [NonEvent]
    public unsafe void WriteEvent(int eventId, Guid uniqueIdentifier, string st1, string st2)
    {
      if (!this.IsEnabled())
        return;
      st1 = st1 ?? string.Empty;
      st2 = st2 ?? string.Empty;
      EventSource.EventData* data = stackalloc EventSource.EventData[3];
      data->DataPointer = (IntPtr) (void*) &uniqueIdentifier;
      data->Size = sizeof (Guid);
      data[1].Size = (st1.Length + 1) * 2;
      data[2].Size = (st2.Length + 1) * 2;
      fixed (char* chPtr1 = st1)
        fixed (char* chPtr2 = st2)
        {
          data[1].DataPointer = (IntPtr) (void*) chPtr1;
          data[2].DataPointer = (IntPtr) (void*) chPtr2;
          this.WriteEventCore(eventId, 3, data);
        }
    }

    [NonEvent]
    public unsafe void WriteEvent(
      int eventId,
      Guid uniqueIdentifier,
      string st1,
      string st2,
      long duration)
    {
      if (!this.IsEnabled())
        return;
      st1 = st1 ?? string.Empty;
      st2 = st2 ?? string.Empty;
      EventSource.EventData* data = stackalloc EventSource.EventData[4];
      data->DataPointer = (IntPtr) (void*) &uniqueIdentifier;
      data->Size = sizeof (Guid);
      data[1].Size = (st1.Length + 1) * 2;
      data[2].Size = (st2.Length + 1) * 2;
      data[3].DataPointer = (IntPtr) (void*) &duration;
      data[3].Size = 8;
      fixed (char* chPtr1 = st1)
        fixed (char* chPtr2 = st2)
        {
          data[1].DataPointer = (IntPtr) (void*) chPtr1;
          data[2].DataPointer = (IntPtr) (void*) chPtr2;
          this.WriteEventCore(eventId, 4, data);
        }
    }

    [NonEvent]
    public unsafe void WriteEvent(int eventId, string st, int i1, long duration)
    {
      if (!this.IsEnabled())
        return;
      st = st ?? string.Empty;
      EventSource.EventData* data = stackalloc EventSource.EventData[3];
      data->Size = (st.Length + 1) * 2;
      data[1].DataPointer = (IntPtr) (void*) &i1;
      data[1].Size = 4;
      data[2].DataPointer = (IntPtr) (void*) &duration;
      data[2].Size = 4;
      fixed (char* chPtr = st)
      {
        data->DataPointer = (IntPtr) (void*) chPtr;
        this.WriteEventCore(eventId, 3, data);
      }
    }

    public void MethodStart(Guid uniqueIdentifier, Guid hostId, string methodName) => this.WriteEvent(1, uniqueIdentifier, hostId, methodName);

    public void MethodStop(Guid uniqueIdentifier, Guid hostId, string methodName, long duration) => this.WriteEvent(2, uniqueIdentifier, hostId, methodName, duration);

    public void NotificationCallbackStart(Guid hostId, string callback) => this.WriteEvent(3, hostId, callback);

    public void NotificationCallbackStop(Guid hostId, string callback, long duration) => this.WriteEvent(4, hostId, callback, duration);

    public void TaskCallbackStart(Guid hostId, string callback) => this.WriteEvent(5, hostId, callback);

    public void TaskCallbackStop(Guid hostId, string callback, long duration) => this.WriteEvent(6, hostId, callback, duration);

    public void StopHostTaskStart(Guid hostId) => this.WriteEvent(7, hostId);

    public void StopHostTaskStop(Guid hostId, long duration) => this.WriteEvent(8, hostId, duration);

    public void RefreshSecurityTokenStart(Guid uniqueIdentifier, string name) => this.WriteEvent(9, uniqueIdentifier, name);

    public void RefreshSecurityTokenStop(
      Guid uniqueIdentifier,
      string name,
      DateTime validFrom,
      DateTime validTo,
      Guid contextId,
      long duration)
    {
      this.WriteEvent(10, (object) uniqueIdentifier, (object) name, (object) validFrom, (object) validTo, (object) contextId, (object) duration);
    }

    public void SQLStart(Guid uniqueIdentifier, string query, string server, string databaseName) => this.WriteEvent(11, uniqueIdentifier, query, server, databaseName);

    public void SQLStop(
      Guid uniqueIdentifier,
      string query,
      string server,
      string databaseName,
      long duration)
    {
      this.WriteEvent(12, uniqueIdentifier, query, server, databaseName, duration);
    }

    public void RESTStart(Guid uniqueIdentifier, string message) => this.WriteEvent(13, uniqueIdentifier, message);

    public void RESTStop(
      Guid uniqueIdentifier,
      Guid originalActivityId,
      string message,
      long duration)
    {
      this.WriteEvent(14, uniqueIdentifier, originalActivityId, message, duration);
    }

    public void WindowsAzureStorageStart(
      Guid uniqueIdentifier,
      string accountName,
      string methodName)
    {
      this.WriteEvent(15, uniqueIdentifier, accountName, methodName);
    }

    public void WindowsAzureStorageStop(
      Guid uniqueIdentifier,
      string accountName,
      string methodName,
      long duration)
    {
      this.WriteEvent(16, uniqueIdentifier, accountName, methodName, duration);
    }

    public void LoadHostStart(Guid hostId) => this.WriteEvent(17, hostId);

    public void LoadHostStop(Guid hostId, long duration) => this.WriteEvent(18, hostId, duration);

    public void CreateServiceInstanceBegin(Guid uniqueIdentifier, Guid hostId, string serviceType) => this.WriteEvent(19, uniqueIdentifier, hostId, serviceType);

    public void CreateServiceInstanceEnd(
      Guid uniqueIdentifier,
      Guid hostId,
      string serviceType,
      long duration)
    {
      this.WriteEvent(20, uniqueIdentifier, hostId, serviceType, duration);
    }

    public void DetectedLockReentryViolation(string lockName) => this.WriteEvent(21, lockName);

    public void DetectedLockUsageViolation(string lockName, string locksHeld) => this.WriteEvent(22, lockName, locksHeld);

    public void RedisStart(
      Guid uniqueIdentifier,
      string operation,
      string ciArea,
      string cacheArea)
    {
      this.WriteEvent(23, uniqueIdentifier, operation, ciArea, cacheArea);
    }

    public void RedisStop(
      Guid uniqueIdentifier,
      string operation,
      string ciArea,
      string cacheArea,
      long duration)
    {
      this.WriteEvent(24, uniqueIdentifier, operation, ciArea, cacheArea, duration);
    }

    public void MessageBusSendBatchStart(
      Guid uniqueIdentifier,
      string messageBusName,
      int numberOfMessages)
    {
      this.WriteEvent(25, uniqueIdentifier, messageBusName, (long) numberOfMessages);
    }

    public void MessageBusSendBatchStop(
      Guid uniqueIdentifier,
      string messageBusName,
      int numberOfMessages,
      long duration)
    {
      this.WriteEvent(26, (object) uniqueIdentifier, (object) messageBusName, (object) numberOfMessages, (object) duration);
    }

    public void ValidateJWTokenStart(Guid e2eid, string issuer, string name) => this.WriteEvent(27, e2eid, issuer, name);

    public void ValidateJWTokenStop(Guid e2eid, string issuer, string name, long duration) => this.WriteEvent(28, e2eid, issuer, name, duration);

    public void TFFileServiceAsCacheStart(
      Guid uniqueIdentifier,
      string operation,
      string ciArea,
      string cacheArea)
    {
      this.WriteEvent(29, uniqueIdentifier, operation, ciArea, cacheArea);
    }

    public void TFFileServiceAsCacheStop(
      Guid uniqueIdentifier,
      string operation,
      string ciArea,
      string cacheArea,
      long duration)
    {
      this.WriteEvent(30, uniqueIdentifier, operation, ciArea, cacheArea, duration);
    }
  }
}
