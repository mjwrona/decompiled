// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Utility.ExtensionNameConflictService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Components;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Utility
{
  public class ExtensionNameConflictService : IExtensionNameConflictService, IVssFrameworkService
  {
    private const string ServiceLayer = "ExtensionNameConflictService";
    private IReadOnlyDictionary<string, string> m_OldExtensionIdentityToNewIdentityMap;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback), false, "/Configuration/Service/Gallery/PMP/**");
      this.LoadConflictList(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      using (systemRequestContext.TraceBlock(12062091, 12062091, "gallery", nameof (ExtensionNameConflictService), nameof (ServiceEnd)))
        systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback));
    }

    public bool ExtensionNameInConflictList(string extensionName) => this.m_OldExtensionIdentityToNewIdentityMap != null ? this.m_OldExtensionIdentityToNewIdentityMap.ContainsKey(extensionName) : throw new ArgumentNullException("ExtensionNameConflictService.m_OldExtensionIdentityToNewIdentityMap");

    public string GetNewExtensionNameIfExistsInConflictList(string extensionName)
    {
      if (this.m_OldExtensionIdentityToNewIdentityMap == null)
        throw new ArgumentNullException("ExtensionNameConflictService.m_OldExtensionIdentityToNewIdentityMap");
      string str;
      return this.m_OldExtensionIdentityToNewIdentityMap.TryGetValue(extensionName, out str) ? str : (string) null;
    }

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadConflictList(requestContext);
    }

    private void LoadConflictList(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>();
      using (PublishedExtensionComponent35 component = requestContext.CreateComponent<PublishedExtensionComponent35>())
      {
        if (component != null)
          this.m_OldExtensionIdentityToNewIdentityMap = component.GetConflictList(requestContext);
      }
      if (this.m_OldExtensionIdentityToNewIdentityMap == null)
        throw new ArgumentNullException("ExtensionNameConflictService.m_OldExtensionIdentityToNewIdentityMap");
    }
  }
}
