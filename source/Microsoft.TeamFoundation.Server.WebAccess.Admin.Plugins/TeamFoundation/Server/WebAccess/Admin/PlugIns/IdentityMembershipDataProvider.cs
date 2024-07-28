// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.IdentityMembershipDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class IdentityMembershipDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.OrganizationAadIdentityMembership";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      requestContext.GetService<IClientLocationProviderService>().AddLocation(requestContext, providerContext.SharedData, ServiceInstanceTypes.SPS);
      IdentityMembershipData data = new IdentityMembershipData()
      {
        HasActiveOrInactiveMembership = false
      };
      if (providerContext.Properties.ContainsKey("originId") && providerContext.Properties["originId"] != null)
      {
        string input = providerContext.Properties["originId"].ToString();
        try
        {
          Guid result;
          if (Guid.TryParse(input, out result))
          {
            Guid organizationAadTenantId = requestContext.GetOrganizationAadTenantId();
            IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
            Microsoft.VisualStudio.Services.Identity.Identity identity = ReadIdentitiesByAadTenantIdOidExtension.ReadIdentityByTenantIdAndOid(context.GetService<IdentityService>(), context, organizationAadTenantId, result);
            if (identity != null)
              data.HasActiveOrInactiveMembership = this.CheckExistingMembershipsInCollection(identity, requestContext);
          }
        }
        catch (Exception ex)
        {
          data = (IdentityMembershipData) null;
          requestContext.TraceException(10050072, "ProjectPermissions", "DataProvider", ex);
        }
      }
      return (object) data;
    }

    private bool CheckExistingMembershipsInCollection(
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new List<Guid>()
      {
        identity.Id
      }, QueryMembership.None, (IEnumerable<string>) null);
      return (source != null ? source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>() : (Microsoft.VisualStudio.Services.Identity.Identity) null) != null;
    }
  }
}
