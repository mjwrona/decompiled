// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MetricsQueryClientFactory
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Azure.Core;
using Azure.Identity;
using Azure.Monitor.Query;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public sealed class MetricsQueryClientFactory : IMetricsQueryClientFactory
  {
    private readonly bool m_useManagedIdentity;
    private readonly string m_clientId;
    private readonly X509Certificate2 m_servicePrincipalCertificate;
    private readonly ConcurrentDictionary<MetricsQueryClientFactory.Key, MetricsQueryClient> m_MetricsQueryClients = new ConcurrentDictionary<MetricsQueryClientFactory.Key, MetricsQueryClient>(2, 1);
    private static MetricsQueryClientFactory s_managedIdentityFactory;
    private static readonly ConcurrentDictionary<MetricsQueryClientFactory.ServicePrincipalKey, MetricsQueryClientFactory> s_servicePrincipalFactories = new ConcurrentDictionary<MetricsQueryClientFactory.ServicePrincipalKey, MetricsQueryClientFactory>(2, 1);
    private static readonly ConcurrentDictionary<string, MetricsQueryClientFactory> s_userAssignedManagedIdentityFactories = new ConcurrentDictionary<string, MetricsQueryClientFactory>(2, 1, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    private MetricsQueryClientFactory(bool useManagedIdentity = false, string clientId = null)
    {
      this.m_clientId = clientId;
      this.m_useManagedIdentity = useManagedIdentity;
    }

    private MetricsQueryClientFactory(
      string servicePrincipalClientId,
      string servicePrincipalCertificateThumbprint)
    {
      this.m_clientId = servicePrincipalClientId;
      this.m_servicePrincipalCertificate = new CertHandler().FindCertificateByThumbprint(servicePrincipalCertificateThumbprint);
    }

    public static MetricsQueryClientFactory Default { get; } = new MetricsQueryClientFactory();

    public static MetricsQueryClientFactory UsingManagedIdentity(string clientId = null)
    {
      if (string.IsNullOrWhiteSpace(clientId))
      {
        if (MetricsQueryClientFactory.s_managedIdentityFactory == null)
          MetricsQueryClientFactory.s_managedIdentityFactory = new MetricsQueryClientFactory(true);
        return MetricsQueryClientFactory.s_managedIdentityFactory;
      }
      MetricsQueryClientFactory queryClientFactory;
      if (!MetricsQueryClientFactory.s_userAssignedManagedIdentityFactories.TryGetValue(clientId, out queryClientFactory))
      {
        queryClientFactory = new MetricsQueryClientFactory(true, clientId);
        MetricsQueryClientFactory.s_userAssignedManagedIdentityFactories.TryAdd(clientId, queryClientFactory);
      }
      return queryClientFactory;
    }

    public static MetricsQueryClientFactory UsingServicePrincipal(
      string servicePrincipalClientId,
      string servicePrincipalCertificateThumbprint)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(servicePrincipalClientId, nameof (servicePrincipalClientId));
      ArgumentUtility.CheckStringForNullOrEmpty(servicePrincipalCertificateThumbprint, nameof (servicePrincipalCertificateThumbprint));
      MetricsQueryClientFactory.ServicePrincipalKey key = new MetricsQueryClientFactory.ServicePrincipalKey(servicePrincipalClientId, servicePrincipalCertificateThumbprint);
      MetricsQueryClientFactory queryClientFactory;
      if (!MetricsQueryClientFactory.s_servicePrincipalFactories.TryGetValue(key, out queryClientFactory))
      {
        queryClientFactory = new MetricsQueryClientFactory(servicePrincipalClientId, servicePrincipalCertificateThumbprint);
        MetricsQueryClientFactory.s_servicePrincipalFactories.TryAdd(key, queryClientFactory);
      }
      return queryClientFactory;
    }

    public MetricsQueryClient GetMetricsQueryClient(string tenantId) => this.GetMetricsQueryClient(tenantId, AadSupportedResources.AzureResourceManager);

    public MetricsQueryClient GetMetricsQueryClient(
      string tenantId,
      string resourceManagerEndpointUrl)
    {
      if (!this.m_useManagedIdentity)
        ArgumentUtility.CheckStringForNullOrEmpty(tenantId, nameof (tenantId));
      if (string.IsNullOrWhiteSpace(resourceManagerEndpointUrl))
        resourceManagerEndpointUrl = AadSupportedResources.AzureResourceManager;
      MetricsQueryClientFactory.Key key = new MetricsQueryClientFactory.Key(tenantId, resourceManagerEndpointUrl);
      MetricsQueryClient metricsQueryClient;
      if (!this.m_MetricsQueryClients.TryGetValue(key, out metricsQueryClient))
      {
        metricsQueryClient = new MetricsQueryClient(this.CreateCredential(tenantId));
        this.m_MetricsQueryClients.TryAdd(key, metricsQueryClient);
      }
      return metricsQueryClient;
    }

    private TokenCredential CreateCredential(string tenantId)
    {
      if (this.m_useManagedIdentity)
        return (TokenCredential) new ManagedIdentityCredential(this.m_clientId, (TokenCredentialOptions) null);
      return this.m_servicePrincipalCertificate != null ? (TokenCredential) new ClientCertificateCredential(tenantId, this.m_clientId, this.m_servicePrincipalCertificate) : (TokenCredential) new LightRailCredential(tenantId);
    }

    private record struct Key(string TenantId, string Endpoint);

    private record struct ServicePrincipalKey(
      string ClientId,
      string ServicePrincipalCertificateThumbprint)
    ;
  }
}
