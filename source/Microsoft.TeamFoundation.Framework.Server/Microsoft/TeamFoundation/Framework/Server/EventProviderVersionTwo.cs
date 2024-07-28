// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.EventProviderVersionTwo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics.Eventing;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class EventProviderVersionTwo : EventProvider
  {
    internal EventProviderVersionTwo(Guid id)
      : base(id)
    {
    }

    internal unsafe bool TemplateInfo8(
      ref EventDescriptor eventDescriptor,
      Guid TraceId,
      int Tracepoint,
      Guid ServiceHost,
      long ContextId,
      string ProcessName,
      string Username,
      string Service,
      string Method,
      string Area,
      string Layer,
      string UserAgent,
      string Uri,
      string Path,
      string UserDefined,
      string ExceptionType,
      string Message,
      Guid VSID,
      Guid UniqueIdentifier,
      Guid E2EID,
      Guid CUID,
      Guid TenantId,
      Guid ProviderId,
      string OrchestrationId,
      sbyte WebSiteId)
    {
      int dataCount = 24;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &TraceId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].DataPointer = (ulong) &Tracepoint;
        eventDataPtr[1].Size = 4U;
        eventDataPtr[2].DataPointer = (ulong) &ServiceHost;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].DataPointer = (ulong) &ContextId;
        eventDataPtr[3].Size = 8U;
        eventDataPtr[4].Size = (uint) ((ProcessName.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((Username.Length + 1) * 2);
        eventDataPtr[6].Size = (uint) ((Service.Length + 1) * 2);
        eventDataPtr[7].Size = (uint) ((Method.Length + 1) * 2);
        eventDataPtr[8].Size = (uint) ((Area.Length + 1) * 2);
        eventDataPtr[9].Size = (uint) ((Layer.Length + 1) * 2);
        eventDataPtr[10].Size = (uint) ((UserAgent.Length + 1) * 2);
        eventDataPtr[11].Size = (uint) ((Uri.Length + 1) * 2);
        eventDataPtr[12].Size = (uint) ((Path.Length + 1) * 2);
        eventDataPtr[13].Size = (uint) ((UserDefined.Length + 1) * 2);
        eventDataPtr[14].Size = (uint) ((ExceptionType.Length + 1) * 2);
        eventDataPtr[15].Size = (uint) ((Message.Length + 1) * 2);
        eventDataPtr[16].DataPointer = (ulong) &VSID;
        eventDataPtr[16].Size = (uint) sizeof (Guid);
        eventDataPtr[17].DataPointer = (ulong) &UniqueIdentifier;
        eventDataPtr[17].Size = (uint) sizeof (Guid);
        eventDataPtr[18].DataPointer = (ulong) &E2EID;
        eventDataPtr[18].Size = (uint) sizeof (Guid);
        eventDataPtr[19].DataPointer = (ulong) &CUID;
        eventDataPtr[19].Size = (uint) sizeof (Guid);
        eventDataPtr[20].DataPointer = (ulong) &TenantId;
        eventDataPtr[20].Size = (uint) sizeof (Guid);
        eventDataPtr[21].DataPointer = (ulong) &ProviderId;
        eventDataPtr[21].Size = (uint) sizeof (Guid);
        eventDataPtr[22].Size = (uint) ((OrchestrationId.Length + 1) * 2);
        eventDataPtr[23].DataPointer = (ulong) &WebSiteId;
        eventDataPtr[23].Size = 1U;
        fixed (char* chPtr1 = ProcessName)
          fixed (char* chPtr2 = Username)
            fixed (char* chPtr3 = Service)
              fixed (char* chPtr4 = Method)
                fixed (char* chPtr5 = Area)
                  fixed (char* chPtr6 = Layer)
                    fixed (char* chPtr7 = UserAgent)
                      fixed (char* chPtr8 = Uri)
                        fixed (char* chPtr9 = Path)
                          fixed (char* chPtr10 = UserDefined)
                            fixed (char* chPtr11 = ExceptionType)
                              fixed (char* chPtr12 = Message)
                                fixed (char* chPtr13 = OrchestrationId)
                                {
                                  eventDataPtr[4].DataPointer = (ulong) chPtr1;
                                  eventDataPtr[5].DataPointer = (ulong) chPtr2;
                                  eventDataPtr[6].DataPointer = (ulong) chPtr3;
                                  eventDataPtr[7].DataPointer = (ulong) chPtr4;
                                  eventDataPtr[8].DataPointer = (ulong) chPtr5;
                                  eventDataPtr[9].DataPointer = (ulong) chPtr6;
                                  eventDataPtr[10].DataPointer = (ulong) chPtr7;
                                  eventDataPtr[11].DataPointer = (ulong) chPtr8;
                                  eventDataPtr[12].DataPointer = (ulong) chPtr9;
                                  eventDataPtr[13].DataPointer = (ulong) chPtr10;
                                  eventDataPtr[14].DataPointer = (ulong) chPtr11;
                                  eventDataPtr[15].DataPointer = (ulong) chPtr12;
                                  eventDataPtr[22].DataPointer = (ulong) chPtr13;
                                  flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                                }
      }
      return flag;
    }

    internal unsafe bool TemplateSQL(
      ref EventDescriptor eventDescriptor,
      string Database,
      string Datasource,
      string Operation,
      short Retries,
      bool Success,
      int TotalTime,
      int ConnectTime,
      int ExecutionTime,
      int WaitTime,
      int ErrorCode,
      string ErrorMessage)
    {
      int dataCount = 11;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((Database.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((Datasource.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((Operation.Length + 1) * 2);
        eventDataPtr[3].DataPointer = (ulong) &Retries;
        eventDataPtr[3].Size = 2U;
        int num = Success ? 1 : 0;
        eventDataPtr[4].DataPointer = (ulong) &num;
        eventDataPtr[4].Size = 4U;
        eventDataPtr[5].DataPointer = (ulong) &TotalTime;
        eventDataPtr[5].Size = 4U;
        eventDataPtr[6].DataPointer = (ulong) &ConnectTime;
        eventDataPtr[6].Size = 4U;
        eventDataPtr[7].DataPointer = (ulong) &ExecutionTime;
        eventDataPtr[7].Size = 4U;
        eventDataPtr[8].DataPointer = (ulong) &WaitTime;
        eventDataPtr[8].Size = 4U;
        eventDataPtr[9].DataPointer = (ulong) &ErrorCode;
        eventDataPtr[9].Size = 4U;
        eventDataPtr[10].Size = (uint) ((ErrorMessage.Length + 1) * 2);
        fixed (char* chPtr1 = Database)
          fixed (char* chPtr2 = Datasource)
            fixed (char* chPtr3 = Operation)
              fixed (char* chPtr4 = ErrorMessage)
              {
                eventDataPtr->DataPointer = (ulong) chPtr1;
                eventDataPtr[1].DataPointer = (ulong) chPtr2;
                eventDataPtr[2].DataPointer = (ulong) chPtr3;
                eventDataPtr[10].DataPointer = (ulong) chPtr4;
                flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
              }
      }
      return flag;
    }

    internal unsafe bool TemplateJobAgentHistory15(
      ref EventDescriptor eventDescriptor,
      string Plugin,
      string JobName,
      Guid JobSource,
      Guid JobId,
      DateTime QueueTime,
      DateTime StartTime,
      long ExecutionTime,
      Guid AgentId,
      int Result,
      string ResultMessage,
      int QueuedReasons,
      int QueueFlags,
      short Priority,
      int LogicalReads,
      int PhysicalReads,
      int CPUTime,
      int ElapsedTime,
      string Feature,
      int SqlExecutionTime,
      int SqlExecutionCount,
      int RedisExecutionTime,
      int RedisExecutionCount,
      int AadExecutionTime,
      int AadExecutionCount,
      int BlobStorageExecutionTime,
      int BlobStorageExecutionCount,
      int TableStorageExecutionTime,
      int TableStorageExecutionCount,
      int ServiceBusExecutionTime,
      int ServiceBusExecutionCount,
      int VssClientExecutionTime,
      int VssClientExecutionCount,
      int SqlRetryExecutionTime,
      int SqlRetryExecutionCount,
      int SqlReadOnlyExecutionTime,
      int SqlReadOnlyExecutionCount,
      long CPUCycles,
      int FinalSqlCommandExecutionTime,
      Guid E2EID,
      int AadGraphExecutionTime,
      int AadGraphExecutionCount,
      int AadTokenExecutionTime,
      int AadTokenExecutionCount,
      int DocDBExecutionTime,
      int DocDBExecutionCount,
      int DocDBRUsConsumed,
      long AllocatedBytes,
      Guid RequesterActivityId,
      Guid RequesterVsid,
      long CPUCyclesAsync,
      long AllocatedBytesAsync)
    {
      int dataCount = 51;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((Plugin.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((JobName.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &JobSource;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].DataPointer = (ulong) &JobId;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        long fileTime1 = QueueTime.ToFileTime();
        eventDataPtr[4].DataPointer = (ulong) &fileTime1;
        eventDataPtr[4].Size = 8U;
        long fileTime2 = StartTime.ToFileTime();
        eventDataPtr[5].DataPointer = (ulong) &fileTime2;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].DataPointer = (ulong) &ExecutionTime;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].DataPointer = (ulong) &AgentId;
        eventDataPtr[7].Size = (uint) sizeof (Guid);
        eventDataPtr[8].DataPointer = (ulong) &Result;
        eventDataPtr[8].Size = 4U;
        eventDataPtr[9].Size = (uint) ((ResultMessage.Length + 1) * 2);
        eventDataPtr[10].DataPointer = (ulong) &QueuedReasons;
        eventDataPtr[10].Size = 4U;
        eventDataPtr[11].DataPointer = (ulong) &QueueFlags;
        eventDataPtr[11].Size = 4U;
        eventDataPtr[12].DataPointer = (ulong) &Priority;
        eventDataPtr[12].Size = 2U;
        eventDataPtr[13].DataPointer = (ulong) &LogicalReads;
        eventDataPtr[13].Size = 4U;
        eventDataPtr[14].DataPointer = (ulong) &PhysicalReads;
        eventDataPtr[14].Size = 4U;
        eventDataPtr[15].DataPointer = (ulong) &CPUTime;
        eventDataPtr[15].Size = 4U;
        eventDataPtr[16].DataPointer = (ulong) &ElapsedTime;
        eventDataPtr[16].Size = 4U;
        eventDataPtr[17].Size = (uint) ((Feature.Length + 1) * 2);
        eventDataPtr[18].DataPointer = (ulong) &SqlExecutionTime;
        eventDataPtr[18].Size = 4U;
        eventDataPtr[19].DataPointer = (ulong) &SqlExecutionCount;
        eventDataPtr[19].Size = 4U;
        eventDataPtr[20].DataPointer = (ulong) &RedisExecutionTime;
        eventDataPtr[20].Size = 4U;
        eventDataPtr[21].DataPointer = (ulong) &RedisExecutionCount;
        eventDataPtr[21].Size = 4U;
        eventDataPtr[22].DataPointer = (ulong) &AadExecutionTime;
        eventDataPtr[22].Size = 4U;
        eventDataPtr[23].DataPointer = (ulong) &AadExecutionCount;
        eventDataPtr[23].Size = 4U;
        eventDataPtr[24].DataPointer = (ulong) &BlobStorageExecutionTime;
        eventDataPtr[24].Size = 4U;
        eventDataPtr[25].DataPointer = (ulong) &BlobStorageExecutionCount;
        eventDataPtr[25].Size = 4U;
        eventDataPtr[26].DataPointer = (ulong) &TableStorageExecutionTime;
        eventDataPtr[26].Size = 4U;
        eventDataPtr[27].DataPointer = (ulong) &TableStorageExecutionCount;
        eventDataPtr[27].Size = 4U;
        eventDataPtr[28].DataPointer = (ulong) &ServiceBusExecutionTime;
        eventDataPtr[28].Size = 4U;
        eventDataPtr[29].DataPointer = (ulong) &ServiceBusExecutionCount;
        eventDataPtr[29].Size = 4U;
        eventDataPtr[30].DataPointer = (ulong) &VssClientExecutionTime;
        eventDataPtr[30].Size = 4U;
        eventDataPtr[31].DataPointer = (ulong) &VssClientExecutionCount;
        eventDataPtr[31].Size = 4U;
        eventDataPtr[32].DataPointer = (ulong) &SqlRetryExecutionTime;
        eventDataPtr[32].Size = 4U;
        eventDataPtr[33].DataPointer = (ulong) &SqlRetryExecutionCount;
        eventDataPtr[33].Size = 4U;
        eventDataPtr[34].DataPointer = (ulong) &SqlReadOnlyExecutionTime;
        eventDataPtr[34].Size = 4U;
        eventDataPtr[35].DataPointer = (ulong) &SqlReadOnlyExecutionCount;
        eventDataPtr[35].Size = 4U;
        eventDataPtr[36].DataPointer = (ulong) &CPUCycles;
        eventDataPtr[36].Size = 8U;
        eventDataPtr[37].DataPointer = (ulong) &FinalSqlCommandExecutionTime;
        eventDataPtr[37].Size = 4U;
        eventDataPtr[38].DataPointer = (ulong) &E2EID;
        eventDataPtr[38].Size = (uint) sizeof (Guid);
        eventDataPtr[39].DataPointer = (ulong) &AadGraphExecutionTime;
        eventDataPtr[39].Size = 4U;
        eventDataPtr[40].DataPointer = (ulong) &AadGraphExecutionCount;
        eventDataPtr[40].Size = 4U;
        eventDataPtr[41].DataPointer = (ulong) &AadTokenExecutionTime;
        eventDataPtr[41].Size = 4U;
        eventDataPtr[42].DataPointer = (ulong) &AadTokenExecutionCount;
        eventDataPtr[42].Size = 4U;
        eventDataPtr[43].DataPointer = (ulong) &DocDBExecutionTime;
        eventDataPtr[43].Size = 4U;
        eventDataPtr[44].DataPointer = (ulong) &DocDBExecutionCount;
        eventDataPtr[44].Size = 4U;
        eventDataPtr[45].DataPointer = (ulong) &DocDBRUsConsumed;
        eventDataPtr[45].Size = 4U;
        eventDataPtr[46].DataPointer = (ulong) &AllocatedBytes;
        eventDataPtr[46].Size = 8U;
        eventDataPtr[47].DataPointer = (ulong) &RequesterActivityId;
        eventDataPtr[47].Size = (uint) sizeof (Guid);
        eventDataPtr[48].DataPointer = (ulong) &RequesterVsid;
        eventDataPtr[48].Size = (uint) sizeof (Guid);
        eventDataPtr[49].DataPointer = (ulong) &CPUCyclesAsync;
        eventDataPtr[49].Size = 8U;
        eventDataPtr[50].DataPointer = (ulong) &AllocatedBytesAsync;
        eventDataPtr[50].Size = 8U;
        fixed (char* chPtr1 = Plugin)
          fixed (char* chPtr2 = JobName)
            fixed (char* chPtr3 = ResultMessage)
              fixed (char* chPtr4 = Feature)
              {
                eventDataPtr->DataPointer = (ulong) chPtr1;
                eventDataPtr[1].DataPointer = (ulong) chPtr2;
                eventDataPtr[9].DataPointer = (ulong) chPtr3;
                eventDataPtr[17].DataPointer = (ulong) chPtr4;
                flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
              }
      }
      return flag;
    }

    internal unsafe bool TemplateActivityLog38(
      ref EventDescriptor eventDescriptor,
      Guid HostId,
      long ContextId,
      string Application,
      string Command,
      int Status,
      int Count,
      DateTime StartTime,
      long ExecutionTime,
      string IdentityName,
      string IPAddress,
      Guid UniqueIdentifier,
      string UserAgent,
      string CommandIdentifier,
      string ExceptionType,
      string ExceptionMessage,
      Guid ActivityId,
      int ResponseCode,
      Guid VSID,
      long TimeToFirstPage,
      int ActivityStatus,
      long ExecutionTimeThreshold,
      bool IsExceptionExpected,
      long DelayTime,
      Guid RelatedActivityId,
      int LogicalReads,
      int PhysicalReads,
      int CPUTime,
      int ElapsedTime,
      string Feature,
      DateTime HostStartTime,
      byte HostType,
      Guid ParentHostId,
      string AnonymousIdentifier,
      int SqlExecutionTime,
      int SqlExecutionCount,
      int RedisExecutionTime,
      int RedisExecutionCount,
      int AadExecutionTime,
      int AadExecutionCount,
      int BlobStorageExecutionTime,
      int BlobStorageExecutionCount,
      int TableStorageExecutionTime,
      int TableStorageExecutionCount,
      int ServiceBusExecutionTime,
      int ServiceBusExecutionCount,
      int VssClientExecutionTime,
      int VssClientExecutionCount,
      int SqlRetryExecutionTime,
      int SqlRetryExecutionCount,
      int SqlReadOnlyExecutionTime,
      int SqlReadOnlyExecutionCount,
      long CPUCycles,
      int FinalSqlCommandExecutionTime,
      Guid E2EID,
      Guid PersistentSessionId,
      Guid PendingAuthenticationSessionId,
      Guid CurrentAuthenticationSessionId,
      Guid CUID,
      Guid TenantId,
      Guid ProviderId,
      long QueueTime,
      string AuthenticationMechanism,
      double TSTUs,
      int AadGraphExecutionTime,
      int AadGraphExecutionCount,
      int AadTokenExecutionTime,
      int AadTokenExecutionCount,
      string ThrottleReason,
      string Referrer,
      string UriStem,
      byte SupportsPublicAccess,
      long ConcurrencySemaphoreTime,
      Guid AuthorizationId,
      long MethodInformationTimeout,
      long PreControllerTime,
      long ControllerTime,
      long PostControllerTime,
      string OrchestrationId,
      int DocDBExecutionTime,
      int DocDBExecutionCount,
      int DocDBRUsConsumed,
      long AllocatedBytes,
      string SmartRouterStatus,
      string SmartRouterReason,
      string SmartRouterTarget,
      Guid OAuthAppId,
      sbyte WebSiteId)
    {
      int dataCount = 87;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &HostId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].DataPointer = (ulong) &ContextId;
        eventDataPtr[1].Size = 8U;
        eventDataPtr[2].Size = (uint) ((Application.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((Command.Length + 1) * 2);
        eventDataPtr[4].DataPointer = (ulong) &Status;
        eventDataPtr[4].Size = 4U;
        eventDataPtr[5].DataPointer = (ulong) &Count;
        eventDataPtr[5].Size = 4U;
        long fileTime1 = StartTime.ToFileTime();
        eventDataPtr[6].DataPointer = (ulong) &fileTime1;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].DataPointer = (ulong) &ExecutionTime;
        eventDataPtr[7].Size = 8U;
        eventDataPtr[8].Size = (uint) ((IdentityName.Length + 1) * 2);
        eventDataPtr[9].Size = (uint) ((IPAddress.Length + 1) * 2);
        eventDataPtr[10].DataPointer = (ulong) &UniqueIdentifier;
        eventDataPtr[10].Size = (uint) sizeof (Guid);
        eventDataPtr[11].Size = (uint) ((UserAgent.Length + 1) * 2);
        eventDataPtr[12].Size = (uint) ((CommandIdentifier.Length + 1) * 2);
        eventDataPtr[13].Size = (uint) ((ExceptionType.Length + 1) * 2);
        eventDataPtr[14].Size = (uint) ((ExceptionMessage.Length + 1) * 2);
        eventDataPtr[15].DataPointer = (ulong) &ActivityId;
        eventDataPtr[15].Size = (uint) sizeof (Guid);
        eventDataPtr[16].DataPointer = (ulong) &ResponseCode;
        eventDataPtr[16].Size = 4U;
        eventDataPtr[17].DataPointer = (ulong) &VSID;
        eventDataPtr[17].Size = (uint) sizeof (Guid);
        eventDataPtr[18].DataPointer = (ulong) &TimeToFirstPage;
        eventDataPtr[18].Size = 8U;
        eventDataPtr[19].DataPointer = (ulong) &ActivityStatus;
        eventDataPtr[19].Size = 4U;
        eventDataPtr[20].DataPointer = (ulong) &ExecutionTimeThreshold;
        eventDataPtr[20].Size = 8U;
        int num = IsExceptionExpected ? 1 : 0;
        eventDataPtr[21].DataPointer = (ulong) &num;
        eventDataPtr[21].Size = 4U;
        eventDataPtr[22].DataPointer = (ulong) &DelayTime;
        eventDataPtr[22].Size = 8U;
        eventDataPtr[23].DataPointer = (ulong) &RelatedActivityId;
        eventDataPtr[23].Size = (uint) sizeof (Guid);
        eventDataPtr[24].DataPointer = (ulong) &LogicalReads;
        eventDataPtr[24].Size = 4U;
        eventDataPtr[25].DataPointer = (ulong) &PhysicalReads;
        eventDataPtr[25].Size = 4U;
        eventDataPtr[26].DataPointer = (ulong) &CPUTime;
        eventDataPtr[26].Size = 4U;
        eventDataPtr[27].DataPointer = (ulong) &ElapsedTime;
        eventDataPtr[27].Size = 4U;
        eventDataPtr[28].Size = (uint) ((Feature.Length + 1) * 2);
        long fileTime2 = HostStartTime.ToFileTime();
        eventDataPtr[29].DataPointer = (ulong) &fileTime2;
        eventDataPtr[29].Size = 8U;
        eventDataPtr[30].DataPointer = (ulong) &HostType;
        eventDataPtr[30].Size = 1U;
        eventDataPtr[31].DataPointer = (ulong) &ParentHostId;
        eventDataPtr[31].Size = (uint) sizeof (Guid);
        eventDataPtr[32].Size = (uint) ((AnonymousIdentifier.Length + 1) * 2);
        eventDataPtr[33].DataPointer = (ulong) &SqlExecutionTime;
        eventDataPtr[33].Size = 4U;
        eventDataPtr[34].DataPointer = (ulong) &SqlExecutionCount;
        eventDataPtr[34].Size = 4U;
        eventDataPtr[35].DataPointer = (ulong) &RedisExecutionTime;
        eventDataPtr[35].Size = 4U;
        eventDataPtr[36].DataPointer = (ulong) &RedisExecutionCount;
        eventDataPtr[36].Size = 4U;
        eventDataPtr[37].DataPointer = (ulong) &AadExecutionTime;
        eventDataPtr[37].Size = 4U;
        eventDataPtr[38].DataPointer = (ulong) &AadExecutionCount;
        eventDataPtr[38].Size = 4U;
        eventDataPtr[39].DataPointer = (ulong) &BlobStorageExecutionTime;
        eventDataPtr[39].Size = 4U;
        eventDataPtr[40].DataPointer = (ulong) &BlobStorageExecutionCount;
        eventDataPtr[40].Size = 4U;
        eventDataPtr[41].DataPointer = (ulong) &TableStorageExecutionTime;
        eventDataPtr[41].Size = 4U;
        eventDataPtr[42].DataPointer = (ulong) &TableStorageExecutionCount;
        eventDataPtr[42].Size = 4U;
        eventDataPtr[43].DataPointer = (ulong) &ServiceBusExecutionTime;
        eventDataPtr[43].Size = 4U;
        eventDataPtr[44].DataPointer = (ulong) &ServiceBusExecutionCount;
        eventDataPtr[44].Size = 4U;
        eventDataPtr[45].DataPointer = (ulong) &VssClientExecutionTime;
        eventDataPtr[45].Size = 4U;
        eventDataPtr[46].DataPointer = (ulong) &VssClientExecutionCount;
        eventDataPtr[46].Size = 4U;
        eventDataPtr[47].DataPointer = (ulong) &SqlRetryExecutionTime;
        eventDataPtr[47].Size = 4U;
        eventDataPtr[48].DataPointer = (ulong) &SqlRetryExecutionCount;
        eventDataPtr[48].Size = 4U;
        eventDataPtr[49].DataPointer = (ulong) &SqlReadOnlyExecutionTime;
        eventDataPtr[49].Size = 4U;
        eventDataPtr[50].DataPointer = (ulong) &SqlReadOnlyExecutionCount;
        eventDataPtr[50].Size = 4U;
        eventDataPtr[51].DataPointer = (ulong) &CPUCycles;
        eventDataPtr[51].Size = 8U;
        eventDataPtr[52].DataPointer = (ulong) &FinalSqlCommandExecutionTime;
        eventDataPtr[52].Size = 4U;
        eventDataPtr[53].DataPointer = (ulong) &E2EID;
        eventDataPtr[53].Size = (uint) sizeof (Guid);
        eventDataPtr[54].DataPointer = (ulong) &PersistentSessionId;
        eventDataPtr[54].Size = (uint) sizeof (Guid);
        eventDataPtr[55].DataPointer = (ulong) &PendingAuthenticationSessionId;
        eventDataPtr[55].Size = (uint) sizeof (Guid);
        eventDataPtr[56].DataPointer = (ulong) &CurrentAuthenticationSessionId;
        eventDataPtr[56].Size = (uint) sizeof (Guid);
        eventDataPtr[57].DataPointer = (ulong) &CUID;
        eventDataPtr[57].Size = (uint) sizeof (Guid);
        eventDataPtr[58].DataPointer = (ulong) &TenantId;
        eventDataPtr[58].Size = (uint) sizeof (Guid);
        eventDataPtr[59].DataPointer = (ulong) &ProviderId;
        eventDataPtr[59].Size = (uint) sizeof (Guid);
        eventDataPtr[60].DataPointer = (ulong) &QueueTime;
        eventDataPtr[60].Size = 8U;
        eventDataPtr[61].Size = (uint) ((AuthenticationMechanism.Length + 1) * 2);
        eventDataPtr[62].DataPointer = (ulong) &TSTUs;
        eventDataPtr[62].Size = 8U;
        eventDataPtr[63].DataPointer = (ulong) &AadGraphExecutionTime;
        eventDataPtr[63].Size = 4U;
        eventDataPtr[64].DataPointer = (ulong) &AadGraphExecutionCount;
        eventDataPtr[64].Size = 4U;
        eventDataPtr[65].DataPointer = (ulong) &AadTokenExecutionTime;
        eventDataPtr[65].Size = 4U;
        eventDataPtr[66].DataPointer = (ulong) &AadTokenExecutionCount;
        eventDataPtr[66].Size = 4U;
        eventDataPtr[67].Size = (uint) ((ThrottleReason.Length + 1) * 2);
        eventDataPtr[68].Size = (uint) ((Referrer.Length + 1) * 2);
        eventDataPtr[69].Size = (uint) ((UriStem.Length + 1) * 2);
        eventDataPtr[70].DataPointer = (ulong) &SupportsPublicAccess;
        eventDataPtr[70].Size = 1U;
        eventDataPtr[71].DataPointer = (ulong) &ConcurrencySemaphoreTime;
        eventDataPtr[71].Size = 8U;
        eventDataPtr[72].DataPointer = (ulong) &AuthorizationId;
        eventDataPtr[72].Size = (uint) sizeof (Guid);
        eventDataPtr[73].DataPointer = (ulong) &MethodInformationTimeout;
        eventDataPtr[73].Size = 8U;
        eventDataPtr[74].DataPointer = (ulong) &PreControllerTime;
        eventDataPtr[74].Size = 8U;
        eventDataPtr[75].DataPointer = (ulong) &ControllerTime;
        eventDataPtr[75].Size = 8U;
        eventDataPtr[76].DataPointer = (ulong) &PostControllerTime;
        eventDataPtr[76].Size = 8U;
        eventDataPtr[77].Size = (uint) ((OrchestrationId.Length + 1) * 2);
        eventDataPtr[78].DataPointer = (ulong) &DocDBExecutionTime;
        eventDataPtr[78].Size = 4U;
        eventDataPtr[79].DataPointer = (ulong) &DocDBExecutionCount;
        eventDataPtr[79].Size = 4U;
        eventDataPtr[80].DataPointer = (ulong) &DocDBRUsConsumed;
        eventDataPtr[80].Size = 4U;
        eventDataPtr[81].DataPointer = (ulong) &AllocatedBytes;
        eventDataPtr[81].Size = 8U;
        eventDataPtr[82].Size = (uint) ((SmartRouterStatus.Length + 1) * 2);
        eventDataPtr[83].Size = (uint) ((SmartRouterReason.Length + 1) * 2);
        eventDataPtr[84].Size = (uint) ((SmartRouterTarget.Length + 1) * 2);
        eventDataPtr[85].DataPointer = (ulong) &OAuthAppId;
        eventDataPtr[85].Size = (uint) sizeof (Guid);
        eventDataPtr[86].DataPointer = (ulong) &WebSiteId;
        eventDataPtr[86].Size = 1U;
        fixed (char* chPtr1 = Application)
          fixed (char* chPtr2 = Command)
            fixed (char* chPtr3 = IdentityName)
              fixed (char* chPtr4 = IPAddress)
                fixed (char* chPtr5 = UserAgent)
                  fixed (char* chPtr6 = CommandIdentifier)
                    fixed (char* chPtr7 = ExceptionType)
                      fixed (char* chPtr8 = ExceptionMessage)
                        fixed (char* chPtr9 = Feature)
                          fixed (char* chPtr10 = AnonymousIdentifier)
                            fixed (char* chPtr11 = AuthenticationMechanism)
                              fixed (char* chPtr12 = ThrottleReason)
                                fixed (char* chPtr13 = Referrer)
                                  fixed (char* chPtr14 = UriStem)
                                    fixed (char* chPtr15 = OrchestrationId)
                                      fixed (char* chPtr16 = SmartRouterStatus)
                                        fixed (char* chPtr17 = SmartRouterReason)
                                          fixed (char* chPtr18 = SmartRouterTarget)
                                          {
                                            eventDataPtr[2].DataPointer = (ulong) chPtr1;
                                            eventDataPtr[3].DataPointer = (ulong) chPtr2;
                                            eventDataPtr[8].DataPointer = (ulong) chPtr3;
                                            eventDataPtr[9].DataPointer = (ulong) chPtr4;
                                            eventDataPtr[11].DataPointer = (ulong) chPtr5;
                                            eventDataPtr[12].DataPointer = (ulong) chPtr6;
                                            eventDataPtr[13].DataPointer = (ulong) chPtr7;
                                            eventDataPtr[14].DataPointer = (ulong) chPtr8;
                                            eventDataPtr[28].DataPointer = (ulong) chPtr9;
                                            eventDataPtr[32].DataPointer = (ulong) chPtr10;
                                            eventDataPtr[61].DataPointer = (ulong) chPtr11;
                                            eventDataPtr[67].DataPointer = (ulong) chPtr12;
                                            eventDataPtr[68].DataPointer = (ulong) chPtr13;
                                            eventDataPtr[69].DataPointer = (ulong) chPtr14;
                                            eventDataPtr[77].DataPointer = (ulong) chPtr15;
                                            eventDataPtr[82].DataPointer = (ulong) chPtr16;
                                            eventDataPtr[83].DataPointer = (ulong) chPtr17;
                                            eventDataPtr[84].DataPointer = (ulong) chPtr18;
                                            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                                          }
      }
      return flag;
    }

    internal unsafe bool TemplateCustomerIntelligence5(
      ref EventDescriptor eventDescriptor,
      int Count,
      string Properties,
      Guid UniqueIdentifier,
      string AnonymousIdentifier,
      Guid HostId,
      Guid ParentHostId,
      byte HostType,
      Guid VSID,
      string Area,
      string Feature,
      string Useragent,
      Guid CUID,
      string DataspaceType,
      string DataspaceId,
      string DataspaceVisibility,
      byte SupportsPublicAccess)
    {
      int dataCount = 16;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &Count;
        eventDataPtr->Size = 4U;
        eventDataPtr[1].Size = (uint) ((Properties.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &UniqueIdentifier;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].Size = (uint) ((AnonymousIdentifier.Length + 1) * 2);
        eventDataPtr[4].DataPointer = (ulong) &HostId;
        eventDataPtr[4].Size = (uint) sizeof (Guid);
        eventDataPtr[5].DataPointer = (ulong) &ParentHostId;
        eventDataPtr[5].Size = (uint) sizeof (Guid);
        eventDataPtr[6].DataPointer = (ulong) &HostType;
        eventDataPtr[6].Size = 1U;
        eventDataPtr[7].DataPointer = (ulong) &VSID;
        eventDataPtr[7].Size = (uint) sizeof (Guid);
        eventDataPtr[8].Size = (uint) ((Area.Length + 1) * 2);
        eventDataPtr[9].Size = (uint) ((Feature.Length + 1) * 2);
        eventDataPtr[10].Size = (uint) ((Useragent.Length + 1) * 2);
        eventDataPtr[11].DataPointer = (ulong) &CUID;
        eventDataPtr[11].Size = (uint) sizeof (Guid);
        eventDataPtr[12].Size = (uint) ((DataspaceType.Length + 1) * 2);
        eventDataPtr[13].Size = (uint) ((DataspaceId.Length + 1) * 2);
        eventDataPtr[14].Size = (uint) ((DataspaceVisibility.Length + 1) * 2);
        eventDataPtr[15].DataPointer = (ulong) &SupportsPublicAccess;
        eventDataPtr[15].Size = 1U;
        fixed (char* chPtr1 = Properties)
          fixed (char* chPtr2 = AnonymousIdentifier)
            fixed (char* chPtr3 = Area)
              fixed (char* chPtr4 = Feature)
                fixed (char* chPtr5 = Useragent)
                  fixed (char* chPtr6 = DataspaceType)
                    fixed (char* chPtr7 = DataspaceId)
                      fixed (char* chPtr8 = DataspaceVisibility)
                      {
                        eventDataPtr[1].DataPointer = (ulong) chPtr1;
                        eventDataPtr[3].DataPointer = (ulong) chPtr2;
                        eventDataPtr[8].DataPointer = (ulong) chPtr3;
                        eventDataPtr[9].DataPointer = (ulong) chPtr4;
                        eventDataPtr[10].DataPointer = (ulong) chPtr5;
                        eventDataPtr[12].DataPointer = (ulong) chPtr6;
                        eventDataPtr[13].DataPointer = (ulong) chPtr7;
                        eventDataPtr[14].DataPointer = (ulong) chPtr8;
                        flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                      }
      }
      return flag;
    }

    internal unsafe bool TemplateKpi(
      ref EventDescriptor eventDescriptor,
      int Count,
      string Metrics)
    {
      int dataCount = 2;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &Count;
        eventDataPtr->Size = 4U;
        eventDataPtr[1].Size = (uint) ((Metrics.Length + 1) * 2);
        fixed (char* chPtr = Metrics)
        {
          eventDataPtr[1].DataPointer = (ulong) chPtr;
          flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
        }
      }
      return flag;
    }

    internal unsafe bool TemplateMachinePoolRequestHistory7(
      ref EventDescriptor eventDescriptor,
      string PoolType,
      string PoolName,
      string InstanceName,
      long RequestId,
      Guid HostId,
      DateTime QueuedTime,
      DateTime AssignedTime,
      DateTime StartTime,
      DateTime FinishTime,
      DateTime UnassignedTime,
      string Outcome,
      string Inputs,
      string Outputs,
      Guid TraceActivityId,
      int MaxParallelism,
      string ImageLabel,
      long TimeoutSeconds,
      long SlaSeconds,
      DateTime SlaStartTime,
      string Tags,
      Guid SubscriptionId,
      string RequiredResourceVersion,
      string SuspiciousActivity,
      string OrchestrationId,
      string PoolRegion,
      string ImageVersion)
    {
      int dataCount = 26;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((PoolType.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((PoolName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((InstanceName.Length + 1) * 2);
        eventDataPtr[3].DataPointer = (ulong) &RequestId;
        eventDataPtr[3].Size = 8U;
        eventDataPtr[4].DataPointer = (ulong) &HostId;
        eventDataPtr[4].Size = (uint) sizeof (Guid);
        short* numPtr1 = stackalloc short[8];
        numPtr1[0] = (short) QueuedTime.Year;
        numPtr1[1] = (short) QueuedTime.Month;
        switch (QueuedTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr1[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr1[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr1[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr1[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr1[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr1[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr1[2] = (short) 6;
            break;
        }
        numPtr1[3] = (short) QueuedTime.Day;
        numPtr1[4] = (short) QueuedTime.Hour;
        numPtr1[5] = (short) QueuedTime.Minute;
        numPtr1[6] = (short) QueuedTime.Second;
        numPtr1[7] = (short) QueuedTime.Millisecond;
        eventDataPtr[5].DataPointer = (ulong) numPtr1;
        eventDataPtr[5].Size = 16U;
        short* numPtr2 = stackalloc short[8];
        numPtr2[0] = (short) AssignedTime.Year;
        numPtr2[1] = (short) AssignedTime.Month;
        switch (AssignedTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr2[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr2[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr2[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr2[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr2[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr2[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr2[2] = (short) 6;
            break;
        }
        numPtr2[3] = (short) AssignedTime.Day;
        numPtr2[4] = (short) AssignedTime.Hour;
        numPtr2[5] = (short) AssignedTime.Minute;
        numPtr2[6] = (short) AssignedTime.Second;
        numPtr2[7] = (short) AssignedTime.Millisecond;
        eventDataPtr[6].DataPointer = (ulong) numPtr2;
        eventDataPtr[6].Size = 16U;
        short* numPtr3 = stackalloc short[8];
        numPtr3[0] = (short) StartTime.Year;
        numPtr3[1] = (short) StartTime.Month;
        switch (StartTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr3[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr3[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr3[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr3[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr3[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr3[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr3[2] = (short) 6;
            break;
        }
        numPtr3[3] = (short) StartTime.Day;
        numPtr3[4] = (short) StartTime.Hour;
        numPtr3[5] = (short) StartTime.Minute;
        numPtr3[6] = (short) StartTime.Second;
        numPtr3[7] = (short) StartTime.Millisecond;
        eventDataPtr[7].DataPointer = (ulong) numPtr3;
        eventDataPtr[7].Size = 16U;
        short* numPtr4 = stackalloc short[8];
        numPtr4[0] = (short) FinishTime.Year;
        numPtr4[1] = (short) FinishTime.Month;
        switch (FinishTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr4[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr4[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr4[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr4[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr4[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr4[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr4[2] = (short) 6;
            break;
        }
        numPtr4[3] = (short) FinishTime.Day;
        numPtr4[4] = (short) FinishTime.Hour;
        numPtr4[5] = (short) FinishTime.Minute;
        numPtr4[6] = (short) FinishTime.Second;
        numPtr4[7] = (short) FinishTime.Millisecond;
        eventDataPtr[8].DataPointer = (ulong) numPtr4;
        eventDataPtr[8].Size = 16U;
        short* numPtr5 = stackalloc short[8];
        numPtr5[0] = (short) UnassignedTime.Year;
        numPtr5[1] = (short) UnassignedTime.Month;
        switch (UnassignedTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr5[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr5[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr5[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr5[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr5[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr5[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr5[2] = (short) 6;
            break;
        }
        numPtr5[3] = (short) UnassignedTime.Day;
        numPtr5[4] = (short) UnassignedTime.Hour;
        numPtr5[5] = (short) UnassignedTime.Minute;
        numPtr5[6] = (short) UnassignedTime.Second;
        numPtr5[7] = (short) UnassignedTime.Millisecond;
        eventDataPtr[9].DataPointer = (ulong) numPtr5;
        eventDataPtr[9].Size = 16U;
        eventDataPtr[10].Size = (uint) ((Outcome.Length + 1) * 2);
        eventDataPtr[11].Size = (uint) ((Inputs.Length + 1) * 2);
        eventDataPtr[12].Size = (uint) ((Outputs.Length + 1) * 2);
        eventDataPtr[13].DataPointer = (ulong) &TraceActivityId;
        eventDataPtr[13].Size = (uint) sizeof (Guid);
        eventDataPtr[14].DataPointer = (ulong) &MaxParallelism;
        eventDataPtr[14].Size = 4U;
        eventDataPtr[15].Size = (uint) ((ImageLabel.Length + 1) * 2);
        eventDataPtr[16].DataPointer = (ulong) &TimeoutSeconds;
        eventDataPtr[16].Size = 8U;
        eventDataPtr[17].DataPointer = (ulong) &SlaSeconds;
        eventDataPtr[17].Size = 8U;
        short* numPtr6 = stackalloc short[8];
        numPtr6[0] = (short) SlaStartTime.Year;
        numPtr6[1] = (short) SlaStartTime.Month;
        switch (SlaStartTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr6[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr6[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr6[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr6[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr6[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr6[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr6[2] = (short) 6;
            break;
        }
        numPtr6[3] = (short) SlaStartTime.Day;
        numPtr6[4] = (short) SlaStartTime.Hour;
        numPtr6[5] = (short) SlaStartTime.Minute;
        numPtr6[6] = (short) SlaStartTime.Second;
        numPtr6[7] = (short) SlaStartTime.Millisecond;
        eventDataPtr[18].DataPointer = (ulong) numPtr6;
        eventDataPtr[18].Size = 16U;
        eventDataPtr[19].Size = (uint) ((Tags.Length + 1) * 2);
        eventDataPtr[20].DataPointer = (ulong) &SubscriptionId;
        eventDataPtr[20].Size = (uint) sizeof (Guid);
        eventDataPtr[21].Size = (uint) ((RequiredResourceVersion.Length + 1) * 2);
        eventDataPtr[22].Size = (uint) ((SuspiciousActivity.Length + 1) * 2);
        eventDataPtr[23].Size = (uint) ((OrchestrationId.Length + 1) * 2);
        eventDataPtr[24].Size = (uint) ((PoolRegion.Length + 1) * 2);
        eventDataPtr[25].Size = (uint) ((ImageVersion.Length + 1) * 2);
        fixed (char* chPtr1 = PoolType)
          fixed (char* chPtr2 = PoolName)
            fixed (char* chPtr3 = InstanceName)
              fixed (char* chPtr4 = Outcome)
                fixed (char* chPtr5 = Inputs)
                  fixed (char* chPtr6 = Outputs)
                    fixed (char* chPtr7 = ImageLabel)
                      fixed (char* chPtr8 = Tags)
                        fixed (char* chPtr9 = RequiredResourceVersion)
                          fixed (char* chPtr10 = SuspiciousActivity)
                            fixed (char* chPtr11 = OrchestrationId)
                              fixed (char* chPtr12 = PoolRegion)
                                fixed (char* chPtr13 = ImageVersion)
                                {
                                  eventDataPtr->DataPointer = (ulong) chPtr1;
                                  eventDataPtr[1].DataPointer = (ulong) chPtr2;
                                  eventDataPtr[2].DataPointer = (ulong) chPtr3;
                                  eventDataPtr[10].DataPointer = (ulong) chPtr4;
                                  eventDataPtr[11].DataPointer = (ulong) chPtr5;
                                  eventDataPtr[12].DataPointer = (ulong) chPtr6;
                                  eventDataPtr[15].DataPointer = (ulong) chPtr7;
                                  eventDataPtr[19].DataPointer = (ulong) chPtr8;
                                  eventDataPtr[21].DataPointer = (ulong) chPtr9;
                                  eventDataPtr[22].DataPointer = (ulong) chPtr10;
                                  eventDataPtr[23].DataPointer = (ulong) chPtr11;
                                  eventDataPtr[24].DataPointer = (ulong) chPtr12;
                                  eventDataPtr[25].DataPointer = (ulong) chPtr13;
                                  flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                                }
      }
      return flag;
    }

    internal unsafe bool TemplateHostHistory3(
      ref EventDescriptor eventDescriptor,
      Guid HostId,
      DateTime ModifiedDate,
      short ActionType,
      Guid ParentHostId,
      string ServerName,
      string DatabaseName,
      string Authority,
      short Status,
      string StatusReason,
      short SupportedFeatures,
      short HostType,
      DateTime LastUserAccess,
      string Name,
      int DatabaseId,
      int StorageAccountId,
      int SubStatus)
    {
      int dataCount = 16;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &HostId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        long fileTime1 = ModifiedDate.ToFileTime();
        eventDataPtr[1].DataPointer = (ulong) &fileTime1;
        eventDataPtr[1].Size = 8U;
        eventDataPtr[2].DataPointer = (ulong) &ActionType;
        eventDataPtr[2].Size = 2U;
        eventDataPtr[3].DataPointer = (ulong) &ParentHostId;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        eventDataPtr[4].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[6].Size = (uint) ((Authority.Length + 1) * 2);
        eventDataPtr[7].DataPointer = (ulong) &Status;
        eventDataPtr[7].Size = 2U;
        eventDataPtr[8].Size = (uint) ((StatusReason.Length + 1) * 2);
        eventDataPtr[9].DataPointer = (ulong) &SupportedFeatures;
        eventDataPtr[9].Size = 2U;
        eventDataPtr[10].DataPointer = (ulong) &HostType;
        eventDataPtr[10].Size = 2U;
        long fileTime2 = LastUserAccess.ToFileTime();
        eventDataPtr[11].DataPointer = (ulong) &fileTime2;
        eventDataPtr[11].Size = 8U;
        eventDataPtr[12].Size = (uint) ((Name.Length + 1) * 2);
        eventDataPtr[13].DataPointer = (ulong) &DatabaseId;
        eventDataPtr[13].Size = 4U;
        eventDataPtr[14].DataPointer = (ulong) &StorageAccountId;
        eventDataPtr[14].Size = 4U;
        eventDataPtr[15].DataPointer = (ulong) &SubStatus;
        eventDataPtr[15].Size = 4U;
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = Authority)
              fixed (char* chPtr4 = StatusReason)
                fixed (char* chPtr5 = Name)
                {
                  eventDataPtr[4].DataPointer = (ulong) chPtr1;
                  eventDataPtr[5].DataPointer = (ulong) chPtr2;
                  eventDataPtr[6].DataPointer = (ulong) chPtr3;
                  eventDataPtr[8].DataPointer = (ulong) chPtr4;
                  eventDataPtr[12].DataPointer = (ulong) chPtr5;
                  flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                }
      }
      return flag;
    }

    internal unsafe bool TemplateSqlRunningStatus6(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string ServerName,
      string DatabaseName,
      short SessionId,
      int Seconds,
      double ElapsedTime,
      string Command,
      short BlockingSessionId,
      short HeadBlockerSessionId,
      int BlockingLevel,
      string Text,
      string Statement,
      string BlockerQueryText,
      string WaitType,
      int WaitTime,
      string LastWaitType,
      string WaitResource,
      long Reads,
      long Writes,
      long LogicalReads,
      int CPUTime,
      int GrantedQueryMemory,
      long RequestedMemory,
      long MaxUsedMemory,
      short Dop,
      string QueryPlan,
      bool IsReadOnly,
      string Listener,
      string LoginName)
    {
      int dataCount = 29;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[3].DataPointer = (ulong) &SessionId;
        eventDataPtr[3].Size = 2U;
        eventDataPtr[4].DataPointer = (ulong) &Seconds;
        eventDataPtr[4].Size = 4U;
        eventDataPtr[5].DataPointer = (ulong) &ElapsedTime;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].Size = (uint) ((Command.Length + 1) * 2);
        eventDataPtr[7].DataPointer = (ulong) &BlockingSessionId;
        eventDataPtr[7].Size = 2U;
        eventDataPtr[8].DataPointer = (ulong) &HeadBlockerSessionId;
        eventDataPtr[8].Size = 2U;
        eventDataPtr[9].DataPointer = (ulong) &BlockingLevel;
        eventDataPtr[9].Size = 4U;
        eventDataPtr[10].Size = (uint) ((Text.Length + 1) * 2);
        eventDataPtr[11].Size = (uint) ((Statement.Length + 1) * 2);
        eventDataPtr[12].Size = (uint) ((BlockerQueryText.Length + 1) * 2);
        eventDataPtr[13].Size = (uint) ((WaitType.Length + 1) * 2);
        eventDataPtr[14].DataPointer = (ulong) &WaitTime;
        eventDataPtr[14].Size = 4U;
        eventDataPtr[15].Size = (uint) ((LastWaitType.Length + 1) * 2);
        eventDataPtr[16].Size = (uint) ((WaitResource.Length + 1) * 2);
        eventDataPtr[17].DataPointer = (ulong) &Reads;
        eventDataPtr[17].Size = 8U;
        eventDataPtr[18].DataPointer = (ulong) &Writes;
        eventDataPtr[18].Size = 8U;
        eventDataPtr[19].DataPointer = (ulong) &LogicalReads;
        eventDataPtr[19].Size = 8U;
        eventDataPtr[20].DataPointer = (ulong) &CPUTime;
        eventDataPtr[20].Size = 4U;
        eventDataPtr[21].DataPointer = (ulong) &GrantedQueryMemory;
        eventDataPtr[21].Size = 4U;
        eventDataPtr[22].DataPointer = (ulong) &RequestedMemory;
        eventDataPtr[22].Size = 8U;
        eventDataPtr[23].DataPointer = (ulong) &MaxUsedMemory;
        eventDataPtr[23].Size = 8U;
        eventDataPtr[24].DataPointer = (ulong) &Dop;
        eventDataPtr[24].Size = 2U;
        eventDataPtr[25].Size = (uint) ((QueryPlan.Length + 1) * 2);
        int num = IsReadOnly ? 1 : 0;
        eventDataPtr[26].DataPointer = (ulong) &num;
        eventDataPtr[26].Size = 4U;
        eventDataPtr[27].Size = (uint) ((Listener.Length + 1) * 2);
        eventDataPtr[28].Size = (uint) ((LoginName.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = Command)
              fixed (char* chPtr4 = Text)
                fixed (char* chPtr5 = Statement)
                  fixed (char* chPtr6 = BlockerQueryText)
                    fixed (char* chPtr7 = WaitType)
                      fixed (char* chPtr8 = LastWaitType)
                        fixed (char* chPtr9 = WaitResource)
                          fixed (char* chPtr10 = QueryPlan)
                            fixed (char* chPtr11 = Listener)
                              fixed (char* chPtr12 = LoginName)
                              {
                                eventDataPtr[1].DataPointer = (ulong) chPtr1;
                                eventDataPtr[2].DataPointer = (ulong) chPtr2;
                                eventDataPtr[6].DataPointer = (ulong) chPtr3;
                                eventDataPtr[10].DataPointer = (ulong) chPtr4;
                                eventDataPtr[11].DataPointer = (ulong) chPtr5;
                                eventDataPtr[12].DataPointer = (ulong) chPtr6;
                                eventDataPtr[13].DataPointer = (ulong) chPtr7;
                                eventDataPtr[15].DataPointer = (ulong) chPtr8;
                                eventDataPtr[16].DataPointer = (ulong) chPtr9;
                                eventDataPtr[25].DataPointer = (ulong) chPtr10;
                                eventDataPtr[27].DataPointer = (ulong) chPtr11;
                                eventDataPtr[28].DataPointer = (ulong) chPtr12;
                                flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                              }
      }
      return flag;
    }

    internal unsafe bool TemplateDatabasePerformanceStatistics14(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string ServerName,
      string DatabaseName,
      DateTime PeriodStart,
      double AverageCpuPercentage,
      double MaximumCpuPercentage,
      double AverageDataIOPercentage,
      double MaximumDataIOPercentage,
      double AverageLogWriteUtilizationPercentage,
      double MaximumLogWriteUtilizationPercentage,
      double AverageMemoryUsagePercentage,
      double MaximumMemoryUsagePercentage,
      string ServiceObjective,
      double AverageWorkerPercentage,
      double MaximumWorkerPercentage,
      double AverageSessionPercentage,
      double MaximumSessionPercentage,
      short dtu_limit,
      string PoolName,
      bool IsReadOnly,
      double AverageXtpPercentage,
      double MaximumXtpPercentage,
      string ResourceVersion,
      int Schedulers,
      string Listener,
      double AverageInstanceCpuPercentage,
      double AverageInstanceMemoryPercentage,
      short ReplicaRole,
      short CompatibilityLevel,
      long RedoQueueSizeKB,
      long RedoRateKBPerSec,
      double SecondaryLagSeconds,
      short SynchronizationHealth,
      string ServiceObjectiveHardware)
    {
      int dataCount = 34;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        short* numPtr = stackalloc short[8];
        numPtr[0] = (short) PeriodStart.Year;
        numPtr[1] = (short) PeriodStart.Month;
        switch (PeriodStart.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr[2] = (short) 6;
            break;
        }
        numPtr[3] = (short) PeriodStart.Day;
        numPtr[4] = (short) PeriodStart.Hour;
        numPtr[5] = (short) PeriodStart.Minute;
        numPtr[6] = (short) PeriodStart.Second;
        numPtr[7] = (short) PeriodStart.Millisecond;
        eventDataPtr[3].DataPointer = (ulong) numPtr;
        eventDataPtr[3].Size = 16U;
        eventDataPtr[4].DataPointer = (ulong) &AverageCpuPercentage;
        eventDataPtr[4].Size = 8U;
        eventDataPtr[5].DataPointer = (ulong) &MaximumCpuPercentage;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].DataPointer = (ulong) &AverageDataIOPercentage;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].DataPointer = (ulong) &MaximumDataIOPercentage;
        eventDataPtr[7].Size = 8U;
        eventDataPtr[8].DataPointer = (ulong) &AverageLogWriteUtilizationPercentage;
        eventDataPtr[8].Size = 8U;
        eventDataPtr[9].DataPointer = (ulong) &MaximumLogWriteUtilizationPercentage;
        eventDataPtr[9].Size = 8U;
        eventDataPtr[10].DataPointer = (ulong) &AverageMemoryUsagePercentage;
        eventDataPtr[10].Size = 8U;
        eventDataPtr[11].DataPointer = (ulong) &MaximumMemoryUsagePercentage;
        eventDataPtr[11].Size = 8U;
        eventDataPtr[12].Size = (uint) ((ServiceObjective.Length + 1) * 2);
        eventDataPtr[13].DataPointer = (ulong) &AverageWorkerPercentage;
        eventDataPtr[13].Size = 8U;
        eventDataPtr[14].DataPointer = (ulong) &MaximumWorkerPercentage;
        eventDataPtr[14].Size = 8U;
        eventDataPtr[15].DataPointer = (ulong) &AverageSessionPercentage;
        eventDataPtr[15].Size = 8U;
        eventDataPtr[16].DataPointer = (ulong) &MaximumSessionPercentage;
        eventDataPtr[16].Size = 8U;
        eventDataPtr[17].DataPointer = (ulong) &dtu_limit;
        eventDataPtr[17].Size = 2U;
        eventDataPtr[18].Size = (uint) ((PoolName.Length + 1) * 2);
        int num = IsReadOnly ? 1 : 0;
        eventDataPtr[19].DataPointer = (ulong) &num;
        eventDataPtr[19].Size = 4U;
        eventDataPtr[20].DataPointer = (ulong) &AverageXtpPercentage;
        eventDataPtr[20].Size = 8U;
        eventDataPtr[21].DataPointer = (ulong) &MaximumXtpPercentage;
        eventDataPtr[21].Size = 8U;
        eventDataPtr[22].Size = (uint) ((ResourceVersion.Length + 1) * 2);
        eventDataPtr[23].DataPointer = (ulong) &Schedulers;
        eventDataPtr[23].Size = 4U;
        eventDataPtr[24].Size = (uint) ((Listener.Length + 1) * 2);
        eventDataPtr[25].DataPointer = (ulong) &AverageInstanceCpuPercentage;
        eventDataPtr[25].Size = 8U;
        eventDataPtr[26].DataPointer = (ulong) &AverageInstanceMemoryPercentage;
        eventDataPtr[26].Size = 8U;
        eventDataPtr[27].DataPointer = (ulong) &ReplicaRole;
        eventDataPtr[27].Size = 2U;
        eventDataPtr[28].DataPointer = (ulong) &CompatibilityLevel;
        eventDataPtr[28].Size = 2U;
        eventDataPtr[29].DataPointer = (ulong) &RedoQueueSizeKB;
        eventDataPtr[29].Size = 8U;
        eventDataPtr[30].DataPointer = (ulong) &RedoRateKBPerSec;
        eventDataPtr[30].Size = 8U;
        eventDataPtr[31].DataPointer = (ulong) &SecondaryLagSeconds;
        eventDataPtr[31].Size = 8U;
        eventDataPtr[32].DataPointer = (ulong) &SynchronizationHealth;
        eventDataPtr[32].Size = 2U;
        eventDataPtr[33].Size = (uint) ((ServiceObjectiveHardware.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = ServiceObjective)
              fixed (char* chPtr4 = PoolName)
                fixed (char* chPtr5 = ResourceVersion)
                  fixed (char* chPtr6 = Listener)
                    fixed (char* chPtr7 = ServiceObjectiveHardware)
                    {
                      eventDataPtr[1].DataPointer = (ulong) chPtr1;
                      eventDataPtr[2].DataPointer = (ulong) chPtr2;
                      eventDataPtr[12].DataPointer = (ulong) chPtr3;
                      eventDataPtr[18].DataPointer = (ulong) chPtr4;
                      eventDataPtr[22].DataPointer = (ulong) chPtr5;
                      eventDataPtr[24].DataPointer = (ulong) chPtr6;
                      eventDataPtr[33].DataPointer = (ulong) chPtr7;
                      flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                    }
      }
      return flag;
    }

    internal unsafe bool TemplateServicingJobDetail(
      ref EventDescriptor eventDescriptor,
      Guid HostId,
      Guid JobId,
      string OperationClass,
      string Operations,
      int JobStatus,
      string JobStatusDesc,
      int Result,
      string ResultDesc,
      DateTime QueueTime,
      DateTime StartTime,
      DateTime EndTime,
      double DurationInSeconds,
      short CompletedStepCount,
      short TotalStepCount)
    {
      int dataCount = 14;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &HostId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].DataPointer = (ulong) &JobId;
        eventDataPtr[1].Size = (uint) sizeof (Guid);
        eventDataPtr[2].Size = (uint) ((OperationClass.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((Operations.Length + 1) * 2);
        eventDataPtr[4].DataPointer = (ulong) &JobStatus;
        eventDataPtr[4].Size = 4U;
        eventDataPtr[5].Size = (uint) ((JobStatusDesc.Length + 1) * 2);
        eventDataPtr[6].DataPointer = (ulong) &Result;
        eventDataPtr[6].Size = 4U;
        eventDataPtr[7].Size = (uint) ((ResultDesc.Length + 1) * 2);
        short* numPtr1 = stackalloc short[8];
        numPtr1[0] = (short) QueueTime.Year;
        numPtr1[1] = (short) QueueTime.Month;
        switch (QueueTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr1[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr1[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr1[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr1[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr1[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr1[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr1[2] = (short) 6;
            break;
        }
        numPtr1[3] = (short) QueueTime.Day;
        numPtr1[4] = (short) QueueTime.Hour;
        numPtr1[5] = (short) QueueTime.Minute;
        numPtr1[6] = (short) QueueTime.Second;
        numPtr1[7] = (short) QueueTime.Millisecond;
        eventDataPtr[8].DataPointer = (ulong) numPtr1;
        eventDataPtr[8].Size = 16U;
        short* numPtr2 = stackalloc short[8];
        numPtr2[0] = (short) StartTime.Year;
        numPtr2[1] = (short) StartTime.Month;
        switch (StartTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr2[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr2[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr2[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr2[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr2[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr2[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr2[2] = (short) 6;
            break;
        }
        numPtr2[3] = (short) StartTime.Day;
        numPtr2[4] = (short) StartTime.Hour;
        numPtr2[5] = (short) StartTime.Minute;
        numPtr2[6] = (short) StartTime.Second;
        numPtr2[7] = (short) StartTime.Millisecond;
        eventDataPtr[9].DataPointer = (ulong) numPtr2;
        eventDataPtr[9].Size = 16U;
        short* numPtr3 = stackalloc short[8];
        numPtr3[0] = (short) EndTime.Year;
        numPtr3[1] = (short) EndTime.Month;
        switch (EndTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr3[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr3[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr3[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr3[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr3[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr3[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr3[2] = (short) 6;
            break;
        }
        numPtr3[3] = (short) EndTime.Day;
        numPtr3[4] = (short) EndTime.Hour;
        numPtr3[5] = (short) EndTime.Minute;
        numPtr3[6] = (short) EndTime.Second;
        numPtr3[7] = (short) EndTime.Millisecond;
        eventDataPtr[10].DataPointer = (ulong) numPtr3;
        eventDataPtr[10].Size = 16U;
        eventDataPtr[11].DataPointer = (ulong) &DurationInSeconds;
        eventDataPtr[11].Size = 8U;
        eventDataPtr[12].DataPointer = (ulong) &CompletedStepCount;
        eventDataPtr[12].Size = 2U;
        eventDataPtr[13].DataPointer = (ulong) &TotalStepCount;
        eventDataPtr[13].Size = 2U;
        fixed (char* chPtr1 = OperationClass)
          fixed (char* chPtr2 = Operations)
            fixed (char* chPtr3 = JobStatusDesc)
              fixed (char* chPtr4 = ResultDesc)
              {
                eventDataPtr[2].DataPointer = (ulong) chPtr1;
                eventDataPtr[3].DataPointer = (ulong) chPtr2;
                eventDataPtr[5].DataPointer = (ulong) chPtr3;
                eventDataPtr[7].DataPointer = (ulong) chPtr4;
                flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
              }
      }
      return flag;
    }

    internal unsafe bool TemplateServicingStepDetail2(
      ref EventDescriptor eventDescriptor,
      DateTime DetailTime,
      string Message,
      string OperationName,
      string GroupName,
      string StepName,
      string EntryKind,
      Guid JobId,
      DateTime QueueTime)
    {
      int dataCount = 8;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        short* numPtr1 = stackalloc short[8];
        numPtr1[0] = (short) DetailTime.Year;
        numPtr1[1] = (short) DetailTime.Month;
        switch (DetailTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr1[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr1[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr1[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr1[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr1[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr1[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr1[2] = (short) 6;
            break;
        }
        numPtr1[3] = (short) DetailTime.Day;
        numPtr1[4] = (short) DetailTime.Hour;
        numPtr1[5] = (short) DetailTime.Minute;
        numPtr1[6] = (short) DetailTime.Second;
        numPtr1[7] = (short) DetailTime.Millisecond;
        eventDataPtr->DataPointer = (ulong) numPtr1;
        eventDataPtr->Size = 16U;
        eventDataPtr[1].Size = (uint) ((Message.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((OperationName.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((GroupName.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((StepName.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((EntryKind.Length + 1) * 2);
        eventDataPtr[6].DataPointer = (ulong) &JobId;
        eventDataPtr[6].Size = (uint) sizeof (Guid);
        short* numPtr2 = stackalloc short[8];
        numPtr2[0] = (short) QueueTime.Year;
        numPtr2[1] = (short) QueueTime.Month;
        switch (QueueTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr2[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr2[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr2[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr2[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr2[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr2[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr2[2] = (short) 6;
            break;
        }
        numPtr2[3] = (short) QueueTime.Day;
        numPtr2[4] = (short) QueueTime.Hour;
        numPtr2[5] = (short) QueueTime.Minute;
        numPtr2[6] = (short) QueueTime.Second;
        numPtr2[7] = (short) QueueTime.Millisecond;
        eventDataPtr[7].DataPointer = (ulong) numPtr2;
        eventDataPtr[7].Size = 16U;
        fixed (char* chPtr1 = Message)
          fixed (char* chPtr2 = OperationName)
            fixed (char* chPtr3 = GroupName)
              fixed (char* chPtr4 = StepName)
                fixed (char* chPtr5 = EntryKind)
                {
                  eventDataPtr[1].DataPointer = (ulong) chPtr1;
                  eventDataPtr[2].DataPointer = (ulong) chPtr2;
                  eventDataPtr[3].DataPointer = (ulong) chPtr3;
                  eventDataPtr[4].DataPointer = (ulong) chPtr4;
                  eventDataPtr[5].DataPointer = (ulong) chPtr5;
                  flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                }
      }
      return flag;
    }

    internal unsafe bool TemplateKpiMetric2(
      ref EventDescriptor eventDescriptor,
      DateTime EventTime,
      string Area,
      string Scope,
      string KpiMetricName,
      double KpiMetricValue,
      Guid HostId,
      string DisplayName,
      string Description)
    {
      int dataCount = 8;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        short* numPtr = stackalloc short[8];
        numPtr[0] = (short) EventTime.Year;
        numPtr[1] = (short) EventTime.Month;
        switch (EventTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr[2] = (short) 6;
            break;
        }
        numPtr[3] = (short) EventTime.Day;
        numPtr[4] = (short) EventTime.Hour;
        numPtr[5] = (short) EventTime.Minute;
        numPtr[6] = (short) EventTime.Second;
        numPtr[7] = (short) EventTime.Millisecond;
        eventDataPtr->DataPointer = (ulong) numPtr;
        eventDataPtr->Size = 16U;
        eventDataPtr[1].Size = (uint) ((Area.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((Scope.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((KpiMetricName.Length + 1) * 2);
        eventDataPtr[4].DataPointer = (ulong) &KpiMetricValue;
        eventDataPtr[4].Size = 8U;
        eventDataPtr[5].DataPointer = (ulong) &HostId;
        eventDataPtr[5].Size = (uint) sizeof (Guid);
        eventDataPtr[6].Size = (uint) ((DisplayName.Length + 1) * 2);
        eventDataPtr[7].Size = (uint) ((Description.Length + 1) * 2);
        fixed (char* chPtr1 = Area)
          fixed (char* chPtr2 = Scope)
            fixed (char* chPtr3 = KpiMetricName)
              fixed (char* chPtr4 = DisplayName)
                fixed (char* chPtr5 = Description)
                {
                  eventDataPtr[1].DataPointer = (ulong) chPtr1;
                  eventDataPtr[2].DataPointer = (ulong) chPtr2;
                  eventDataPtr[3].DataPointer = (ulong) chPtr3;
                  eventDataPtr[6].DataPointer = (ulong) chPtr4;
                  eventDataPtr[7].DataPointer = (ulong) chPtr5;
                  flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                }
      }
      return flag;
    }

    internal unsafe bool TemplateEventMetric2(
      ref EventDescriptor eventDescriptor,
      DateTime EventTime,
      string EventSource,
      string EventMetricName,
      double EventMetricValue,
      int DatabaseId,
      string DeploymentId,
      Guid HostId,
      string MachineName,
      string RoleInstanceId,
      string Scope,
      string EventType,
      int EventId)
    {
      int dataCount = 12;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        short* numPtr = stackalloc short[8];
        numPtr[0] = (short) EventTime.Year;
        numPtr[1] = (short) EventTime.Month;
        switch (EventTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr[2] = (short) 6;
            break;
        }
        numPtr[3] = (short) EventTime.Day;
        numPtr[4] = (short) EventTime.Hour;
        numPtr[5] = (short) EventTime.Minute;
        numPtr[6] = (short) EventTime.Second;
        numPtr[7] = (short) EventTime.Millisecond;
        eventDataPtr->DataPointer = (ulong) numPtr;
        eventDataPtr->Size = 16U;
        eventDataPtr[1].Size = (uint) ((EventSource.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((EventMetricName.Length + 1) * 2);
        eventDataPtr[3].DataPointer = (ulong) &EventMetricValue;
        eventDataPtr[3].Size = 8U;
        eventDataPtr[4].DataPointer = (ulong) &DatabaseId;
        eventDataPtr[4].Size = 4U;
        eventDataPtr[5].Size = (uint) ((DeploymentId.Length + 1) * 2);
        eventDataPtr[6].DataPointer = (ulong) &HostId;
        eventDataPtr[6].Size = (uint) sizeof (Guid);
        eventDataPtr[7].Size = (uint) ((MachineName.Length + 1) * 2);
        eventDataPtr[8].Size = (uint) ((RoleInstanceId.Length + 1) * 2);
        eventDataPtr[9].Size = (uint) ((Scope.Length + 1) * 2);
        eventDataPtr[10].Size = (uint) ((EventType.Length + 1) * 2);
        eventDataPtr[11].DataPointer = (ulong) &EventId;
        eventDataPtr[11].Size = 4U;
        fixed (char* chPtr1 = EventSource)
          fixed (char* chPtr2 = EventMetricName)
            fixed (char* chPtr3 = DeploymentId)
              fixed (char* chPtr4 = MachineName)
                fixed (char* chPtr5 = RoleInstanceId)
                  fixed (char* chPtr6 = Scope)
                    fixed (char* chPtr7 = EventType)
                    {
                      eventDataPtr[1].DataPointer = (ulong) chPtr1;
                      eventDataPtr[2].DataPointer = (ulong) chPtr2;
                      eventDataPtr[5].DataPointer = (ulong) chPtr3;
                      eventDataPtr[7].DataPointer = (ulong) chPtr4;
                      eventDataPtr[8].DataPointer = (ulong) chPtr5;
                      eventDataPtr[9].DataPointer = (ulong) chPtr6;
                      eventDataPtr[10].DataPointer = (ulong) chPtr7;
                      flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                    }
      }
      return flag;
    }

    internal unsafe bool TemplateCommerce(
      ref EventDescriptor eventDescriptor,
      string EntityTypeName,
      string DataFeed)
    {
      int dataCount = 2;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((EntityTypeName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((DataFeed.Length + 1) * 2);
        fixed (char* chPtr1 = EntityTypeName)
          fixed (char* chPtr2 = DataFeed)
          {
            eventDataPtr->DataPointer = (ulong) chPtr1;
            eventDataPtr[1].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateUser(
      ref EventDescriptor eventDescriptor,
      string EntityTypeName,
      string DataFeed)
    {
      int dataCount = 2;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((EntityTypeName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((DataFeed.Length + 1) * 2);
        fixed (char* chPtr1 = EntityTypeName)
          fixed (char* chPtr2 = DataFeed)
          {
            eventDataPtr->DataPointer = (ulong) chPtr1;
            eventDataPtr[1].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateAccount(
      ref EventDescriptor eventDescriptor,
      string EntityTypeName,
      string DataFeed)
    {
      int dataCount = 2;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((EntityTypeName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((DataFeed.Length + 1) * 2);
        fixed (char* chPtr1 = EntityTypeName)
          fixed (char* chPtr2 = DataFeed)
          {
            eventDataPtr->DataPointer = (ulong) chPtr1;
            eventDataPtr[1].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateSubscription(
      ref EventDescriptor eventDescriptor,
      string EntityTypeName,
      string DataFeed)
    {
      int dataCount = 2;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((EntityTypeName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((DataFeed.Length + 1) * 2);
        fixed (char* chPtr1 = EntityTypeName)
          fixed (char* chPtr2 = DataFeed)
          {
            eventDataPtr->DataPointer = (ulong) chPtr1;
            eventDataPtr[1].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateTableSpaceUsage3(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string ServerName,
      string DatabaseName,
      string SchemaName,
      string TableName,
      string IndexName,
      string IndexType,
      string Compression,
      int IndexId,
      long RowCount,
      double UsedSpaceInMB,
      double ReservedSpaceInMB,
      double InRowUsedSpaceInMB,
      double InRowReservedSpaceInMB,
      double LobUsedSpaceInMB,
      double LobReservedSpaceInMB,
      string Listener,
      int PartitionNumber,
      string PartitionBoundary)
    {
      int dataCount = 19;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((SchemaName.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((TableName.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((IndexName.Length + 1) * 2);
        eventDataPtr[6].Size = (uint) ((IndexType.Length + 1) * 2);
        eventDataPtr[7].Size = (uint) ((Compression.Length + 1) * 2);
        eventDataPtr[8].DataPointer = (ulong) &IndexId;
        eventDataPtr[8].Size = 4U;
        eventDataPtr[9].DataPointer = (ulong) &RowCount;
        eventDataPtr[9].Size = 8U;
        eventDataPtr[10].DataPointer = (ulong) &UsedSpaceInMB;
        eventDataPtr[10].Size = 8U;
        eventDataPtr[11].DataPointer = (ulong) &ReservedSpaceInMB;
        eventDataPtr[11].Size = 8U;
        eventDataPtr[12].DataPointer = (ulong) &InRowUsedSpaceInMB;
        eventDataPtr[12].Size = 8U;
        eventDataPtr[13].DataPointer = (ulong) &InRowReservedSpaceInMB;
        eventDataPtr[13].Size = 8U;
        eventDataPtr[14].DataPointer = (ulong) &LobUsedSpaceInMB;
        eventDataPtr[14].Size = 8U;
        eventDataPtr[15].DataPointer = (ulong) &LobReservedSpaceInMB;
        eventDataPtr[15].Size = 8U;
        eventDataPtr[16].Size = (uint) ((Listener.Length + 1) * 2);
        eventDataPtr[17].DataPointer = (ulong) &PartitionNumber;
        eventDataPtr[17].Size = 4U;
        eventDataPtr[18].Size = (uint) ((PartitionBoundary.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = SchemaName)
              fixed (char* chPtr4 = TableName)
                fixed (char* chPtr5 = IndexName)
                  fixed (char* chPtr6 = IndexType)
                    fixed (char* chPtr7 = Compression)
                      fixed (char* chPtr8 = Listener)
                        fixed (char* chPtr9 = PartitionBoundary)
                        {
                          eventDataPtr[1].DataPointer = (ulong) chPtr1;
                          eventDataPtr[2].DataPointer = (ulong) chPtr2;
                          eventDataPtr[3].DataPointer = (ulong) chPtr3;
                          eventDataPtr[4].DataPointer = (ulong) chPtr4;
                          eventDataPtr[5].DataPointer = (ulong) chPtr5;
                          eventDataPtr[6].DataPointer = (ulong) chPtr6;
                          eventDataPtr[7].DataPointer = (ulong) chPtr7;
                          eventDataPtr[16].DataPointer = (ulong) chPtr8;
                          eventDataPtr[18].DataPointer = (ulong) chPtr9;
                          flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                        }
      }
      return flag;
    }

    internal unsafe bool TemplateLicensing(
      ref EventDescriptor eventDescriptor,
      string EntityTypeName,
      string DataFeed)
    {
      int dataCount = 2;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((EntityTypeName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((DataFeed.Length + 1) * 2);
        fixed (char* chPtr1 = EntityTypeName)
          fixed (char* chPtr2 = DataFeed)
          {
            eventDataPtr->DataPointer = (ulong) chPtr1;
            eventDataPtr[1].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateIdentity_SessionTokens(
      ref EventDescriptor eventDescriptor,
      string Operation,
      string SessionTokenType,
      string Error,
      Guid ClientId,
      Guid AccessId,
      Guid AuthorizationId,
      Guid HostAuthorizationId,
      Guid UserId,
      DateTime ValidFrom,
      DateTime ValidTo,
      string DisplayName,
      string Scope,
      string TargetAccounts,
      bool IsValid,
      bool IsPublic,
      string PublicData,
      string Source)
    {
      int dataCount = 17;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((Operation.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((SessionTokenType.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((Error.Length + 1) * 2);
        eventDataPtr[3].DataPointer = (ulong) &ClientId;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        eventDataPtr[4].DataPointer = (ulong) &AccessId;
        eventDataPtr[4].Size = (uint) sizeof (Guid);
        eventDataPtr[5].DataPointer = (ulong) &AuthorizationId;
        eventDataPtr[5].Size = (uint) sizeof (Guid);
        eventDataPtr[6].DataPointer = (ulong) &HostAuthorizationId;
        eventDataPtr[6].Size = (uint) sizeof (Guid);
        eventDataPtr[7].DataPointer = (ulong) &UserId;
        eventDataPtr[7].Size = (uint) sizeof (Guid);
        short* numPtr1 = stackalloc short[8];
        numPtr1[0] = (short) ValidFrom.Year;
        numPtr1[1] = (short) ValidFrom.Month;
        switch (ValidFrom.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr1[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr1[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr1[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr1[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr1[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr1[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr1[2] = (short) 6;
            break;
        }
        numPtr1[3] = (short) ValidFrom.Day;
        numPtr1[4] = (short) ValidFrom.Hour;
        numPtr1[5] = (short) ValidFrom.Minute;
        numPtr1[6] = (short) ValidFrom.Second;
        numPtr1[7] = (short) ValidFrom.Millisecond;
        eventDataPtr[8].DataPointer = (ulong) numPtr1;
        eventDataPtr[8].Size = 16U;
        short* numPtr2 = stackalloc short[8];
        numPtr2[0] = (short) ValidTo.Year;
        numPtr2[1] = (short) ValidTo.Month;
        switch (ValidTo.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr2[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr2[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr2[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr2[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr2[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr2[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr2[2] = (short) 6;
            break;
        }
        numPtr2[3] = (short) ValidTo.Day;
        numPtr2[4] = (short) ValidTo.Hour;
        numPtr2[5] = (short) ValidTo.Minute;
        numPtr2[6] = (short) ValidTo.Second;
        numPtr2[7] = (short) ValidTo.Millisecond;
        eventDataPtr[9].DataPointer = (ulong) numPtr2;
        eventDataPtr[9].Size = 16U;
        eventDataPtr[10].Size = (uint) ((DisplayName.Length + 1) * 2);
        eventDataPtr[11].Size = (uint) ((Scope.Length + 1) * 2);
        eventDataPtr[12].Size = (uint) ((TargetAccounts.Length + 1) * 2);
        int num1 = IsValid ? 1 : 0;
        eventDataPtr[13].DataPointer = (ulong) &num1;
        eventDataPtr[13].Size = 4U;
        int num2 = IsPublic ? 1 : 0;
        eventDataPtr[14].DataPointer = (ulong) &num2;
        eventDataPtr[14].Size = 4U;
        eventDataPtr[15].Size = (uint) ((PublicData.Length + 1) * 2);
        eventDataPtr[16].Size = (uint) ((Source.Length + 1) * 2);
        fixed (char* chPtr1 = Operation)
          fixed (char* chPtr2 = SessionTokenType)
            fixed (char* chPtr3 = Error)
              fixed (char* chPtr4 = DisplayName)
                fixed (char* chPtr5 = Scope)
                  fixed (char* chPtr6 = TargetAccounts)
                    fixed (char* chPtr7 = PublicData)
                      fixed (char* chPtr8 = Source)
                      {
                        eventDataPtr->DataPointer = (ulong) chPtr1;
                        eventDataPtr[1].DataPointer = (ulong) chPtr2;
                        eventDataPtr[2].DataPointer = (ulong) chPtr3;
                        eventDataPtr[10].DataPointer = (ulong) chPtr4;
                        eventDataPtr[11].DataPointer = (ulong) chPtr5;
                        eventDataPtr[12].DataPointer = (ulong) chPtr6;
                        eventDataPtr[15].DataPointer = (ulong) chPtr7;
                        eventDataPtr[16].DataPointer = (ulong) chPtr8;
                        flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                      }
      }
      return flag;
    }

    internal unsafe bool TemplateNotification(
      ref EventDescriptor eventDescriptor,
      DateTime CreatedDate,
      string EventTaskName,
      Guid HostId,
      string EventType,
      string Identifier,
      string DataFeed)
    {
      int dataCount = 6;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        long fileTime = CreatedDate.ToFileTime();
        eventDataPtr->DataPointer = (ulong) &fileTime;
        eventDataPtr->Size = 8U;
        eventDataPtr[1].Size = (uint) ((EventTaskName.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &HostId;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].Size = (uint) ((EventType.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((Identifier.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((DataFeed.Length + 1) * 2);
        fixed (char* chPtr1 = EventTaskName)
          fixed (char* chPtr2 = EventType)
            fixed (char* chPtr3 = Identifier)
              fixed (char* chPtr4 = DataFeed)
              {
                eventDataPtr[1].DataPointer = (ulong) chPtr1;
                eventDataPtr[3].DataPointer = (ulong) chPtr2;
                eventDataPtr[4].DataPointer = (ulong) chPtr3;
                eventDataPtr[5].DataPointer = (ulong) chPtr4;
                flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
              }
      }
      return flag;
    }

    internal unsafe bool TemplateQDS3(
      ref EventDescriptor eventDescriptor,
      long RunId,
      string ServerName,
      string DatabaseName,
      DateTime PeriodStart,
      DateTime PeriodEnd,
      string QueryText,
      long QueryId,
      string ObjectName,
      long QueryTextId,
      long PlanId,
      long TotalPhysicalReads,
      long TotalCpuTime,
      long AverageRowCount,
      long TotalExecutions,
      long TotalLogicalReads,
      long AverageCpuTime,
      long TotalAborted,
      long TotalExceptions,
      long TotalLogicalWrites,
      double AverageDop,
      long AverageQueryMaxUsedMemory,
      long QueryHash,
      long QueryPlanHash,
      string Listener)
    {
      int dataCount = 24;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &RunId;
        eventDataPtr->Size = 8U;
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        short* numPtr1 = stackalloc short[8];
        numPtr1[0] = (short) PeriodStart.Year;
        numPtr1[1] = (short) PeriodStart.Month;
        switch (PeriodStart.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr1[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr1[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr1[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr1[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr1[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr1[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr1[2] = (short) 6;
            break;
        }
        numPtr1[3] = (short) PeriodStart.Day;
        numPtr1[4] = (short) PeriodStart.Hour;
        numPtr1[5] = (short) PeriodStart.Minute;
        numPtr1[6] = (short) PeriodStart.Second;
        numPtr1[7] = (short) PeriodStart.Millisecond;
        eventDataPtr[3].DataPointer = (ulong) numPtr1;
        eventDataPtr[3].Size = 16U;
        short* numPtr2 = stackalloc short[8];
        numPtr2[0] = (short) PeriodEnd.Year;
        numPtr2[1] = (short) PeriodEnd.Month;
        switch (PeriodEnd.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr2[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr2[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr2[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr2[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr2[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr2[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr2[2] = (short) 6;
            break;
        }
        numPtr2[3] = (short) PeriodEnd.Day;
        numPtr2[4] = (short) PeriodEnd.Hour;
        numPtr2[5] = (short) PeriodEnd.Minute;
        numPtr2[6] = (short) PeriodEnd.Second;
        numPtr2[7] = (short) PeriodEnd.Millisecond;
        eventDataPtr[4].DataPointer = (ulong) numPtr2;
        eventDataPtr[4].Size = 16U;
        eventDataPtr[5].Size = (uint) ((QueryText.Length + 1) * 2);
        eventDataPtr[6].DataPointer = (ulong) &QueryId;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].Size = (uint) ((ObjectName.Length + 1) * 2);
        eventDataPtr[8].DataPointer = (ulong) &QueryTextId;
        eventDataPtr[8].Size = 8U;
        eventDataPtr[9].DataPointer = (ulong) &PlanId;
        eventDataPtr[9].Size = 8U;
        eventDataPtr[10].DataPointer = (ulong) &TotalPhysicalReads;
        eventDataPtr[10].Size = 8U;
        eventDataPtr[11].DataPointer = (ulong) &TotalCpuTime;
        eventDataPtr[11].Size = 8U;
        eventDataPtr[12].DataPointer = (ulong) &AverageRowCount;
        eventDataPtr[12].Size = 8U;
        eventDataPtr[13].DataPointer = (ulong) &TotalExecutions;
        eventDataPtr[13].Size = 8U;
        eventDataPtr[14].DataPointer = (ulong) &TotalLogicalReads;
        eventDataPtr[14].Size = 8U;
        eventDataPtr[15].DataPointer = (ulong) &AverageCpuTime;
        eventDataPtr[15].Size = 8U;
        eventDataPtr[16].DataPointer = (ulong) &TotalAborted;
        eventDataPtr[16].Size = 8U;
        eventDataPtr[17].DataPointer = (ulong) &TotalExceptions;
        eventDataPtr[17].Size = 8U;
        eventDataPtr[18].DataPointer = (ulong) &TotalLogicalWrites;
        eventDataPtr[18].Size = 8U;
        eventDataPtr[19].DataPointer = (ulong) &AverageDop;
        eventDataPtr[19].Size = 8U;
        eventDataPtr[20].DataPointer = (ulong) &AverageQueryMaxUsedMemory;
        eventDataPtr[20].Size = 8U;
        eventDataPtr[21].DataPointer = (ulong) &QueryHash;
        eventDataPtr[21].Size = 8U;
        eventDataPtr[22].DataPointer = (ulong) &QueryPlanHash;
        eventDataPtr[22].Size = 8U;
        eventDataPtr[23].Size = (uint) ((Listener.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = QueryText)
              fixed (char* chPtr4 = ObjectName)
                fixed (char* chPtr5 = Listener)
                {
                  eventDataPtr[1].DataPointer = (ulong) chPtr1;
                  eventDataPtr[2].DataPointer = (ulong) chPtr2;
                  eventDataPtr[5].DataPointer = (ulong) chPtr3;
                  eventDataPtr[7].DataPointer = (ulong) chPtr4;
                  eventDataPtr[23].DataPointer = (ulong) chPtr5;
                  flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                }
      }
      return flag;
    }

    internal unsafe bool TemplateIdentity_Reads(
      ref EventDescriptor eventDescriptor,
      string ClassName,
      string Flavor,
      string Identifier,
      string QueryMembership,
      string PropertyNameFilters,
      string Options,
      string CallStack)
    {
      int dataCount = 7;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((ClassName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((Flavor.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((Identifier.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((QueryMembership.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((PropertyNameFilters.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((Options.Length + 1) * 2);
        eventDataPtr[6].Size = (uint) ((CallStack.Length + 1) * 2);
        fixed (char* chPtr1 = ClassName)
          fixed (char* chPtr2 = Flavor)
            fixed (char* chPtr3 = Identifier)
              fixed (char* chPtr4 = QueryMembership)
                fixed (char* chPtr5 = PropertyNameFilters)
                  fixed (char* chPtr6 = Options)
                    fixed (char* chPtr7 = CallStack)
                    {
                      eventDataPtr->DataPointer = (ulong) chPtr1;
                      eventDataPtr[1].DataPointer = (ulong) chPtr2;
                      eventDataPtr[2].DataPointer = (ulong) chPtr3;
                      eventDataPtr[3].DataPointer = (ulong) chPtr4;
                      eventDataPtr[4].DataPointer = (ulong) chPtr5;
                      eventDataPtr[5].DataPointer = (ulong) chPtr6;
                      eventDataPtr[6].DataPointer = (ulong) chPtr7;
                      flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                    }
      }
      return flag;
    }

    internal unsafe bool TemplateIdentity_Token(
      ref EventDescriptor eventDescriptor,
      string ClassName,
      string Header,
      string Claims,
      string Nonce)
    {
      int dataCount = 4;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((ClassName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((Header.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((Claims.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((Nonce.Length + 1) * 2);
        fixed (char* chPtr1 = ClassName)
          fixed (char* chPtr2 = Header)
            fixed (char* chPtr3 = Claims)
              fixed (char* chPtr4 = Nonce)
              {
                eventDataPtr->DataPointer = (ulong) chPtr1;
                eventDataPtr[1].DataPointer = (ulong) chPtr2;
                eventDataPtr[2].DataPointer = (ulong) chPtr3;
                eventDataPtr[3].DataPointer = (ulong) chPtr4;
                flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
              }
      }
      return flag;
    }

    internal unsafe bool TemplateIdentity_Cache_Changes(
      ref EventDescriptor eventDescriptor,
      string StoreType,
      string EventType,
      string SearchFilter,
      string DomainId,
      string EventValue,
      string QueryMembership,
      string CacheResult,
      string CallStack)
    {
      int dataCount = 8;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((StoreType.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((EventType.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((SearchFilter.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((DomainId.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((EventValue.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((QueryMembership.Length + 1) * 2);
        eventDataPtr[6].Size = (uint) ((CacheResult.Length + 1) * 2);
        eventDataPtr[7].Size = (uint) ((CallStack.Length + 1) * 2);
        fixed (char* chPtr1 = StoreType)
          fixed (char* chPtr2 = EventType)
            fixed (char* chPtr3 = SearchFilter)
              fixed (char* chPtr4 = DomainId)
                fixed (char* chPtr5 = EventValue)
                  fixed (char* chPtr6 = QueryMembership)
                    fixed (char* chPtr7 = CacheResult)
                      fixed (char* chPtr8 = CallStack)
                      {
                        eventDataPtr->DataPointer = (ulong) chPtr1;
                        eventDataPtr[1].DataPointer = (ulong) chPtr2;
                        eventDataPtr[2].DataPointer = (ulong) chPtr3;
                        eventDataPtr[3].DataPointer = (ulong) chPtr4;
                        eventDataPtr[4].DataPointer = (ulong) chPtr5;
                        eventDataPtr[5].DataPointer = (ulong) chPtr6;
                        eventDataPtr[6].DataPointer = (ulong) chPtr7;
                        eventDataPtr[7].DataPointer = (ulong) chPtr8;
                        flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                      }
      }
      return flag;
    }

    internal unsafe bool TemplateIdentity_Sql_Changes(
      ref EventDescriptor eventDescriptor,
      string EventType,
      string DomainId,
      string EventValue,
      string QueryMembership,
      string CallStack)
    {
      int dataCount = 5;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((EventType.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((DomainId.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((EventValue.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((QueryMembership.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((CallStack.Length + 1) * 2);
        fixed (char* chPtr1 = EventType)
          fixed (char* chPtr2 = DomainId)
            fixed (char* chPtr3 = EventValue)
              fixed (char* chPtr4 = QueryMembership)
                fixed (char* chPtr5 = CallStack)
                {
                  eventDataPtr->DataPointer = (ulong) chPtr1;
                  eventDataPtr[1].DataPointer = (ulong) chPtr2;
                  eventDataPtr[2].DataPointer = (ulong) chPtr3;
                  eventDataPtr[3].DataPointer = (ulong) chPtr4;
                  eventDataPtr[4].DataPointer = (ulong) chPtr5;
                  flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                }
      }
      return flag;
    }

    internal unsafe bool TemplateOrganization(
      ref EventDescriptor eventDescriptor,
      string EntityTypeName,
      string DataFeed)
    {
      int dataCount = 2;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((EntityTypeName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((DataFeed.Length + 1) * 2);
        fixed (char* chPtr1 = EntityTypeName)
          fixed (char* chPtr2 = DataFeed)
          {
            eventDataPtr->DataPointer = (ulong) chPtr1;
            eventDataPtr[1].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateDirectory_Member(
      ref EventDescriptor eventDescriptor,
      string EntityTypeName,
      string DataFeed)
    {
      int dataCount = 2;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((EntityTypeName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((DataFeed.Length + 1) * 2);
        fixed (char* chPtr1 = EntityTypeName)
          fixed (char* chPtr2 = DataFeed)
          {
            eventDataPtr->DataPointer = (ulong) chPtr1;
            eventDataPtr[1].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateStorageMetricsTransactions5(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string StorageAccount,
      string StorageService,
      DateTime StartingIntervalUTC,
      long TotalIngress,
      long TotalEgress,
      long TotalRequests,
      long TotalBillableRequests,
      double Availability,
      double AverageE2ELatency,
      double AverageServerLatency,
      double PercentSuccess,
      double PercentThrottlingError,
      double PercentTimeoutError,
      double PercentServerOtherError,
      double PercentClientOtherError,
      double PercentAuthorizationError,
      double PercentNetworkError,
      long Success,
      long AnonymousSuccess,
      long SasSuccess,
      long ThrottlingError,
      long AnonymousThrottlingError,
      long SasThrottlingError,
      long ClientTimeoutError,
      long AnonymousClientTimeoutError,
      long SasClientTimeoutError,
      long ServerTimeoutError,
      long AnonymousServerTimeoutError,
      long SasServerTimeoutError,
      long ClientOtherError,
      long SasClientOtherError,
      long AnonymousClientOtherError,
      long ServerOtherError,
      long AnonymousServerOtherError,
      long SasServerOtherError,
      long AuthorizationError,
      long AnonymousAuthorizationError,
      long SasAuthorizationError,
      long NetworkError,
      long AnonymousNetworkError,
      long SasNetworkError,
      string OperationType,
      string StorageCluster,
      string StorageKind,
      string StorageSku,
      DateTime BlobGeoLastSyncTime,
      DateTime TableGeoLastSyncTime,
      DateTime QueueGeoLastSyncTime)
    {
      int dataCount = 49;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((StorageAccount.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((StorageService.Length + 1) * 2);
        long fileTime1 = StartingIntervalUTC.ToFileTime();
        eventDataPtr[3].DataPointer = (ulong) &fileTime1;
        eventDataPtr[3].Size = 8U;
        eventDataPtr[4].DataPointer = (ulong) &TotalIngress;
        eventDataPtr[4].Size = 8U;
        eventDataPtr[5].DataPointer = (ulong) &TotalEgress;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].DataPointer = (ulong) &TotalRequests;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].DataPointer = (ulong) &TotalBillableRequests;
        eventDataPtr[7].Size = 8U;
        eventDataPtr[8].DataPointer = (ulong) &Availability;
        eventDataPtr[8].Size = 8U;
        eventDataPtr[9].DataPointer = (ulong) &AverageE2ELatency;
        eventDataPtr[9].Size = 8U;
        eventDataPtr[10].DataPointer = (ulong) &AverageServerLatency;
        eventDataPtr[10].Size = 8U;
        eventDataPtr[11].DataPointer = (ulong) &PercentSuccess;
        eventDataPtr[11].Size = 8U;
        eventDataPtr[12].DataPointer = (ulong) &PercentThrottlingError;
        eventDataPtr[12].Size = 8U;
        eventDataPtr[13].DataPointer = (ulong) &PercentTimeoutError;
        eventDataPtr[13].Size = 8U;
        eventDataPtr[14].DataPointer = (ulong) &PercentServerOtherError;
        eventDataPtr[14].Size = 8U;
        eventDataPtr[15].DataPointer = (ulong) &PercentClientOtherError;
        eventDataPtr[15].Size = 8U;
        eventDataPtr[16].DataPointer = (ulong) &PercentAuthorizationError;
        eventDataPtr[16].Size = 8U;
        eventDataPtr[17].DataPointer = (ulong) &PercentNetworkError;
        eventDataPtr[17].Size = 8U;
        eventDataPtr[18].DataPointer = (ulong) &Success;
        eventDataPtr[18].Size = 8U;
        eventDataPtr[19].DataPointer = (ulong) &AnonymousSuccess;
        eventDataPtr[19].Size = 8U;
        eventDataPtr[20].DataPointer = (ulong) &SasSuccess;
        eventDataPtr[20].Size = 8U;
        eventDataPtr[21].DataPointer = (ulong) &ThrottlingError;
        eventDataPtr[21].Size = 8U;
        eventDataPtr[22].DataPointer = (ulong) &AnonymousThrottlingError;
        eventDataPtr[22].Size = 8U;
        eventDataPtr[23].DataPointer = (ulong) &SasThrottlingError;
        eventDataPtr[23].Size = 8U;
        eventDataPtr[24].DataPointer = (ulong) &ClientTimeoutError;
        eventDataPtr[24].Size = 8U;
        eventDataPtr[25].DataPointer = (ulong) &AnonymousClientTimeoutError;
        eventDataPtr[25].Size = 8U;
        eventDataPtr[26].DataPointer = (ulong) &SasClientTimeoutError;
        eventDataPtr[26].Size = 8U;
        eventDataPtr[27].DataPointer = (ulong) &ServerTimeoutError;
        eventDataPtr[27].Size = 8U;
        eventDataPtr[28].DataPointer = (ulong) &AnonymousServerTimeoutError;
        eventDataPtr[28].Size = 8U;
        eventDataPtr[29].DataPointer = (ulong) &SasServerTimeoutError;
        eventDataPtr[29].Size = 8U;
        eventDataPtr[30].DataPointer = (ulong) &ClientOtherError;
        eventDataPtr[30].Size = 8U;
        eventDataPtr[31].DataPointer = (ulong) &SasClientOtherError;
        eventDataPtr[31].Size = 8U;
        eventDataPtr[32].DataPointer = (ulong) &AnonymousClientOtherError;
        eventDataPtr[32].Size = 8U;
        eventDataPtr[33].DataPointer = (ulong) &ServerOtherError;
        eventDataPtr[33].Size = 8U;
        eventDataPtr[34].DataPointer = (ulong) &AnonymousServerOtherError;
        eventDataPtr[34].Size = 8U;
        eventDataPtr[35].DataPointer = (ulong) &SasServerOtherError;
        eventDataPtr[35].Size = 8U;
        eventDataPtr[36].DataPointer = (ulong) &AuthorizationError;
        eventDataPtr[36].Size = 8U;
        eventDataPtr[37].DataPointer = (ulong) &AnonymousAuthorizationError;
        eventDataPtr[37].Size = 8U;
        eventDataPtr[38].DataPointer = (ulong) &SasAuthorizationError;
        eventDataPtr[38].Size = 8U;
        eventDataPtr[39].DataPointer = (ulong) &NetworkError;
        eventDataPtr[39].Size = 8U;
        eventDataPtr[40].DataPointer = (ulong) &AnonymousNetworkError;
        eventDataPtr[40].Size = 8U;
        eventDataPtr[41].DataPointer = (ulong) &SasNetworkError;
        eventDataPtr[41].Size = 8U;
        eventDataPtr[42].Size = (uint) ((OperationType.Length + 1) * 2);
        eventDataPtr[43].Size = (uint) ((StorageCluster.Length + 1) * 2);
        eventDataPtr[44].Size = (uint) ((StorageKind.Length + 1) * 2);
        eventDataPtr[45].Size = (uint) ((StorageSku.Length + 1) * 2);
        long fileTime2 = BlobGeoLastSyncTime.ToFileTime();
        eventDataPtr[46].DataPointer = (ulong) &fileTime2;
        eventDataPtr[46].Size = 8U;
        long fileTime3 = TableGeoLastSyncTime.ToFileTime();
        eventDataPtr[47].DataPointer = (ulong) &fileTime3;
        eventDataPtr[47].Size = 8U;
        long fileTime4 = QueueGeoLastSyncTime.ToFileTime();
        eventDataPtr[48].DataPointer = (ulong) &fileTime4;
        eventDataPtr[48].Size = 8U;
        fixed (char* chPtr1 = StorageAccount)
          fixed (char* chPtr2 = StorageService)
            fixed (char* chPtr3 = OperationType)
              fixed (char* chPtr4 = StorageCluster)
                fixed (char* chPtr5 = StorageKind)
                  fixed (char* chPtr6 = StorageSku)
                  {
                    eventDataPtr[1].DataPointer = (ulong) chPtr1;
                    eventDataPtr[2].DataPointer = (ulong) chPtr2;
                    eventDataPtr[42].DataPointer = (ulong) chPtr3;
                    eventDataPtr[43].DataPointer = (ulong) chPtr4;
                    eventDataPtr[44].DataPointer = (ulong) chPtr5;
                    eventDataPtr[45].DataPointer = (ulong) chPtr6;
                    flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                  }
      }
      return flag;
    }

    internal unsafe bool TemplateStorageAnalyticsLogs(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string StorageAccount,
      string StorageService,
      string VersionNumber,
      DateTime RequestStartTime,
      string OperationType,
      string RequestStatus,
      string HttpStatusCode,
      string EndtoEndLatencyInMs,
      string ServerLatencyInMs,
      string AuthenticationType,
      string RequesterAccountName,
      string OwnerAccountName,
      string RequestUrl,
      string RequestedObjectKey,
      Guid RequestIdHeader,
      long OperationCount,
      string RequesterIpAddress,
      string RequestVersionHeader,
      long RequestHeaderSize,
      long RequestPacketSize,
      long ResponseHeaderSize,
      long ResponsePacketSize,
      long RequestContentLength,
      string RequestMd5,
      string ServerMd5,
      string EtagIdentifier,
      DateTime LastModifiedTime,
      string ConditionsUsed,
      string UserAgentHeader,
      string ReferrerHeader,
      string ClientRequestId)
    {
      int dataCount = 32;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((StorageAccount.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((StorageService.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((VersionNumber.Length + 1) * 2);
        long fileTime1 = RequestStartTime.ToFileTime();
        eventDataPtr[4].DataPointer = (ulong) &fileTime1;
        eventDataPtr[4].Size = 8U;
        eventDataPtr[5].Size = (uint) ((OperationType.Length + 1) * 2);
        eventDataPtr[6].Size = (uint) ((RequestStatus.Length + 1) * 2);
        eventDataPtr[7].Size = (uint) ((HttpStatusCode.Length + 1) * 2);
        eventDataPtr[8].Size = (uint) ((EndtoEndLatencyInMs.Length + 1) * 2);
        eventDataPtr[9].Size = (uint) ((ServerLatencyInMs.Length + 1) * 2);
        eventDataPtr[10].Size = (uint) ((AuthenticationType.Length + 1) * 2);
        eventDataPtr[11].Size = (uint) ((RequesterAccountName.Length + 1) * 2);
        eventDataPtr[12].Size = (uint) ((OwnerAccountName.Length + 1) * 2);
        eventDataPtr[13].Size = (uint) ((RequestUrl.Length + 1) * 2);
        eventDataPtr[14].Size = (uint) ((RequestedObjectKey.Length + 1) * 2);
        eventDataPtr[15].DataPointer = (ulong) &RequestIdHeader;
        eventDataPtr[15].Size = (uint) sizeof (Guid);
        eventDataPtr[16].DataPointer = (ulong) &OperationCount;
        eventDataPtr[16].Size = 8U;
        eventDataPtr[17].Size = (uint) ((RequesterIpAddress.Length + 1) * 2);
        eventDataPtr[18].Size = (uint) ((RequestVersionHeader.Length + 1) * 2);
        eventDataPtr[19].DataPointer = (ulong) &RequestHeaderSize;
        eventDataPtr[19].Size = 8U;
        eventDataPtr[20].DataPointer = (ulong) &RequestPacketSize;
        eventDataPtr[20].Size = 8U;
        eventDataPtr[21].DataPointer = (ulong) &ResponseHeaderSize;
        eventDataPtr[21].Size = 8U;
        eventDataPtr[22].DataPointer = (ulong) &ResponsePacketSize;
        eventDataPtr[22].Size = 8U;
        eventDataPtr[23].DataPointer = (ulong) &RequestContentLength;
        eventDataPtr[23].Size = 8U;
        eventDataPtr[24].Size = (uint) ((RequestMd5.Length + 1) * 2);
        eventDataPtr[25].Size = (uint) ((ServerMd5.Length + 1) * 2);
        eventDataPtr[26].Size = (uint) ((EtagIdentifier.Length + 1) * 2);
        long fileTime2 = LastModifiedTime.ToFileTime();
        eventDataPtr[27].DataPointer = (ulong) &fileTime2;
        eventDataPtr[27].Size = 8U;
        eventDataPtr[28].Size = (uint) ((ConditionsUsed.Length + 1) * 2);
        eventDataPtr[29].Size = (uint) ((UserAgentHeader.Length + 1) * 2);
        eventDataPtr[30].Size = (uint) ((ReferrerHeader.Length + 1) * 2);
        eventDataPtr[31].Size = (uint) ((ClientRequestId.Length + 1) * 2);
        fixed (char* chPtr1 = StorageAccount)
          fixed (char* chPtr2 = StorageService)
            fixed (char* chPtr3 = VersionNumber)
              fixed (char* chPtr4 = OperationType)
                fixed (char* chPtr5 = RequestStatus)
                  fixed (char* chPtr6 = HttpStatusCode)
                    fixed (char* chPtr7 = EndtoEndLatencyInMs)
                      fixed (char* chPtr8 = ServerLatencyInMs)
                        fixed (char* chPtr9 = AuthenticationType)
                          fixed (char* chPtr10 = RequesterAccountName)
                            fixed (char* chPtr11 = OwnerAccountName)
                              fixed (char* chPtr12 = RequestUrl)
                                fixed (char* chPtr13 = RequestedObjectKey)
                                  fixed (char* chPtr14 = RequesterIpAddress)
                                    fixed (char* chPtr15 = RequestVersionHeader)
                                      fixed (char* chPtr16 = RequestMd5)
                                        fixed (char* chPtr17 = ServerMd5)
                                          fixed (char* chPtr18 = EtagIdentifier)
                                            fixed (char* chPtr19 = ConditionsUsed)
                                              fixed (char* chPtr20 = UserAgentHeader)
                                                fixed (char* chPtr21 = ReferrerHeader)
                                                  fixed (char* chPtr22 = ClientRequestId)
                                                  {
                                                    eventDataPtr[1].DataPointer = (ulong) chPtr1;
                                                    eventDataPtr[2].DataPointer = (ulong) chPtr2;
                                                    eventDataPtr[3].DataPointer = (ulong) chPtr3;
                                                    eventDataPtr[5].DataPointer = (ulong) chPtr4;
                                                    eventDataPtr[6].DataPointer = (ulong) chPtr5;
                                                    eventDataPtr[7].DataPointer = (ulong) chPtr6;
                                                    eventDataPtr[8].DataPointer = (ulong) chPtr7;
                                                    eventDataPtr[9].DataPointer = (ulong) chPtr8;
                                                    eventDataPtr[10].DataPointer = (ulong) chPtr9;
                                                    eventDataPtr[11].DataPointer = (ulong) chPtr10;
                                                    eventDataPtr[12].DataPointer = (ulong) chPtr11;
                                                    eventDataPtr[13].DataPointer = (ulong) chPtr12;
                                                    eventDataPtr[14].DataPointer = (ulong) chPtr13;
                                                    eventDataPtr[17].DataPointer = (ulong) chPtr14;
                                                    eventDataPtr[18].DataPointer = (ulong) chPtr15;
                                                    eventDataPtr[24].DataPointer = (ulong) chPtr16;
                                                    eventDataPtr[25].DataPointer = (ulong) chPtr17;
                                                    eventDataPtr[26].DataPointer = (ulong) chPtr18;
                                                    eventDataPtr[28].DataPointer = (ulong) chPtr19;
                                                    eventDataPtr[29].DataPointer = (ulong) chPtr20;
                                                    eventDataPtr[30].DataPointer = (ulong) chPtr21;
                                                    eventDataPtr[31].DataPointer = (ulong) chPtr22;
                                                    flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                                                  }
      }
      return flag;
    }

    internal unsafe bool TemplateResourceUtilization3(
      ref EventDescriptor eventDescriptor,
      DateTime StartTime,
      Guid HostId,
      Guid VSID,
      Guid ActivityId,
      string DataFeed,
      Guid CUID,
      int Channel)
    {
      int dataCount = 7;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        short* numPtr = stackalloc short[8];
        numPtr[0] = (short) StartTime.Year;
        numPtr[1] = (short) StartTime.Month;
        switch (StartTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr[2] = (short) 6;
            break;
        }
        numPtr[3] = (short) StartTime.Day;
        numPtr[4] = (short) StartTime.Hour;
        numPtr[5] = (short) StartTime.Minute;
        numPtr[6] = (short) StartTime.Second;
        numPtr[7] = (short) StartTime.Millisecond;
        eventDataPtr->DataPointer = (ulong) numPtr;
        eventDataPtr->Size = 16U;
        eventDataPtr[1].DataPointer = (ulong) &HostId;
        eventDataPtr[1].Size = (uint) sizeof (Guid);
        eventDataPtr[2].DataPointer = (ulong) &VSID;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].DataPointer = (ulong) &ActivityId;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        eventDataPtr[4].Size = (uint) ((DataFeed.Length + 1) * 2);
        eventDataPtr[5].DataPointer = (ulong) &CUID;
        eventDataPtr[5].Size = (uint) sizeof (Guid);
        eventDataPtr[6].DataPointer = (ulong) &Channel;
        eventDataPtr[6].Size = 4U;
        fixed (char* chPtr = DataFeed)
        {
          eventDataPtr[4].DataPointer = (ulong) chPtr;
          flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
        }
      }
      return flag;
    }

    internal unsafe bool TemplateHostingServiceCertficates(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string Source,
      string Thumbprint,
      string SerialNumber,
      string SubjectName,
      string IssuerName,
      string SignatureAlgorithm,
      DateTime CreatedDate,
      DateTime ExpiryDate)
    {
      int dataCount = 9;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((Source.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((Thumbprint.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((SerialNumber.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((SubjectName.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((IssuerName.Length + 1) * 2);
        eventDataPtr[6].Size = (uint) ((SignatureAlgorithm.Length + 1) * 2);
        long fileTime1 = CreatedDate.ToFileTime();
        eventDataPtr[7].DataPointer = (ulong) &fileTime1;
        eventDataPtr[7].Size = 8U;
        long fileTime2 = ExpiryDate.ToFileTime();
        eventDataPtr[8].DataPointer = (ulong) &fileTime2;
        eventDataPtr[8].Size = 8U;
        fixed (char* chPtr1 = Source)
          fixed (char* chPtr2 = Thumbprint)
            fixed (char* chPtr3 = SerialNumber)
              fixed (char* chPtr4 = SubjectName)
                fixed (char* chPtr5 = IssuerName)
                  fixed (char* chPtr6 = SignatureAlgorithm)
                  {
                    eventDataPtr[1].DataPointer = (ulong) chPtr1;
                    eventDataPtr[2].DataPointer = (ulong) chPtr2;
                    eventDataPtr[3].DataPointer = (ulong) chPtr3;
                    eventDataPtr[4].DataPointer = (ulong) chPtr4;
                    eventDataPtr[5].DataPointer = (ulong) chPtr5;
                    eventDataPtr[6].DataPointer = (ulong) chPtr6;
                    flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                  }
      }
      return flag;
    }

    internal unsafe bool TemplateSqlServerAlwaysOnHealthStats(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string ListenerName,
      Guid GroupId,
      string GroupName,
      Guid ReplicaId,
      string ReplicaServerName,
      Guid ReplicaDatabaseId,
      string DatabaseName,
      string ConnectedStateDesc,
      string AvailabilityModeDesc,
      string SynchronizationStateDesc,
      string ReplicaRoleDesc,
      int IsLocal,
      int IsJoined,
      int IsSuspended,
      string SuspendReasonDesc,
      int IsFailoverReady,
      int EstimatedDataLossIfFailoverNotReadyInSec,
      double EstimatedRecoveryTime,
      long FileStreamSendRate,
      long LogSendQueueSize,
      long LogSendRate,
      long RedoQueueSize,
      long RedoRate,
      double SynchronizationPerformance,
      string LastCommitLsn,
      DateTime LastCommitTime,
      string LastHardenedLsn,
      DateTime LastHardenedTime,
      string LastReceivedLsn,
      DateTime LastReceivedTime,
      string LastSentLsn,
      DateTime LastSentTime,
      string LastRedoneLsn,
      DateTime LastRedoneTime,
      string EndOfLogLsn,
      string RecoveryLsn,
      string TruncationLsn,
      DateTime CollectionTime)
    {
      int dataCount = 39;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((ListenerName.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &GroupId;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].Size = (uint) ((GroupName.Length + 1) * 2);
        eventDataPtr[4].DataPointer = (ulong) &ReplicaId;
        eventDataPtr[4].Size = (uint) sizeof (Guid);
        eventDataPtr[5].Size = (uint) ((ReplicaServerName.Length + 1) * 2);
        eventDataPtr[6].DataPointer = (ulong) &ReplicaDatabaseId;
        eventDataPtr[6].Size = (uint) sizeof (Guid);
        eventDataPtr[7].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[8].Size = (uint) ((ConnectedStateDesc.Length + 1) * 2);
        eventDataPtr[9].Size = (uint) ((AvailabilityModeDesc.Length + 1) * 2);
        eventDataPtr[10].Size = (uint) ((SynchronizationStateDesc.Length + 1) * 2);
        eventDataPtr[11].Size = (uint) ((ReplicaRoleDesc.Length + 1) * 2);
        eventDataPtr[12].DataPointer = (ulong) &IsLocal;
        eventDataPtr[12].Size = 4U;
        eventDataPtr[13].DataPointer = (ulong) &IsJoined;
        eventDataPtr[13].Size = 4U;
        eventDataPtr[14].DataPointer = (ulong) &IsSuspended;
        eventDataPtr[14].Size = 4U;
        eventDataPtr[15].Size = (uint) ((SuspendReasonDesc.Length + 1) * 2);
        eventDataPtr[16].DataPointer = (ulong) &IsFailoverReady;
        eventDataPtr[16].Size = 4U;
        eventDataPtr[17].DataPointer = (ulong) &EstimatedDataLossIfFailoverNotReadyInSec;
        eventDataPtr[17].Size = 4U;
        eventDataPtr[18].DataPointer = (ulong) &EstimatedRecoveryTime;
        eventDataPtr[18].Size = 8U;
        eventDataPtr[19].DataPointer = (ulong) &FileStreamSendRate;
        eventDataPtr[19].Size = 8U;
        eventDataPtr[20].DataPointer = (ulong) &LogSendQueueSize;
        eventDataPtr[20].Size = 8U;
        eventDataPtr[21].DataPointer = (ulong) &LogSendRate;
        eventDataPtr[21].Size = 8U;
        eventDataPtr[22].DataPointer = (ulong) &RedoQueueSize;
        eventDataPtr[22].Size = 8U;
        eventDataPtr[23].DataPointer = (ulong) &RedoRate;
        eventDataPtr[23].Size = 8U;
        eventDataPtr[24].DataPointer = (ulong) &SynchronizationPerformance;
        eventDataPtr[24].Size = 8U;
        eventDataPtr[25].Size = (uint) ((LastCommitLsn.Length + 1) * 2);
        long fileTime1 = LastCommitTime.ToFileTime();
        eventDataPtr[26].DataPointer = (ulong) &fileTime1;
        eventDataPtr[26].Size = 8U;
        eventDataPtr[27].Size = (uint) ((LastHardenedLsn.Length + 1) * 2);
        long fileTime2 = LastHardenedTime.ToFileTime();
        eventDataPtr[28].DataPointer = (ulong) &fileTime2;
        eventDataPtr[28].Size = 8U;
        eventDataPtr[29].Size = (uint) ((LastReceivedLsn.Length + 1) * 2);
        long fileTime3 = LastReceivedTime.ToFileTime();
        eventDataPtr[30].DataPointer = (ulong) &fileTime3;
        eventDataPtr[30].Size = 8U;
        eventDataPtr[31].Size = (uint) ((LastSentLsn.Length + 1) * 2);
        long fileTime4 = LastSentTime.ToFileTime();
        eventDataPtr[32].DataPointer = (ulong) &fileTime4;
        eventDataPtr[32].Size = 8U;
        eventDataPtr[33].Size = (uint) ((LastRedoneLsn.Length + 1) * 2);
        long fileTime5 = LastRedoneTime.ToFileTime();
        eventDataPtr[34].DataPointer = (ulong) &fileTime5;
        eventDataPtr[34].Size = 8U;
        eventDataPtr[35].Size = (uint) ((EndOfLogLsn.Length + 1) * 2);
        eventDataPtr[36].Size = (uint) ((RecoveryLsn.Length + 1) * 2);
        eventDataPtr[37].Size = (uint) ((TruncationLsn.Length + 1) * 2);
        long fileTime6 = CollectionTime.ToFileTime();
        eventDataPtr[38].DataPointer = (ulong) &fileTime6;
        eventDataPtr[38].Size = 8U;
        fixed (char* chPtr1 = ListenerName)
          fixed (char* chPtr2 = GroupName)
            fixed (char* chPtr3 = ReplicaServerName)
              fixed (char* chPtr4 = DatabaseName)
                fixed (char* chPtr5 = ConnectedStateDesc)
                  fixed (char* chPtr6 = AvailabilityModeDesc)
                    fixed (char* chPtr7 = SynchronizationStateDesc)
                      fixed (char* chPtr8 = ReplicaRoleDesc)
                        fixed (char* chPtr9 = SuspendReasonDesc)
                          fixed (char* chPtr10 = LastCommitLsn)
                            fixed (char* chPtr11 = LastHardenedLsn)
                              fixed (char* chPtr12 = LastReceivedLsn)
                                fixed (char* chPtr13 = LastSentLsn)
                                  fixed (char* chPtr14 = LastRedoneLsn)
                                    fixed (char* chPtr15 = EndOfLogLsn)
                                      fixed (char* chPtr16 = RecoveryLsn)
                                        fixed (char* chPtr17 = TruncationLsn)
                                        {
                                          eventDataPtr[1].DataPointer = (ulong) chPtr1;
                                          eventDataPtr[3].DataPointer = (ulong) chPtr2;
                                          eventDataPtr[5].DataPointer = (ulong) chPtr3;
                                          eventDataPtr[7].DataPointer = (ulong) chPtr4;
                                          eventDataPtr[8].DataPointer = (ulong) chPtr5;
                                          eventDataPtr[9].DataPointer = (ulong) chPtr6;
                                          eventDataPtr[10].DataPointer = (ulong) chPtr7;
                                          eventDataPtr[11].DataPointer = (ulong) chPtr8;
                                          eventDataPtr[15].DataPointer = (ulong) chPtr9;
                                          eventDataPtr[25].DataPointer = (ulong) chPtr10;
                                          eventDataPtr[27].DataPointer = (ulong) chPtr11;
                                          eventDataPtr[29].DataPointer = (ulong) chPtr12;
                                          eventDataPtr[31].DataPointer = (ulong) chPtr13;
                                          eventDataPtr[33].DataPointer = (ulong) chPtr14;
                                          eventDataPtr[35].DataPointer = (ulong) chPtr15;
                                          eventDataPtr[36].DataPointer = (ulong) chPtr16;
                                          eventDataPtr[37].DataPointer = (ulong) chPtr17;
                                          flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                                        }
      }
      return flag;
    }

    internal unsafe bool TemplateXEventData6(
      ref EventDescriptor eventDescriptor,
      DateTime EventTime,
      int SequenceNumber,
      Guid ActivityId,
      Guid UniqueIdentifier,
      Guid HostId,
      Guid VSID,
      byte Type,
      string ObjectName,
      string Actions,
      string Fields,
      string ServerName,
      string DatabaseName,
      string XEventTypeName)
    {
      int dataCount = 13;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        long fileTime = EventTime.ToFileTime();
        eventDataPtr->DataPointer = (ulong) &fileTime;
        eventDataPtr->Size = 8U;
        eventDataPtr[1].DataPointer = (ulong) &SequenceNumber;
        eventDataPtr[1].Size = 4U;
        eventDataPtr[2].DataPointer = (ulong) &ActivityId;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].DataPointer = (ulong) &UniqueIdentifier;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        eventDataPtr[4].DataPointer = (ulong) &HostId;
        eventDataPtr[4].Size = (uint) sizeof (Guid);
        eventDataPtr[5].DataPointer = (ulong) &VSID;
        eventDataPtr[5].Size = (uint) sizeof (Guid);
        eventDataPtr[6].DataPointer = (ulong) &Type;
        eventDataPtr[6].Size = 1U;
        eventDataPtr[7].Size = (uint) ((ObjectName.Length + 1) * 2);
        eventDataPtr[8].Size = (uint) ((Actions.Length + 1) * 2);
        eventDataPtr[9].Size = (uint) ((Fields.Length + 1) * 2);
        eventDataPtr[10].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[11].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[12].Size = (uint) ((XEventTypeName.Length + 1) * 2);
        fixed (char* chPtr1 = ObjectName)
          fixed (char* chPtr2 = Actions)
            fixed (char* chPtr3 = Fields)
              fixed (char* chPtr4 = ServerName)
                fixed (char* chPtr5 = DatabaseName)
                  fixed (char* chPtr6 = XEventTypeName)
                  {
                    eventDataPtr[7].DataPointer = (ulong) chPtr1;
                    eventDataPtr[8].DataPointer = (ulong) chPtr2;
                    eventDataPtr[9].DataPointer = (ulong) chPtr3;
                    eventDataPtr[10].DataPointer = (ulong) chPtr4;
                    eventDataPtr[11].DataPointer = (ulong) chPtr5;
                    eventDataPtr[12].DataPointer = (ulong) chPtr6;
                    flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                  }
      }
      return flag;
    }

    internal unsafe bool TemplateMemoryClerks3(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string ServerName,
      string DatabaseName,
      string ClerkName,
      long SizeKB,
      bool IsReadonly,
      string Listener)
    {
      int dataCount = 7;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((ClerkName.Length + 1) * 2);
        eventDataPtr[4].DataPointer = (ulong) &SizeKB;
        eventDataPtr[4].Size = 8U;
        int num = IsReadonly ? 1 : 0;
        eventDataPtr[5].DataPointer = (ulong) &num;
        eventDataPtr[5].Size = 4U;
        eventDataPtr[6].Size = (uint) ((Listener.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = ClerkName)
              fixed (char* chPtr4 = Listener)
              {
                eventDataPtr[1].DataPointer = (ulong) chPtr1;
                eventDataPtr[2].DataPointer = (ulong) chPtr2;
                eventDataPtr[3].DataPointer = (ulong) chPtr3;
                eventDataPtr[6].DataPointer = (ulong) chPtr4;
                flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
              }
      }
      return flag;
    }

    internal unsafe bool TemplateResourceSemaphores3(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string ServerName,
      string DatabaseName,
      int ResourceSemaphoreId,
      long TargetMemoryKB,
      long MaxTargetMemoryKB,
      long TotalMemoryKB,
      long AvailableMemoryKB,
      long GrantedMemoryKB,
      long UsedMemoryKB,
      int GranteeCount,
      int WaiterCount,
      long TimeoutErrorCount,
      long ForcedGrantCount,
      int PoolId,
      bool IsReadonly,
      string Listener)
    {
      int dataCount = 17;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[3].DataPointer = (ulong) &ResourceSemaphoreId;
        eventDataPtr[3].Size = 4U;
        eventDataPtr[4].DataPointer = (ulong) &TargetMemoryKB;
        eventDataPtr[4].Size = 8U;
        eventDataPtr[5].DataPointer = (ulong) &MaxTargetMemoryKB;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].DataPointer = (ulong) &TotalMemoryKB;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].DataPointer = (ulong) &AvailableMemoryKB;
        eventDataPtr[7].Size = 8U;
        eventDataPtr[8].DataPointer = (ulong) &GrantedMemoryKB;
        eventDataPtr[8].Size = 8U;
        eventDataPtr[9].DataPointer = (ulong) &UsedMemoryKB;
        eventDataPtr[9].Size = 8U;
        eventDataPtr[10].DataPointer = (ulong) &GranteeCount;
        eventDataPtr[10].Size = 4U;
        eventDataPtr[11].DataPointer = (ulong) &WaiterCount;
        eventDataPtr[11].Size = 4U;
        eventDataPtr[12].DataPointer = (ulong) &TimeoutErrorCount;
        eventDataPtr[12].Size = 8U;
        eventDataPtr[13].DataPointer = (ulong) &ForcedGrantCount;
        eventDataPtr[13].Size = 8U;
        eventDataPtr[14].DataPointer = (ulong) &PoolId;
        eventDataPtr[14].Size = 4U;
        int num = IsReadonly ? 1 : 0;
        eventDataPtr[15].DataPointer = (ulong) &num;
        eventDataPtr[15].Size = 4U;
        eventDataPtr[16].Size = (uint) ((Listener.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = Listener)
            {
              eventDataPtr[1].DataPointer = (ulong) chPtr1;
              eventDataPtr[2].DataPointer = (ulong) chPtr2;
              eventDataPtr[16].DataPointer = (ulong) chPtr3;
              flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
            }
      }
      return flag;
    }

    internal unsafe bool TemplateQueryOptimizerMemoryGateways3(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string ServerName,
      string DatabaseName,
      int PoolId,
      int MaxCount,
      int ActiveCount,
      int WaiterCount,
      long ThresholdFactor,
      long Threshold,
      bool IsActive,
      bool IsReadonly,
      string Listener)
    {
      int dataCount = 12;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[3].DataPointer = (ulong) &PoolId;
        eventDataPtr[3].Size = 4U;
        eventDataPtr[4].DataPointer = (ulong) &MaxCount;
        eventDataPtr[4].Size = 4U;
        eventDataPtr[5].DataPointer = (ulong) &ActiveCount;
        eventDataPtr[5].Size = 4U;
        eventDataPtr[6].DataPointer = (ulong) &WaiterCount;
        eventDataPtr[6].Size = 4U;
        eventDataPtr[7].DataPointer = (ulong) &ThresholdFactor;
        eventDataPtr[7].Size = 8U;
        eventDataPtr[8].DataPointer = (ulong) &Threshold;
        eventDataPtr[8].Size = 8U;
        int num1 = IsActive ? 1 : 0;
        eventDataPtr[9].DataPointer = (ulong) &num1;
        eventDataPtr[9].Size = 4U;
        int num2 = IsReadonly ? 1 : 0;
        eventDataPtr[10].DataPointer = (ulong) &num2;
        eventDataPtr[10].Size = 4U;
        eventDataPtr[11].Size = (uint) ((Listener.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = Listener)
            {
              eventDataPtr[1].DataPointer = (ulong) chPtr1;
              eventDataPtr[2].DataPointer = (ulong) chPtr2;
              eventDataPtr[11].DataPointer = (ulong) chPtr3;
              flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
            }
      }
      return flag;
    }

    internal unsafe bool TemplateSQLPerformanceCounters3(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string ServerName,
      string DatabaseName,
      string CounterName,
      long CounterValue,
      bool IsReadonly,
      string Listener)
    {
      int dataCount = 7;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((CounterName.Length + 1) * 2);
        eventDataPtr[4].DataPointer = (ulong) &CounterValue;
        eventDataPtr[4].Size = 8U;
        int num = IsReadonly ? 1 : 0;
        eventDataPtr[5].DataPointer = (ulong) &num;
        eventDataPtr[5].Size = 4U;
        eventDataPtr[6].Size = (uint) ((Listener.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = CounterName)
              fixed (char* chPtr4 = Listener)
              {
                eventDataPtr[1].DataPointer = (ulong) chPtr1;
                eventDataPtr[2].DataPointer = (ulong) chPtr2;
                eventDataPtr[3].DataPointer = (ulong) chPtr3;
                eventDataPtr[6].DataPointer = (ulong) chPtr4;
                flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
              }
      }
      return flag;
    }

    internal unsafe bool TemplateHttpOutgoingRequests6(
      ref EventDescriptor eventDescriptor,
      DateTime StartTime,
      int TimeTaken,
      string HttpClientName,
      string HttpMethod,
      string UrlHost,
      string UrlPath,
      int ResponseCode,
      string ErrorMessage,
      Guid E2EID,
      string AfdRefInfo,
      string RequestPriority,
      Guid CalledActivityId,
      string RequestPhase,
      string OrchestrationId,
      int TokenRetries,
      int HandlerStartTime,
      int BufferedRequestTime,
      int RequestSendTime,
      int ResponseContentTime,
      int GetTokenTime,
      int TrailingTime)
    {
      int dataCount = 21;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        long fileTime = StartTime.ToFileTime();
        eventDataPtr->DataPointer = (ulong) &fileTime;
        eventDataPtr->Size = 8U;
        eventDataPtr[1].DataPointer = (ulong) &TimeTaken;
        eventDataPtr[1].Size = 4U;
        eventDataPtr[2].Size = (uint) ((HttpClientName.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((HttpMethod.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((UrlHost.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((UrlPath.Length + 1) * 2);
        eventDataPtr[6].DataPointer = (ulong) &ResponseCode;
        eventDataPtr[6].Size = 4U;
        eventDataPtr[7].Size = (uint) ((ErrorMessage.Length + 1) * 2);
        eventDataPtr[8].DataPointer = (ulong) &E2EID;
        eventDataPtr[8].Size = (uint) sizeof (Guid);
        eventDataPtr[9].Size = (uint) ((AfdRefInfo.Length + 1) * 2);
        eventDataPtr[10].Size = (uint) ((RequestPriority.Length + 1) * 2);
        eventDataPtr[11].DataPointer = (ulong) &CalledActivityId;
        eventDataPtr[11].Size = (uint) sizeof (Guid);
        eventDataPtr[12].Size = (uint) ((RequestPhase.Length + 1) * 2);
        eventDataPtr[13].Size = (uint) ((OrchestrationId.Length + 1) * 2);
        eventDataPtr[14].DataPointer = (ulong) &TokenRetries;
        eventDataPtr[14].Size = 4U;
        eventDataPtr[15].DataPointer = (ulong) &HandlerStartTime;
        eventDataPtr[15].Size = 4U;
        eventDataPtr[16].DataPointer = (ulong) &BufferedRequestTime;
        eventDataPtr[16].Size = 4U;
        eventDataPtr[17].DataPointer = (ulong) &RequestSendTime;
        eventDataPtr[17].Size = 4U;
        eventDataPtr[18].DataPointer = (ulong) &ResponseContentTime;
        eventDataPtr[18].Size = 4U;
        eventDataPtr[19].DataPointer = (ulong) &GetTokenTime;
        eventDataPtr[19].Size = 4U;
        eventDataPtr[20].DataPointer = (ulong) &TrailingTime;
        eventDataPtr[20].Size = 4U;
        fixed (char* chPtr1 = HttpClientName)
          fixed (char* chPtr2 = HttpMethod)
            fixed (char* chPtr3 = UrlHost)
              fixed (char* chPtr4 = UrlPath)
                fixed (char* chPtr5 = ErrorMessage)
                  fixed (char* chPtr6 = AfdRefInfo)
                    fixed (char* chPtr7 = RequestPriority)
                      fixed (char* chPtr8 = RequestPhase)
                        fixed (char* chPtr9 = OrchestrationId)
                        {
                          eventDataPtr[2].DataPointer = (ulong) chPtr1;
                          eventDataPtr[3].DataPointer = (ulong) chPtr2;
                          eventDataPtr[4].DataPointer = (ulong) chPtr3;
                          eventDataPtr[5].DataPointer = (ulong) chPtr4;
                          eventDataPtr[7].DataPointer = (ulong) chPtr5;
                          eventDataPtr[9].DataPointer = (ulong) chPtr6;
                          eventDataPtr[10].DataPointer = (ulong) chPtr7;
                          eventDataPtr[12].DataPointer = (ulong) chPtr8;
                          eventDataPtr[13].DataPointer = (ulong) chPtr9;
                          flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                        }
      }
      return flag;
    }

    internal unsafe bool TemplatePackagingTraces3(
      ref EventDescriptor eventDescriptor,
      Guid ActivityId,
      string CollectionHostName,
      Guid HostId,
      Guid ProjectId,
      string Protocol,
      string Command,
      string FeedId,
      string FeedName,
      string ViewId,
      string ViewName,
      string PackageName,
      string PackageVersion,
      int ResponseCode,
      bool IsSlow,
      long TimeToFirstPageInMs,
      long ExecutionTimeInMs,
      long QueueTimeInMs,
      bool IsFailed,
      string ExceptionType,
      string ExceptionMessage,
      string IdentityName,
      string UserAgent,
      string ClientSessionId,
      string RefererHeader,
      string SourceIp,
      string HostIp,
      string BuildNumber,
      string CommitId,
      string DataCurrentVersion,
      string DataDestinationVersion,
      string DataMigrationState,
      string FeatureFlagsOn,
      string FeatureFlagsOff,
      string StackTrace,
      string TimingsTrace,
      string Uri,
      string HttpMethod,
      Guid RelatedActivityId,
      Guid E2EID,
      string Properties,
      string PackageStorageId,
      string ProjectVisibility,
      string OrchestrationId)
    {
      int dataCount = 43;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ActivityId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((CollectionHostName.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &HostId;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].DataPointer = (ulong) &ProjectId;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        eventDataPtr[4].Size = (uint) ((Protocol.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((Command.Length + 1) * 2);
        eventDataPtr[6].Size = (uint) ((FeedId.Length + 1) * 2);
        eventDataPtr[7].Size = (uint) ((FeedName.Length + 1) * 2);
        eventDataPtr[8].Size = (uint) ((ViewId.Length + 1) * 2);
        eventDataPtr[9].Size = (uint) ((ViewName.Length + 1) * 2);
        eventDataPtr[10].Size = (uint) ((PackageName.Length + 1) * 2);
        eventDataPtr[11].Size = (uint) ((PackageVersion.Length + 1) * 2);
        eventDataPtr[12].DataPointer = (ulong) &ResponseCode;
        eventDataPtr[12].Size = 4U;
        int num1 = IsSlow ? 1 : 0;
        eventDataPtr[13].DataPointer = (ulong) &num1;
        eventDataPtr[13].Size = 4U;
        eventDataPtr[14].DataPointer = (ulong) &TimeToFirstPageInMs;
        eventDataPtr[14].Size = 8U;
        eventDataPtr[15].DataPointer = (ulong) &ExecutionTimeInMs;
        eventDataPtr[15].Size = 8U;
        eventDataPtr[16].DataPointer = (ulong) &QueueTimeInMs;
        eventDataPtr[16].Size = 8U;
        int num2 = IsFailed ? 1 : 0;
        eventDataPtr[17].DataPointer = (ulong) &num2;
        eventDataPtr[17].Size = 4U;
        eventDataPtr[18].Size = (uint) ((ExceptionType.Length + 1) * 2);
        eventDataPtr[19].Size = (uint) ((ExceptionMessage.Length + 1) * 2);
        eventDataPtr[20].Size = (uint) ((IdentityName.Length + 1) * 2);
        eventDataPtr[21].Size = (uint) ((UserAgent.Length + 1) * 2);
        eventDataPtr[22].Size = (uint) ((ClientSessionId.Length + 1) * 2);
        eventDataPtr[23].Size = (uint) ((RefererHeader.Length + 1) * 2);
        eventDataPtr[24].Size = (uint) ((SourceIp.Length + 1) * 2);
        eventDataPtr[25].Size = (uint) ((HostIp.Length + 1) * 2);
        eventDataPtr[26].Size = (uint) ((BuildNumber.Length + 1) * 2);
        eventDataPtr[27].Size = (uint) ((CommitId.Length + 1) * 2);
        eventDataPtr[28].Size = (uint) ((DataCurrentVersion.Length + 1) * 2);
        eventDataPtr[29].Size = (uint) ((DataDestinationVersion.Length + 1) * 2);
        eventDataPtr[30].Size = (uint) ((DataMigrationState.Length + 1) * 2);
        eventDataPtr[31].Size = (uint) ((FeatureFlagsOn.Length + 1) * 2);
        eventDataPtr[32].Size = (uint) ((FeatureFlagsOff.Length + 1) * 2);
        eventDataPtr[33].Size = (uint) ((StackTrace.Length + 1) * 2);
        eventDataPtr[34].Size = (uint) ((TimingsTrace.Length + 1) * 2);
        eventDataPtr[35].Size = (uint) ((Uri.Length + 1) * 2);
        eventDataPtr[36].Size = (uint) ((HttpMethod.Length + 1) * 2);
        eventDataPtr[37].DataPointer = (ulong) &RelatedActivityId;
        eventDataPtr[37].Size = (uint) sizeof (Guid);
        eventDataPtr[38].DataPointer = (ulong) &E2EID;
        eventDataPtr[38].Size = (uint) sizeof (Guid);
        eventDataPtr[39].Size = (uint) ((Properties.Length + 1) * 2);
        eventDataPtr[40].Size = (uint) ((PackageStorageId.Length + 1) * 2);
        eventDataPtr[41].Size = (uint) ((ProjectVisibility.Length + 1) * 2);
        eventDataPtr[42].Size = (uint) ((OrchestrationId.Length + 1) * 2);
        fixed (char* chPtr1 = CollectionHostName)
          fixed (char* chPtr2 = Protocol)
            fixed (char* chPtr3 = Command)
              fixed (char* chPtr4 = FeedId)
                fixed (char* chPtr5 = FeedName)
                  fixed (char* chPtr6 = ViewId)
                    fixed (char* chPtr7 = ViewName)
                      fixed (char* chPtr8 = PackageName)
                        fixed (char* chPtr9 = PackageVersion)
                          fixed (char* chPtr10 = ExceptionType)
                            fixed (char* chPtr11 = ExceptionMessage)
                              fixed (char* chPtr12 = IdentityName)
                                fixed (char* chPtr13 = UserAgent)
                                  fixed (char* chPtr14 = ClientSessionId)
                                    fixed (char* chPtr15 = RefererHeader)
                                      fixed (char* chPtr16 = SourceIp)
                                        fixed (char* chPtr17 = HostIp)
                                          fixed (char* chPtr18 = BuildNumber)
                                            fixed (char* chPtr19 = CommitId)
                                              fixed (char* chPtr20 = DataCurrentVersion)
                                                fixed (char* chPtr21 = DataDestinationVersion)
                                                  fixed (char* chPtr22 = DataMigrationState)
                                                    fixed (char* chPtr23 = FeatureFlagsOn)
                                                      fixed (char* chPtr24 = FeatureFlagsOff)
                                                        fixed (char* chPtr25 = StackTrace)
                                                          fixed (char* chPtr26 = TimingsTrace)
                                                            fixed (char* chPtr27 = Uri)
                                                              fixed (char* chPtr28 = HttpMethod)
                                                                fixed (char* chPtr29 = Properties)
                                                                  fixed (char* chPtr30 = PackageStorageId)
                                                                    fixed (char* chPtr31 = ProjectVisibility)
                                                                      fixed (char* chPtr32 = OrchestrationId)
                                                                      {
                                                                        eventDataPtr[1].DataPointer = (ulong) chPtr1;
                                                                        eventDataPtr[4].DataPointer = (ulong) chPtr2;
                                                                        eventDataPtr[5].DataPointer = (ulong) chPtr3;
                                                                        eventDataPtr[6].DataPointer = (ulong) chPtr4;
                                                                        eventDataPtr[7].DataPointer = (ulong) chPtr5;
                                                                        eventDataPtr[8].DataPointer = (ulong) chPtr6;
                                                                        eventDataPtr[9].DataPointer = (ulong) chPtr7;
                                                                        eventDataPtr[10].DataPointer = (ulong) chPtr8;
                                                                        eventDataPtr[11].DataPointer = (ulong) chPtr9;
                                                                        eventDataPtr[18].DataPointer = (ulong) chPtr10;
                                                                        eventDataPtr[19].DataPointer = (ulong) chPtr11;
                                                                        eventDataPtr[20].DataPointer = (ulong) chPtr12;
                                                                        eventDataPtr[21].DataPointer = (ulong) chPtr13;
                                                                        eventDataPtr[22].DataPointer = (ulong) chPtr14;
                                                                        eventDataPtr[23].DataPointer = (ulong) chPtr15;
                                                                        eventDataPtr[24].DataPointer = (ulong) chPtr16;
                                                                        eventDataPtr[25].DataPointer = (ulong) chPtr17;
                                                                        eventDataPtr[26].DataPointer = (ulong) chPtr18;
                                                                        eventDataPtr[27].DataPointer = (ulong) chPtr19;
                                                                        eventDataPtr[28].DataPointer = (ulong) chPtr20;
                                                                        eventDataPtr[29].DataPointer = (ulong) chPtr21;
                                                                        eventDataPtr[30].DataPointer = (ulong) chPtr22;
                                                                        eventDataPtr[31].DataPointer = (ulong) chPtr23;
                                                                        eventDataPtr[32].DataPointer = (ulong) chPtr24;
                                                                        eventDataPtr[33].DataPointer = (ulong) chPtr25;
                                                                        eventDataPtr[34].DataPointer = (ulong) chPtr26;
                                                                        eventDataPtr[35].DataPointer = (ulong) chPtr27;
                                                                        eventDataPtr[36].DataPointer = (ulong) chPtr28;
                                                                        eventDataPtr[39].DataPointer = (ulong) chPtr29;
                                                                        eventDataPtr[40].DataPointer = (ulong) chPtr30;
                                                                        eventDataPtr[41].DataPointer = (ulong) chPtr31;
                                                                        eventDataPtr[42].DataPointer = (ulong) chPtr32;
                                                                        flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                                                                      }
      }
      return flag;
    }

    internal unsafe bool TemplateGeoReplicationLinkStatus2(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string ServerName,
      string DatabaseName,
      Guid LinkGuid,
      string PartnerServer,
      string PartnerDatabase,
      DateTime LastReplication,
      int ReplicationLagSec,
      int ReplicationState,
      string ReplicationStateDescription,
      byte Role,
      string RoleDescription,
      byte SecondaryAllowConnections,
      string SecondaryAllowConnectionsDescription,
      DateTime LastCommit,
      string Listener)
    {
      int dataCount = 16;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[3].DataPointer = (ulong) &LinkGuid;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        eventDataPtr[4].Size = (uint) ((PartnerServer.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((PartnerDatabase.Length + 1) * 2);
        long fileTime1 = LastReplication.ToFileTime();
        eventDataPtr[6].DataPointer = (ulong) &fileTime1;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].DataPointer = (ulong) &ReplicationLagSec;
        eventDataPtr[7].Size = 4U;
        eventDataPtr[8].DataPointer = (ulong) &ReplicationState;
        eventDataPtr[8].Size = 4U;
        eventDataPtr[9].Size = (uint) ((ReplicationStateDescription.Length + 1) * 2);
        eventDataPtr[10].DataPointer = (ulong) &Role;
        eventDataPtr[10].Size = 1U;
        eventDataPtr[11].Size = (uint) ((RoleDescription.Length + 1) * 2);
        eventDataPtr[12].DataPointer = (ulong) &SecondaryAllowConnections;
        eventDataPtr[12].Size = 1U;
        eventDataPtr[13].Size = (uint) ((SecondaryAllowConnectionsDescription.Length + 1) * 2);
        long fileTime2 = LastCommit.ToFileTime();
        eventDataPtr[14].DataPointer = (ulong) &fileTime2;
        eventDataPtr[14].Size = 8U;
        eventDataPtr[15].Size = (uint) ((Listener.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = PartnerServer)
              fixed (char* chPtr4 = PartnerDatabase)
                fixed (char* chPtr5 = ReplicationStateDescription)
                  fixed (char* chPtr6 = RoleDescription)
                    fixed (char* chPtr7 = SecondaryAllowConnectionsDescription)
                      fixed (char* chPtr8 = Listener)
                      {
                        eventDataPtr[1].DataPointer = (ulong) chPtr1;
                        eventDataPtr[2].DataPointer = (ulong) chPtr2;
                        eventDataPtr[4].DataPointer = (ulong) chPtr3;
                        eventDataPtr[5].DataPointer = (ulong) chPtr4;
                        eventDataPtr[9].DataPointer = (ulong) chPtr5;
                        eventDataPtr[11].DataPointer = (ulong) chPtr6;
                        eventDataPtr[13].DataPointer = (ulong) chPtr7;
                        eventDataPtr[15].DataPointer = (ulong) chPtr8;
                        flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                      }
      }
      return flag;
    }

    internal unsafe bool TemplateXEventDataRPCCompleted5(
      ref EventDescriptor eventDescriptor,
      DateTime EventTime,
      int SequenceNumber,
      Guid ActivityId,
      Guid UniqueIdentifier,
      Guid HostId,
      Guid VSID,
      byte Type,
      string ObjectName,
      ulong CpuTime,
      ulong Duration,
      ulong PhysicalReads,
      ulong LogicalReads,
      ulong Writes,
      string Result,
      ulong RowCount,
      string ConnectionResetOption,
      string Statement,
      double TSTUs,
      string ServerName,
      string DatabaseName,
      bool IsGoverned,
      bool IsReadScaleOut)
    {
      int dataCount = 22;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        long fileTime = EventTime.ToFileTime();
        eventDataPtr->DataPointer = (ulong) &fileTime;
        eventDataPtr->Size = 8U;
        eventDataPtr[1].DataPointer = (ulong) &SequenceNumber;
        eventDataPtr[1].Size = 4U;
        eventDataPtr[2].DataPointer = (ulong) &ActivityId;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].DataPointer = (ulong) &UniqueIdentifier;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        eventDataPtr[4].DataPointer = (ulong) &HostId;
        eventDataPtr[4].Size = (uint) sizeof (Guid);
        eventDataPtr[5].DataPointer = (ulong) &VSID;
        eventDataPtr[5].Size = (uint) sizeof (Guid);
        eventDataPtr[6].DataPointer = (ulong) &Type;
        eventDataPtr[6].Size = 1U;
        eventDataPtr[7].Size = (uint) ((ObjectName.Length + 1) * 2);
        eventDataPtr[8].DataPointer = (ulong) &CpuTime;
        eventDataPtr[8].Size = 8U;
        eventDataPtr[9].DataPointer = (ulong) &Duration;
        eventDataPtr[9].Size = 8U;
        eventDataPtr[10].DataPointer = (ulong) &PhysicalReads;
        eventDataPtr[10].Size = 8U;
        eventDataPtr[11].DataPointer = (ulong) &LogicalReads;
        eventDataPtr[11].Size = 8U;
        eventDataPtr[12].DataPointer = (ulong) &Writes;
        eventDataPtr[12].Size = 8U;
        eventDataPtr[13].Size = (uint) ((Result.Length + 1) * 2);
        eventDataPtr[14].DataPointer = (ulong) &RowCount;
        eventDataPtr[14].Size = 8U;
        eventDataPtr[15].Size = (uint) ((ConnectionResetOption.Length + 1) * 2);
        eventDataPtr[16].Size = (uint) ((Statement.Length + 1) * 2);
        eventDataPtr[17].DataPointer = (ulong) &TSTUs;
        eventDataPtr[17].Size = 8U;
        eventDataPtr[18].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[19].Size = (uint) ((DatabaseName.Length + 1) * 2);
        int num1 = IsGoverned ? 1 : 0;
        eventDataPtr[20].DataPointer = (ulong) &num1;
        eventDataPtr[20].Size = 4U;
        int num2 = IsReadScaleOut ? 1 : 0;
        eventDataPtr[21].DataPointer = (ulong) &num2;
        eventDataPtr[21].Size = 4U;
        fixed (char* chPtr1 = ObjectName)
          fixed (char* chPtr2 = Result)
            fixed (char* chPtr3 = ConnectionResetOption)
              fixed (char* chPtr4 = Statement)
                fixed (char* chPtr5 = ServerName)
                  fixed (char* chPtr6 = DatabaseName)
                  {
                    eventDataPtr[7].DataPointer = (ulong) chPtr1;
                    eventDataPtr[13].DataPointer = (ulong) chPtr2;
                    eventDataPtr[15].DataPointer = (ulong) chPtr3;
                    eventDataPtr[16].DataPointer = (ulong) chPtr4;
                    eventDataPtr[18].DataPointer = (ulong) chPtr5;
                    eventDataPtr[19].DataPointer = (ulong) chPtr6;
                    flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                  }
      }
      return flag;
    }

    internal unsafe bool TemplateJobAgentJobStarted2(
      ref EventDescriptor eventDescriptor,
      string Plugin,
      string JobName,
      Guid JobSource,
      Guid JobId,
      DateTime QueueTime,
      DateTime StartTime,
      Guid AgentId,
      int QueuedReasons,
      int QueueFlags,
      short Priority,
      string Feature,
      Guid E2EID,
      Guid RequesterActivityId,
      Guid RequesterVsid)
    {
      int dataCount = 14;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((Plugin.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((JobName.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &JobSource;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].DataPointer = (ulong) &JobId;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        long fileTime1 = QueueTime.ToFileTime();
        eventDataPtr[4].DataPointer = (ulong) &fileTime1;
        eventDataPtr[4].Size = 8U;
        long fileTime2 = StartTime.ToFileTime();
        eventDataPtr[5].DataPointer = (ulong) &fileTime2;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].DataPointer = (ulong) &AgentId;
        eventDataPtr[6].Size = (uint) sizeof (Guid);
        eventDataPtr[7].DataPointer = (ulong) &QueuedReasons;
        eventDataPtr[7].Size = 4U;
        eventDataPtr[8].DataPointer = (ulong) &QueueFlags;
        eventDataPtr[8].Size = 4U;
        eventDataPtr[9].DataPointer = (ulong) &Priority;
        eventDataPtr[9].Size = 2U;
        eventDataPtr[10].Size = (uint) ((Feature.Length + 1) * 2);
        eventDataPtr[11].DataPointer = (ulong) &E2EID;
        eventDataPtr[11].Size = (uint) sizeof (Guid);
        eventDataPtr[12].DataPointer = (ulong) &RequesterActivityId;
        eventDataPtr[12].Size = (uint) sizeof (Guid);
        eventDataPtr[13].DataPointer = (ulong) &RequesterVsid;
        eventDataPtr[13].Size = (uint) sizeof (Guid);
        fixed (char* chPtr1 = Plugin)
          fixed (char* chPtr2 = JobName)
            fixed (char* chPtr3 = Feature)
            {
              eventDataPtr->DataPointer = (ulong) chPtr1;
              eventDataPtr[1].DataPointer = (ulong) chPtr2;
              eventDataPtr[10].DataPointer = (ulong) chPtr3;
              flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
            }
      }
      return flag;
    }

    internal unsafe bool TemplateCloudServiceRoleDetails10(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      Guid AzureSubscriptionId,
      Guid AzureSubscriptionAadTenantId,
      string RoleType,
      int RoleCountMin,
      int RoleCount,
      int RoleCountMax,
      string RoleSize,
      int RoleCores,
      long RoleMemoryMB,
      string HostedServiceDnsName,
      string BuildNumber,
      string OSImageVersion,
      string DeploymentOsVersion,
      string DeploymentHotfixes,
      bool EncryptionAtHost,
      string SecurityType,
      bool SecureBootEnabled,
      bool VTpmEnabled,
      string OSDiskStorageAccountType,
      int OSDiskSizeInGB,
      string DeploymentRing,
      int WeekdayPeakRoleCountMin,
      int WeekdayPeakRoleCountMax,
      string WeekdayPeakStartTime,
      string WeekdayPeakEndTime,
      int WeekendPeakRoleCountMin,
      int WeekendPeakRoleCountMax,
      string WeekendPeakStartTime,
      string WeekendPeakEndTime,
      string Zones)
    {
      int dataCount = 31;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].DataPointer = (ulong) &AzureSubscriptionId;
        eventDataPtr[1].Size = (uint) sizeof (Guid);
        eventDataPtr[2].DataPointer = (ulong) &AzureSubscriptionAadTenantId;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].Size = (uint) ((RoleType.Length + 1) * 2);
        eventDataPtr[4].DataPointer = (ulong) &RoleCountMin;
        eventDataPtr[4].Size = 4U;
        eventDataPtr[5].DataPointer = (ulong) &RoleCount;
        eventDataPtr[5].Size = 4U;
        eventDataPtr[6].DataPointer = (ulong) &RoleCountMax;
        eventDataPtr[6].Size = 4U;
        eventDataPtr[7].Size = (uint) ((RoleSize.Length + 1) * 2);
        eventDataPtr[8].DataPointer = (ulong) &RoleCores;
        eventDataPtr[8].Size = 4U;
        eventDataPtr[9].DataPointer = (ulong) &RoleMemoryMB;
        eventDataPtr[9].Size = 8U;
        eventDataPtr[10].Size = (uint) ((HostedServiceDnsName.Length + 1) * 2);
        eventDataPtr[11].Size = (uint) ((BuildNumber.Length + 1) * 2);
        eventDataPtr[12].Size = (uint) ((OSImageVersion.Length + 1) * 2);
        eventDataPtr[13].Size = (uint) ((DeploymentOsVersion.Length + 1) * 2);
        eventDataPtr[14].Size = (uint) ((DeploymentHotfixes.Length + 1) * 2);
        int num1 = EncryptionAtHost ? 1 : 0;
        eventDataPtr[15].DataPointer = (ulong) &num1;
        eventDataPtr[15].Size = 4U;
        eventDataPtr[16].Size = (uint) ((SecurityType.Length + 1) * 2);
        int num2 = SecureBootEnabled ? 1 : 0;
        eventDataPtr[17].DataPointer = (ulong) &num2;
        eventDataPtr[17].Size = 4U;
        int num3 = VTpmEnabled ? 1 : 0;
        eventDataPtr[18].DataPointer = (ulong) &num3;
        eventDataPtr[18].Size = 4U;
        eventDataPtr[19].Size = (uint) ((OSDiskStorageAccountType.Length + 1) * 2);
        eventDataPtr[20].DataPointer = (ulong) &OSDiskSizeInGB;
        eventDataPtr[20].Size = 4U;
        eventDataPtr[21].Size = (uint) ((DeploymentRing.Length + 1) * 2);
        eventDataPtr[22].DataPointer = (ulong) &WeekdayPeakRoleCountMin;
        eventDataPtr[22].Size = 4U;
        eventDataPtr[23].DataPointer = (ulong) &WeekdayPeakRoleCountMax;
        eventDataPtr[23].Size = 4U;
        eventDataPtr[24].Size = (uint) ((WeekdayPeakStartTime.Length + 1) * 2);
        eventDataPtr[25].Size = (uint) ((WeekdayPeakEndTime.Length + 1) * 2);
        eventDataPtr[26].DataPointer = (ulong) &WeekendPeakRoleCountMin;
        eventDataPtr[26].Size = 4U;
        eventDataPtr[27].DataPointer = (ulong) &WeekendPeakRoleCountMax;
        eventDataPtr[27].Size = 4U;
        eventDataPtr[28].Size = (uint) ((WeekendPeakStartTime.Length + 1) * 2);
        eventDataPtr[29].Size = (uint) ((WeekendPeakEndTime.Length + 1) * 2);
        eventDataPtr[30].Size = (uint) ((Zones.Length + 1) * 2);
        fixed (char* chPtr1 = RoleType)
          fixed (char* chPtr2 = RoleSize)
            fixed (char* chPtr3 = HostedServiceDnsName)
              fixed (char* chPtr4 = BuildNumber)
                fixed (char* chPtr5 = OSImageVersion)
                  fixed (char* chPtr6 = DeploymentOsVersion)
                    fixed (char* chPtr7 = DeploymentHotfixes)
                      fixed (char* chPtr8 = SecurityType)
                        fixed (char* chPtr9 = OSDiskStorageAccountType)
                          fixed (char* chPtr10 = DeploymentRing)
                            fixed (char* chPtr11 = WeekdayPeakStartTime)
                              fixed (char* chPtr12 = WeekdayPeakEndTime)
                                fixed (char* chPtr13 = WeekendPeakStartTime)
                                  fixed (char* chPtr14 = WeekendPeakEndTime)
                                    fixed (char* chPtr15 = Zones)
                                    {
                                      eventDataPtr[3].DataPointer = (ulong) chPtr1;
                                      eventDataPtr[7].DataPointer = (ulong) chPtr2;
                                      eventDataPtr[10].DataPointer = (ulong) chPtr3;
                                      eventDataPtr[11].DataPointer = (ulong) chPtr4;
                                      eventDataPtr[12].DataPointer = (ulong) chPtr5;
                                      eventDataPtr[13].DataPointer = (ulong) chPtr6;
                                      eventDataPtr[14].DataPointer = (ulong) chPtr7;
                                      eventDataPtr[16].DataPointer = (ulong) chPtr8;
                                      eventDataPtr[19].DataPointer = (ulong) chPtr9;
                                      eventDataPtr[21].DataPointer = (ulong) chPtr10;
                                      eventDataPtr[24].DataPointer = (ulong) chPtr11;
                                      eventDataPtr[25].DataPointer = (ulong) chPtr12;
                                      eventDataPtr[28].DataPointer = (ulong) chPtr13;
                                      eventDataPtr[29].DataPointer = (ulong) chPtr14;
                                      eventDataPtr[30].DataPointer = (ulong) chPtr15;
                                      flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                                    }
      }
      return flag;
    }

    internal unsafe bool TemplateSurveyEvent2(
      ref EventDescriptor eventDescriptor,
      Guid UniqueIdentifier,
      string AnonymousIdentifier,
      Guid TenantId,
      Guid HostId,
      Guid ParentHostId,
      byte HostType,
      Guid VSID,
      Guid CUID,
      string Area,
      string Feature,
      string UserAgent,
      string Properties,
      string DataspaceType,
      string DataspaceId,
      string DataspaceVisibility,
      byte SupportsPublicAccess)
    {
      int dataCount = 16;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &UniqueIdentifier;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((AnonymousIdentifier.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &TenantId;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].DataPointer = (ulong) &HostId;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        eventDataPtr[4].DataPointer = (ulong) &ParentHostId;
        eventDataPtr[4].Size = (uint) sizeof (Guid);
        eventDataPtr[5].DataPointer = (ulong) &HostType;
        eventDataPtr[5].Size = 1U;
        eventDataPtr[6].DataPointer = (ulong) &VSID;
        eventDataPtr[6].Size = (uint) sizeof (Guid);
        eventDataPtr[7].DataPointer = (ulong) &CUID;
        eventDataPtr[7].Size = (uint) sizeof (Guid);
        eventDataPtr[8].Size = (uint) ((Area.Length + 1) * 2);
        eventDataPtr[9].Size = (uint) ((Feature.Length + 1) * 2);
        eventDataPtr[10].Size = (uint) ((UserAgent.Length + 1) * 2);
        eventDataPtr[11].Size = (uint) ((Properties.Length + 1) * 2);
        eventDataPtr[12].Size = (uint) ((DataspaceType.Length + 1) * 2);
        eventDataPtr[13].Size = (uint) ((DataspaceId.Length + 1) * 2);
        eventDataPtr[14].Size = (uint) ((DataspaceVisibility.Length + 1) * 2);
        eventDataPtr[15].DataPointer = (ulong) &SupportsPublicAccess;
        eventDataPtr[15].Size = 1U;
        fixed (char* chPtr1 = AnonymousIdentifier)
          fixed (char* chPtr2 = Area)
            fixed (char* chPtr3 = Feature)
              fixed (char* chPtr4 = UserAgent)
                fixed (char* chPtr5 = Properties)
                  fixed (char* chPtr6 = DataspaceType)
                    fixed (char* chPtr7 = DataspaceId)
                      fixed (char* chPtr8 = DataspaceVisibility)
                      {
                        eventDataPtr[1].DataPointer = (ulong) chPtr1;
                        eventDataPtr[8].DataPointer = (ulong) chPtr2;
                        eventDataPtr[9].DataPointer = (ulong) chPtr3;
                        eventDataPtr[10].DataPointer = (ulong) chPtr4;
                        eventDataPtr[11].DataPointer = (ulong) chPtr5;
                        eventDataPtr[12].DataPointer = (ulong) chPtr6;
                        eventDataPtr[13].DataPointer = (ulong) chPtr7;
                        eventDataPtr[14].DataPointer = (ulong) chPtr8;
                        flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                      }
      }
      return flag;
    }

    internal unsafe bool TemplateTuningRecommendation2(
      ref EventDescriptor eventDescriptor,
      long RunId,
      string ServerName,
      string DatabaseName,
      string Name,
      string Type,
      string Reason,
      DateTime ValidSince,
      DateTime LastRefresh,
      string State,
      bool IsExecutableAction,
      bool IsRevertableAction,
      DateTime ExecuteActionStartTime,
      ulong ExecuteActionDurationMilliseconds,
      string ExecuteActionInitiatedBy,
      DateTime ExecuteActionInitiatedTime,
      DateTime RevertActionStartTime,
      ulong RevertActionDurationMilliseconds,
      string RevertActionInitiatedBy,
      DateTime RevertActionInitiatedTime,
      int Score,
      string Details,
      string Listener)
    {
      int dataCount = 22;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &RunId;
        eventDataPtr->Size = 8U;
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((Name.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((Type.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((Reason.Length + 1) * 2);
        long fileTime1 = ValidSince.ToFileTime();
        eventDataPtr[6].DataPointer = (ulong) &fileTime1;
        eventDataPtr[6].Size = 8U;
        long fileTime2 = LastRefresh.ToFileTime();
        eventDataPtr[7].DataPointer = (ulong) &fileTime2;
        eventDataPtr[7].Size = 8U;
        eventDataPtr[8].Size = (uint) ((State.Length + 1) * 2);
        int num1 = IsExecutableAction ? 1 : 0;
        eventDataPtr[9].DataPointer = (ulong) &num1;
        eventDataPtr[9].Size = 4U;
        int num2 = IsRevertableAction ? 1 : 0;
        eventDataPtr[10].DataPointer = (ulong) &num2;
        eventDataPtr[10].Size = 4U;
        long fileTime3 = ExecuteActionStartTime.ToFileTime();
        eventDataPtr[11].DataPointer = (ulong) &fileTime3;
        eventDataPtr[11].Size = 8U;
        eventDataPtr[12].DataPointer = (ulong) &ExecuteActionDurationMilliseconds;
        eventDataPtr[12].Size = 8U;
        eventDataPtr[13].Size = (uint) ((ExecuteActionInitiatedBy.Length + 1) * 2);
        long fileTime4 = ExecuteActionInitiatedTime.ToFileTime();
        eventDataPtr[14].DataPointer = (ulong) &fileTime4;
        eventDataPtr[14].Size = 8U;
        long fileTime5 = RevertActionStartTime.ToFileTime();
        eventDataPtr[15].DataPointer = (ulong) &fileTime5;
        eventDataPtr[15].Size = 8U;
        eventDataPtr[16].DataPointer = (ulong) &RevertActionDurationMilliseconds;
        eventDataPtr[16].Size = 8U;
        eventDataPtr[17].Size = (uint) ((RevertActionInitiatedBy.Length + 1) * 2);
        long fileTime6 = RevertActionInitiatedTime.ToFileTime();
        eventDataPtr[18].DataPointer = (ulong) &fileTime6;
        eventDataPtr[18].Size = 8U;
        eventDataPtr[19].DataPointer = (ulong) &Score;
        eventDataPtr[19].Size = 4U;
        eventDataPtr[20].Size = (uint) ((Details.Length + 1) * 2);
        eventDataPtr[21].Size = (uint) ((Listener.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = Name)
              fixed (char* chPtr4 = Type)
                fixed (char* chPtr5 = Reason)
                  fixed (char* chPtr6 = State)
                    fixed (char* chPtr7 = ExecuteActionInitiatedBy)
                      fixed (char* chPtr8 = RevertActionInitiatedBy)
                        fixed (char* chPtr9 = Details)
                          fixed (char* chPtr10 = Listener)
                          {
                            eventDataPtr[1].DataPointer = (ulong) chPtr1;
                            eventDataPtr[2].DataPointer = (ulong) chPtr2;
                            eventDataPtr[3].DataPointer = (ulong) chPtr3;
                            eventDataPtr[4].DataPointer = (ulong) chPtr4;
                            eventDataPtr[5].DataPointer = (ulong) chPtr5;
                            eventDataPtr[8].DataPointer = (ulong) chPtr6;
                            eventDataPtr[13].DataPointer = (ulong) chPtr7;
                            eventDataPtr[17].DataPointer = (ulong) chPtr8;
                            eventDataPtr[20].DataPointer = (ulong) chPtr9;
                            eventDataPtr[21].DataPointer = (ulong) chPtr10;
                            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                          }
      }
      return flag;
    }

    internal unsafe bool TemplateServiceBusActivity4(
      ref EventDescriptor eventDescriptor,
      Guid HostId,
      byte HostType,
      string TopicName,
      string Plugin,
      Guid SourceInstanceId,
      Guid SourceInstanceType,
      bool Status,
      string ExceptionType,
      string ExceptionMessage,
      DateTime StartTime,
      int LogicalReads,
      int PhysicalReads,
      Guid ActivityId,
      Guid E2EID,
      Guid UniqueIdentifier,
      int CPUTime,
      int ElapsedTime,
      long CPUCycles,
      int SqlExecutionTime,
      int SqlExecutionCount,
      int RedisExecutionTime,
      int RedisExecutionCount,
      int AadGraphExecutionTime,
      int AadGraphExecutionCount,
      int AadTokenExecutionTime,
      int AadTokenExecutionCount,
      int BlobStorageExecutionTime,
      int BlobStorageExecutionCount,
      int TableStorageExecutionTime,
      int TableStorageExecutionCount,
      int ServiceBusExecutionTime,
      int ServiceBusExecutionCount,
      int VssClientExecutionTime,
      int VssClientExecutionCount,
      int SqlRetryExecutionTime,
      int SqlRetryExecutionCount,
      int SqlReadOnlyExecutionTime,
      int SqlReadOnlyExecutionCount,
      int FinalSqlCommandExecutionTime,
      string MessageId,
      string Namespace,
      string ContentType)
    {
      int dataCount = 42;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &HostId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].DataPointer = (ulong) &HostType;
        eventDataPtr[1].Size = 1U;
        eventDataPtr[2].Size = (uint) ((TopicName.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((Plugin.Length + 1) * 2);
        eventDataPtr[4].DataPointer = (ulong) &SourceInstanceId;
        eventDataPtr[4].Size = (uint) sizeof (Guid);
        eventDataPtr[5].DataPointer = (ulong) &SourceInstanceType;
        eventDataPtr[5].Size = (uint) sizeof (Guid);
        int num = Status ? 1 : 0;
        eventDataPtr[6].DataPointer = (ulong) &num;
        eventDataPtr[6].Size = 4U;
        eventDataPtr[7].Size = (uint) ((ExceptionType.Length + 1) * 2);
        eventDataPtr[8].Size = (uint) ((ExceptionMessage.Length + 1) * 2);
        long fileTime = StartTime.ToFileTime();
        eventDataPtr[9].DataPointer = (ulong) &fileTime;
        eventDataPtr[9].Size = 8U;
        eventDataPtr[10].DataPointer = (ulong) &LogicalReads;
        eventDataPtr[10].Size = 4U;
        eventDataPtr[11].DataPointer = (ulong) &PhysicalReads;
        eventDataPtr[11].Size = 4U;
        eventDataPtr[12].DataPointer = (ulong) &ActivityId;
        eventDataPtr[12].Size = (uint) sizeof (Guid);
        eventDataPtr[13].DataPointer = (ulong) &E2EID;
        eventDataPtr[13].Size = (uint) sizeof (Guid);
        eventDataPtr[14].DataPointer = (ulong) &UniqueIdentifier;
        eventDataPtr[14].Size = (uint) sizeof (Guid);
        eventDataPtr[15].DataPointer = (ulong) &CPUTime;
        eventDataPtr[15].Size = 4U;
        eventDataPtr[16].DataPointer = (ulong) &ElapsedTime;
        eventDataPtr[16].Size = 4U;
        eventDataPtr[17].DataPointer = (ulong) &CPUCycles;
        eventDataPtr[17].Size = 8U;
        eventDataPtr[18].DataPointer = (ulong) &SqlExecutionTime;
        eventDataPtr[18].Size = 4U;
        eventDataPtr[19].DataPointer = (ulong) &SqlExecutionCount;
        eventDataPtr[19].Size = 4U;
        eventDataPtr[20].DataPointer = (ulong) &RedisExecutionTime;
        eventDataPtr[20].Size = 4U;
        eventDataPtr[21].DataPointer = (ulong) &RedisExecutionCount;
        eventDataPtr[21].Size = 4U;
        eventDataPtr[22].DataPointer = (ulong) &AadGraphExecutionTime;
        eventDataPtr[22].Size = 4U;
        eventDataPtr[23].DataPointer = (ulong) &AadGraphExecutionCount;
        eventDataPtr[23].Size = 4U;
        eventDataPtr[24].DataPointer = (ulong) &AadTokenExecutionTime;
        eventDataPtr[24].Size = 4U;
        eventDataPtr[25].DataPointer = (ulong) &AadTokenExecutionCount;
        eventDataPtr[25].Size = 4U;
        eventDataPtr[26].DataPointer = (ulong) &BlobStorageExecutionTime;
        eventDataPtr[26].Size = 4U;
        eventDataPtr[27].DataPointer = (ulong) &BlobStorageExecutionCount;
        eventDataPtr[27].Size = 4U;
        eventDataPtr[28].DataPointer = (ulong) &TableStorageExecutionTime;
        eventDataPtr[28].Size = 4U;
        eventDataPtr[29].DataPointer = (ulong) &TableStorageExecutionCount;
        eventDataPtr[29].Size = 4U;
        eventDataPtr[30].DataPointer = (ulong) &ServiceBusExecutionTime;
        eventDataPtr[30].Size = 4U;
        eventDataPtr[31].DataPointer = (ulong) &ServiceBusExecutionCount;
        eventDataPtr[31].Size = 4U;
        eventDataPtr[32].DataPointer = (ulong) &VssClientExecutionTime;
        eventDataPtr[32].Size = 4U;
        eventDataPtr[33].DataPointer = (ulong) &VssClientExecutionCount;
        eventDataPtr[33].Size = 4U;
        eventDataPtr[34].DataPointer = (ulong) &SqlRetryExecutionTime;
        eventDataPtr[34].Size = 4U;
        eventDataPtr[35].DataPointer = (ulong) &SqlRetryExecutionCount;
        eventDataPtr[35].Size = 4U;
        eventDataPtr[36].DataPointer = (ulong) &SqlReadOnlyExecutionTime;
        eventDataPtr[36].Size = 4U;
        eventDataPtr[37].DataPointer = (ulong) &SqlReadOnlyExecutionCount;
        eventDataPtr[37].Size = 4U;
        eventDataPtr[38].DataPointer = (ulong) &FinalSqlCommandExecutionTime;
        eventDataPtr[38].Size = 4U;
        eventDataPtr[39].Size = (uint) ((MessageId.Length + 1) * 2);
        eventDataPtr[40].Size = (uint) ((Namespace.Length + 1) * 2);
        eventDataPtr[41].Size = (uint) ((ContentType.Length + 1) * 2);
        fixed (char* chPtr1 = TopicName)
          fixed (char* chPtr2 = Plugin)
            fixed (char* chPtr3 = ExceptionType)
              fixed (char* chPtr4 = ExceptionMessage)
                fixed (char* chPtr5 = MessageId)
                  fixed (char* chPtr6 = Namespace)
                    fixed (char* chPtr7 = ContentType)
                    {
                      eventDataPtr[2].DataPointer = (ulong) chPtr1;
                      eventDataPtr[3].DataPointer = (ulong) chPtr2;
                      eventDataPtr[7].DataPointer = (ulong) chPtr3;
                      eventDataPtr[8].DataPointer = (ulong) chPtr4;
                      eventDataPtr[39].DataPointer = (ulong) chPtr5;
                      eventDataPtr[40].DataPointer = (ulong) chPtr6;
                      eventDataPtr[41].DataPointer = (ulong) chPtr7;
                      flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                    }
      }
      return flag;
    }

    internal unsafe bool TemplateClientTrace2(
      ref EventDescriptor eventDescriptor,
      string Properties,
      Guid UniqueIdentifier,
      string AnonymousIdentifier,
      Guid HostId,
      Guid ParentHostId,
      byte HostType,
      Guid VSID,
      string Area,
      string Feature,
      string Useragent,
      Guid CUID,
      string Method,
      string Component,
      string Message,
      string ExceptionType,
      Guid E2EID,
      Guid TenantId,
      Guid ProviderId,
      DateTime StartTime)
    {
      int dataCount = 19;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((Properties.Length + 1) * 2);
        eventDataPtr[1].DataPointer = (ulong) &UniqueIdentifier;
        eventDataPtr[1].Size = (uint) sizeof (Guid);
        eventDataPtr[2].Size = (uint) ((AnonymousIdentifier.Length + 1) * 2);
        eventDataPtr[3].DataPointer = (ulong) &HostId;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        eventDataPtr[4].DataPointer = (ulong) &ParentHostId;
        eventDataPtr[4].Size = (uint) sizeof (Guid);
        eventDataPtr[5].DataPointer = (ulong) &HostType;
        eventDataPtr[5].Size = 1U;
        eventDataPtr[6].DataPointer = (ulong) &VSID;
        eventDataPtr[6].Size = (uint) sizeof (Guid);
        eventDataPtr[7].Size = (uint) ((Area.Length + 1) * 2);
        eventDataPtr[8].Size = (uint) ((Feature.Length + 1) * 2);
        eventDataPtr[9].Size = (uint) ((Useragent.Length + 1) * 2);
        eventDataPtr[10].DataPointer = (ulong) &CUID;
        eventDataPtr[10].Size = (uint) sizeof (Guid);
        eventDataPtr[11].Size = (uint) ((Method.Length + 1) * 2);
        eventDataPtr[12].Size = (uint) ((Component.Length + 1) * 2);
        eventDataPtr[13].Size = (uint) ((Message.Length + 1) * 2);
        eventDataPtr[14].Size = (uint) ((ExceptionType.Length + 1) * 2);
        eventDataPtr[15].DataPointer = (ulong) &E2EID;
        eventDataPtr[15].Size = (uint) sizeof (Guid);
        eventDataPtr[16].DataPointer = (ulong) &TenantId;
        eventDataPtr[16].Size = (uint) sizeof (Guid);
        eventDataPtr[17].DataPointer = (ulong) &ProviderId;
        eventDataPtr[17].Size = (uint) sizeof (Guid);
        long fileTime = StartTime.ToFileTime();
        eventDataPtr[18].DataPointer = (ulong) &fileTime;
        eventDataPtr[18].Size = 8U;
        fixed (char* chPtr1 = Properties)
          fixed (char* chPtr2 = AnonymousIdentifier)
            fixed (char* chPtr3 = Area)
              fixed (char* chPtr4 = Feature)
                fixed (char* chPtr5 = Useragent)
                  fixed (char* chPtr6 = Method)
                    fixed (char* chPtr7 = Component)
                      fixed (char* chPtr8 = Message)
                        fixed (char* chPtr9 = ExceptionType)
                        {
                          eventDataPtr->DataPointer = (ulong) chPtr1;
                          eventDataPtr[2].DataPointer = (ulong) chPtr2;
                          eventDataPtr[7].DataPointer = (ulong) chPtr3;
                          eventDataPtr[8].DataPointer = (ulong) chPtr4;
                          eventDataPtr[9].DataPointer = (ulong) chPtr5;
                          eventDataPtr[11].DataPointer = (ulong) chPtr6;
                          eventDataPtr[12].DataPointer = (ulong) chPtr7;
                          eventDataPtr[13].DataPointer = (ulong) chPtr8;
                          eventDataPtr[14].DataPointer = (ulong) chPtr9;
                          flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                        }
      }
      return flag;
    }

    internal unsafe bool TemplateActivityLogCore6(
      ref EventDescriptor eventDescriptor,
      Guid HostId,
      string Application,
      string Command,
      int Status,
      int Count,
      DateTime StartTime,
      long ExecutionTime,
      string UserAgent,
      string ExceptionType,
      Guid VSID,
      long TimeToFirstPage,
      int ActivityStatus,
      bool IsExceptionExpected,
      string Feature,
      byte HostType,
      Guid ParentHostId,
      string AnonymousIdentifier,
      Guid CUID,
      Guid TenantId,
      Guid ActivityId,
      Guid UniqueIdentifier,
      int CPUTime,
      int ElapsedTime,
      long DelayTime,
      int SqlExecutionTime,
      int SqlExecutionCount,
      int RedisExecutionTime,
      int RedisExecutionCount,
      int AadExecutionTime,
      int AadExecutionCount,
      int BlobStorageExecutionTime,
      int BlobStorageExecutionCount,
      int TableStorageExecutionTime,
      int TableStorageExecutionCount,
      int ServiceBusExecutionTime,
      int ServiceBusExecutionCount,
      int VssClientExecutionTime,
      int VssClientExecutionCount,
      int SqlRetryExecutionTime,
      int SqlRetryExecutionCount,
      int SqlReadOnlyExecutionTime,
      int SqlReadOnlyExecutionCount,
      byte SupportsPublicAccess,
      Guid PendingAuthenticationSessionId,
      string UriStem)
    {
      int dataCount = 45;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &HostId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((Application.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((Command.Length + 1) * 2);
        eventDataPtr[3].DataPointer = (ulong) &Status;
        eventDataPtr[3].Size = 4U;
        eventDataPtr[4].DataPointer = (ulong) &Count;
        eventDataPtr[4].Size = 4U;
        long fileTime = StartTime.ToFileTime();
        eventDataPtr[5].DataPointer = (ulong) &fileTime;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].DataPointer = (ulong) &ExecutionTime;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].Size = (uint) ((UserAgent.Length + 1) * 2);
        eventDataPtr[8].Size = (uint) ((ExceptionType.Length + 1) * 2);
        eventDataPtr[9].DataPointer = (ulong) &VSID;
        eventDataPtr[9].Size = (uint) sizeof (Guid);
        eventDataPtr[10].DataPointer = (ulong) &TimeToFirstPage;
        eventDataPtr[10].Size = 8U;
        eventDataPtr[11].DataPointer = (ulong) &ActivityStatus;
        eventDataPtr[11].Size = 4U;
        int num = IsExceptionExpected ? 1 : 0;
        eventDataPtr[12].DataPointer = (ulong) &num;
        eventDataPtr[12].Size = 4U;
        eventDataPtr[13].Size = (uint) ((Feature.Length + 1) * 2);
        eventDataPtr[14].DataPointer = (ulong) &HostType;
        eventDataPtr[14].Size = 1U;
        eventDataPtr[15].DataPointer = (ulong) &ParentHostId;
        eventDataPtr[15].Size = (uint) sizeof (Guid);
        eventDataPtr[16].Size = (uint) ((AnonymousIdentifier.Length + 1) * 2);
        eventDataPtr[17].DataPointer = (ulong) &CUID;
        eventDataPtr[17].Size = (uint) sizeof (Guid);
        eventDataPtr[18].DataPointer = (ulong) &TenantId;
        eventDataPtr[18].Size = (uint) sizeof (Guid);
        eventDataPtr[19].DataPointer = (ulong) &ActivityId;
        eventDataPtr[19].Size = (uint) sizeof (Guid);
        eventDataPtr[20].DataPointer = (ulong) &UniqueIdentifier;
        eventDataPtr[20].Size = (uint) sizeof (Guid);
        eventDataPtr[21].DataPointer = (ulong) &CPUTime;
        eventDataPtr[21].Size = 4U;
        eventDataPtr[22].DataPointer = (ulong) &ElapsedTime;
        eventDataPtr[22].Size = 4U;
        eventDataPtr[23].DataPointer = (ulong) &DelayTime;
        eventDataPtr[23].Size = 8U;
        eventDataPtr[24].DataPointer = (ulong) &SqlExecutionTime;
        eventDataPtr[24].Size = 4U;
        eventDataPtr[25].DataPointer = (ulong) &SqlExecutionCount;
        eventDataPtr[25].Size = 4U;
        eventDataPtr[26].DataPointer = (ulong) &RedisExecutionTime;
        eventDataPtr[26].Size = 4U;
        eventDataPtr[27].DataPointer = (ulong) &RedisExecutionCount;
        eventDataPtr[27].Size = 4U;
        eventDataPtr[28].DataPointer = (ulong) &AadExecutionTime;
        eventDataPtr[28].Size = 4U;
        eventDataPtr[29].DataPointer = (ulong) &AadExecutionCount;
        eventDataPtr[29].Size = 4U;
        eventDataPtr[30].DataPointer = (ulong) &BlobStorageExecutionTime;
        eventDataPtr[30].Size = 4U;
        eventDataPtr[31].DataPointer = (ulong) &BlobStorageExecutionCount;
        eventDataPtr[31].Size = 4U;
        eventDataPtr[32].DataPointer = (ulong) &TableStorageExecutionTime;
        eventDataPtr[32].Size = 4U;
        eventDataPtr[33].DataPointer = (ulong) &TableStorageExecutionCount;
        eventDataPtr[33].Size = 4U;
        eventDataPtr[34].DataPointer = (ulong) &ServiceBusExecutionTime;
        eventDataPtr[34].Size = 4U;
        eventDataPtr[35].DataPointer = (ulong) &ServiceBusExecutionCount;
        eventDataPtr[35].Size = 4U;
        eventDataPtr[36].DataPointer = (ulong) &VssClientExecutionTime;
        eventDataPtr[36].Size = 4U;
        eventDataPtr[37].DataPointer = (ulong) &VssClientExecutionCount;
        eventDataPtr[37].Size = 4U;
        eventDataPtr[38].DataPointer = (ulong) &SqlRetryExecutionTime;
        eventDataPtr[38].Size = 4U;
        eventDataPtr[39].DataPointer = (ulong) &SqlRetryExecutionCount;
        eventDataPtr[39].Size = 4U;
        eventDataPtr[40].DataPointer = (ulong) &SqlReadOnlyExecutionTime;
        eventDataPtr[40].Size = 4U;
        eventDataPtr[41].DataPointer = (ulong) &SqlReadOnlyExecutionCount;
        eventDataPtr[41].Size = 4U;
        eventDataPtr[42].DataPointer = (ulong) &SupportsPublicAccess;
        eventDataPtr[42].Size = 1U;
        eventDataPtr[43].DataPointer = (ulong) &PendingAuthenticationSessionId;
        eventDataPtr[43].Size = (uint) sizeof (Guid);
        eventDataPtr[44].Size = (uint) ((UriStem.Length + 1) * 2);
        fixed (char* chPtr1 = Application)
          fixed (char* chPtr2 = Command)
            fixed (char* chPtr3 = UserAgent)
              fixed (char* chPtr4 = ExceptionType)
                fixed (char* chPtr5 = Feature)
                  fixed (char* chPtr6 = AnonymousIdentifier)
                    fixed (char* chPtr7 = UriStem)
                    {
                      eventDataPtr[1].DataPointer = (ulong) chPtr1;
                      eventDataPtr[2].DataPointer = (ulong) chPtr2;
                      eventDataPtr[7].DataPointer = (ulong) chPtr3;
                      eventDataPtr[8].DataPointer = (ulong) chPtr4;
                      eventDataPtr[13].DataPointer = (ulong) chPtr5;
                      eventDataPtr[16].DataPointer = (ulong) chPtr6;
                      eventDataPtr[44].DataPointer = (ulong) chPtr7;
                      flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                    }
      }
      return flag;
    }

    internal unsafe bool TemplateEuiiTrace(
      ref EventDescriptor eventDescriptor,
      string Properties,
      Guid UniqueIdentifier,
      string AnonymousIdentifier,
      Guid HostId,
      Guid ParentHostId,
      byte HostType,
      Guid VSID,
      string Area,
      string Feature,
      string Useragent,
      Guid CUID,
      string Method,
      string Uri,
      string Component,
      string Message,
      string ExceptionType,
      Guid E2EID,
      Guid TenantId,
      Guid ProviderId,
      DateTime StartTime)
    {
      int dataCount = 20;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((Properties.Length + 1) * 2);
        eventDataPtr[1].DataPointer = (ulong) &UniqueIdentifier;
        eventDataPtr[1].Size = (uint) sizeof (Guid);
        eventDataPtr[2].Size = (uint) ((AnonymousIdentifier.Length + 1) * 2);
        eventDataPtr[3].DataPointer = (ulong) &HostId;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        eventDataPtr[4].DataPointer = (ulong) &ParentHostId;
        eventDataPtr[4].Size = (uint) sizeof (Guid);
        eventDataPtr[5].DataPointer = (ulong) &HostType;
        eventDataPtr[5].Size = 1U;
        eventDataPtr[6].DataPointer = (ulong) &VSID;
        eventDataPtr[6].Size = (uint) sizeof (Guid);
        eventDataPtr[7].Size = (uint) ((Area.Length + 1) * 2);
        eventDataPtr[8].Size = (uint) ((Feature.Length + 1) * 2);
        eventDataPtr[9].Size = (uint) ((Useragent.Length + 1) * 2);
        eventDataPtr[10].DataPointer = (ulong) &CUID;
        eventDataPtr[10].Size = (uint) sizeof (Guid);
        eventDataPtr[11].Size = (uint) ((Method.Length + 1) * 2);
        eventDataPtr[12].Size = (uint) ((Uri.Length + 1) * 2);
        eventDataPtr[13].Size = (uint) ((Component.Length + 1) * 2);
        eventDataPtr[14].Size = (uint) ((Message.Length + 1) * 2);
        eventDataPtr[15].Size = (uint) ((ExceptionType.Length + 1) * 2);
        eventDataPtr[16].DataPointer = (ulong) &E2EID;
        eventDataPtr[16].Size = (uint) sizeof (Guid);
        eventDataPtr[17].DataPointer = (ulong) &TenantId;
        eventDataPtr[17].Size = (uint) sizeof (Guid);
        eventDataPtr[18].DataPointer = (ulong) &ProviderId;
        eventDataPtr[18].Size = (uint) sizeof (Guid);
        long fileTime = StartTime.ToFileTime();
        eventDataPtr[19].DataPointer = (ulong) &fileTime;
        eventDataPtr[19].Size = 8U;
        fixed (char* chPtr1 = Properties)
          fixed (char* chPtr2 = AnonymousIdentifier)
            fixed (char* chPtr3 = Area)
              fixed (char* chPtr4 = Feature)
                fixed (char* chPtr5 = Useragent)
                  fixed (char* chPtr6 = Method)
                    fixed (char* chPtr7 = Uri)
                      fixed (char* chPtr8 = Component)
                        fixed (char* chPtr9 = Message)
                          fixed (char* chPtr10 = ExceptionType)
                          {
                            eventDataPtr->DataPointer = (ulong) chPtr1;
                            eventDataPtr[2].DataPointer = (ulong) chPtr2;
                            eventDataPtr[7].DataPointer = (ulong) chPtr3;
                            eventDataPtr[8].DataPointer = (ulong) chPtr4;
                            eventDataPtr[9].DataPointer = (ulong) chPtr5;
                            eventDataPtr[11].DataPointer = (ulong) chPtr6;
                            eventDataPtr[12].DataPointer = (ulong) chPtr7;
                            eventDataPtr[13].DataPointer = (ulong) chPtr8;
                            eventDataPtr[14].DataPointer = (ulong) chPtr9;
                            eventDataPtr[15].DataPointer = (ulong) chPtr10;
                            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                          }
      }
      return flag;
    }

    internal unsafe bool TemplateDetectedEuiiEvent(
      ref EventDescriptor eventDescriptor,
      DateTime EventTime,
      string Source,
      int EuiiType,
      string Message)
    {
      int dataCount = 4;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        long fileTime = EventTime.ToFileTime();
        eventDataPtr->DataPointer = (ulong) &fileTime;
        eventDataPtr->Size = 8U;
        eventDataPtr[1].Size = (uint) ((Source.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &EuiiType;
        eventDataPtr[2].Size = 4U;
        eventDataPtr[3].Size = (uint) ((Message.Length + 1) * 2);
        fixed (char* chPtr1 = Source)
          fixed (char* chPtr2 = Message)
          {
            eventDataPtr[1].DataPointer = (ulong) chPtr1;
            eventDataPtr[3].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateActivityLogMapping4(
      ref EventDescriptor eventDescriptor,
      Guid ActivityId,
      Guid E2EID,
      string IdentityName,
      string IPAddress,
      string AnonymousIdentifier,
      Guid CUID,
      Guid VSID,
      DateTime StartTime)
    {
      int dataCount = 8;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ActivityId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].DataPointer = (ulong) &E2EID;
        eventDataPtr[1].Size = (uint) sizeof (Guid);
        eventDataPtr[2].Size = (uint) ((IdentityName.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((IPAddress.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((AnonymousIdentifier.Length + 1) * 2);
        eventDataPtr[5].DataPointer = (ulong) &CUID;
        eventDataPtr[5].Size = (uint) sizeof (Guid);
        eventDataPtr[6].DataPointer = (ulong) &VSID;
        eventDataPtr[6].Size = (uint) sizeof (Guid);
        long fileTime = StartTime.ToFileTime();
        eventDataPtr[7].DataPointer = (ulong) &fileTime;
        eventDataPtr[7].Size = 8U;
        fixed (char* chPtr1 = IdentityName)
          fixed (char* chPtr2 = IPAddress)
            fixed (char* chPtr3 = AnonymousIdentifier)
            {
              eventDataPtr[2].DataPointer = (ulong) chPtr1;
              eventDataPtr[3].DataPointer = (ulong) chPtr2;
              eventDataPtr[4].DataPointer = (ulong) chPtr3;
              flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
            }
      }
      return flag;
    }

    internal unsafe bool TemplateServicePrincipalIsMember(
      ref EventDescriptor eventDescriptor,
      Guid ServicePrincipalId,
      string GroupSid,
      byte HostType,
      string StackTrace,
      int ExecutionCount)
    {
      int dataCount = 5;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ServicePrincipalId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((GroupSid.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &HostType;
        eventDataPtr[2].Size = 1U;
        eventDataPtr[3].Size = (uint) ((StackTrace.Length + 1) * 2);
        eventDataPtr[4].DataPointer = (ulong) &ExecutionCount;
        eventDataPtr[4].Size = 4U;
        fixed (char* chPtr1 = GroupSid)
          fixed (char* chPtr2 = StackTrace)
          {
            eventDataPtr[1].DataPointer = (ulong) chPtr1;
            eventDataPtr[3].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateServiceBusPublishMetadata2(
      ref EventDescriptor eventDescriptor,
      Guid HostId,
      byte HostType,
      string TopicName,
      string MessageId,
      string TargetScaleUnits,
      bool Status,
      DateTime StartTime,
      long PublishTimeMs,
      Guid ActivityId,
      Guid E2EID,
      Guid UniqueIdentifier,
      string Namespace,
      string ContentType)
    {
      int dataCount = 13;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &HostId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].DataPointer = (ulong) &HostType;
        eventDataPtr[1].Size = 1U;
        eventDataPtr[2].Size = (uint) ((TopicName.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((MessageId.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((TargetScaleUnits.Length + 1) * 2);
        int num = Status ? 1 : 0;
        eventDataPtr[5].DataPointer = (ulong) &num;
        eventDataPtr[5].Size = 4U;
        long fileTime = StartTime.ToFileTime();
        eventDataPtr[6].DataPointer = (ulong) &fileTime;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].DataPointer = (ulong) &PublishTimeMs;
        eventDataPtr[7].Size = 8U;
        eventDataPtr[8].DataPointer = (ulong) &ActivityId;
        eventDataPtr[8].Size = (uint) sizeof (Guid);
        eventDataPtr[9].DataPointer = (ulong) &E2EID;
        eventDataPtr[9].Size = (uint) sizeof (Guid);
        eventDataPtr[10].DataPointer = (ulong) &UniqueIdentifier;
        eventDataPtr[10].Size = (uint) sizeof (Guid);
        eventDataPtr[11].Size = (uint) ((Namespace.Length + 1) * 2);
        eventDataPtr[12].Size = (uint) ((ContentType.Length + 1) * 2);
        fixed (char* chPtr1 = TopicName)
          fixed (char* chPtr2 = MessageId)
            fixed (char* chPtr3 = TargetScaleUnits)
              fixed (char* chPtr4 = Namespace)
                fixed (char* chPtr5 = ContentType)
                {
                  eventDataPtr[2].DataPointer = (ulong) chPtr1;
                  eventDataPtr[3].DataPointer = (ulong) chPtr2;
                  eventDataPtr[4].DataPointer = (ulong) chPtr3;
                  eventDataPtr[11].DataPointer = (ulong) chPtr4;
                  eventDataPtr[12].DataPointer = (ulong) chPtr5;
                  flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                }
      }
      return flag;
    }

    internal unsafe bool TemplateEuiiUser(
      ref EventDescriptor eventDescriptor,
      string EntityTypeName,
      string DataFeed)
    {
      int dataCount = 2;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((EntityTypeName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((DataFeed.Length + 1) * 2);
        fixed (char* chPtr1 = EntityTypeName)
          fixed (char* chPtr2 = DataFeed)
          {
            eventDataPtr->DataPointer = (ulong) chPtr1;
            eventDataPtr[1].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateOrchestrationLog3(
      ref EventDescriptor eventDescriptor,
      string OrchestrationId,
      DateTime StartTime,
      DateTime EndTime,
      long ExecutionTimeThreshold,
      byte OrchestrationStatus,
      string Application,
      string Feature,
      string Command,
      string ExceptionType,
      string ExceptionMessage,
      bool IsExceptionExpected,
      byte HostType,
      Guid HostId,
      Guid ParentHostId,
      Guid VSID,
      Guid CUID,
      string AnonymousIdentifier)
    {
      int dataCount = 17;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((OrchestrationId.Length + 1) * 2);
        long fileTime1 = StartTime.ToFileTime();
        eventDataPtr[1].DataPointer = (ulong) &fileTime1;
        eventDataPtr[1].Size = 8U;
        long fileTime2 = EndTime.ToFileTime();
        eventDataPtr[2].DataPointer = (ulong) &fileTime2;
        eventDataPtr[2].Size = 8U;
        eventDataPtr[3].DataPointer = (ulong) &ExecutionTimeThreshold;
        eventDataPtr[3].Size = 8U;
        eventDataPtr[4].DataPointer = (ulong) &OrchestrationStatus;
        eventDataPtr[4].Size = 1U;
        eventDataPtr[5].Size = (uint) ((Application.Length + 1) * 2);
        eventDataPtr[6].Size = (uint) ((Feature.Length + 1) * 2);
        eventDataPtr[7].Size = (uint) ((Command.Length + 1) * 2);
        eventDataPtr[8].Size = (uint) ((ExceptionType.Length + 1) * 2);
        eventDataPtr[9].Size = (uint) ((ExceptionMessage.Length + 1) * 2);
        int num = IsExceptionExpected ? 1 : 0;
        eventDataPtr[10].DataPointer = (ulong) &num;
        eventDataPtr[10].Size = 4U;
        eventDataPtr[11].DataPointer = (ulong) &HostType;
        eventDataPtr[11].Size = 1U;
        eventDataPtr[12].DataPointer = (ulong) &HostId;
        eventDataPtr[12].Size = (uint) sizeof (Guid);
        eventDataPtr[13].DataPointer = (ulong) &ParentHostId;
        eventDataPtr[13].Size = (uint) sizeof (Guid);
        eventDataPtr[14].DataPointer = (ulong) &VSID;
        eventDataPtr[14].Size = (uint) sizeof (Guid);
        eventDataPtr[15].DataPointer = (ulong) &CUID;
        eventDataPtr[15].Size = (uint) sizeof (Guid);
        eventDataPtr[16].Size = (uint) ((AnonymousIdentifier.Length + 1) * 2);
        fixed (char* chPtr1 = OrchestrationId)
          fixed (char* chPtr2 = Application)
            fixed (char* chPtr3 = Feature)
              fixed (char* chPtr4 = Command)
                fixed (char* chPtr5 = ExceptionType)
                  fixed (char* chPtr6 = ExceptionMessage)
                    fixed (char* chPtr7 = AnonymousIdentifier)
                    {
                      eventDataPtr->DataPointer = (ulong) chPtr1;
                      eventDataPtr[5].DataPointer = (ulong) chPtr2;
                      eventDataPtr[6].DataPointer = (ulong) chPtr3;
                      eventDataPtr[7].DataPointer = (ulong) chPtr4;
                      eventDataPtr[8].DataPointer = (ulong) chPtr5;
                      eventDataPtr[9].DataPointer = (ulong) chPtr6;
                      eventDataPtr[16].DataPointer = (ulong) chPtr7;
                      flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                    }
      }
      return flag;
    }

    internal unsafe bool TemplateServiceBusMetrics(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string ServiceBusNamespace,
      string SKUTier,
      DateTime StartingIntervalUTC,
      double TotalSuccessfulRequests,
      double TotalServerErrors,
      double TotalUserErrors,
      double TotalThrottledRequests,
      double TotalIncomingRequests,
      double TotalIncomingMessages,
      double TotalOutgoingMessages,
      double TotalActiveConnections,
      double AverageSizeInBytes,
      double AverageMessages,
      double AverageActiveMessages)
    {
      int dataCount = 15;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((ServiceBusNamespace.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((SKUTier.Length + 1) * 2);
        long fileTime = StartingIntervalUTC.ToFileTime();
        eventDataPtr[3].DataPointer = (ulong) &fileTime;
        eventDataPtr[3].Size = 8U;
        eventDataPtr[4].DataPointer = (ulong) &TotalSuccessfulRequests;
        eventDataPtr[4].Size = 8U;
        eventDataPtr[5].DataPointer = (ulong) &TotalServerErrors;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].DataPointer = (ulong) &TotalUserErrors;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].DataPointer = (ulong) &TotalThrottledRequests;
        eventDataPtr[7].Size = 8U;
        eventDataPtr[8].DataPointer = (ulong) &TotalIncomingRequests;
        eventDataPtr[8].Size = 8U;
        eventDataPtr[9].DataPointer = (ulong) &TotalIncomingMessages;
        eventDataPtr[9].Size = 8U;
        eventDataPtr[10].DataPointer = (ulong) &TotalOutgoingMessages;
        eventDataPtr[10].Size = 8U;
        eventDataPtr[11].DataPointer = (ulong) &TotalActiveConnections;
        eventDataPtr[11].Size = 8U;
        eventDataPtr[12].DataPointer = (ulong) &AverageSizeInBytes;
        eventDataPtr[12].Size = 8U;
        eventDataPtr[13].DataPointer = (ulong) &AverageMessages;
        eventDataPtr[13].Size = 8U;
        eventDataPtr[14].DataPointer = (ulong) &AverageActiveMessages;
        eventDataPtr[14].Size = 8U;
        fixed (char* chPtr1 = ServiceBusNamespace)
          fixed (char* chPtr2 = SKUTier)
          {
            eventDataPtr[1].DataPointer = (ulong) chPtr1;
            eventDataPtr[2].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateSQLSpinlocks3(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string ServerName,
      string DatabaseName,
      string SpinlockName,
      long Collisions,
      long Spins,
      float SpinsPerCollision,
      long SleepTime,
      long Backoffs,
      bool IsReadOnly,
      string Listener)
    {
      int dataCount = 11;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((SpinlockName.Length + 1) * 2);
        eventDataPtr[4].DataPointer = (ulong) &Collisions;
        eventDataPtr[4].Size = 8U;
        eventDataPtr[5].DataPointer = (ulong) &Spins;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].DataPointer = (ulong) &SpinsPerCollision;
        eventDataPtr[6].Size = 4U;
        eventDataPtr[7].DataPointer = (ulong) &SleepTime;
        eventDataPtr[7].Size = 8U;
        eventDataPtr[8].DataPointer = (ulong) &Backoffs;
        eventDataPtr[8].Size = 8U;
        int num = IsReadOnly ? 1 : 0;
        eventDataPtr[9].DataPointer = (ulong) &num;
        eventDataPtr[9].Size = 4U;
        eventDataPtr[10].Size = (uint) ((Listener.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = SpinlockName)
              fixed (char* chPtr4 = Listener)
              {
                eventDataPtr[1].DataPointer = (ulong) chPtr1;
                eventDataPtr[2].DataPointer = (ulong) chPtr2;
                eventDataPtr[3].DataPointer = (ulong) chPtr3;
                eventDataPtr[10].DataPointer = (ulong) chPtr4;
                flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
              }
      }
      return flag;
    }

    internal unsafe bool TemplateJobHistoryCore(
      ref EventDescriptor eventDescriptor,
      string Plugin,
      string JobName,
      Guid JobSource,
      Guid JobId,
      DateTime QueueTime,
      DateTime StartTime,
      long ExecutionTime,
      Guid AgentId,
      int Result,
      int QueuedReasons,
      int QueueFlags,
      short Priority,
      int LogicalReads,
      int PhysicalReads,
      int CPUTime,
      int ElapsedTime,
      string Feature,
      int SqlExecutionTime,
      int SqlExecutionCount,
      int RedisExecutionTime,
      int RedisExecutionCount,
      int AadExecutionTime,
      int AadExecutionCount,
      int BlobStorageExecutionTime,
      int BlobStorageExecutionCount,
      int TableStorageExecutionTime,
      int TableStorageExecutionCount,
      int ServiceBusExecutionTime,
      int ServiceBusExecutionCount,
      int VssClientExecutionTime,
      int VssClientExecutionCount,
      int SqlRetryExecutionTime,
      int SqlRetryExecutionCount,
      int SqlReadOnlyExecutionTime,
      int SqlReadOnlyExecutionCount,
      long CPUCycles,
      int FinalSqlCommandExecutionTime,
      Guid E2EID,
      int AadGraphExecutionTime,
      int AadGraphExecutionCount,
      int AadTokenExecutionTime,
      int AadTokenExecutionCount)
    {
      int dataCount = 42;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((Plugin.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((JobName.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &JobSource;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].DataPointer = (ulong) &JobId;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        long fileTime1 = QueueTime.ToFileTime();
        eventDataPtr[4].DataPointer = (ulong) &fileTime1;
        eventDataPtr[4].Size = 8U;
        long fileTime2 = StartTime.ToFileTime();
        eventDataPtr[5].DataPointer = (ulong) &fileTime2;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].DataPointer = (ulong) &ExecutionTime;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].DataPointer = (ulong) &AgentId;
        eventDataPtr[7].Size = (uint) sizeof (Guid);
        eventDataPtr[8].DataPointer = (ulong) &Result;
        eventDataPtr[8].Size = 4U;
        eventDataPtr[9].DataPointer = (ulong) &QueuedReasons;
        eventDataPtr[9].Size = 4U;
        eventDataPtr[10].DataPointer = (ulong) &QueueFlags;
        eventDataPtr[10].Size = 4U;
        eventDataPtr[11].DataPointer = (ulong) &Priority;
        eventDataPtr[11].Size = 2U;
        eventDataPtr[12].DataPointer = (ulong) &LogicalReads;
        eventDataPtr[12].Size = 4U;
        eventDataPtr[13].DataPointer = (ulong) &PhysicalReads;
        eventDataPtr[13].Size = 4U;
        eventDataPtr[14].DataPointer = (ulong) &CPUTime;
        eventDataPtr[14].Size = 4U;
        eventDataPtr[15].DataPointer = (ulong) &ElapsedTime;
        eventDataPtr[15].Size = 4U;
        eventDataPtr[16].Size = (uint) ((Feature.Length + 1) * 2);
        eventDataPtr[17].DataPointer = (ulong) &SqlExecutionTime;
        eventDataPtr[17].Size = 4U;
        eventDataPtr[18].DataPointer = (ulong) &SqlExecutionCount;
        eventDataPtr[18].Size = 4U;
        eventDataPtr[19].DataPointer = (ulong) &RedisExecutionTime;
        eventDataPtr[19].Size = 4U;
        eventDataPtr[20].DataPointer = (ulong) &RedisExecutionCount;
        eventDataPtr[20].Size = 4U;
        eventDataPtr[21].DataPointer = (ulong) &AadExecutionTime;
        eventDataPtr[21].Size = 4U;
        eventDataPtr[22].DataPointer = (ulong) &AadExecutionCount;
        eventDataPtr[22].Size = 4U;
        eventDataPtr[23].DataPointer = (ulong) &BlobStorageExecutionTime;
        eventDataPtr[23].Size = 4U;
        eventDataPtr[24].DataPointer = (ulong) &BlobStorageExecutionCount;
        eventDataPtr[24].Size = 4U;
        eventDataPtr[25].DataPointer = (ulong) &TableStorageExecutionTime;
        eventDataPtr[25].Size = 4U;
        eventDataPtr[26].DataPointer = (ulong) &TableStorageExecutionCount;
        eventDataPtr[26].Size = 4U;
        eventDataPtr[27].DataPointer = (ulong) &ServiceBusExecutionTime;
        eventDataPtr[27].Size = 4U;
        eventDataPtr[28].DataPointer = (ulong) &ServiceBusExecutionCount;
        eventDataPtr[28].Size = 4U;
        eventDataPtr[29].DataPointer = (ulong) &VssClientExecutionTime;
        eventDataPtr[29].Size = 4U;
        eventDataPtr[30].DataPointer = (ulong) &VssClientExecutionCount;
        eventDataPtr[30].Size = 4U;
        eventDataPtr[31].DataPointer = (ulong) &SqlRetryExecutionTime;
        eventDataPtr[31].Size = 4U;
        eventDataPtr[32].DataPointer = (ulong) &SqlRetryExecutionCount;
        eventDataPtr[32].Size = 4U;
        eventDataPtr[33].DataPointer = (ulong) &SqlReadOnlyExecutionTime;
        eventDataPtr[33].Size = 4U;
        eventDataPtr[34].DataPointer = (ulong) &SqlReadOnlyExecutionCount;
        eventDataPtr[34].Size = 4U;
        eventDataPtr[35].DataPointer = (ulong) &CPUCycles;
        eventDataPtr[35].Size = 8U;
        eventDataPtr[36].DataPointer = (ulong) &FinalSqlCommandExecutionTime;
        eventDataPtr[36].Size = 4U;
        eventDataPtr[37].DataPointer = (ulong) &E2EID;
        eventDataPtr[37].Size = (uint) sizeof (Guid);
        eventDataPtr[38].DataPointer = (ulong) &AadGraphExecutionTime;
        eventDataPtr[38].Size = 4U;
        eventDataPtr[39].DataPointer = (ulong) &AadGraphExecutionCount;
        eventDataPtr[39].Size = 4U;
        eventDataPtr[40].DataPointer = (ulong) &AadTokenExecutionTime;
        eventDataPtr[40].Size = 4U;
        eventDataPtr[41].DataPointer = (ulong) &AadTokenExecutionCount;
        eventDataPtr[41].Size = 4U;
        fixed (char* chPtr1 = Plugin)
          fixed (char* chPtr2 = JobName)
            fixed (char* chPtr3 = Feature)
            {
              eventDataPtr->DataPointer = (ulong) chPtr1;
              eventDataPtr[1].DataPointer = (ulong) chPtr2;
              eventDataPtr[16].DataPointer = (ulong) chPtr3;
              flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
            }
      }
      return flag;
    }

    internal unsafe bool TemplateRedisCacheMetrics(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string RedisCacheInstance,
      string SKUTier,
      DateTime StartingIntervalUTC,
      double TotalConnectedClients,
      double TotalCommandsProcessed,
      double TotalCacheHits,
      double TotalCacheMisses,
      double TotalUsedMemory,
      double TotalUsedMemoryRss,
      double TotalServerLoad,
      double TotalProcessorTime,
      double TotalOperationsPerSecond,
      double TotalGetCommands,
      double TotalSetCommands,
      double TotalEvictedKeys,
      double TotalTotalKeys,
      double TotalExpiredKeys,
      double TotalUsedMemoryPercentage,
      double TotalCacheRead,
      double TotalErrors)
    {
      int dataCount = 21;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((RedisCacheInstance.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((SKUTier.Length + 1) * 2);
        long fileTime = StartingIntervalUTC.ToFileTime();
        eventDataPtr[3].DataPointer = (ulong) &fileTime;
        eventDataPtr[3].Size = 8U;
        eventDataPtr[4].DataPointer = (ulong) &TotalConnectedClients;
        eventDataPtr[4].Size = 8U;
        eventDataPtr[5].DataPointer = (ulong) &TotalCommandsProcessed;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].DataPointer = (ulong) &TotalCacheHits;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].DataPointer = (ulong) &TotalCacheMisses;
        eventDataPtr[7].Size = 8U;
        eventDataPtr[8].DataPointer = (ulong) &TotalUsedMemory;
        eventDataPtr[8].Size = 8U;
        eventDataPtr[9].DataPointer = (ulong) &TotalUsedMemoryRss;
        eventDataPtr[9].Size = 8U;
        eventDataPtr[10].DataPointer = (ulong) &TotalServerLoad;
        eventDataPtr[10].Size = 8U;
        eventDataPtr[11].DataPointer = (ulong) &TotalProcessorTime;
        eventDataPtr[11].Size = 8U;
        eventDataPtr[12].DataPointer = (ulong) &TotalOperationsPerSecond;
        eventDataPtr[12].Size = 8U;
        eventDataPtr[13].DataPointer = (ulong) &TotalGetCommands;
        eventDataPtr[13].Size = 8U;
        eventDataPtr[14].DataPointer = (ulong) &TotalSetCommands;
        eventDataPtr[14].Size = 8U;
        eventDataPtr[15].DataPointer = (ulong) &TotalEvictedKeys;
        eventDataPtr[15].Size = 8U;
        eventDataPtr[16].DataPointer = (ulong) &TotalTotalKeys;
        eventDataPtr[16].Size = 8U;
        eventDataPtr[17].DataPointer = (ulong) &TotalExpiredKeys;
        eventDataPtr[17].Size = 8U;
        eventDataPtr[18].DataPointer = (ulong) &TotalUsedMemoryPercentage;
        eventDataPtr[18].Size = 8U;
        eventDataPtr[19].DataPointer = (ulong) &TotalCacheRead;
        eventDataPtr[19].Size = 8U;
        eventDataPtr[20].DataPointer = (ulong) &TotalErrors;
        eventDataPtr[20].Size = 8U;
        fixed (char* chPtr1 = RedisCacheInstance)
          fixed (char* chPtr2 = SKUTier)
          {
            eventDataPtr[1].DataPointer = (ulong) chPtr1;
            eventDataPtr[2].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateServiceHostExtended(
      ref EventDescriptor eventDescriptor,
      Guid HostId,
      byte HostType,
      Guid ParentHostId,
      string HostName,
      string DatabaseServerName,
      string DatabaseName,
      short Status,
      string StatusReason,
      DateTime LastUserAccess,
      bool IsDeleted)
    {
      int dataCount = 10;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &HostId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].DataPointer = (ulong) &HostType;
        eventDataPtr[1].Size = 1U;
        eventDataPtr[2].DataPointer = (ulong) &ParentHostId;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].Size = (uint) ((HostName.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((DatabaseServerName.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[6].DataPointer = (ulong) &Status;
        eventDataPtr[6].Size = 2U;
        eventDataPtr[7].Size = (uint) ((StatusReason.Length + 1) * 2);
        long fileTime = LastUserAccess.ToFileTime();
        eventDataPtr[8].DataPointer = (ulong) &fileTime;
        eventDataPtr[8].Size = 8U;
        int num = IsDeleted ? 1 : 0;
        eventDataPtr[9].DataPointer = (ulong) &num;
        eventDataPtr[9].Size = 4U;
        fixed (char* chPtr1 = HostName)
          fixed (char* chPtr2 = DatabaseServerName)
            fixed (char* chPtr3 = DatabaseName)
              fixed (char* chPtr4 = StatusReason)
              {
                eventDataPtr[3].DataPointer = (ulong) chPtr1;
                eventDataPtr[4].DataPointer = (ulong) chPtr2;
                eventDataPtr[5].DataPointer = (ulong) chPtr3;
                eventDataPtr[7].DataPointer = (ulong) chPtr4;
                flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
              }
      }
      return flag;
    }

    internal unsafe bool TemplateFeatureFlagStatusHostVSID(
      ref EventDescriptor eventDescriptor,
      long RunId,
      string FeatureFlagName,
      string EffectiveState,
      string ExplicitState,
      string HostId,
      string VSID)
    {
      int dataCount = 6;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &RunId;
        eventDataPtr->Size = 8U;
        eventDataPtr[1].Size = (uint) ((FeatureFlagName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((EffectiveState.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((ExplicitState.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((HostId.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((VSID.Length + 1) * 2);
        fixed (char* chPtr1 = FeatureFlagName)
          fixed (char* chPtr2 = EffectiveState)
            fixed (char* chPtr3 = ExplicitState)
              fixed (char* chPtr4 = HostId)
                fixed (char* chPtr5 = VSID)
                {
                  eventDataPtr[1].DataPointer = (ulong) chPtr1;
                  eventDataPtr[2].DataPointer = (ulong) chPtr2;
                  eventDataPtr[3].DataPointer = (ulong) chPtr3;
                  eventDataPtr[4].DataPointer = (ulong) chPtr4;
                  eventDataPtr[5].DataPointer = (ulong) chPtr5;
                  flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                }
      }
      return flag;
    }

    internal unsafe bool TemplateDocDBStorageMetrics(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string AccountName,
      string DatabaseCategory,
      string DatabaseId,
      string CollectionId,
      long CollectionSizeUsage,
      long CollectionSizeQuota,
      string PartitionKeyRangeId,
      long PartitionKeyRangeDocumentCount,
      long PartitionKeyRangeSize,
      string PartitionKeyRangeDominantPartitionKeys)
    {
      int dataCount = 11;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((AccountName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseCategory.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((DatabaseId.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((CollectionId.Length + 1) * 2);
        eventDataPtr[5].DataPointer = (ulong) &CollectionSizeUsage;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].DataPointer = (ulong) &CollectionSizeQuota;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].Size = (uint) ((PartitionKeyRangeId.Length + 1) * 2);
        eventDataPtr[8].DataPointer = (ulong) &PartitionKeyRangeDocumentCount;
        eventDataPtr[8].Size = 8U;
        eventDataPtr[9].DataPointer = (ulong) &PartitionKeyRangeSize;
        eventDataPtr[9].Size = 8U;
        eventDataPtr[10].Size = (uint) ((PartitionKeyRangeDominantPartitionKeys.Length + 1) * 2);
        fixed (char* chPtr1 = AccountName)
          fixed (char* chPtr2 = DatabaseCategory)
            fixed (char* chPtr3 = DatabaseId)
              fixed (char* chPtr4 = CollectionId)
                fixed (char* chPtr5 = PartitionKeyRangeId)
                  fixed (char* chPtr6 = PartitionKeyRangeDominantPartitionKeys)
                  {
                    eventDataPtr[1].DataPointer = (ulong) chPtr1;
                    eventDataPtr[2].DataPointer = (ulong) chPtr2;
                    eventDataPtr[3].DataPointer = (ulong) chPtr3;
                    eventDataPtr[4].DataPointer = (ulong) chPtr4;
                    eventDataPtr[7].DataPointer = (ulong) chPtr5;
                    eventDataPtr[10].DataPointer = (ulong) chPtr6;
                    flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                  }
      }
      return flag;
    }

    internal unsafe bool TemplateAzureSearchMetrics(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string AzureSearchInstance,
      string SKUTier,
      DateTime StartingIntervalUTC,
      double SearchLatency,
      double QueriesPerSecond,
      double ThrottledSearchQueriesPercentage)
    {
      int dataCount = 7;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((AzureSearchInstance.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((SKUTier.Length + 1) * 2);
        long fileTime = StartingIntervalUTC.ToFileTime();
        eventDataPtr[3].DataPointer = (ulong) &fileTime;
        eventDataPtr[3].Size = 8U;
        eventDataPtr[4].DataPointer = (ulong) &SearchLatency;
        eventDataPtr[4].Size = 8U;
        eventDataPtr[5].DataPointer = (ulong) &QueriesPerSecond;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].DataPointer = (ulong) &ThrottledSearchQueriesPercentage;
        eventDataPtr[6].Size = 8U;
        fixed (char* chPtr1 = AzureSearchInstance)
          fixed (char* chPtr2 = SKUTier)
          {
            eventDataPtr[1].DataPointer = (ulong) chPtr1;
            eventDataPtr[2].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateDocDBRUMetrics(
      ref EventDescriptor eventDescriptor,
      string AccountName,
      string DatabaseCategory,
      string DatabaseId,
      string CollectionId,
      string DocumentType,
      long ConsumedRUs)
    {
      int dataCount = 6;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((AccountName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((DatabaseCategory.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseId.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((CollectionId.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((DocumentType.Length + 1) * 2);
        eventDataPtr[5].DataPointer = (ulong) &ConsumedRUs;
        eventDataPtr[5].Size = 8U;
        fixed (char* chPtr1 = AccountName)
          fixed (char* chPtr2 = DatabaseCategory)
            fixed (char* chPtr3 = DatabaseId)
              fixed (char* chPtr4 = CollectionId)
                fixed (char* chPtr5 = DocumentType)
                {
                  eventDataPtr->DataPointer = (ulong) chPtr1;
                  eventDataPtr[1].DataPointer = (ulong) chPtr2;
                  eventDataPtr[2].DataPointer = (ulong) chPtr3;
                  eventDataPtr[3].DataPointer = (ulong) chPtr4;
                  eventDataPtr[4].DataPointer = (ulong) chPtr5;
                  flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                }
      }
      return flag;
    }

    internal unsafe bool TemplateVirtualFileStats(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string ServerName,
      string DatabaseName,
      bool ReadOnly,
      short databaseId,
      short fileId,
      long sampleMs,
      long numReads,
      long numBytesRead,
      long ioStallReadMs,
      long numWrites,
      long numBytesWritten,
      long ioStallWriteMs,
      long ioStall,
      long sizeOnDiskBytes,
      long ioStallQueuedReadMs,
      long ioStallQueueWriteMs)
    {
      int dataCount = 17;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        int num = ReadOnly ? 1 : 0;
        eventDataPtr[3].DataPointer = (ulong) &num;
        eventDataPtr[3].Size = 4U;
        eventDataPtr[4].DataPointer = (ulong) &databaseId;
        eventDataPtr[4].Size = 2U;
        eventDataPtr[5].DataPointer = (ulong) &fileId;
        eventDataPtr[5].Size = 2U;
        eventDataPtr[6].DataPointer = (ulong) &sampleMs;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].DataPointer = (ulong) &numReads;
        eventDataPtr[7].Size = 8U;
        eventDataPtr[8].DataPointer = (ulong) &numBytesRead;
        eventDataPtr[8].Size = 8U;
        eventDataPtr[9].DataPointer = (ulong) &ioStallReadMs;
        eventDataPtr[9].Size = 8U;
        eventDataPtr[10].DataPointer = (ulong) &numWrites;
        eventDataPtr[10].Size = 8U;
        eventDataPtr[11].DataPointer = (ulong) &numBytesWritten;
        eventDataPtr[11].Size = 8U;
        eventDataPtr[12].DataPointer = (ulong) &ioStallWriteMs;
        eventDataPtr[12].Size = 8U;
        eventDataPtr[13].DataPointer = (ulong) &ioStall;
        eventDataPtr[13].Size = 8U;
        eventDataPtr[14].DataPointer = (ulong) &sizeOnDiskBytes;
        eventDataPtr[14].Size = 8U;
        eventDataPtr[15].DataPointer = (ulong) &ioStallQueuedReadMs;
        eventDataPtr[15].Size = 8U;
        eventDataPtr[16].DataPointer = (ulong) &ioStallQueueWriteMs;
        eventDataPtr[16].Size = 8U;
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
          {
            eventDataPtr[1].DataPointer = (ulong) chPtr1;
            eventDataPtr[2].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateHostPreferredRegionUpdate(
      ref EventDescriptor eventDescriptor,
      Guid HostId,
      string PreferredRegion,
      string RegionUpdateType)
    {
      int dataCount = 3;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &HostId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((PreferredRegion.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((RegionUpdateType.Length + 1) * 2);
        fixed (char* chPtr1 = PreferredRegion)
          fixed (char* chPtr2 = RegionUpdateType)
          {
            eventDataPtr[1].DataPointer = (ulong) chPtr1;
            eventDataPtr[2].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    internal unsafe bool TemplateOrganizationTenant2(
      ref EventDescriptor eventDescriptor,
      Guid HostId,
      byte HostType,
      string HostName,
      Guid ParentHostId,
      byte ParentHostType,
      string ParentHostName,
      Guid TenantId,
      DateTime TenantLastModified,
      string PreferredRegion)
    {
      int dataCount = 9;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &HostId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].DataPointer = (ulong) &HostType;
        eventDataPtr[1].Size = 1U;
        eventDataPtr[2].Size = (uint) ((HostName.Length + 1) * 2);
        eventDataPtr[3].DataPointer = (ulong) &ParentHostId;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        eventDataPtr[4].DataPointer = (ulong) &ParentHostType;
        eventDataPtr[4].Size = 1U;
        eventDataPtr[5].Size = (uint) ((ParentHostName.Length + 1) * 2);
        eventDataPtr[6].DataPointer = (ulong) &TenantId;
        eventDataPtr[6].Size = (uint) sizeof (Guid);
        long fileTime = TenantLastModified.ToFileTime();
        eventDataPtr[7].DataPointer = (ulong) &fileTime;
        eventDataPtr[7].Size = 8U;
        eventDataPtr[8].Size = (uint) ((PreferredRegion.Length + 1) * 2);
        fixed (char* chPtr1 = HostName)
          fixed (char* chPtr2 = ParentHostName)
            fixed (char* chPtr3 = PreferredRegion)
            {
              eventDataPtr[2].DataPointer = (ulong) chPtr1;
              eventDataPtr[5].DataPointer = (ulong) chPtr2;
              eventDataPtr[8].DataPointer = (ulong) chPtr3;
              flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
            }
      }
      return flag;
    }

    internal unsafe bool TemplateDatabaseDetails5(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      int DatabaseId,
      string ServerName,
      string DatabaseName,
      long Version,
      string ServiceLevel,
      string PoolName,
      int PoolMaxDatabaseLimit,
      int Tenants,
      int MaxTenants,
      string Status,
      string StatusReason,
      DateTime StatusChangedDate,
      string Flags,
      string MinServiceObjective,
      string MaxServiceObjective,
      int RetentionDays,
      string ConnectionString,
      DateTime CreatedOn,
      string ServiceObjective,
      string BackupStorageRedundancy,
      bool IsZoneRedundant,
      string Collation,
      string Location,
      string DefaultSecondaryLocation,
      string ReadScale,
      int HighAvailabilityReplicaCount,
      double MaxSizeInGB,
      double MaxLogSizeInGB,
      string Kind)
    {
      int dataCount = 30;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].DataPointer = (ulong) &DatabaseId;
        eventDataPtr[1].Size = 4U;
        eventDataPtr[2].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[4].DataPointer = (ulong) &Version;
        eventDataPtr[4].Size = 8U;
        eventDataPtr[5].Size = (uint) ((ServiceLevel.Length + 1) * 2);
        eventDataPtr[6].Size = (uint) ((PoolName.Length + 1) * 2);
        eventDataPtr[7].DataPointer = (ulong) &PoolMaxDatabaseLimit;
        eventDataPtr[7].Size = 4U;
        eventDataPtr[8].DataPointer = (ulong) &Tenants;
        eventDataPtr[8].Size = 4U;
        eventDataPtr[9].DataPointer = (ulong) &MaxTenants;
        eventDataPtr[9].Size = 4U;
        eventDataPtr[10].Size = (uint) ((Status.Length + 1) * 2);
        eventDataPtr[11].Size = (uint) ((StatusReason.Length + 1) * 2);
        long fileTime1 = StatusChangedDate.ToFileTime();
        eventDataPtr[12].DataPointer = (ulong) &fileTime1;
        eventDataPtr[12].Size = 8U;
        eventDataPtr[13].Size = (uint) ((Flags.Length + 1) * 2);
        eventDataPtr[14].Size = (uint) ((MinServiceObjective.Length + 1) * 2);
        eventDataPtr[15].Size = (uint) ((MaxServiceObjective.Length + 1) * 2);
        eventDataPtr[16].DataPointer = (ulong) &RetentionDays;
        eventDataPtr[16].Size = 4U;
        eventDataPtr[17].Size = (uint) ((ConnectionString.Length + 1) * 2);
        long fileTime2 = CreatedOn.ToFileTime();
        eventDataPtr[18].DataPointer = (ulong) &fileTime2;
        eventDataPtr[18].Size = 8U;
        eventDataPtr[19].Size = (uint) ((ServiceObjective.Length + 1) * 2);
        eventDataPtr[20].Size = (uint) ((BackupStorageRedundancy.Length + 1) * 2);
        int num = IsZoneRedundant ? 1 : 0;
        eventDataPtr[21].DataPointer = (ulong) &num;
        eventDataPtr[21].Size = 4U;
        eventDataPtr[22].Size = (uint) ((Collation.Length + 1) * 2);
        eventDataPtr[23].Size = (uint) ((Location.Length + 1) * 2);
        eventDataPtr[24].Size = (uint) ((DefaultSecondaryLocation.Length + 1) * 2);
        eventDataPtr[25].Size = (uint) ((ReadScale.Length + 1) * 2);
        eventDataPtr[26].DataPointer = (ulong) &HighAvailabilityReplicaCount;
        eventDataPtr[26].Size = 4U;
        eventDataPtr[27].DataPointer = (ulong) &MaxSizeInGB;
        eventDataPtr[27].Size = 8U;
        eventDataPtr[28].DataPointer = (ulong) &MaxLogSizeInGB;
        eventDataPtr[28].Size = 8U;
        eventDataPtr[29].Size = (uint) ((Kind.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = ServiceLevel)
              fixed (char* chPtr4 = PoolName)
                fixed (char* chPtr5 = Status)
                  fixed (char* chPtr6 = StatusReason)
                    fixed (char* chPtr7 = Flags)
                      fixed (char* chPtr8 = MinServiceObjective)
                        fixed (char* chPtr9 = MaxServiceObjective)
                          fixed (char* chPtr10 = ConnectionString)
                            fixed (char* chPtr11 = ServiceObjective)
                              fixed (char* chPtr12 = BackupStorageRedundancy)
                                fixed (char* chPtr13 = Collation)
                                  fixed (char* chPtr14 = Location)
                                    fixed (char* chPtr15 = DefaultSecondaryLocation)
                                      fixed (char* chPtr16 = ReadScale)
                                        fixed (char* chPtr17 = Kind)
                                        {
                                          eventDataPtr[2].DataPointer = (ulong) chPtr1;
                                          eventDataPtr[3].DataPointer = (ulong) chPtr2;
                                          eventDataPtr[5].DataPointer = (ulong) chPtr3;
                                          eventDataPtr[6].DataPointer = (ulong) chPtr4;
                                          eventDataPtr[10].DataPointer = (ulong) chPtr5;
                                          eventDataPtr[11].DataPointer = (ulong) chPtr6;
                                          eventDataPtr[13].DataPointer = (ulong) chPtr7;
                                          eventDataPtr[14].DataPointer = (ulong) chPtr8;
                                          eventDataPtr[15].DataPointer = (ulong) chPtr9;
                                          eventDataPtr[17].DataPointer = (ulong) chPtr10;
                                          eventDataPtr[19].DataPointer = (ulong) chPtr11;
                                          eventDataPtr[20].DataPointer = (ulong) chPtr12;
                                          eventDataPtr[22].DataPointer = (ulong) chPtr13;
                                          eventDataPtr[23].DataPointer = (ulong) chPtr14;
                                          eventDataPtr[24].DataPointer = (ulong) chPtr15;
                                          eventDataPtr[25].DataPointer = (ulong) chPtr16;
                                          eventDataPtr[29].DataPointer = (ulong) chPtr17;
                                          flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                                        }
      }
      return flag;
    }

    internal unsafe bool TemplateDatabaseConnectionInfo(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string ServerName,
      string DatabaseName,
      bool IsReadOnly,
      string HostName,
      string ProgramName,
      int HostProcessId,
      int Count,
      int InactiveCount,
      string SampleText)
    {
      int dataCount = 10;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        int num = IsReadOnly ? 1 : 0;
        eventDataPtr[3].DataPointer = (ulong) &num;
        eventDataPtr[3].Size = 4U;
        eventDataPtr[4].Size = (uint) ((HostName.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((ProgramName.Length + 1) * 2);
        eventDataPtr[6].DataPointer = (ulong) &HostProcessId;
        eventDataPtr[6].Size = 4U;
        eventDataPtr[7].DataPointer = (ulong) &Count;
        eventDataPtr[7].Size = 4U;
        eventDataPtr[8].DataPointer = (ulong) &InactiveCount;
        eventDataPtr[8].Size = 4U;
        eventDataPtr[9].Size = (uint) ((SampleText.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = HostName)
              fixed (char* chPtr4 = ProgramName)
                fixed (char* chPtr5 = SampleText)
                {
                  eventDataPtr[1].DataPointer = (ulong) chPtr1;
                  eventDataPtr[2].DataPointer = (ulong) chPtr2;
                  eventDataPtr[4].DataPointer = (ulong) chPtr3;
                  eventDataPtr[5].DataPointer = (ulong) chPtr4;
                  eventDataPtr[9].DataPointer = (ulong) chPtr5;
                  flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                }
      }
      return flag;
    }

    internal unsafe bool TemplateOrchestrationActivityLog1(
      ref EventDescriptor eventDescriptor,
      Guid HostId,
      string OrchestrationId,
      string Name,
      string Version,
      DateTime QueueTime,
      DateTime StartTime,
      long ExecutionTime,
      long CPUExecutionTime,
      string ExceptionType,
      string ExceptionMessage,
      Guid ActivityId,
      Guid E2EID,
      long CPUCycles,
      long AllocatedBytes)
    {
      int dataCount = 14;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &HostId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((OrchestrationId.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((Name.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((Version.Length + 1) * 2);
        short* numPtr1 = stackalloc short[8];
        numPtr1[0] = (short) QueueTime.Year;
        numPtr1[1] = (short) QueueTime.Month;
        switch (QueueTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr1[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr1[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr1[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr1[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr1[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr1[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr1[2] = (short) 6;
            break;
        }
        numPtr1[3] = (short) QueueTime.Day;
        numPtr1[4] = (short) QueueTime.Hour;
        numPtr1[5] = (short) QueueTime.Minute;
        numPtr1[6] = (short) QueueTime.Second;
        numPtr1[7] = (short) QueueTime.Millisecond;
        eventDataPtr[4].DataPointer = (ulong) numPtr1;
        eventDataPtr[4].Size = 16U;
        short* numPtr2 = stackalloc short[8];
        numPtr2[0] = (short) StartTime.Year;
        numPtr2[1] = (short) StartTime.Month;
        switch (StartTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr2[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr2[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr2[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr2[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr2[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr2[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr2[2] = (short) 6;
            break;
        }
        numPtr2[3] = (short) StartTime.Day;
        numPtr2[4] = (short) StartTime.Hour;
        numPtr2[5] = (short) StartTime.Minute;
        numPtr2[6] = (short) StartTime.Second;
        numPtr2[7] = (short) StartTime.Millisecond;
        eventDataPtr[5].DataPointer = (ulong) numPtr2;
        eventDataPtr[5].Size = 16U;
        eventDataPtr[6].DataPointer = (ulong) &ExecutionTime;
        eventDataPtr[6].Size = 8U;
        eventDataPtr[7].DataPointer = (ulong) &CPUExecutionTime;
        eventDataPtr[7].Size = 8U;
        eventDataPtr[8].Size = (uint) ((ExceptionType.Length + 1) * 2);
        eventDataPtr[9].Size = (uint) ((ExceptionMessage.Length + 1) * 2);
        eventDataPtr[10].DataPointer = (ulong) &ActivityId;
        eventDataPtr[10].Size = (uint) sizeof (Guid);
        eventDataPtr[11].DataPointer = (ulong) &E2EID;
        eventDataPtr[11].Size = (uint) sizeof (Guid);
        eventDataPtr[12].DataPointer = (ulong) &CPUCycles;
        eventDataPtr[12].Size = 8U;
        eventDataPtr[13].DataPointer = (ulong) &AllocatedBytes;
        eventDataPtr[13].Size = 8U;
        fixed (char* chPtr1 = OrchestrationId)
          fixed (char* chPtr2 = Name)
            fixed (char* chPtr3 = Version)
              fixed (char* chPtr4 = ExceptionType)
                fixed (char* chPtr5 = ExceptionMessage)
                {
                  eventDataPtr[1].DataPointer = (ulong) chPtr1;
                  eventDataPtr[2].DataPointer = (ulong) chPtr2;
                  eventDataPtr[3].DataPointer = (ulong) chPtr3;
                  eventDataPtr[8].DataPointer = (ulong) chPtr4;
                  eventDataPtr[9].DataPointer = (ulong) chPtr5;
                  flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                }
      }
      return flag;
    }

    internal unsafe bool TemplateDatabaseCounters(
      ref EventDescriptor eventDescriptor,
      string ServerName,
      string DatabaseName,
      Guid HostId,
      Guid ProjectId,
      string CounterName,
      long CounterValue,
      int LeftOverPercent)
    {
      int dataCount = 7;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &HostId;
        eventDataPtr[2].Size = (uint) sizeof (Guid);
        eventDataPtr[3].DataPointer = (ulong) &ProjectId;
        eventDataPtr[3].Size = (uint) sizeof (Guid);
        eventDataPtr[4].Size = (uint) ((CounterName.Length + 1) * 2);
        eventDataPtr[5].DataPointer = (ulong) &CounterValue;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].DataPointer = (ulong) &LeftOverPercent;
        eventDataPtr[6].Size = 4U;
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = CounterName)
            {
              eventDataPtr->DataPointer = (ulong) chPtr1;
              eventDataPtr[1].DataPointer = (ulong) chPtr2;
              eventDataPtr[4].DataPointer = (ulong) chPtr3;
              flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
            }
      }
      return flag;
    }

    internal unsafe bool TemplateDatabaseIdentityColumns(
      ref EventDescriptor eventDescriptor,
      string ServerName,
      string DatabaseName,
      string SchemaName,
      string TableName,
      string IdentityColumnName,
      long IdentityColumnValue,
      string IdentityColumnDatatype,
      int LeftOverPercent)
    {
      int dataCount = 8;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((SchemaName.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((TableName.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((IdentityColumnName.Length + 1) * 2);
        eventDataPtr[5].DataPointer = (ulong) &IdentityColumnValue;
        eventDataPtr[5].Size = 8U;
        eventDataPtr[6].Size = (uint) ((IdentityColumnDatatype.Length + 1) * 2);
        eventDataPtr[7].DataPointer = (ulong) &LeftOverPercent;
        eventDataPtr[7].Size = 4U;
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = SchemaName)
              fixed (char* chPtr4 = TableName)
                fixed (char* chPtr5 = IdentityColumnName)
                  fixed (char* chPtr6 = IdentityColumnDatatype)
                  {
                    eventDataPtr->DataPointer = (ulong) chPtr1;
                    eventDataPtr[1].DataPointer = (ulong) chPtr2;
                    eventDataPtr[2].DataPointer = (ulong) chPtr3;
                    eventDataPtr[3].DataPointer = (ulong) chPtr4;
                    eventDataPtr[4].DataPointer = (ulong) chPtr5;
                    eventDataPtr[6].DataPointer = (ulong) chPtr6;
                    flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                  }
      }
      return flag;
    }

    internal unsafe bool TemplateDatabaseServicePrincipals(
      ref EventDescriptor eventDescriptor,
      string ServerName,
      string DatabaseName,
      int ServicePrincipalId,
      string ServicePrincipalName,
      string TypeDesc,
      string AuthenticationTypeDesc,
      DateTime CreateTime,
      DateTime ModifyTime,
      string StateDesc,
      string PermissionName)
    {
      int dataCount = 10;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &ServicePrincipalId;
        eventDataPtr[2].Size = 4U;
        eventDataPtr[3].Size = (uint) ((ServicePrincipalName.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((TypeDesc.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((AuthenticationTypeDesc.Length + 1) * 2);
        short* numPtr1 = stackalloc short[8];
        numPtr1[0] = (short) CreateTime.Year;
        numPtr1[1] = (short) CreateTime.Month;
        switch (CreateTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr1[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr1[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr1[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr1[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr1[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr1[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr1[2] = (short) 6;
            break;
        }
        numPtr1[3] = (short) CreateTime.Day;
        numPtr1[4] = (short) CreateTime.Hour;
        numPtr1[5] = (short) CreateTime.Minute;
        numPtr1[6] = (short) CreateTime.Second;
        numPtr1[7] = (short) CreateTime.Millisecond;
        eventDataPtr[6].DataPointer = (ulong) numPtr1;
        eventDataPtr[6].Size = 16U;
        short* numPtr2 = stackalloc short[8];
        numPtr2[0] = (short) ModifyTime.Year;
        numPtr2[1] = (short) ModifyTime.Month;
        switch (ModifyTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr2[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr2[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr2[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr2[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr2[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr2[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr2[2] = (short) 6;
            break;
        }
        numPtr2[3] = (short) ModifyTime.Day;
        numPtr2[4] = (short) ModifyTime.Hour;
        numPtr2[5] = (short) ModifyTime.Minute;
        numPtr2[6] = (short) ModifyTime.Second;
        numPtr2[7] = (short) ModifyTime.Millisecond;
        eventDataPtr[7].DataPointer = (ulong) numPtr2;
        eventDataPtr[7].Size = 16U;
        eventDataPtr[8].Size = (uint) ((StateDesc.Length + 1) * 2);
        eventDataPtr[9].Size = (uint) ((PermissionName.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = ServicePrincipalName)
              fixed (char* chPtr4 = TypeDesc)
                fixed (char* chPtr5 = AuthenticationTypeDesc)
                  fixed (char* chPtr6 = StateDesc)
                    fixed (char* chPtr7 = PermissionName)
                    {
                      eventDataPtr->DataPointer = (ulong) chPtr1;
                      eventDataPtr[1].DataPointer = (ulong) chPtr2;
                      eventDataPtr[3].DataPointer = (ulong) chPtr3;
                      eventDataPtr[4].DataPointer = (ulong) chPtr4;
                      eventDataPtr[5].DataPointer = (ulong) chPtr5;
                      eventDataPtr[8].DataPointer = (ulong) chPtr6;
                      eventDataPtr[9].DataPointer = (ulong) chPtr7;
                      flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                    }
      }
      return flag;
    }

    internal unsafe bool TemplateSqlRowLockInfo2(
      ref EventDescriptor eventDescriptor,
      Guid ExecutionId,
      string ServerName,
      string DatabaseName,
      bool IsReadOnly,
      string SchemaName,
      string TableName,
      string IndexName,
      long HobtId,
      int ObjectId)
    {
      int dataCount = 9;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &ExecutionId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        int num = IsReadOnly ? 1 : 0;
        eventDataPtr[3].DataPointer = (ulong) &num;
        eventDataPtr[3].Size = 4U;
        eventDataPtr[4].Size = (uint) ((SchemaName.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((TableName.Length + 1) * 2);
        eventDataPtr[6].Size = (uint) ((IndexName.Length + 1) * 2);
        eventDataPtr[7].DataPointer = (ulong) &HobtId;
        eventDataPtr[7].Size = 8U;
        eventDataPtr[8].DataPointer = (ulong) &ObjectId;
        eventDataPtr[8].Size = 4U;
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = SchemaName)
              fixed (char* chPtr4 = TableName)
                fixed (char* chPtr5 = IndexName)
                {
                  eventDataPtr[1].DataPointer = (ulong) chPtr1;
                  eventDataPtr[2].DataPointer = (ulong) chPtr2;
                  eventDataPtr[4].DataPointer = (ulong) chPtr3;
                  eventDataPtr[5].DataPointer = (ulong) chPtr4;
                  eventDataPtr[6].DataPointer = (ulong) chPtr5;
                  flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                }
      }
      return flag;
    }

    internal unsafe bool TemplateDatabasePrincipals(
      ref EventDescriptor eventDescriptor,
      string ServerName,
      string DatabaseName,
      int PrincipalId,
      string PrincipalName,
      string RoleName,
      string Permissions,
      string TypeDesc,
      DateTime CreateTime,
      DateTime ModifyTime)
    {
      int dataCount = 9;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[1].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[2].DataPointer = (ulong) &PrincipalId;
        eventDataPtr[2].Size = 4U;
        eventDataPtr[3].Size = (uint) ((PrincipalName.Length + 1) * 2);
        eventDataPtr[4].Size = (uint) ((RoleName.Length + 1) * 2);
        eventDataPtr[5].Size = (uint) ((Permissions.Length + 1) * 2);
        eventDataPtr[6].Size = (uint) ((TypeDesc.Length + 1) * 2);
        short* numPtr1 = stackalloc short[8];
        numPtr1[0] = (short) CreateTime.Year;
        numPtr1[1] = (short) CreateTime.Month;
        switch (CreateTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr1[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr1[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr1[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr1[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr1[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr1[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr1[2] = (short) 6;
            break;
        }
        numPtr1[3] = (short) CreateTime.Day;
        numPtr1[4] = (short) CreateTime.Hour;
        numPtr1[5] = (short) CreateTime.Minute;
        numPtr1[6] = (short) CreateTime.Second;
        numPtr1[7] = (short) CreateTime.Millisecond;
        eventDataPtr[7].DataPointer = (ulong) numPtr1;
        eventDataPtr[7].Size = 16U;
        short* numPtr2 = stackalloc short[8];
        numPtr2[0] = (short) ModifyTime.Year;
        numPtr2[1] = (short) ModifyTime.Month;
        switch (ModifyTime.DayOfWeek)
        {
          case DayOfWeek.Sunday:
            numPtr2[2] = (short) 0;
            break;
          case DayOfWeek.Monday:
            numPtr2[2] = (short) 1;
            break;
          case DayOfWeek.Tuesday:
            numPtr2[2] = (short) 2;
            break;
          case DayOfWeek.Wednesday:
            numPtr2[2] = (short) 3;
            break;
          case DayOfWeek.Thursday:
            numPtr2[2] = (short) 4;
            break;
          case DayOfWeek.Friday:
            numPtr2[2] = (short) 5;
            break;
          case DayOfWeek.Saturday:
            numPtr2[2] = (short) 6;
            break;
        }
        numPtr2[3] = (short) ModifyTime.Day;
        numPtr2[4] = (short) ModifyTime.Hour;
        numPtr2[5] = (short) ModifyTime.Minute;
        numPtr2[6] = (short) ModifyTime.Second;
        numPtr2[7] = (short) ModifyTime.Millisecond;
        eventDataPtr[8].DataPointer = (ulong) numPtr2;
        eventDataPtr[8].Size = 16U;
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = PrincipalName)
              fixed (char* chPtr4 = RoleName)
                fixed (char* chPtr5 = Permissions)
                  fixed (char* chPtr6 = TypeDesc)
                  {
                    eventDataPtr->DataPointer = (ulong) chPtr1;
                    eventDataPtr[1].DataPointer = (ulong) chPtr2;
                    eventDataPtr[3].DataPointer = (ulong) chPtr3;
                    eventDataPtr[4].DataPointer = (ulong) chPtr4;
                    eventDataPtr[5].DataPointer = (ulong) chPtr5;
                    eventDataPtr[6].DataPointer = (ulong) chPtr6;
                    flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
                  }
      }
      return flag;
    }

    internal unsafe bool TemplateXEventSessions(
      ref EventDescriptor eventDescriptor,
      int SessionId,
      string ServerName,
      string DatabaseName,
      string SessionName,
      bool IsEventFileTruncated,
      int BuffersLogged,
      int BuffersDropped,
      string EventFileName)
    {
      int dataCount = 8;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &SessionId;
        eventDataPtr->Size = 4U;
        eventDataPtr[1].Size = (uint) ((ServerName.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((DatabaseName.Length + 1) * 2);
        eventDataPtr[3].Size = (uint) ((SessionName.Length + 1) * 2);
        int num = IsEventFileTruncated ? 1 : 0;
        eventDataPtr[4].DataPointer = (ulong) &num;
        eventDataPtr[4].Size = 4U;
        eventDataPtr[5].DataPointer = (ulong) &BuffersLogged;
        eventDataPtr[5].Size = 4U;
        eventDataPtr[6].DataPointer = (ulong) &BuffersDropped;
        eventDataPtr[6].Size = 4U;
        eventDataPtr[7].Size = (uint) ((EventFileName.Length + 1) * 2);
        fixed (char* chPtr1 = ServerName)
          fixed (char* chPtr2 = DatabaseName)
            fixed (char* chPtr3 = SessionName)
              fixed (char* chPtr4 = EventFileName)
              {
                eventDataPtr[1].DataPointer = (ulong) chPtr1;
                eventDataPtr[2].DataPointer = (ulong) chPtr2;
                eventDataPtr[3].DataPointer = (ulong) chPtr3;
                eventDataPtr[7].DataPointer = (ulong) chPtr4;
                flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
              }
      }
      return flag;
    }

    internal unsafe bool TemplateGitThrottlingSettings(
      ref EventDescriptor eventDescriptor,
      int Size,
      int Tarpit,
      int WorkUnitSize)
    {
      int dataCount = 3;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &Size;
        eventDataPtr->Size = 4U;
        eventDataPtr[1].DataPointer = (ulong) &Tarpit;
        eventDataPtr[1].Size = 4U;
        eventDataPtr[2].DataPointer = (ulong) &WorkUnitSize;
        eventDataPtr[2].Size = 4U;
        flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
      }
      return flag;
    }

    internal unsafe bool TemplateHostPreferredGeographyUpdate(
      ref EventDescriptor eventDescriptor,
      Guid HostId,
      string PreferredGeography,
      string GeographyUpdateType)
    {
      int dataCount = 3;
      bool flag = true;
      if (this.IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
      {
        byte* data = stackalloc byte[sizeof (EventProviderVersionTwo.EventData) * dataCount];
        EventProviderVersionTwo.EventData* eventDataPtr = (EventProviderVersionTwo.EventData*) data;
        eventDataPtr->DataPointer = (ulong) &HostId;
        eventDataPtr->Size = (uint) sizeof (Guid);
        eventDataPtr[1].Size = (uint) ((PreferredGeography.Length + 1) * 2);
        eventDataPtr[2].Size = (uint) ((GeographyUpdateType.Length + 1) * 2);
        fixed (char* chPtr1 = PreferredGeography)
          fixed (char* chPtr2 = GeographyUpdateType)
          {
            eventDataPtr[1].DataPointer = (ulong) chPtr1;
            eventDataPtr[2].DataPointer = (ulong) chPtr2;
            flag = this.WriteEvent(ref eventDescriptor, dataCount, (IntPtr) (void*) data);
          }
      }
      return flag;
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    private struct EventData
    {
      [FieldOffset(0)]
      internal ulong DataPointer;
      [FieldOffset(8)]
      internal uint Size;
      [FieldOffset(12)]
      internal int Reserved;
    }
  }
}
