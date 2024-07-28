// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.VssSignalRHubGroupCache
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.SignalR.DataAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.VisualStudio.Services.SignalR
{
  internal sealed class VssSignalRHubGroupCache : VssVersionedCacheService<VssSignalRHubCacheData>
  {
    private const string c_layer = "HubGroupCache";
    private const string c_maxCacheSize = "MaxCacheSize";
    private const string c_inactivityInterval = "InactivityInterval";
    private const string c_hubSettingsPrefix = "/Service/SignalR/Settings/Hubs/";
    private const string c_hubSettingsMaxCacheSizeFormat = "/Service/SignalR/Settings/Hubs/{0}/MaxCacheSize";
    private const string c_hubSettingsInactivityIntervalFormat = "/Service/SignalR/Settings/Hubs/{0}/InactivityInterval";
    internal static readonly int DefaultMaxCacheSize = 64;
    internal static readonly TimeSpan DefaultInactivityInterval = TimeSpan.FromMinutes(15.0);
    private static readonly RegistryQuery s_hubSettingsQuery = (RegistryQuery) "/Service/SignalR/Settings/Hubs/...";

    public void AddConnections(
      IVssRequestContext requestContext,
      VssSignalRHubGroupId groupId,
      string connectionId,
      Func<IVssRequestContext, VssSignalRHubGroupId, string, VssSignalRHubGroupConnection> updateBackingStore)
    {
      this.Synchronize<VssSignalRHubGroupConnection>(requestContext, (Func<VssSignalRHubGroupConnection>) (() => updateBackingStore(requestContext, groupId, connectionId)), (Action<VssSignalRHubCacheData, VssSignalRHubGroupConnection>) ((cache, addedConnection) =>
      {
        VssSignalRHubGroup hubGroup;
        if (addedConnection == null || !cache.TryGetHubGroup(requestContext, addedConnection.GroupId, out hubGroup))
          return;
        hubGroup.Connections.Add(addedConnection);
      }));
    }

    public VssSignalRHubGroup GetGroup(
      IVssRequestContext requestContext,
      string hubName,
      string groupName,
      Func<IVssRequestContext, VssSignalRHubGroupId, VssSignalRHubGroup> readFromBackingStore)
    {
      VssSignalRHubGroup hubGroup = (VssSignalRHubGroup) null;
      VssSignalRHubGroupId groupId = new VssSignalRHubGroupId(hubName, groupName);
      if (!this.TryRead(requestContext, (Func<VssSignalRHubCacheData, bool>) (cache => cache.TryGetHubGroup(requestContext, groupId, out hubGroup))))
        hubGroup = this.Synchronize<VssSignalRHubGroup>(requestContext, (Func<VssSignalRHubGroup>) (() => readFromBackingStore(requestContext, groupId)), (Action<VssSignalRHubCacheData, VssSignalRHubGroup>) ((cache, storedGroup) =>
        {
          if (storedGroup == null)
            return;
          cache.AddHubGroup(requestContext, storedGroup);
        }));
      return hubGroup;
    }

    public void RemoveGroup(IVssRequestContext requestContext, string hubName, string groupName) => this.Invalidate<VssSignalRHubCacheData>(requestContext, (Func<VssSignalRHubCacheData, VssSignalRHubCacheData>) (cache =>
    {
      cache.RemoveHubGroup(requestContext, new VssSignalRHubGroupId(hubName, groupName));
      return cache;
    }));

    public void RemoveConnection(
      IVssRequestContext requestContext,
      VssSignalRHubGroupId groupId,
      string connectionId,
      Func<IVssRequestContext, VssSignalRHubGroupId, string, IList<VssSignalRHubGroupConnection>> updateBackingStore)
    {
      this.Synchronize<IList<VssSignalRHubGroupConnection>>(requestContext, (Func<IList<VssSignalRHubGroupConnection>>) (() => updateBackingStore(requestContext, groupId, connectionId)), (Action<VssSignalRHubCacheData, IList<VssSignalRHubGroupConnection>>) ((cache, removedConnections) =>
      {
        foreach (IGrouping<VssSignalRHubGroupId, VssSignalRHubGroupConnection> grouping in removedConnections.GroupBy<VssSignalRHubGroupConnection, VssSignalRHubGroupId>((Func<VssSignalRHubGroupConnection, VssSignalRHubGroupId>) (x => x.GroupId)))
        {
          VssSignalRHubGroup hubGroup;
          if (cache.TryGetHubGroup(requestContext, grouping.Key, out hubGroup))
          {
            foreach (VssSignalRHubGroupConnection rhubGroupConnection in (IEnumerable<VssSignalRHubGroupConnection>) grouping)
              hubGroup.Connections.Remove(rhubGroupConnection);
          }
        }
      }));
    }

    protected override VssSignalRHubCacheData InitializeCache(IVssRequestContext requestContext) => new VssSignalRHubCacheData(this, this.InitializeHubs(requestContext));

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.RegisterNotification(requestContext, "Default", VssSignalRSqlNotificationEventIds.HubGroupConnectionAdded, new SqlNotificationHandler(this.OnHubGroupConnectionsChanged), true);
      service.RegisterNotification(requestContext, "Default", VssSignalRSqlNotificationEventIds.HubGroupConnectionRemoved, new SqlNotificationHandler(this.OnHubGroupConnectionsChanged), true);
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnHubSettingsChanged), in VssSignalRHubGroupCache.s_hubSettingsQuery);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(requestContext, "Default", VssSignalRSqlNotificationEventIds.HubGroupConnectionAdded, new SqlNotificationHandler(this.OnHubGroupConnectionsChanged), false);
      service.UnregisterNotification(requestContext, "Default", VssSignalRSqlNotificationEventIds.HubGroupConnectionRemoved, new SqlNotificationHandler(this.OnHubGroupConnectionsChanged), false);
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnHubSettingsChanged));
      base.ServiceEnd(requestContext);
    }

    private IDictionary<string, VssSignalRHub> InitializeHubs(IVssRequestContext requestContext)
    {
      Dictionary<string, VssSignalRHub> dictionary = new Dictionary<string, VssSignalRHub>((IEqualityComparer<string>) StringComparer.Ordinal);
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, VssSignalRHubGroupCache.s_hubSettingsQuery);
      foreach (HubDescriptor hub in (IEnumerable<HubDescriptor>) GlobalHost.DependencyResolver.Resolve<IHubDescriptorProvider>().GetHubs())
      {
        string path1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/SignalR/Settings/Hubs/{0}/MaxCacheSize", (object) hub.Name);
        string path2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/SignalR/Settings/Hubs/{0}/InactivityInterval", (object) hub.Name);
        int valueFromPath1 = registryEntryCollection.GetValueFromPath<int>(path1, VssSignalRHubGroupCache.DefaultMaxCacheSize);
        TimeSpan valueFromPath2 = registryEntryCollection.GetValueFromPath<TimeSpan>(path2, VssSignalRHubGroupCache.DefaultInactivityInterval);
        dictionary.Add(hub.Name, new VssSignalRHub(this, hub.Name, valueFromPath1, valueFromPath2));
      }
      return (IDictionary<string, VssSignalRHub>) dictionary;
    }

    private void OnHubSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.Synchronize(requestContext, (Action) (() => { }), (Action<VssSignalRHubCacheData>) (cache =>
      {
        int length = "/Service/SignalR/Settings/Hubs/".Length;
        foreach (RegistryEntry changedEntry in changedEntries)
        {
          bool flag1 = changedEntry.Name.Equals("MaxCacheSize", StringComparison.OrdinalIgnoreCase);
          bool flag2 = changedEntry.Name.Equals("InactivityInterval", StringComparison.OrdinalIgnoreCase);
          if (flag1 || flag2)
          {
            int num = changedEntry.Path.IndexOf("/", length);
            if (num < 0)
              num = changedEntry.Path.Length;
            string key = changedEntry.Path.Substring(length, num - length);
            VssSignalRHub vssSignalRhub;
            if (cache.Hubs.TryGetValue(key, out vssSignalRhub))
            {
              if (flag2)
                vssSignalRhub.InactivityInterval.Value = changedEntry.GetValue<TimeSpan>(VssSignalRHubGroupCache.DefaultInactivityInterval);
              else if (flag1)
                vssSignalRhub.MaxCacheSize.Value = changedEntry.GetValue<int>(VssSignalRHubGroupCache.DefaultMaxCacheSize);
            }
          }
        }
      }));
    }

    private void OnHubGroupConnectionsChanged(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      IList<VssSignalRHubGroupConnection> connections = this.ReadConnections(requestContext, args.Data);
      if (connections == null)
        this.Reset(requestContext);
      else
        this.Synchronize<IList<VssSignalRHubGroupConnection>>(requestContext, (Func<IList<VssSignalRHubGroupConnection>>) (() => connections), (Action<VssSignalRHubCacheData, IList<VssSignalRHubGroupConnection>>) ((cacheData, updatedConnections) =>
        {
          foreach (IGrouping<VssSignalRHubGroupId, VssSignalRHubGroupConnection> grouping in updatedConnections.GroupBy<VssSignalRHubGroupConnection, VssSignalRHubGroupId>((Func<VssSignalRHubGroupConnection, VssSignalRHubGroupId>) (x => x.GroupId)))
            cacheData.RemoveHubGroup(requestContext, grouping.Key);
        }));
    }

    private IList<VssSignalRHubGroupConnection> ReadConnections(
      IVssRequestContext requestContext,
      string eventData)
    {
      if (string.IsNullOrEmpty(eventData))
        return (IList<VssSignalRHubGroupConnection>) Array.Empty<VssSignalRHubGroupConnection>();
      List<VssSignalRHubGroupConnection> rhubGroupConnectionList = new List<VssSignalRHubGroupConnection>();
      try
      {
        using (StringReader stringReader = new StringReader(eventData))
        {
          StringReader input = stringReader;
          using (XmlReader reader1 = XmlReader.Create((TextReader) input, new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            IgnoreWhitespace = true,
            XmlResolver = (XmlResolver) null
          }))
          {
            int content = (int) reader1.MoveToContent();
            rhubGroupConnectionList.AddRange((IEnumerable<VssSignalRHubGroupConnection>) XmlUtility.ArrayOfObjectFromXml<VssSignalRHubGroupConnection>(reader1, "VssSignalRHubGroupConnection", false, (Func<XmlReader, VssSignalRHubGroupConnection>) (reader => VssSignalRHubGroupCache.DeserializeConnection(requestContext, reader))));
          }
        }
      }
      catch (Exception ex)
      {
        rhubGroupConnectionList = (List<VssSignalRHubGroupConnection>) null;
        requestContext.Trace(10017101, TraceLevel.Warning, "SignalR", "HubGroupCache", "Error: Event data passed to filter notification is not valid: {0}. {1}", (object) eventData, (object) ex);
      }
      return (IList<VssSignalRHubGroupConnection>) rhubGroupConnectionList;
    }

    private static VssSignalRHubGroupConnection DeserializeConnection(
      IVssRequestContext requestContext,
      XmlReader reader)
    {
      int num = reader.IsEmptyElement ? 1 : 0;
      VssSignalRHubGroupConnection rhubGroupConnection = new VssSignalRHubGroupConnection();
      reader.Read();
      if (num == 0)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "GroupId":
              rhubGroupConnection.GroupId = VssSignalRHubGroupCache.DeserializeHubGroupId(requestContext, reader);
              continue;
            case "ConnectionId":
              rhubGroupConnection.ConnectionId = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "UserId":
              rhubGroupConnection.UserId = XmlUtility.GuidFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return rhubGroupConnection;
    }

    private static VssSignalRHubGroupId DeserializeHubGroupId(
      IVssRequestContext requestContext,
      XmlReader reader)
    {
      int num = reader.IsEmptyElement ? 1 : 0;
      VssSignalRHubGroupId signalRhubGroupId = new VssSignalRHubGroupId();
      reader.Read();
      if (num == 0)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "HubName":
              signalRhubGroupId.HubName = XmlUtility.StringFromXmlElement(reader);
              continue;
            case "GroupName":
              signalRhubGroupId.GroupName = XmlUtility.StringFromXmlElement(reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return signalRhubGroupId;
    }
  }
}
