// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.AbstractSearchCountHandler
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Query;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;

namespace Microsoft.VisualStudio.Services.Search.WebServer
{
  public abstract class AbstractSearchCountHandler : AbstractCountHandler
  {
    protected AbstractSearchCountHandler()
    {
    }

    protected AbstractSearchCountHandler(ICountRequestForwarder forwarder)
      : base(forwarder)
    {
    }

    protected override void SetIndexingStatusInResponse(
      IVssRequestContext requestContext,
      CountResponse response)
    {
      this.SetStatusBasedOnIndexingStatus(response, this.GetIndexingStatus(requestContext));
    }

    private CollectionIndexingStatus GetIndexingStatus(IVssRequestContext requestContext)
    {
      CollectionIndexingStatus indexingStatus = CollectionIndexingStatus.NotIndexing;
      IIndexingStatusService service = requestContext.GetService<IIndexingStatusService>();
      if (service.Supports(this.EntityType))
        indexingStatus = service.GetCollectionIndexingStatus(this.EntityType);
      return indexingStatus;
    }

    private void SetStatusBasedOnIndexingStatus(
      CountResponse response,
      CollectionIndexingStatus indexingStatus)
    {
      switch (indexingStatus)
      {
        case CollectionIndexingStatus.NotIndexing:
          break;
        case CollectionIndexingStatus.Onboarding:
          response.AddError(new ErrorData()
          {
            ErrorCode = "AccountIsBeingOnboarded",
            ErrorType = ErrorType.Warning
          });
          break;
        case CollectionIndexingStatus.Reindexing:
          response.AddError(new ErrorData()
          {
            ErrorCode = "AccountIsBeingReindexed",
            ErrorType = ErrorType.Warning
          });
          break;
        case CollectionIndexingStatus.BranchIndexing:
          response.AddError(new ErrorData()
          {
            ErrorCode = "BranchesAreBeingIndexed",
            ErrorType = ErrorType.Warning
          });
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (indexingStatus), (object) indexingStatus, (string) null);
      }
    }
  }
}
