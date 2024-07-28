// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.TfsCollectionRedirectedService
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  public class TfsCollectionRedirectedService : ICollectionRedirectionService, IVssFrameworkService
  {
    private TfsCollectionRedirectedService.RedirectSettings m_redirectSettings;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckProjectCollectionRequestContext();
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.Update), "/Service/SearchShared/Setting/RedirectCollectionGuid");
      Interlocked.CompareExchange<TfsCollectionRedirectedService.RedirectSettings>(ref this.m_redirectSettings, new TfsCollectionRedirectedService.RedirectSettings(systemRequestContext), (TfsCollectionRedirectedService.RedirectSettings) null);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.Update));

    public T GetClient<T>(IVssRequestContext requestContext, VssHttpClientOptions httpClientOptions = null) where T : VssHttpClientBase
    {
      TfsCollectionRedirectedService.RedirectSettings redirectSettings = this.m_redirectSettings;
      return (redirectSettings != null ? (redirectSettings.HasRedirection ? 1 : 0) : 0) == 0 ? requestContext.GetClient<T>(httpClientOptions) : this.m_redirectSettings.GetRedirectedClient<T>(requestContext, httpClientOptions);
    }

    public T GetClient<T>(IVssRequestContext requestContext, Guid serviceAreaId) where T : VssHttpClientBase
    {
      TfsCollectionRedirectedService.RedirectSettings redirectSettings = this.m_redirectSettings;
      return (redirectSettings != null ? (redirectSettings.HasRedirection ? 1 : 0) : 0) == 0 ? requestContext.GetClient<T>(serviceAreaId) : this.m_redirectSettings.GetRedirectedClient<T>(requestContext);
    }

    public string GetCollectionName(IVssRequestContext requestContext)
    {
      TfsCollectionRedirectedService.RedirectSettings redirectSettings = this.m_redirectSettings;
      return (redirectSettings != null ? (redirectSettings.HasRedirection ? 1 : 0) : 0) == 0 ? requestContext.GetCollectionName() : this.m_redirectSettings.RedirectedCollection.Name;
    }

    public Uri GetCollectionUrl(IVssRequestContext requestContext)
    {
      TfsCollectionRedirectedService.RedirectSettings redirectSettings = this.m_redirectSettings;
      return (redirectSettings != null ? (redirectSettings.HasRedirection ? 1 : 0) : 0) == 0 ? TfsCollectionRedirectedService.GetCollectionUrlFromRequestContext(requestContext) : this.m_redirectSettings.RedirectedCollectionTfsUri;
    }

    public bool HasCollectionRedirected() => this.m_redirectSettings != null && this.m_redirectSettings.HasRedirection;

    public T GetFeedClient<T>(IVssRequestContext requestContext) where T : VssHttpClientBase
    {
      TfsCollectionRedirectedService.RedirectSettings redirectSettings = this.m_redirectSettings;
      return (redirectSettings != null ? (redirectSettings.HasRedirection ? 1 : 0) : 0) == 0 ? requestContext.GetClient<T>() : this.m_redirectSettings.GetRedirectedFeedClient<T>(requestContext);
    }

    private static Uri GetCollectionUrlFromRequestContext(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, requestContext.GetCollectionID(), ServiceInstanceTypes.TFS);
    }

    private void Update(IVssRequestContext requestContext, RegistryEntryCollection changedEntries) => Volatile.Write<TfsCollectionRedirectedService.RedirectSettings>(ref this.m_redirectSettings, new TfsCollectionRedirectedService.RedirectSettings(requestContext));

    private class RedirectSettings
    {
      private readonly Guid m_feedServiceId = Guid.Parse("00000036-0000-8888-8000-000000000000");

      public RedirectSettings(IVssRequestContext requestContext)
      {
        Guid configValue = requestContext.GetConfigValue<Guid>("/Service/SearchShared/Setting/RedirectCollectionGuid", TeamFoundationHostType.ProjectCollection, Guid.Empty);
        if (configValue != Guid.Empty)
        {
          requestContext.TraceAlways(1080129, TraceLevel.Info, "Indexing Pipeline", "Crawl", "This collection is configured to be redirected to collection id {0}", (object) configValue);
          this.HasRedirection = true;
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          this.RedirectedCollectionTfsUri = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, configValue, ServiceInstanceTypes.TFS);
          using (ProjectCollectionHttpClient redirectedClient = this.GetRedirectedClient<ProjectCollectionHttpClient>(requestContext))
            this.RedirectedCollection = new ProjectCollectionHttpClientWrapper(requestContext, new TraceMetaData(1080129, "Indexing Pipeline", "Crawl")).GetProjectCollection(redirectedClient, configValue.ToString());
        }
        else
        {
          this.RedirectedCollectionTfsUri = (Uri) null;
          this.RedirectedCollection = (TeamProjectCollection) null;
          this.HasRedirection = false;
        }
      }

      public T GetRedirectedClient<T>(
        IVssRequestContext requestContext,
        VssHttpClientOptions httpClientOptions = null)
        where T : VssHttpClientBase
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        ICreateClient clientProvider = vssRequestContext.ClientProvider as ICreateClient;
        IVssRequestContext requestContext1 = vssRequestContext;
        Uri collectionTfsUri = this.RedirectedCollectionTfsUri;
        string logAs = FormattableString.Invariant(FormattableStringFactory.Create("orgsearch indexing onbehalf of {0}", (object) requestContext.ServiceHost.InstanceId));
        VssHttpClientOptions httpClientOptions1 = httpClientOptions;
        Guid targetServicePrincipal = new Guid();
        VssHttpClientOptions httpClientOptions2 = httpClientOptions1;
        return clientProvider.CreateClient<T>(requestContext1, collectionTfsUri, logAs, (ApiResourceLocationCollection) null, targetServicePrincipal: targetServicePrincipal, httpClientOptions: httpClientOptions2);
      }

      public T GetRedirectedFeedClient<T>(IVssRequestContext requestContext) where T : VssHttpClientBase
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IUrlHostResolutionService service = vssRequestContext.GetService<IUrlHostResolutionService>();
        ICreateClient clientProvider = vssRequestContext.ClientProvider as ICreateClient;
        IVssRequestContext requestContext1 = vssRequestContext;
        Guid id = this.RedirectedCollection.Id;
        Guid feedServiceId = this.m_feedServiceId;
        Uri hostUri = service.GetHostUri(requestContext1, id, feedServiceId);
        return clientProvider.CreateClient<T>(vssRequestContext, hostUri, FormattableString.Invariant(FormattableStringFactory.Create("orgsearch package indexing onbehalf of {0}", (object) requestContext.ServiceHost.InstanceId)), (ApiResourceLocationCollection) null);
      }

      public bool HasRedirection { get; set; }

      public Uri RedirectedCollectionTfsUri { get; set; }

      public TeamProjectCollection RedirectedCollection { get; set; }
    }
  }
}
