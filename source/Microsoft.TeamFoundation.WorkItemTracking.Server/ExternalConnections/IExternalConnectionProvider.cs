// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.IExternalConnectionProvider
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  [InheritedExport]
  public interface IExternalConnectionProvider
  {
    IEnumerable<string> ServiceEndpointTypesSupported { get; }

    ExternalConnectionProvisionResult ProvisionIntegration(
      IVssRequestContext requestContext,
      Guid projectId,
      ServiceEndpoint serviceEndpoint,
      IEnumerable<string> repoIdsToAdd,
      IEnumerable<string> repoIdsToRemove);

    void FinalizeProvision(
      IVssRequestContext requestContext,
      Guid projectId,
      ExternalConnectionProvisionResult provisionResult);

    void PopulateMetadata(
      IVssRequestContext requestContext,
      Guid projectId,
      ExternalConnection connection);

    string GetProviderKey(ServiceEndpoint serviceEndpoint);

    string ExternalProviderType { get; }

    bool IsBuiltInProvider { get; }

    bool ShouldMatchConnectionCreatorForAddOrUpdateRepo(ServiceEndpoint endpoint);

    bool CanCreateNewConnection(
      ServiceEndpoint serviceEndpoint,
      IEnumerable<ExternalConnection> existingConnectionsFromSameProvider);

    string GetConnectionNameForCreation(ServiceEndpoint serviceEndpoint);

    Dictionary<string, string> GetRepoIdsByRepoNames(
      IVssRequestContext requestContext,
      Guid projectId,
      ServiceEndpoint serviceEndpoint,
      IList<string> repoNames);
  }
}
