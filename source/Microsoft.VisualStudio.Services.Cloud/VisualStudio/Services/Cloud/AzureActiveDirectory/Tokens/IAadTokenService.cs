// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens.IAadTokenService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens
{
  [DefaultServiceImplementation(typeof (FrameworkAadTokenService))]
  public interface IAadTokenService : IVssFrameworkService
  {
    string DefaultResource { get; }

    JwtSecurityToken AcquireToken(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      IdentityDescriptor IdentityDescriptor = null);

    JwtSecurityToken AcquireToken(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      SubjectDescriptor subjectDescriptor);

    JwtSecurityToken AcquireToken(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    JwtSecurityToken AcquireAppToken(
      IVssRequestContext requestContext,
      string resource,
      string tenantId);

    JwtSecurityToken AcquireTokenByAuthorizationCode(
      IVssRequestContext requestContext,
      string authCode,
      string resource,
      string tenantId,
      Uri replyToUri,
      Microsoft.VisualStudio.Services.Identity.Identity identityDescriptor);

    string TryUpdateRefreshTokenOnBehalfOfUser(
      IVssRequestContext requestContext,
      string accessToken,
      string rosurce,
      string tenantId,
      Microsoft.VisualStudio.Services.Identity.Identity identity);
  }
}
