// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.ServiceHooksEventService
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public class ServiceHooksEventService : IVssFrameworkService
  {
    private static readonly string s_layer = typeof (ServiceHooksEventService).Name;
    private static readonly string s_area = typeof (ServiceHooksEventService).Namespace;
    private static readonly string ServiceHooksRootPath = "/Service/ServiceHooks/Settings";
    private static readonly string IgnoreSHWITRootPath = ServiceHooksEventService.ServiceHooksRootPath + "/IgnoreWorkItems";
    private List<int> m_ignoredWorkItemIds = new List<int>();

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService service = systemRequestContext.GetService<IVssRegistryService>();
      service.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ClearIgnoredWorkItems), ServiceHooksEventService.IgnoreSHWITRootPath + "/*");
      RegistryEntryCollection registryEntries = service.ReadEntriesFallThru(systemRequestContext, (RegistryQuery) (ServiceHooksEventService.IgnoreSHWITRootPath + "/*"));
      this.ReadIgnoredWorkItems(systemRequestContext, registryEntries);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ClearIgnoredWorkItems));

    private void ClearIgnoredWorkItems(
      IVssRequestContext requestContext,
      RegistryEntryCollection registryEntries)
    {
      this.m_ignoredWorkItemIds = (List<int>) null;
    }

    private void ReadIgnoredWorkItems(
      IVssRequestContext requestContext,
      RegistryEntryCollection registryEntries)
    {
      foreach (RegistryEntry registryEntry in registryEntries)
      {
        string str = registryEntry.Value;
        char[] chArray = new char[1]{ ',' };
        foreach (string s in str.Split(chArray))
        {
          int result;
          if (int.TryParse(s, out result))
            this.m_ignoredWorkItemIds.Add(result);
        }
      }
    }

    private void EnsureIgnoredWorkItemsLoaded(IVssRequestContext requestContext)
    {
      if (this.m_ignoredWorkItemIds != null)
        return;
      RegistryEntryCollection registryEntries = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) (ServiceHooksEventService.IgnoreSHWITRootPath + "/*"));
      this.m_ignoredWorkItemIds = new List<int>();
      this.ReadIgnoredWorkItems(requestContext, registryEntries);
    }

    public bool ShouldIgnoreWorkItem(IVssRequestContext requestContext, int workItemId)
    {
      this.EnsureIgnoredWorkItemsLoaded(requestContext);
      return this.m_ignoredWorkItemIds.Contains(workItemId);
    }
  }
}
