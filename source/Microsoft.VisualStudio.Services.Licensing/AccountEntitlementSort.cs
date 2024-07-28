// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.AccountEntitlementSort
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class AccountEntitlementSort
  {
    private static readonly ReadOnlyCollection<string> validSortColumns = new ReadOnlyCollection<string>((IList<string>) new List<string>()
    {
      "name",
      "lastAccessed",
      "dateCreated"
    });

    public string SortColumn { get; set; } = string.Empty;

    public SortOrder SortOrder { get; set; }

    public bool IsNotSorted() => string.IsNullOrEmpty(this.SortColumn);

    public static AccountEntitlementSort Parse(string orderByQueryString)
    {
      if (orderByQueryString == null)
        return (AccountEntitlementSort) null;
      try
      {
        string[] groups = orderByQueryString.Split(new string[0], StringSplitOptions.RemoveEmptyEntries);
        if (((IEnumerable<string>) groups).Count<string>() == 2 || ((IEnumerable<string>) groups).Count<string>() == 1)
        {
          string str = AccountEntitlementSort.validSortColumns.FirstOrDefault<string>((Func<string, bool>) (value => value.Equals(groups[0], StringComparison.OrdinalIgnoreCase)));
          if (str == null)
            throw new InvalidQueryStringException(LicensingResources.InvalidOrderByProperty((object) StringUtil.Truncate(groups[0], 100, true)));
          SortOrder sortOrder = SortOrder.Ascending;
          if (!string.IsNullOrWhiteSpace(str))
          {
            if (((IEnumerable<string>) groups).Count<string>() == 2)
            {
              if ("asc".Equals(groups[1], StringComparison.OrdinalIgnoreCase) || "ascending".Equals(groups[1], StringComparison.OrdinalIgnoreCase))
              {
                sortOrder = SortOrder.Ascending;
              }
              else
              {
                if (!"desc".Equals(groups[1], StringComparison.OrdinalIgnoreCase) && !"descending".Equals(groups[1], StringComparison.OrdinalIgnoreCase))
                  throw new InvalidQueryStringException(LicensingResources.InvalidOrderByQuery((object) StringUtil.Truncate(orderByQueryString, 100, true)));
                sortOrder = SortOrder.Descending;
              }
            }
            return new AccountEntitlementSort()
            {
              SortColumn = str,
              SortOrder = sortOrder
            };
          }
        }
        throw new InvalidQueryStringException(LicensingResources.InvalidOrderByQuery((object) StringUtil.Truncate(orderByQueryString, 100, true)));
      }
      catch (Exception ex)
      {
        if (!(ex is InvalidQueryStringException))
          throw new InvalidQueryStringException(LicensingResources.InvalidOrderByQuery((object) StringUtil.Truncate(orderByQueryString, 100, true)), ex);
        throw;
      }
    }

    public string ToQueryString()
    {
      if (string.IsNullOrEmpty(this.SortColumn))
        return string.Empty;
      return this.SortOrder == SortOrder.Descending ? string.Format("{0} desc", (object) this.SortColumn) : this.SortColumn;
    }
  }
}
