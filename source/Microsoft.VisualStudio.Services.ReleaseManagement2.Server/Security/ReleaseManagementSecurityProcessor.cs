// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security.ReleaseManagementSecurityProcessor
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.DeploymentTracking.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Constants;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required use of types")]
  public static class ReleaseManagementSecurityProcessor
  {
    public const string Area = "ReleaseManagementService";
    public const string Layer = "ReleaseManagementLayer";
    private const string ReleaseManagementSecurityGroupXmlRelativePath = "Web Services\\bin\\ReleaseManagementSecurityGroups.xml";
    private const string ReleaseManagementFaultInRegistryPath = "/Service/ReleaseManagement/FaultIn/{0}";
    public static readonly Guid MetaTaskNamespaceId = new Guid("f6a4de49-dbe2-4704-86dc-f8ec1a294436");
    public static readonly Guid AgentPoolNamespaceId = new Guid("101EAE8C-1709-47F9-B228-0E476C35B3BA");
    public static readonly Guid LibraryNamespaceId = new Guid("B7E84409-6553-448A-BBB2-AF228E07CBEB");
    public static readonly string AgentQueueToken = "AgentQueues";
    public static readonly string MachineGroupToken = "MachineGroups";
    public static readonly string LibraryToken = "Library";
    [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "Required to be public for tests")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Required to be public for tests")]
    public static Action<IVssRequestContext, Guid, IdentityDescriptor> EnsureReleaseAdministratorPermissionFunc = new Action<IVssRequestContext, Guid, IdentityDescriptor>(ReleaseManagementSecurityProcessor.EnsureReleaseAdministratorPermissionOnRemoteNamespaces);

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "optional param")]
    public static bool CheckAttributePermission(
      IVssRequestContext requestContext,
      ReleaseManagementSecurityPermissionAttribute securityAttribute,
      Dictionary<string, object> controllerArgumentsDictionary,
      Guid projectId,
      bool isByePassFaultInHeaderSet = false)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (securityAttribute == null)
        throw new ArgumentNullException(nameof (securityAttribute));
      if (controllerArgumentsDictionary == null)
        throw new ArgumentNullException(nameof (controllerArgumentsDictionary));
      if (projectId.Equals(Guid.Empty))
        return true;
      ReleaseManagementSecurityProcessor.InitializePermissionsIfRequired(requestContext, projectId, securityAttribute, isByePassFaultInHeaderSet);
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseManagementSecurityProcessor.CheckAttributePermission", 1961013))
      {
        int releaseDefinitionId = 0;
        string b = (string) null;
        switch (securityAttribute.ApiArgType)
        {
          case ReleaseManagementSecurityArgumentType.Project:
            ReleaseManagementSecurityInfo securityInfo1 = new ReleaseManagementSecurityInfo()
            {
              ProjectId = projectId,
              Path = b,
              ReleaseDefinitionId = releaseDefinitionId,
              Permission = securityAttribute.Permission
            };
            bool flag1 = requestContext.HasPermission(securityInfo1, securityAttribute.AlwaysAllowAdministrators);
            if (securityAttribute.ApiArgType == ReleaseManagementSecurityArgumentType.UpdateReleaseDefinition)
            {
              string folderPath = ReleaseManagementSecurityProcessor.GetFolderPath(requestContext, projectId, releaseDefinitionId);
              if (!string.Equals(folderPath, b, StringComparison.OrdinalIgnoreCase))
              {
                ReleaseManagementSecurityInfo securityInfo2 = new ReleaseManagementSecurityInfo()
                {
                  ProjectId = projectId,
                  Path = folderPath,
                  ReleaseDefinitionId = releaseDefinitionId,
                  Permission = securityAttribute.Permission
                };
                bool flag2 = requestContext.HasPermission(securityInfo2, securityAttribute.AlwaysAllowAdministrators);
                flag1 &= flag2;
              }
            }
            if (flag1 && securityInfo1.Permission != ReleaseManagementSecurityPermissions.None)
              requestContext.SetSecurityTokenCache(securityInfo1.GetToken(), securityInfo1.Permission);
            releaseManagementTimer.RecordLap("Service", "ReleaseManagementSecurityProcessor.CheckAttributePermission.HasPermissions", 1961014);
            return flag1;
          case ReleaseManagementSecurityArgumentType.ReleaseDefinitionId:
            releaseDefinitionId = ReleaseManagementSecurityProcessor.GetParameterValue<int>(controllerArgumentsDictionary, securityAttribute.ApiArgName);
            b = ReleaseManagementSecurityProcessor.GetFolderPath(requestContext, projectId, releaseDefinitionId);
            goto case ReleaseManagementSecurityArgumentType.Project;
          case ReleaseManagementSecurityArgumentType.ReleaseDefinition:
          case ReleaseManagementSecurityArgumentType.UpdateReleaseDefinition:
            ReleaseDefinition parameterValue1 = ReleaseManagementSecurityProcessor.GetParameterValue<ReleaseDefinition>(controllerArgumentsDictionary, securityAttribute.ApiArgName);
            releaseDefinitionId = parameterValue1.Id;
            b = parameterValue1.Path;
            goto case ReleaseManagementSecurityArgumentType.Project;
          case ReleaseManagementSecurityArgumentType.ReleaseId:
            int parameterValue2 = ReleaseManagementSecurityProcessor.GetParameterValue<int>(controllerArgumentsDictionary, securityAttribute.ApiArgName);
            KeyValuePair<int, string> definitionFolderPathAndId = ReleaseManagementSecurityProcessor.GetReleaseDefinitionFolderPathAndId(requestContext, projectId, parameterValue2);
            releaseDefinitionId = definitionFolderPathAndId.Key;
            b = definitionFolderPathAndId.Value;
            goto case ReleaseManagementSecurityArgumentType.Project;
          case ReleaseManagementSecurityArgumentType.ReleaseStartMetadataObject:
            releaseDefinitionId = ReleaseManagementSecurityProcessor.GetParameterValue<ReleaseStartMetadata>(controllerArgumentsDictionary, securityAttribute.ApiArgName).DefinitionId;
            b = ReleaseManagementSecurityProcessor.GetFolderPath(requestContext, projectId, releaseDefinitionId);
            goto case ReleaseManagementSecurityArgumentType.Project;
          case ReleaseManagementSecurityArgumentType.DeploymentResource:
            releaseDefinitionId = ReleaseManagementSecurityProcessor.GetParameterValue<DeploymentResource>(controllerArgumentsDictionary, securityAttribute.ApiArgName).ReleaseDefinitionId;
            b = ReleaseManagementSecurityProcessor.GetFolderPath(requestContext, projectId, releaseDefinitionId);
            goto case ReleaseManagementSecurityArgumentType.Project;
          default:
            requestContext.Trace(1900000, TraceLevel.Error, "ReleaseManagementService", "ReleaseManagementLayer", Resources.UnexpectedSecurityAttribute, (object) securityAttribute.ApiArgType);
            throw new ReleaseManagementException(Resources.UnexpectedSecurityAttribute, new object[1]
            {
              (object) securityAttribute.ApiArgType
            });
        }
      }
    }

    private static void InitializePermissionsIfRequired(
      IVssRequestContext requestContext,
      Guid projectId,
      ReleaseManagementSecurityPermissionAttribute securityAttribute,
      bool isByePassFaultInHeaderSet)
    {
      if (securityAttribute.AllowByePassDataspaceFaultIn & isByePassFaultInHeaderSet)
        return;
      ReleaseManagementSecurityProcessor.InitializePermissionsIfRequired(requestContext, projectId);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.TeamFoundation.Framework.Server.VssRequestContextExtensions.Trace(Microsoft.TeamFoundation.Framework.Server.IVssRequestContext,System.Int32,System.Diagnostics.TraceLevel,System.String,System.String,System.String)", Justification = "This is used in trace which need not be localized")]
    public static bool InitializePermissionsIfRequired(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      using (ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseManagementSecurityProcessor.InitializePermissionsIfRequired", 1961015))
      {
        using (IVssRequestContext vssRequestContext = requestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(requestContext, requestContext.ServiceHost.InstanceId, RequestContextType.SystemContext, throwIfShutdown: false))
        {
          if (ReleaseManagementSecurityProcessor.ShouldInitializePermissions(vssRequestContext, projectId))
          {
            if (vssRequestContext.GetService<ILeaseService>().TryAcquireLease(vssRequestContext, projectId.ToString("D"), TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(1.0), out ILeaseInfo _))
            {
              if (ReleaseManagementSecurityProcessor.ShouldInitializePermissions(vssRequestContext, projectId))
              {
                string faultInRegistryKey = ReleaseManagementSecurityProcessor.GetProjectFaultInRegistryKey(projectId);
                IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
                service.SetValue<bool>(vssRequestContext, faultInRegistryKey, true);
                vssRequestContext.CreateDataspaces(projectId);
                ReleaseManagementSecurityProcessor.InitializePermissions(vssRequestContext, projectId);
                service.DeleteEntries(vssRequestContext, (IEnumerable<string>) new List<string>()
                {
                  faultInRegistryKey
                });
                return true;
              }
            }
            else
              vssRequestContext.Trace(1976408, TraceLevel.Info, "ReleaseManagementService", "ReleaseManagementLayer", "Cannot Acquire Initialization Lease. Some other thread has already started the initialization.");
          }
          return false;
        }
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    public static void InitializePermissions(IVssRequestContext requestContext, Guid projectId)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      requestContext.TraceEnter(1971026, "ReleaseManagementService", "ReleaseManagementLayer", nameof (InitializePermissions));
      List<IAccessControlList> accessControlListList = new List<IAccessControlList>();
      List<IAccessControlEntry> aces = new List<IAccessControlEntry>();
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, SecurityConstants.ReleaseManagementSecurityNamespaceId).Secured();
      IdentityService service = requestContext.GetService<IdentityService>();
      IVssRequestContext requestContext1 = requestContext.Elevate();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ListGroups(requestContext1, new Guid[1]
      {
        projectId
      }, false, (IEnumerable<string>) null);
      if (!ReleaseManagementSecurityProcessor.TryAddReleaseAdministratorsGroupPermission(requestContext, service, identityList, projectId, (IList<IAccessControlEntry>) aces))
        requestContext.Trace(1971054, TraceLevel.Error, "ReleaseManagementService", "ReleaseManagementLayer", Resources.CannotFindOrCreateReleaseAdministratorGrooup);
      ReleaseManagementSecurityProcessor.AddDefaultGroupsPermissions(requestContext, identityList, (IList<IAccessControlEntry>) aces);
      ReleaseManagementSecurityProcessor.AddWellKnownIdentitiesPermissions(requestContext1, (IList<IAccessControlEntry>) aces);
      List<Microsoft.VisualStudio.Services.Identity.Identity> serviceIdentities = ReleaseManagementSecurityProcessor.GetBuildServiceIdentities(requestContext1, projectId);
      ReleaseManagementSecurityPermissions identityPermission = ReleaseManagementSecurityProcessor.GetBuildServiceIdentityPermission(requestContext);
      ReleaseManagementSecurityProcessor.AddBuildServicePermissions((IList<IAccessControlEntry>) aces, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) serviceIdentities, identityPermission);
      accessControlListList.Add((IAccessControlList) new Microsoft.TeamFoundation.Framework.Server.AccessControlList(ReleaseManagementSecurityHelper.GetToken(projectId), true, (IEnumerable<IAccessControlEntry>) aces));
      securityNamespace.SetAccessControlLists(requestContext1, (IEnumerable<IAccessControlList>) accessControlListList);
      requestContext.TraceLeave(1971027, "ReleaseManagementService", "ReleaseManagementLayer", nameof (InitializePermissions));
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This service step must gracefully exit on not finding any projects.")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a servicing Step, we do not want the string to be localized")]
    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "This is a servicing Step, we do not want the string to be localized")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "VSSF requires that this method not be a static method.")]
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "This is because of dependency on many classes from vssf framework.")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is a servicing Step, we do not want the string to be localized")]
    public static void AddDeleteReleasesAndManageDeploymentsPermissionsToContributorsGroup(
      IVssRequestContext requestContext,
      IProjectService projectService,
      IDataspaceService dataspaceService,
      ReleaseManagementSecurityProcessor.ServicingLogger logger)
    {
      if (logger == null)
        throw new ArgumentNullException(nameof (logger));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (projectService == null)
        throw new ArgumentNullException(nameof (projectService));
      if (dataspaceService == null)
        throw new ArgumentNullException(nameof (dataspaceService));
      logger(ServicingStepLogEntryKind.Informational, "Begin: Initialize delete releases/manage deployments permissions.");
      try
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        IEnumerable<ProjectInfo> projects = projectService.GetProjects(vssRequestContext, ProjectState.WellFormed);
        logger(ServicingStepLogEntryKind.Informational, string.Format((IFormatProvider) CultureInfo.InstalledUICulture, "Project count: {0}", (object) projects.Count<ProjectInfo>()));
        foreach (ProjectInfo project in projects)
        {
          try
          {
            if (dataspaceService.QueryDataspace(requestContext, "ReleaseManagement", project.Id, false) != null)
            {
              logger(ServicingStepLogEntryKind.Informational, string.Format((IFormatProvider) CultureInfo.InstalledUICulture, "Begin: Initialize delete releases/manage deployments permissions for project {0}.", (object) project.Name));
              ReleaseManagementSecurityProcessor.AddDefaultPermissionsToGroupIfItIsNotAlreadySet(logger, requestContext, project, "Project Administrators", (IEnumerable<ReleaseManagementSecurityPermissions>) new List<ReleaseManagementSecurityPermissions>()
              {
                ReleaseManagementSecurityPermissions.DeleteReleases,
                ReleaseManagementSecurityPermissions.ManageDeployments
              });
              ReleaseManagementSecurityProcessor.AddDefaultPermissionsToGroupIfItIsNotAlreadySet(logger, requestContext, project, "Contributors", (IEnumerable<ReleaseManagementSecurityPermissions>) new List<ReleaseManagementSecurityPermissions>()
              {
                ReleaseManagementSecurityPermissions.DeleteReleases,
                ReleaseManagementSecurityPermissions.ManageDeployments
              });
              ReleaseManagementSecurityProcessor.AddDefaultPermissionsToGroupIfItIsNotAlreadySet(logger, requestContext, project, SecurityConstants.ReleaseManagersGroupName, (IEnumerable<ReleaseManagementSecurityPermissions>) new List<ReleaseManagementSecurityPermissions>()
              {
                ReleaseManagementSecurityPermissions.DeleteReleases,
                ReleaseManagementSecurityPermissions.ManageDeployments
              });
              ReleaseManagementSecurityProcessor.AddDefaultPermissionsToGroupIfItIsNotAlreadySet(logger, requestContext, project, SecurityConstants.ReleaseAdministratorsGroup, (IEnumerable<ReleaseManagementSecurityPermissions>) new List<ReleaseManagementSecurityPermissions>()
              {
                ReleaseManagementSecurityPermissions.DeleteReleases,
                ReleaseManagementSecurityPermissions.ManageDeployments
              });
              ReleaseManagementSecurityProcessor.AddPermissionsToWellKnownIdentities(logger, vssRequestContext, project, (IEnumerable<ReleaseManagementSecurityPermissions>) new List<ReleaseManagementSecurityPermissions>()
              {
                ReleaseManagementSecurityPermissions.DeleteReleases,
                ReleaseManagementSecurityPermissions.ManageDeployments
              });
              logger(ServicingStepLogEntryKind.Informational, string.Format((IFormatProvider) CultureInfo.InstalledUICulture, "End: Initialize delete releases/manage deployments permissions for project {0}.", (object) project.Name));
            }
            else
              logger(ServicingStepLogEntryKind.Informational, string.Format((IFormatProvider) CultureInfo.InstalledUICulture, "Project {0} is not associated with Release Management, Ignoring.", (object) project.Name));
          }
          catch (Exception ex)
          {
            logger(ServicingStepLogEntryKind.Warning, string.Format((IFormatProvider) CultureInfo.InstalledUICulture, "Failed to initialize delete releases/manage deployments permissions for project: {0}, Exception: {1}", (object) project.Name, (object) ex));
            requestContext.TraceException(1971028, "ReleaseManagementService", "ReleaseManagementLayer", ex);
          }
        }
      }
      catch (Exception ex)
      {
        logger(ServicingStepLogEntryKind.Warning, string.Format((IFormatProvider) CultureInfo.InstalledUICulture, "Failed to initialize delete releases/manage deployments permissions for ActivityId: {0}, Exception: {1}", (object) requestContext.ActivityId, (object) ex));
        requestContext.TraceException(1971028, "ReleaseManagementService", "ReleaseManagementLayer", ex);
      }
      logger(ServicingStepLogEntryKind.Informational, "End: Initialize delete releases/manage deployments permissions.");
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a servicing Step, we do not want the string to be localized")]
    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "This is a servicing Step, we do not want the string to be localized")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is a servicing Step, we do not want the string to be localized")]
    public static void AddPermissionsToWellKnownIdentities(
      ReleaseManagementSecurityProcessor.ServicingLogger logger,
      IVssRequestContext elevatedContext,
      ProjectInfo project,
      IEnumerable<ReleaseManagementSecurityPermissions> permissions)
    {
      if (logger == null)
        throw new ArgumentNullException(nameof (logger));
      if (elevatedContext == null)
        throw new ArgumentNullException(nameof (elevatedContext));
      if (project == null)
        throw new ArgumentNullException(nameof (project));
      if (permissions == null)
        throw new ArgumentNullException(nameof (permissions));
      logger(ServicingStepLogEntryKind.Informational, "Begin: Initializing permissions for WellKnown identities.");
      IVssSecurityNamespace securedNamespaceService = elevatedContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(elevatedContext, SecurityConstants.ReleaseManagementSecurityNamespaceId).Secured();
      List<IdentityDescriptor> descriptors = new List<IdentityDescriptor>()
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
      };
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = elevatedContext.GetService<IdentityService>().ReadIdentities(elevatedContext, (IList<IdentityDescriptor>) descriptors, QueryMembership.None, (IEnumerable<string>) null);
      if (identityList != null)
      {
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList)
        {
          if (identity != null)
          {
            logger(ServicingStepLogEntryKind.Informational, string.Format((IFormatProvider) CultureInfo.InstalledUICulture, "Initializing permissions for WellKnown identity: {0}, project: {1}.", (object) identity.Descriptor.Identifier, (object) project.Name));
            ReleaseManagementSecurityProcessor.SetPermissionsIfRequired(logger, project, identity.Descriptor.Identifier, permissions, securedNamespaceService, elevatedContext, identity);
          }
        }
      }
      logger(ServicingStepLogEntryKind.Informational, "End: Initializing permissions for WellKnown identities.");
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a servicing Step, we do not want the string to be localized")]
    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "This is a servicing Step, we do not want the string to be localized")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is a servicing Step, we do not want the string to be localized")]
    public static void AddDefaultPermissionsToGroupIfItIsNotAlreadySet(
      ReleaseManagementSecurityProcessor.ServicingLogger logger,
      IVssRequestContext requestContext,
      ProjectInfo project,
      string groupName,
      IEnumerable<ReleaseManagementSecurityPermissions> permissions)
    {
      if (logger == null)
        throw new ArgumentNullException(nameof (logger));
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (project == null)
        throw new ArgumentNullException(nameof (project));
      if (permissions == null)
        throw new ArgumentNullException(nameof (permissions));
      requestContext.TraceEnter(1971026, "ReleaseManagementService", "ReleaseManagementLayer", nameof (AddDefaultPermissionsToGroupIfItIsNotAlreadySet));
      IVssSecurityNamespace securedNamespaceService = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, SecurityConstants.ReleaseManagementSecurityNamespaceId).Secured();
      IdentityService service = requestContext.GetService<IdentityService>();
      IVssRequestContext elevatedContext = requestContext.Elevate();
      IVssRequestContext requestContext1 = elevatedContext;
      Guid[] scopeIds = new Guid[1]{ project.Id };
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = service.ListGroups(requestContext1, scopeIds, false, (IEnumerable<string>) null);
      List<string> localizedGroupNames = ReleaseManagementSecurityProcessor.GetLocalizedGroupNames(requestContext).FirstOrDefault<KeyValuePair<string, List<string>>>((Func<KeyValuePair<string, List<string>>, bool>) (x => x.Key.Equals(groupName))).Value;
      if (localizedGroupNames.IsNullOrEmpty<string>())
        localizedGroupNames = new List<string>()
        {
          groupName
        };
      Func<Microsoft.VisualStudio.Services.Identity.Identity, bool> predicate = (Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group =>
      {
        string accountName = group.GetProperty<string>("Account", string.Empty);
        return localizedGroupNames.Any<string>((Func<string, bool>) (localGroupName => string.Equals(accountName, localGroupName, StringComparison.OrdinalIgnoreCase)));
      });
      Microsoft.VisualStudio.Services.Identity.Identity identity = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>(predicate);
      if (identity != null)
        ReleaseManagementSecurityProcessor.SetPermissionsIfRequired(logger, project, groupName, permissions, securedNamespaceService, elevatedContext, identity);
      else
        logger(ServicingStepLogEntryKind.Informational, string.Format((IFormatProvider) CultureInfo.InstalledUICulture, "Group with name {0} is not found for project {1}", (object) groupName, (object) project.Name));
      requestContext.TraceLeave(1971027, "ReleaseManagementService", "ReleaseManagementLayer", nameof (AddDefaultPermissionsToGroupIfItIsNotAlreadySet));
    }

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Framework internals might change in future")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "servicingContext", Justification = "servicingContext is a valid term here.")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "elevatedContext", Justification = "elevatedContext is a valid term here.")]
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Arguments are already validated in calling method.")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is a servicing Step, we do not want the string to be localized")]
    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "This is a servicing Step, we do not want the string to be localized")]
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is a servicing Step, we do not want the string to be localized")]
    public static void SetPermissionsIfRequired(
      ReleaseManagementSecurityProcessor.ServicingLogger logger,
      ProjectInfo project,
      string identityName,
      IEnumerable<ReleaseManagementSecurityPermissions> permissions,
      IVssSecurityNamespace securedNamespaceService,
      IVssRequestContext elevatedContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      logger(ServicingStepLogEntryKind.Informational, string.Format((IFormatProvider) CultureInfo.InstalledUICulture, "Begin: Initializing permissions for identity: {0}, project: {1}", (object) identityName, (object) project.Name));
      string token = ReleaseManagementSecurityHelper.GetToken(project.Id);
      IAccessControlEntry accessControlEntry = (IAccessControlEntry) null;
      IAccessControlList accessControlList = securedNamespaceService.QueryAccessControlLists(elevatedContext, token, (IEnumerable<IdentityDescriptor>) new List<IdentityDescriptor>()
      {
        identity.Descriptor
      }, false, false).FirstOrDefault<IAccessControlList>();
      if (accessControlList != null)
        accessControlEntry = accessControlList.AccessControlEntries.FirstOrDefault<IAccessControlEntry>();
      ReleaseManagementSecurityPermissions allow = ReleaseManagementSecurityPermissions.None;
      foreach (ReleaseManagementSecurityPermissions permission in permissions)
      {
        if (accessControlEntry != null && (((ReleaseManagementSecurityPermissions) accessControlEntry.Allow & permission) > ReleaseManagementSecurityPermissions.None || ((ReleaseManagementSecurityPermissions) accessControlEntry.Deny & permission) > ReleaseManagementSecurityPermissions.None))
          logger(ServicingStepLogEntryKind.Informational, string.Format((IFormatProvider) CultureInfo.InstalledUICulture, "Permission {0} is already set for identity: {1}, project: {2}, existingAccessControlEntryAllowed: {3}, existingAccessControlEntryDenied: {4}", (object) permission, (object) identityName, (object) project.Name, (object) accessControlEntry.Allow, (object) accessControlEntry.Deny));
        else
          allow |= permission;
      }
      if (allow != ReleaseManagementSecurityPermissions.None)
        securedNamespaceService.SetAccessControlEntries(elevatedContext, token, (IEnumerable<IAccessControlEntry>) new List<IAccessControlEntry>()
        {
          (IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(identity.Descriptor, (int) allow, 0)
        }, true, false);
      logger(ServicingStepLogEntryKind.Informational, string.Format((IFormatProvider) CultureInfo.InstalledUICulture, "End: Initializing permissions for identity: {0}, project: {1}", (object) identityName, (object) project.Name));
    }

    public static string InitializeBuildServicePermissions(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> serviceIdentities = ReleaseManagementSecurityProcessor.GetBuildServiceIdentities(requestContext, projectId);
      return ReleaseManagementSecurityProcessor.InitializeBuildServicePermissions(requestContext, projectId, (string) null, 0, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) serviceIdentities);
    }

    public static string InitializeBuildServicePermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      string releaseDefinitionPath,
      int releaseDefinitionId)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> serviceIdentities = ReleaseManagementSecurityProcessor.GetBuildServiceIdentities(requestContext, projectId);
      return ReleaseManagementSecurityProcessor.InitializeBuildServicePermissions(requestContext, projectId, releaseDefinitionPath, releaseDefinitionId, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) serviceIdentities);
    }

    public static string InitializeBuildServicePermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      string releaseDefinitionPath,
      int releaseDefinitionId,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> serviceIdentities)
    {
      string token = ReleaseManagementSecurityHelper.GetToken(projectId, releaseDefinitionPath, releaseDefinitionId);
      return ReleaseManagementSecurityProcessor.InitializeBuildServicePermissions(requestContext, token, serviceIdentities);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Required use of types")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to handle any type of exception here")]
    public static string InitializeBuildServicePermissions(
      IVssRequestContext requestContext,
      string securityToken,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> serviceIdentities)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (string.IsNullOrEmpty(securityToken))
        throw new ArgumentNullException(nameof (securityToken));
      if (serviceIdentities == null || serviceIdentities.Count == 0)
        throw new ArgumentNullException(nameof (serviceIdentities));
      StringBuilder stringBuilder = new StringBuilder();
      ReleaseManagementSecurityPermissions identityPermission = ReleaseManagementSecurityProcessor.GetBuildServiceIdentityPermission(requestContext);
      try
      {
        IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, SecurityConstants.ReleaseManagementSecurityNamespaceId);
        List<IAccessControlEntry> aces1 = new List<IAccessControlEntry>();
        List<IAccessControlList> list1 = securityNamespace.QueryAccessControlLists(requestContext, securityToken, true, false).ToList<IAccessControlList>();
        List<IAccessControlEntry> list2 = list1.SelectMany<IAccessControlList, IAccessControlEntry>((Func<IAccessControlList, IEnumerable<IAccessControlEntry>>) (x => x.AccessControlEntries)).ToList<IAccessControlEntry>();
        bool flag = false;
        foreach (Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) serviceIdentities)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = serviceIdentity;
          IAccessControlEntry accessControlEntry = list2.FirstOrDefault<IAccessControlEntry>((Func<IAccessControlEntry, bool>) (x => IdentityDescriptorComparer.Instance.Equals(x.Descriptor, identity.Descriptor)));
          if (accessControlEntry != null)
          {
            stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ACE for identity {0} has been set already :: Allow: {1}, EffectiveAllow:{2}, Deny: {3}, EffectiveDeny:{4}", (object) identity.DisplayName, (object) accessControlEntry.Allow, (object) accessControlEntry.EffectiveAllow, (object) accessControlEntry.Deny, (object) accessControlEntry.EffectiveDeny));
            if (((ReleaseManagementSecurityPermissions) accessControlEntry.EffectiveAllow & identityPermission) != identityPermission)
            {
              stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Setting required permissions {0} explicitly on ACE of build service identity {1}", (object) identityPermission, (object) identity.DisplayName));
              accessControlEntry.Allow |= (int) identityPermission;
              accessControlEntry.Deny &= (int) ~identityPermission;
              flag = true;
            }
          }
          else
          {
            stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot find ACE for identity {0}, setting it now with permissions: {1}", (object) identity.DisplayName, (object) identityPermission));
            List<IAccessControlEntry> aces2 = aces1;
            List<Microsoft.VisualStudio.Services.Identity.Identity> serviceIdentities1 = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
            serviceIdentities1.Add(identity);
            int permissions = (int) identityPermission;
            ReleaseManagementSecurityProcessor.AddBuildServicePermissions((IList<IAccessControlEntry>) aces2, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) serviceIdentities1, (ReleaseManagementSecurityPermissions) permissions);
          }
        }
        if (aces1.Count > 0)
        {
          flag = true;
          list1.Add((IAccessControlList) new Microsoft.TeamFoundation.Framework.Server.AccessControlList(securityToken, true, (IEnumerable<IAccessControlEntry>) aces1));
        }
        if (flag)
          securityNamespace.SetAccessControlLists(requestContext, (IEnumerable<IAccessControlList>) list1);
        foreach (IAccessControlEntry accessControlEntry in securityNamespace.QueryAccessControlLists(requestContext, securityToken, serviceIdentities.Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (x => x.Descriptor)), true, false).SelectMany<IAccessControlList, IAccessControlEntry>((Func<IAccessControlList, IEnumerable<IAccessControlEntry>>) (x => x.AccessControlEntries)))
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ACE for identity {0} :: Allow: {1}, EffectiveAllow: {2}, Deny: {3}, EffectiveDeny: {4}", (object) accessControlEntry.Descriptor.Identifier, (object) accessControlEntry.Allow, (object) accessControlEntry.EffectiveAllow, (object) accessControlEntry.Deny, (object) accessControlEntry.EffectiveDeny);
          stringBuilder.AppendLine(str);
        }
      }
      catch (Exception ex)
      {
        stringBuilder.AppendLine(ex.Message);
      }
      return stringBuilder.ToString();
    }

    public static void RemoveAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<string> tokens)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (tokens == null || tokens.Count<string>() <= 0)
        throw new ArgumentNullException(nameof (tokens));
      requestContext.Elevate().GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, SecurityConstants.ReleaseManagementSecurityNamespaceId).RemoveAccessControlLists(requestContext, tokens, true);
    }

    public static void SetAccessControlEntry(
      IVssRequestContext requestContext,
      string token,
      IdentityBase identity,
      ReleaseManagementSecurityPermissions allowPermission,
      ReleaseManagementSecurityPermissions denyPermission)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (string.IsNullOrEmpty(token))
        throw new ArgumentNullException(nameof (token));
      if (identity == null)
        throw new ArgumentNullException(nameof (identity));
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext.Elevate(), SecurityConstants.ReleaseManagementSecurityNamespaceId);
      List<IAccessControlEntry> accessControlEntryList1 = new List<IAccessControlEntry>()
      {
        (IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry()
        {
          Descriptor = identity.Descriptor,
          Allow = (int) allowPermission,
          Deny = (int) denyPermission
        }
      };
      IVssRequestContext requestContext1 = requestContext.Elevate();
      string token1 = token;
      List<IAccessControlEntry> accessControlEntryList2 = accessControlEntryList1;
      securityNamespace.SetAccessControlEntries(requestContext1, token1, (IEnumerable<IAccessControlEntry>) accessControlEntryList2, true);
    }

    public static IList<T> FilterComponents<T>(
      IVssRequestContext requestContext,
      IEnumerable<T> components,
      Func<T, ReleaseManagementSecurityInfo> getSecurityInfo,
      bool shouldSecureComponents,
      out IList<T> excludedComponents)
    {
      excludedComponents = (IList<T>) new List<T>();
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (getSecurityInfo == null)
        throw new ArgumentNullException(nameof (getSecurityInfo));
      if (components == null)
        return (IList<T>) null;
      using (ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseManagementSecurityProcessor.FilterComponents", 1900038))
      {
        List<T> objList = new List<T>();
        foreach (T component in components)
        {
          ReleaseManagementSecurityInfo securityInfo = getSecurityInfo(component);
          if (securityInfo == null)
            throw new ArgumentNullException("securityInfo");
          if (requestContext.HasPermission(securityInfo, true))
          {
            objList.Add(component);
            if (shouldSecureComponents)
            {
              requestContext.SetSecurityTokenCache(securityInfo.GetToken(), securityInfo.Permission);
              if (component is ReleaseManagementSecuredObject result)
                requestContext.SetSecuredObject<ReleaseManagementSecuredObject>(result);
            }
          }
          else
            excludedComponents.Add(component);
        }
        return (IList<T>) objList;
      }
    }

    public static IList<T> FilterComponents<T>(
      IVssRequestContext requestContext,
      IEnumerable<T> components,
      Func<T, ReleaseManagementSecurityInfo> getSecurityInfo,
      bool shouldSecureComponents)
    {
      IList<T> excludedComponents = (IList<T>) null;
      return ReleaseManagementSecurityProcessor.FilterComponents<T>(requestContext, components, getSecurityInfo, shouldSecureComponents, out excludedComponents);
    }

    public static IList<T> SecureComponents<T>(
      IVssRequestContext requestContext,
      IList<T> components,
      Func<T, ReleaseManagementSecurityInfo> getSecurityInfo)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (getSecurityInfo == null)
        throw new ArgumentNullException(nameof (getSecurityInfo));
      if (components == null)
        return (IList<T>) null;
      using (ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseManagementSecurityProcessor.SecureComponents", 1900039))
      {
        foreach (T component in (IEnumerable<T>) components)
        {
          ReleaseManagementSecurityInfo securityInfo = getSecurityInfo(component);
          if (securityInfo == null)
            throw new ArgumentNullException("securityInfo");
          if (component is ReleaseManagementSecuredObject managementSecuredObject)
            managementSecuredObject.SetSecuredObject(securityInfo.GetToken(), (int) securityInfo.Permission);
        }
        return components;
      }
    }

    public static string GetFolderPath(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseDefinitionId)
    {
      return ReleaseManagementSecurityProcessor.GetFolderPaths(requestContext, projectId, (IList<int>) new List<int>()
      {
        releaseDefinitionId
      })[releaseDefinitionId];
    }

    public static IDictionary<int, string> GetFolderPaths(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> releaseDefinitionIds)
    {
      if (releaseDefinitionIds == null)
        throw new ArgumentNullException(nameof (releaseDefinitionIds));
      return requestContext.GetService<ReleaseDefinitionsService>().GetFolderPaths(requestContext, projectId, (IEnumerable<int>) releaseDefinitionIds);
    }

    public static ReleaseManagementSecurityInfo GetSecurityInfo(
      Guid projectId,
      string folderPath,
      int releaseDefinitionId,
      ReleaseManagementSecurityPermissions permission)
    {
      return new ReleaseManagementSecurityInfo()
      {
        ProjectId = projectId,
        Path = folderPath,
        ReleaseDefinitionId = releaseDefinitionId,
        Permission = permission
      };
    }

    private static ReleaseManagementSecurityPermissions GetBuildServiceIdentityPermission(
      IVssRequestContext requestContext)
    {
      ReleaseManagementSecurityPermissions identityPermission = ReleaseManagementSecurityPermissions.ViewReleaseDefinition | ReleaseManagementSecurityPermissions.ViewReleases | ReleaseManagementSecurityPermissions.ManageTaskHubExtension;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.ManageTaskHubExtensionPermissions"))
        identityPermission = ReleaseManagementSecurityPermissions.ViewReleaseDefinition | ReleaseManagementSecurityPermissions.ManageReleases | ReleaseManagementSecurityPermissions.ViewReleases | ReleaseManagementSecurityPermissions.EditReleaseEnvironment;
      return identityPermission;
    }

    private static string GetProjectFaultInRegistryKey(Guid projectId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/ReleaseManagement/FaultIn/{0}", (object) projectId);

    private static bool ShouldInitializePermissions(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      if (projectId.Equals(Guid.Empty))
        return false;
      if (!requestContext.IsRequiredDataspaceAvailable(projectId))
        return true;
      string faultInRegistryKey = ReleaseManagementSecurityProcessor.GetProjectFaultInRegistryKey(projectId);
      return requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) faultInRegistryKey, false);
    }

    private static T GetParameterValue<T>(
      Dictionary<string, object> argDictionary,
      string searchParameterName)
    {
      string parameterName = searchParameterName.ToUpperInvariant();
      List<string> list = argDictionary.Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (kvp => kvp.Key)).ToList<string>();
      if (!list.Any<string>((Func<string, bool>) (paramName => paramName.ToUpperInvariant().Equals(parameterName))))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ParameterByNameNotFound, (object) parameterName, (object) list));
      if (argDictionary.Single<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => kvp.Key.ToUpperInvariant().Equals(parameterName))).Value is T parameterValue)
        return parameterValue;
      if (!typeof (T).IsPrimitive)
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.NonPrimitiveParameterNotConvertibleError, (object) searchParameterName, (object) typeof (T).Name));
      throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ParameterNotConvertibleError, (object) searchParameterName, (object) typeof (T).Name));
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static bool TryCreateReleaseAdministratorsGroup(
      IVssRequestContext requestContext,
      IdentityService identityService,
      Guid projectId,
      out Microsoft.VisualStudio.Services.Identity.Identity releaseAdministratorsGroup)
    {
      using (ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseManagementSecurityProcessor.TryCreateReleaseAdministratorsGroup", 1961018))
      {
        IVssRequestContext requestContext1 = requestContext.Elevate();
        try
        {
          releaseAdministratorsGroup = identityService.CreateGroup(requestContext1, projectId, (string) null, SecurityConstants.ReleaseAdministratorsGroup, Resources.ReleaseAdministratorsGroupDescription);
        }
        catch (GroupCreationException ex)
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> source = identityService.ListGroups(requestContext1, new Guid[1]
          {
            projectId
          }, false, (IEnumerable<string>) null);
          releaseAdministratorsGroup = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group => group.GetProperty<string>("Account", string.Empty).Equals(SecurityConstants.ReleaseAdministratorsGroup, StringComparison.OrdinalIgnoreCase)));
        }
        catch (Exception ex) when (ex is GroupScopeDoesNotExistException || ex is AccessCheckException)
        {
          releaseAdministratorsGroup = (Microsoft.VisualStudio.Services.Identity.Identity) null;
          requestContext.Trace(1976412, TraceLevel.Error, "ReleaseManagementService", "ReleaseManagementLayer", "Cannot create release administrator group as the project {0} got deleted, Exception: {1}", (object) projectId, (object) ex);
          throw new ReleaseManagementException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ProjectDoesNotExists, (object) projectId));
        }
        return releaseAdministratorsGroup != null;
      }
    }

    private static void EnsureReleaseAdministratorPermissionOnRemoteNamespaces(
      IVssRequestContext requestContext,
      Guid projectId,
      IdentityDescriptor releaseAdministratorsDescriptor)
    {
      ReleaseManagementSecurityProcessor.EnsureReleaseAdministratorMetaTaskPermissions(requestContext, projectId, releaseAdministratorsDescriptor);
      ReleaseManagementSecurityProcessor.EnsureReleaseAdministratorQueuePermissions(requestContext, projectId, releaseAdministratorsDescriptor);
      ReleaseManagementSecurityProcessor.EnsureReleaseAdministratorMachineGroupPermissions(requestContext, projectId, releaseAdministratorsDescriptor);
      ReleaseManagementSecurityProcessor.EnsureReleaseAdministratorLibraryPermissions(requestContext, projectId, releaseAdministratorsDescriptor);
    }

    private static bool TryAddReleaseAdministratorsGroupPermission(
      IVssRequestContext requestContext,
      IdentityService identityService,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> existingGroups,
      Guid projectId,
      IList<IAccessControlEntry> aces)
    {
      using (ReleaseManagementTimer.Create(requestContext, "Service", "ReleaseManagementSecurityProcessor.TryAddReleaseAdministratorsGroupPermission", 1961019))
      {
        Microsoft.VisualStudio.Services.Identity.Identity releaseAdministratorsGroup = existingGroups.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group =>
        {
          string property = group.GetProperty<string>("Account", string.Empty);
          return property.Equals(SecurityConstants.ReleaseAdministratorsGroup, StringComparison.OrdinalIgnoreCase) || property.Equals(SecurityConstants.ReleaseManagersGroupName, StringComparison.OrdinalIgnoreCase);
        }));
        if (releaseAdministratorsGroup != null || ReleaseManagementSecurityProcessor.TryCreateReleaseAdministratorsGroup(requestContext, identityService, projectId, out releaseAdministratorsGroup))
        {
          ReleaseManagementSecurityProcessor.EnsureReleaseAdministratorPermissionFunc(requestContext, projectId, releaseAdministratorsGroup.Descriptor);
          aces.Add((IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(releaseAdministratorsGroup.Descriptor, 8191, 0));
        }
        return releaseAdministratorsGroup != null;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Wanted to catch any failure occurred")]
    private static void EnsureReleaseAdministratorQueuePermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      IdentityDescriptor descriptor)
    {
      IVssRequestContext context = requestContext.Elevate();
      try
      {
        SecurityHttpClient securityClient = context.GetClient<SecurityHttpClient>(ServiceInstanceTypes.TFS);
        Microsoft.VisualStudio.Services.Security.AccessControlEntry[] acccesControlEntries = new Microsoft.VisualStudio.Services.Security.AccessControlEntry[1]
        {
          new Microsoft.VisualStudio.Services.Security.AccessControlEntry()
          {
            Allow = 63,
            Deny = 0,
            Descriptor = descriptor
          }
        };
        Func<Task<IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>>> func = (Func<Task<IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>>>) (() => securityClient.SetAccessControlEntriesAsync(ReleaseManagementSecurityProcessor.AgentPoolNamespaceId, ReleaseManagementSecurityProcessor.GetAgentQueueToken(projectId), (IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>) acccesControlEntries, true));
        requestContext.ExecuteAsyncAndSyncResult<IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>>(func);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1961016, TraceLevel.Error, "ReleaseManagementService", "ReleaseManagementLayer", ex);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Wanted to catch any failure occurred")]
    private static void EnsureReleaseAdministratorMachineGroupPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      IdentityDescriptor descriptor)
    {
      IVssRequestContext context = requestContext.Elevate();
      try
      {
        SecurityHttpClient securityClient = context.GetClient<SecurityHttpClient>(ServiceInstanceTypes.TFS);
        Microsoft.VisualStudio.Services.Security.AccessControlEntry[] acccesControlEntries = new Microsoft.VisualStudio.Services.Security.AccessControlEntry[1]
        {
          new Microsoft.VisualStudio.Services.Security.AccessControlEntry()
          {
            Allow = 59,
            Deny = 0,
            Descriptor = descriptor
          }
        };
        Func<Task<IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>>> func = (Func<Task<IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>>>) (() => securityClient.SetAccessControlEntriesAsync(ReleaseManagementSecurityProcessor.AgentPoolNamespaceId, ReleaseManagementSecurityProcessor.GetMachineGroupToken(projectId), (IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>) acccesControlEntries, true));
        requestContext.ExecuteAsyncAndSyncResult<IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>>(func);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1961017, TraceLevel.Error, "ReleaseManagementService", "ReleaseManagementLayer", ex);
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Wanted to catch any failure occurred")]
    private static void EnsureReleaseAdministratorLibraryPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      IdentityDescriptor descriptor)
    {
      IVssRequestContext context = requestContext.Elevate();
      try
      {
        Func<Task<List<VariableGroup>>> func1 = (Func<Task<List<VariableGroup>>>) (() => requestContext.GetClient<TaskAgentHttpClient>().GetVariableGroupsAsync(projectId));
        requestContext.ExecuteAsyncAndSyncResult<List<VariableGroup>>(func1);
        SecurityHttpClient securityClient = context.GetClient<SecurityHttpClient>(ServiceInstanceTypes.TFS);
        Microsoft.VisualStudio.Services.Security.AccessControlEntry[] aces = new Microsoft.VisualStudio.Services.Security.AccessControlEntry[1]
        {
          new Microsoft.VisualStudio.Services.Security.AccessControlEntry()
          {
            Allow = 19,
            Deny = 0,
            Descriptor = descriptor
          }
        };
        Func<Task<IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>>> func2 = (Func<Task<IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>>>) (() => securityClient.SetAccessControlEntriesAsync(ReleaseManagementSecurityProcessor.LibraryNamespaceId, ReleaseManagementSecurityProcessor.GetLibraryToken(projectId), (IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>) aces, true));
        requestContext.ExecuteAsyncAndSyncResult<IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>>(func2);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1961030, TraceLevel.Error, "ReleaseManagementService", "ReleaseManagementLayer", ex);
      }
    }

    private static string GetMachineGroupToken(Guid projectId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) ReleaseManagementSecurityProcessor.MachineGroupToken, (object) projectId);

    private static string GetAgentQueueToken(Guid projectId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) ReleaseManagementSecurityProcessor.AgentQueueToken, (object) projectId);

    private static string GetLibraryToken(Guid projectId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) ReleaseManagementSecurityProcessor.LibraryToken, (object) projectId);

    private static void AddDefaultGroupsPermissions(
      IVssRequestContext requestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> groups,
      IList<IAccessControlEntry> aces)
    {
      Dictionary<string, List<string>> localizedGroupNames = ReleaseManagementSecurityProcessor.GetLocalizedGroupNames(requestContext);
      List<Tuple<List<string>, int, int>> tupleList = new List<Tuple<List<string>, int, int>>();
      KeyValuePair<string, List<string>> keyValuePair = localizedGroupNames.FirstOrDefault<KeyValuePair<string, List<string>>>((Func<KeyValuePair<string, List<string>>, bool>) (x => x.Key.Equals("Contributors")));
      tupleList.Add(new Tuple<List<string>, int, int>(keyValuePair.Value, 3583, 0));
      keyValuePair = localizedGroupNames.FirstOrDefault<KeyValuePair<string, List<string>>>((Func<KeyValuePair<string, List<string>>, bool>) (x => x.Key.Equals("Readers")));
      tupleList.Add(new Tuple<List<string>, int, int>(keyValuePair.Value, 33, 0));
      keyValuePair = localizedGroupNames.FirstOrDefault<KeyValuePair<string, List<string>>>((Func<KeyValuePair<string, List<string>>, bool>) (x => x.Key.Equals("Project Administrators")));
      tupleList.Add(new Tuple<List<string>, int, int>(keyValuePair.Value, 8191, 0));
      foreach (Tuple<List<string>, int, int> tuple in tupleList)
      {
        Tuple<List<string>, int, int> groupPermission = tuple;
        Microsoft.VisualStudio.Services.Identity.Identity identity = groups.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group =>
        {
          string accountName = group.GetProperty<string>("Account", string.Empty);
          return groupPermission.Item1.Any<string>((Func<string, bool>) (item => item.Equals(accountName, StringComparison.OrdinalIgnoreCase)));
        }));
        if (identity != null)
          aces.Add((IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(identity.Descriptor, groupPermission.Item2, groupPermission.Item3));
      }
    }

    private static Dictionary<string, List<string>> GetLocalizedGroupNames(
      IVssRequestContext requestContext)
    {
      List<string> stringList1 = new List<string>((IEnumerable<string>) LocalizedGroupNames.Readers.Values);
      List<string> stringList2 = new List<string>((IEnumerable<string>) LocalizedGroupNames.Contributors.Values);
      List<string> stringList3 = new List<string>((IEnumerable<string>) LocalizedGroupNames.ProjectAdministrators.Values);
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        using (SafeHandle registryKey = RegistryHelper.OpenSubKey(RegistryHive.LocalMachine, "Software\\Microsoft\\TeamFoundationServer\\19.0\\InstalledComponents\\ApplicationTier", RegistryAccessMask.Execute))
        {
          if (registryKey != null)
          {
            string path1 = (string) RegistryHelper.GetValue(registryKey, "InstallPath", (object) null);
            if (!string.IsNullOrWhiteSpace(path1))
            {
              string str = Path.Combine(path1, "Web Services\\bin\\ReleaseManagementSecurityGroups.xml");
              if (File.Exists(str))
              {
                try
                {
                  XDocument xdocument = XDocument.Load(str);
                  if (xdocument.Root == null)
                    throw new ReleaseManagementException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.RootElementNotFound));
                  foreach (XElement element in xdocument.Root.Elements((XName) "language"))
                  {
                    stringList1.Add(element.Attribute((XName) "readers").Value);
                    stringList2.Add(element.Attribute((XName) "contributors").Value);
                    stringList3.Add(element.Attribute((XName) "projectAdministrators").Value);
                  }
                }
                catch (XmlException ex)
                {
                  throw new ReleaseManagementException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SecurityGroupXmlInvalidError, (object) ex.Message));
                }
                catch (NullReferenceException ex)
                {
                  throw new ReleaseManagementException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SecurityGroupXmlAttributeInvalidError, (object) ex.Message));
                }
              }
            }
          }
        }
      }
      return new Dictionary<string, List<string>>()
      {
        {
          "Readers",
          stringList1
        },
        {
          "Contributors",
          stringList2
        },
        {
          "Project Administrators",
          stringList3
        }
      };
    }

    private static void AddBuildServicePermissions(
      IList<IAccessControlEntry> aces,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> serviceIdentities,
      ReleaseManagementSecurityPermissions permissions)
    {
      foreach (Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) serviceIdentities)
      {
        if (serviceIdentity != null)
          aces.Add((IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(serviceIdentity.Descriptor, (int) permissions, 0));
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This log is not read by the customer")]
    private static List<Microsoft.VisualStudio.Services.Identity.Identity> GetBuildServiceIdentities(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> serviceIdentities = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity = ServiceIdentityProvisioner.GetServiceIdentity(requestContext, ReleaseAuthorizationScope.ProjectCollection, Guid.Empty, true);
        serviceIdentities.Add(serviceIdentity);
      }
      catch (ReleaseManagementServiceException ex)
      {
        requestContext.Trace(1976488, TraceLevel.Error, "ReleaseManagementService", "Service", "Cannot find build service identity at collection scope");
      }
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity serviceIdentity = ServiceIdentityProvisioner.GetServiceIdentity(requestContext, ReleaseAuthorizationScope.Project, projectId, true);
        serviceIdentities.Add(serviceIdentity);
      }
      catch (ReleaseManagementServiceException ex)
      {
        requestContext.Trace(1976488, TraceLevel.Error, "ReleaseManagementService", "Service", "Cannot find build service identity at project scope {0}", (object) projectId);
      }
      return serviceIdentities;
    }

    private static void AddWellKnownIdentitiesPermissions(
      IVssRequestContext requestContext,
      IList<IAccessControlEntry> aces)
    {
      List<IdentityDescriptor> descriptors = new List<IdentityDescriptor>()
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
      };
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) descriptors, QueryMembership.None, (IEnumerable<string>) null);
      if (identityList == null)
        return;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList)
      {
        if (identity != null)
          aces.Add((IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(identity.Descriptor, 8191, 0));
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Validation is done by CheckkForNull")]
    private static KeyValuePair<int, string> GetReleaseDefinitionFolderPathAndId(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.GetService<ReleasesService>().GetReleaseDefinitionFolderPathAndId(requestContext, projectId, releaseId);
    }

    private static void EnsureReleaseAdministratorMetaTaskPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      IdentityDescriptor releaseAdminDescriptor)
    {
      Func<Task<List<TaskGroup>>> func = (Func<Task<List<TaskGroup>>>) (() => requestContext.GetClient<TaskAgentHttpClient>().GetTaskGroupsAsync(projectId, new Guid?(), new bool?(), new Guid?(), new bool?(), new int?(), new DateTime?(), new TaskGroupQueryOrder?(), (object) null, new CancellationToken()));
      requestContext.ExecuteAsyncAndGetResult<List<TaskGroup>>(func);
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext.Elevate(), ReleaseManagementSecurityProcessor.MetaTaskNamespaceId);
      if (securityNamespace == null)
        return;
      List<IAccessControlEntry> accessControlEntryList = new List<IAccessControlEntry>()
      {
        (IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(releaseAdminDescriptor, 8191, 0)
      };
      securityNamespace.SetAccessControlEntries(requestContext.Elevate(), projectId.ToString(), (IEnumerable<IAccessControlEntry>) accessControlEntryList, true, false);
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    private static void SetSecurityTokenCache(
      this IVssRequestContext requestContext,
      string securityToken,
      ReleaseManagementSecurityPermissions permissions)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (permissions == ReleaseManagementSecurityPermissions.None)
      {
        requestContext.Trace(1900000, TraceLevel.Error, "ReleaseManagementService", "ReleaseManagementLayer", "None permission should not be cached : {0}", (object) new StackTrace());
        throw new InvalidOperationException("None permission should not be cached");
      }
      if (requestContext.Items.ContainsKey("releaseManagementSecurityTokenKey") || requestContext.Items.ContainsKey("releaseManagementSecurityPermissionKey"))
      {
        requestContext.Trace(1900000, TraceLevel.Error, "ReleaseManagementService", "ReleaseManagementLayer", "Security cache can not be overwritten : {0}", (object) new StackTrace());
        throw new InvalidOperationException("Security cache can not be overwritten");
      }
      requestContext.Items["releaseManagementSecurityTokenKey"] = (object) securityToken;
      requestContext.Items["releaseManagementSecurityPermissionKey"] = (object) permissions;
    }

    [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "This is required to mock the servicing logger")]
    public delegate void ServicingLogger(ServicingStepLogEntryKind logEntryKind, string message);
  }
}
