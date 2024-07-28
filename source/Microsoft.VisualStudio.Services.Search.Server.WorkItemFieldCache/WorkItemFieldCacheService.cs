// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache.WorkItemFieldCacheService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 79AB9CC0-954C-4F8E-A45A-BE8292FA9E70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache
{
  public class WorkItemFieldCacheService : 
    VssMemoryCacheService<string, WorkItemField>,
    IWorkItemFieldCacheService,
    IVssFrameworkService
  {
    internal ILockName Lock;
    internal HashSet<Guid> NotificationsFired;
    [StaticSafe]
    private static readonly MultiPatternTextFilter s_deploymentLevelWorkItemFieldSelector = new MultiPatternTextFilter("-WEF_????????????????????????????????_*,-System.IterationLevel*,-System.AreaLevel*,-System.Watermark");
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1081903, "Common", nameof (WorkItemFieldCacheService));
    private const string ReferenceNameKey = "refNameKey";
    private IVssMemoryCacheGrouping<string, WorkItemField, string> m_displayNameGrouping;
    private IVssMemoryCacheGrouping<string, WorkItemField, string> m_referenceNameGrouping;
    private MultiPatternTextFilter m_collectionLevelWorkItemFieldSelector;

    public WorkItemFieldCacheService()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && !systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnsupportedHostTypeException();
      base.ServiceStart(systemRequestContext);
      this.Lock = systemRequestContext.ServiceHost.CreateLockName(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}DataLock", (object) systemRequestContext.ServiceHost.InstanceId.ToString()));
      this.m_displayNameGrouping = VssMemoryCacheGroupingFactory.Create<string, WorkItemField, string>(systemRequestContext, this.MemoryCache, (Func<string, WorkItemField, IEnumerable<string>>) ((key, value) => (IEnumerable<string>) new string[1]
      {
        value.Name.Replace(" ", string.Empty)
      }), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_referenceNameGrouping = VssMemoryCacheGroupingFactory.Create<string, WorkItemField, string>(systemRequestContext, this.MemoryCache, (Func<string, WorkItemField, IEnumerable<string>>) ((key, value) => (IEnumerable<string>) new string[1]
      {
        "refNameKey"
      }), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        this.NotificationsFired = new HashSet<Guid>();
        systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.WITFieldDefinitionUpdated, new SqlNotificationCallback(this.OnNotification), false);
        this.InitializeCollectionFieldSelector(systemRequestContext);
        this.InitializeCache(systemRequestContext);
      }
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("ServiceStartTime", nameof (WorkItemFieldCacheService), (double) stopwatch.ElapsedMilliseconds);
    }

    internal virtual void InitializeCache(IVssRequestContext systemRequestContext) => this.UpdateCacheWithFields(systemRequestContext, "Service Start", systemRequestContext.IsHostProcessType(HostProcessType.JobAgent));

    private void InitializeCollectionFieldSelector(IVssRequestContext systemRequestContext)
    {
      string configValue = systemRequestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/WorkItemIgnoredFieldsReferenceNames", TeamFoundationHostType.ProjectCollection);
      if (string.IsNullOrWhiteSpace(configValue))
        return;
      this.m_collectionLevelWorkItemFieldSelector = new MultiPatternTextFilter(string.Join(",", ((IEnumerable<string>) configValue.Split(new char[2]
      {
        ',',
        ';'
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (field => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-{0}", (object) field)))));
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.WITFieldDefinitionUpdated, new SqlNotificationCallback(this.OnNotification), false);
      base.ServiceEnd(systemRequestContext);
    }

    public bool TryGetFieldByRefName(
      IVssRequestContext requestContext,
      string fieldRefName,
      out WorkItemField fieldData,
      bool forceUpdate = false)
    {
      if (this.TryGetFieldByRefNameInternal(requestContext, fieldRefName, out fieldData))
        return true;
      if (forceUpdate)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        using (requestContext.AcquireWriterLock(this.Lock))
        {
          if (this.TryGetFieldByRefNameInternal(requestContext, fieldRefName, out fieldData))
            return true;
          this.UpdateCacheWithFields(requestContext, "Update forced while TryGetFieldByRefName", true)?.TryGetValue(fieldRefName, out fieldData);
        }
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("CacheSyncronousUpdateTime", nameof (WorkItemFieldCacheService), (double) stopwatch.ElapsedMilliseconds);
      }
      return fieldData != null;
    }

    public bool TryGetFieldByName(
      IVssRequestContext requestContext,
      string fieldName,
      out IEnumerable<WorkItemField> fieldsData)
    {
      HashSet<WorkItemField> source = new HashSet<WorkItemField>();
      using (requestContext.AcquireReaderLock(this.Lock))
      {
        IEnumerable<string> keys;
        if (this.m_displayNameGrouping.TryGetKeys(fieldName.Replace(" ", string.Empty), out keys))
        {
          foreach (string fieldRefName in keys)
          {
            WorkItemField fieldData;
            if (this.TryGetFieldByRefName(requestContext, fieldRefName, out fieldData, false))
              source.Add(fieldData);
          }
        }
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        {
          IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
          IEnumerable<WorkItemField> fieldsData1;
          if (this.GetCacheService(requestContext1).TryGetFieldByName(requestContext1, fieldName, out fieldsData1))
            source.UnionWith(fieldsData1);
        }
        fieldsData = (IEnumerable<WorkItemField>) source;
      }
      return source.Any<WorkItemField>();
    }

    public IEnumerable<WorkItemField> GetFieldsList(IVssRequestContext requestContext)
    {
      List<WorkItemField> fieldsList = new List<WorkItemField>();
      using (requestContext.AcquireReaderLock(this.Lock))
      {
        IEnumerable<string> keys;
        if (this.m_referenceNameGrouping.TryGetKeys("refNameKey", out keys))
        {
          foreach (string fieldRefName in keys)
          {
            WorkItemField fieldData;
            if (this.TryGetFieldByRefName(requestContext, fieldRefName, out fieldData, false))
              fieldsList.Add(fieldData);
          }
        }
      }
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        WorkItemFieldCacheService cacheService = this.GetCacheService(requestContext1);
        fieldsList.AddRange(cacheService.GetFieldsList(requestContext1));
      }
      return (IEnumerable<WorkItemField>) fieldsList;
    }

    public HashSet<string> GetIdentityFieldsList(IVssRequestContext requestContext)
    {
      IEnumerable<WorkItemField> fieldsList = this.GetFieldsList(requestContext);
      HashSet<string> identityFieldsList = new HashSet<string>();
      foreach (WorkItemField workItemField in fieldsList)
      {
        if (workItemField.IsIdentity)
          identityFieldsList.Add(workItemField.ReferenceName);
      }
      return identityFieldsList;
    }

    internal virtual bool TryGetFieldByRefNameInternal(
      IVssRequestContext requestContext,
      string fieldRefName,
      out WorkItemField entry)
    {
      if (this.SynchronizedRead(requestContext, fieldRefName, out entry))
        return true;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        if (this.GetCacheService(requestContext1).TryGetFieldByRefName(requestContext1, fieldRefName, out entry, false))
          return true;
      }
      return false;
    }

    internal IDictionary<string, WorkItemField> UpdateCacheWithFields(
      IVssRequestContext requestContext,
      string reason = "SQL Notification",
      bool notifyOtherInstances = false)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(WorkItemFieldCacheService.s_traceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Updating the WorkItem Field Cache Reason : {0}", (object) reason));
      IEnumerable<WorkItemField> newFields = this.GetFieldsFromTfs(requestContext);
      if (newFields == null)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(WorkItemFieldCacheService.s_traceMetadata, "Fetching workitem fields from TFS did not return any data");
        return (IDictionary<string, WorkItemField>) null;
      }
      this.SynchronizedWrite(requestContext, newFields.Where<WorkItemField>((Func<WorkItemField, bool>) (f => !this.IsSystemField(f))));
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      this.GetCacheService(requestContext1).SynchronizedWrite(requestContext1, newFields.Where<WorkItemField>((Func<WorkItemField, bool>) (f => this.IsSystemField(f))));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(WorkItemFieldCacheService.s_traceMetadata, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "System Fields list:{0}", (object) string.Join(",", newFields.Where<WorkItemField>(new Func<WorkItemField, bool>(this.IsSystemField)).Select<WorkItemField, string>((Func<WorkItemField, string>) (f => f.ReferenceName))))));
      if (notifyOtherInstances)
      {
        Guid guid = Guid.NewGuid();
        this.NotificationsFired.Add(guid);
        try
        {
          requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.WITFieldDefinitionUpdated, guid.ToString());
        }
        catch (Exception ex)
        {
          this.NotificationsFired.Remove(guid);
        }
      }
      return (IDictionary<string, WorkItemField>) newFields.ToDictionary<WorkItemField, string, WorkItemField>((Func<WorkItemField, string>) (f => f.ReferenceName), (Func<WorkItemField, WorkItemField>) (f => f), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    private IEnumerable<WorkItemField> SynchronizedWrite(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemField> fields)
    {
      if (requestContext.IsWriterLockHeld(this.Lock))
        return this.UpdateCacheWithFieldsHelper(requestContext, fields);
      Stopwatch stopwatch = Stopwatch.StartNew();
      using (requestContext.AcquireWriterLock(this.Lock))
        this.UpdateCacheWithFieldsHelper(requestContext, fields);
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("FieldCacheWriteLockedTime", nameof (WorkItemFieldCacheService), (double) stopwatch.ElapsedMilliseconds);
      return fields;
    }

    internal void OnNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081904, "Common", nameof (WorkItemFieldCacheService), nameof (OnNotification));
      try
      {
        Guid result;
        if (Guid.TryParse(eventData, out result))
        {
          bool flag = false;
          using (requestContext.AcquireReaderLock(this.Lock))
            flag = this.NotificationsFired.Contains(result);
          if (!flag)
          {
            requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTaskCallback(this.UpdateCacheInBackground));
          }
          else
          {
            using (requestContext.AcquireWriterLock(this.Lock))
              this.NotificationsFired.Remove(result);
          }
        }
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(WorkItemFieldCacheService.s_traceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to deserialize the notification data to Guid. EventData:{0}. Possible missing notification", (object) eventData));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081905, "Common", nameof (WorkItemFieldCacheService), nameof (OnNotification));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    internal virtual void UpdateCacheInBackground(IVssRequestContext requestContext, object unused)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      try
      {
        this.UpdateCacheWithFields(requestContext, "SQL notification");
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    private IEnumerable<WorkItemField> UpdateCacheWithFieldsHelper(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemField> fields)
    {
      this.Clear(requestContext);
      foreach (WorkItemField field in fields)
        this.TryAdd(requestContext, field.ReferenceName, field);
      return fields;
    }

    internal virtual IEnumerable<WorkItemField> GetFieldsFromTfs(IVssRequestContext requestContext)
    {
      IEnumerable<WorkItemField> source = (IEnumerable<WorkItemField>) null;
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>(requestContext.GetHttpClientOptionsForEventualReadConsistencyLevel("Search.Server.WorkItem.EnableReadReplica"));
        source = ExponentialBackoffRetryInvoker.Instance.Invoke<IEnumerable<WorkItemField>>((Func<object>) (() => (object) client.GetWorkItemFieldsAsync().Result), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkitemFieldCacheService/GetFieldsFromTfsRetryCount"), requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkitemFieldCacheService/GetFieldsFromTfsRetryInitialDelayInMillis"), true, WorkItemFieldCacheService.s_traceMetadata).Where<WorkItemField>((Func<WorkItemField, bool>) (f => WorkItemFieldCacheService.s_deploymentLevelWorkItemFieldSelector.IsMatch(f.ReferenceName)));
        if (this.m_collectionLevelWorkItemFieldSelector != null)
          source = source.Where<WorkItemField>((Func<WorkItemField, bool>) (f => this.m_collectionLevelWorkItemFieldSelector.IsMatch(f.ReferenceName)));
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("TimeToWorkItemFieldsFetchedFromTfs", nameof (WorkItemFieldCacheService), (double) stopwatch.ElapsedMilliseconds);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(WorkItemFieldCacheService.s_traceMetadata, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to fetch fields from Tfs Server. Exception:{0} StackTrace {1}", (object) ex.Message, (object) ex.StackTrace));
      }
      return source;
    }

    private bool SynchronizedRead(
      IVssRequestContext requestContext,
      string fieldName,
      out WorkItemField entry)
    {
      if (requestContext.IsWriterLockHeld(this.Lock) || requestContext.IsReaderLockHeld(this.Lock))
        return this.TryGetValue(requestContext, fieldName, out entry);
      using (requestContext.AcquireReaderLock(this.Lock))
        return this.TryGetValue(requestContext, fieldName, out entry);
    }

    private bool IsSystemField(WorkItemField entry) => entry.ReferenceName.StartsWith("System.", StringComparison.OrdinalIgnoreCase);

    internal virtual WorkItemFieldCacheService GetCacheService(IVssRequestContext requestContext) => requestContext.GetService<WorkItemFieldCacheService>();
  }
}
