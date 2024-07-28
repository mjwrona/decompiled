// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AccountDetailsService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class AccountDetailsService : IAccountDetailsService, IVssFrameworkService
  {
    private const string TraceArea = "Commerce";
    private const string TraceLayer = "AccountDetailsService";

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public Guid GetTenantIdForOrganization(IVssRequestContext requestContext, string organizationId)
    {
      Guid result;
      if (!Guid.TryParse(organizationId, out result))
        throw new ArgumentException("Not a valid organization identifier");
      return requestContext.GetAuthenticatedIdentity() != null ? CommerceDeploymentHelper.GetOrganizationTenantId(requestContext.Elevate(), result) : throw new InvalidAccessException("Requested resource cannot be accessed without a valid identity");
    }
  }
}
