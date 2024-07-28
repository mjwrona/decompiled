// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.WebApi.MultiScaleUnitDistributor`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.WebApi
{
  internal class MultiScaleUnitDistributor<TClient> where TClient : class, IVssHttpClient
  {
    private readonly Uri baseAddress;
    private readonly VssCredentials credentials;
    private readonly Guid serviceInstanceType;

    public MultiScaleUnitDistributor(
      Uri baseAddress,
      VssCredentials credentials,
      Guid serviceInstanceType)
    {
      this.baseAddress = baseAddress;
      this.credentials = credentials;
      this.serviceInstanceType = serviceInstanceType;
    }

    public MultiScaleUnitDistributor(
      Uri baseAddress,
      HttpMessageHandler pipeline,
      Guid serviceInstanceType)
      : this(baseAddress, MultiScaleUnitDistributor<TClient>.ExtractCredentialsFromPipeline(pipeline), serviceInstanceType)
    {
    }

    protected internal virtual async Task ExecuteDistributedAsync(
      Func<TClient, object, CancellationToken, Task> requestAsync,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IEnumerable<LocationMapping> source = await this.GetServiceLocationMappingsAsync(cancellationToken).ConfigureAwait(false);
      if (!source.Any<LocationMapping>())
        throw new InvalidOperationException(string.Format("No location mappings were found for service {0}", (object) this.serviceInstanceType));
      foreach (LocationMapping locationMapping in source)
      {
        Type type = typeof (TClient);
        object[] objArray = new object[2]
        {
          (object) new Uri(locationMapping.Location),
          (object) this.credentials
        };
        await requestAsync(Activator.CreateInstance(type, objArray) as TClient, userState, cancellationToken).ConfigureAwait(false);
      }
    }

    protected internal virtual async Task<IEnumerable<LocationMapping>> GetServiceLocationMappingsAsync(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MultiScaleUnitDistributor<TClient> scaleUnitDistributor = this;
      // ISSUE: reference to a compiler-generated method
      return (IEnumerable<LocationMapping>) (await (await scaleUnitDistributor.GetRootLocationClientAsync(scaleUnitDistributor.baseAddress, scaleUnitDistributor.credentials, cancellationToken).ConfigureAwait(false)).GetServiceDefinitionsAsync("LocationService2").ConfigureAwait(false)).Where<ServiceDefinition>(new Func<ServiceDefinition, bool>(scaleUnitDistributor.\u003CGetServiceLocationMappingsAsync\u003Eb__3_0)).Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (s => s.LocationMappings.Any<LocationMapping>((Func<LocationMapping, bool>) (m => m.AccessMappingMoniker == AccessMappingConstants.PublicAccessMappingMoniker)))).Select<ServiceDefinition, LocationMapping>((Func<ServiceDefinition, LocationMapping>) (s => s.LocationMappings.First<LocationMapping>((Func<LocationMapping, bool>) (m => m.AccessMappingMoniker == AccessMappingConstants.PublicAccessMappingMoniker)))).GroupBy<LocationMapping, string>((Func<LocationMapping, string>) (m => m.Location)).Select<IGrouping<string, LocationMapping>, LocationMapping>((Func<IGrouping<string, LocationMapping>, LocationMapping>) (g => g.First<LocationMapping>())).ToList<LocationMapping>();
    }

    protected internal virtual async Task<LocationHttpClient> GetRootLocationClientAsync(
      Uri baseUri,
      VssCredentials vssCredentials,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LocationHttpClient httpClient = new LocationHttpClient(baseUri, vssCredentials);
      ServiceDefinition serviceDefinition = await httpClient.GetServiceDefinitionAsync("LocationService2", LocationServiceConstants.RootIdentifier, cancellationToken).ConfigureAwait(false);
      if (serviceDefinition == null)
        return httpClient;
      return new LocationHttpClient(new Uri((serviceDefinition.LocationMappings.FirstOrDefault<LocationMapping>((Func<LocationMapping, bool>) (m => m.AccessMappingMoniker == AccessMappingConstants.PublicAccessMappingMoniker)) ?? throw new InvalidOperationException("No public access mapping defined for root location service.")).Location), vssCredentials);
    }

    private static VssCredentials ExtractCredentialsFromPipeline(HttpMessageHandler pipeline)
    {
      DelegatingHandler delegatingHandler1 = pipeline as DelegatingHandler;
      DelegatingHandler delegatingHandler2 = (DelegatingHandler) null;
      for (; delegatingHandler1 != null; delegatingHandler1 = delegatingHandler1.InnerHandler as DelegatingHandler)
        delegatingHandler2 = delegatingHandler1;
      if (!(delegatingHandler2?.InnerHandler is VssHttpMessageHandler innerHandler))
        throw new InvalidOperationException("Unable to locate credentials in message handler pipeline.");
      return innerHandler.Credentials;
    }
  }
}
