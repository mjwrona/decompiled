// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.HooksPublisherPublishersController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "hooks", ResourceName = "Publishers")]
  public class HooksPublisherPublishersController : ServiceHooksPublisherControllerBase
  {
    [HttpGet]
    [ClientExample("GET__hooks_publishers.json", null, null, null)]
    public IEnumerable<Publisher> ListPublishers()
    {
      IList<Publisher> publisherModels = this.TfsRequestContext.GetService<ServiceHooksPublisherService>().GetPublishers(this.TfsRequestContext).ToPublisherModels(this.TfsRequestContext);
      publisherModels.SetPublisherUrl(this.Url, this.TfsRequestContext);
      return (IEnumerable<Publisher>) publisherModels;
    }

    public Publisher GetPublisher(string publisherId)
    {
      Publisher publisherModel = this.TfsRequestContext.GetService<ServiceHooksPublisherService>().GetPublisher(this.TfsRequestContext, publisherId).ToPublisherModel(this.TfsRequestContext);
      publisherModel.SetPublisherUrl(this.Url, this.TfsRequestContext);
      return publisherModel;
    }
  }
}
