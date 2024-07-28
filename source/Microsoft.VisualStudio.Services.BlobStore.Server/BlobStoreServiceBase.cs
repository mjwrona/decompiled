// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.BlobStoreServiceBase
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public abstract class BlobStoreServiceBase : ArtifactsServiceBase
  {
    private readonly List<StrongBoxItemChangeHandler> strongBoxItemChangeHandlers = new List<StrongBoxItemChangeHandler>();

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext deploymentRequestContext = systemRequestContext.GetElevatedDeploymentRequestContext();
      foreach (StrongBoxItemChangeHandler itemChangeHandler in this.strongBoxItemChangeHandlers)
        deploymentRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(deploymentRequestContext, new StrongBoxItemChangedCallback(itemChangeHandler.OnStrongBoxItemChanged));
      this.strongBoxItemChangeHandlers.Clear();
      base.ServiceEnd(systemRequestContext);
    }

    protected virtual IEnumerable<StrongBoxConnectionString> GetAzureConnectionStrings(
      IVssRequestContext tfsRequestContext,
      PhysicalDomainInfo physicalDomainInfo)
    {
      return StorageAccountConfigurationFacade.ReadAllStorageAccounts(tfsRequestContext.GetElevatedDeploymentRequestContext(), physicalDomainInfo);
    }

    protected virtual void RegisterStrongBoxChangedEvents(
      IVssRequestContext tfsRequestContext,
      StrongBoxChangeHandlerMapping changeHandlerMapping,
      ISecretItemChangeListener listener)
    {
      IVssRequestContext deploymentRequestContext = tfsRequestContext.GetElevatedDeploymentRequestContext();
      ITeamFoundationStrongBoxService service = deploymentRequestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemChangeHandler itemChangeHandler = new StrongBoxItemChangeHandler(listener);
      IVssRequestContext requestContext = deploymentRequestContext;
      StrongBoxItemChangedCallback callback = new StrongBoxItemChangedCallback(itemChangeHandler.OnStrongBoxItemChanged);
      string drawerName = changeHandlerMapping.DrawerName;
      string[] filters = new string[1]
      {
        changeHandlerMapping.KeyName
      };
      service.RegisterNotification(requestContext, callback, drawerName, (IEnumerable<string>) filters);
      this.strongBoxItemChangeHandlers.Add(itemChangeHandler);
    }
  }
}
