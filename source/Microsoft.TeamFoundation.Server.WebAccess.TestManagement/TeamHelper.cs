// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TeamHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class TeamHelper : TestHelperBase
  {
    internal TeamHelper(TestManagerRequestContext testContext)
      : base(testContext)
    {
    }

    internal TeamDaysOffModel GetTeamDaysOff(string iterationId)
    {
      if (this.TestContext.Team == null || string.IsNullOrEmpty(iterationId))
        return new TeamDaysOffModel();
      Guid iterationId1 = new Guid(iterationId);
      ITeamConfigurationService service = this.TestContext.TfsRequestContext.GetService<ITeamConfigurationService>();
      ITeamSettings teamSettings = service.GetTeamSettings(this.TestContext.TfsRequestContext, this.TestContext.Team, false, false);
      TeamCapacity iterationCapacity = service.GetTeamIterationCapacity(this.TestContext.TfsRequestContext, this.TestContext.Team, teamSettings, iterationId1);
      DateTime dateTime1 = TimeZoneInfo.ConvertTime(DateTime.Now, this.TestContext.TfsRequestContext.GetCollectionTimeZone());
      DateTime dateTime2 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, 0, 0, 0, DateTimeKind.Utc);
      return new TeamDaysOffModel()
      {
        TeamDaysOffDates = iterationCapacity.TeamDaysOffDates == null || !iterationCapacity.TeamDaysOffDates.Any<DateRange>() ? new JsObject[0] : iterationCapacity.TeamDaysOffDates.Select<DateRange, JsObject>((Func<DateRange, JsObject>) (x =>
        {
          return new JsObject()
          {
            {
              "start",
              (object) x.Start
            },
            {
              "end",
              (object) x.End
            }
          };
        })).ToArray<JsObject>(),
        Weekends = (teamSettings != null ? ((IEnumerable<DayOfWeek>) teamSettings.Weekends.Days).Select<DayOfWeek, int>((Func<DayOfWeek, int>) (w => (int) w)).ToList<int>() : (List<int>) null) ?? new List<int>(),
        CurrentDate = dateTime2
      };
    }

    internal bool IsDataTierUpdated() => !this.TestContext.TestRequestContext.IsDataTierOld;
  }
}
