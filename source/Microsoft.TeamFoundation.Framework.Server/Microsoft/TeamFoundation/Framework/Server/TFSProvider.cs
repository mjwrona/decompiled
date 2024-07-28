// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TFSProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics.Eventing;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TFSProvider : IDisposable
  {
    internal EventProviderVersionTwo m_provider = new EventProviderVersionTwo(new Guid("80761876-6844-47d5-8106-f8ed2aa8687b"));
    protected EventDescriptor Event_Debug_Info8;
    protected EventDescriptor Event_Debug_Error8;
    protected EventDescriptor Event_Debug_Warning8;
    protected EventDescriptor Event_Debug_Verbose8;
    protected EventDescriptor Event_SQL_Info;
    protected EventDescriptor Event_JobAgentHistory_Info14;
    protected EventDescriptor Event_ActivityLog_Info38;
    protected EventDescriptor Event_CustomerIntelligence_Info5;
    protected EventDescriptor Event_Kpi_Info;
    protected EventDescriptor Event_MachinePool_RequestHistory7;
    protected EventDescriptor Event_HostHistory3;
    protected EventDescriptor Event_SqlRunningStatus6;
    protected EventDescriptor Event_DatabasePerformanceStatistics14;
    protected EventDescriptor Event_ServicingJobDetail;
    protected EventDescriptor Event_ServicingStepDetail2;
    protected EventDescriptor Event_KpiMetric_Info2;
    protected EventDescriptor Event_EventMetric_Info2;
    protected EventDescriptor Event_Commerce_Info;
    protected EventDescriptor Event_User_Info;
    protected EventDescriptor Event_Account_Info;
    protected EventDescriptor Event_Subscription_Info;
    protected EventDescriptor Event_TableSpaceUsage3;
    protected EventDescriptor Event_Licensing_Info;
    protected EventDescriptor Event_Identity_SessionToken;
    protected EventDescriptor Notification_Event_Info;
    protected EventDescriptor Event_QDS3;
    protected EventDescriptor Event_Identity_Reads;
    protected EventDescriptor Event_Identity_Token;
    protected EventDescriptor Event_Identity_Cache_Changes;
    protected EventDescriptor Event_Identity_Sql_Changes;
    protected EventDescriptor Event_Organization_Info;
    protected EventDescriptor Event_Directory_Member_Info;
    protected EventDescriptor Event_StorageMetricsTransactions5;
    protected EventDescriptor Event_StorageAnalyticsLogs;
    protected EventDescriptor Event_ResourceUtilization_Info3;
    protected EventDescriptor Event_HostingServiceCertficates_Info;
    protected EventDescriptor Event_SqlServerAlwaysOnHealthStats_Info;
    protected EventDescriptor Event_XEventData_Info6;
    protected EventDescriptor Event_MemoryClerks3;
    protected EventDescriptor Event_ResourceSemaphores3;
    protected EventDescriptor Event_QueryOptimizerMemoryGateways3;
    protected EventDescriptor Event_SQLPerformanceCounters3;
    protected EventDescriptor Event_HttpOutgoingRequests6;
    protected EventDescriptor Event_PackagingTraces3;
    protected EventDescriptor Event_GeoReplicationLinkStatus2;
    protected EventDescriptor Event_XEventData_RPCCompleted5;
    protected EventDescriptor Event_JobAgentJobStarted_Info;
    protected EventDescriptor Event_CloudServiceRoleDetails10;
    protected EventDescriptor Event_SurveyEvent2;
    protected EventDescriptor Event_TuningRecommendation2;
    protected EventDescriptor Event_ServiceBusActivity4;
    protected EventDescriptor Event_ClientTrace_Info2;
    protected EventDescriptor Event_ActivityLogCore_Info6;
    protected EventDescriptor Event_EuiiTrace;
    protected EventDescriptor Event_DetectedEuiiEvent;
    protected EventDescriptor Event_ActivityLogMapping_Info4;
    protected EventDescriptor Event_ServicePrincipalIsMember_Info;
    protected EventDescriptor Event_ServiceBusPublishMetadata2;
    protected EventDescriptor Event_EuiiUser_Info;
    protected EventDescriptor Event_OrchestrationLog_Info3;
    protected EventDescriptor Event_ServiceBusMetrics_Info;
    protected EventDescriptor Event_SQLSpinlocks_Info3;
    protected EventDescriptor Event_JobHistoryCore_Info;
    protected EventDescriptor Event_RedisCacheMetrics_Info;
    protected EventDescriptor Event_ServiceHostExtended_Info;
    protected EventDescriptor Event_FeatureFlagStatus_Info_HostVSID;
    protected EventDescriptor Event_DocDBMetrics_Info;
    protected EventDescriptor Event_AzureSearchMetrics_Info;
    protected EventDescriptor Event_DocDBRUMetrics_Info;
    protected EventDescriptor Event_VirtualFileStats_Info;
    protected EventDescriptor Event_HostPreferredRegionUpdate_Info;
    protected EventDescriptor Event_LowPriorityProductTrace_Info;
    protected EventDescriptor Event_OrganizationTenant_Info2;
    protected EventDescriptor Event_DatabaseDetails5;
    protected EventDescriptor Event_DatabaseConnectionInfo;
    protected EventDescriptor Event_OrchestrationActivityLog_Info1;
    protected EventDescriptor Event_DatabaseCounters_Info;
    protected EventDescriptor Event_DatabaseIdentityColumns_Info;
    protected EventDescriptor Event_DatabaseServicePrincipals_Info;
    protected EventDescriptor Event_SqlRowLockInfo_Info2;
    protected EventDescriptor Event_DatabasePrincipals_Info;
    protected EventDescriptor Event_XEventSessions_Info;
    protected EventDescriptor Event_GitThrottlingSettings_Info;
    protected EventDescriptor Event_HostPreferredGeographyUpdate_Info;

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.m_provider.Dispose();
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public TFSProvider()
    {
      this.Event_Debug_Info8 = new EventDescriptor(0, (byte) 7, (byte) 16, (byte) 4, (byte) 10, 1, -9223372036854775807L);
      this.Event_Debug_Error8 = new EventDescriptor(1, (byte) 7, (byte) 16, (byte) 2, (byte) 11, 1, -9223372036854775806L);
      this.Event_Debug_Warning8 = new EventDescriptor(2, (byte) 7, (byte) 16, (byte) 3, (byte) 12, 1, -9223372036854775800L);
      this.Event_Debug_Verbose8 = new EventDescriptor(3, (byte) 7, (byte) 16, (byte) 5, (byte) 13, 1, -9223372036854775804L);
      this.Event_SQL_Info = new EventDescriptor(4, (byte) 0, (byte) 16, (byte) 4, (byte) 10, (int) byte.MaxValue, -9223372036854775807L);
      this.Event_JobAgentHistory_Info14 = new EventDescriptor(5, (byte) 14, (byte) 16, (byte) 4, (byte) 10, 5, -9223372036854775807L);
      this.Event_ActivityLog_Info38 = new EventDescriptor(6, (byte) 37, (byte) 16, (byte) 4, (byte) 10, 6, -9223372036854775807L);
      this.Event_CustomerIntelligence_Info5 = new EventDescriptor(7, (byte) 4, (byte) 16, (byte) 4, (byte) 10, 7, -9223372036854775807L);
      this.Event_Kpi_Info = new EventDescriptor(8, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 8, -9223372036854775807L);
      this.Event_MachinePool_RequestHistory7 = new EventDescriptor(9, (byte) 6, (byte) 16, (byte) 4, (byte) 10, 9, -9223372036854775807L);
      this.Event_HostHistory3 = new EventDescriptor(10, (byte) 2, (byte) 16, (byte) 4, (byte) 10, 10, -9223372036854775807L);
      this.Event_SqlRunningStatus6 = new EventDescriptor(11, (byte) 5, (byte) 17, (byte) 4, (byte) 10, 11, 4611686018427387905L);
      this.Event_DatabasePerformanceStatistics14 = new EventDescriptor(12, (byte) 13, (byte) 16, (byte) 4, (byte) 10, 12, -9223372036854775807L);
      this.Event_ServicingJobDetail = new EventDescriptor(14, (byte) 0, (byte) 17, (byte) 4, (byte) 10, 14, 4611686018427387905L);
      this.Event_ServicingStepDetail2 = new EventDescriptor(15, (byte) 1, (byte) 17, (byte) 4, (byte) 10, 15, 4611686018427387905L);
      this.Event_KpiMetric_Info2 = new EventDescriptor(16, (byte) 1, (byte) 17, (byte) 4, (byte) 10, 16, 4611686018427387905L);
      this.Event_EventMetric_Info2 = new EventDescriptor(17, (byte) 1, (byte) 17, (byte) 4, (byte) 10, 17, 4611686018427387905L);
      this.Event_Commerce_Info = new EventDescriptor(19, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 19, -9223372036854775807L);
      this.Event_User_Info = new EventDescriptor(20, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 20, -9223372036854775807L);
      this.Event_Account_Info = new EventDescriptor(22, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 22, -9223372036854775807L);
      this.Event_Subscription_Info = new EventDescriptor(23, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 23, -9223372036854775807L);
      this.Event_TableSpaceUsage3 = new EventDescriptor(24, (byte) 2, (byte) 16, (byte) 4, (byte) 10, 24, -9223372036854775807L);
      this.Event_Licensing_Info = new EventDescriptor(25, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 25, -9223372036854775807L);
      this.Event_Identity_SessionToken = new EventDescriptor(26, (byte) 0, (byte) 17, (byte) 4, (byte) 10, 26, 4611686018427387905L);
      this.Notification_Event_Info = new EventDescriptor(27, (byte) 0, (byte) 16, (byte) 4, (byte) 10, (int) byte.MaxValue, -9223372036854775804L);
      this.Event_QDS3 = new EventDescriptor(28, (byte) 2, (byte) 17, (byte) 4, (byte) 10, 28, 4611686018427387905L);
      this.Event_Identity_Reads = new EventDescriptor(29, (byte) 0, (byte) 17, (byte) 4, (byte) 10, 29, 4611686018427387905L);
      this.Event_Identity_Token = new EventDescriptor(30, (byte) 0, (byte) 17, (byte) 4, (byte) 10, 30, 4611686018427387905L);
      this.Event_Identity_Cache_Changes = new EventDescriptor(31, (byte) 0, (byte) 17, (byte) 4, (byte) 10, 31, 4611686018427387905L);
      this.Event_Identity_Sql_Changes = new EventDescriptor(32, (byte) 0, (byte) 17, (byte) 4, (byte) 10, 32, 4611686018427387905L);
      this.Event_Organization_Info = new EventDescriptor(33, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 33, -9223372036854775807L);
      this.Event_Directory_Member_Info = new EventDescriptor(34, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 34, -9223372036854775807L);
      this.Event_StorageMetricsTransactions5 = new EventDescriptor(35, (byte) 4, (byte) 16, (byte) 4, (byte) 10, 35, -9223372036854775807L);
      this.Event_StorageAnalyticsLogs = new EventDescriptor(36, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 36, -9223372036854775807L);
      this.Event_ResourceUtilization_Info3 = new EventDescriptor(37, (byte) 2, (byte) 17, (byte) 4, (byte) 10, 37, 4611686018427387905L);
      this.Event_HostingServiceCertficates_Info = new EventDescriptor(39, (byte) 0, (byte) 17, (byte) 4, (byte) 10, 39, 4611686018427387905L);
      this.Event_SqlServerAlwaysOnHealthStats_Info = new EventDescriptor(40, (byte) 0, (byte) 17, (byte) 4, (byte) 10, 40, 4611686018427387905L);
      this.Event_XEventData_Info6 = new EventDescriptor(41, (byte) 5, (byte) 17, (byte) 4, (byte) 10, 41, 4611686018427387905L);
      this.Event_MemoryClerks3 = new EventDescriptor(42, (byte) 2, (byte) 17, (byte) 4, (byte) 10, 42, 4611686018427387905L);
      this.Event_ResourceSemaphores3 = new EventDescriptor(43, (byte) 2, (byte) 17, (byte) 4, (byte) 10, 43, 4611686018427387905L);
      this.Event_QueryOptimizerMemoryGateways3 = new EventDescriptor(44, (byte) 2, (byte) 17, (byte) 4, (byte) 10, 44, 4611686018427387905L);
      this.Event_SQLPerformanceCounters3 = new EventDescriptor(45, (byte) 2, (byte) 17, (byte) 4, (byte) 10, 45, 4611686018427387905L);
      this.Event_HttpOutgoingRequests6 = new EventDescriptor(46, (byte) 5, (byte) 17, (byte) 4, (byte) 10, 46, 4611686018427387905L);
      this.Event_PackagingTraces3 = new EventDescriptor(47, (byte) 3, (byte) 17, (byte) 4, (byte) 10, 47, 4611686018427387905L);
      this.Event_GeoReplicationLinkStatus2 = new EventDescriptor(48, (byte) 1, (byte) 17, (byte) 4, (byte) 10, 48, 4611686018427387905L);
      this.Event_XEventData_RPCCompleted5 = new EventDescriptor(49, (byte) 4, (byte) 17, (byte) 4, (byte) 10, 49, 4611686018427387905L);
      this.Event_JobAgentJobStarted_Info = new EventDescriptor(50, (byte) 1, (byte) 16, (byte) 4, (byte) 10, 50, -9223372036854775807L);
      this.Event_CloudServiceRoleDetails10 = new EventDescriptor(51, (byte) 9, (byte) 17, (byte) 4, (byte) 10, 51, 4611686018427387905L);
      this.Event_SurveyEvent2 = new EventDescriptor(52, (byte) 1, (byte) 17, (byte) 4, (byte) 10, 52, 4611686018427387905L);
      this.Event_TuningRecommendation2 = new EventDescriptor(53, (byte) 1, (byte) 17, (byte) 4, (byte) 10, 53, 4611686018427387905L);
      this.Event_ServiceBusActivity4 = new EventDescriptor(54, (byte) 3, (byte) 17, (byte) 4, (byte) 10, 54, 4611686018427387905L);
      this.Event_ClientTrace_Info2 = new EventDescriptor(55, (byte) 1, (byte) 17, (byte) 4, (byte) 10, 55, 4611686018427387905L);
      this.Event_ActivityLogCore_Info6 = new EventDescriptor(56, (byte) 5, (byte) 16, (byte) 4, (byte) 10, 56, -9223372036854775807L);
      this.Event_EuiiTrace = new EventDescriptor(57, (byte) 0, (byte) 17, (byte) 4, (byte) 10, 57, 4611686018427387905L);
      this.Event_DetectedEuiiEvent = new EventDescriptor(58, (byte) 0, (byte) 17, (byte) 4, (byte) 10, 58, 4611686018427387905L);
      this.Event_ActivityLogMapping_Info4 = new EventDescriptor(59, (byte) 3, (byte) 16, (byte) 4, (byte) 10, 59, -9223372036854775807L);
      this.Event_ServicePrincipalIsMember_Info = new EventDescriptor(60, (byte) 0, (byte) 17, (byte) 4, (byte) 10, (int) byte.MaxValue, 4611686018427387905L);
      this.Event_ServiceBusPublishMetadata2 = new EventDescriptor(61, (byte) 1, (byte) 17, (byte) 4, (byte) 10, 61, 4611686018427387905L);
      this.Event_EuiiUser_Info = new EventDescriptor(62, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 62, -9223372036854775807L);
      this.Event_OrchestrationLog_Info3 = new EventDescriptor(63, (byte) 2, (byte) 16, (byte) 4, (byte) 10, 63, -9223372036854775807L);
      this.Event_ServiceBusMetrics_Info = new EventDescriptor(64, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 64, -9223372036854775807L);
      this.Event_SQLSpinlocks_Info3 = new EventDescriptor(65, (byte) 2, (byte) 16, (byte) 4, (byte) 10, (int) byte.MaxValue, -9223372036854775807L);
      this.Event_JobHistoryCore_Info = new EventDescriptor(66, (byte) 0, (byte) 16, (byte) 4, (byte) 10, (int) byte.MaxValue, -9223372036854775807L);
      this.Event_RedisCacheMetrics_Info = new EventDescriptor(67, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 67, -9223372036854775807L);
      this.Event_ServiceHostExtended_Info = new EventDescriptor(68, (byte) 1, (byte) 16, (byte) 4, (byte) 10, (int) byte.MaxValue, -9223372036854775807L);
      this.Event_FeatureFlagStatus_Info_HostVSID = new EventDescriptor(69, (byte) 1, (byte) 16, (byte) 4, (byte) 10, 69, -9223372036854775807L);
      this.Event_DocDBMetrics_Info = new EventDescriptor(70, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 70, -9223372036854775807L);
      this.Event_AzureSearchMetrics_Info = new EventDescriptor(71, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 71, -9223372036854775807L);
      this.Event_DocDBRUMetrics_Info = new EventDescriptor(72, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 72, -9223372036854775807L);
      this.Event_VirtualFileStats_Info = new EventDescriptor(73, (byte) 0, (byte) 16, (byte) 4, (byte) 10, (int) byte.MaxValue, -9223372036854775807L);
      this.Event_HostPreferredRegionUpdate_Info = new EventDescriptor(74, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 74, -9223372036854775807L);
      this.Event_LowPriorityProductTrace_Info = new EventDescriptor(75, (byte) 0, (byte) 18, (byte) 4, (byte) 10, 75, 2305843009213693953L);
      this.Event_OrganizationTenant_Info2 = new EventDescriptor(76, (byte) 1, (byte) 16, (byte) 4, (byte) 10, 76, -9223372036854775807L);
      this.Event_DatabaseDetails5 = new EventDescriptor(77, (byte) 4, (byte) 17, (byte) 4, (byte) 10, 77, 4611686018427387905L);
      this.Event_DatabaseConnectionInfo = new EventDescriptor(78, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 78, -9223372036854775807L);
      this.Event_OrchestrationActivityLog_Info1 = new EventDescriptor(79, (byte) 1, (byte) 16, (byte) 4, (byte) 10, 79, -9223372036854775807L);
      this.Event_DatabaseCounters_Info = new EventDescriptor(80, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 80, -9223372036854775807L);
      this.Event_DatabaseIdentityColumns_Info = new EventDescriptor(81, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 81, -9223372036854775807L);
      this.Event_DatabaseServicePrincipals_Info = new EventDescriptor(82, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 82, -9223372036854775807L);
      this.Event_SqlRowLockInfo_Info2 = new EventDescriptor(83, (byte) 1, (byte) 16, (byte) 4, (byte) 10, 83, -9223372036854775807L);
      this.Event_DatabasePrincipals_Info = new EventDescriptor(84, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 84, -9223372036854775807L);
      this.Event_XEventSessions_Info = new EventDescriptor(85, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 85, -9223372036854775807L);
      this.Event_GitThrottlingSettings_Info = new EventDescriptor(86, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 86, -9223372036854775807L);
      this.Event_HostPreferredGeographyUpdate_Info = new EventDescriptor(87, (byte) 0, (byte) 16, (byte) 4, (byte) 10, 87, -9223372036854775807L);
    }

    public bool EventWriteEvent_Debug_Info8(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateInfo8(ref this.Event_Debug_Info8, TraceId, Tracepoint, ServiceHost, ContextId, ProcessName, Username, Service, Method, Area, Layer, UserAgent, Uri, Path, UserDefined, ExceptionType, Message, VSID, UniqueIdentifier, E2EID, CUID, TenantId, ProviderId, OrchestrationId, WebSiteId);
    }

    public bool EventWriteEvent_Debug_Error8(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateInfo8(ref this.Event_Debug_Error8, TraceId, Tracepoint, ServiceHost, ContextId, ProcessName, Username, Service, Method, Area, Layer, UserAgent, Uri, Path, UserDefined, ExceptionType, Message, VSID, UniqueIdentifier, E2EID, CUID, TenantId, ProviderId, OrchestrationId, WebSiteId);
    }

    public bool EventWriteEvent_Debug_Warning8(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateInfo8(ref this.Event_Debug_Warning8, TraceId, Tracepoint, ServiceHost, ContextId, ProcessName, Username, Service, Method, Area, Layer, UserAgent, Uri, Path, UserDefined, ExceptionType, Message, VSID, UniqueIdentifier, E2EID, CUID, TenantId, ProviderId, OrchestrationId, WebSiteId);
    }

    public bool EventWriteEvent_Debug_Verbose8(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateInfo8(ref this.Event_Debug_Verbose8, TraceId, Tracepoint, ServiceHost, ContextId, ProcessName, Username, Service, Method, Area, Layer, UserAgent, Uri, Path, UserDefined, ExceptionType, Message, VSID, UniqueIdentifier, E2EID, CUID, TenantId, ProviderId, OrchestrationId, WebSiteId);
    }

    public bool EventWriteEvent_SQL_Info(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateSQL(ref this.Event_SQL_Info, Database, Datasource, Operation, Retries, Success, TotalTime, ConnectTime, ExecutionTime, WaitTime, ErrorCode, ErrorMessage);
    }

    public bool EventWriteEvent_JobAgentHistory_Info14(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateJobAgentHistory15(ref this.Event_JobAgentHistory_Info14, Plugin, JobName, JobSource, JobId, QueueTime, StartTime, ExecutionTime, AgentId, Result, ResultMessage, QueuedReasons, QueueFlags, Priority, LogicalReads, PhysicalReads, CPUTime, ElapsedTime, Feature, SqlExecutionTime, SqlExecutionCount, RedisExecutionTime, RedisExecutionCount, AadExecutionTime, AadExecutionCount, BlobStorageExecutionTime, BlobStorageExecutionCount, TableStorageExecutionTime, TableStorageExecutionCount, ServiceBusExecutionTime, ServiceBusExecutionCount, VssClientExecutionTime, VssClientExecutionCount, SqlRetryExecutionTime, SqlRetryExecutionCount, SqlReadOnlyExecutionTime, SqlReadOnlyExecutionCount, CPUCycles, FinalSqlCommandExecutionTime, E2EID, AadGraphExecutionTime, AadGraphExecutionCount, AadTokenExecutionTime, AadTokenExecutionCount, DocDBExecutionTime, DocDBExecutionCount, DocDBRUsConsumed, AllocatedBytes, RequesterActivityId, RequesterVsid, CPUCyclesAsync, AllocatedBytesAsync);
    }

    public bool EventWriteEvent_ActivityLog_Info38(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateActivityLog38(ref this.Event_ActivityLog_Info38, HostId, ContextId, Application, Command, Status, Count, StartTime, ExecutionTime, IdentityName, IPAddress, UniqueIdentifier, UserAgent, CommandIdentifier, ExceptionType, ExceptionMessage, ActivityId, ResponseCode, VSID, TimeToFirstPage, ActivityStatus, ExecutionTimeThreshold, IsExceptionExpected, DelayTime, RelatedActivityId, LogicalReads, PhysicalReads, CPUTime, ElapsedTime, Feature, HostStartTime, HostType, ParentHostId, AnonymousIdentifier, SqlExecutionTime, SqlExecutionCount, RedisExecutionTime, RedisExecutionCount, AadExecutionTime, AadExecutionCount, BlobStorageExecutionTime, BlobStorageExecutionCount, TableStorageExecutionTime, TableStorageExecutionCount, ServiceBusExecutionTime, ServiceBusExecutionCount, VssClientExecutionTime, VssClientExecutionCount, SqlRetryExecutionTime, SqlRetryExecutionCount, SqlReadOnlyExecutionTime, SqlReadOnlyExecutionCount, CPUCycles, FinalSqlCommandExecutionTime, E2EID, PersistentSessionId, PendingAuthenticationSessionId, CurrentAuthenticationSessionId, CUID, TenantId, ProviderId, QueueTime, AuthenticationMechanism, TSTUs, AadGraphExecutionTime, AadGraphExecutionCount, AadTokenExecutionTime, AadTokenExecutionCount, ThrottleReason, Referrer, UriStem, SupportsPublicAccess, ConcurrencySemaphoreTime, AuthorizationId, MethodInformationTimeout, PreControllerTime, ControllerTime, PostControllerTime, OrchestrationId, DocDBExecutionTime, DocDBExecutionCount, DocDBRUsConsumed, AllocatedBytes, SmartRouterStatus, SmartRouterReason, SmartRouterTarget, OAuthAppId, WebSiteId);
    }

    public bool EventWriteEvent_CustomerIntelligence_Info5(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateCustomerIntelligence5(ref this.Event_CustomerIntelligence_Info5, Count, Properties, UniqueIdentifier, AnonymousIdentifier, HostId, ParentHostId, HostType, VSID, Area, Feature, Useragent, CUID, DataspaceType, DataspaceId, DataspaceVisibility, SupportsPublicAccess);
    }

    public bool EventWriteEvent_Kpi_Info(int Count, string Metrics) => !this.m_provider.IsEnabled() || this.m_provider.TemplateKpi(ref this.Event_Kpi_Info, Count, Metrics);

    public bool EventWriteEvent_MachinePool_RequestHistory7(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateMachinePoolRequestHistory7(ref this.Event_MachinePool_RequestHistory7, PoolType, PoolName, InstanceName, RequestId, HostId, QueuedTime, AssignedTime, StartTime, FinishTime, UnassignedTime, Outcome, Inputs, Outputs, TraceActivityId, MaxParallelism, ImageLabel, TimeoutSeconds, SlaSeconds, SlaStartTime, Tags, SubscriptionId, RequiredResourceVersion, SuspiciousActivity, OrchestrationId, PoolRegion, ImageVersion);
    }

    public bool EventWriteEvent_HostHistory3(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateHostHistory3(ref this.Event_HostHistory3, HostId, ModifiedDate, ActionType, ParentHostId, ServerName, DatabaseName, Authority, Status, StatusReason, SupportedFeatures, HostType, LastUserAccess, Name, DatabaseId, StorageAccountId, SubStatus);
    }

    public bool EventWriteEvent_SqlRunningStatus6(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateSqlRunningStatus6(ref this.Event_SqlRunningStatus6, ExecutionId, ServerName, DatabaseName, SessionId, Seconds, ElapsedTime, Command, BlockingSessionId, HeadBlockerSessionId, BlockingLevel, Text, Statement, BlockerQueryText, WaitType, WaitTime, LastWaitType, WaitResource, Reads, Writes, LogicalReads, CPUTime, GrantedQueryMemory, RequestedMemory, MaxUsedMemory, Dop, QueryPlan, IsReadOnly, Listener, LoginName);
    }

    public bool EventWriteEvent_DatabasePerformanceStatistics14(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDatabasePerformanceStatistics14(ref this.Event_DatabasePerformanceStatistics14, ExecutionId, ServerName, DatabaseName, PeriodStart, AverageCpuPercentage, MaximumCpuPercentage, AverageDataIOPercentage, MaximumDataIOPercentage, AverageLogWriteUtilizationPercentage, MaximumLogWriteUtilizationPercentage, AverageMemoryUsagePercentage, MaximumMemoryUsagePercentage, ServiceObjective, AverageWorkerPercentage, MaximumWorkerPercentage, AverageSessionPercentage, MaximumSessionPercentage, dtu_limit, PoolName, IsReadOnly, AverageXtpPercentage, MaximumXtpPercentage, ResourceVersion, Schedulers, Listener, AverageInstanceCpuPercentage, AverageInstanceMemoryPercentage, ReplicaRole, CompatibilityLevel, RedoQueueSizeKB, RedoRateKBPerSec, SecondaryLagSeconds, SynchronizationHealth, ServiceObjectiveHardware);
    }

    public bool EventWriteEvent_ServicingJobDetail(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateServicingJobDetail(ref this.Event_ServicingJobDetail, HostId, JobId, OperationClass, Operations, JobStatus, JobStatusDesc, Result, ResultDesc, QueueTime, StartTime, EndTime, DurationInSeconds, CompletedStepCount, TotalStepCount);
    }

    public bool EventWriteEvent_ServicingStepDetail2(
      DateTime DetailTime,
      string Message,
      string OperationName,
      string GroupName,
      string StepName,
      string EntryKind,
      Guid JobId,
      DateTime QueueTime)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateServicingStepDetail2(ref this.Event_ServicingStepDetail2, DetailTime, Message, OperationName, GroupName, StepName, EntryKind, JobId, QueueTime);
    }

    public bool EventWriteEvent_KpiMetric_Info2(
      DateTime EventTime,
      string Area,
      string Scope,
      string KpiMetricName,
      double KpiMetricValue,
      Guid HostId,
      string DisplayName,
      string Description)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateKpiMetric2(ref this.Event_KpiMetric_Info2, EventTime, Area, Scope, KpiMetricName, KpiMetricValue, HostId, DisplayName, Description);
    }

    public bool EventWriteEvent_EventMetric_Info2(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateEventMetric2(ref this.Event_EventMetric_Info2, EventTime, EventSource, EventMetricName, EventMetricValue, DatabaseId, DeploymentId, HostId, MachineName, RoleInstanceId, Scope, EventType, EventId);
    }

    public bool EventWriteEvent_Commerce_Info(string EntityTypeName, string DataFeed) => !this.m_provider.IsEnabled() || this.m_provider.TemplateCommerce(ref this.Event_Commerce_Info, EntityTypeName, DataFeed);

    public bool EventWriteEvent_User_Info(string EntityTypeName, string DataFeed) => !this.m_provider.IsEnabled() || this.m_provider.TemplateUser(ref this.Event_User_Info, EntityTypeName, DataFeed);

    public bool EventWriteEvent_Account_Info(string EntityTypeName, string DataFeed) => !this.m_provider.IsEnabled() || this.m_provider.TemplateAccount(ref this.Event_Account_Info, EntityTypeName, DataFeed);

    public bool EventWriteEvent_Subscription_Info(string EntityTypeName, string DataFeed) => !this.m_provider.IsEnabled() || this.m_provider.TemplateSubscription(ref this.Event_Subscription_Info, EntityTypeName, DataFeed);

    public bool EventWriteEvent_TableSpaceUsage3(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateTableSpaceUsage3(ref this.Event_TableSpaceUsage3, ExecutionId, ServerName, DatabaseName, SchemaName, TableName, IndexName, IndexType, Compression, IndexId, RowCount, UsedSpaceInMB, ReservedSpaceInMB, InRowUsedSpaceInMB, InRowReservedSpaceInMB, LobUsedSpaceInMB, LobReservedSpaceInMB, Listener, PartitionNumber, PartitionBoundary);
    }

    public bool EventWriteEvent_Licensing_Info(string EntityTypeName, string DataFeed) => !this.m_provider.IsEnabled() || this.m_provider.TemplateLicensing(ref this.Event_Licensing_Info, EntityTypeName, DataFeed);

    public bool EventWriteEvent_Identity_SessionToken(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateIdentity_SessionTokens(ref this.Event_Identity_SessionToken, Operation, SessionTokenType, Error, ClientId, AccessId, AuthorizationId, HostAuthorizationId, UserId, ValidFrom, ValidTo, DisplayName, Scope, TargetAccounts, IsValid, IsPublic, PublicData, Source);
    }

    public bool EventWriteNotification_Event_Info(
      DateTime CreatedDate,
      string EventTaskName,
      Guid HostId,
      string EventType,
      string Identifier,
      string DataFeed)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateNotification(ref this.Notification_Event_Info, CreatedDate, EventTaskName, HostId, EventType, Identifier, DataFeed);
    }

    public bool EventWriteEvent_QDS3(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateQDS3(ref this.Event_QDS3, RunId, ServerName, DatabaseName, PeriodStart, PeriodEnd, QueryText, QueryId, ObjectName, QueryTextId, PlanId, TotalPhysicalReads, TotalCpuTime, AverageRowCount, TotalExecutions, TotalLogicalReads, AverageCpuTime, TotalAborted, TotalExceptions, TotalLogicalWrites, AverageDop, AverageQueryMaxUsedMemory, QueryHash, QueryPlanHash, Listener);
    }

    public bool EventWriteEvent_Identity_Reads(
      string ClassName,
      string Flavor,
      string Identifier,
      string QueryMembership,
      string PropertyNameFilters,
      string Options,
      string CallStack)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateIdentity_Reads(ref this.Event_Identity_Reads, ClassName, Flavor, Identifier, QueryMembership, PropertyNameFilters, Options, CallStack);
    }

    public bool EventWriteEvent_Identity_Token(
      string ClassName,
      string Header,
      string Claims,
      string Nonce)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateIdentity_Token(ref this.Event_Identity_Token, ClassName, Header, Claims, Nonce);
    }

    public bool EventWriteEvent_Identity_Cache_Changes(
      string StoreType,
      string EventType,
      string SearchFilter,
      string DomainId,
      string EventValue,
      string QueryMembership,
      string CacheResult,
      string CallStack)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateIdentity_Cache_Changes(ref this.Event_Identity_Cache_Changes, StoreType, EventType, SearchFilter, DomainId, EventValue, QueryMembership, CacheResult, CallStack);
    }

    public bool EventWriteEvent_Identity_Sql_Changes(
      string EventType,
      string DomainId,
      string EventValue,
      string QueryMembership,
      string CallStack)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateIdentity_Sql_Changes(ref this.Event_Identity_Sql_Changes, EventType, DomainId, EventValue, QueryMembership, CallStack);
    }

    public bool EventWriteEvent_Organization_Info(string EntityTypeName, string DataFeed) => !this.m_provider.IsEnabled() || this.m_provider.TemplateOrganization(ref this.Event_Organization_Info, EntityTypeName, DataFeed);

    public bool EventWriteEvent_Directory_Member_Info(string EntityTypeName, string DataFeed) => !this.m_provider.IsEnabled() || this.m_provider.TemplateDirectory_Member(ref this.Event_Directory_Member_Info, EntityTypeName, DataFeed);

    public bool EventWriteEvent_StorageMetricsTransactions5(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateStorageMetricsTransactions5(ref this.Event_StorageMetricsTransactions5, ExecutionId, StorageAccount, StorageService, StartingIntervalUTC, TotalIngress, TotalEgress, TotalRequests, TotalBillableRequests, Availability, AverageE2ELatency, AverageServerLatency, PercentSuccess, PercentThrottlingError, PercentTimeoutError, PercentServerOtherError, PercentClientOtherError, PercentAuthorizationError, PercentNetworkError, Success, AnonymousSuccess, SasSuccess, ThrottlingError, AnonymousThrottlingError, SasThrottlingError, ClientTimeoutError, AnonymousClientTimeoutError, SasClientTimeoutError, ServerTimeoutError, AnonymousServerTimeoutError, SasServerTimeoutError, ClientOtherError, SasClientOtherError, AnonymousClientOtherError, ServerOtherError, AnonymousServerOtherError, SasServerOtherError, AuthorizationError, AnonymousAuthorizationError, SasAuthorizationError, NetworkError, AnonymousNetworkError, SasNetworkError, OperationType, StorageCluster, StorageKind, StorageSku, BlobGeoLastSyncTime, TableGeoLastSyncTime, QueueGeoLastSyncTime);
    }

    public bool EventWriteEvent_StorageAnalyticsLogs(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateStorageAnalyticsLogs(ref this.Event_StorageAnalyticsLogs, ExecutionId, StorageAccount, StorageService, VersionNumber, RequestStartTime, OperationType, RequestStatus, HttpStatusCode, EndtoEndLatencyInMs, ServerLatencyInMs, AuthenticationType, RequesterAccountName, OwnerAccountName, RequestUrl, RequestedObjectKey, RequestIdHeader, OperationCount, RequesterIpAddress, RequestVersionHeader, RequestHeaderSize, RequestPacketSize, ResponseHeaderSize, ResponsePacketSize, RequestContentLength, RequestMd5, ServerMd5, EtagIdentifier, LastModifiedTime, ConditionsUsed, UserAgentHeader, ReferrerHeader, ClientRequestId);
    }

    public bool EventWriteEvent_ResourceUtilization_Info3(
      DateTime StartTime,
      Guid HostId,
      Guid VSID,
      Guid ActivityId,
      string DataFeed,
      Guid CUID,
      int Channel)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateResourceUtilization3(ref this.Event_ResourceUtilization_Info3, StartTime, HostId, VSID, ActivityId, DataFeed, CUID, Channel);
    }

    public bool EventWriteEvent_HostingServiceCertficates_Info(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateHostingServiceCertficates(ref this.Event_HostingServiceCertficates_Info, ExecutionId, Source, Thumbprint, SerialNumber, SubjectName, IssuerName, SignatureAlgorithm, CreatedDate, ExpiryDate);
    }

    public bool EventWriteEvent_SqlServerAlwaysOnHealthStats_Info(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateSqlServerAlwaysOnHealthStats(ref this.Event_SqlServerAlwaysOnHealthStats_Info, ExecutionId, ListenerName, GroupId, GroupName, ReplicaId, ReplicaServerName, ReplicaDatabaseId, DatabaseName, ConnectedStateDesc, AvailabilityModeDesc, SynchronizationStateDesc, ReplicaRoleDesc, IsLocal, IsJoined, IsSuspended, SuspendReasonDesc, IsFailoverReady, EstimatedDataLossIfFailoverNotReadyInSec, EstimatedRecoveryTime, FileStreamSendRate, LogSendQueueSize, LogSendRate, RedoQueueSize, RedoRate, SynchronizationPerformance, LastCommitLsn, LastCommitTime, LastHardenedLsn, LastHardenedTime, LastReceivedLsn, LastReceivedTime, LastSentLsn, LastSentTime, LastRedoneLsn, LastRedoneTime, EndOfLogLsn, RecoveryLsn, TruncationLsn, CollectionTime);
    }

    public bool EventWriteEvent_XEventData_Info6(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateXEventData6(ref this.Event_XEventData_Info6, EventTime, SequenceNumber, ActivityId, UniqueIdentifier, HostId, VSID, Type, ObjectName, Actions, Fields, ServerName, DatabaseName, XEventTypeName);
    }

    public bool EventWriteEvent_MemoryClerks3(
      Guid ExecutionId,
      string ServerName,
      string DatabaseName,
      string ClerkName,
      long SizeKB,
      bool IsReadonly,
      string Listener)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateMemoryClerks3(ref this.Event_MemoryClerks3, ExecutionId, ServerName, DatabaseName, ClerkName, SizeKB, IsReadonly, Listener);
    }

    public bool EventWriteEvent_ResourceSemaphores3(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateResourceSemaphores3(ref this.Event_ResourceSemaphores3, ExecutionId, ServerName, DatabaseName, ResourceSemaphoreId, TargetMemoryKB, MaxTargetMemoryKB, TotalMemoryKB, AvailableMemoryKB, GrantedMemoryKB, UsedMemoryKB, GranteeCount, WaiterCount, TimeoutErrorCount, ForcedGrantCount, PoolId, IsReadonly, Listener);
    }

    public bool EventWriteEvent_QueryOptimizerMemoryGateways3(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateQueryOptimizerMemoryGateways3(ref this.Event_QueryOptimizerMemoryGateways3, ExecutionId, ServerName, DatabaseName, PoolId, MaxCount, ActiveCount, WaiterCount, ThresholdFactor, Threshold, IsActive, IsReadonly, Listener);
    }

    public bool EventWriteEvent_SQLPerformanceCounters3(
      Guid ExecutionId,
      string ServerName,
      string DatabaseName,
      string CounterName,
      long CounterValue,
      bool IsReadonly,
      string Listener)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateSQLPerformanceCounters3(ref this.Event_SQLPerformanceCounters3, ExecutionId, ServerName, DatabaseName, CounterName, CounterValue, IsReadonly, Listener);
    }

    public bool EventWriteEvent_HttpOutgoingRequests6(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateHttpOutgoingRequests6(ref this.Event_HttpOutgoingRequests6, StartTime, TimeTaken, HttpClientName, HttpMethod, UrlHost, UrlPath, ResponseCode, ErrorMessage, E2EID, AfdRefInfo, RequestPriority, CalledActivityId, RequestPhase, OrchestrationId, TokenRetries, HandlerStartTime, BufferedRequestTime, RequestSendTime, ResponseContentTime, GetTokenTime, TrailingTime);
    }

    public bool EventWriteEvent_PackagingTraces3(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplatePackagingTraces3(ref this.Event_PackagingTraces3, ActivityId, CollectionHostName, HostId, ProjectId, Protocol, Command, FeedId, FeedName, ViewId, ViewName, PackageName, PackageVersion, ResponseCode, IsSlow, TimeToFirstPageInMs, ExecutionTimeInMs, QueueTimeInMs, IsFailed, ExceptionType, ExceptionMessage, IdentityName, UserAgent, ClientSessionId, RefererHeader, SourceIp, HostIp, BuildNumber, CommitId, DataCurrentVersion, DataDestinationVersion, DataMigrationState, FeatureFlagsOn, FeatureFlagsOff, StackTrace, TimingsTrace, Uri, HttpMethod, RelatedActivityId, E2EID, Properties, PackageStorageId, ProjectVisibility, OrchestrationId);
    }

    public bool EventWriteEvent_GeoReplicationLinkStatus2(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateGeoReplicationLinkStatus2(ref this.Event_GeoReplicationLinkStatus2, ExecutionId, ServerName, DatabaseName, LinkGuid, PartnerServer, PartnerDatabase, LastReplication, ReplicationLagSec, ReplicationState, ReplicationStateDescription, Role, RoleDescription, SecondaryAllowConnections, SecondaryAllowConnectionsDescription, LastCommit, Listener);
    }

    public bool EventWriteEvent_XEventData_RPCCompleted5(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateXEventDataRPCCompleted5(ref this.Event_XEventData_RPCCompleted5, EventTime, SequenceNumber, ActivityId, UniqueIdentifier, HostId, VSID, Type, ObjectName, CpuTime, Duration, PhysicalReads, LogicalReads, Writes, Result, RowCount, ConnectionResetOption, Statement, TSTUs, ServerName, DatabaseName, IsGoverned, IsReadScaleOut);
    }

    public bool EventWriteEvent_JobAgentJobStarted_Info(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateJobAgentJobStarted2(ref this.Event_JobAgentJobStarted_Info, Plugin, JobName, JobSource, JobId, QueueTime, StartTime, AgentId, QueuedReasons, QueueFlags, Priority, Feature, E2EID, RequesterActivityId, RequesterVsid);
    }

    public bool EventWriteEvent_CloudServiceRoleDetails10(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateCloudServiceRoleDetails10(ref this.Event_CloudServiceRoleDetails10, ExecutionId, AzureSubscriptionId, AzureSubscriptionAadTenantId, RoleType, RoleCountMin, RoleCount, RoleCountMax, RoleSize, RoleCores, RoleMemoryMB, HostedServiceDnsName, BuildNumber, OSImageVersion, DeploymentOsVersion, DeploymentHotfixes, EncryptionAtHost, SecurityType, SecureBootEnabled, VTpmEnabled, OSDiskStorageAccountType, OSDiskSizeInGB, DeploymentRing, WeekdayPeakRoleCountMin, WeekdayPeakRoleCountMax, WeekdayPeakStartTime, WeekdayPeakEndTime, WeekendPeakRoleCountMin, WeekendPeakRoleCountMax, WeekendPeakStartTime, WeekendPeakEndTime, Zones);
    }

    public bool EventWriteEvent_SurveyEvent2(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateSurveyEvent2(ref this.Event_SurveyEvent2, UniqueIdentifier, AnonymousIdentifier, TenantId, HostId, ParentHostId, HostType, VSID, CUID, Area, Feature, UserAgent, Properties, DataspaceType, DataspaceId, DataspaceVisibility, SupportsPublicAccess);
    }

    public bool EventWriteEvent_TuningRecommendation2(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateTuningRecommendation2(ref this.Event_TuningRecommendation2, RunId, ServerName, DatabaseName, Name, Type, Reason, ValidSince, LastRefresh, State, IsExecutableAction, IsRevertableAction, ExecuteActionStartTime, ExecuteActionDurationMilliseconds, ExecuteActionInitiatedBy, ExecuteActionInitiatedTime, RevertActionStartTime, RevertActionDurationMilliseconds, RevertActionInitiatedBy, RevertActionInitiatedTime, Score, Details, Listener);
    }

    public bool EventWriteEvent_ServiceBusActivity4(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateServiceBusActivity4(ref this.Event_ServiceBusActivity4, HostId, HostType, TopicName, Plugin, SourceInstanceId, SourceInstanceType, Status, ExceptionType, ExceptionMessage, StartTime, LogicalReads, PhysicalReads, ActivityId, E2EID, UniqueIdentifier, CPUTime, ElapsedTime, CPUCycles, SqlExecutionTime, SqlExecutionCount, RedisExecutionTime, RedisExecutionCount, AadGraphExecutionTime, AadGraphExecutionCount, AadTokenExecutionTime, AadTokenExecutionCount, BlobStorageExecutionTime, BlobStorageExecutionCount, TableStorageExecutionTime, TableStorageExecutionCount, ServiceBusExecutionTime, ServiceBusExecutionCount, VssClientExecutionTime, VssClientExecutionCount, SqlRetryExecutionTime, SqlRetryExecutionCount, SqlReadOnlyExecutionTime, SqlReadOnlyExecutionCount, FinalSqlCommandExecutionTime, MessageId, Namespace, ContentType);
    }

    public bool EventWriteEvent_ClientTrace_Info2(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateClientTrace2(ref this.Event_ClientTrace_Info2, Properties, UniqueIdentifier, AnonymousIdentifier, HostId, ParentHostId, HostType, VSID, Area, Feature, Useragent, CUID, Method, Component, Message, ExceptionType, E2EID, TenantId, ProviderId, StartTime);
    }

    public bool EventWriteEvent_ActivityLogCore_Info6(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateActivityLogCore6(ref this.Event_ActivityLogCore_Info6, HostId, Application, Command, Status, Count, StartTime, ExecutionTime, UserAgent, ExceptionType, VSID, TimeToFirstPage, ActivityStatus, IsExceptionExpected, Feature, HostType, ParentHostId, AnonymousIdentifier, CUID, TenantId, ActivityId, UniqueIdentifier, CPUTime, ElapsedTime, DelayTime, SqlExecutionTime, SqlExecutionCount, RedisExecutionTime, RedisExecutionCount, AadExecutionTime, AadExecutionCount, BlobStorageExecutionTime, BlobStorageExecutionCount, TableStorageExecutionTime, TableStorageExecutionCount, ServiceBusExecutionTime, ServiceBusExecutionCount, VssClientExecutionTime, VssClientExecutionCount, SqlRetryExecutionTime, SqlRetryExecutionCount, SqlReadOnlyExecutionTime, SqlReadOnlyExecutionCount, SupportsPublicAccess, PendingAuthenticationSessionId, UriStem);
    }

    public bool EventWriteEvent_EuiiTrace(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateEuiiTrace(ref this.Event_EuiiTrace, Properties, UniqueIdentifier, AnonymousIdentifier, HostId, ParentHostId, HostType, VSID, Area, Feature, Useragent, CUID, Method, Uri, Component, Message, ExceptionType, E2EID, TenantId, ProviderId, StartTime);
    }

    public bool EventWriteEvent_DetectedEuiiEvent(
      DateTime EventTime,
      string Source,
      int EuiiType,
      string Message)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDetectedEuiiEvent(ref this.Event_DetectedEuiiEvent, EventTime, Source, EuiiType, Message);
    }

    public bool EventWriteEvent_ActivityLogMapping_Info4(
      Guid ActivityId,
      Guid E2EID,
      string IdentityName,
      string IPAddress,
      string AnonymousIdentifier,
      Guid CUID,
      Guid VSID,
      DateTime StartTime)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateActivityLogMapping4(ref this.Event_ActivityLogMapping_Info4, ActivityId, E2EID, IdentityName, IPAddress, AnonymousIdentifier, CUID, VSID, StartTime);
    }

    public bool EventWriteEvent_ServicePrincipalIsMember_Info(
      Guid ServicePrincipalId,
      string GroupSid,
      byte HostType,
      string StackTrace,
      int ExecutionCount)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateServicePrincipalIsMember(ref this.Event_ServicePrincipalIsMember_Info, ServicePrincipalId, GroupSid, HostType, StackTrace, ExecutionCount);
    }

    public bool EventWriteEvent_ServiceBusPublishMetadata2(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateServiceBusPublishMetadata2(ref this.Event_ServiceBusPublishMetadata2, HostId, HostType, TopicName, MessageId, TargetScaleUnits, Status, StartTime, PublishTimeMs, ActivityId, E2EID, UniqueIdentifier, Namespace, ContentType);
    }

    public bool EventWriteEvent_EuiiUser_Info(string EntityTypeName, string DataFeed) => !this.m_provider.IsEnabled() || this.m_provider.TemplateEuiiUser(ref this.Event_EuiiUser_Info, EntityTypeName, DataFeed);

    public bool EventWriteEvent_OrchestrationLog_Info3(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateOrchestrationLog3(ref this.Event_OrchestrationLog_Info3, OrchestrationId, StartTime, EndTime, ExecutionTimeThreshold, OrchestrationStatus, Application, Feature, Command, ExceptionType, ExceptionMessage, IsExceptionExpected, HostType, HostId, ParentHostId, VSID, CUID, AnonymousIdentifier);
    }

    public bool EventWriteEvent_ServiceBusMetrics_Info(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateServiceBusMetrics(ref this.Event_ServiceBusMetrics_Info, ExecutionId, ServiceBusNamespace, SKUTier, StartingIntervalUTC, TotalSuccessfulRequests, TotalServerErrors, TotalUserErrors, TotalThrottledRequests, TotalIncomingRequests, TotalIncomingMessages, TotalOutgoingMessages, TotalActiveConnections, AverageSizeInBytes, AverageMessages, AverageActiveMessages);
    }

    public bool EventWriteEvent_SQLSpinlocks_Info3(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateSQLSpinlocks3(ref this.Event_SQLSpinlocks_Info3, ExecutionId, ServerName, DatabaseName, SpinlockName, Collisions, Spins, SpinsPerCollision, SleepTime, Backoffs, IsReadOnly, Listener);
    }

    public bool EventWriteEvent_JobHistoryCore_Info(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateJobHistoryCore(ref this.Event_JobHistoryCore_Info, Plugin, JobName, JobSource, JobId, QueueTime, StartTime, ExecutionTime, AgentId, Result, QueuedReasons, QueueFlags, Priority, LogicalReads, PhysicalReads, CPUTime, ElapsedTime, Feature, SqlExecutionTime, SqlExecutionCount, RedisExecutionTime, RedisExecutionCount, AadExecutionTime, AadExecutionCount, BlobStorageExecutionTime, BlobStorageExecutionCount, TableStorageExecutionTime, TableStorageExecutionCount, ServiceBusExecutionTime, ServiceBusExecutionCount, VssClientExecutionTime, VssClientExecutionCount, SqlRetryExecutionTime, SqlRetryExecutionCount, SqlReadOnlyExecutionTime, SqlReadOnlyExecutionCount, CPUCycles, FinalSqlCommandExecutionTime, E2EID, AadGraphExecutionTime, AadGraphExecutionCount, AadTokenExecutionTime, AadTokenExecutionCount);
    }

    public bool EventWriteEvent_RedisCacheMetrics_Info(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateRedisCacheMetrics(ref this.Event_RedisCacheMetrics_Info, ExecutionId, RedisCacheInstance, SKUTier, StartingIntervalUTC, TotalConnectedClients, TotalCommandsProcessed, TotalCacheHits, TotalCacheMisses, TotalUsedMemory, TotalUsedMemoryRss, TotalServerLoad, TotalProcessorTime, TotalOperationsPerSecond, TotalGetCommands, TotalSetCommands, TotalEvictedKeys, TotalTotalKeys, TotalExpiredKeys, TotalUsedMemoryPercentage, TotalCacheRead, TotalErrors);
    }

    public bool EventWriteEvent_ServiceHostExtended_Info(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateServiceHostExtended(ref this.Event_ServiceHostExtended_Info, HostId, HostType, ParentHostId, HostName, DatabaseServerName, DatabaseName, Status, StatusReason, LastUserAccess, IsDeleted);
    }

    public bool EventWriteEvent_FeatureFlagStatus_Info_HostVSID(
      long RunId,
      string FeatureFlagName,
      string EffectiveState,
      string ExplicitState,
      string HostId,
      string VSID)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateFeatureFlagStatusHostVSID(ref this.Event_FeatureFlagStatus_Info_HostVSID, RunId, FeatureFlagName, EffectiveState, ExplicitState, HostId, VSID);
    }

    public bool EventWriteEvent_DocDBMetrics_Info(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDocDBStorageMetrics(ref this.Event_DocDBMetrics_Info, ExecutionId, AccountName, DatabaseCategory, DatabaseId, CollectionId, CollectionSizeUsage, CollectionSizeQuota, PartitionKeyRangeId, PartitionKeyRangeDocumentCount, PartitionKeyRangeSize, PartitionKeyRangeDominantPartitionKeys);
    }

    public bool EventWriteEvent_AzureSearchMetrics_Info(
      Guid ExecutionId,
      string AzureSearchInstance,
      string SKUTier,
      DateTime StartingIntervalUTC,
      double SearchLatency,
      double QueriesPerSecond,
      double ThrottledSearchQueriesPercentage)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateAzureSearchMetrics(ref this.Event_AzureSearchMetrics_Info, ExecutionId, AzureSearchInstance, SKUTier, StartingIntervalUTC, SearchLatency, QueriesPerSecond, ThrottledSearchQueriesPercentage);
    }

    public bool EventWriteEvent_DocDBRUMetrics_Info(
      string AccountName,
      string DatabaseCategory,
      string DatabaseId,
      string CollectionId,
      string DocumentType,
      long ConsumedRUs)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDocDBRUMetrics(ref this.Event_DocDBRUMetrics_Info, AccountName, DatabaseCategory, DatabaseId, CollectionId, DocumentType, ConsumedRUs);
    }

    public bool EventWriteEvent_VirtualFileStats_Info(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateVirtualFileStats(ref this.Event_VirtualFileStats_Info, ExecutionId, ServerName, DatabaseName, ReadOnly, databaseId, fileId, sampleMs, numReads, numBytesRead, ioStallReadMs, numWrites, numBytesWritten, ioStallWriteMs, ioStall, sizeOnDiskBytes, ioStallQueuedReadMs, ioStallQueueWriteMs);
    }

    public bool EventWriteEvent_HostPreferredRegionUpdate_Info(
      Guid HostId,
      string PreferredRegion,
      string RegionUpdateType)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateHostPreferredRegionUpdate(ref this.Event_HostPreferredRegionUpdate_Info, HostId, PreferredRegion, RegionUpdateType);
    }

    public bool EventWriteEvent_LowPriorityProductTrace_Info(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateInfo8(ref this.Event_LowPriorityProductTrace_Info, TraceId, Tracepoint, ServiceHost, ContextId, ProcessName, Username, Service, Method, Area, Layer, UserAgent, Uri, Path, UserDefined, ExceptionType, Message, VSID, UniqueIdentifier, E2EID, CUID, TenantId, ProviderId, OrchestrationId, WebSiteId);
    }

    public bool EventWriteEvent_OrganizationTenant_Info2(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateOrganizationTenant2(ref this.Event_OrganizationTenant_Info2, HostId, HostType, HostName, ParentHostId, ParentHostType, ParentHostName, TenantId, TenantLastModified, PreferredRegion);
    }

    public bool EventWriteEvent_DatabaseDetails5(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDatabaseDetails5(ref this.Event_DatabaseDetails5, ExecutionId, DatabaseId, ServerName, DatabaseName, Version, ServiceLevel, PoolName, PoolMaxDatabaseLimit, Tenants, MaxTenants, Status, StatusReason, StatusChangedDate, Flags, MinServiceObjective, MaxServiceObjective, RetentionDays, ConnectionString, CreatedOn, ServiceObjective, BackupStorageRedundancy, IsZoneRedundant, Collation, Location, DefaultSecondaryLocation, ReadScale, HighAvailabilityReplicaCount, MaxSizeInGB, MaxLogSizeInGB, Kind);
    }

    public bool EventWriteEvent_DatabaseConnectionInfo(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDatabaseConnectionInfo(ref this.Event_DatabaseConnectionInfo, ExecutionId, ServerName, DatabaseName, IsReadOnly, HostName, ProgramName, HostProcessId, Count, InactiveCount, SampleText);
    }

    public bool EventWriteEvent_OrchestrationActivityLog_Info1(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateOrchestrationActivityLog1(ref this.Event_OrchestrationActivityLog_Info1, HostId, OrchestrationId, Name, Version, QueueTime, StartTime, ExecutionTime, CPUExecutionTime, ExceptionType, ExceptionMessage, ActivityId, E2EID, CPUCycles, AllocatedBytes);
    }

    public bool EventWriteEvent_DatabaseCounters_Info(
      string ServerName,
      string DatabaseName,
      Guid HostId,
      Guid ProjectId,
      string CounterName,
      long CounterValue,
      int LeftOverPercent)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDatabaseCounters(ref this.Event_DatabaseCounters_Info, ServerName, DatabaseName, HostId, ProjectId, CounterName, CounterValue, LeftOverPercent);
    }

    public bool EventWriteEvent_DatabaseIdentityColumns_Info(
      string ServerName,
      string DatabaseName,
      string SchemaName,
      string TableName,
      string IdentityColumnName,
      long IdentityColumnValue,
      string IdentityColumnDatatype,
      int LeftOverPercent)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDatabaseIdentityColumns(ref this.Event_DatabaseIdentityColumns_Info, ServerName, DatabaseName, SchemaName, TableName, IdentityColumnName, IdentityColumnValue, IdentityColumnDatatype, LeftOverPercent);
    }

    public bool EventWriteEvent_DatabaseServicePrincipals_Info(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDatabaseServicePrincipals(ref this.Event_DatabaseServicePrincipals_Info, ServerName, DatabaseName, ServicePrincipalId, ServicePrincipalName, TypeDesc, AuthenticationTypeDesc, CreateTime, ModifyTime, StateDesc, PermissionName);
    }

    public bool EventWriteEvent_SqlRowLockInfo_Info2(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateSqlRowLockInfo2(ref this.Event_SqlRowLockInfo_Info2, ExecutionId, ServerName, DatabaseName, IsReadOnly, SchemaName, TableName, IndexName, HobtId, ObjectId);
    }

    public bool EventWriteEvent_DatabasePrincipals_Info(
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
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateDatabasePrincipals(ref this.Event_DatabasePrincipals_Info, ServerName, DatabaseName, PrincipalId, PrincipalName, RoleName, Permissions, TypeDesc, CreateTime, ModifyTime);
    }

    public bool EventWriteEvent_XEventSessions_Info(
      int SessionId,
      string ServerName,
      string DatabaseName,
      string SessionName,
      bool IsEventFileTruncated,
      int BuffersLogged,
      int BuffersDropped,
      string EventFileName)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateXEventSessions(ref this.Event_XEventSessions_Info, SessionId, ServerName, DatabaseName, SessionName, IsEventFileTruncated, BuffersLogged, BuffersDropped, EventFileName);
    }

    public bool EventWriteEvent_GitThrottlingSettings_Info(int Size, int Tarpit, int WorkUnitSize) => !this.m_provider.IsEnabled() || this.m_provider.TemplateGitThrottlingSettings(ref this.Event_GitThrottlingSettings_Info, Size, Tarpit, WorkUnitSize);

    public bool EventWriteEvent_HostPreferredGeographyUpdate_Info(
      Guid HostId,
      string PreferredGeography,
      string GeographyUpdateType)
    {
      return !this.m_provider.IsEnabled() || this.m_provider.TemplateHostPreferredGeographyUpdate(ref this.Event_HostPreferredGeographyUpdate_Info, HostId, PreferredGeography, GeographyUpdateType);
    }
  }
}
