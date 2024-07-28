// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Location.ILocationService
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi.Location
{
  [VssClientServiceImplementation(typeof (LocationService))]
  public interface ILocationService : IVssClientService
  {
    ILocationDataProvider GetLocationData(Guid locationAreaIdentifier);

    string GetLocationServiceUrl(Guid locationAreaIdentifier);

    string GetLocationServiceUrl(Guid locationAreaIdentifier, string accessMappingMoniker);

    Task<ILocationDataProvider> GetLocationDataAsync(
      Guid locationAreaIdentifier,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<string> GetLocationServiceUrlAsync(
      Guid locationAreaIdentifier,
      string accessMappingMoniker = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
