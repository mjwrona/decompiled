// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingExtensionEntitlementsBatchController
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [ControllerApiVersion(1.0)]
  public class LicensingExtensionEntitlementsBatchController : LicensingApiController
  {
    private readonly ServiceFactory<IExtensionEntitlementService> m_extensionEntitlementServiceFactory;

    public LicensingExtensionEntitlementsBatchController()
      : this((ServiceFactory<IExtensionEntitlementService>) (x => x.GetService<IExtensionEntitlementService>()))
    {
    }

    internal LicensingExtensionEntitlementsBatchController(
      ServiceFactory<IExtensionEntitlementService> extensionEntitlementServiceFactory)
    {
      this.m_extensionEntitlementServiceFactory = extensionEntitlementServiceFactory;
    }

    [HttpPut]
    [TraceFilterWithException(1040060, 1040068, 1040069)]
    [ClientLocationId("1D42DDC2-3E7D-4DAA-A0EB-E12C1DBD7C72")]
    [Obsolete]
    public IDictionary<Guid, IList<string>> GetExtensionsAssignedToUsers([FromBody] IList<Guid> userIds) => (IDictionary<Guid, IList<string>>) this.m_extensionEntitlementServiceFactory(this.TfsRequestContext).GetExtensionsAssignedToUsers(this.TfsRequestContext, userIds).ToDedupedDictionary<KeyValuePair<Guid, IList<ExtensionSource>>, Guid, IList<string>>((Func<KeyValuePair<Guid, IList<ExtensionSource>>, Guid>) (item => item.Key), (Func<KeyValuePair<Guid, IList<ExtensionSource>>, IList<string>>) (item => (IList<string>) item.Value.Select<ExtensionSource, string>((Func<ExtensionSource, string>) (val => val.ExtensionGalleryId)).ToList<string>()));
  }
}
