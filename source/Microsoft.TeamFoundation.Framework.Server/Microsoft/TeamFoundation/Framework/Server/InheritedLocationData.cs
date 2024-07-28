// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.InheritedLocationData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class InheritedLocationData
  {
    public static void Install(IVssRequestContext requestContext, ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      requestContext.CheckDeploymentRequestContext();
      WebApiConfiguration.Initialize(requestContext);
      ExtendedLocationDataService service1 = requestContext.GetService<ExtendedLocationDataService>();
      ILocationService service2 = requestContext.GetService<ILocationService>();
      Dictionary<Guid, ServiceDefinition> dedupedDictionary = requestContext.GetService<InheritedLocationDataService>().GetInheritedDefinitionsFromStore(requestContext, Guid.Empty).GetAllServiceDefinitions().Where<ServiceDefinition>((Func<ServiceDefinition, bool>) (y => y.IsResourceLocation())).ToDedupedDictionary<ServiceDefinition, Guid, ServiceDefinition>((Func<ServiceDefinition, Guid>) (x => x.Identifier), (Func<ServiceDefinition, ServiceDefinition>) (x => x));
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<ServiceDefinition> definitions = service1.GetDefinitions(requestContext1);
      foreach (ServiceDefinition serviceDefinition1 in definitions)
      {
        ServiceDefinition serviceDefinition2;
        if (dedupedDictionary.TryGetValue(serviceDefinition1.Identifier, out serviceDefinition2) && !serviceDefinition2.ServiceType.Equals(serviceDefinition1.ServiceType, StringComparison.OrdinalIgnoreCase))
          throw new InvalidOperationException(string.Format("Cannot save resource location with identifier {0} and area {1} since there exists a resource location \r\n                                                           with area {2}. Changing the resource area is a compat break since the client may depend on it. You should add\r\n                                                           another identifier and remove the existing service definition if needed using a servicing step (LocationStepPerformer.RemoveLocationData).", (object) serviceDefinition1.Identifier, (object) serviceDefinition1.ServiceType, (object) serviceDefinition2.ServiceType));
      }
      service2.SaveServiceDefinitions(requestContext, definitions);
    }

    public static void Validate(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      ILocationService service = requestContext.GetService<ILocationService>();
      string selfReferenceUrl = service.GetSelfReferenceUrl(requestContext, service.GetPublicAccessMapping(requestContext));
      ICreateClient clientProvider = requestContext.ClientProvider as ICreateClient;
      requestContext.RootContext.Items[RequestContextItemsKeys.BypassLoopbackHandler] = (object) true;
      IVssRequestContext requestContext1 = requestContext;
      Uri baseUri = new Uri(selfReferenceUrl);
      Guid targetServicePrincipal = new Guid();
      InheritedLocationData.OptionsHttpClient client = clientProvider.CreateClient<InheritedLocationData.OptionsHttpClient>(requestContext1, baseUri, "LocationService", (ApiResourceLocationCollection) null, targetServicePrincipal: targetServicePrincipal);
      List<ApiResourceLocation> resourceLocationList = new List<ApiResourceLocation>();
      HashSet<ServiceDefinition> serviceDefinitionSet = new HashSet<ServiceDefinition>(requestContext.GetService<InheritedLocationDataService>().GetData(requestContext, Guid.Empty, TeamFoundationHostType.All).GetAllServiceDefinitions(), (IEqualityComparer<ServiceDefinition>) ServiceDefinitionComparer.Instance);
      CancellationToken cancellationToken = new CancellationToken();
      foreach (ApiResourceLocation resourceLocation in client.GetResourceLocationsAsync(true, cancellationToken: cancellationToken).SyncResult<IEnumerable<ApiResourceLocation>>())
      {
        ServiceDefinition serviceDefinition = resourceLocation.ToServiceDefinition();
        if (!serviceDefinitionSet.Contains(serviceDefinition))
          resourceLocationList.Add(resourceLocation);
      }
      if (resourceLocationList.Count > 0)
      {
        StringBuilder stringBuilder = new StringBuilder();
        string str = requestContext.ExecutionEnvironment.IsHostedDeployment ? "C:\\LR\\<service>\\Services\\<service>\\Plugins" : "C:\\Program Files\\Azure DevOps Server 2022\\Tools\\Plugins";
        stringBuilder.AppendLine("The following ApiResourceLocations were missing from the location service. Please ensure the extension dll that registers them is in the services plugin directory (e.g. " + str + ". This will allow them to be registered with the location service.");
        foreach (ApiResourceLocation resourceLocation in resourceLocationList)
          stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Area: {0}, Id: {1}", (object) resourceLocation.Area, (object) resourceLocation.Id));
        throw new InheritedLocationDataValidatorException(stringBuilder.ToString());
      }
    }

    private class OptionsHttpClient : VssHttpClientBase
    {
      public OptionsHttpClient(Uri baseUrl, VssCredentials credentials)
        : base(baseUrl, credentials)
      {
      }

      public OptionsHttpClient(
        Uri baseUrl,
        VssCredentials credentials,
        VssHttpRequestSettings settings)
        : base(baseUrl, credentials, settings)
      {
      }

      public OptionsHttpClient(
        Uri baseUrl,
        VssCredentials credentials,
        params DelegatingHandler[] handlers)
        : base(baseUrl, credentials, handlers)
      {
      }

      public OptionsHttpClient(
        Uri baseUrl,
        VssCredentials credentials,
        VssHttpRequestSettings settings,
        params DelegatingHandler[] handlers)
        : base(baseUrl, credentials, settings, handlers)
      {
      }

      public OptionsHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
        : base(baseUrl, pipeline, disposeHandler)
      {
      }
    }
  }
}
