// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Framework.StagedDeploymentService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Framework
{
  public class StagedDeploymentService : IStagedDeploymentService, IVssFrameworkService
  {
    private const string StagedDeploymentServiceSettingsRoot = "/Service/StagedDeployment";
    private const string StagedDeploymentServiceRegistryNotificationFilter = "/Service/StagedDeployment/...";
    private const int DefaultRegistryEntryPercentage = 0;

    private ServiceFactory<IVssRegistryService> RegistryService { get; set; }

    private RegistryEntryCollection RegistryEntries { get; set; }

    public StagedDeploymentService()
      : this((ServiceFactory<IVssRegistryService>) (requestContext => (IVssRegistryService) requestContext.GetService<CachedRegistryService>()))
    {
    }

    public StagedDeploymentService(
      ServiceFactory<IVssRegistryService> registryService)
    {
      this.RegistryService = registryService;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.RegistryService(systemRequestContext).RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/StagedDeployment/...");
      this.Initialize(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.RegistryService(systemRequestContext).UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    public bool AllowNewEntryThrough(
      IVssRequestContext requestContext,
      string percentValueName,
      string entry)
    {
      return this.AllowNewEntryThrough(requestContext, percentValueName, entry.GetHashCode());
    }

    public bool AllowNewEntryThrough(
      IVssRequestContext requestContext,
      string percentValueName,
      Guid entry)
    {
      return this.AllowNewEntryThrough(requestContext, percentValueName, entry.GetHashCode());
    }

    public bool AllowNewEntryThrough(
      IVssRequestContext requestContext,
      string percentValueName,
      int hashCode)
    {
      return (hashCode & int.MaxValue) % 100 + 1 <= this.RegistryEntries.GetValueFromPath<int>("/Service/StagedDeployment/" + percentValueName, 0);
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.Initialize(requestContext);
    }

    private void Initialize(IVssRequestContext requestContext) => this.RegistryEntries = this.RegistryService(requestContext).ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/StagedDeployment/*");
  }
}
