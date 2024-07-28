// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TeamProjectModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.ProjectResources;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class TeamProjectModel : IComparable<TeamProjectModel>
  {
    public TeamProjectModel(TfsWebContext webContext, string collectionName)
      : this(webContext.TfsRequestContext, webContext.Project, false, collectionName)
    {
    }

    public TeamProjectModel(
      TfsWebContext webContext,
      bool includeTeamMembers,
      string collectionName)
      : this(webContext.TfsRequestContext, webContext.Project, includeTeamMembers, collectionName)
    {
    }

    public TeamProjectModel(
      IVssRequestContext requestContext,
      ProjectInfo project,
      bool includeTeamMembers,
      string collectionName,
      bool includeTeamInformation = true,
      int maxTeams = 2147483647)
    {
      this.Initialize(requestContext, project, includeTeamInformation);
      this.CollectionName = !string.IsNullOrEmpty(collectionName) ? collectionName : requestContext.ServiceHost.Name;
      if (includeTeamInformation)
      {
        IEnumerable<WebApiTeam> source = requestContext.GetService<ITeamService>().QueryTeamsInProject(requestContext, project.Id).OrderBy<WebApiTeam, string>((Func<WebApiTeam, string>) (team => team.Name), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).Take<WebApiTeam>(maxTeams);
        this.Teams = new List<IdentityViewModelBase>();
        foreach (TeamFoundationIdentity identity in !includeTeamMembers ? source.Select<WebApiTeam, TeamFoundationIdentity>((Func<WebApiTeam, TeamFoundationIdentity>) (x => IdentityUtil.Convert(x.Identity))) : (IEnumerable<TeamFoundationIdentity>) requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, source.Select<WebApiTeam, IdentityDescriptor>((Func<WebApiTeam, IdentityDescriptor>) (x => x.Identity.Descriptor)).ToArray<IdentityDescriptor>(), MembershipQuery.Direct, ReadIdentityOptions.None, (IEnumerable<string>) null))
          this.Teams.Add(IdentityImageUtility.GetIdentityViewModel<IdentityViewModelBase>(identity));
        this.Teams.Sort();
      }
      if (project.Properties == null)
        return;
      IList<ProjectProperty> properties = project.Properties;
      ProjectProperty projectProperty;
      Guid result;
      if ((projectProperty = properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (property => property.Name.Equals(ProcessTemplateIdPropertyNames.ProcessTemplateType, StringComparison.OrdinalIgnoreCase)))) == null || !Guid.TryParse((string) projectProperty.Value, out result))
        return;
      this.ProcessTemplateName = this.GetProcessTemplateName(requestContext, result);
    }

    public TeamProjectModel(IVssRequestContext requestContext, CatalogNode node) => this.Initialize(requestContext, node);

    public TeamProjectModel(IVssRequestContext requestContext, ProjectInfo project) => this.Initialize(requestContext, project, true);

    public TeamProjectModel(
      IVssRequestContext requestContext,
      ProjectInfo project,
      ProcessDescriptor templateDescriptor)
    {
      if (templateDescriptor != null)
        this.ProcessTemplateName = templateDescriptor.Name;
      this.Initialize(requestContext, project, true);
    }

    private void Initialize(IVssRequestContext requestContext, CatalogNode node)
    {
      CatalogResource resource = node.Resource;
      this.DisplayName = resource.Properties["ProjectName"];
      this.Uri = resource.Properties["ProjectUri"];
      this.Description = resource.Description;
      this.State = resource.Properties["ProjectState"].ParseEnum<ProjectState>();
      this.DisplayState = this.GetDisplayState(this.State);
      this.IsMalformed = false;
      this.Visibility = ProjectVisibility.Private;
      this.SetPermissionsAndIds(requestContext, true);
    }

    private void Initialize(
      IVssRequestContext requestContext,
      ProjectInfo project,
      bool includeTeamInformation)
    {
      this.DisplayName = project.Name;
      this.Uri = project.Uri;
      this.Description = project.Description;
      this.State = project.State;
      this.DisplayState = this.GetDisplayState(this.State);
      this.IsMalformed = this.IsProjectMalformed(requestContext, project);
      this.Visibility = project.Visibility;
      this.SetPermissionsAndIds(requestContext, includeTeamInformation);
    }

    private void SetPermissionsAndIds(
      IVssRequestContext requestContext,
      bool includeTeamInformation)
    {
      this.ProjectId = TeamProjectModel.ExtractProjectId(this.Uri);
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        if (includeTeamInformation)
          this.DefaultTeamId = requestContext.GetService<ITeamService>().GetDefaultTeamId(requestContext, ProjectInfo.GetProjectId(this.Uri));
        try
        {
          this.HasDeletePermission = this.CheckDeletePermission(requestContext, this.Uri);
        }
        catch (Exception ex)
        {
          this.HasDeletePermission = false;
        }
        try
        {
          this.HasRenamePermission = this.CheckRenamePermission(requestContext, this.Uri);
        }
        catch (Exception ex)
        {
          this.HasRenamePermission = false;
        }
      }
      else
      {
        this.HasDeletePermission = false;
        this.HasRenamePermission = false;
      }
      this.IsHosted = requestContext.IsHosted();
    }

    private bool CheckDeletePermission(IVssRequestContext requestContext, string projectUri)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string token = securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, projectUri);
      return securityNamespace.HasPermission(requestContext, token, TeamProjectPermissions.Delete);
    }

    private bool CheckRenamePermission(IVssRequestContext requestContext, string projectUri)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string token = securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, projectUri);
      return securityNamespace.HasPermission(requestContext, token, TeamProjectPermissions.Rename);
    }

    internal void PopulateMemberCount(IVssRequestContext requestContext)
    {
      TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
      HashSet<IdentityDescriptor> identityDescriptorSet = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      TeamFoundationIdentity[] source1 = service.ListApplicationGroups(requestContext, this.Uri, ReadIdentityOptions.None, (IEnumerable<string>) null);
      TeamFoundationIdentity[] source2 = service.ReadIdentities(requestContext, ((IEnumerable<TeamFoundationIdentity>) source1).Select<TeamFoundationIdentity, IdentityDescriptor>((Func<TeamFoundationIdentity, IdentityDescriptor>) (x => x.Descriptor)).ToArray<IdentityDescriptor>(), MembershipQuery.Expanded, ReadIdentityOptions.None, (IEnumerable<string>) null);
      foreach (TeamFoundationIdentity readIdentity in service.ReadIdentities(requestContext, ((IEnumerable<TeamFoundationIdentity>) source2).SelectMany<TeamFoundationIdentity, IdentityDescriptor>((Func<TeamFoundationIdentity, IEnumerable<IdentityDescriptor>>) (x => (IEnumerable<IdentityDescriptor>) x.Members)).ToArray<IdentityDescriptor>(), MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null))
      {
        if (readIdentity != null && (!readIdentity.IsContainer || string.Equals(readIdentity.Descriptor.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase)))
          identityDescriptorSet.Add(readIdentity.Descriptor);
      }
      this.MemberCount = identityDescriptorSet.Count;
    }

    private string GetProcessTemplateName(IVssRequestContext requestContext, Guid templateTypeId)
    {
      ProcessDescriptor descriptor;
      return !requestContext.GetService<ITeamFoundationProcessService>().TryGetProcessDescriptor(requestContext, templateTypeId, out descriptor) ? string.Empty : descriptor.Name;
    }

    public static Guid ExtractProjectId(string projectUri) => new Guid(LinkingUtilities.DecodeUri(projectUri.Trim()).ToolSpecificId);

    public string GetDisplayState(ProjectState state)
    {
      switch (state)
      {
        case ProjectState.New:
          return ProjectModelResources.ProjectStateCreating;
        case ProjectState.WellFormed:
          return ProjectModelResources.ProjectStateOnline;
        case ProjectState.Deleting:
          return ProjectModelResources.ProjectStateDeleting;
        case ProjectState.CreatePending:
          return ProjectModelResources.ProjectStateCreating;
        default:
          return ProjectModelResources.ProjectStateOnline;
      }
    }

    public bool IsProjectMalformed(IVssRequestContext requestContext, ProjectInfo project)
    {
      PlatformProjectService service = requestContext.GetService<PlatformProjectService>();
      return project.State != ProjectState.WellFormed && service.CanDeleteProject(requestContext, project);
    }

    public string Description { get; private set; }

    public string DisplayName { get; set; }

    public int MemberCount { get; private set; }

    public string Scope { get; set; }

    public string CollectionName { get; internal set; }

    public Guid DefaultTeamId { get; internal set; }

    public Guid ProjectId { get; internal set; }

    public List<IdentityViewModelBase> Teams { get; private set; }

    public string Uri { get; private set; }

    public bool HasDeletePermission { get; private set; }

    public bool HasRenamePermission { get; private set; }

    public string ProcessTemplateName { get; private set; }

    public bool IsHosted { get; private set; }

    public int CompareTo(TeamProjectModel other) => this.DisplayName.CompareTo(other.DisplayName);

    public ProjectState State { get; private set; }

    public string DisplayState { get; private set; }

    public bool IsMalformed { get; private set; }

    public ProjectVisibility Visibility { get; private set; }
  }
}
