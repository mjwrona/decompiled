// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Utils.DirectoryDiscoveryServiceHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DiscoveryService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Utils
{
  internal sealed class DirectoryDiscoveryServiceHelper
  {
    internal const string c_DefaultUserImageKey = "user";
    internal const string c_DefaultVsoGroupImageKey = "vsogroup";
    internal const string c_DefaultGroupImageKey = "aadgroup";
    internal const string c_DefaultDdsUserImage = "ip-content-user-default.png";
    internal const string c_DefaultVsoGroupImage = "ip-content-vso-group-default.png";
    internal const string c_DefaultAadGroupImage = "ip-content-aad-group-default.png";

    [TfsTraceFilter(505231, 505240)]
    internal static IDirectoryEntity GetEntityFromTFIdentifier(
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

    [TfsTraceFilter(505241, 505250)]
    internal static byte[] GetEntityAvatarBytes(IVssRequestContext requestContext, string entityId)
    {
      DirectoryDiscoveryService service = requestContext.GetService<DirectoryDiscoveryService>();
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Application) && !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return (byte[]) null;
      DirectoryDiscoveryService discoveryService = service;
      IVssRequestContext context = requestContext;
      DirectoryGetAvatarsRequest getAvatarsRequest = new DirectoryGetAvatarsRequest();
      getAvatarsRequest.Directories = (IEnumerable<string>) new string[2]
      {
        "src",
        "vsd"
      };
      getAvatarsRequest.ObjectIds = (IEnumerable<string>) new string[1]
      {
        entityId
      };
      DirectoryGetAvatarsRequest request = getAvatarsRequest;
      DirectoryGetAvatarsResponse avatars = discoveryService.GetAvatars(context, request);
      return avatars == null || avatars.Results == null || avatars.Results.Count != 1 || !avatars.Results.Keys.Contains(entityId) ? (byte[]) null : avatars.Results[entityId];
    }

    [TfsTraceFilter(505271, 505280)]
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
  }
}
