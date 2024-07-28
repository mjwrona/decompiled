// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.IDelegatedAuthorizationConfigurationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.Settings;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [DefaultServiceImplementation(typeof (DelegatedAuthorizationConfigurationService))]
  public interface IDelegatedAuthorizationConfigurationService : IVssFrameworkService
  {
    AuthorizationScopeConfiguration GetConfiguration(IVssRequestContext requestContext);

    DelegatedAuthorizationSettings GetSettings(IVssRequestContext requestContext);

    VssSigningCredentials GetSigningCredentials(
      IVssRequestContext requestContext,
      bool useOldSigningCredentials,
      bool force = false);

    IList<X509Certificate2> GetSigningPublicKeys(IVssRequestContext requestContext);
  }
}
