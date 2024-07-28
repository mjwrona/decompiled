// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingEntitlementsBatchController
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing.Http;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [ControllerApiVersion(1.0)]
  public class LicensingEntitlementsBatchController : LicensingApiController
  {
    private readonly ServiceFactory<ILicensingEntitlementService> m_licensingEntitlementServiceFactory;

    public LicensingEntitlementsBatchController()
      : this((ServiceFactory<ILicensingEntitlementService>) (x => x.GetService<ILicensingEntitlementService>()))
    {
    }

    internal LicensingEntitlementsBatchController(
      ServiceFactory<ILicensingEntitlementService> licensingEntitlementServiceFactory)
    {
      this.m_licensingEntitlementServiceFactory = licensingEntitlementServiceFactory;
    }

    [HttpPost]
    [TraceFilterWithException(1039401, 1039403, 1039402)]
    [ClientLocationId("CC3A0130-78AD-4A00-B1CA-49BEF42F4656")]
    [ActionName("GetUsersEntitlements")]
    public IList<AccountEntitlement> GetAccountEntitlementsBatch([FromBody] IList<Guid> userIds) => this.m_licensingEntitlementServiceFactory(this.TfsRequestContext).GetAccountEntitlements(this.TfsRequestContext, userIds);

    [HttpPost]
    [TraceFilterWithException(1039404, 1039406, 1039405)]
    [ClientLocationId("CC3A0130-78AD-4A00-B1CA-49BEF42F4656")]
    [ActionName("GetAvailableUsersEntitlements")]
    public IList<AccountEntitlement> ObtainAvailableAccountEntitlements([FromBody] IList<Guid> userIds) => this.m_licensingEntitlementServiceFactory(this.TfsRequestContext).ObtainAvailableAccountEntitlements(this.TfsRequestContext, userIds);
  }
}
