// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.HostIdMappingService
// Assembly: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D6C9F672-10C9-4D5D-90D5-E07E56C1D4C0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping
{
  public class HostIdMappingService : IHostIdMappingService, IVssFrameworkService
  {
    protected virtual string Layer => nameof (HostIdMappingService);

    protected virtual string Area => TracingPoints.ExternalServiceEventUrlHostRoutingArea;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<Guid> GetHostIds(
      IVssRequestContext requestContext,
      string providerId,
      HostIdMappingData mappingData)
    {
      requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "GetHostIds providerId={0}, mappingData={1}", (object) providerId, (object) mappingData);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string routingNamespace1 = this.GetRoutingNamespace(providerId, mappingData.PropertyName, true);
      NameResolutionEntry entryInternal = this.GetEntryInternal(vssRequestContext, routingNamespace1, mappingData.Id, (string) null);
      if (entryInternal != null)
      {
        Guid guid = entryInternal.Value;
        requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "GetHostIds reverseLookupId={0} providerId = {1}", (object) guid, (object) providerId);
        IInternalNameResolutionService service = vssRequestContext.GetService<IInternalNameResolutionService>();
        IList<NameResolutionEntry> nameResolutionEntryList = service.QueryEntriesForValue(vssRequestContext, guid, QueryOptions.None);
        try
        {
          string routingNamespace2 = this.GetRoutingNamespace(providerId, mappingData.PropertyName);
          List<NameResolutionQuery> queries = new List<NameResolutionQuery>();
          foreach (NameResolutionEntry nameResolutionEntry in (IEnumerable<NameResolutionEntry>) nameResolutionEntryList)
            queries.Add(new NameResolutionQuery()
            {
              Name = nameResolutionEntry.Name,
              Namespace = routingNamespace2
            });
          IReadOnlyList<NameResolutionEntry> source = service.QueryEntries(vssRequestContext, (IReadOnlyList<NameResolutionQuery>) queries);
          return source == null ? (IEnumerable<Guid>) null : source.Where<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (e => e != null)).Select<NameResolutionEntry, Guid>((Func<NameResolutionEntry, Guid>) (e => e.Value));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(TracingPoints.EventsRouting.Mapping, this.Area, this.Layer, ex);
        }
      }
      else
        requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Error, this.Area, this.Layer, string.Format("GetHostIds - primary reverse lookup entry NOT FOUND. providerId={0}, mappingData={1} reverseRoutingNamespace={2}", (object) providerId, (object) mappingData, (object) routingNamespace1));
      return (IEnumerable<Guid>) null;
    }

    public Guid? GetHostId(
      IVssRequestContext requestContext,
      string providerId,
      HostIdMappingData mappingData,
      bool useExactMatching = false)
    {
      requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "GetHostId providerId={0}, mappingData={1}, useExactMatching={2}", (object) providerId, (object) mappingData, (object) useExactMatching);
      string routingNamespace = this.GetRoutingNamespace(providerId, mappingData.PropertyName);
      NameResolutionEntry entryInternal = this.GetEntryInternal(requestContext, routingNamespace, mappingData.Id, mappingData.Qualifier, !useExactMatching);
      if (entryInternal == null || !entryInternal.IsEnabled)
        return new Guid?();
      requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "GetHostId - entry found. Value={0}", (object) entryInternal.Value);
      return new Guid?(entryInternal.Value);
    }

    public List<string> GetUnneededMappings(
      IVssRequestContext requestContext,
      string providerId,
      string propertyName,
      List<HostIdMappingData> mappingDataList)
    {
      IList<NameResolutionEntry> allReverseMappings = this.GetAllReverseMappings(requestContext, providerId, propertyName, mappingDataList);
      List<string> routingKeysForMappings = this.GetRoutingKeysForMappings(mappingDataList);
      List<string> unneededMappings = new List<string>();
      foreach (NameResolutionEntry nameResolutionEntry in (IEnumerable<NameResolutionEntry>) allReverseMappings)
      {
        if (!routingKeysForMappings.Contains(nameResolutionEntry.Name))
          unneededMappings.Add(nameResolutionEntry.Name);
      }
      return unneededMappings;
    }

    private IList<NameResolutionEntry> GetAllReverseMappings(
      IVssRequestContext requestContext,
      string providerId,
      string propertyName,
      List<HostIdMappingData> mappingDataList)
    {
      IList<NameResolutionEntry> collection = (IList<NameResolutionEntry>) new List<NameResolutionEntry>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string routingNamespace = this.GetRoutingNamespace(providerId, propertyName, true);
      foreach (HostIdMappingData mappingData in mappingDataList)
      {
        NameResolutionEntry entryInternal = this.GetEntryInternal(vssRequestContext, routingNamespace, mappingData.Id, (string) null);
        if (entryInternal != null)
        {
          Guid guid = entryInternal.Value;
          requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "GetHostIds reverseLookupId={0} providerId = {1}", (object) guid, (object) providerId);
          IInternalNameResolutionService service = vssRequestContext.GetService<IInternalNameResolutionService>();
          collection.AddRange<NameResolutionEntry, IList<NameResolutionEntry>>((IEnumerable<NameResolutionEntry>) service.QueryEntriesForValue(vssRequestContext, guid, QueryOptions.None));
        }
      }
      return collection;
    }

    public Guid AddRoute(
      IVssRequestContext requestContext,
      IHostIdMappingProviderData providerData,
      string mappingProperty)
    {
      ArgumentUtility.CheckForNull<IHostIdMappingProviderData>(providerData, nameof (providerData));
      ArgumentUtility.CheckForNull<IReadOnlyList<IHostIdMappingRouter>>(providerData.Routers, "Routers");
      requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Layer, "AddRoute providerId={0}, mappingProperty ={1}", providerData.ProviderId, (object) mappingProperty);
      Guid guid = requestContext.ServiceHost.InstanceId;
      foreach (IHostIdMappingRouter router in (IEnumerable<IHostIdMappingRouter>) providerData.Routers)
      {
        Guid conflictingHostId;
        if (this.TryAddHostIdMapping(requestContext, router, providerData.ProviderId, mappingProperty, (string) null, out conflictingHostId))
        {
          requestContext.TraceAlways(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "AddRoute - First connection. billingHostId={0}, mappingProperty ={1}", (object) guid, (object) mappingProperty);
        }
        else
        {
          guid = conflictingHostId;
          requestContext.TraceAlways(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "AddRoute - Existing connection already exists. billingHostId={0}, mappingProperty ={1}", (object) guid, (object) mappingProperty);
        }
      }
      return guid;
    }

    public void RemoveRoute(
      IVssRequestContext requestContext,
      IHostIdMappingProviderData providerData,
      string mappingProperty)
    {
      ArgumentUtility.CheckForNull<IHostIdMappingProviderData>(providerData, nameof (providerData));
      ArgumentUtility.CheckForNull<IReadOnlyList<IHostIdMappingRouter>>(providerData.Routers, "Routers");
      requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "RemoveRoute providerId={0}, mappingProperty ={1}", (object) providerData.ProviderId, (object) mappingProperty);
      foreach (IHostIdMappingRouter hostIdMappingRouter in providerData.Routers.OfType<IHostIdMappingRouter>())
        hostIdMappingRouter.RemoveHostIdMapping(requestContext, providerData.ProviderId, mappingProperty, (string) null);
    }

    public bool TryAddHostIdMapping(
      IVssRequestContext requestContext,
      IHostIdMappingRouter mappingRouter,
      string providerId,
      string mappingProperty,
      string qualifier,
      out Guid conflictingHostId)
    {
      try
      {
        mappingRouter.AddHostIdMapping(requestContext, providerId, mappingProperty, qualifier);
        conflictingHostId = Guid.Empty;
        return true;
      }
      catch (NameResolutionEntryAlreadyExistsException ex)
      {
        HostIdMappingData mappingData = mappingRouter.GetMappingData(requestContext, mappingProperty, qualifier);
        string routingNamespace = this.GetRoutingNamespace(providerId, mappingData.PropertyName);
        NameResolutionEntry entryInternal = this.GetEntryInternal(requestContext, routingNamespace, mappingProperty, qualifier);
        if (entryInternal.Value != Guid.Empty && entryInternal.Value != requestContext.ServiceHost.InstanceId && mappingRouter.OverrideOnDeletedOrganization(requestContext) && !this.HostHasActiveEntries(requestContext, entryInternal.Value))
        {
          mappingRouter.AddHostIdMapping(requestContext, providerId, mappingProperty, qualifier, true);
          conflictingHostId = Guid.Empty;
          return true;
        }
        conflictingHostId = entryInternal != null ? entryInternal.Value : Guid.Empty;
        return false;
      }
    }

    public bool TryRemoveHostIdMapping(
      IVssRequestContext requestContext,
      IHostIdMappingRouter mappingRouter,
      string providerId,
      HostIdMappingData mappingData,
      Guid expectedHostId)
    {
      string routingNamespace = this.GetRoutingNamespace(providerId, mappingData.PropertyName);
      NameResolutionEntry entryInternal = this.GetEntryInternal(requestContext, routingNamespace, mappingData.Id, mappingData.Qualifier);
      if (entryInternal != null && entryInternal.Value == expectedHostId)
      {
        mappingRouter.RemoveHostIdMapping(requestContext, providerId, mappingData.Id, mappingData.Qualifier);
        return true;
      }
      requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "TryRemoveHostIdMapping - Failed to remove mapping - providerId={0}, mappingData={1}, expectedHostId={2}", (object) providerId, (object) mappingData, (object) expectedHostId);
      return false;
    }

    public void AddHostIdMapping(
      IVssRequestContext requestContext,
      string providerId,
      HostIdMappingData mappingData,
      Guid hostId,
      bool overrideExisting = false)
    {
      requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "AddHostIdMapping providerId={0}, mappingData={1}, hostId={2}", (object) providerId, (object) mappingData, (object) hostId);
      string routingNamespace = this.GetRoutingNamespace(providerId, mappingData.PropertyName);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      INameResolutionService service = vssRequestContext.GetService<INameResolutionService>();
      string routingKey = this.GetRoutingKey(mappingData.Id, mappingData.Qualifier);
      if (!this.TryCreateReverseLookupEntry(vssRequestContext, service, providerId, mappingData, overrideExisting) && !string.IsNullOrEmpty(mappingData.Qualifier))
        return;
      this.CreateEntry(service, vssRequestContext, routingNamespace, routingKey, requestContext.ServiceHost.InstanceId, overrideExisting);
    }

    private List<string> GetRoutingKeysForMappings(List<HostIdMappingData> mappingDataList)
    {
      List<string> routingKeysForMappings = new List<string>();
      foreach (HostIdMappingData mappingData in mappingDataList)
      {
        routingKeysForMappings.Add(this.GetRoutingKey(mappingData.Id, mappingData.Qualifier));
        string str = "{" + mappingData.Id + "}";
        if (!routingKeysForMappings.Contains(str))
          routingKeysForMappings.Add(str);
      }
      return routingKeysForMappings;
    }

    public void RemoveHostIdMapping(
      IVssRequestContext requestContext,
      string providerId,
      HostIdMappingData mappingData)
    {
      requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "RemoveHostIdMapping providerId={0}, mappingData={1}", (object) providerId, (object) mappingData);
      string routingKey = this.GetRoutingKey(mappingData.Id, mappingData.Qualifier);
      this.RemoveHostIdMappingViaKey(requestContext, providerId, mappingData.PropertyName, routingKey);
    }

    public void RemoveHostIdMappingViaKey(
      IVssRequestContext requestContext,
      string providerId,
      string propertyName,
      string key)
    {
      requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "RemoveHostIdMapping providerId={0}, key={1}", (object) providerId, (object) key);
      string routingNamespace1 = this.GetRoutingNamespace(providerId, propertyName);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      INameResolutionService service = vssRequestContext.GetService<INameResolutionService>();
      try
      {
        service.DeleteEntry(vssRequestContext, routingNamespace1, key);
        string routingNamespace2 = this.GetRoutingNamespace(providerId, propertyName, true);
        service.DeleteEntry(vssRequestContext, routingNamespace2, key);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(TracingPoints.EventsRouting.Mapping, this.Area, this.Layer, ex);
      }
    }

    public void RemoveHostIdMappings(
      IVssRequestContext requestContext,
      string providerId,
      HostIdMappingData mappingData)
    {
      requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "RemoveHostIdMappings providerId={0}, mappingData={1}", (object) providerId, (object) mappingData);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string routingNamespace1 = this.GetRoutingNamespace(providerId, mappingData.PropertyName, true);
      NameResolutionEntry entryInternal = this.GetEntryInternal(vssRequestContext, routingNamespace1, mappingData.Id, (string) null);
      if (entryInternal != null)
      {
        Guid guid = entryInternal.Value;
        requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "RemoveHostIdMappings reverseLookupId={0} providerId = {1}", (object) guid, (object) providerId);
        IInternalNameResolutionService service = vssRequestContext.GetService<IInternalNameResolutionService>();
        IList<NameResolutionEntry> nameResolutionEntryList = service.QueryEntriesForValue(vssRequestContext, guid, QueryOptions.None);
        try
        {
          string routingNamespace = this.GetRoutingNamespace(providerId, mappingData.PropertyName);
          service.DeleteEntries(vssRequestContext, nameResolutionEntryList.Select<NameResolutionEntry, NameResolutionEntry>((Func<NameResolutionEntry, NameResolutionEntry>) (e => this.ChangeNamespace(e, routingNamespace))));
          service.DeleteEntries(vssRequestContext, (IEnumerable<NameResolutionEntry>) nameResolutionEntryList);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(TracingPoints.EventsRouting.Mapping, this.Area, this.Layer, ex);
        }
      }
      else
        requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Error, this.Area, this.Layer, string.Format("RemoveHostIdMappings - primary reverse lookup entry NOT FOUND. providerId={0}, mappingData={1} reverseRoutingNamespace={2}", (object) providerId, (object) mappingData, (object) routingNamespace1));
    }

    private bool TryCreateReverseLookupEntry(
      IVssRequestContext requestContext,
      INameResolutionService nameResolutionService,
      string providerId,
      HostIdMappingData mappingData,
      bool overrideExisting = false)
    {
      requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "CreateReverseLookupEntry providerId={0}, mappingData={1}", (object) providerId, (object) mappingData);
      string routingNamespace = this.GetRoutingNamespace(providerId, mappingData.PropertyName, true);
      Guid guid = Guid.NewGuid();
      try
      {
        if (!string.IsNullOrEmpty(mappingData.Qualifier))
        {
          NameResolutionEntry entryInternal = this.GetEntryInternal(requestContext, routingNamespace, mappingData.Id, (string) null);
          if (entryInternal != null)
          {
            guid = entryInternal.Value;
            requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "CreateReverseLookupEntry reverseLookupId={0} providerId={1}", (object) guid, (object) providerId);
          }
          else
          {
            requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Error, this.Area, this.Layer, string.Format("CreateReverseLookupEntry - no primary reverse lookup entry found! providerId={0}, mappingData={1}", (object) providerId, (object) mappingData));
            string routingKey = this.GetRoutingKey(mappingData.Id, (string) null);
            this.CreateEntry(nameResolutionService, requestContext, routingNamespace, routingKey, guid);
          }
        }
        string routingKey1 = this.GetRoutingKey(mappingData.Id, mappingData.Qualifier);
        bool overrideExisting1 = overrideExisting && !string.IsNullOrEmpty(mappingData.Qualifier);
        this.CreateEntry(nameResolutionService, requestContext, routingNamespace, routingKey1, guid, overrideExisting1);
        return true;
      }
      catch (NameResolutionEntryAlreadyExistsException ex)
      {
        requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, string.Format("CreateReverseLookupEntry - Create failed but the entry already exists. providerId={0}, mappingData={1}", (object) providerId, (object) mappingData));
        return true;
      }
      catch (AccessCheckException ex)
      {
        requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Error, this.Area, this.Layer, "CreateReverseLookupEntry - Access check failed. TFS still doesn't have permissions to update MPS on the namespace: " + routingNamespace + ".");
        return false;
      }
    }

    protected bool HostHasActiveEntries(IVssRequestContext requestContext, Guid host)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      int num = vssRequestContext.GetService<IInternalNameResolutionService>().QueryEntriesForValue(vssRequestContext, host, QueryOptions.None).Any<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (e => e.Namespace == "Collection" && e.IsEnabled)) ? 1 : 0;
      if (num != 0)
        return num != 0;
      requestContext.TraceAlways(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, string.Format("HostHasActiveEntries - Host {0} is deleted", (object) host));
      return num != 0;
    }

    private string GetRoutingNamespace(
      string providerId,
      string mappingProperty,
      bool isReverseLookup = false)
    {
      return isReverseLookup ? providerId + "-" + mappingProperty + "-reverseLookup" : providerId + "-" + mappingProperty;
    }

    private NameResolutionEntry ChangeNamespace(
      NameResolutionEntry originalEntry,
      string routingNamespace)
    {
      NameResolutionEntry nameResolutionEntry = originalEntry.Clone();
      nameResolutionEntry.Namespace = routingNamespace;
      return nameResolutionEntry;
    }

    private void CreateEntry(
      INameResolutionService nameResolutionService,
      IVssRequestContext requestContext,
      string routingNamespace,
      string key,
      Guid value,
      bool overrideExisting = false)
    {
      requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "CreateEntry routingNamespace={0}, key={1}, value={2}", (object) routingNamespace, (object) key, (object) value);
      NameResolutionEntry entry = new NameResolutionEntry(routingNamespace, key)
      {
        Value = value,
        IsEnabled = true
      };
      nameResolutionService.SetEntry(requestContext, entry, overrideExisting);
      requestContext.GetService<NameResolutionStore>().DeleteEntry(requestContext, routingNamespace, key);
    }

    private NameResolutionEntry GetEntryInternal(
      IVssRequestContext requestContext,
      string routingNamespace,
      string id,
      string qualifier,
      bool tryFallbackKeys = true)
    {
      requestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "GetEntryInternal routingNamespace={0}, id={1}, qualifier={2}", (object) routingNamespace, (object) id, (object) qualifier);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      NameResolutionEntry entry = (NameResolutionEntry) null;
      string routingKey1 = this.GetRoutingKey(id, qualifier);
      if (!this.TryQueryEntry(vssRequestContext, routingNamespace, routingKey1, out entry))
      {
        vssRequestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "GetEntryInternal NOT FOUND. key={0}", (object) routingKey1);
        if (!tryFallbackKeys)
          return entry;
        if (qualifier != null)
        {
          string routingKey2 = this.GetRoutingKey(id, (string) null);
          if (!this.TryQueryEntry(vssRequestContext, routingNamespace, routingKey2, out entry))
            vssRequestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "GetEntryInternal NOT FOUND for root key. key={0}", (object) routingKey2);
        }
        if (entry == null)
        {
          string key = id;
          if (!this.TryQueryEntry(vssRequestContext, routingNamespace, key, out entry))
            vssRequestContext.Trace(TracingPoints.EventsRouting.Mapping, TraceLevel.Info, this.Area, this.Layer, "GetEntryInternal NOT FOUND for back compat (just id). key={0}", (object) key);
        }
      }
      return entry;
    }

    private bool TryQueryEntry(
      IVssRequestContext deploymentRequestContext,
      string routingNamespace,
      string key,
      out NameResolutionEntry entry)
    {
      INameResolutionService service = deploymentRequestContext.GetService<INameResolutionService>();
      entry = service.QueryEntry(deploymentRequestContext, routingNamespace, key);
      if (entry != null)
        return true;
      deploymentRequestContext.GetService<NameResolutionStore>().DeleteEntry(deploymentRequestContext, routingNamespace, key);
      return false;
    }

    private string GetRoutingKey(string id, string qualifier) => string.IsNullOrEmpty(qualifier) ? "{" + id + "}" : "{" + id + "}-" + qualifier;
  }
}
