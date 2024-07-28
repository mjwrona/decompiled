// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.UserTenantsDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class UserTenantsDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.OrganizationAadUserTenants";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      UserTenantData userTenantData = UserTenantsDataProvider.FetchUserTenantData(requestContext);
      return (object) new UserTenantsData()
      {
        UserTenantData = userTenantData
      };
    }

    private static UserTenantData FetchUserTenantData(IVssRequestContext requestContext)
    {
      List<AadTenant> list = OrganizationAdminDataProviderHelper.GetTenants(requestContext).ToList<AadTenant>();
      return new UserTenantData()
      {
        UserContext = requestContext.UserContext != (IdentityDescriptor) null ? requestContext.UserContext.ToString() : string.Empty,
        UserTenants = list.Select<AadTenant, TenantData>((Func<AadTenant, TenantData>) (ut => new TenantData()
        {
          Id = ut.ObjectId,
          DisplayName = ut.DisplayName
        })).ToList<TenantData>()
      };
    }
  }
}
