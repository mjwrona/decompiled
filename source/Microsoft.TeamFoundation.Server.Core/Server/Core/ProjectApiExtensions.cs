// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectApiExtensions
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.GraphProfile.WebApi;
using Microsoft.TeamFoundation.Server.Core.CollectionManagement.WebServer;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class ProjectApiExtensions
  {
    private const string c_projectAdminsGroupCacheKey = "ProjectAdminsGroup";

    public static TeamProjectReference ToTeamProjectReference(
      this ProjectInfo projectInfo,
      IVssRequestContext requestContext,
      bool includeDefaultTeamUrl = false)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(projectInfo, nameof (projectInfo));
      TeamProjectReference projectReference = new TeamProjectReference()
      {
        Id = projectInfo.Id,
        Abbreviation = projectInfo.Abbreviation,
        Name = projectInfo.Name,
        Url = ProjectApiExtensions.GetProjectResourceUrl(requestContext, projectInfo.Id),
        State = projectInfo.State,
        Description = projectInfo.Description,
        Revision = projectInfo.Revision,
        Visibility = projectInfo.Visibility,
        LastUpdateTime = projectInfo.LastUpdateTime
      };
      if (includeDefaultTeamUrl)
        projectReference.AddDefaultTeamImageUrl(requestContext, projectInfo.Properties);
      return projectReference;
    }

    public static TeamProject ToTeamProject(
      this ProjectInfo projectInfo,
      IVssRequestContext requestContext,
      bool includeCapabilities,
      bool includeCollection = false,
      bool includeDefaultTeam = false)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      TeamProject project = new TeamProject(projectInfo.ToTeamProjectReference(requestContext));
      project.TfsUri = projectInfo.Uri;
      if (includeCollection)
      {
        TeamProjectCollection parentCollection = ProjectApiExtensions.GetParentCollection(requestContext);
        project.Links.AddLink("collection", parentCollection.Url, (ISecuredObject) project);
        string href = UriUtility.Combine(((ReferenceLink) parentCollection.Links.Links["web"]).Href, projectInfo.Name, false).ToString();
        project.Links.AddLink("web", href, (ISecuredObject) project);
      }
      if (includeDefaultTeam)
        project.DefaultTeam = ProjectApiExtensions.GetDefaultTeam(requestContext, project);
      if (includeCapabilities)
        project.AddCapabilities(requestContext, projectInfo.Properties);
      return project;
    }

    public static WebApiProject ToWebApiProject(
      this TeamProject project,
      IVssRequestContext requestContext)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      WebApiProject webApiProject = new WebApiProject((TeamProjectReference) project);
      webApiProject.Capabilities = project.Capabilities;
      webApiProject.Description = project.Description;
      webApiProject.TfsUri = project.TfsUri;
      webApiProject.Collection = ProjectApiExtensions.GetParentCollection(requestContext).ToWebApiProjectCollectionRef();
      webApiProject.DefaultTeam = ProjectApiExtensions.GetDefaultTeam(requestContext, project);
      return webApiProject;
    }

    public static OperationReference Update(
      IVssRequestContext requestContext,
      Guid projectId,
      string newName,
      string newAbbreviation,
      string newDescription,
      ProjectVisibility newVisibility)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      IProjectService service = requestContext.GetService<IProjectService>();
      ProjectInfo projectToUpdate = ProjectInfo.GetProjectToUpdate(projectId);
      projectToUpdate.Name = newName;
      projectToUpdate.Abbreviation = newAbbreviation;
      projectToUpdate.Description = newDescription;
      projectToUpdate.Visibility = newVisibility;
      ProjectInfo projectInfo = (ProjectInfo) null;
      IVssRequestContext requestContext1 = requestContext;
      ProjectInfo project = projectToUpdate;
      ref ProjectInfo local = ref projectInfo;
      Guid jobId = service.UpdateProject(requestContext1, project, out local);
      return JobOperationsUtility.GetOperationReference(requestContext, jobId);
    }

    public static string GetProjectResourceUrl(IVssRequestContext requestContext, Guid id)
    {
      ServerCoreApiExtensions.VerifyCollectionRequest(requestContext);
      return ServerCoreApiExtensions.GetCoreResourceUriString(requestContext, CoreConstants.ProjectsLocationId, (object) new
      {
        projectId = id
      });
    }

    private static WebApiTeamRef GetDefaultTeam(
      IVssRequestContext requestContext,
      TeamProject project)
    {
      WebApiTeam defaultTeam = requestContext.GetService<ITeamService>().GetDefaultTeam(requestContext, project.Id);
      return defaultTeam != null ? new WebApiTeamRef((WebApiTeamRef) defaultTeam, (ISecuredObject) project) : (WebApiTeamRef) null;
    }

    private static TeamProjectCollection GetParentCollection(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      return vssRequestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(vssRequestContext, requestContext.ServiceHost.InstanceId, ServiceHostFilterFlags.None).ToTeamProjectCollection(requestContext);
    }

    private static void AddCapabilities(
      this TeamProject project,
      IVssRequestContext requestContext,
      IList<ProjectProperty> properties)
    {
      project.Capabilities = new Dictionary<string, Dictionary<string, string>>();
      ProjectProperty projectProperty1 = properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (p => string.Equals(p.Name, ProjectApiConstants.ProcessTemplateIdProjectPropertyName, StringComparison.OrdinalIgnoreCase)));
      ProjectProperty projectProperty2 = properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (p => string.Equals(p.Name, ProjectApiConstants.ProcessTemplateNameProjectPropertyName, StringComparison.OrdinalIgnoreCase)));
      ProjectProperty projectProperty3 = properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (p => string.Equals(p.Name, "System.SourceControlCapabilityFlags", StringComparison.OrdinalIgnoreCase)));
      ProjectProperty projectProperty4 = properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (p => string.Equals(p.Name, "System.SourceControlGitEnabled", StringComparison.OrdinalIgnoreCase)));
      ProjectProperty projectProperty5 = properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (p => string.Equals(p.Name, "System.SourceControlTfvcEnabled", StringComparison.OrdinalIgnoreCase)));
      if (projectProperty1 != null)
      {
        Guid result;
        ProcessDescriptor descriptor;
        if (Guid.TryParse((string) projectProperty1.Value, out result) && requestContext.GetService<ITeamFoundationProcessService>().TryGetSpecificProcessDescriptor(requestContext, result, out descriptor))
          project.Capabilities.Add(TeamProjectCapabilitiesConstants.ProcessTemplateCapabilityName, new Dictionary<string, string>()
          {
            {
              TeamProjectCapabilitiesConstants.ProcessTemplateCapabilityTemplateNameAttributeName,
              descriptor.Name
            },
            {
              TeamProjectCapabilitiesConstants.ProcessTemplateCapabilityTemplateTypeIdAttributeName,
              descriptor.TypeId.ToString("D")
            }
          });
      }
      else if (projectProperty2 != null && !string.IsNullOrEmpty((string) projectProperty2.Value))
        project.Capabilities.Add(TeamProjectCapabilitiesConstants.ProcessTemplateCapabilityName, new Dictionary<string, string>()
        {
          {
            TeamProjectCapabilitiesConstants.ProcessTemplateCapabilityTemplateNameAttributeName,
            (string) projectProperty2.Value
          }
        });
      SourceControlTypes result1;
      if (projectProperty3 == null || !Enum.TryParse<SourceControlTypes>((string) projectProperty3.Value, out result1))
        result1 = SourceControlTypes.Tfvc;
      string str1 = projectProperty4 == null ? bool.FalseString : (string) projectProperty4.Value;
      string str2 = projectProperty5 == null ? bool.FalseString : (string) projectProperty5.Value;
      project.Capabilities.Add(TeamProjectCapabilitiesConstants.VersionControlCapabilityName, new Dictionary<string, string>()
      {
        {
          TeamProjectCapabilitiesConstants.VersionControlCapabilityAttributeName,
          result1.ToString()
        },
        {
          TeamProjectCapabilitiesConstants.VersionControlGitEnabledAttributeName,
          str1
        },
        {
          TeamProjectCapabilitiesConstants.VersionControlTfvcEnabledAttributeName,
          str2
        }
      });
    }

    private static void AddDefaultTeamImageUrl(
      this TeamProjectReference projectReference,
      IVssRequestContext requestContext,
      IList<ProjectProperty> properties)
    {
      ProjectProperty projectProperty = properties != null ? properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (p => string.Equals(p.Name, TeamConstants.DefaultTeamPropertyName, StringComparison.OrdinalIgnoreCase))) : (ProjectProperty) null;
      Guid result;
      if (projectProperty == null || !Guid.TryParse(projectProperty.Value.ToString(), out result))
        return;
      Uri uri = requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "GraphProfile", GraphProfileResourceIds.MemberAvatars.MemberAvatarsLocationId, (object) new
      {
        memberDescriptor = result
      }).AppendQuery("overrideDisplayName", projectReference.Name).AppendQuery("size", GraphMemberAvatarSize.Large.ToString("d"));
      projectReference.DefaultTeamImageUrl = uri.AbsoluteUri;
    }

    public static Microsoft.VisualStudio.Services.Identity.Identity GetProjectAdministratorsGroup(
      this IVssRequestContext requestContext,
      string projectUri)
    {
      string scopedGroupCacheKey = ProjectsUtility.GetProjectScopedGroupCacheKey(projectUri, "ProjectAdminsGroup");
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity;
      if (!requestContext.TryGetItem<Microsoft.VisualStudio.Services.Identity.Identity>(scopedGroupCacheKey, out readIdentity))
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        Guid id = service.GetScope(requestContext, ProjectInfo.GetProjectId(projectUri)).Id;
        readIdentity = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          IdentityDomain.MapFromWellKnownIdentifier(id, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup)
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
        requestContext.Items[scopedGroupCacheKey] = (object) readIdentity;
      }
      return readIdentity;
    }
  }
}
