// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices.TeamConfigurationService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.WebServices;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.WebServices
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/TeamConfiguration/01", Description = "DevOps Team Configuration web service")]
  [ClientService(ServerConfiguration = ServerConfiguration.TfsTeamProjectCollection, ComponentName = "vstfs", ServiceName = "TeamConfigurationService", CollectionServiceIdentifier = "56BAA505-9D62-4E68-B64C-B88697DC5322")]
  public class TeamConfigurationService : TeamFoundationWebService
  {
    private const string s_area = "Agile";
    private const string s_layer = "TeamConfigurationService";

    [WebMethod]
    public TeamConfiguration[] GetTeamConfigurations([AllowEmptyArray] Guid[] teamIds)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetTeamConfigurations), MethodType.Admin, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<Guid>(nameof (teamIds), (IList<Guid>) teamIds);
        this.EnterMethod(methodInformation);
        using (this.RequestContext.TraceBlock(220331, 220339, "Agile", nameof (TeamConfigurationService), nameof (GetTeamConfigurations)))
        {
          try
          {
            return new TeamConfigurationWebServiceUtil().GetTeamConfigurations(this.RequestContext, teamIds);
          }
          catch (Exception ex)
          {
            throw this.HandleException(ex);
          }
        }
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public TeamConfiguration[] GetTeamConfigurationsForUser([AllowEmptyArray] string[] projectUris)
    {
      MethodInformation methodInformation = new MethodInformation(nameof (GetTeamConfigurationsForUser), MethodType.Admin, EstimatedMethodCost.Low);
      methodInformation.AddArrayParameter<string>(nameof (projectUris), (IList<string>) projectUris);
      this.EnterMethod(methodInformation);
      try
      {
        using (this.RequestContext.TraceBlock(220301, 220309, "Agile", nameof (TeamConfigurationService), nameof (GetTeamConfigurationsForUser)))
        {
          bool flag = false;
          try
          {
            ArgumentUtility.CheckForNull<string[]>(projectUris, nameof (projectUris));
            flag = true;
            return this.RequestContext.IsFeatureEnabled("TeamConfigurationService.GetTeamConfigurationsForUser.Safeguard.BypassAll") ? Array.Empty<TeamConfiguration>() : new TeamConfigurationWebServiceUtil().GetTeamConfigurationsForUser(this.RequestContext, projectUris);
          }
          catch (ArgumentException ex)
          {
            if (flag)
            {
              this.RequestContext.Trace(220305, TraceLevel.Info, "Agile", nameof (TeamConfigurationService), string.Join(";", projectUris));
              this.RequestContext.TraceException(220306, "Agile", nameof (TeamConfigurationService), (Exception) ex);
              if (this.RequestContext.IsFeatureEnabled("TeamConfigurationService.GetTeamConfigurationsForUser.Safeguard.BypassOnException"))
                return Array.Empty<TeamConfiguration>();
            }
            throw this.HandleException((Exception) ex);
          }
          catch (Exception ex)
          {
            this.RequestContext.TraceException(220306, "Agile", nameof (TeamConfigurationService), ex);
            if (this.RequestContext.IsFeatureEnabled("TeamConfigurationService.GetTeamConfigurationsForUser.Safeguard.BypassOnException"))
              return Array.Empty<TeamConfiguration>();
            throw this.HandleException(ex);
          }
        }
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    public void SetTeamSettings(Guid teamId, TeamSettings teamSettings)
    {
      MethodInformation methodInformation = new MethodInformation(nameof (SetTeamSettings), MethodType.Admin, EstimatedMethodCost.Low);
      methodInformation.AddParameter(nameof (teamId), (object) teamId);
      methodInformation.AddParameter(nameof (teamSettings), (object) teamSettings);
      this.EnterMethod(methodInformation);
      try
      {
        using (this.RequestContext.TraceBlock(220311, 220319, "Agile", nameof (TeamConfigurationService), nameof (SetTeamSettings)))
        {
          try
          {
            new TeamConfigurationWebServiceUtil().SetTeamSettings(this.RequestContext, teamId, teamSettings);
          }
          catch (Exception ex)
          {
            throw this.HandleException(ex);
          }
        }
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
