// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AzurePublisherController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "azurepublisher")]
  public class AzurePublisherController : GalleryController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<AzurePublisherExistsException>(HttpStatusCode.Conflict);
      exceptionMap.AddStatusCode<AzurePublisherDoesNotExistException>(HttpStatusCode.NotFound);
    }

    [HttpGet]
    public AzurePublisher QueryAssociatedAzurePublisher(string publisherName) => this.TfsRequestContext.GetService<IPublisherService>().QueryAssociatedAzurePublisher(this.TfsRequestContext, publisherName);

    [HttpPut]
    public AzurePublisher AssociateAzurePublisher(string publisherName, string azurePublisherId) => this.TfsRequestContext.GetService<IPublisherService>().AssociateAzurePublisher(this.TfsRequestContext, new AzurePublisher(publisherName, azurePublisherId));
  }
}
