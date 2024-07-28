// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.AccountLicensingEntitlements2Controller
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [ControllerApiVersion(7.1)]
  [VersionedApiControllerCustomName(Area = "AccountLicensing", ResourceName = "Entitlements", ResourceVersion = 2)]
  public class AccountLicensingEntitlements2Controller : LicensingEntitlementsControllerBase
  {
    [HttpGet]
    [ActionName("Search")]
    [TraceFilterWithException(1039516, 1039517, 1039518)]
    [ClientLocationId("10909FD7-A10D-45F9-AD7F-A364A076A9C6")]
    public PagedAccountEntitlements SearchAccountEntitlements(
      string continuation = null,
      [FromUri(Name = "$filter")] string filter = null,
      [FromUri(Name = "$orderBy")] string orderBy = null)
    {
      return this.m_accountEntitlementServiceFactory(this.TfsRequestContext).SearchMemberAccountEntitlements(this.TfsRequestContext, continuation, filter, orderBy);
    }
  }
}
