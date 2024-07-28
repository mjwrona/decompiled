// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.DataProviders.AdminCollectionPolicyDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Helpers;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.DataProviders
{
  public class AdminCollectionPolicyDataProvider : AdminPolicyDataProvider
  {
    public override string Name => "Admin.Organization.Administration.CollectionPolicy";

    public override object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      OrganizationAdministrationDataProviderHelper.ValidateContext(requestContext);
      return base.GetData(requestContext, providerContext, contribution);
    }
  }
}
