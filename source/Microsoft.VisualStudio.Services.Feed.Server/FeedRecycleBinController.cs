// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedRecycleBinController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "FeedRecycleBin")]
  [FeatureEnabled("Packaging.Feed.Service")]
  public class FeedRecycleBinController : FeedApiController
  {
    [HttpGet]
    [PublicProjectRequestRestrictions]
    public IEnumerable<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedsFromRecycleBin()
    {
      IFeedRecycleBinService recycleBinService = this.FeedRecycleBinService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      return recycleBinService.GetFeedsFromRecycleBin(tfsRequestContext, projectReference).SetSecuredObject();
    }

    [HttpPatch]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    public void RestoreDeletedFeed(
      string feedId,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<IDictionary<string, object>> patchJson)
    {
      patchJson.ThrowIfNull<PatchDocument<IDictionary<string, object>>>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (patchJson))));
      List<IPatchOperation<IDictionary<string, object>>> list = patchJson.Operations.ToList<IPatchOperation<IDictionary<string, object>>>();
      if (list.Count == 1 && !(list[0].Path != "/isDeleted") && list[0].Operation == Operation.Replace)
      {
        bool? nullable = list[0].Value as bool?;
        bool flag = false;
        if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
        {
          IFeedRecycleBinService recycleBinService = this.FeedRecycleBinService;
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          ProjectInfo projectInfo = this.ProjectInfo;
          ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
          string feedId1 = feedId;
          recycleBinService.RestoreDeletedFeed(tfsRequestContext, projectReference, feedId1);
          return;
        }
      }
      throw new NotSupportedException(Resources.Error_RecycleBinPatchOnlySupportsRestoreFeed());
    }

    [HttpDelete]
    public void PermanentDeleteFeed(string feedId)
    {
      IFeedRecycleBinService recycleBinService = this.FeedRecycleBinService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      string feedId1 = feedId;
      recycleBinService.PermanentDeleteFeed(tfsRequestContext, projectReference, feedId1);
    }
  }
}
