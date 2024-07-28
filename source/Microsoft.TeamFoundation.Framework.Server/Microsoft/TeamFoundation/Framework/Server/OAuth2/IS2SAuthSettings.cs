// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.IS2SAuthSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public interface IS2SAuthSettings
  {
    bool Enabled { get; }

    string TenantDomain { get; }

    string FirstPartyTenantDomain { get; }

    string TenantId { get; }

    string FirstPartyTenantId { get; }

    Uri IssuanceEndpoint { get; }

    string Issuer { get; }

    string FirstPartyIssuer { get; }

    Guid PrimaryServicePrincipal { get; }

    X509Certificate2 GetSigningCertificate(IVssRequestContext requestContext);

    bool DisableAADTestSlice { get; }

    IEnumerable<string> FirstPartyServicePrincipals { get; }
  }
}
