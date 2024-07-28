// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ICommerceAccountHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.UserMapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [InheritedExport]
  public interface ICommerceAccountHandler
  {
    void UpdateAccountTenant(IVssRequestContext requestContext, Guid tenantId, Guid accountId);

    bool CheckAccountNameAvailability(
      IVssRequestContext requestContext,
      string resourceName,
      out string invalidReason);

    IList<AzureRegion> GetSupportedAzureRegions(IVssRequestContext requestContext);

    string GetTenantName(IVssRequestContext requestContext);

    IList<Guid> QueryAccountIds(
      IVssRequestContext context,
      Guid userId,
      UserType userType,
      bool useEqualsCheckForUserTypeMatch = false,
      bool includeDeletedAccounts = false);
  }
}
