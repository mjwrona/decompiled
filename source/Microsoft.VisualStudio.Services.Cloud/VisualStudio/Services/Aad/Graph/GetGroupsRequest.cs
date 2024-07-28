// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.Graph.GetGroupsRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Services.Aad.Graph
{
  public class GetGroupsRequest : AadGraphClientPagedRequest<GetGroupsResponse>
  {
    private bool includeDistributionGroups = true;

    public IEnumerable<string> DisplayNamePrefixes { get; set; }

    public IEnumerable<string> MailNicknamePrefixes { get; set; }

    public IEnumerable<string> MailPrefixes { get; set; }

    public IEnumerable<string> OnPremisesSecurityIdentifiers { get; set; }

    public IEnumerable<string> ImmutableIds { get; set; }

    public bool IncludeDistributionGroups
    {
      internal get => this.includeDistributionGroups;
      set => this.includeDistributionGroups = value;
    }

    internal override void Validate()
    {
      int num = AadQueryUtils.CountTerms(this.DisplayNamePrefixes, this.MailNicknamePrefixes, this.MailPrefixes, this.OnPremisesSecurityIdentifiers, this.ImmutableIds);
      if (num > 10)
        throw new ArgumentException(string.Format("Number of search terms ({0}) exceeds maximum (10).", (object) num));
    }

    internal override GetGroupsResponse Execute(
      IVssRequestContext context,
      GraphConnection connection)
    {
      FilterGenerator filter = new FilterGenerator();
      string empty = string.Empty;
      string str1 = string.Empty;
      string str2 = string.Empty;
      if (!this.IncludeDistributionGroups)
        str1 = GetGroupsRequest.ExtendFilter(str1, "securityEnabled eq " + true.ToString().ToLowerInvariant(), ExpressionType.And);
      if (this.DisplayNamePrefixes != null)
      {
        foreach (string displayNamePrefix in this.DisplayNamePrefixes)
        {
          if (!string.IsNullOrEmpty(displayNamePrefix))
            str2 = GetGroupsRequest.ExtendFilter(str2, "startswith(displayName,'" + AadQueryUtils.SanitizeInput(displayNamePrefix) + "')", ExpressionType.Or);
        }
      }
      if (this.MailNicknamePrefixes != null)
      {
        foreach (string mailNicknamePrefix in this.MailNicknamePrefixes)
        {
          if (!string.IsNullOrEmpty(mailNicknamePrefix))
            str2 = GetGroupsRequest.ExtendFilter(str2, "startswith(mailNickname,'" + AadQueryUtils.SanitizeInput(mailNicknamePrefix) + "')", ExpressionType.Or);
        }
      }
      if (this.MailPrefixes != null)
      {
        foreach (string mailPrefix in this.MailPrefixes)
        {
          if (!string.IsNullOrEmpty(mailPrefix))
            str2 = GetGroupsRequest.ExtendFilter(str2, "startswith(mail,'" + AadQueryUtils.SanitizeInput(mailPrefix) + "')", ExpressionType.Or);
        }
      }
      if (this.OnPremisesSecurityIdentifiers != null)
      {
        foreach (string securityIdentifier in this.OnPremisesSecurityIdentifiers)
        {
          if (!string.IsNullOrEmpty(securityIdentifier))
            str2 = GetGroupsRequest.ExtendFilter(str2, "onPremisesSecurityIdentifier eq '" + AadQueryUtils.SanitizeInput(securityIdentifier) + "'", ExpressionType.Or);
        }
      }
      if (this.ImmutableIds != null)
      {
        foreach (string immutableId in this.ImmutableIds)
        {
          if (!string.IsNullOrEmpty(immutableId))
            str2 = GetGroupsRequest.ExtendFilter(str2, "immutableId eq '" + AadQueryUtils.SanitizeInput(immutableId) + "'", ExpressionType.Or);
        }
      }
      if (!string.IsNullOrWhiteSpace(str2) || !string.IsNullOrWhiteSpace(str1))
      {
        string str3 = GetGroupsRequest.ComposeFilterExpressions(str2, str1);
        filter.OverrideQueryFilter = str3;
      }
      int? maxResults = this.MaxResults;
      if (maxResults.HasValue)
      {
        FilterGenerator filterGenerator = filter;
        maxResults = this.MaxResults;
        int num = maxResults.Value;
        filterGenerator.Top = num;
      }
      PagedResults<Group> pagedResults = connection.List<Group>(this.PagingToken, filter);
      if (pagedResults == null || pagedResults.Results == null)
        throw new AadException("Failed to get groups: connection returned an invalid response.");
      GetGroupsResponse getGroupsResponse = new GetGroupsResponse();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      getGroupsResponse.Groups = pagedResults.Results.Select<Group, AadGroup>(GetGroupsRequest.\u003C\u003EO.\u003C0\u003E__ConvertGroup ?? (GetGroupsRequest.\u003C\u003EO.\u003C0\u003E__ConvertGroup = new Func<Group, AadGroup>(AadGraphClient.ConvertGroup)));
      getGroupsResponse.PagingToken = pagedResults.PageToken;
      return getGroupsResponse;
    }

    private static string ExtendFilter(
      string currentFilter,
      string newExpression,
      ExpressionType conjunction)
    {
      if (conjunction == ExpressionType.Or)
        return !string.IsNullOrEmpty(currentFilter) ? currentFilter + " or " + newExpression : newExpression;
      if (conjunction != ExpressionType.And)
        throw new AadInternalException("The request filter can only be extended with the Or conjunction");
      return !string.IsNullOrEmpty(currentFilter) ? currentFilter + " and " + newExpression : newExpression;
    }

    public override string ToString() => string.Format("GetGroupsRequest{0}DisplayNamePrefixes={1},MailNicknamePrefixes={2}{3}", (object) "{", (object) AadQueryUtils.ToString(this.DisplayNamePrefixes), (object) AadQueryUtils.ToString(this.MailNicknamePrefixes), (object) "}");

    private static string ComposeFilterExpressions(string orExpression, string andExpression)
    {
      bool flag1 = !string.IsNullOrWhiteSpace(andExpression);
      bool flag2 = !string.IsNullOrWhiteSpace(orExpression);
      if (!flag1 && !flag2)
        return string.Empty;
      if (!flag1 & flag2)
        return orExpression;
      return flag1 && !flag2 ? andExpression : andExpression + " and (" + orExpression + ")";
    }
  }
}
