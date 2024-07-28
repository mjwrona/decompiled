// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MsGraphFilterBuilder
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Aad;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  internal class MsGraphFilterBuilder : IMsGraphFilterBuilder
  {
    private readonly StringBuilder builder = new StringBuilder();

    public string BuildFilter() => this.builder.ToString();

    public IMsGraphFilterBuilder WithSearchPrefix(
      IEnumerable<string> searchTerms,
      string searchFilter)
    {
      foreach (string searchTerm in searchTerms)
      {
        if (!string.IsNullOrEmpty(searchTerm))
          this.ExtendFilter(this.builder, AadQueryUtils.SanitizeInput(searchTerm), searchFilter, SearchType.Prefix);
      }
      return (IMsGraphFilterBuilder) this;
    }

    public IMsGraphFilterBuilder WithSearchEqualByGuid(
      IEnumerable<string> searchTerms,
      string searchFilter)
    {
      foreach (string searchTerm in searchTerms)
      {
        if (this.IsValidGuid(searchTerm))
          this.ExtendFilter(this.builder, searchTerm, searchFilter, SearchType.Eq);
      }
      return (IMsGraphFilterBuilder) this;
    }

    public IMsGraphFilterBuilder WithSearchEqualByString(
      IEnumerable<string> searchTerms,
      string searchFilter)
    {
      foreach (string searchTerm in searchTerms)
      {
        if (!string.IsNullOrEmpty(searchTerm))
          this.ExtendFilter(this.builder, AadQueryUtils.SanitizeInput(searchTerm), searchFilter, SearchType.Eq);
      }
      return (IMsGraphFilterBuilder) this;
    }

    public IMsGraphFilterBuilder WithSingleSearchEqualByBooleanExpressionAnd(
      string searchTerm,
      string searchFilter)
    {
      if (!string.IsNullOrEmpty(searchTerm))
        this.ExtendFilterAnd(this.builder, AadQueryUtils.SanitizeInput(searchTerm), searchFilter, SearchType.Eq);
      return (IMsGraphFilterBuilder) this;
    }

    public IMsGraphFilterBuilder WithCustomSearchParam(
      IEnumerable<string> searchTerms,
      Func<string, string> searchFilter)
    {
      foreach (string searchTerm in searchTerms)
      {
        if (!string.IsNullOrEmpty(searchTerm))
          this.ExtendCustomFilter(this.builder, searchFilter(searchTerm), SearchType.Custom);
      }
      return (IMsGraphFilterBuilder) this;
    }

    private bool IsValidGuid(string str) => Guid.TryParse(str, out Guid _);

    private StringBuilder ExtendFilter(
      StringBuilder currentFilter,
      string searchTerm,
      string searchFilter,
      SearchType searchType,
      ExpressionType conjunction = ExpressionType.Or)
    {
      if (conjunction != ExpressionType.Or)
        throw new AadInternalException("The request filter can only be extended with the Or conjunction");
      if (currentFilter.Length != 0)
        currentFilter.Append(" or ");
      if (searchType == SearchType.Prefix)
        return currentFilter.Append("startsWith(").Append(searchFilter).Append(",'").Append(searchTerm).Append("')");
      if (SearchType.Eq == searchType)
        return currentFilter.Append(searchFilter).Append(" eq '").Append(searchTerm).Append("'");
      throw new AadInternalException("The request filter can only be setup to search for prefixes or equality");
    }

    private StringBuilder ExtendCustomFilter(
      StringBuilder currentFilter,
      string customSearchFilter,
      SearchType searchType,
      ExpressionType conjunction = ExpressionType.Or)
    {
      if (SearchType.Custom != searchType)
        throw new AadInternalException("The request filter can only be setup to search for custom search terms");
      if (conjunction != ExpressionType.Or)
        throw new AadInternalException("The request filter can only be extended with the Or conjunction");
      if (currentFilter.Length != 0)
        currentFilter.Append(" or ");
      return currentFilter.Append(customSearchFilter);
    }

    public StringBuilder ExtendFilterAnd(
      StringBuilder currentFilter,
      string searchTerm,
      string searchFilter,
      SearchType searchType,
      ExpressionType conjunction = ExpressionType.And)
    {
      if (SearchType.Eq != searchType || conjunction != ExpressionType.And)
        throw new AadInternalException("The request filter can only be setup to search for equality and can only be extended with the AND conjunction");
      if (currentFilter.Length == 0)
        return currentFilter.Append(searchFilter).Append(" eq ").Append(searchTerm);
      currentFilter.Insert(0, searchFilter + " eq " + searchTerm + " and (");
      return currentFilter.Append(")");
    }
  }
}
