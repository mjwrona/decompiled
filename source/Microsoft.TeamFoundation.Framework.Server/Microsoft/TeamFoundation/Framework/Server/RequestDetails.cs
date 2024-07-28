// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RequestDetails
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class RequestDetails
  {
    public string UniqueAgentIdentifier;
    public string AuthenticationType;
    public MethodInformation Method;
    public string DomainUserName;
    public string ServiceName;
    public string Feature;
    public Exception Status;
    public int Count;
    public Guid InstanceId;
    public long ContextId;
    public Guid ActivityId;
    public DateTime StartTime;
    public DateTime EndTime;
    public long QueueTime;
    public long DelayTime;
    public long ConcurrencySemaphoreTime;
    public long ExecutionTime;
    public long TimeToFirstPage;
    public int LogicalReads;
    public int PhysicalReads;
    public int CpuTime;
    public int ElapsedTime;
    public int SqlExecutionTime;
    public int SqlExecutionCount;
    public int FinalSqlCommandExecutionTime;
    public int SqlReadOnlyExecutionTime;
    public int SqlReadOnlyExecutionCount;
    public int SqlRetryExecutionTime;
    public int SqlRetryExecutionCount;
    public int RedisExecutionTime;
    public int RedisExecutionCount;
    public int AadGraphExecutionTime;
    public int AadGraphExecutionCount;
    public int AadTokenExecutionTime;
    public int AadTokenExecutionCount;
    public int BlobStorageExecutionTime;
    public int BlobStorageExecutionCount;
    public int TableStorageExecutionTime;
    public int TableStorageExecutionCount;
    public int ServiceBusExecutionTime;
    public int ServiceBusExecutionCount;
    public int VssClientExecutionTime;
    public int VssClientExecutionCount;
    public DateTime HostStartTime;
    public TeamFoundationHostType HostType;
    public Guid ParentHostId;
    public Guid HostId;
    public string HostName;
    public string DatabaseServerName;
    public string DatabaseName;
    public string Title;
    public Guid UniqueIdentifier;
    public string AuthenticatedUserName;
    public string RemoteIPAddress;
    public string UserAgent;
    public string Command;
    public IList<MethodTime> RecursiveSqlCalls;
    public int ResponseCode;
    public object LogItemsObject;
    public Guid VSID;
    public IdentityTracingItems IdentityTracingItems;
    public string AuthenticationMechanism;
    public Guid E2EId;
    public string OrchestrationId;
    private object m_object = new object();
    public ActivityStatus ActivityStatus;
    public long ExecutionTimeThreshold;
    public bool IsExceptionExpected;
    public bool CanAggregate;
    public string AnonymousIdentifier;
    public long CPUCycles;
    public long AllocatedBytes;
    public Guid PersistentSessionId;
    public Guid PendingAuthenticationSessionId;
    public Guid CurrentAuthenticationSessionId;
    public double TSTUs;
    public string ThrottleReason;
    public string Referrer;
    public string UriStem;
    public readonly int TempCorrelationId;
    public SupportsPublicAccess SupportsPublicAccess;
    public Guid AuthorizationId;
    public Guid OAuthAppId;
    public long MethodInformationTimeout;
    public long PreControllerTime;
    public long ControllerTime;
    public long PostControllerTime;
    public string SmartRouterStatus;
    public string SmartRouterReason;
    public string SmartRouterTarget;
    private static int s_tempCorrelationId;

    public RequestDetails() => this.TempCorrelationId = Interlocked.Increment(ref RequestDetails.s_tempCorrelationId);

    public RequestDetails AddRef(RequestDetails requestContext)
    {
      lock (this.m_object)
      {
        if (requestContext.StartTime < this.StartTime)
          this.StartTime = requestContext.StartTime;
        if (requestContext.EndTime > this.EndTime)
          this.EndTime = requestContext.EndTime;
        this.DelayTime += requestContext.DelayTime;
        this.ConcurrencySemaphoreTime += requestContext.ConcurrencySemaphoreTime;
        this.ExecutionTime += requestContext.ExecutionTime;
        this.TimeToFirstPage += requestContext.TimeToFirstPage;
        this.QueueTime += requestContext.QueueTime;
        if (requestContext.RecursiveSqlCalls != null)
          this.RecursiveSqlCalls.Concat<MethodTime>((IEnumerable<MethodTime>) requestContext.RecursiveSqlCalls);
        this.LogicalReads += requestContext.LogicalReads;
        this.PhysicalReads += requestContext.PhysicalReads;
        this.CpuTime += requestContext.CpuTime;
        this.ElapsedTime += requestContext.ElapsedTime;
        this.SqlExecutionTime += requestContext.SqlExecutionTime;
        this.SqlExecutionCount += requestContext.SqlExecutionCount;
        this.FinalSqlCommandExecutionTime += requestContext.FinalSqlCommandExecutionTime;
        this.SqlReadOnlyExecutionTime += requestContext.SqlReadOnlyExecutionTime;
        this.SqlReadOnlyExecutionCount += requestContext.SqlReadOnlyExecutionCount;
        this.SqlRetryExecutionTime += requestContext.SqlRetryExecutionTime;
        this.SqlRetryExecutionCount += requestContext.SqlRetryExecutionCount;
        this.RedisExecutionTime += requestContext.RedisExecutionTime;
        this.RedisExecutionCount += requestContext.RedisExecutionCount;
        this.AadGraphExecutionTime += requestContext.AadGraphExecutionTime;
        this.AadGraphExecutionCount += requestContext.AadGraphExecutionCount;
        this.AadTokenExecutionTime += requestContext.AadTokenExecutionTime;
        this.AadTokenExecutionCount += requestContext.AadTokenExecutionCount;
        this.BlobStorageExecutionTime += requestContext.BlobStorageExecutionTime;
        this.BlobStorageExecutionCount += requestContext.BlobStorageExecutionCount;
        this.TableStorageExecutionTime += requestContext.TableStorageExecutionTime;
        this.TableStorageExecutionCount += requestContext.TableStorageExecutionCount;
        this.ServiceBusExecutionTime += requestContext.ServiceBusExecutionTime;
        this.ServiceBusExecutionCount += requestContext.ServiceBusExecutionCount;
        this.VssClientExecutionTime += requestContext.VssClientExecutionTime;
        this.VssClientExecutionCount += requestContext.VssClientExecutionCount;
        this.CPUCycles += requestContext.CPUCycles;
        this.AllocatedBytes += requestContext.AllocatedBytes;
        this.PreControllerTime += requestContext.PreControllerTime;
        this.ControllerTime += requestContext.ControllerTime;
        this.PostControllerTime += requestContext.PostControllerTime;
        this.TSTUs += requestContext.TSTUs;
        this.DocDBExecutionTime += requestContext.DocDBExecutionTime;
        this.DocDBExecutionCount += requestContext.DocDBExecutionCount;
        this.DocDBRUsConsumed += requestContext.DocDBRUsConsumed;
        ++this.Count;
      }
      return this;
    }

    public int DocDBExecutionTime { get; set; }

    public int DocDBExecutionCount { get; set; }

    public int DocDBRUsConsumed { get; set; }

    internal bool IncludeMethodDetails(bool includeDetails, int maxCompressedThresholdTime)
    {
      if (this.Method != null)
      {
        if (this.Status != null)
          includeDetails = true;
        else if (this.Count == 1 && this.ExecutionTime > (long) maxCompressedThresholdTime)
          includeDetails = true;
      }
      else
        includeDetails = false;
      return includeDetails;
    }
  }
}
