// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.IOAuth2SettingsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  [DefaultServiceImplementation(typeof (OAuth2SettingsService))]
  public interface IOAuth2SettingsService : IVssFrameworkService
  {
    long GetVersion(IVssRequestContext requestContext);

    bool AreSigningKeysExpired(IVssRequestContext requestContext);

    IOAuth2CommonSettings GetCommonSettings(IVssRequestContext requestContext);

    IDelegatedAuthSettings GetDelegatedAuthSettings(IVssRequestContext requestContext);

    IUserAuthSettings GetUserAuthSettings(IVssRequestContext requestContext);

    IAADAuthSettings GetAADAuthSettings(IVssRequestContext requestContext);

    IS2SAuthSettings GetS2SAuthSettings(IVssRequestContext requestContext);

    IJWTSigningSettings GetJWTSigningSettings(IVssRequestContext requestContext);

    IDictionary<string, X509Certificate2> GetRegistryBasedValidationCertificates(
      IVssRequestContext requestContext);

    IDictionary<string, X509Certificate2> GetValidationCertificates(
      IVssRequestContext requestContext);

    bool UpdateUserAuthCertificatesIfExpired(IVssRequestContext requestContext);
  }
}
