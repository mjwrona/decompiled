// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility.TfsWebContextExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility
{
  public static class TfsWebContextExtensions
  {
    public static IAgileSettings GetTeamAgileSettings(this TfsWebContext tfsWebContext, Guid teamId) => TfsWebContextExtensions.Implementation.Instance.GetTeamAgileSettings(tfsWebContext, teamId);

    public class Implementation
    {
      private static TfsWebContextExtensions.Implementation s_instance;

      protected Implementation()
      {
      }

      public static TfsWebContextExtensions.Implementation Instance
      {
        get
        {
          if (TfsWebContextExtensions.Implementation.s_instance == null)
            TfsWebContextExtensions.Implementation.s_instance = new TfsWebContextExtensions.Implementation();
          return TfsWebContextExtensions.Implementation.s_instance;
        }
        internal set => TfsWebContextExtensions.Implementation.s_instance = value;
      }

      public virtual IAgileSettings GetTeamAgileSettings(TfsWebContext tfsWebContext, Guid teamId)
      {
        ArgumentUtility.CheckForEmptyGuid(teamId, nameof (teamId));
        IVssRequestContext tfsRequestContext = tfsWebContext.TfsRequestContext;
        return (IAgileSettings) new AgileSettings(tfsRequestContext, CommonStructureProjectInfo.ConvertProjectInfo(tfsWebContext.Project), tfsRequestContext.GetService<ITeamService>().GetTeamByGuid(tfsRequestContext, teamId) ?? throw new TeamNotFoundException(teamId.ToString()));
      }
    }
  }
}
