// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ILocationService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public interface ILocationService
  {
    AccessMapping ClientAccessMapping { get; }

    AccessMapping DefaultAccessMapping { get; }

    IEnumerable<AccessMapping> ConfiguredAccessMappings { get; }

    void SaveServiceDefinition(ServiceDefinition serviceDefinition);

    void SaveServiceDefinitions(IEnumerable<ServiceDefinition> serviceDefinitions);

    void RemoveServiceDefinition(string serviceType, Guid serviceIdentifier);

    void RemoveServiceDefinition(ServiceDefinition serviceDefinition);

    void RemoveServiceDefinitions(IEnumerable<ServiceDefinition> serviceDefinitions);

    ServiceDefinition FindServiceDefinition(string serviceType, Guid serviceIdentifier);

    IEnumerable<ServiceDefinition> FindServiceDefinitions(string serviceType);

    IEnumerable<ServiceDefinition> FindServiceDefinitionsByToolType(string toolType);

    string LocationForCurrentConnection(string serviceType, Guid serviceIdentifier);

    string LocationForCurrentConnection(ServiceDefinition serviceDefinition);

    string LocationForAccessMapping(
      string serviceType,
      Guid serviceIdentifier,
      AccessMapping accessMapping);

    string LocationForAccessMapping(
      ServiceDefinition serviceDefinition,
      AccessMapping accessMapping);

    AccessMapping ConfigureAccessMapping(
      string moniker,
      string displayName,
      string accessPoint,
      bool makeDefault);

    void SetDefaultAccessMapping(AccessMapping accessMapping);

    AccessMapping GetAccessMapping(string moniker);

    void RemoveAccessMapping(string moniker);
  }
}
