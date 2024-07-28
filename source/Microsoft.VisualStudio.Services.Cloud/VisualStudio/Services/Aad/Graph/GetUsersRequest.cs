// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetUsersRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetUsersRequest : AadGraphClientPagedRequest<GetUsersResponse>
  {
    public IEnumerable<string> DisplayNamePrefixes { get; set; }

    public IEnumerable<string> SurnamePrefixes { get; set; }

    public IEnumerable<string> MailPrefixes { get; set; }

    public IEnumerable<string> MailNicknamePrefixes { get; set; }

    public IEnumerable<string> UserPrincipalNamePrefixes { get; set; }

    public IEnumerable<string> OnPremiseSecurityIdentifiers { get; set; }

    public IEnumerable<string> ImmutableIds { get; set; }

    public string ExpandProperty { get; set; }

    internal override void Validate()
    {
      int num = AadQueryUtils.CountTerms(this.DisplayNamePrefixes, this.SurnamePrefixes, this.MailPrefixes, this.MailNicknamePrefixes, this.UserPrincipalNamePrefixes, this.OnPremiseSecurityIdentifiers, this.ImmutableIds);
      if (num > 10)
        throw new ArgumentException(string.Format("Number of search terms ({0}) exceeds maximum (10).", (object) num));
    }

    internal override GetUsersResponse Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      FilterGenerator filter = new FilterGenerator();
      string currentFilter = string.Empty;
      if (this.DisplayNamePrefixes != null)
      {
        foreach (string displayNamePrefix in this.DisplayNamePrefixes)
        {
          if (!string.IsNullOrEmpty(displayNamePrefix))
            currentFilter = GetUsersRequest.ExtendFilter(currentFilter, "startswith(displayName,'" + AadQueryUtils.SanitizeInput(displayNamePrefix) + "')", ExpressionType.Or);
        }
      }
      if (this.SurnamePrefixes != null)
      {
        foreach (string surnamePrefix in this.SurnamePrefixes)
        {
          if (!string.IsNullOrEmpty(surnamePrefix))
            currentFilter = GetUsersRequest.ExtendFilter(currentFilter, "startswith(surname,'" + AadQueryUtils.SanitizeInput(surnamePrefix) + "')", ExpressionType.Or);
        }
      }
      if (this.MailPrefixes != null)
      {
        foreach (string mailPrefix in this.MailPrefixes)
        {
          if (!string.IsNullOrEmpty(mailPrefix))
          {
            currentFilter = GetUsersRequest.ExtendFilter(currentFilter, "startswith(mail,'" + AadQueryUtils.SanitizeInput(mailPrefix) + "')", ExpressionType.Or);
            currentFilter = GetUsersRequest.ExtendFilter(currentFilter, "otherMails/any(c:startswith(c,'" + AadQueryUtils.SanitizeInput(mailPrefix) + "'))", ExpressionType.Or);
          }
        }
      }
      if (this.MailNicknamePrefixes != null)
      {
        foreach (string mailNicknamePrefix in this.MailNicknamePrefixes)
        {
          if (!string.IsNullOrEmpty(mailNicknamePrefix))
            currentFilter = GetUsersRequest.ExtendFilter(currentFilter, "startswith(mailNickname,'" + AadQueryUtils.SanitizeInput(mailNicknamePrefix) + "')", ExpressionType.Or);
        }
      }
      if (this.UserPrincipalNamePrefixes != null)
      {
        foreach (string principalNamePrefix in this.UserPrincipalNamePrefixes)
        {
          if (!string.IsNullOrEmpty(principalNamePrefix))
            currentFilter = GetUsersRequest.ExtendFilter(currentFilter, "startswith(userPrincipalName,'" + AadQueryUtils.SanitizeInput(principalNamePrefix) + "')", ExpressionType.Or);
        }
      }
      if (this.OnPremiseSecurityIdentifiers != null)
      {
        foreach (string securityIdentifier in this.OnPremiseSecurityIdentifiers)
        {
          if (!string.IsNullOrEmpty(securityIdentifier))
            currentFilter = GetUsersRequest.ExtendFilter(currentFilter, "onPremisesSecurityIdentifier eq '" + AadQueryUtils.SanitizeInput(securityIdentifier) + "'", ExpressionType.Or);
        }
      }
      if (this.ImmutableIds != null)
      {
        foreach (string immutableId in this.ImmutableIds)
        {
          if (!string.IsNullOrEmpty(immutableId))
            currentFilter = GetUsersRequest.ExtendFilter(currentFilter, "immutableId eq '" + AadQueryUtils.SanitizeInput(immutableId) + "'", ExpressionType.Or);
        }
      }
      if (currentFilter != null)
        filter.OverrideQueryFilter = currentFilter;
      if (this.ExpandProperty == "Manager")
      {
        filter.ExpandProperty = LinkProperty.Manager;
        context.Trace(44750021, TraceLevel.Info, "VisualStudio.Services.Aad", "Service", "Usage of 'Manager' expand on 'GetUsers' Request");
      }
      int? maxResults = this.MaxResults;
      if (maxResults.HasValue)
      {
        FilterGenerator filterGenerator = filter;
        maxResults = this.MaxResults;
        int num = maxResults.Value;
        filterGenerator.Top = num;
      }
      PagedResults<User> pagedResults = connection.List<User>(this.PagingToken, filter);
      if (pagedResults == null || pagedResults.Results == null)
        throw new AadException("Failed to get users: connection returned an invalid response.");
      bool skipHasThumbnailPhoto = context.IsFeatureEnabled("VisualStudio.Services.Aad.SkipHasThumbnailPhoto");
      GetUsersResponse getUsersResponse = new GetUsersResponse();
      getUsersResponse.Users = pagedResults.Results.Select<User, AadUser>((Func<User, AadUser>) (user => AadGraphClient.ConvertUser(user, skipHasThumbnailPhoto)));
      getUsersResponse.PagingToken = pagedResults.PageToken;
      return getUsersResponse;
    }

    private static string ExtendFilter(
      string currentFilter,
      string newExpression,
      ExpressionType conjunction)
    {
      if (conjunction != ExpressionType.Or)
        throw new AadInternalException("The request filter can only be extended with the Or conjunction");
      return !string.IsNullOrEmpty(currentFilter) ? currentFilter + " or " + newExpression : newExpression;
    }

    public override string ToString() => string.Format("GetUsersRequest{0}DisplayNamePrefixes={1},SurnamePrefixes={2},MailPrefixes={3},MailNicknamePrefixes={4},UserPrincipalNamePrefixes={5}{6}", (object) "{", (object) AadQueryUtils.ToString(this.DisplayNamePrefixes), (object) AadQueryUtils.ToString(this.SurnamePrefixes), (object) AadQueryUtils.ToString(this.MailPrefixes), (object) AadQueryUtils.ToString(this.MailNicknamePrefixes), (object) AadQueryUtils.ToString(this.UserPrincipalNamePrefixes), (object) "}");
  }
}
