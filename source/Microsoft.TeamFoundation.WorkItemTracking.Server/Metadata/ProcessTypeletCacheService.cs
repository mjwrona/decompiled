// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProcessTypeletCacheService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class ProcessTypeletCacheService : 
    VssMemoryCacheService<Guid, IReadOnlyCollection<ProcessTypelet>>
  {
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromHours(24.0);
    private const int c_maxLength = 200;

    public ProcessTypeletCacheService()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, new MemoryCacheConfiguration<Guid, IReadOnlyCollection<ProcessTypelet>>().WithMaxElements(200))
    {
      this.InactivityInterval.Value = ProcessTypeletCacheService.s_maxCacheInactivityAge;
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.RegisterNotification(requestContext, "Default", SpecialGuids.WorkItemTypeletDeleted, new SqlNotificationCallback(this.OnWorkItemTypeletChanged), true);
      service.RegisterNotification(requestContext, "Default", SpecialGuids.WorkItemTypeletChanged, new SqlNotificationCallback(this.OnWorkItemTypeletChanged), true);
      service.RegisterNotification(requestContext, "Default", DBNotificationIds.WorkItemTypeBehaviorReferenceModified, new SqlNotificationCallback(this.OnWorkItemTypeletChanged), true);
      service.RegisterNotification(requestContext, "Default", DBNotificationIds.WorkItemTrackingProvisionedMetadataChanged, new SqlNotificationCallback(this.OnOldProvisioning), true);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(requestContext, "Default", SpecialGuids.WorkItemTypeletDeleted, new SqlNotificationCallback(this.OnWorkItemTypeletChanged), true);
      service.UnregisterNotification(requestContext, "Default", SpecialGuids.WorkItemTypeletChanged, new SqlNotificationCallback(this.OnWorkItemTypeletChanged), true);
      service.UnregisterNotification(requestContext, "Default", DBNotificationIds.WorkItemTypeBehaviorReferenceModified, new SqlNotificationCallback(this.OnWorkItemTypeletChanged), true);
      service.UnregisterNotification(requestContext, "Default", DBNotificationIds.WorkItemTrackingProvisionedMetadataChanged, new SqlNotificationCallback(this.OnOldProvisioning), true);
      base.ServiceEnd(requestContext);
    }

    public IReadOnlyCollection<ProcessTypelet> GetProcessTypelets(
      IVssRequestContext requestContext,
      Guid processId,
      bool bypassCache = false)
    {
      return this.GetTypeletsFromRightSource(requestContext, processId, bypassCache);
    }

    private IReadOnlyCollection<ProcessTypelet> GetTypeletsFromRightSource(
      IVssRequestContext requestContext,
      Guid processId,
      bool bypassCache = false)
    {
      return requestContext.TraceBlock<IReadOnlyCollection<ProcessTypelet>>(911002, 911003, "WorkItemType", nameof (ProcessTypeletCacheService), "ProcesTypeletCacheService.GetTypeletsFromRightSource", (Func<IReadOnlyCollection<ProcessTypelet>>) (() =>
      {
        bool flag = processId == ProcessConstants.SystemProcessId || requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processId).IsSystem;
        IReadOnlyCollection<ProcessTypelet> typeletsFromRightSource = (IReadOnlyCollection<ProcessTypelet>) null;
        if (flag)
          typeletsFromRightSource = this.GetTypeletsFromDeploymentContext(requestContext, processId);
        else if (bypassCache || !this.TryGetValue(requestContext, processId, out typeletsFromRightSource))
          typeletsFromRightSource = this.RefreshCollectionTypelets(requestContext, processId);
        typeletsFromRightSource = typeletsFromRightSource ?? (IReadOnlyCollection<ProcessTypelet>) new List<ProcessTypelet>(0);
        return typeletsFromRightSource;
      }));
    }

    private IReadOnlyCollection<ProcessTypelet> RefreshCollectionTypelets(
      IVssRequestContext requestContext,
      Guid processId)
    {
      return requestContext.TraceBlock<IReadOnlyCollection<ProcessTypelet>>(911004, 911005, "WorkItemType", nameof (ProcessTypeletCacheService), "ProcesTypeletCacheService.RefreshCollectionTypelets", (Func<IReadOnlyCollection<ProcessTypelet>>) (() =>
      {
        IReadOnlyCollection<ProcessTypelet> processTypelets = ProcessTypeletFactory.TransformIdentitiyRulesInTyplets(requestContext, this.GetTypeletsFromCollectionContext(requestContext, requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, processId)));
        this.Set(requestContext, processId, processTypelets);
        return processTypelets;
      }));
    }

    protected virtual IReadOnlyCollection<ProcessTypelet> GetTypeletsFromCollectionContext(
      IVssRequestContext requestContext,
      ProcessDescriptor process)
    {
      return (IReadOnlyCollection<ProcessTypelet>) requestContext.TraceBlock<Dictionary<string, ProcessTypelet>.ValueCollection>(911006, 911007, "WorkItemType", nameof (ProcessTypeletCacheService), "ProcesTypeletCacheService.GetTypeletsFromCollectionContext", (Func<Dictionary<string, ProcessTypelet>.ValueCollection>) (() =>
      {
        Dictionary<string, ProcessTypelet> typelets = this.GetRawTypeletsFromCollectionContext(requestContext, process.TypeId).ToDictionary<ProcessTypelet, string>((Func<ProcessTypelet, string>) (t => t.ReferenceName), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName);
        if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) && typelets.Any<KeyValuePair<string, ProcessTypelet>>())
        {
          Dictionary<string, ProcessTypelet> thisAndAncestorTypelets = new Dictionary<string, ProcessTypelet>((IDictionary<string, ProcessTypelet>) typelets, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName);
          Action<ProcessTypelet> action = (Action<ProcessTypelet>) (typelet =>
          {
            ProcessTypelet derivedBehavior;
            if (thisAndAncestorTypelets.TryGetValue(typelet.ReferenceName, out derivedBehavior))
            {
              if (!(typelet is Behavior) || !(derivedBehavior is Behavior))
                return;
              Behavior baseBehavior = typelet as Behavior;
              Behavior behavior = Behavior.Create(derivedBehavior as Behavior, baseBehavior);
              thisAndAncestorTypelets[behavior.ReferenceName] = (ProcessTypelet) behavior;
              if (!typelets.ContainsKey(behavior.ReferenceName))
                return;
              typelets[behavior.ReferenceName] = (ProcessTypelet) behavior;
            }
            else
              thisAndAncestorTypelets.Add(typelet.ReferenceName, typelet);
          });
          Dictionary<string, ProcessTypelet> dictionary = typelets;
          if (process.IsDerived)
          {
            foreach (ProcessTypelet processTypelet in (IEnumerable<ProcessTypelet>) this.GetTypeletsFromRightSource(requestContext, process.Inherits))
              action(processTypelet);
          }
          foreach (ProcessTypelet processTypelet in (IEnumerable<ProcessTypelet>) this.GetTypeletsFromDeploymentContext(requestContext, ProcessConstants.SystemProcessId))
            action(processTypelet);
          foreach (ProcessTypelet processTypelet in thisAndAncestorTypelets.Values)
            processTypelet.ResolveTypeReference((IReadOnlyCollection<ProcessTypelet>) thisAndAncestorTypelets.Values);
        }
        return typelets.Values;
      }));
    }

    protected virtual IReadOnlyCollection<ProcessTypelet> GetRawTypeletsFromCollectionContext(
      IVssRequestContext requestContext,
      Guid processId)
    {
      requestContext.TraceEnter(911008, "WorkItemType", nameof (ProcessTypeletCacheService), "ProcesTypeletCacheService.GetRawTypeletsFromCollectionContext");
      try
      {
        List<WorkItemTypeletRecord> workItemTypelets;
        using (WorkItemTypeExtensionComponent component = WorkItemTypeExtensionComponent.CreateComponent(requestContext))
          workItemTypelets = component.GetWorkItemTypelets(processId);
        WorkItemTrackingFieldService service = requestContext.GetService<WorkItemTrackingFieldService>();
        return ProcessTypeletFactory.Create(requestContext, (IEnumerable<WorkItemTypeletRecord>) workItemTypelets, service);
      }
      finally
      {
        requestContext.TraceLeave(911009, "WorkItemType", nameof (ProcessTypeletCacheService), "ProcesTypeletCacheService.GetRawTypeletsFromCollectionContext");
      }
    }

    private void OnWorkItemTypeletChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      Guid result;
      if (Guid.TryParse(eventData, out result))
      {
        this.Remove(requestContext, result);
      }
      else
      {
        requestContext.Trace(911001, TraceLevel.Info, "WorkItemType", nameof (ProcessTypeletCacheService), string.Format("Failed to understand what changed from eventData: {0}", (object) eventData));
        this.Clear(requestContext);
      }
    }

    private void OnOldProvisioning(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.Clear(requestContext);
    }

    private IReadOnlyCollection<ProcessTypelet> GetTypeletsFromDeploymentContext(
      IVssRequestContext requestContext,
      Guid processId)
    {
      return (IReadOnlyCollection<ProcessTypelet>) requestContext.TraceBlock<Dictionary<string, ProcessTypelet>.ValueCollection>(911010, 911011, "WorkItemType", nameof (ProcessTypeletCacheService), "ProcesTypeletCacheService.GetTypeletsFromDeploymentContext", (Func<Dictionary<string, ProcessTypelet>.ValueCollection>) (() =>
      {
        Dictionary<string, ProcessTypelet> typelets = this.GetRawTypeletsFromDeploymentContext(requestContext, processId).ToDictionary<ProcessTypelet, string>((Func<ProcessTypelet, string>) (t => t.ReferenceName), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName);
        if (typelets.Any<KeyValuePair<string, ProcessTypelet>>())
        {
          Dictionary<string, ProcessTypelet> thisAndAncestorTypelets = new Dictionary<string, ProcessTypelet>((IDictionary<string, ProcessTypelet>) typelets, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeReferenceName);
          Action<ProcessTypelet> action = (Action<ProcessTypelet>) (typelet =>
          {
            ProcessTypelet derivedBehavior;
            if (thisAndAncestorTypelets.TryGetValue(typelet.ReferenceName, out derivedBehavior))
            {
              if (!(typelet is Behavior) || !(derivedBehavior is Behavior))
                return;
              Behavior baseBehavior = typelet as Behavior;
              Behavior behavior = Behavior.Create(derivedBehavior as Behavior, baseBehavior);
              thisAndAncestorTypelets[behavior.ReferenceName] = (ProcessTypelet) behavior;
              if (!typelets.ContainsKey(behavior.ReferenceName))
                return;
              typelets[behavior.ReferenceName] = (ProcessTypelet) behavior;
            }
            else
              thisAndAncestorTypelets.Add(typelet.ReferenceName, typelet);
          });
          if (processId != ProcessConstants.SystemProcessId)
          {
            foreach (ProcessTypelet processTypelet in (IEnumerable<ProcessTypelet>) this.GetTypeletsFromDeploymentContext(requestContext, ProcessConstants.SystemProcessId))
              action(processTypelet);
          }
          foreach (ProcessTypelet processTypelet in thisAndAncestorTypelets.Values)
            processTypelet.ResolveTypeReference((IReadOnlyCollection<ProcessTypelet>) thisAndAncestorTypelets.Values);
        }
        return typelets.Values;
      }));
    }

    private IReadOnlyCollection<ProcessTypelet> GetRawTypeletsFromDeploymentContext(
      IVssRequestContext requestContext,
      Guid processId)
    {
      IReadOnlyCollection<WorkItemTypeletRecord> itemTypeletRecords;
      return requestContext.TraceBlock<IReadOnlyCollection<ProcessTypelet>>(911012, 911013, "WorkItemType", nameof (ProcessTypeletCacheService), "ProcesTypeletCacheService.GetRawTypeletsFromDeploymentContext", (Func<IReadOnlyCollection<ProcessTypelet>>) (() => requestContext.To(TeamFoundationHostType.Deployment).GetService<ProcessTypeletCacheService.SystemProcessTypeletCacheService>().TryGetValue(requestContext, processId, out itemTypeletRecords) ? ProcessTypeletFactory.Create(requestContext, (IEnumerable<WorkItemTypeletRecord>) (itemTypeletRecords ?? (IReadOnlyCollection<WorkItemTypeletRecord>) new List<WorkItemTypeletRecord>(0)), requestContext.GetService<WorkItemTrackingFieldService>()) : (IReadOnlyCollection<ProcessTypelet>) new List<ProcessTypelet>()));
    }

    internal class SystemProcessTypeletCacheService : 
      WorkItemTrackingOutOfBoxCacheVersioning<IReadOnlyCollection<WorkItemTypeletRecord>>
    {
      protected override void ServiceStart(IVssRequestContext requestContext)
      {
        requestContext.CheckServiceHostType(TeamFoundationHostType.Deployment);
        base.ServiceStart(requestContext);
        this.InitializeData(requestContext);
      }

      protected override void ServiceEnd(IVssRequestContext requestContext)
      {
      }

      public override bool TryGetValue(
        IVssRequestContext requestContext,
        Guid key,
        out IReadOnlyCollection<WorkItemTypeletRecord> value)
      {
        if (this.ClearCacheOnStaleCacheVersion(requestContext))
        {
          requestContext.Trace(911531, TraceLevel.Info, "WorkItemType", "BehaviorService", string.Format("ClearCacheOnStaleCacheVersion for key : {0}", (object) key));
          this.InitializeData(requestContext);
        }
        int num = base.TryGetValue(requestContext, key, out value) ? 1 : 0;
        requestContext.Trace(911531, TraceLevel.Info, "WorkItemType", "BehaviorService", string.Format("key : {0} value count : {1}", (object) key, (object) (value != null ? value.Count : 0)));
        return num != 0;
      }

      private void InitializeData(IVssRequestContext requestContext)
      {
        this.Set(requestContext, ProcessConstants.SystemProcessId, this.ReadSystemProcessTypelets(requestContext));
        this.Set(requestContext, ProcessTemplateTypeIdentifiers.MsfAgileSoftwareDevelopment, this.ReadAgileProcessTypelets(requestContext));
        this.Set(requestContext, ProcessTemplateTypeIdentifiers.MsfCmmiProcessImprovement, this.ReadCMMIProcessTypelets(requestContext));
        this.Set(requestContext, ProcessTemplateTypeIdentifiers.VisualStudioScrum, this.ReadScrumProcessTypelets(requestContext));
        this.Set(requestContext, ProcessTemplateTypeIdentifiers.MsfHydroProcess, this.ReadHydroProcessTypelets(requestContext));
      }

      private IReadOnlyCollection<WorkItemTypeletRecord> ReadSystemProcessTypelets(
        IVssRequestContext requestContext)
      {
        IReadOnlyCollection<WorkItemTypeletRecord> source = this.DeserializeTypelets(MetadataResourceManager.GetSystemProcessTypeletResources(requestContext));
        WorkItemTypeletRecord itemTypeletRecord = source.FirstOrDefault<WorkItemTypeletRecord>((Func<WorkItemTypeletRecord, bool>) (t => TFStringComparer.WorkItemTypeReferenceName.Equals(t.ReferenceName, SystemWorkItemType.ReferenceName)));
        if (itemTypeletRecord == null)
          return source;
        CultureInfo culture = requestContext.ServiceHost.GetCulture(requestContext);
        Layout workItemTypeLayout = new LayoutHelper(WellKnownProcessLayout.GetAgileBugLayout(requestContext)).CreateDefaultSystemWorkItemTypeLayout(itemTypeletRecord.ProcessId.ToString(), itemTypeletRecord.ReferenceName, culture);
        itemTypeletRecord.Form = JsonConvert.SerializeObject((object) workItemTypeLayout);
        return source;
      }

      private IReadOnlyCollection<WorkItemTypeletRecord> ReadAgileProcessTypelets(
        IVssRequestContext requestContext)
      {
        return this.DeserializeTypelets(MetadataResourceManager.GetAgileProcessTypeletResources(requestContext, ProcessTemplateTypeIdentifiers.MsfAgileSoftwareDevelopment));
      }

      private IReadOnlyCollection<WorkItemTypeletRecord> ReadCMMIProcessTypelets(
        IVssRequestContext requestContext)
      {
        return this.DeserializeTypelets(MetadataResourceManager.GetCMMIProcessTypeletResources(requestContext, ProcessTemplateTypeIdentifiers.MsfCmmiProcessImprovement));
      }

      private IReadOnlyCollection<WorkItemTypeletRecord> ReadScrumProcessTypelets(
        IVssRequestContext requestContext)
      {
        return this.DeserializeTypelets(MetadataResourceManager.GetScrumProcessTypeletResources(requestContext, ProcessTemplateTypeIdentifiers.VisualStudioScrum));
      }

      private IReadOnlyCollection<WorkItemTypeletRecord> ReadHydroProcessTypelets(
        IVssRequestContext requestContext)
      {
        return this.DeserializeTypelets(MetadataResourceManager.GetHydroProcessTypeletResources(requestContext, ProcessTemplateTypeIdentifiers.MsfHydroProcess));
      }

      private IReadOnlyCollection<WorkItemTypeletRecord> DeserializeTypelets(
        IReadOnlyCollection<string> typeletXmls)
      {
        List<WorkItemTypeletRecord> itemTypeletRecordList = new List<WorkItemTypeletRecord>();
        foreach (string typeletXml in (IEnumerable<string>) typeletXmls)
          itemTypeletRecordList.Add(TeamFoundationSerializationUtility.Deserialize<WorkItemTypeletRecord>(typeletXml));
        return (IReadOnlyCollection<WorkItemTypeletRecord>) itemTypeletRecordList;
      }
    }
  }
}
