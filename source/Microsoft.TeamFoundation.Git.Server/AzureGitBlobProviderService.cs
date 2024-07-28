// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.AzureGitBlobProviderService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class AzureGitBlobProviderService : IVssFrameworkService
  {
    private IBlobProvider m_blobProvider;
    private const string c_layer = "AzureGitBlobProviderService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
      {
        "GitBlobStorageConnectionString"
      });
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.InitializeBlobProvider), true, FrameworkServerConstants.FileServiceRegistryRootPath + "/...");
      systemRequestContext.GetService<ICompositeBlobProviderService>().RegisterNotification(systemRequestContext, new SecondaryBlobProvidersChanged(this.OnSecondaryBlobProvidersChanged));
      this.InitializeBlobProvider(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged));
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.InitializeBlobProvider));
      systemRequestContext.GetService<ICompositeBlobProviderService>().UnregisterNotification(systemRequestContext, new SecondaryBlobProvidersChanged(this.OnSecondaryBlobProvidersChanged));
      if (this.m_blobProvider == null)
        return;
      this.m_blobProvider.ServiceEnd(systemRequestContext);
    }

    public IBlobProvider GetBlobProvider(IVssRequestContext deploymentRC)
    {
      if (!deploymentRC.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(deploymentRC.ServiceHost.HostType);
      if (this.m_blobProvider == null)
        this.InitializeBlobProvider(deploymentRC);
      return this.m_blobProvider;
    }

    private void InitializeBlobProvider(
      IVssRequestContext deploymentRC,
      RegistryEntryCollection changedEntries = null)
    {
      string typeName = deploymentRC.GetService<IVssRegistryService>().GetValue(deploymentRC, (RegistryQuery) FrameworkServerConstants.FileServiceRemoteBlobProvider, string.Empty);
      string[] strArray = !string.IsNullOrEmpty(typeName) ? ((IEnumerable<string>) typeName.Split(',')).Select<string, string>((Func<string, string>) (part => part.Trim())).ToArray<string>() : throw new BlobProviderConfigurationException(Resources.Format("IBlobProviderTypeNotFound", (object) nameof (AzureGitBlobProviderService)));
      string remoteBlobProviderType = strArray[0];
      string remoteBlobProviderAssembly = strArray.Length >= 2 ? strArray[1] : (string) null;
      provider = (IBlobProvider) null;
      Type type1 = Type.GetType(typeName);
      if (type1 != (Type) null)
      {
        if (!(Activator.CreateInstance(type1) is IBlobProvider provider))
          deploymentRC.Trace(14449, TraceLevel.Warning, GitServerUtils.TraceArea, nameof (AzureGitBlobProviderService), "Type '" + typeName + "' does not implement IBlobProvider");
      }
      else
        deploymentRC.Trace(14448, TraceLevel.Warning, GitServerUtils.TraceArea, nameof (AzureGitBlobProviderService), "Could not find type: " + typeName);
      if (provider == null)
        provider = deploymentRC.GetExtension<IBlobProvider>((Func<IBlobProvider, bool>) (x =>
        {
          Type type2 = x.GetType();
          if (!(type2.FullName == remoteBlobProviderType))
            return false;
          return string.IsNullOrEmpty(remoteBlobProviderAssembly) || remoteBlobProviderAssembly == type2.Assembly.GetName().Name;
        }));
      if (provider != null)
      {
        Type type3 = provider.GetType();
        deploymentRC.Trace(1013011, TraceLevel.Info, GitServerUtils.TraceArea, nameof (AzureGitBlobProviderService), "Selected and starting IBlobProvider plugin candidate {0}", (object) type3);
        string connectionString = this.GetStorageConnectionString(deploymentRC);
        Dictionary<string, string> settings = (Dictionary<string, string>) null;
        if (!string.IsNullOrEmpty(connectionString))
          settings = new Dictionary<string, string>()
          {
            ["BlobStorageConnectionStringOverride"] = connectionString,
            ["DrawerName"] = "ConfigurationSecrets",
            ["LookupKey"] = "GitBlobStorageConnectionString"
          };
        provider.ServiceStart(deploymentRC, (IDictionary<string, string>) settings);
        ICompositeBlobProviderService service = deploymentRC.GetService<ICompositeBlobProviderService>();
        if (service.HasSecondaryProviders(deploymentRC))
          provider = service.CreateCompositeBlobProvider(deploymentRC, provider);
        this.m_blobProvider?.ServiceEnd(deploymentRC);
        this.m_blobProvider = provider;
        deploymentRC.Trace(1013012, TraceLevel.Info, GitServerUtils.TraceArea, nameof (AzureGitBlobProviderService), "Plugin {0} started successfully", (object) type3);
      }
      if (this.m_blobProvider == null)
        throw new BlobProviderConfigurationException(Resources.Format("BlobProviderNoPluginsFound", (object) typeName));
    }

    private string GetStorageConnectionString(IVssRequestContext deploymentContext)
    {
      ITeamFoundationStrongBoxService service = deploymentContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(deploymentContext, "ConfigurationSecrets", "GitBlobStorageConnectionString", true);
      return service.GetString(deploymentContext, itemInfo);
    }

    private void OnStrongBoxItemChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      if (!itemNames.Any<StrongBoxItemName>((Func<StrongBoxItemName, bool>) (i => !i.LookupKey.EndsWith("-previous"))))
        return;
      this.InitializeBlobProvider(requestContext);
    }

    public void OnSecondaryBlobProvidersChanged(IVssRequestContext requestContext)
    {
      requestContext.TraceAlways(1013815, TraceLevel.Info, GitServerUtils.TraceArea, nameof (AzureGitBlobProviderService), "Secondary blob providers changed, re-initializing blob providers.");
      this.InitializeBlobProvider(requestContext);
    }
  }
}
