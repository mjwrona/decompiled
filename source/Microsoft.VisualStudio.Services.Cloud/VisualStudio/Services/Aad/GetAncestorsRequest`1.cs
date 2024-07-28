// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetAncestorsRequest`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetAncestorsRequest<TIdentifier> : AadServicePagedRequest
  {
    public GetAncestorsRequest()
    {
    }

    internal GetAncestorsRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    public TIdentifier Identifier { get; set; }

    public int Expand { get; set; }

    internal override void Validate()
    {
      AadServiceUtils.ValidateId<TIdentifier>(this.Identifier, name: "Identifier");
      if (this.Expand != 1)
        throw new ArgumentException("Unsupported group expansion: " + this.Expand.ToString());
    }

    internal override AadServiceResponse Execute(AadServiceRequestContext context)
    {
      IAadGraphClient graphClient = context.GetGraphClient();
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      GetAncestorsRequest ancestorsRequest = new GetAncestorsRequest();
      ancestorsRequest.AccessToken = context.GetAccessToken();
      ancestorsRequest.ObjectId = AadServiceUtils.MapIds<TIdentifier>(context.VssRequestContext, (IEnumerable<TIdentifier>) new TIdentifier[1]
      {
        this.Identifier
      }).First<KeyValuePair<TIdentifier, Guid>>().Value;
      ancestorsRequest.MaxResults = this.MaxResults;
      ancestorsRequest.PagingToken = this.PagingToken;
      ancestorsRequest.Expand = this.Expand;
      GetAncestorsRequest request = ancestorsRequest;
      Microsoft.VisualStudio.Services.Aad.Graph.GetAncestorsResponse ancestors = graphClient.GetAncestors(vssRequestContext, request);
      GetAncestorsResponse ancestorsResponse = new GetAncestorsResponse();
      ancestorsResponse.Ancestors = ancestors.Ancestors;
      ancestorsResponse.PagingToken = ancestors.PagingToken;
      return (AadServiceResponse) ancestorsResponse;
    }
  }
}
