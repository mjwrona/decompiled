// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenAdmin.ITokenAdminManagementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.TokenAdmin.Client;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.TokenAdmin
{
  [DefaultServiceImplementation(typeof (FrameworkTokenAdminManagementService))]
  public interface ITokenAdminManagementService : IVssFrameworkService
  {
    TokenAdminPagedSessionTokens ListPersonalAccessTokensByUser(
      IVssRequestContext requestContext,
      SubjectDescriptor userSubjectDescriptor,
      int? pageSize,
      Guid? continuationToken,
      bool isPublic = false);

    void RevokeAuthorizations(
      IVssRequestContext requestContext,
      IEnumerable<Guid> authorizationIds,
      bool isPublic = false);

    SessionTokenResult GetPersonalAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false);

    void RevokePersonalAccessToken(
      IVssRequestContext requestContext,
      string accessTokenKey,
      bool isPublic = false);
  }
}
