// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Helpers.DirectoryDiscoveryServiceHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DiscoveryService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Helpers
{
  public class DirectoryDiscoveryServiceHelper
  {
    public static IDirectoryEntity GetEntityFromTFIdentifier(
      IVssRequestContext requestContext,
      string tfId)
    {
      if (string.IsNullOrWhiteSpace(tfId))
        return (IDirectoryEntity) null;
      string entityId;
      if (!DirectoryDiscoveryServiceHelper.TryParseTFIdentifier(requestContext, tfId, out entityId) || string.IsNullOrWhiteSpace(entityId))
        return (IDirectoryEntity) null;
      DirectoryDiscoveryService service = requestContext.GetService<DirectoryDiscoveryService>();
      IVssRequestContext context = requestContext;
      DirectoryGetEntitiesRequest getEntitiesRequest = new DirectoryGetEntitiesRequest();
      getEntitiesRequest.Directories = (IEnumerable<string>) new string[2]
      {
        "src",
        "vsd"
      };
      getEntitiesRequest.EntityIds = (IEnumerable<string>) new string[1]
      {
        entityId
      };
      getEntitiesRequest.PropertiesToReturn = (IEnumerable<string>) new string[1]
      {
        "DisplayName"
      };
      DirectoryGetEntitiesRequest request = getEntitiesRequest;
      DirectoryGetEntitiesResponse entities = service.GetEntities(context, request);
      DirectoryGetEntityResult directoryGetEntityResult;
      return entities == null || entities.Results == null || entities.Results.Count != 1 || !entities.Results.TryGetValue(entityId, out directoryGetEntityResult) || directoryGetEntityResult.Exception != null ? (IDirectoryEntity) null : directoryGetEntityResult.Entity;
    }

    private static bool TryParseTFIdentifier(
      IVssRequestContext requestContext,
      string tfId,
      out string entityId)
    {
      entityId = (string) null;
      if (string.IsNullOrWhiteSpace(tfId))
        return false;
      DirectoryDiscoveryService service = requestContext.GetService<DirectoryDiscoveryService>();
      string str = tfId.ToLowerInvariant().Trim();
      if (string.IsNullOrWhiteSpace(str))
        return false;
      Guid result = Guid.Empty;
      if (!Guid.TryParse(str, out result))
      {
        if (!DirectoryUtils.IsEntityId(str))
          return false;
        entityId = str;
        return true;
      }
      DirectoryDiscoveryService discoveryService = service;
      IVssRequestContext context = requestContext;
      DirectoryConvertKeysRequest convertKeysRequest = new DirectoryConvertKeysRequest();
      convertKeysRequest.Directories = (IEnumerable<string>) new string[2]
      {
        "src",
        "vsd"
      };
      convertKeysRequest.ConvertFrom = "VisualStudioIdentifier";
      convertKeysRequest.ConvertTo = "DirectoryEntityIdentifier";
      convertKeysRequest.Keys = (IEnumerable<string>) new string[1]
      {
        str
      };
      DirectoryConvertKeysRequest request = convertKeysRequest;
      DirectoryConvertKeysResponse convertKeysResponse = discoveryService.ConvertKeys(context, request);
      if (convertKeysResponse.Results == null || convertKeysResponse.Results.Count != 1 || convertKeysResponse.Results.FirstOrDefault<KeyValuePair<string, DirectoryConvertKeyResult>>().Value.Exception != null)
        return false;
      entityId = convertKeysResponse.Results.FirstOrDefault<KeyValuePair<string, DirectoryConvertKeyResult>>().Value.Key;
      return true;
    }

    public static IdentityViewData BuildFilteredIdentitiesViewModel(
      IEnumerable<IDirectoryEntity> filteredIdentities,
      bool hasMoreItems,
      int totalItems,
      Guid collectionHostId)
    {
      IEnumerable<GraphViewModel> graphViewModels = Enumerable.Empty<GraphViewModel>();
      if (filteredIdentities != null)
        graphViewModels = (IEnumerable<GraphViewModel>) filteredIdentities.Select<IDirectoryEntity, GraphViewModel>((Func<IDirectoryEntity, GraphViewModel>) (s => new GraphViewModel(s, collectionHostId))).ToList<GraphViewModel>();
      return new IdentityViewData()
      {
        Identities = graphViewModels,
        HasMore = hasMoreItems,
        TotalIdentityCount = totalItems,
        CollectionHostId = collectionHostId
      };
    }
  }
}
