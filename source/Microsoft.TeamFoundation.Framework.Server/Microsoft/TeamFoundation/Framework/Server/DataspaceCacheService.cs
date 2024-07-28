// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataspaceCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataspaceCacheService : VssVersionedCacheService<DataspaceCacheData>
  {
    private INotificationRegistration m_deletedRegistration;
    private INotificationRegistration m_changedRegistration;
    private static readonly XmlSerializer s_dataspaceSerializer = new XmlSerializer(typeof (List<Dataspace>));
    private const string c_area = "Caching";
    private const string c_layer = "DataspaceCache";
    internal static readonly string DisableBulkLoadPath = "/Configuration/DataspaceService/DisableCacheBulkLoad";
    internal static readonly RegistryQuery DisableBulkLoadRegistryQuery = new RegistryQuery(DataspaceCacheService.DisableBulkLoadPath, false);

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      this.m_deletedRegistration = service.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.DataspaceDataDeleted, new SqlNotificationCallback(this.OnDataspaceRemoved), false, false);
      this.m_changedRegistration = service.CreateRegistration(systemRequestContext, "Default", SqlNotificationEventClasses.DataspaceDataChanged, new SqlNotificationCallback(this.OnDataspaceChanged), false, false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_deletedRegistration.Unregister(systemRequestContext);
      this.m_changedRegistration.Unregister(systemRequestContext);
      base.ServiceEnd(systemRequestContext);
    }

    public Dataspace GetDataspace(IVssRequestContext requestContext, int dataspaceId)
    {
      Dataspace dataspace;
      return this.Read<Dataspace>(requestContext, (Func<DataspaceCacheData, Dataspace>) (cacheData => cacheData.DataspacesById.TryGetValue(dataspaceId, out dataspace) ? Dataspace.Clone(dataspace) : (Dataspace) null)) ?? this.FaultInDataspace(requestContext, (Func<DataspaceComponent, Dataspace>) (component => component.QueryDataspace(dataspaceId)));
    }

    public Dataspace GetDataspace(
      IVssRequestContext requestContext,
      string dataspaceCategory,
      Guid dataspaceIdentifier)
    {
      Dictionary<Guid, Dataspace> dictionary;
      Dataspace dataspace;
      return this.Read<Dataspace>(requestContext, (Func<DataspaceCacheData, Dataspace>) (cacheData => cacheData.DataspacesByCategoryIdentifier.TryGetValue(dataspaceCategory, out dictionary) && dictionary.TryGetValue(dataspaceIdentifier, out dataspace) ? Dataspace.Clone(dataspace) : (Dataspace) null)) ?? this.FaultInDataspace(requestContext, (Func<DataspaceComponent, Dataspace>) (component => component.QueryDataspace(dataspaceCategory, dataspaceIdentifier)));
    }

    private void OnDataspaceRemoved(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.OnDataspaceChanged(requestContext, eventData);
    }

    private void OnDataspaceChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.OnDataspaceChanged(requestContext, eventData);
    }

    private void OnDataspaceChanged(IVssRequestContext requestContext, string eventData)
    {
      IEnumerable<Dataspace> dataspaces = this.FilterNotificationInternal(requestContext, eventData);
      if (dataspaces == null)
      {
        this.Reset(requestContext);
      }
      else
      {
        Func<IEnumerable<Dataspace>> writeOperation = (Func<IEnumerable<Dataspace>>) (() => dataspaces);
        this.Synchronize<IEnumerable<Dataspace>>(requestContext, writeOperation, (Action<DataspaceCacheData, IEnumerable<Dataspace>>) ((cacheData, updatedDataspaces) =>
        {
          foreach (Dataspace updatedDataspace in updatedDataspaces)
            cacheData.RemoveDataspace(requestContext, updatedDataspace);
        }));
      }
    }

    protected override DataspaceCacheData InitializeCache(IVssRequestContext requestContext)
    {
      List<Dataspace> source;
      if (!requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in DataspaceCacheService.DisableBulkLoadRegistryQuery, true, false))
      {
        using (DataspaceComponent component = requestContext.CreateComponent<DataspaceComponent>())
          source = component.QueryDataspaces();
      }
      else
        source = new List<Dataspace>();
      Dictionary<string, Dictionary<Guid, Dataspace>> dataspacesByCategoryIdentifier = new Dictionary<string, Dictionary<Guid, Dataspace>>();
      Dictionary<int, Dataspace> dataspacesById = new Dictionary<int, Dataspace>();
      foreach (string key in source.Select<Dataspace, string>((Func<Dataspace, string>) (x => x.DataspaceCategory)).Distinct<string>())
        dataspacesByCategoryIdentifier.Add(key, new Dictionary<Guid, Dataspace>());
      foreach (Dataspace dataspace in source)
      {
        dataspacesByCategoryIdentifier[dataspace.DataspaceCategory].Add(dataspace.DataspaceIdentifier, dataspace);
        dataspacesById.Add(dataspace.DataspaceId, dataspace);
      }
      return new DataspaceCacheData(dataspacesByCategoryIdentifier, dataspacesById);
    }

    private IEnumerable<Dataspace> FilterNotificationInternal(
      IVssRequestContext requestContext,
      string eventData)
    {
      if (string.IsNullOrEmpty(eventData))
        return (IEnumerable<Dataspace>) Array.Empty<Dataspace>();
      using (StringReader stringReader = new StringReader(eventData))
      {
        try
        {
          return (IEnumerable<Dataspace>) DataspaceCacheService.s_dataspaceSerializer.Deserialize((TextReader) stringReader);
        }
        catch (Exception ex)
        {
          requestContext.Trace(1002110, TraceLevel.Warning, "Caching", "DataspaceCache", "Error: Event data passed to filter notification is not valid: {0}.  EXCEPTION: {1}", (object) eventData, (object) ex);
          return (IEnumerable<Dataspace>) null;
        }
      }
    }

    private Dataspace FaultInDataspace(
      IVssRequestContext requestContext,
      Func<DataspaceComponent, Dataspace> func)
    {
      return this.Synchronize<Tuple<Dataspace, bool>>(requestContext, (Func<Tuple<Dataspace, bool>>) (() =>
      {
        using (DataspaceComponent component = requestContext.CreateComponent<DataspaceComponent>())
          return Tuple.Create<Dataspace, bool>(func(component), component.Version >= 2);
      }), (Action<DataspaceCacheData, Tuple<Dataspace, bool>>) ((cacheData, tuple) =>
      {
        if (tuple.Item1 == null || !tuple.Item2)
          return;
        cacheData.AddDataspace(requestContext, tuple.Item1);
      })).Item1;
    }
  }
}
