// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.TeamFoundationProjectUserRecentActivityComponent2
// Assembly: Microsoft.Azure.Boards.RecentActivity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 684DCFA4-4764-4794-94A6-960AF811434C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.RecentActivity.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.Azure.Boards.RecentActivity
{
  internal class TeamFoundationProjectUserRecentActivityComponent2 : 
    TeamFoundationProjectUserRecentActivityComponent
  {
    public override void CleanupRecentProjectUserActivity(
      IVssRequestContext requestContext,
      int maxActivitiesPerUser,
      int maxDaysPerActivity)
    {
      this.PrepareStoredProcedure("prc_CleanupRecentProjectUserActivities");
      this.BindInt("@maxActivitiesPerUser", maxActivitiesPerUser);
      this.BindInt("@maxDaysPerActivity", maxDaysPerActivity);
      this.ExecuteNonQuery();
    }
  }
}
