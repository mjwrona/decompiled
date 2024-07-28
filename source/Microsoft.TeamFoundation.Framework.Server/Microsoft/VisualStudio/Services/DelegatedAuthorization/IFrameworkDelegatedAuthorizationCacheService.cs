// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.IFrameworkDelegatedAuthorizationCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [DefaultServiceImplementation(typeof (FrameworkDelegatedAuthorizationCacheService))]
  internal interface IFrameworkDelegatedAuthorizationCacheService : IVssFrameworkService
  {
    bool TryGetAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      out AccessToken accessTokenResult);

    void SetAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      AccessToken accessTokenResult);

    void InvalidateAccessToken(IVssRequestContext requestContext, string accessTokenKey);

    void InvalidateAccessTokens(
      IVssRequestContext requestContext,
      IEnumerable<string> accessTokenKeys);

    void AddNoAccessTokenEntryToLocalCache(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false);

    void Clear(IVssRequestContext requestContext);

    AccessToken NoAccessTokenEntry { get; }
  }
}
