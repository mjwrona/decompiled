// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetGroupsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad.Graph;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Aad
{
  public class GetGroupsRequest : AadServicePagedRequest
  {
    private bool includeDistributionGroups = true;

    public GetGroupsRequest()
    {
    }

    internal GetGroupsRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    public IEnumerable<string> DisplayNamePrefixes { get; set; }

    public IEnumerable<string> MailNicknamePrefixes { get; set; }

    public IEnumerable<string> MailPrefixes { get; set; }

    public IEnumerable<string> OnPremiseSecurityIdentifiers { get; set; }

    public IEnumerable<string> ImmutableIds { get; set; }

    public bool IncludeDistributionGroups
    {
      internal get => this.includeDistributionGroups;
      set => this.includeDistributionGroups = value;
    }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.BothAadAndMicrosoftGraph;

    internal override void Validate()
    {
      int num = AadQueryUtils.CountTerms(this.DisplayNamePrefixes, this.MailNicknamePrefixes, this.MailPrefixes, this.OnPremiseSecurityIdentifiers, this.ImmutableIds);
      if (num > 10)
        throw new ArgumentException(string.Format("Number of search terms ({0}) exceeds maximum (10).", (object) num));
    }

    internal override AadServiceResponse Execute(AadServiceRequestContext context)
    {
      IAadGraphClient graphClient = context.GetGraphClient();
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      Microsoft.VisualStudio.Services.Aad.Graph.GetGroupsRequest request = new Microsoft.VisualStudio.Services.Aad.Graph.GetGroupsRequest();
      request.AccessToken = context.GetAccessToken();
      request.DisplayNamePrefixes = this.DisplayNamePrefixes;
      request.MailNicknamePrefixes = this.MailNicknamePrefixes;
      request.MailPrefixes = this.MailPrefixes;
      request.OnPremisesSecurityIdentifiers = this.OnPremiseSecurityIdentifiers;
      request.ImmutableIds = this.ImmutableIds;
      request.IncludeDistributionGroups = this.IncludeDistributionGroups;
      request.MaxResults = this.MaxResults;
      request.PagingToken = this.PagingToken;
      Microsoft.VisualStudio.Services.Aad.Graph.GetGroupsResponse groups = graphClient.GetGroups(vssRequestContext, request);
      GetGroupsResponse getGroupsResponse = new GetGroupsResponse();
      getGroupsResponse.Groups = groups.Groups;
      getGroupsResponse.PagingToken = groups.PagingToken;
      return (AadServiceResponse) getGroupsResponse;
    }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      MsGraphGetGroupsRequest request;
      if (string.IsNullOrEmpty(this.PagingToken))
      {
        MsGraphGetGroupsRequest getGroupsRequest = new MsGraphGetGroupsRequest();
        getGroupsRequest.AccessToken = context.GetAccessToken(true);
        getGroupsRequest.DisplayNamePrefixes = this.DisplayNamePrefixes;
        getGroupsRequest.MailNicknamePrefixes = this.MailNicknamePrefixes;
        getGroupsRequest.MailPrefixes = this.MailPrefixes;
        getGroupsRequest.OnPremisesSecurityIdentifiers = this.OnPremiseSecurityIdentifiers;
        getGroupsRequest.IncludeDistributionGroups = this.IncludeDistributionGroups;
        getGroupsRequest.PageSize = this.MaxResults;
        request = getGroupsRequest;
      }
      else
      {
        MsGraphGetGroupsRequest getGroupsRequest = new MsGraphGetGroupsRequest();
        getGroupsRequest.AccessToken = context.GetAccessToken(true);
        getGroupsRequest.PagingToken = this.PagingToken;
        request = getGroupsRequest;
      }
      MsGraphGetGroupsResponse groups = context.GetMsGraphClient().GetGroups(context.VssRequestContext, request);
      GetGroupsResponse getGroupsResponse = new GetGroupsResponse();
      getGroupsResponse.Groups = groups.Groups;
      getGroupsResponse.PagingToken = groups.PagingToken;
      return (AadServiceResponse) getGroupsResponse;
    }
  }
}
