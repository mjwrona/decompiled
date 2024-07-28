// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser.AccountEntitlementFilterQuerySyntacticTreeVisitor
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser
{
  internal class AccountEntitlementFilterQuerySyntacticTreeVisitor : 
    AccountEntitlementFilterQuerySyntacticTreeVisitorBase<IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>>
  {
    private static readonly AccountEntitlementSinglePropertyFilterQuerySyntacticTreeVisitor singlePropertyVisitor = new AccountEntitlementSinglePropertyFilterQuerySyntacticTreeVisitor();

    public override IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter> Visit(
      BinaryOperatorToken tokenIn)
    {
      AccountEntitlementSinglePropertyFilter singlePropertyFilter = tokenIn.Accept<AccountEntitlementSinglePropertyFilter>((ISyntacticTreeVisitor<AccountEntitlementSinglePropertyFilter>) AccountEntitlementFilterQuerySyntacticTreeVisitor.singlePropertyVisitor);
      if (singlePropertyFilter != null)
        return (IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>) new Dictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>()
        {
          {
            singlePropertyFilter.PropertyType,
            singlePropertyFilter
          }
        };
      if (tokenIn.OperatorKind != 1)
        return (IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>) null;
      IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter> first = tokenIn.Left.Accept<IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>>((ISyntacticTreeVisitor<IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>>) this);
      IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter> second = tokenIn.Right.Accept<IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>>((ISyntacticTreeVisitor<IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>>) this);
      if (first == null || second == null)
        return (IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>) null;
      return first.Keys.Intersect<AccountEntitlementFilterSinglePropertyType>((IEnumerable<AccountEntitlementFilterSinglePropertyType>) second.Keys).Count<AccountEntitlementFilterSinglePropertyType>() > 0 ? (IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>) null : (IDictionary<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>) first.Union<KeyValuePair<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>>((IEnumerable<KeyValuePair<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>>) second).ToDictionary<KeyValuePair<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>, AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>((Func<KeyValuePair<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>, AccountEntitlementFilterSinglePropertyType>) (p => p.Key), (Func<KeyValuePair<AccountEntitlementFilterSinglePropertyType, AccountEntitlementSinglePropertyFilter>, AccountEntitlementSinglePropertyFilter>) (p => p.Value));
    }
  }
}
