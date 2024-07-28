// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicePrincipalIsMemberReporter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicePrincipalIsMemberReporter : VssBaseService, IVssFrameworkService
  {
    private ILockName m_lock;
    private ConcurrentDictionary<ServicePrincipalIsMemberReporter.RecordKey, ServicePrincipalIsMemberReporter.Record> m_records;
    private ServicePrincipalIsMemberReporter.ServiceSettings m_settings;
    private bool m_stopping;
    private static readonly RegistryQuery s_registrySettingsQuery = (RegistryQuery) "/Service/ServicePrincipalIsMemberReporter/*";
    private const string c_area = "Identity";
    private const string c_layer = "ServicePrincipalIsMemberReporter";
    private const string c_randomKey = "$ServicePrincipalIsMemberReporterRandom";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      this.m_records = new ConcurrentDictionary<ServicePrincipalIsMemberReporter.RecordKey, ServicePrincipalIsMemberReporter.Record>(ServicePrincipalIsMemberReporter.RecordKey.Comparer);
      this.m_lock = this.CreateLockName(requestContext, nameof (ServicePrincipalIsMemberReporter));
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), in ServicePrincipalIsMemberReporter.s_registrySettingsQuery);
      Interlocked.CompareExchange<ServicePrincipalIsMemberReporter.ServiceSettings>(ref this.m_settings, new ServicePrincipalIsMemberReporter.ServiceSettings(requestContext), (ServicePrincipalIsMemberReporter.ServiceSettings) null);
      requestContext.GetService<TeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.OnFlushRecords), (object) null, this.m_settings.FlushIntervalInMilliseconds));
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      this.m_stopping = true;
      requestContext.GetService<TeamFoundationTaskService>().RemoveTask(requestContext, new TeamFoundationTaskCallback(this.OnFlushRecords));
      this.OnFlushRecords(requestContext, (object) null);
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<ServicePrincipalIsMemberReporter.ServiceSettings>(ref this.m_settings, new ServicePrincipalIsMemberReporter.ServiceSettings(requestContext));
    }

    private void OnFlushRecords(IVssRequestContext requestContext, object taskArg)
    {
      requestContext.CheckDeploymentRequestContext();
      List<ServicePrincipalIsMemberReporter.Record> recordList = new List<ServicePrincipalIsMemberReporter.Record>();
      using (requestContext.AcquireWriterLock(this.m_lock))
      {
        if (this.m_settings.Enabled)
          recordList.AddRange((IEnumerable<ServicePrincipalIsMemberReporter.Record>) this.m_records.Values);
        this.m_records.Clear();
      }
      if (recordList.Count <= 0)
        return;
      TeamFoundationTracingService service = requestContext.GetService<TeamFoundationTracingService>();
      foreach (ServicePrincipalIsMemberReporter.Record record in recordList)
        service.TraceServicePrincipalIsMember(record.Key.ServicePrincipalId, record.Key.GroupSid, (byte) record.Key.HostType, record.Key.StackTrace, record.ExecutionCount);
    }

    public void AddRecord(
      IVssRequestContext requestContext,
      IdentityDescriptor memberDescriptor,
      IdentityDescriptor containerDescriptor)
    {
      if (this.m_stopping)
        return;
      ServicePrincipalIsMemberReporter.ServiceSettings settings = this.m_settings;
      Guid spId;
      if (settings == null || !settings.Enabled || !settings.CollectDataFromDeploymentHost && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || (IdentityDescriptor) null == containerDescriptor || !containerDescriptor.IsTeamFoundationType() || !ServicePrincipals.IsServicePrincipal(requestContext, memberDescriptor, true, out spId))
        return;
      if (settings.SampleRate > 1)
      {
        object obj;
        Random random;
        if (!requestContext.RootContext.Items.TryGetValue("$ServicePrincipalIsMemberReporterRandom", out obj))
        {
          random = new Random((int) requestContext.RootContext.ContextId);
          requestContext.RootContext.Items["$ServicePrincipalIsMemberReporterRandom"] = (object) random;
        }
        else
          random = (Random) obj;
        if (random.Next(settings.SampleRate - 1) != 0)
          return;
      }
      string stackTrace = new StackTrace(1, false).ToString().Replace("Microsoft.TeamFoundation", "MS.TF").Replace("Microsoft.VisualStudio", "MS.VS");
      if (stackTrace.Length > settings.MaxStackTraceLengthInChars)
        stackTrace = stackTrace.Substring(0, settings.MaxStackTraceLengthInChars);
      string identifier = IdentityMapper.MapToWellKnownIdentifier(containerDescriptor, requestContext.ServiceHost.InstanceId).Identifier;
      ServicePrincipalIsMemberReporter.RecordKey key = new ServicePrincipalIsMemberReporter.RecordKey(spId, identifier, requestContext.ServiceHost.HostType, stackTrace);
      requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      using (requestContext.AcquireReaderLock(this.m_lock))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        this.m_records.GetOrAdd(key, ServicePrincipalIsMemberReporter.\u003C\u003EO.\u003C0\u003E__CreateRecord ?? (ServicePrincipalIsMemberReporter.\u003C\u003EO.\u003C0\u003E__CreateRecord = new Func<ServicePrincipalIsMemberReporter.RecordKey, ServicePrincipalIsMemberReporter.Record>(ServicePrincipalIsMemberReporter.CreateRecord))).IncrementExecutionCount();
      }
    }

    private static ServicePrincipalIsMemberReporter.Record CreateRecord(
      ServicePrincipalIsMemberReporter.RecordKey key)
    {
      return new ServicePrincipalIsMemberReporter.Record(key);
    }

    private class ServiceSettings
    {
      public readonly bool Enabled;
      public readonly bool CollectDataFromDeploymentHost;
      public readonly int FlushIntervalInMilliseconds;
      public readonly int MaxStackTraceLengthInChars;
      public readonly int SampleRate;

      public ServiceSettings(IVssRequestContext requestContext)
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, ServicePrincipalIsMemberReporter.s_registrySettingsQuery);
        this.Enabled = registryEntryCollection.GetValueFromPath<bool>(nameof (Enabled), requestContext.ExecutionEnvironment.IsHostedDeployment);
        this.CollectDataFromDeploymentHost = registryEntryCollection.GetValueFromPath<bool>(nameof (CollectDataFromDeploymentHost), true);
        this.FlushIntervalInMilliseconds = registryEntryCollection.GetValueFromPath<int>(nameof (FlushIntervalInMilliseconds), 120000);
        this.MaxStackTraceLengthInChars = registryEntryCollection.GetValueFromPath<int>(nameof (MaxStackTraceLengthInChars), 20000);
        this.SampleRate = registryEntryCollection.GetValueFromPath<int>(nameof (SampleRate), 50);
      }
    }

    private struct RecordKey
    {
      public readonly Guid ServicePrincipalId;
      public readonly string GroupSid;
      public readonly TeamFoundationHostType HostType;
      public readonly string StackTrace;
      public static readonly IEqualityComparer<ServicePrincipalIsMemberReporter.RecordKey> Comparer = (IEqualityComparer<ServicePrincipalIsMemberReporter.RecordKey>) new ServicePrincipalIsMemberReporter.RecordKey.RecordKeyComparer();

      public RecordKey(
        Guid servicePrincipalId,
        string groupSid,
        TeamFoundationHostType hostType,
        string stackTrace)
      {
        this.ServicePrincipalId = servicePrincipalId;
        this.GroupSid = groupSid;
        this.HostType = hostType;
        this.StackTrace = stackTrace;
      }

      private class RecordKeyComparer : IEqualityComparer<ServicePrincipalIsMemberReporter.RecordKey>
      {
        public bool Equals(
          ServicePrincipalIsMemberReporter.RecordKey x,
          ServicePrincipalIsMemberReporter.RecordKey y)
        {
          return x.HostType == y.HostType && x.ServicePrincipalId == y.ServicePrincipalId && StringComparer.OrdinalIgnoreCase.Equals(x.GroupSid, y.GroupSid) && StringComparer.OrdinalIgnoreCase.Equals(x.StackTrace, y.StackTrace);
        }

        public int GetHashCode(ServicePrincipalIsMemberReporter.RecordKey obj) => (obj.ServicePrincipalId.GetHashCode() ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.GroupSid) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.StackTrace)) + 5 * (int) obj.HostType;
      }
    }

    private class Record
    {
      public readonly ServicePrincipalIsMemberReporter.RecordKey Key;
      private int m_count;

      public Record(ServicePrincipalIsMemberReporter.RecordKey key)
      {
        this.Key = key;
        this.m_count = 0;
      }

      public void IncrementExecutionCount() => Interlocked.Increment(ref this.m_count);

      public int ExecutionCount => this.m_count;
    }
  }
}
