// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingEntitlementsController
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [ControllerApiVersion(1.0)]
  public class LicensingEntitlementsController : LicensingEntitlementsControllerBase
  {
    [HttpGet]
    [ActionName("Search")]
    [TraceFilterWithException(1039516, 1039517, 1039518)]
    [ClientLocationId("06A191B0-EC2F-4EE2-8C15-3AB15EAFE8E6")]
    public PagedAccountEntitlements SearchAccountEntitlements(
      string continuation = null,
      [FromUri(Name = "$filter")] string filter = null,
      [FromUri(Name = "$orderBy")] string orderBy = null)
    {
      return this.m_accountEntitlementServiceFactory(this.TfsRequestContext).SearchAccountEntitlements(this.TfsRequestContext, continuation, filter, orderBy);
    }
  }
}
