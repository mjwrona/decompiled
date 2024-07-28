// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IdentitySecurityNamespaceSettingsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class IdentitySecurityNamespaceSettingsService : 
    IIdentitySecurityNamespaceSettingsService,
    IVssFrameworkService
  {
    private static readonly char[] IdentitySecurityTokenDelimiter = new char[1]
    {
      '\\'
    };
    private IdentitySecurityNamespaceSettingsService.IdentityPermissionServiceSettings m_settings;
    private const string Area = "Identity";
    private const string Layer = "IdentitySecurityNamespaceSettingsService";
    private static readonly RegistryQuery s_registrySettingsQuery = (RegistryQuery) "/Configuration/IdentitySecurity/DefaultScopePermissions";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), in IdentitySecurityNamespaceSettingsService.s_registrySettingsQuery);
      Interlocked.CompareExchange<IdentitySecurityNamespaceSettingsService.IdentityPermissionServiceSettings>(ref this.m_settings, new IdentitySecurityNamespaceSettingsService.IdentityPermissionServiceSettings(requestContext), (IdentitySecurityNamespaceSettingsService.IdentityPermissionServiceSettings) null);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<IdentitySecurityNamespaceSettingsService.IdentityPermissionServiceSettings>(ref this.m_settings, new IdentitySecurityNamespaceSettingsService.IdentityPermissionServiceSettings(requestContext));
    }

    public Dictionary<GroupScopeType, Dictionary<string, ScopeWellKnownGroupPermissionEntry>> GetScopeWellKnownGroupPermissionEntries(
      IVssRequestContext requestContext)
    {
      return this.m_settings.scopePermissionsMap;
    }

    private class IdentityPermissionServiceSettings
    {
      internal readonly Dictionary<GroupScopeType, Dictionary<string, ScopeWellKnownGroupPermissionEntry>> scopePermissionsMap;

      public IdentityPermissionServiceSettings(IVssRequestContext requestContext)
      {
        string registryValue = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, in IdentitySecurityNamespaceSettingsService.s_registrySettingsQuery);
        if (registryValue.IsNullOrEmpty<char>())
          return;
        requestContext.TraceConditionally(1040081, TraceLevel.Info, "Identity", nameof (IdentitySecurityNamespaceSettingsService), (Func<string>) (() => string.Format("Registry path {0} has {1}.", (object) IdentitySecurityNamespaceSettingsService.s_registrySettingsQuery, (object) registryValue)));
        ScopeWellKnownGroupPermissionEntries permissionEntries = TeamFoundationSerializationUtility.Deserialize<ScopeWellKnownGroupPermissionEntries>(registryValue);
        this.scopePermissionsMap = new Dictionary<GroupScopeType, Dictionary<string, ScopeWellKnownGroupPermissionEntry>>();
        foreach (ScopeWellKnownGroupPermissionEntry groupPermissionEntry in permissionEntries.WellKnownGroupPermissionEntries)
        {
          GroupScopeType groupScopeType = groupPermissionEntry.GroupScopeType;
          if (this.scopePermissionsMap.ContainsKey(groupScopeType))
            this.scopePermissionsMap[groupScopeType].TryAdd<string, ScopeWellKnownGroupPermissionEntry>(groupPermissionEntry.IdentityDescriptor, groupPermissionEntry);
          else
            this.scopePermissionsMap.Add(groupScopeType, new Dictionary<string, ScopeWellKnownGroupPermissionEntry>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
            {
              {
                groupPermissionEntry.IdentityDescriptor,
                groupPermissionEntry
              }
            });
        }
      }
    }
  }
}
