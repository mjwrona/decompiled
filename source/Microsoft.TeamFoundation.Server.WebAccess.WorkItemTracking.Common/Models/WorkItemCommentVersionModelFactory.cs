// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemCommentVersionModelFactory
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  internal class WorkItemCommentVersionModelFactory
  {
    internal static Dictionary<int, WorkItemCommentVersionModel> Create(
      IVssRequestContext requestContext,
      Dictionary<int, WorkItemCommentVersionRecord> commentVersions,
      ISecuredObject securedObject)
    {
      List<Guid> vsids = new List<Guid>();
      if (commentVersions != null && commentVersions.Count > 0)
      {
        foreach (WorkItemCommentVersionRecord commentVersionRecord in commentVersions.Values)
        {
          vsids.Add(commentVersionRecord.CreatedBy);
          vsids.Add(commentVersionRecord.ModifiedBy);
          Guid result;
          if (Guid.TryParse(commentVersionRecord.CreatedOnBehalfOf, out result))
            vsids.Add(result);
        }
      }
      IDictionary<Guid, IdentityReference> identityReferencesById = IdentityReferenceBuilder.Create(requestContext, (IEnumerable<Guid>) vsids, true, true);
      return commentVersions.ToDictionary<KeyValuePair<int, WorkItemCommentVersionRecord>, int, WorkItemCommentVersionModel>((Func<KeyValuePair<int, WorkItemCommentVersionRecord>, int>) (pair => pair.Key), (Func<KeyValuePair<int, WorkItemCommentVersionRecord>, WorkItemCommentVersionModel>) (pair => WorkItemCommentVersionModel.FromCommentVersionRecord(requestContext, pair.Value, identityReferencesById, securedObject)));
    }
  }
}
