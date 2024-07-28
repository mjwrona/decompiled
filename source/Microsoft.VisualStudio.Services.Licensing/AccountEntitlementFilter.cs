// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.AccountEntitlementFilter
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.OData.UriParser;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing.Internal;
using Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class AccountEntitlementFilter
  {
    private static UriQueryExpressionParser parser = new UriQueryExpressionParser(128);
    private static readonly AccountEntitlementFilterQuerySyntacticTreeVisitor parseTreeVisitor = new AccountEntitlementFilterQuerySyntacticTreeVisitor();

    public IList<LicenseFilter> Licenses { get; set; } = (IList<LicenseFilter>) new List<LicenseFilter>();

    public string NameOrEmail { get; set; } = string.Empty;

    public IList<IdentityMetaType> UserTypes { get; set; } = (IList<IdentityMetaType>) new List<IdentityMetaType>();

    public IList<AssignmentSource> AssignmentSources { get; set; } = (IList<AssignmentSource>) new List<AssignmentSource>();

    public bool IsEmpty()
    {
      return IsNullOrEmpty<LicenseFilter>(this.Licenses) && IsNullOrEmpty<IdentityMetaType>(this.UserTypes) && string.IsNullOrEmpty(this.NameOrEmail);

      static bool IsNullOrEmpty<T>(IList<T> list) => list == null || list.Count == 0;
    }

    public string ToQueryString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.Licenses.Count > 0)
      {
        if (this.Licenses.Count > 1)
          stringBuilder.Append("(");
        stringBuilder.Append(string.Join(" or ", this.Licenses.Select<LicenseFilter, string>((Func<LicenseFilter, string>) (license => license.ToQueryString()))));
        if (this.Licenses.Count > 1)
          stringBuilder.Append(")");
      }
      if (!string.IsNullOrEmpty(this.NameOrEmail))
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(" and ");
        stringBuilder.AppendFormat("{0} eq '{1}'", (object) "name", (object) ParsingUtilities.EscapeSingleQuote(this.NameOrEmail));
      }
      if (this.UserTypes.Count > 0)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(" and ");
        if (this.UserTypes.Count > 1)
          stringBuilder.Append("(");
        stringBuilder.Append(string.Join(" or ", this.UserTypes.Select<IdentityMetaType, string>((Func<IdentityMetaType, string>) (userType => string.Format("{0} eq '{1}'", (object) nameof (userType), (object) GraphObjectExtensionHelpers.ConvertToGraphUserMetaType(userType))))));
        if (this.UserTypes.Count > 1)
          stringBuilder.Append(")");
      }
      if (this.AssignmentSources.Count > 0)
      {
        if (stringBuilder.Length > 0)
          stringBuilder.Append(" and ");
        if (this.AssignmentSources.Count > 1)
          stringBuilder.Append("(");
        stringBuilder.Append(string.Join(" or ", this.AssignmentSources.Select<AssignmentSource, string>((Func<AssignmentSource, string>) (source => string.Format("{0} eq '{1}'", (object) "assignmentSource", (object) source)))));
        if (this.AssignmentSources.Count > 1)
          stringBuilder.Append(")");
      }
      return stringBuilder.ToString();
    }

    public static AccountEntitlementFilter Parse(string filterQueryString)
    {
      if (filterQueryString == null)
        return (AccountEntitlementFilter) null;
      try
      {
        IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter> dictionary = AccountEntitlementFilter.parser.ParseFilter(filterQueryString).Accept<IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>>((ISyntacticTreeVisitor<IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>>) AccountEntitlementFilter.parseTreeVisitor);
        if (dictionary == null)
          throw new InvalidQueryStringException(LicensingResources.InvalidFilterQuery((object) StringUtil.Truncate(filterQueryString, 100, true)));
        AccountEntitlementFilter entitlementFilter = new AccountEntitlementFilter();
        foreach (AccountEntitlementSinglePropertyFilter singlePropertyFilter in (IEnumerable<AccountEntitlementSinglePropertyFilter>) dictionary.Values)
        {
          switch (singlePropertyFilter)
          {
            case AccountEntitlementLicensesFilter _:
              entitlementFilter.Licenses = (IList<LicenseFilter>) ((AccountEntitlementLicensesFilter) singlePropertyFilter).Value.ToList<LicenseFilter>();
              continue;
            case AccountEntitlementNameFilter _:
              entitlementFilter.NameOrEmail = ((AccountEntitlementNameFilter) singlePropertyFilter).Value;
              continue;
            case AccountEntitlementAssignmentSourceFilter _:
              entitlementFilter.AssignmentSources = (IList<AssignmentSource>) ((AccountEntitlementAssignmentSourceFilter) singlePropertyFilter).Value.ToList<AssignmentSource>();
              continue;
            case AccountEntitlementUserTypeFilter _:
              entitlementFilter.UserTypes = (IList<IdentityMetaType>) ((AccountEntitlementUserTypeFilter) singlePropertyFilter).Value.ToList<IdentityMetaType>();
              continue;
            default:
              continue;
          }
        }
        return entitlementFilter;
      }
      catch (Exception ex)
      {
        if (!(ex is InvalidQueryStringException))
          throw new InvalidQueryStringException(LicensingResources.InvalidFilterQuery((object) StringUtil.Truncate(filterQueryString, 100, true)), ex);
        throw;
      }
    }
  }
}
