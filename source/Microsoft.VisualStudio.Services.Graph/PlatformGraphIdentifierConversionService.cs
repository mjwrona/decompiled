// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.PlatformGraphIdentifierConversionService
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class PlatformGraphIdentifierConversionService : 
    GraphIdentifierConversionServiceBase,
    IPlatformGraphIdentifierConversionService,
    IGraphIdentifierConversionService,
    IVssFrameworkService
  {
    private IConfigQueryable<bool> _emptyGuidAsStorageKeyConfigFlag;
    private static VssPerformanceCounter m_callsPerSecCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_GraphIdentifierConversion_CallsPerSec", nameof (PlatformGraphIdentifierConversionService));
    private IDisposableReadOnlyList<IReadIdentitiesFromDatabaseExtension> m_readIdentitiesFromDatabaseExtensions;

    public PlatformGraphIdentifierConversionService()
      : this(ConfigProxy.Create<bool>("GetIdentityKeyMapByCuidConfHelper.EmptyGuidAsStorageKeyIfIdentityKeyMapWasNotFound"))
    {
    }

    public PlatformGraphIdentifierConversionService(
      IConfigQueryable<bool> emptyGuidAsStorageKeyConfigFlag)
    {
      this._emptyGuidAsStorageKeyConfigFlag = emptyGuidAsStorageKeyConfigFlag;
    }

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      this.m_readIdentitiesFromDatabaseExtensions = systemRequestContext.GetExtensions<IReadIdentitiesFromDatabaseExtension>();
      this._emptyGuidAsStorageKeyConfigFlag.QueryByCtx<bool>(systemRequestContext);
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_readIdentitiesFromDatabaseExtensions.Dispose();
      base.ServiceEnd(systemRequestContext);
    }

    protected override Guid GetStorageKeyByGroupScopeDescriptor(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor)
    {
      IdentityScope identityScope = GraphSubjectHelper.FetchIdentityScope(requestContext, descriptor);
      return identityScope == null ? new Guid() : identityScope.LocalScopeId;
    }

    protected override IdentityKeyMap GetKeyMapByCuidBasedDescriptor(
      IVssRequestContext organizationContext,
      SubjectDescriptor descriptor)
    {
      return ((IPlatformGraphIdentifierConversionService) this).GetIdentityKeyMapByCuid(organizationContext, descriptor.GetCuid());
    }

    protected override SubjectDescriptor GetDescriptorByStorageKeyFromRemote(
      IVssRequestContext collectionOrOrganizationContext,
      Guid storageKey)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(collectionOrOrganizationContext, nameof (collectionOrOrganizationContext));
      ArgumentUtility.CheckForEmptyGuid(storageKey, nameof (storageKey));
      collectionOrOrganizationContext.TraceDataConditionally(15270431, TraceLevel.Verbose, "Graph", "GraphIdentifierConversionServiceBase", "Received input parameters", (Func<object>) (() => (object) new
      {
        storageKey = storageKey
      }), nameof (GetDescriptorByStorageKeyFromRemote));
      collectionOrOrganizationContext.CheckServiceHostId(this.ServiceHostId, (IVssFrameworkService) this);
      SubjectDescriptor subjectDescriptor;
      if (collectionOrOrganizationContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        subjectDescriptor = this.GetDescriptorByStorageKeyUsingIMS(collectionOrOrganizationContext, storageKey);
      }
      else
      {
        IdentityKeyMap identityKeyMap = ((IPlatformGraphIdentifierConversionService) this).GetIdentityKeyMapByStorageKey(collectionOrOrganizationContext, storageKey);
        collectionOrOrganizationContext.TraceDataConditionally(15270432, TraceLevel.Verbose, "Graph", "GraphIdentifierConversionServiceBase", "Read identity key map by storage key", (Func<object>) (() => (object) new
        {
          identityKeyMap = identityKeyMap
        }), nameof (GetDescriptorByStorageKeyFromRemote));
        subjectDescriptor = !identityKeyMap.IsNullOrEmpty() ? new SubjectDescriptor(identityKeyMap.SubjectType, identityKeyMap.Cuid.ToString("D")) : this.GetDescriptorByStorageKeyUsingIMS(collectionOrOrganizationContext, storageKey);
      }
      collectionOrOrganizationContext.TraceDataConditionally(15270439, TraceLevel.Verbose, "Graph", "GraphIdentifierConversionServiceBase", "Returning descriptor", (Func<object>) (() => (object) new
      {
        storageKey = storageKey,
        subjectDescriptor = subjectDescriptor
      }), nameof (GetDescriptorByStorageKeyFromRemote));
      return subjectDescriptor;
    }

    private SubjectDescriptor GetDescriptorByStorageKeyUsingIMS(
      IVssRequestContext requestContext,
      Guid storageKey)
    {
      try
      {
        requestContext.TraceEnter(10007295, "Graph", "GraphIdentifierConversionServiceBase", nameof (GetDescriptorByStorageKeyUsingIMS));
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForEmptyGuid(storageKey, nameof (storageKey));
        requestContext.TraceDataConditionally(15270452, TraceLevel.Verbose, "Graph", "GraphIdentifierConversionServiceBase", "Received input parameters", (Func<object>) (() => (object) new
        {
          storageKey = storageKey
        }), nameof (GetDescriptorByStorageKeyUsingIMS));
        requestContext.CheckServiceHostId(this.ServiceHostId, (IVssFrameworkService) this);
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          storageKey
        }, QueryMembership.None, (IEnumerable<string>) null).Single<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity != null)
        {
          requestContext.TraceDataConditionally(15270462, TraceLevel.Verbose, "Graph", "GraphIdentifierConversionServiceBase", "Found storageKey in identity service", (Func<object>) (() => (object) new
          {
            storageKey = storageKey
          }), nameof (GetDescriptorByStorageKeyUsingIMS));
          return identity.SubjectDescriptor;
        }
        IdentityScope scopeByLocalId = requestContext.GetService<IPlatformIdentityServiceInternal>().GetScopeByLocalId(requestContext, storageKey);
        SubjectDescriptor storageKeyUsingIms = scopeByLocalId != null ? scopeByLocalId.SubjectDescriptor : new SubjectDescriptor();
        if (storageKeyUsingIms == new SubjectDescriptor())
          requestContext.TraceDataConditionally(15270463, TraceLevel.Verbose, "Graph", "GraphIdentifierConversionServiceBase", "Found storageKey in scopes", (Func<object>) (() => (object) new
          {
            storageKey = storageKey
          }), nameof (GetDescriptorByStorageKeyUsingIMS));
        else
          requestContext.TraceDataConditionally(15270464, TraceLevel.Verbose, "Graph", "GraphIdentifierConversionServiceBase", "Did not find storageKey", (Func<object>) (() => (object) new
          {
            storageKey = storageKey
          }), nameof (GetDescriptorByStorageKeyUsingIMS));
        return storageKeyUsingIms;
      }
      finally
      {
        requestContext.TraceLeave(10007296, "Graph", "GraphIdentifierConversionServiceBase", nameof (GetDescriptorByStorageKeyUsingIMS));
      }
    }

    IList<IdentityKeyMap> IPlatformGraphIdentifierConversionService.UpdateIdentityKeyMaps(
      IVssRequestContext organizationContext,
      IList<IdentityKeyMap> keyMaps)
    {
      try
      {
        PlatformGraphIdentifierConversionService.m_callsPerSecCounter.Increment();
        organizationContext.TraceEnter(10007230, "Graph", "GraphIdentifierConversionServiceBase", "UpdateIdentityKeyMaps");
        organizationContext.CheckOrganizationOnlyRequestContext();
        IList<IdentityKeyMap> keyMapsToCreate = keyMaps;
        IList<IdentityKeyMap> keyMapsToMunge;
        ((IPlatformGraphIdentifierConversionService) this).CategorizeKeyMaps(organizationContext, keyMaps, out keyMapsToCreate, out keyMapsToMunge);
        this.MungeCuids(organizationContext, keyMapsToMunge);
        if (!keyMapsToCreate.Any<IdentityKeyMap>())
          return keyMaps;
        PlatformGraphIdentifierConversionService.UpdateIdentityKeyMapsInDatabase(organizationContext, keyMapsToCreate);
        return keyMapsToCreate;
      }
      finally
      {
        organizationContext.TraceLeave(10007231, "Graph", "GraphIdentifierConversionServiceBase", "UpdateIdentityKeyMaps");
      }
    }

    private static void UpdateIdentityKeyMapsInDatabase(
      IVssRequestContext organizationContext,
      IList<IdentityKeyMap> keyMaps)
    {
      organizationContext.CheckOrganizationOnlyRequestContext();
      if (keyMaps == null || keyMaps.Count == 0)
        return;
      using (IdentityKeyMapComponent component = organizationContext.CreateComponent<IdentityKeyMapComponent>())
        component.UpdateIdentityKeyMaps(keyMaps);
      PlatformGraphIdentifierConversionService.BroadcastIdentityKeyMapChangesToFramework(organizationContext, keyMaps);
    }

    private static void BroadcastIdentityKeyMapChangesToFramework(
      IVssRequestContext organizationContext,
      IList<IdentityKeyMap> keyMaps)
    {
      IdentityKeyMapChangeMessage mapChangeMessage = PlatformGraphIdentifierConversionService.GetKeyMapChangeMessage(organizationContext, keyMaps);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      TeamFoundationTask task = new TeamFoundationTask(PlatformGraphIdentifierConversionService.\u003C\u003EO.\u003C0\u003E__PublishServiceBusNotification ?? (PlatformGraphIdentifierConversionService.\u003C\u003EO.\u003C0\u003E__PublishServiceBusNotification = new TeamFoundationTaskCallback(PlatformGraphIdentifierConversionService.PublishServiceBusNotification)), (object) mapChangeMessage, 0);
      organizationContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTaskService>().AddTask(organizationContext, task);
    }

    private static IdentityKeyMapChangeMessage GetKeyMapChangeMessage(
      IVssRequestContext organizationContext,
      IList<IdentityKeyMap> keyMaps)
    {
      Guid instanceId = organizationContext.ServiceHost.InstanceId;
      if (keyMaps.Count > 1)
        return new IdentityKeyMapChangeMessage()
        {
          InstanceId = instanceId,
          ChangeType = IdentityKeyMapChangeType.BulkChange
        };
      IdentityKeyMapChangeMessage.Change[] array = keyMaps.Select<IdentityKeyMap, IdentityKeyMapChangeMessage.Change>((Func<IdentityKeyMap, IdentityKeyMapChangeMessage.Change>) (keyMap => new IdentityKeyMapChangeMessage.Change()
      {
        StorageKey = keyMap.StorageKey,
        Descriptor = new SubjectDescriptor(keyMap.SubjectType, keyMap.Cuid.ToString())
      })).ToArray<IdentityKeyMapChangeMessage.Change>();
      return new IdentityKeyMapChangeMessage()
      {
        InstanceId = instanceId,
        ChangeType = IdentityKeyMapChangeType.Updated,
        Changes = array
      };
    }

    private static void PublishServiceBusNotification(
      IVssRequestContext requestContext,
      object taskArgs)
    {
      requestContext.TraceEnter(10007240, "Graph", "GraphIdentifierConversionServiceBase", nameof (PublishServiceBusNotification));
      if (taskArgs is IdentityKeyMapChangeMessage mapChangeMessage)
      {
        try
        {
          requestContext.GetService<IMessageBusPublisherService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Graph", (object[]) new IdentityKeyMapChangeMessage[1]
          {
            mapChangeMessage
          }, false);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10007248, "Graph", "GraphIdentifierConversionServiceBase", ex);
        }
      }
      requestContext.TraceLeave(10007249, "Graph", "GraphIdentifierConversionServiceBase", nameof (PublishServiceBusNotification));
    }

    IdentityKeyMap IPlatformGraphIdentifierConversionService.GetIdentityKeyMapByCuid(
      IVssRequestContext organizationContext,
      Guid cuid)
    {
      try
      {
        PlatformGraphIdentifierConversionService.m_callsPerSecCounter.Increment();
        organizationContext.TraceEnter(10007200, "Graph", "GraphIdentifierConversionServiceBase", "GetIdentityKeyMapByCuid");
        organizationContext.CheckOrganizationOnlyRequestContext();
        if (!this.IsExchangableId(cuid))
          return (IdentityKeyMap) null;
        IGraphIdentifierConversionCacheService service = organizationContext.GetService<IGraphIdentifierConversionCacheService>();
        IdentityKeyMap identityKeyMap;
        if (service.TryGetIdentityKeyMapByCuid(organizationContext, cuid, out identityKeyMap))
        {
          organizationContext.TraceDataConditionally(10007202, TraceLevel.Info, "Graph", "GraphIdentifierConversionServiceBase", "Found IdentityKeyMap in Platform cache; CUID", (Func<object>) (() => (object) identityKeyMap), "GetIdentityKeyMapByCuid");
          if (IdentityIdChecker.IsCuid(identityKeyMap.StorageKey))
            organizationContext.TraceDataConditionally(10007204, TraceLevel.Error, "Graph", "GraphIdentifierConversionServiceBase", "Retrieved an IdentityKeyMap from PlatformCache with a StorageKey which is actually a CUID", (Func<object>) (() => (object) identityKeyMap), "GetIdentityKeyMapByCuid");
          return identityKeyMap;
        }
        IdentityKeyMap identityKeyMapFromRemote = this.GetIdentityKeyMapByCuidFromRemote(organizationContext, cuid);
        if (!identityKeyMapFromRemote.IsValid())
          return (IdentityKeyMap) null;
        if (!identityKeyMapFromRemote.IsNullOrEmpty())
          service.AddIdentityKeyMap(organizationContext, identityKeyMapFromRemote);
        organizationContext.TraceDataConditionally(10007203, TraceLevel.Info, "Graph", "GraphIdentifierConversionServiceBase", "Retrieved IdentityKeyMap from DB", (Func<object>) (() => (object) identityKeyMap), "GetIdentityKeyMapByCuid");
        if (IdentityIdChecker.IsCuid(identityKeyMapFromRemote.StorageKey))
          organizationContext.TraceDataConditionally(10007205, TraceLevel.Error, "Graph", "GraphIdentifierConversionServiceBase", "Retrieved an IdentityKeyMap from DB with a StorageKey which is actually a CUID", (Func<object>) (() => (object) identityKeyMapFromRemote), "GetIdentityKeyMapByCuid");
        return identityKeyMapFromRemote;
      }
      finally
      {
        organizationContext.TraceLeave(10007201, "Graph", "GraphIdentifierConversionServiceBase", "GetIdentityKeyMapByCuid");
      }
    }

    IdentityKeyMap IPlatformGraphIdentifierConversionService.GetIdentityKeyMapByStorageKey(
      IVssRequestContext organizationContext,
      Guid storageKey)
    {
      try
      {
        PlatformGraphIdentifierConversionService.m_callsPerSecCounter.Increment();
        organizationContext.TraceEnter(10007210, "Graph", "GraphIdentifierConversionServiceBase", "GetIdentityKeyMapByStorageKey");
        organizationContext.CheckOrganizationOnlyRequestContext();
        if (!this.IsExchangableId(storageKey))
          return (IdentityKeyMap) null;
        IGraphIdentifierConversionCacheService service = organizationContext.GetService<IGraphIdentifierConversionCacheService>();
        IdentityKeyMap identityKeyMap;
        if (service.TryGetIdentityKeyMapByStorageKey(organizationContext, storageKey, out identityKeyMap))
          return identityKeyMap;
        IdentityKeyMap storageKeyFromRemote = this.GetIdentityKeyMapByStorageKeyFromRemote(organizationContext, storageKey);
        if (!storageKeyFromRemote.IsValid())
          return (IdentityKeyMap) null;
        if (!storageKeyFromRemote.IsNullOrEmpty())
          service.AddIdentityKeyMap(organizationContext, storageKeyFromRemote);
        return storageKeyFromRemote;
      }
      finally
      {
        organizationContext.TraceLeave(10007211, "Graph", "GraphIdentifierConversionServiceBase", "GetIdentityKeyMapByStorageKey");
      }
    }

    private IdentityKeyMap GetIdentityKeyMapByCuidFromRemote(
      IVssRequestContext organizationContext,
      Guid cuid)
    {
      try
      {
        organizationContext.TraceEnter(10007290, "Graph", "GraphIdentifierConversionServiceBase", nameof (GetIdentityKeyMapByCuidFromRemote));
        organizationContext.CheckOrganizationOnlyRequestContext();
        using (IdentityKeyMapComponent component = organizationContext.CreateComponent<IdentityKeyMapComponent>())
        {
          IdentityKeyMap keyMap = component.GetIdentityKeyMapByCuid(cuid);
          if (keyMap.IsValid())
          {
            organizationContext.TraceConditionally(10007292, TraceLevel.Info, "Graph", "GraphIdentifierConversionServiceBase", (Func<string>) (() => string.Format("{0} received from db for cuid {1}", (object) keyMap, (object) cuid)));
            return keyMap;
          }
          organizationContext.TraceConditionally(10007293, TraceLevel.Info, "Graph", "GraphIdentifierConversionServiceBase", (Func<string>) (() => string.Format("No storage key received from db for cuid {0}", (object) cuid)));
          if (this._emptyGuidAsStorageKeyConfigFlag.QueryByCtx<bool>(organizationContext))
            return new IdentityKeyMap()
            {
              Cuid = cuid,
              StorageKey = Guid.Empty,
              SubjectType = "ukn"
            };
          return new IdentityKeyMap()
          {
            Cuid = cuid,
            StorageKey = cuid,
            SubjectType = "ukn"
          };
        }
      }
      finally
      {
        organizationContext.TraceLeave(10007299, "Graph", "GraphIdentifierConversionServiceBase", nameof (GetIdentityKeyMapByCuidFromRemote));
      }
    }

    private IdentityKeyMap GetIdentityKeyMapByStorageKeyFromRemote(
      IVssRequestContext organizationContext,
      Guid storageKey)
    {
      try
      {
        organizationContext.TraceEnter(10007280, "Graph", "GraphIdentifierConversionServiceBase", nameof (GetIdentityKeyMapByStorageKeyFromRemote));
        organizationContext.CheckOrganizationOnlyRequestContext();
        using (IdentityKeyMapComponent component = organizationContext.CreateComponent<IdentityKeyMapComponent>())
        {
          IdentityKeyMap keyMap = component.GetIdentityKeyMapByStorageKey(storageKey);
          if (keyMap.IsValid())
          {
            organizationContext.TraceConditionally(10007282, TraceLevel.Info, "Graph", "GraphIdentifierConversionServiceBase", (Func<string>) (() => string.Format("{0} received from db for id {1}", (object) keyMap, (object) storageKey)));
            return keyMap;
          }
          organizationContext.TraceConditionally(10007283, TraceLevel.Info, "Graph", "GraphIdentifierConversionServiceBase", (Func<string>) (() => string.Format("No identity key map received from db for id {0}", (object) storageKey)));
          return new IdentityKeyMap()
          {
            Cuid = storageKey,
            StorageKey = storageKey,
            SubjectType = "ukn"
          };
        }
      }
      finally
      {
        organizationContext.TraceLeave(10007281, "Graph", "GraphIdentifierConversionServiceBase", nameof (GetIdentityKeyMapByStorageKeyFromRemote));
      }
    }

    void IPlatformGraphIdentifierConversionService.CategorizeKeyMaps(
      IVssRequestContext requestContext,
      IList<IdentityKeyMap> keyMaps,
      out IList<IdentityKeyMap> keyMapsToCreate,
      out IList<IdentityKeyMap> keyMapsToMunge)
    {
      List<Guid> cuids = keyMaps.Select<IdentityKeyMap, Guid>((Func<IdentityKeyMap, Guid>) (map => map.Cuid)).ToList<Guid>();
      IDictionary<Guid, IdentityKeyMap> preexistingKeyMaps;
      using (IdentityKeyMapComponent component = requestContext.CreateComponent<IdentityKeyMapComponent>())
        preexistingKeyMaps = component.GetIdentityKeyMapsByCuids((IEnumerable<Guid>) cuids);
      if (preexistingKeyMaps.IsNullOrEmpty<KeyValuePair<Guid, IdentityKeyMap>>())
      {
        requestContext.TraceDataConditionally(1030741, TraceLevel.Verbose, "Graph", "GraphIdentifierConversionServiceBase", "Did not find any preexisting key maps", (Func<object>) (() => (object) new
        {
          cuids = cuids
        }), "CategorizeKeyMaps");
        keyMapsToCreate = keyMaps;
        keyMapsToMunge = (IList<IdentityKeyMap>) Array.Empty<IdentityKeyMap>();
      }
      else
      {
        requestContext.TraceDataConditionally(1030742, TraceLevel.Verbose, "Graph", "GraphIdentifierConversionServiceBase", "Found preexisting key maps", (Func<object>) (() => (object) new
        {
          cuids = cuids,
          preexistingKeyMaps = preexistingKeyMaps
        }), "CategorizeKeyMaps");
        keyMapsToCreate = (IList<IdentityKeyMap>) new List<IdentityKeyMap>();
        keyMapsToMunge = (IList<IdentityKeyMap>) new List<IdentityKeyMap>();
        foreach (IdentityKeyMap keyMap in (IEnumerable<IdentityKeyMap>) keyMaps)
        {
          IdentityKeyMap newKeyMap = keyMap;
          IdentityKeyMap preexistingKeyMap = preexistingKeyMaps.GetValueOrDefault<Guid, IdentityKeyMap>(newKeyMap.Cuid);
          if (preexistingKeyMap == null)
          {
            keyMapsToCreate.Add(newKeyMap);
            requestContext.TraceDataConditionally(1030743, TraceLevel.Verbose, "Graph", "GraphIdentifierConversionServiceBase", "Creating new key map", (Func<object>) (() => (object) new
            {
              newKeyMap = newKeyMap
            }), "CategorizeKeyMaps");
          }
          else if (preexistingKeyMap.StorageKey != newKeyMap.StorageKey || preexistingKeyMap.SubjectType != newKeyMap.SubjectType)
          {
            keyMapsToMunge.Add(preexistingKeyMap);
            keyMapsToCreate.Add(newKeyMap);
            requestContext.TraceDataConditionally(1030744, TraceLevel.Verbose, "Graph", "GraphIdentifierConversionServiceBase", "Munging preexisting key map and creating new key map", (Func<object>) (() => (object) new
            {
              preexistingKeyMap = preexistingKeyMap,
              newKeyMap = newKeyMap
            }), "CategorizeKeyMaps");
          }
          else
            requestContext.TraceDataConditionally(1030745, TraceLevel.Verbose, "Graph", "GraphIdentifierConversionServiceBase", "Ignoring key map equal to preexisting key map", (Func<object>) (() => (object) new
            {
              preexistingKeyMap = preexistingKeyMap,
              newKeyMap = newKeyMap
            }), "CategorizeKeyMaps");
        }
      }
    }

    private void MungeCuids(IVssRequestContext requestContext, IList<IdentityKeyMap> keyMapsToMunge)
    {
      if (keyMapsToMunge.IsNullOrEmpty<IdentityKeyMap>())
        return;
      foreach (IdentityKeyMap identityKeyMap in (IEnumerable<IdentityKeyMap>) keyMapsToMunge)
      {
        IdentityKeyMap keyMap = identityKeyMap;
        Guid previousCuid = keyMap.Cuid;
        keyMap.Cuid = this.GetMungedCuid();
        requestContext.TraceDataConditionally(1030754, TraceLevel.Verbose, "Graph", "GraphIdentifierConversionServiceBase", "Munged CUID of key map", (Func<object>) (() => (object) new
        {
          keyMap = keyMap,
          previousCuid = previousCuid
        }), nameof (MungeCuids));
      }
      PlatformGraphIdentifierConversionService.UpdateIdentityKeyMapsInDatabase(requestContext, keyMapsToMunge);
    }

    private Guid GetMungedCuid()
    {
      byte[] byteArray = Guid.NewGuid().ToByteArray();
      byteArray[7] %= (byte) 16;
      return new Guid(byteArray);
    }
  }
}
