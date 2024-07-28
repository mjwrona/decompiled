// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProfileOrganizationsAadMembershipDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class ProfileOrganizationsAadMembershipDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.ProfileOrganizationsAadMembership";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      List<IdentityDescriptor> list = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        requestContext.UserContext
      }, QueryMembership.ExpandedUp, (IEnumerable<string>) null).First<Microsoft.VisualStudio.Services.Identity.Identity>().MemberOf.Where<IdentityDescriptor>(ProfileOrganizationsAadMembershipDataProvider.\u003C\u003EO.\u003C0\u003E__IsAadGroup ?? (ProfileOrganizationsAadMembershipDataProvider.\u003C\u003EO.\u003C0\u003E__IsAadGroup = new Func<IdentityDescriptor, bool>(AadIdentityHelper.IsAadGroup))).ToList<IdentityDescriptor>();
      IEnumerable<string> values = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) list.Take<IdentityDescriptor>(5).ToList<IdentityDescriptor>(), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (x => !x.DisplayName.StartsWith("[TEAM FOUNDATION]\\") ? x.DisplayName : x.DisplayName.Substring("[TEAM FOUNDATION]\\".Length)));
      return (object) new AadMembership()
      {
        Names = string.Join(", ", values),
        Total = list.Count
      };
    }
  }
}
