// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.TeamsViewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class TeamsViewDataProvider : IExtensionDataProvider
  {
    private const string ClassificationNodeDuplicateNameException = "VS402371";

    public string Name => "Admin.TeamsView";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      if (providerContext.Properties.ContainsKey("setDefaultTeam") && providerContext.Properties["setDefaultTeam"] != null && bool.Parse(providerContext.Properties["setDefaultTeam"].ToString()))
        return (object) this.updateDefaultTeam(requestContext, providerContext);
      if (providerContext.Properties.ContainsKey("setTeamAdmins") && providerContext.Properties["setTeamAdmins"] != null && bool.Parse(providerContext.Properties["setTeamAdmins"].ToString()))
        return (object) this.addTeamAdmins(requestContext, providerContext);
      if (providerContext.Properties.ContainsKey("setAreaPath") && providerContext.Properties["setAreaPath"] != null && bool.Parse(providerContext.Properties["setAreaPath"].ToString()))
        return (object) this.createAreaPath(requestContext, providerContext);
      if (providerContext.Properties.ContainsKey("removeAdmins") && providerContext.Properties["removeAdmins"] != null && bool.Parse(providerContext.Properties["removeAdmins"].ToString()))
        return (object) this.removeAdministrators(requestContext, providerContext);
      if (providerContext.Properties.ContainsKey("getTeam") && providerContext.Properties["getTeam"] != null && bool.Parse(providerContext.Properties["getTeam"].ToString()))
        return this.getTeamInProject(requestContext, providerContext);
      return providerContext.Properties.ContainsKey("isDefaultTeam") && providerContext.Properties["isDefaultTeam"] != null && bool.Parse(providerContext.Properties["isDefaultTeam"].ToString()) ? this.isDefaultTeam(requestContext, providerContext) : (object) null;
    }

    private HttpStatusCodeResult updateDefaultTeam(
      IVssRequestContext requestContext,
      DataProviderContext providerContext)
    {
      Guid teamId = Guid.Empty;
      if (providerContext.Properties.ContainsKey("teamId") && providerContext.Properties["teamId"] != null)
        teamId = new Guid(providerContext.Properties["teamId"].ToString());
      try
      {
        if (teamId != Guid.Empty)
        {
          ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
          requestContext.GetService<ITeamService>().SetDefaultTeamId(requestContext, project.Id, teamId);
        }
        return new HttpStatusCodeResult(HttpStatusCode.NoContent);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050075, TraceLevel.Error, "TeamsPivot", "DataProvider", ex);
        return (HttpStatusCodeResult) null;
      }
    }

    private HttpStatusCodeResult addTeamAdmins(
      IVssRequestContext requestContext,
      DataProviderContext providerContext)
    {
      try
      {
        string subjectDescriptorString1 = "";
        if (providerContext.Properties.ContainsKey("teamDescriptor") && providerContext.Properties["teamDescriptor"] != null)
          subjectDescriptorString1 = providerContext.Properties["teamDescriptor"].ToString();
        if (subjectDescriptorString1 != null && !subjectDescriptorString1.Equals(""))
        {
          IdentityDescriptor identityDescriptor1 = SubjectDescriptor.FromString(subjectDescriptorString1).ToIdentityDescriptor(requestContext);
          object source;
          providerContext.Properties.TryGetValue("admins", out source);
          if (source != null)
          {
            string[] array = ((IEnumerable) source).Cast<object>().Select<object, string>((Func<object, string>) (x => x.ToString())).ToArray<string>();
            TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
            foreach (string subjectDescriptorString2 in array)
            {
              IdentityDescriptor identityDescriptor2 = SubjectDescriptor.FromString(subjectDescriptorString2).ToIdentityDescriptor(requestContext);
              service.AddGroupAdministrator(requestContext, identityDescriptor1, identityDescriptor2);
            }
          }
          return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
        requestContext.Trace(10050082, TraceLevel.Error, "TeamsPivot", "DataProvider", "teamDescriptor has a null value or is an empty string");
        return (HttpStatusCodeResult) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050082, TraceLevel.Error, "TeamsPivot", "DataProvider", ex);
        return (HttpStatusCodeResult) null;
      }
    }

    private HttpStatusCodeResult createAreaPath(
      IVssRequestContext requestContext,
      DataProviderContext providerContext)
    {
      try
      {
        string subjectDescriptorString = "";
        if (providerContext.Properties.ContainsKey("teamDescriptor") && providerContext.Properties["teamDescriptor"] != null)
          subjectDescriptorString = providerContext.Properties["teamDescriptor"].ToString();
        if (subjectDescriptorString != null && !subjectDescriptorString.Equals(""))
        {
          IdentityDescriptor identityDescriptor1 = SubjectDescriptor.FromString(subjectDescriptorString).ToIdentityDescriptor(requestContext);
          TfsWebContext webContext = (TfsWebContext) WebContextFactory.GetWebContext(requestContext);
          WebApiTeam identityDescriptor2 = requestContext.GetService<ITeamService>().GetTeamByIdentityDescriptor(requestContext, identityDescriptor1);
          TeamConfigurationHelper.SetDefaultSettings(requestContext, webContext.Project, identityDescriptor2, TeamAreaAction.CreateNew);
          return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
        requestContext.Trace(10050083, TraceLevel.Error, "TeamsPivot", "DataProvider", "teamDescriptor has a null value or is an empty string");
        return (HttpStatusCodeResult) null;
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("VS402371"))
          return new HttpStatusCodeResult(HttpStatusCode.PreconditionFailed, ex.Message);
        requestContext.TraceException(10050083, TraceLevel.Error, "TeamsPivot", "DataProvider", ex);
        return (HttpStatusCodeResult) null;
      }
    }

    private HttpStatusCodeResult removeAdministrators(
      IVssRequestContext requestContext,
      DataProviderContext providerContext)
    {
      try
      {
        string subjectDescriptorString1 = "";
        string subjectDescriptorString2 = "";
        if (providerContext.Properties.ContainsKey("teamDescriptor") && providerContext.Properties["teamDescriptor"] != null)
          subjectDescriptorString1 = providerContext.Properties["teamDescriptor"].ToString();
        if (providerContext.Properties.ContainsKey("adminDescriptor") && providerContext.Properties["adminDescriptor"] != null)
          subjectDescriptorString2 = providerContext.Properties["adminDescriptor"].ToString();
        if (subjectDescriptorString1 != null && !subjectDescriptorString1.Equals("") && subjectDescriptorString2 != null && !subjectDescriptorString2.Equals(""))
        {
          IdentityDescriptor identityDescriptor1 = SubjectDescriptor.FromString(subjectDescriptorString1).ToIdentityDescriptor(requestContext);
          IdentityDescriptor identityDescriptor2 = SubjectDescriptor.FromString(subjectDescriptorString2).ToIdentityDescriptor(requestContext);
          requestContext.GetService<TeamFoundationIdentityService>().RemoveGroupAdministrator(requestContext, identityDescriptor1, identityDescriptor2);
          return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
        requestContext.Trace(10050091, TraceLevel.Error, "TeamsPivot", "DataProvider", "teamDescriptor or adminDescriptor has a null value or is an empty string");
        return (HttpStatusCodeResult) null;
      }
      catch (Exception ex)
      {
        requestContext.Trace(10050091, TraceLevel.Error, "TeamsPivot", "DataProvider", ex.Message);
        return (HttpStatusCodeResult) null;
      }
    }

    private object getTeamInProject(
      IVssRequestContext requestContext,
      DataProviderContext providerContext)
    {
      try
      {
        string teamIdOrName = "";
        if (providerContext.Properties.ContainsKey("teamId") && providerContext.Properties["teamId"] != null)
          teamIdOrName = providerContext.Properties["teamId"].ToString();
        if (teamIdOrName != null && !teamIdOrName.Equals(""))
        {
          ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
          return (object) requestContext.GetService<ITeamService>().GetTeamInProject(requestContext, project.Id, teamIdOrName);
        }
        requestContext.Trace(10050092, TraceLevel.Error, "TeamsPivot", "DataProvider", "teamId has a null value or is an empty string");
        return (object) null;
      }
      catch (Exception ex)
      {
        requestContext.Trace(10050092, TraceLevel.Error, "TeamsPivot", "DataProvider", ex.Message);
        return (object) null;
      }
    }

    private object isDefaultTeam(
      IVssRequestContext requestContext,
      DataProviderContext providerContext)
    {
      try
      {
        string str = "";
        if (providerContext.Properties.ContainsKey("teamId") && providerContext.Properties["teamId"] != null)
          str = providerContext.Properties["teamId"].ToString();
        if (!string.IsNullOrWhiteSpace(str))
        {
          ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
          Guid defaultTeamId = requestContext.GetService<ITeamService>().GetDefaultTeamId(requestContext, project.Id);
          return (object) (str == defaultTeamId.ToString());
        }
        requestContext.Trace(10050098, TraceLevel.Error, "TeamsPivot", "DataProvider", "teamId has a null value or is an empty string");
        return (object) false;
      }
      catch (Exception ex)
      {
        requestContext.Trace(10050098, TraceLevel.Error, "TeamsPivot", "DataProvider", ex.Message);
        return (object) true;
      }
    }
  }
}
