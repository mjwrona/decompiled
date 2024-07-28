// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings.TeamCreationSubscriber
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings
{
  internal class TeamCreationSubscriber : ISubscriber
  {
    private static readonly Type[] s_subscribedTypes = Array.Empty<Type>();

    static TeamCreationSubscriber()
    {
      try
      {
        TeamCreationSubscriber.s_subscribedTypes = new Type[1]
        {
          Assembly.Load("Microsoft.TeamFoundation.Server.Core").GetType("Microsoft.TeamFoundation.Server.Core.TeamFoundationIdentityPropertiesUpdateEvent")
        };
      }
      catch (Exception ex)
      {
      }
    }

    public string Name => "Team Configuration Service: Team Creation Handler";

    public SubscriberPriority Priority => SubscriberPriority.Normal;

    public Type[] SubscribedTypes() => TeamCreationSubscriber.s_subscribedTypes;

    public EventNotificationStatus ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties)
    {
      statusCode = 0;
      statusMessage = (string) null;
      properties = (ExceptionPropertyCollection) null;
      CustomerIntelligenceData properties1 = new CustomerIntelligenceData();
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.WorkItemTracking, "TeamCreationSubscriber.ProcessEvent", properties1);
      try
      {
        requestContext.TraceEnter(240252, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (ProcessEvent));
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return requestContext.GetService<ILegacyTeamCreationSubscriberService>().ProcessEvent(requestContext, notificationType, notificationEventArgs, TeamCreationSubscriber.\u003C\u003EO.\u003C0\u003E__ProcessChangedIdentity ?? (TeamCreationSubscriber.\u003C\u003EO.\u003C0\u003E__ProcessChangedIdentity = new Action<IVssRequestContext, IdentityDescriptor, HashSet<string>>(TeamCreationSubscriber.ProcessChangedIdentity)), out statusCode, out statusMessage, out properties);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290006, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, ex);
      }
      finally
      {
        requestContext.TraceLeave(240253, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (ProcessEvent));
      }
      return EventNotificationStatus.ActionPermitted;
    }

    private static void ProcessChangedIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      HashSet<string> modifiedProperties)
    {
      TeamConfigurationService service1 = requestContext.GetService<TeamConfigurationService>();
      ITeamService service2 = requestContext.GetService<ITeamService>();
      if (modifiedProperties == null || !modifiedProperties.Contains(TeamConstants.TeamPropertyName))
        return;
      WebApiTeam identityDescriptor = service2.GetTeamByIdentityDescriptor(requestContext, descriptor);
      if (identityDescriptor == null)
        return;
      ProjectProcessConfiguration processSettings = requestContext.GetService<IProjectConfigurationService>().GetProcessSettings(requestContext, ProjectInfo.GetProjectUri(identityDescriptor.ProjectId), false);
      service1.SetBugsBehavior(requestContext, identityDescriptor, processSettings.BugsBehavior, false);
      service1.PopulateTeamWeekends(requestContext, identityDescriptor);
    }
  }
}
