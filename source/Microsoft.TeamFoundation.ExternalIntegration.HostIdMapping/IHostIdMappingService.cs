// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.IHostIdMappingService
// Assembly: Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D6C9F672-10C9-4D5D-90D5-E07E56C1D4C0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping
{
  [DefaultServiceImplementation(typeof (HostIdMappingService))]
  public interface IHostIdMappingService : IVssFrameworkService
  {
    IEnumerable<Guid> GetHostIds(
      IVssRequestContext deploymentRequestContext,
      string providerId,
      HostIdMappingData mappingData);

    Guid? GetHostId(
      IVssRequestContext deploymentRequestContext,
      string providerId,
      HostIdMappingData mappingData,
      bool useExactMatching = false);

    Guid AddRoute(
      IVssRequestContext requestContext,
      IHostIdMappingProviderData providerData,
      string mappingProperty);

    void RemoveRoute(
      IVssRequestContext requestContext,
      IHostIdMappingProviderData providerData,
      string mappingProperty);

    bool TryAddHostIdMapping(
      IVssRequestContext requestContext,
      IHostIdMappingRouter mappingRouter,
      string providerId,
      string mappingProperty,
      string qualifier,
      out Guid conflictingHostId);

    bool TryRemoveHostIdMapping(
      IVssRequestContext requestContext,
      IHostIdMappingRouter mappingRouter,
      string providerId,
      HostIdMappingData mappingData,
      Guid expectedHostId);

    void AddHostIdMapping(
      IVssRequestContext requestContext,
      string providerId,
      HostIdMappingData mappingData,
      Guid hostId,
      bool overrideExisting = false);

    void RemoveHostIdMapping(
      IVssRequestContext requestContext,
      string providerId,
      HostIdMappingData mappingData);

    void RemoveHostIdMappings(
      IVssRequestContext requestContext,
      string providerId,
      HostIdMappingData mappingData);

    void RemoveHostIdMappingViaKey(
      IVssRequestContext requestContext,
      string providerId,
      string propertyName,
      string key);

    List<string> GetUnneededMappings(
      IVssRequestContext requestContext,
      string providerId,
      string propertyName,
      List<HostIdMappingData> resourceRepoMappingDataList);
  }
}
