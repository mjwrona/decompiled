// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.GetUsersRequest
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
  public class GetUsersRequest : AadServicePagedRequest
  {
    public GetUsersRequest()
    {
    }

    internal GetUsersRequest(AadServiceRequest request) => this.CopyPropertiesFrom(request);

    public IEnumerable<string> DisplayNamePrefixes { get; set; }

    public IEnumerable<string> SurnamePrefixes { get; set; }

    public IEnumerable<string> MailPrefixes { get; set; }

    public IEnumerable<string> MailNicknamePrefixes { get; set; }

    public IEnumerable<string> UserPrincipalNamePrefixes { get; set; }

    public IEnumerable<string> OnPremiseSecurityIdentifiers { get; set; }

    public IEnumerable<string> ImmutableIds { get; set; }

    public string ExpandProperty { get; set; }

    internal override GraphApiSupportLevel GraphApiSupportLevel => GraphApiSupportLevel.BothAadAndMicrosoftGraph;

    internal override void Validate()
    {
      int num = AadQueryUtils.CountTerms(this.DisplayNamePrefixes, this.SurnamePrefixes, this.MailPrefixes, this.MailNicknamePrefixes, this.UserPrincipalNamePrefixes, this.OnPremiseSecurityIdentifiers, this.ImmutableIds);
      if (num > 10)
        throw new ArgumentException(string.Format("Number of search terms ({0}) exceeds maximum (10).", (object) num));
    }

    internal override AadServiceResponse Execute(AadServiceRequestContext context)
    {
      IAadGraphClient graphClient = context.GetGraphClient();
      IVssRequestContext vssRequestContext = context.VssRequestContext;
      Microsoft.VisualStudio.Services.Aad.Graph.GetUsersRequest request = new Microsoft.VisualStudio.Services.Aad.Graph.GetUsersRequest();
      request.AccessToken = context.GetAccessToken();
      request.DisplayNamePrefixes = this.DisplayNamePrefixes;
      request.SurnamePrefixes = this.SurnamePrefixes;
      request.MailPrefixes = this.MailPrefixes;
      request.MailNicknamePrefixes = this.MailNicknamePrefixes;
      request.UserPrincipalNamePrefixes = this.UserPrincipalNamePrefixes;
      request.OnPremiseSecurityIdentifiers = this.OnPremiseSecurityIdentifiers;
      request.ImmutableIds = this.ImmutableIds;
      request.ExpandProperty = this.ExpandProperty;
      request.MaxResults = this.MaxResults;
      request.PagingToken = this.PagingToken;
      Microsoft.VisualStudio.Services.Aad.Graph.GetUsersResponse users = graphClient.GetUsers(vssRequestContext, request);
      GetUsersResponse getUsersResponse = new GetUsersResponse();
      getUsersResponse.Users = users.Users;
      getUsersResponse.PagingToken = users.PagingToken;
      return (AadServiceResponse) getUsersResponse;
    }

    internal override AadServiceResponse ExecuteWithMicrosoftGraph(
      AadServiceRequestContext context,
      bool bypassCache = false)
    {
      MsGraphGetUsersRequest request;
      if (string.IsNullOrEmpty(this.PagingToken))
      {
        MsGraphGetUsersRequest graphGetUsersRequest = new MsGraphGetUsersRequest();
        graphGetUsersRequest.AccessToken = context.GetAccessToken(true);
        graphGetUsersRequest.DisplayNamePrefixes = this.DisplayNamePrefixes;
        graphGetUsersRequest.SurnamePrefixes = this.SurnamePrefixes;
        graphGetUsersRequest.MailPrefixes = this.MailPrefixes;
        graphGetUsersRequest.MailNicknamePrefixes = this.MailNicknamePrefixes;
        graphGetUsersRequest.UserPrincipalNamePrefixes = this.UserPrincipalNamePrefixes;
        graphGetUsersRequest.OnPremiseSecurityIdentifiers = this.OnPremiseSecurityIdentifiers;
        graphGetUsersRequest.ImmutableIds = this.ImmutableIds;
        graphGetUsersRequest.ExpandProperty = this.ExpandProperty;
        graphGetUsersRequest.PageSize = this.MaxResults;
        request = graphGetUsersRequest;
      }
      else
      {
        MsGraphGetUsersRequest graphGetUsersRequest = new MsGraphGetUsersRequest();
        graphGetUsersRequest.AccessToken = context.GetAccessToken(true);
        graphGetUsersRequest.PagingToken = this.PagingToken;
        request = graphGetUsersRequest;
      }
      MsGraphGetUsersResponse users = context.GetMsGraphClient().GetUsers(context.VssRequestContext, request);
      GetUsersResponse getUsersResponse = new GetUsersResponse();
      getUsersResponse.Users = users.Users;
      getUsersResponse.PagingToken = users.PagingToken;
      return (AadServiceResponse) getUsersResponse;
    }
  }
}
