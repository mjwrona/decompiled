// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingAccountAssignedExtensionsController
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
  public class LicensingAccountAssignedExtensionsController : LicensingApiController
  {
    private const string s_layer = "LicensingAccountAssignedExtensionsController";

    [HttpGet]
    [TraceFilterWithException(1040050, 1040058, 1040059)]
    [ClientLocationId("01BCE8D3-C130-480F-A332-474AE3F6662E")]
    public IEnumerable<AccountLicenseExtensionUsage> GetExtensionLicenseUsage()
    {
      try
      {
        return this.TfsRequestContext.GetService<IExtensionEntitlementService>().GetExtensionLicenseUsage(this.TfsRequestContext) ?? (IEnumerable<AccountLicenseExtensionUsage>) new List<AccountLicenseExtensionUsage>();
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(1040059, "VisualStudio.Services.LicensingService", nameof (LicensingAccountAssignedExtensionsController), ex);
        throw;
      }
    }
  }
}
