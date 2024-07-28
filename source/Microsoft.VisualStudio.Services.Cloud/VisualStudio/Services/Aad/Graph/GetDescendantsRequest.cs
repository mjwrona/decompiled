// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetDescendantsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetDescendantsRequest : AadGraphClientPagedRequest<GetDescendantsResponse>
  {
    public Guid ObjectId { get; set; }

    public int Expand { get; set; }

    internal override void Validate()
    {
      ArgumentUtility.CheckForEmptyGuid(this.ObjectId, "ObjectId");
      if (this.Expand != 1 && this.Expand != -1)
        throw new ArgumentException("Unsupported group expansion: " + this.Expand.ToString());
    }

    internal override GetDescendantsResponse Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      switch (this.Expand)
      {
        case -1:
          return this.GetDescendantsTransitive(context, connection);
        case 1:
          return this.GetDescendantsDirect(context, connection);
        default:
          throw new AadInternalException("Unsupported group expansion: " + this.Expand.ToString());
      }
    }

    private GetDescendantsResponse GetDescendantsDirect(
      IVssRequestContext context,
      GraphConnection connection)
    {
      DirectoryObject directoryObject = new DirectoryObject()
      {
        ObjectId = this.ObjectId.ToString()
      };
      PagedResults<GraphObject> pagedResults = !this.MaxResults.HasValue ? connection.GetLinkedObjects((GraphObject) directoryObject, LinkProperty.Members, this.PagingToken) : connection.GetLinkedObjects((GraphObject) directoryObject, LinkProperty.Members, this.PagingToken, this.MaxResults.Value);
      if (pagedResults == null)
        throw new AadException("Failed to get links: connection returned an invalid response.");
      bool skipHasThumbnailPhoto = context.IsFeatureEnabled("VisualStudio.Services.Aad.SkipHasThumbnailPhoto");
      GetDescendantsResponse descendantsDirect = new GetDescendantsResponse();
      descendantsDirect.Descendants = pagedResults.Results.Select<GraphObject, AadObject>((Func<GraphObject, AadObject>) (obj => AadGraphClient.ConvertObject(obj, skipHasThumbnailPhoto))).Where<AadObject>((Func<AadObject, bool>) (x => x != null));
      descendantsDirect.PagingToken = pagedResults.PageToken;
      return descendantsDirect;
    }

    private GetDescendantsResponse GetDescendantsTransitive(
      IVssRequestContext context,
      GraphConnection connection)
    {
      IList<User> transitiveGroupMembers = connection.GetTransitiveGroupMembers(this.ObjectId.ToString(), false, out Group _);
      if (transitiveGroupMembers == null)
        throw new AadException("Failed to get transitive group members: connection returned an invalid response.");
      bool skipHasThumbnailPhoto = context.IsFeatureEnabled("VisualStudio.Services.Aad.SkipHasThumbnailPhoto");
      return new GetDescendantsResponse()
      {
        Descendants = (IEnumerable<AadObject>) transitiveGroupMembers.Select<User, AadUser>((Func<User, AadUser>) (user => AadGraphClient.ConvertUser(user, skipHasThumbnailPhoto)))
      };
    }

    public override string ToString() => string.Format("GetDescendantsRequest{0}ObjectId={1},Expand={2}{3}", (object) "{", (object) this.ObjectId, (object) this.Expand, (object) "}");
  }
}
