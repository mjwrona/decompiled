// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.DeliveryViewFilterBuilder
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public static class DeliveryViewFilterBuilder
  {
    internal static DeliveryViewFilter Create(
      DeliveryViewPropertyCollection properties,
      int revision,
      DateTime? startDate = null,
      DateTime? endDate = null,
      IEnumerable<string> additionalFields = null)
    {
      ArgumentUtility.CheckForNull<DeliveryViewPropertyCollection>(properties, nameof (properties));
      DeliveryViewFilterBuilder.EnsureDateRangeAreValid(startDate, endDate);
      List<List<FilterClause>> filterClauseListList = DeliveryViewFilterBuilder.RetrieveFilterClauses(properties);
      DeliveryViewFilter deliveryViewFilter = new DeliveryViewFilter();
      deliveryViewFilter.ExpandedTeamBacklogMappings = (IReadOnlyList<TeamBacklogMapping>) properties.TeamBacklogMappings.AsEmptyIfNull<TeamBacklogMapping>().ToList<TeamBacklogMapping>();
      deliveryViewFilter.UnExpandedTeamBacklogMappings = (IReadOnlyList<TeamBacklogMapping>) new List<TeamBacklogMapping>();
      DateTime dateTime1;
      DateTime dateTime2;
      if (!startDate.HasValue)
      {
        dateTime1 = DateTime.Now;
        dateTime1 = dateTime1.Date;
        dateTime2 = dateTime1.AddDays(-14.0);
      }
      else
      {
        dateTime1 = startDate.Value;
        dateTime2 = dateTime1.Date;
      }
      deliveryViewFilter.StartDate = dateTime2;
      DateTime dateTime3;
      if (!endDate.HasValue)
      {
        dateTime1 = DateTime.Now;
        dateTime1 = dateTime1.Date;
        dateTime3 = dateTime1.AddDays(49.0);
      }
      else
      {
        dateTime1 = endDate.Value;
        dateTime3 = dateTime1.Date;
      }
      deliveryViewFilter.EndDate = dateTime3;
      deliveryViewFilter.AdditionalFields = (IEnumerable<string>) additionalFields.AsEmptyIfNull<string>().ToList<string>();
      deliveryViewFilter.Criteria = (IReadOnlyList<FilterClause>) properties.Criteria.AsEmptyIfNull<FilterClause>().ToList<FilterClause>();
      deliveryViewFilter.Revision = revision;
      deliveryViewFilter.FilterClauses = (IReadOnlyList<List<FilterClause>>) filterClauseListList;
      return deliveryViewFilter;
    }

    private static List<List<FilterClause>> RetrieveFilterClauses(
      DeliveryViewPropertyCollection properties)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Rule>>(properties.CardStyleSettings, "CardStyleSettings");
      List<List<FilterClause>> filterClauseListList = new List<List<FilterClause>>();
      foreach (Rule cardStyleSetting in properties.CardStyleSettings)
        filterClauseListList.Add(cardStyleSetting.Clauses.ToList<FilterClause>());
      return filterClauseListList;
    }

    internal static void EnsureDateRangeAreValid(DateTime? startDate, DateTime? endDate)
    {
      if (startDate.HasValue ^ endDate.HasValue)
        throw new ArgumentException(Resources.MissingTimelineDateMessage());
      if (startDate.HasValue && (startDate.Value <= (DateTime) SqlDateTime.MinValue || startDate.Value >= (DateTime) SqlDateTime.MaxValue))
        throw new ArgumentOutOfRangeException(nameof (startDate));
      if (endDate.HasValue && (endDate.Value <= (DateTime) SqlDateTime.MinValue || endDate.Value >= (DateTime) SqlDateTime.MaxValue))
        throw new ArgumentOutOfRangeException(nameof (endDate));
    }
  }
}
