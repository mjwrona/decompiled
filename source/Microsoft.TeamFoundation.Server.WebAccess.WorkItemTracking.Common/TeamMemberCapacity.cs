// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class TeamMemberCapacity
  {
    public TeamMemberCapacity()
    {
      this.DaysOffDates = (IList<DateRange>) new List<DateRange>();
      this.Activities = (IList<Activity>) new List<Activity>();
    }

    public Guid TeamMemberId { get; set; }

    public IList<DateRange> DaysOffDates { get; set; }

    public IList<Activity> Activities { get; set; }

    public string DisplayName { get; set; }

    public bool IsEmpty
    {
      get
      {
        if (this.Activities != null && this.Activities.Any<Activity>())
          return false;
        return this.DaysOffDates == null || !this.DaysOffDates.Any<DateRange>();
      }
    }

    public string GetTeamMemberDisplayName(IVssRequestContext requestContext)
    {
      if (!string.IsNullOrEmpty(this.DisplayName))
        return this.DisplayName;
      requestContext.Trace(290075, TraceLevel.Warning, "Agile", TfsTraceLayers.BusinessLogic, "GetTeamMemberDisplayName: Display name is missing, going to IdentityService. Id = {0}", (object) this.TeamMemberId);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
      {
        this.TeamMemberId
      }, QueryMembership.None, (IEnumerable<string>) null);
      if (identityList[0] == null)
      {
        requestContext.Trace(290005, TraceLevel.Warning, "WebAccess", TfsTraceLayers.BusinessLogic, "Could not resolve identity {0}", (object) this.TeamMemberId);
        return string.Empty;
      }
      this.DisplayName = identityList[0].GetLegacyDistinctDisplayName();
      return this.DisplayName;
    }

    public bool HasData()
    {
      if (this.Activities != null && this.Activities.Count > 0)
        return true;
      return this.DaysOffDates != null && this.DaysOffDates.Count != 0;
    }

    public JsObject ToJson()
    {
      JsObject jsObject = new JsObject();
      jsObject.Add("TeamMemberId", (object) this.TeamMemberId);
      jsObject.Add("Capacity", (object) (float) (this.Activities == null || !this.Activities.Any<Activity>() ? 0.0 : (double) this.Activities.First<Activity>().CapacityPerDay));
      jsObject.Add("DaysOffDates", this.DaysOffDates != null ? (object) this.DaysOffDates.Select<DateRange, JsObject>((Func<DateRange, JsObject>) (d => d.ToJson())) : (object) (IEnumerable<JsObject>) new JsObject[0]);
      jsObject.Add("Activity", this.Activities == null || !this.Activities.Any<Activity>() ? (object) (string) null : (object) this.Activities.First<Activity>().Name);
      JsObject json = jsObject;
      if (!string.IsNullOrEmpty(this.DisplayName))
        json.Add("DisplayName", (object) this.DisplayName);
      return json;
    }

    public void Validate()
    {
      ArgumentUtility.CheckForEmptyGuid(this.TeamMemberId, "TeamMemberId");
      ArgumentUtility.CheckForNull<IList<DateRange>>(this.DaysOffDates, "DaysOffDates");
      foreach (DateRange daysOffDate in (IEnumerable<DateRange>) this.DaysOffDates)
      {
        ArgumentUtility.CheckForNull<DateRange>(daysOffDate, "DaysOffDates");
        daysOffDate.Validate();
      }
      if (this.Activities != null)
      {
        foreach (Activity activity in (IEnumerable<Activity>) this.Activities)
        {
          ArgumentUtility.CheckGreaterThanOrEqualToZero(activity.CapacityPerDay, "Activities.CapacityPerDay");
          ArgumentUtility.CheckValueEqualsToInfinity(activity.CapacityPerDay, "Activities.CapacityPerDay");
        }
        if (this.Activities.GroupBy<Activity, string>((Func<Activity, string>) (x => x.Name)).Any<IGrouping<string, Activity>>((Func<IGrouping<string, Activity>, bool>) (g => g.Count<Activity>() > 1)))
          throw new ArgumentOutOfRangeException("activities");
      }
      DateRange.CheckForOverlaps((ICollection<DateRange>) this.DaysOffDates, "DaysOffDates");
    }
  }
}
