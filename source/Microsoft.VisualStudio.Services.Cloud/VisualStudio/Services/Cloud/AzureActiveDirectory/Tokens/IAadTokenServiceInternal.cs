// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens.IAadTokenServiceInternal
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens
{
  [DefaultServiceImplementation(typeof (FrameworkAadTokenService))]
  internal interface IAadTokenServiceInternal : IAadTokenService, IVssFrameworkService
  {
    JwtSecurityToken AcquireTokenFromCache(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      IdentityDescriptor IdentityDescriptor = null);

    JwtSecurityToken AcquireTokenFromCache(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      SubjectDescriptor subjectDescriptor);

    JwtSecurityToken AcquireTokenFromCache(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      Microsoft.VisualStudio.Services.Identity.Identity identity);

    Task<string> AcquireTokenAsync(
      IVssRequestContext requestContext,
      string resource,
      string tenantId,
      IdentityDescriptor identityDescriptor);
  }
}
