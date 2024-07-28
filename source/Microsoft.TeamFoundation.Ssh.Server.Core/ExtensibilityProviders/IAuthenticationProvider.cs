// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.Core.ExtensibilityProviders.IAuthenticationProvider
// Assembly: Microsoft.TeamFoundation.Ssh.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3DF8FBEE-AA1B-4659-8650-E7C7E1E085EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Ssh.Server.Core.ExtensibilityProviders
{
  public interface IAuthenticationProvider
  {
    bool IsForwardedAuthSupported(IVssRequestContext requestContext);

    bool IsPublicKeySupported(IVssRequestContext requestContext);

    Microsoft.VisualStudio.Services.Identity.Identity AuthenticatePublicKey(
      IVssRequestContext requestContext,
      string username,
      ICryptographicKeyPair publicKey);

    (Microsoft.VisualStudio.Services.Identity.Identity identity, Guid? e2eId, string clientIp) AuthenticateForwardedCredentials(
      IVssRequestContext requestContext,
      string username,
      string forwardedToken);

    string GenerateForwardCredentials(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string targetAudience);
  }
}
