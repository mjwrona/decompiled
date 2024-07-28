// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ClientIdentityDetailsProviderService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.IdentityImage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ClientIdentityDetailsProviderService : 
    IClientIdentityDetailsProviderService,
    IVssFrameworkService
  {
    private const string c_imageIdsSharedDataKey = "_identityDetails";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void AddIdentity(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      Guid identityId)
    {
      Dictionary<Guid, IdentityDetails> detailsCache = this.LoadIdentityCache(requestContext, sharedData);
      if (detailsCache.ContainsKey(identityId))
        return;
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        identityId
      }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.CacheIdentity(requestContext, sharedData, identity, detailsCache);
    }

    public void AddIdentity(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      Dictionary<Guid, IdentityDetails> detailsCache = this.LoadIdentityCache(requestContext, sharedData);
      if (detailsCache.ContainsKey(identity.Id))
        return;
      this.CacheIdentity(requestContext, sharedData, identity, detailsCache);
    }

    private void CacheIdentity(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      Dictionary<Guid, IdentityDetails> detailsCache)
    {
      if (identity == null)
        return;
      Guid? cachedIdentityImageId = IdentityImageServiceUtil.GetIdentityImageService(requestContext).GetCachedIdentityImageId(requestContext, identity.Id);
      IdentityDetails identityDetails = new IdentityDetails()
      {
        IdentityImageId = cachedIdentityImageId ?? Guid.Empty,
        DisplayName = identity.DisplayName
      };
      detailsCache[identity.Id] = identityDetails;
    }

    private Dictionary<Guid, IdentityDetails> LoadIdentityCache(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData)
    {
      object obj;
      Dictionary<Guid, IdentityDetails> dictionary;
      if (!sharedData.TryGetValue("_identityDetails", out obj) || !(obj is WebSdkMetadataDictionary<Guid, IdentityDetails>))
      {
        dictionary = (Dictionary<Guid, IdentityDetails>) new WebSdkMetadataDictionary<Guid, IdentityDetails>();
        sharedData.Add("_identityDetails", (object) dictionary);
      }
      else
        dictionary = (Dictionary<Guid, IdentityDetails>) obj;
      return dictionary;
    }
  }
}
