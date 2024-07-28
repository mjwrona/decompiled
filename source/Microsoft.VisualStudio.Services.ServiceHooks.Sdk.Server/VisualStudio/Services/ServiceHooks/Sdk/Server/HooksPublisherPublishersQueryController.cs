// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.HooksPublisherPublishersQueryController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "hooks", ResourceName = "PublishersQuery")]
  [ClientGroupByResource("publishers")]
  public class HooksPublisherPublishersQueryController : ServiceHooksPublisherControllerBase
  {
    [HttpPost]
    public PublishersQuery QueryPublishers(PublishersQuery query)
    {
      ArgumentUtility.CheckForNull<PublishersQuery>(query, nameof (query));
      IEnumerable<ServiceHooksPublisher> serviceHooksPublishers = this.FindPublishers((string) null);
      if (query.PublisherIds != null && query.PublisherIds.Count > 0)
      {
        HashSet<string> ids = new HashSet<string>((IEnumerable<string>) query.PublisherIds);
        serviceHooksPublishers = serviceHooksPublishers.Where<ServiceHooksPublisher>((Func<ServiceHooksPublisher, bool>) (p => ids.Contains(p.Id)));
      }
      query.Results = serviceHooksPublishers.ToPublisherModels(this.TfsRequestContext, query.PublisherInputs);
      return query;
    }
  }
}
