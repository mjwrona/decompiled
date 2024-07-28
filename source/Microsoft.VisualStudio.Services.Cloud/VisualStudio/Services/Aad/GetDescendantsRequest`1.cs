// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetDescendantsRequest`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad.Graph;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetDescendantsRequest<TIdentifier> : AadServicePagedRequest
  {
    public GetDescendantsRequest()
    {
    }

    internal GetDescendantsRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    public TIdentifier Identifier { get; set; }

    public int Expand { get; set; } = 1;

    internal override void Validate()
    {
      AadServiceUtils.ValidateId<TIdentifier>(this.Identifier, AadServiceUtils.IdentifierType.Group, "Identifier");
      if (this.Expand != 1)
        throw new ArgumentException("Unsupported group expansion: " + this.Expand.ToString());
    }

    internal override AadServiceResponse Execute(AadServiceRequestContext context)
    {
      IAadGraphClient graphClient = context.GetGraphClient();
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      GetDescendantsRequest descendantsRequest = new GetDescendantsRequest();
      descendantsRequest.AccessToken = context.GetAccessToken();
      descendantsRequest.ObjectId = AadServiceUtils.MapIds<TIdentifier>(context.VssRequestContext, (IEnumerable<TIdentifier>) new TIdentifier[1]
      {
        this.Identifier
      }).First<KeyValuePair<TIdentifier, Guid>>().Value;
      descendantsRequest.MaxResults = this.MaxResults;
      descendantsRequest.PagingToken = this.PagingToken;
      descendantsRequest.Expand = this.Expand;
      GetDescendantsRequest request = descendantsRequest;
      Microsoft.VisualStudio.Services.Aad.Graph.GetDescendantsResponse descendants = graphClient.GetDescendants(vssRequestContext, request);
      GetDescendantsResponse descendantsResponse = new GetDescendantsResponse();
      descendantsResponse.Descendants = descendants.Descendants;
      descendantsResponse.PagingToken = descendants.PagingToken;
      return (AadServiceResponse) descendantsResponse;
    }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      IMicrosoftGraphClient msGraphClient = context.GetMsGraphClient();
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      MsGraphGetDescendantsRequest descendantsRequest = new MsGraphGetDescendantsRequest();
      descendantsRequest.AccessToken = context.GetAccessToken(true);
      descendantsRequest.GroupId = AadServiceUtils.MapIds<TIdentifier>(context.VssRequestContext, (IEnumerable<TIdentifier>) new TIdentifier[1]
      {
        this.Identifier
      }).First<KeyValuePair<TIdentifier, Guid>>().Value;
      descendantsRequest.PageSize = this.MaxResults;
      descendantsRequest.PagingToken = this.PagingToken;
      MsGraphGetDescendantsRequest request = descendantsRequest;
      MsGraphGetDescendantsResponse descendants = msGraphClient.GetDescendants(vssRequestContext, request);
      GetDescendantsResponse descendantsResponse = new GetDescendantsResponse();
      descendantsResponse.Descendants = descendants.Descendants;
      descendantsResponse.PagingToken = descendants.PagingToken;
      return (AadServiceResponse) descendantsResponse;
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.BothAadAndMicrosoftGraph;

    internal override bool UseBetaGraphVersion => true;
  }
}
