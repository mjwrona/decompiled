// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionDataCollectionQueryController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  [VersionedApiControllerCustomName(Area = "ExtensionManagement", ResourceName = "ExtensionDataCollectionQuery")]
  [ClientInternalUseOnly(false)]
  public class ExtensionDataCollectionQueryController : TfsApiController
  {
    [HttpPost]
    [ClientLocationId("56C331F1-CE53-4318-ADFD-4DB5C52A7A2E")]
    public List<ExtensionDataCollection> QueryCollectionsByName(
      string publisherName,
      string extensionName,
      ExtensionDataCollectionQuery collectionQuery)
    {
      return this.TfsRequestContext.GetService<ExtensionDataService>().QueryCollections(this.TfsRequestContext, publisherName, extensionName, collectionQuery);
    }
  }
}
