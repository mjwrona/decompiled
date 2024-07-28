// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetAncestorsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetAncestorsRequest : AadGraphClientPagedRequest<GetAncestorsResponse>
  {
    public Guid ObjectId { get; set; }

    public int Expand { get; set; }

    internal override void Validate()
    {
      ArgumentUtility.CheckForEmptyGuid(this.ObjectId, "ObjectId");
      if (this.Expand != 1)
        throw new ArgumentException("Unsupported group expansion: " + this.Expand.ToString());
    }

    internal override GetAncestorsResponse Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      Group group = new Group(this.ObjectId.ToString());
      PagedResults<GraphObject> pagedResults = !this.MaxResults.HasValue ? connection.GetLinkedObjects((GraphObject) group, LinkProperty.MemberOf, this.PagingToken) : connection.GetLinkedObjects((GraphObject) group, LinkProperty.MemberOf, this.PagingToken, this.MaxResults.Value);
      if (pagedResults == null)
        throw new AadException("Failed to get links: connection returned an invalid response.");
      GetAncestorsResponse ancestorsResponse = new GetAncestorsResponse();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ancestorsResponse.Ancestors = pagedResults.Results.Where<GraphObject>((Func<GraphObject, bool>) (o => o is Group)).Cast<Group>().Select<Group, AadGroup>(GetAncestorsRequest.\u003C\u003EO.\u003C0\u003E__ConvertGroup ?? (GetAncestorsRequest.\u003C\u003EO.\u003C0\u003E__ConvertGroup = new Func<Group, AadGroup>(AadGraphClient.ConvertGroup)));
      ancestorsResponse.PagingToken = pagedResults.PageToken;
      return ancestorsResponse;
    }

    public override string ToString() => string.Format("GetAncestorsRequest{0}ObjectId={1},Expand={2}{3}", (object) "{", (object) this.ObjectId, (object) this.Expand, (object) "}");
  }
}
