// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UserActivityService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class UserActivityService : IVssFrameworkService
  {
    private static readonly int s_activityWindowDays = 28;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.CheckOnPremisesDeployment();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public static int ActivityWindowDays => UserActivityService.s_activityWindowDays;

    public virtual List<UserActivityEntry> GetUserActivity(IVssRequestContext requestContext)
    {
      TeamFoundationHostManagementService service = requestContext.GetService<TeamFoundationHostManagementService>();
      TeamFoundationServiceHostProperties serviceHostProperties = service.QueryServiceHostProperties(requestContext, requestContext.ServiceHost.InstanceId, ServiceHostFilterFlags.IncludeChildren);
      List<UserActivityEntry> userActivity = new List<UserActivityEntry>();
      using (UserActivityComponent component = requestContext.CreateComponent<UserActivityComponent>())
        userActivity.AddRange((IEnumerable<UserActivityEntry>) component.QueryUserActivity(UserActivityService.s_activityWindowDays));
      foreach (TeamFoundationServiceHostProperties child in serviceHostProperties.Children)
      {
        if (child != null && child.Status == TeamFoundationServiceHostStatus.Started)
        {
          using (IVssRequestContext context = service.BeginRequest(requestContext, child.Id, RequestContextType.SystemContext, true, true))
          {
            using (UserActivityComponent component = context.CreateComponent<UserActivityComponent>())
              userActivity.AddRange((IEnumerable<UserActivityEntry>) component.QueryUserActivity(UserActivityService.s_activityWindowDays));
          }
        }
      }
      return userActivity;
    }

    public virtual UserActivity GetActiveUserStatistics(IVssRequestContext requestContext) => new UserActivity(this.GetUserActivity(requestContext));
  }
}
