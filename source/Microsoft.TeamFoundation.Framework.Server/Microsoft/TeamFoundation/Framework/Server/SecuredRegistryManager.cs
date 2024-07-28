// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecuredRegistryManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SecuredRegistryManager : ISecuredRegistryManager, IVssFrameworkService
  {
    private TeamFoundationSecurityService m_securityService;
    private CachedRegistryService m_registryService;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_registryService = systemRequestContext.GetService<CachedRegistryService>();
      this.m_securityService = systemRequestContext.GetService<TeamFoundationSecurityService>();
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public List<RegistryEntry> QueryRegistryEntries(
      IVssRequestContext requestContext,
      string registryPathPattern,
      bool includeFolders)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(registryPathPattern, nameof (registryPathPattern));
      IVssSecurityNamespace securityNamespace = this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.RegistryNamespaceId);
      RegistryEntryCollection registryEntryCollection = this.m_registryService.ReadEntries(requestContext, (RegistryQuery) registryPathPattern, includeFolders);
      List<RegistryEntry> registryEntryList = new List<RegistryEntry>(registryEntryCollection.Count);
      foreach (RegistryEntry registryEntry in registryEntryCollection)
      {
        if (securityNamespace.HasPermission(requestContext, registryEntry.Path, 1))
          registryEntryList.Add(registryEntry);
      }
      return registryEntryList;
    }

    public virtual bool UserHiveOperationsAllowed(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) || this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);

    private void CheckGlobalReadPermission(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);
    }

    public List<RegistryEntry> QueryUserEntries(
      IVssRequestContext requestContext,
      string registryPathPattern,
      bool includeFolders)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(registryPathPattern, nameof (registryPathPattern));
      this.CheckGlobalReadPermission(requestContext);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      RegistryEntryCollection registryEntryCollection = this.m_registryService.ReadEntries(requestContext, userIdentity, registryPathPattern, includeFolders);
      List<RegistryEntry> registryEntryList = new List<RegistryEntry>(registryEntryCollection.Count);
      foreach (RegistryEntry registryEntry in registryEntryCollection)
        registryEntryList.Add(registryEntry);
      return registryEntryList;
    }

    public void UpdateRegistryEntries(
      IVssRequestContext requestContext,
      RegistryEntry[] registryEntries)
    {
      ArgumentUtility.CheckForNull<RegistryEntry[]>(registryEntries, nameof (registryEntries));
      IVssSecurityNamespace securityNamespace = this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.RegistryNamespaceId);
      foreach (RegistryEntry registryEntry in registryEntries)
        securityNamespace.CheckPermission(requestContext, registryEntry.Path, 2);
      this.m_registryService.WriteEntries(requestContext, (IEnumerable<RegistryEntry>) registryEntries);
    }

    public void UpdateUserEntries(
      IVssRequestContext requestContext,
      RegistryEntry[] registryEntries)
    {
      ArgumentUtility.CheckForNull<RegistryEntry[]>(registryEntries, nameof (registryEntries));
      this.CheckGlobalReadPermission(requestContext);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      this.m_registryService.WriteEntries(requestContext, userIdentity, (IEnumerable<RegistryEntry>) registryEntries);
    }

    public int RemoveRegistryEntries(
      IVssRequestContext requestContext,
      string[] registryPathPatterns)
    {
      ArgumentUtility.CheckForNull<string[]>(registryPathPatterns, nameof (registryPathPatterns));
      IVssSecurityNamespace securityNamespace = this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.RegistryNamespaceId);
      List<string> stringList = new List<string>();
      foreach (string registryPathPattern in registryPathPatterns)
      {
        foreach (RegistryEntry readEntry in this.m_registryService.ReadEntries(requestContext, (RegistryQuery) registryPathPattern))
        {
          if (securityNamespace.HasPermission(requestContext, readEntry.Path, 2))
            stringList.Add(readEntry.Path);
        }
      }
      return this.m_registryService.DeleteEntries(requestContext, stringList.ToArray());
    }

    public int RemoveUserEntries(IVssRequestContext requestContext, string[] registryPathPatterns)
    {
      ArgumentUtility.CheckForNull<string[]>(registryPathPatterns, nameof (registryPathPatterns));
      this.CheckGlobalReadPermission(requestContext);
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      List<string> stringList = new List<string>();
      foreach (string registryPathPattern in registryPathPatterns)
      {
        foreach (RegistryEntry readEntry in this.m_registryService.ReadEntries(requestContext, userIdentity, registryPathPattern))
          stringList.Add(readEntry.Path);
      }
      return this.m_registryService.DeleteEntries(requestContext, userIdentity, stringList.ToArray());
    }

    public List<RegistryAuditEntry> QueryAuditLog(
      IVssRequestContext requestContext,
      int changeIndex,
      bool returnOlder)
    {
      this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 2);
      return this.m_registryService.QueryAuditLog(requestContext, changeIndex, returnOlder);
    }
  }
}
