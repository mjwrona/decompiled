// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamCapacity
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class TeamCapacity
  {
    private const string c_teamCapacity = "TeamCapacity";
    private const string c_teamDaysOffDates = "TeamDaysOffDates";
    private const string c_teamMembers = "TeamMemberCapacityCollection";
    private AgileTracingUtils m_tracingUtils = new AgileTracingUtils();

    public TeamCapacity()
    {
      this.TeamMemberCapacityCollection = (IReadOnlyCollection<TeamMemberCapacity>) new List<TeamMemberCapacity>();
      this.TeamDaysOffDates = new List<DateRange>();
    }

    public List<DateRange> TeamDaysOffDates { get; set; }

    public IReadOnlyCollection<TeamMemberCapacity> TeamMemberCapacityCollection { get; set; }

    public bool IsEmpty => !this.TeamDaysOffDates.Any<DateRange>() && !this.TeamMemberCapacityCollection.Where<TeamMemberCapacity>((Func<TeamMemberCapacity, bool>) (m => !m.IsEmpty)).Any<TeamMemberCapacity>();

    public JsObject ToJson(IVssRequestContext requestContext, Guid iterationId)
    {
      using (this.m_tracingUtils.TraceBlock(requestContext, 910701, 910702, memberName: nameof (ToJson)))
      {
        ConversionHelper conversionHelper = new ConversionHelper();
        JsObject json = new JsObject();
        json.Add("TeamDaysOffDates", this.TeamDaysOffDates != null ? (object) conversionHelper.ToJson(conversionHelper.ConvertToWebApiDateRange((IEnumerable<DateRange>) this.TeamDaysOffDates)) : (object) new JsObject[0]);
        json.Add("TeamMemberCapacityCollection", this.TeamMemberCapacityCollection != null ? (object) conversionHelper.ToJson(conversionHelper.ConvertToTeamMemberCapacityReferences(requestContext, iterationId, (IEnumerable<TeamMemberCapacity>) this.TeamMemberCapacityCollection).Where<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity>((Func<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity, bool>) (x => !string.IsNullOrEmpty(x.TeamMember.DisplayName)))) : (object) new JsObject[0]);
        return json;
      }
    }

    public void ResolveIdentities(IVssRequestContext tfsRequestContext)
    {
      using (this.m_tracingUtils.TraceBlock(tfsRequestContext, 290014, 290015, memberName: nameof (ResolveIdentities)))
      {
        IEnumerable<Guid> source = this.TeamMemberCapacityCollection.Select<TeamMemberCapacity, Guid>((Func<TeamMemberCapacity, Guid>) (tm => tm.TeamMemberId));
        foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) tfsRequestContext.GetService<IdentityService>().ReadIdentities(tfsRequestContext, (IList<Guid>) source.ToList<Guid>(), QueryMembership.None, (IEnumerable<string>) null))
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = readIdentity;
          if (identity != null)
          {
            string distinctDisplayName = identity.GetLegacyDistinctDisplayName();
            if (!string.IsNullOrEmpty(distinctDisplayName))
            {
              TeamMemberCapacity teamMemberCapacity = this.TeamMemberCapacityCollection.FirstOrDefault<TeamMemberCapacity>((Func<TeamMemberCapacity, bool>) (tm => tm.TeamMemberId == identity.Id));
              if (teamMemberCapacity != null)
                teamMemberCapacity.DisplayName = distinctDisplayName;
            }
          }
        }
      }
    }

    public void Validate()
    {
      ArgumentUtility.CheckForNull<List<DateRange>>(this.TeamDaysOffDates, "TeamDaysOffDates");
      ArgumentUtility.CheckForNull<IReadOnlyCollection<TeamMemberCapacity>>(this.TeamMemberCapacityCollection, "TeamMemberCapacityCollection");
      HashSet<Guid> guidSet = new HashSet<Guid>();
      foreach (TeamMemberCapacity teamMemberCapacity in (IEnumerable<TeamMemberCapacity>) this.TeamMemberCapacityCollection)
      {
        ArgumentUtility.CheckForNull<TeamMemberCapacity>(teamMemberCapacity, "memberCapacity");
        teamMemberCapacity.Validate();
        if (guidSet.Contains(teamMemberCapacity.TeamMemberId))
          throw new ArgumentException("memberCapacity");
        guidSet.Add(teamMemberCapacity.TeamMemberId);
      }
      foreach (DateRange teamDaysOffDate in this.TeamDaysOffDates)
      {
        ArgumentUtility.CheckForNull<DateRange>(teamDaysOffDate, "TeamDaysOffDates");
        teamDaysOffDate.Validate();
      }
      DateRange.CheckForOverlaps((ICollection<DateRange>) this.TeamDaysOffDates, "TeamDaysOffDates");
    }

    public int CalculateWorkingDaysOfIteration(
      DateTime startDate,
      DateTime endDate,
      DateTime currentDate,
      ITeamSettings teamSettings)
    {
      ArgumentUtility.CheckForNull<ITeamSettings>(teamSettings, nameof (teamSettings));
      int workingDaysOfIteration = 0;
      if (currentDate >= startDate && currentDate <= endDate)
        startDate = currentDate;
      if (endDate > DateTime.MaxValue.AddDays(-1.0))
        endDate = endDate.AddDays(-1.0);
      for (DateTime iterateDate = startDate; iterateDate <= endDate; iterateDate = iterateDate.AddDays(1.0))
      {
        if (!((IEnumerable<DayOfWeek>) teamSettings.Weekends.Days).Any<DayOfWeek>((Func<DayOfWeek, bool>) (day => day == iterateDate.DayOfWeek)) && !this.TeamDaysOffDates.Any<DateRange>((Func<DateRange, bool>) (dayOffRange => iterateDate >= dayOffRange.Start && iterateDate <= dayOffRange.End)))
          ++workingDaysOfIteration;
      }
      return workingDaysOfIteration;
    }
  }
}
