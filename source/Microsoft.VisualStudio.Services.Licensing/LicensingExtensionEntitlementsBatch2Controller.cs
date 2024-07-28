// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingExtensionEntitlementsBatch2Controller
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
  public class LicensingExtensionEntitlementsBatch2Controller : LicensingApiController
  {
    private readonly ServiceFactory<IExtensionEntitlementService> m_extensionEntitlementServiceFactory;

    public LicensingExtensionEntitlementsBatch2Controller()
      : this((ServiceFactory<IExtensionEntitlementService>) (x => x.GetService<IExtensionEntitlementService>()))
    {
    }

    internal LicensingExtensionEntitlementsBatch2Controller(
      ServiceFactory<IExtensionEntitlementService> extensionEntitlementServiceFactory)
    {
      this.m_extensionEntitlementServiceFactory = extensionEntitlementServiceFactory;
    }

    [HttpPut]
    [TraceFilterWithException(1040060, 1040068, 1040069)]
    [ClientLocationId("1D42DDC2-3E7D-4DAA-A0EB-E12C1DBD7C72")]
    public IDictionary<Guid, IList<ExtensionSource>> BulkGetExtensionsAssignedToUsers(
      [FromBody] IList<Guid> userIds)
    {
      return this.m_extensionEntitlementServiceFactory(this.TfsRequestContext).GetExtensionsAssignedToUsers(this.TfsRequestContext, userIds);
    }
  }
}
