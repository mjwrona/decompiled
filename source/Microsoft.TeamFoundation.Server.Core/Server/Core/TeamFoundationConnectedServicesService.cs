// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationConnectedServicesService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.Core
{
  public sealed class TeamFoundationConnectedServicesService : IVssFrameworkService
  {
    private static readonly string s_Area = "ConnectedServicesService";
    private static readonly string s_Layer = "Service";

    private TeamFoundationConnectedServicesService()
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ConnectedServiceMetadata CreateConnectedService(
      IVssRequestContext requestContext,
      ConnectedServiceCreationData connectedServiceCreationData)
    {
      requestContext.TraceEnter(30050, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, nameof (CreateConnectedService));
      try
      {
        ArgumentUtility.CheckForNull<ConnectedServiceCreationData>(connectedServiceCreationData, "connectedServiceSpec");
        connectedServiceCreationData.Validate();
        ProjectInfo projectInfo = TeamFoundationConnectedServicesService.GetProjectInfo(requestContext, connectedServiceCreationData.ServiceMetadata.TeamProject, true);
        IVssRequestContext boxRequestContext = TeamFoundationConnectedServicesService.GetStrongBoxRequestContext(requestContext, projectInfo.Uri);
        TeamFoundationStrongBoxService service1 = requestContext.GetService<TeamFoundationStrongBoxService>();
        string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.ServiceData", (object) connectedServiceCreationData.ServiceMetadata.Name);
        bool flag = false;
        Guid drawerId = service1.UnlockDrawer(boxRequestContext, name, false);
        if (drawerId == Guid.Empty)
        {
          drawerId = service1.CreateDrawer(boxRequestContext, name);
          flag = true;
        }
        TeamFoundationIdentityService service2 = requestContext.GetService<TeamFoundationIdentityService>();
        try
        {
          service1.AddString(boxRequestContext, drawerId, "Endpoint", connectedServiceCreationData.Endpoint);
          service1.AddString(boxRequestContext, drawerId, "Credentials", connectedServiceCreationData.CredentialsXml);
          if (!requestContext.IsServicingContext || connectedServiceCreationData.ServiceMetadata.AuthenticatedBy == Guid.Empty)
            connectedServiceCreationData.ServiceMetadata.AuthenticatedBy = service2.ReadRequestIdentity(requestContext).TeamFoundationId;
          using (ConnectedServiceComponent component = requestContext.CreateComponent<ConnectedServiceComponent>())
          {
            component.AddConnectedService(connectedServiceCreationData.ServiceMetadata, projectInfo.Id);
            return connectedServiceCreationData.ServiceMetadata;
          }
        }
        catch (Exception ex)
        {
          if (flag)
            service1.DeleteDrawer(boxRequestContext, drawerId);
          throw;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(30051, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(30052, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, nameof (CreateConnectedService));
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ConnectedServiceMetadata AddConnectedServiceProjectAssociation(
      IVssRequestContext requestContext,
      string name,
      string teamProject)
    {
      requestContext.TraceEnter(30055, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, nameof (AddConnectedServiceProjectAssociation));
      try
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(name, nameof (name));
        Guid projectId = TeamFoundationConnectedServicesService.GetProjectId(TeamFoundationConnectedServicesService.GetProjectInfo(requestContext, teamProject, true).Uri);
        ConnectedServiceMetadata connectedServiceMetadata = (ConnectedServiceMetadata) null;
        using (ConnectedServiceComponent component = requestContext.CreateComponent<ConnectedServiceComponent>())
          connectedServiceMetadata = component.AddConnectedServiceProjectAssociation(name, projectId, teamProject);
        return connectedServiceMetadata != null ? connectedServiceMetadata : throw new ArgumentException(FrameworkResources.ConnectedServiceNotFound((object) name));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(30056, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(30057, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, "CreateConnectedService");
      }
    }

    public List<ConnectedServiceMetadata> QueryConnectedServices(
      IVssRequestContext requestContext,
      string teamProject)
    {
      requestContext.TraceEnter(30060, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, nameof (QueryConnectedServices));
      ArgumentUtility.CheckStringForNullOrEmpty(teamProject, nameof (teamProject));
      if (!requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1, false))
        return new List<ConnectedServiceMetadata>();
      ProjectInfo projectInfo = TeamFoundationConnectedServicesService.GetProjectInfo(requestContext, teamProject, true);
      try
      {
        using (ConnectedServiceComponent component = requestContext.CreateComponent<ConnectedServiceComponent>())
          return component.GetConnectedServices(projectInfo.Id, projectInfo.Name);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(30061, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(30062, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, "GetConnectedServices");
      }
    }

    public bool DoesConnectedServiceExist(
      IVssRequestContext requestContext,
      string name,
      string teamProject)
    {
      requestContext.TraceEnter(30068, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, nameof (DoesConnectedServiceExist));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
        ProjectInfo projectInfo = TeamFoundationConnectedServicesService.GetProjectInfo(requestContext, teamProject, true);
        IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
        securityNamespace.CheckPermission(requestContext, securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, projectInfo.Uri), TeamProjectPermissions.GenericWrite, false);
        using (ConnectedServiceComponent component = requestContext.CreateComponent<ConnectedServiceComponent>())
        {
          if (component.GetConnectedService(name, Guid.Empty, string.Empty) != null)
            return true;
        }
        return false;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(30069, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(30069, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, nameof (DoesConnectedServiceExist));
      }
    }

    public ConnectedService GetConnectedService(
      IVssRequestContext requestContext,
      string name,
      string teamProject)
    {
      requestContext.TraceEnter(30070, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, nameof (GetConnectedService));
      try
      {
        ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
        ProjectInfo projectInfo = TeamFoundationConnectedServicesService.GetProjectInfo(requestContext, teamProject, true);
        IVssRequestContext requestContext1 = !requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext.Elevate(), FrameworkSecurity.StrongBoxNamespaceId).HasPermission(requestContext, FrameworkSecurity.StrongBoxSecurityNamespaceRootToken, 32, false) ? TeamFoundationConnectedServicesService.GetStrongBoxRequestContext(requestContext, projectInfo.Uri) : requestContext;
        TeamFoundationStrongBoxService service1 = requestContext.GetService<TeamFoundationStrongBoxService>();
        TeamFoundationLockingService service2 = requestContext.GetService<TeamFoundationLockingService>();
        Guid drawerId = service1.UnlockDrawer(requestContext1, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.ServiceData", (object) name), false);
        if (drawerId == Guid.Empty)
          return (ConnectedService) null;
        using (service2.AcquireLock(requestContext, TeamFoundationLockMode.Shared, drawerId.ToString()))
        {
          ConnectedServiceMetadata connectedService;
          using (ConnectedServiceComponent component = requestContext.CreateComponent<ConnectedServiceComponent>())
            connectedService = component.GetConnectedService(name, projectInfo.Id, projectInfo.Name);
          if (connectedService == null)
            return (ConnectedService) null;
          List<StrongBoxItemInfo> drawerContents = service1.GetDrawerContents(requestContext1, drawerId);
          return new ConnectedService(connectedService, drawerContents);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(30071, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(30072, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, nameof (GetConnectedService));
      }
    }

    public void DeleteConnectedService(
      IVssRequestContext requestContext,
      string name,
      string teamProject)
    {
      requestContext.TraceEnter(30080, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, nameof (DeleteConnectedService));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckStringForNullOrEmpty(teamProject, nameof (teamProject));
      try
      {
        ProjectInfo projectInfo = TeamFoundationConnectedServicesService.GetProjectInfo(requestContext, teamProject, true);
        IVssRequestContext boxRequestContext = TeamFoundationConnectedServicesService.GetStrongBoxRequestContext(requestContext, projectInfo.Uri);
        TeamFoundationStrongBoxService service1 = requestContext.GetService<TeamFoundationStrongBoxService>();
        TeamFoundationLockingService service2 = requestContext.GetService<TeamFoundationLockingService>();
        Guid drawerId = service1.UnlockDrawer(boxRequestContext, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.ServiceData", (object) name), false);
        if (drawerId == Guid.Empty)
          return;
        using (service2.AcquireLock(requestContext, TeamFoundationLockMode.Exclusive, drawerId.ToString()))
        {
          bool flag;
          using (ConnectedServiceComponent component = requestContext.CreateComponent<ConnectedServiceComponent>())
            flag = component.DeleteProjectAssociationsAndConnectedServiceIfNotReferenced(name, projectInfo.Id);
          requestContext.GetService<TeamFoundationEventService>().PublishNotification(requestContext, (object) new ConnectedServiceDeletedEvent(name, projectInfo.Name));
          if (!flag)
            return;
          service1.DeleteDrawer(boxRequestContext, drawerId);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(30081, TraceLevel.Error, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(30082, TeamFoundationConnectedServicesService.s_Area, TeamFoundationConnectedServicesService.s_Layer, nameof (DeleteConnectedService));
      }
    }

    internal static Guid GetProjectId(string projectUri)
    {
      try
      {
        return ProjectInfo.GetProjectId(projectUri);
      }
      catch (ArgumentException ex)
      {
        throw new ProjectDoesNotExistException();
      }
    }

    internal static ProjectInfo GetProjectInfo(
      IVssRequestContext requestContext,
      string projFilter,
      bool throwOnProjectNotFound)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projFilter, nameof (projFilter));
      IProjectService service = requestContext.GetService<IProjectService>();
      Guid result;
      ProjectInfo projectInfo;
      if (!Guid.TryParse(projFilter, out result))
      {
        try
        {
          projectInfo = service.GetProject(requestContext, projFilter);
        }
        catch (ProjectDoesNotExistWithNameException ex)
        {
          projectInfo = (ProjectInfo) null;
        }
      }
      else
      {
        try
        {
          projectInfo = service.GetProject(requestContext, result);
        }
        catch (ProjectDoesNotExistException ex)
        {
          projectInfo = (ProjectInfo) null;
        }
      }
      return !(projectInfo == null & throwOnProjectNotFound) ? projectInfo : throw new ProjectDoesNotExistException();
    }

    private static IVssRequestContext GetStrongBoxRequestContext(
      IVssRequestContext requestContext,
      string projectUri)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      securityNamespace.CheckPermission(requestContext, securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, projectUri), TeamProjectPermissions.GenericWrite, false);
      return requestContext.Elevate();
    }
  }
}
