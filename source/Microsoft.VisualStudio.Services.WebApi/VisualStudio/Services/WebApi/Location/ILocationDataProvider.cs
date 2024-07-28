// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Location.ILocationDataProvider
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Location;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi.Location
{
  public interface ILocationDataProvider
  {
    Guid InstanceId { get; }

    Guid InstanceType { get; }

    AccessMapping ClientAccessMapping { get; }

    AccessMapping DefaultAccessMapping { get; }

    IEnumerable<AccessMapping> ConfiguredAccessMappings { get; }

    ServiceDefinition FindServiceDefinition(string serviceType, Guid serviceIdentifier);

    IEnumerable<ServiceDefinition> FindServiceDefinitions(string serviceType);

    string LocationForCurrentConnection(string serviceType, Guid serviceIdentifier);

    string LocationForCurrentConnection(ServiceDefinition serviceDefinition);

    string LocationForAccessMapping(
      string serviceType,
      Guid serviceIdentifier,
      AccessMapping accessMapping);

    string LocationForAccessMapping(
      ServiceDefinition serviceDefinition,
      AccessMapping accessMapping);

    AccessMapping GetAccessMapping(string moniker);

    ApiResourceLocationCollection GetResourceLocations();

    Task<Guid> GetInstanceIdAsync(CancellationToken cancellationToken = default (CancellationToken));

    Task<Guid> GetInstanceTypeAsync(CancellationToken cancellationToken = default (CancellationToken));

    Task<AccessMapping> GetClientAccessMappingAsync(CancellationToken cancellationToken = default (CancellationToken));

    Task<AccessMapping> GetDefaultAccessMappingAsync(CancellationToken cancellationToken = default (CancellationToken));

    Task<IEnumerable<AccessMapping>> GetConfiguredAccessMappingsAsync(
      CancellationToken cancellationToken = default (CancellationToken));

    Task<ServiceDefinition> FindServiceDefinitionAsync(
      string serviceType,
      Guid serviceIdentifier,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IEnumerable<ServiceDefinition>> FindServiceDefinitionsAsync(
      string serviceType,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<string> LocationForCurrentConnectionAsync(
      string serviceType,
      Guid serviceIdentifier,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<string> LocationForCurrentConnectionAsync(
      ServiceDefinition serviceDefinition,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<string> LocationForAccessMappingAsync(
      string serviceType,
      Guid serviceIdentifier,
      AccessMapping accessMapping,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<string> LocationForAccessMappingAsync(
      ServiceDefinition serviceDefinition,
      AccessMapping accessMapping,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AccessMapping> GetAccessMappingAsync(string moniker, CancellationToken cancellationToken = default (CancellationToken));

    Task<ApiResourceLocationCollection> GetResourceLocationsAsync(
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
