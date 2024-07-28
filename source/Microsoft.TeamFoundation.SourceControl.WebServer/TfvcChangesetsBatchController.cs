// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcChangesetsBatchController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientGroupByResource("changesets")]
  public class TfvcChangesetsBatchController : TfvcApiController
  {
    [ClientExample("POST__tfvc_changesetsBatched.json", "Returns the Tfvc changes for a given set of changes", null, null)]
    [HttpPost]
    public IList<TfvcChangesetRef> GetBatchedChangesets(
      TfvcChangesetsRequestData changesetsRequestData)
    {
      if (changesetsRequestData == null || changesetsRequestData.changesetIds == null || changesetsRequestData.changesetIds.Length == 0)
        throw new InvalidArgumentValueException(Resources.Get("ErrorChangesetsNotSpecified"));
      TfvcChangesetsCollection changesetsCollection = new TfvcChangesetsCollection();
      foreach (int changesetId in changesetsRequestData.changesetIds)
      {
        try
        {
          TfvcChangeset changesetById = TfvcChangesetUtility.GetChangesetById(this.TfsRequestContext, this.Url, VersionSpecCommon.ParseChangesetNumber((VersionSpecFactory) VersionSpec.ServerVersionSpecFactory, changesetId.ToString()), 0, false, false, false, changesetsRequestData.commentLength);
          changesetById.Links = changesetsRequestData.IncludeLinks ? changesetById.GetChangesetsReferenceLinks(this.TfsRequestContext, this.Url) : (ReferenceLinks) null;
          changesetsCollection.Add((TfvcChangesetRef) changesetById);
        }
        catch (InvalidVersionSpecException ex)
        {
          changesetsCollection.Add((TfvcChangesetRef) null);
        }
        catch (ChangesetNotFoundException ex)
        {
          changesetsCollection.Add((TfvcChangesetRef) null);
        }
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return !changesetsCollection.TrueForAll(TfvcChangesetsBatchController.\u003C\u003EO.\u003C0\u003E__IsNull ?? (TfvcChangesetsBatchController.\u003C\u003EO.\u003C0\u003E__IsNull = new Predicate<TfvcChangesetRef>(TfvcChangesetsBatchController.IsNull))) ? (IList<TfvcChangesetRef>) changesetsCollection : throw new ChangesetBatchNotFoundException(Resources.Get("ErrorChangesetBatchNotFound"));
    }

    private static bool IsNull(TfvcChangesetRef tfvcChangesetRef) => tfvcChangesetRef == null;
  }
}
