// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.RegistryService.Server.VirtualCachedRegistryService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.RegistryService.Server
{
  public sealed class VirtualCachedRegistryService : 
    ICachedRegistryService,
    IVssRegistryService,
    IVssFrameworkService
  {
    private bool m_hasParent;
    private object m_notificationsLock = new object();
    private Dictionary<RegistrySettingsChangedCallback, VirtualCachedRegistryService.RegistryCallbackEntry> m_notifications = new Dictionary<RegistrySettingsChangedCallback, VirtualCachedRegistryService.RegistryCallbackEntry>(RegistrySettingsChangedCallbackComparer.Instance);
    private const string c_area = "Registry";
    private const string c_layer = "VirtualCachedRegistryService";

    public void ServiceStart(IVssRequestContext requestContext) => this.m_hasParent = this.GetParent(requestContext) != null;

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      if (!this.m_hasParent)
        return;
      foreach (VirtualCachedRegistryService.RegistryCallbackEntry registryCallbackEntry in this.m_notifications.Values)
      {
        if (registryCallbackEntry.IsFallThru)
        {
          string registeredMessage = VirtualCachedRegistryService.CreateNotificationNotUnRegisteredMessage(registryCallbackEntry.Callback);
          requestContext.Trace(97060, TraceLevel.Error, "Registry", nameof (VirtualCachedRegistryService), registeredMessage);
        }
      }
    }

    public IEnumerable<RegistryItem> Read(IVssRequestContext requestContext, in RegistryQuery query) => (IEnumerable<RegistryItem>) RegistryItem.EmptyArray;

    public IEnumerable<IEnumerable<RegistryItem>> Read(
      IVssRequestContext requestContext,
      IEnumerable<RegistryQuery> queries)
    {
      return (IEnumerable<IEnumerable<RegistryItem>>) queries.Select<RegistryQuery, RegistryItem[]>((Func<RegistryQuery, RegistryItem[]>) (s => RegistryItem.EmptyArray));
    }

    public void Write(IVssRequestContext requestContext, IEnumerable<RegistryItem> items) => throw new VirtualServiceHostException();

    public IVssRegistryService GetParent(IVssRequestContext requestContext) => (IVssRegistryService) requestContext.GetParentService<ICachedRegistryService>();

    public void RegisterNotification(
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback,
      bool fallThru,
      IEnumerable<RegistryQuery> filters,
      Guid serviceHostId = default (Guid))
    {
      lock (this.m_notificationsLock)
      {
        VirtualCachedRegistryService.RegistryCallbackEntry registryCallbackEntry1;
        if (!this.m_notifications.TryGetValue(callback, out registryCallbackEntry1))
        {
          VirtualCachedRegistryService.RegistryCallbackEntry registryCallbackEntry2 = new VirtualCachedRegistryService.RegistryCallbackEntry(callback, fallThru);
          this.m_notifications.Add(callback, registryCallbackEntry2);
        }
        else
          registryCallbackEntry1.IsFallThru |= fallThru;
      }
      if (!fallThru)
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Parent);
      if (vssRequestContext == null)
        return;
      serviceHostId = serviceHostId == Guid.Empty ? requestContext.ServiceHost.InstanceId : serviceHostId;
      vssRequestContext.GetService<ICachedRegistryService>().RegisterNotification(vssRequestContext, callback, true, filters, serviceHostId);
    }

    public void UnregisterNotification(
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback)
    {
      bool flag = false;
      lock (this.m_notificationsLock)
      {
        VirtualCachedRegistryService.RegistryCallbackEntry registryCallbackEntry;
        if (this.m_notifications.TryGetValue(callback, out registryCallbackEntry))
        {
          this.m_notifications.Remove(callback);
          flag = registryCallbackEntry.IsFallThru;
        }
      }
      if (!flag)
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Parent);
      if (vssRequestContext == null)
        return;
      vssRequestContext.GetService<ICachedRegistryService>().UnregisterNotification(vssRequestContext, callback);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static string CreateNotificationNotUnRegisteredMessage(
      RegistrySettingsChangedCallback callback)
    {
      return string.Format("A fall-thru registry notification was not unregistered prior to service host shutdown. Target Type: {0}. Callback method Name: {1}. Callback method declaring type: {2}", (object) (callback.Target?.GetType()?.FullName ?? "<null>"), (object) callback.Method.Name, (object) callback.Method.DeclaringType.FullName);
    }

    IEnumerable<RegistryItem> IVssRegistryService.Read(
      IVssRequestContext requestContext,
      in RegistryQuery query)
    {
      return this.Read(requestContext, in query);
    }

    private class RegistryCallbackEntry
    {
      public readonly RegistrySettingsChangedCallback Callback;
      public bool IsFallThru;

      public RegistryCallbackEntry(RegistrySettingsChangedCallback callback, bool isFallThru)
      {
        this.Callback = callback;
        this.IsFallThru = isFallThru;
      }
    }
  }
}
