// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.Classifier.UpstreamStatusUpdateHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.Classifier
{
  public class UpstreamStatusUpdateHandler : IUpstreamStatusHandler
  {
    private readonly IUpstreamStatusStorage upstreamStatusStorage;
    private readonly ITimeProvider timeProvider;
    private readonly ITracerService tracerService;

    public UpstreamStatusUpdateHandler(
      IUpstreamStatusStorage upstreamStatusStorage,
      ITimeProvider timeProvider,
      ITracerService tracerService)
    {
      this.upstreamStatusStorage = upstreamStatusStorage;
      this.timeProvider = timeProvider;
      this.tracerService = tracerService;
    }

    public async Task Handle(
      IFeedRequest feedRequest,
      UpstreamPackageRefreshResult results,
      IEnumerable<UpstreamSource> upstreams)
    {
      UpstreamStatusUpdateHandler sendInTheThisObject = this;
      if (results.Category == UpstreamStatusCategory.Aborted)
        return;
      if (results.RefreshScope == UpstreamRefreshScope.Package)
        throw new NotImplementedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageScopeUpstreamStatusNotImplemented());
      using (ITracerBlock tracer = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        try
        {
          if (results.Category != UpstreamStatusCategory.FullRefreshSuccess && results.Results != null && results.Results.Any<RefreshPackageResult>() && results.Results.First<RefreshPackageResult>().IsFailed)
            await sendInTheThisObject.upstreamStatusStorage.AddUpstreamRefreshStatus(UpstreamKey.FromFeed(feedRequest.Feed, results.Results.First<RefreshPackageResult>().UpstreamSource), results.RefreshScope, (IEnumerable<UpstreamStatusCategory>) new UpstreamStatusCategory[1]
            {
              results.Category
            }, sendInTheThisObject.timeProvider.Now);
          else if (results.Category == UpstreamStatusCategory.FullRefreshSuccess && results.RefreshScope == UpstreamRefreshScope.Full)
          {
            foreach (UpstreamSource upstream in upstreams)
              await sendInTheThisObject.upstreamStatusStorage.AddUpstreamRefreshStatus(UpstreamKey.FromFeed(feedRequest.Feed, upstream), results.RefreshScope, (IEnumerable<UpstreamStatusCategory>) new UpstreamStatusCategory[1]
              {
                results.Category
              }, sendInTheThisObject.timeProvider.Now);
          }
          else
            tracer.TraceInfo(string.Format("Upstream status not stored. Category: '{0}', scope: '{1}', results: '{2}'", (object) results.Category, (object) results.RefreshScope, (object) results.Results));
        }
        catch (Exception ex)
        {
          tracer.TraceError(string.Format("Failed to store upstream status. Category: '{0}', scope: '{1}', results: '{2}'", (object) results.Category, (object) results.RefreshScope, (object) results.Results));
          tracer.TraceException(ex);
        }
      }
    }
  }
}
