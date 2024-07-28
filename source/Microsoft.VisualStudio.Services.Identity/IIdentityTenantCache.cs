// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IIdentityTenantCache
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.Identity
{
  [InheritedExport]
  internal interface IIdentityTenantCache
  {
    bool TryGetTenants(
      IVssRequestContext requestContext,
      Guid identityId,
      out List<TenantInfo> tenantsResponse);

    void SetTenants(IVssRequestContext requestContext, Guid identityId, List<TenantInfo> tenants);

    void InvalidateGetTenants(IVssRequestContext requestContext, Guid identityId);
  }
}
