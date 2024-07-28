// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.VersionControlProcessTemplateUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class VersionControlProcessTemplateUtility
  {
    public static TeamProjectFolderPermission[] GetTfvcProjectFolderPermissions(
      IVssRequestContext requestContext,
      ProjectInfo project,
      bool defaultToProjectAdmin = false)
    {
      try
      {
        requestContext.TraceEnter(513210, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetTfvcProjectFolderPermissions));
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
        List<TeamProjectFolderPermission> folderPermissionList = new List<TeamProjectFolderPermission>();
        XmlDocument versionControlProcess = VersionControlProcessTemplateUtility.GetVersionControlProcess(requestContext, project);
        if (versionControlProcess != null)
        {
          foreach (XmlNode selectNode in versionControlProcess.DocumentElement.SelectNodes("//tasks/task[@id='VersionControlTask']/taskXml/permission"))
          {
            string csv1 = selectNode.Attributes["allow"]?.Value ?? string.Empty;
            string csv2 = selectNode.Attributes["denies"]?.Value ?? string.Empty;
            string identity = selectNode.Attributes["identity"]?.Value;
            string identityName;
            if (VersionControlProcessTemplateUtility.TryParseIdentity(requestContext, identity, project, out identityName))
            {
              TeamProjectFolderPermission folderPermission = new TeamProjectFolderPermission()
              {
                AllowPermission = VersionControlProcessTemplateUtility.SplitAndTrim(csv1),
                DenyPermission = VersionControlProcessTemplateUtility.SplitAndTrim(csv2),
                IdentityName = identityName
              };
              folderPermissionList.Add(folderPermission);
            }
          }
        }
        if (folderPermissionList.Count < 1 & defaultToProjectAdmin)
        {
          requestContext.Trace(513211, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Process template permissions could not be retrieved.  Default to adding the Project Admin Group with TFVC folder allow All permissions for project: {0} (projectId: {1})", (object) project.Name, (object) project.Id);
          TeamProjectFolderPermission folderPermission = new TeamProjectFolderPermission()
          {
            IdentityName = VersionControlProcessTemplateUtility.GetProjectAdminGroupIdentity(requestContext, project.Uri).UniqueName,
            AllowPermission = new string[1]{ "All" }
          };
          folderPermissionList.Add(folderPermission);
        }
        return folderPermissionList.ToArray();
      }
      finally
      {
        requestContext.TraceLeave(513212, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetTfvcProjectFolderPermissions));
      }
    }

    public static List<AccessControlEntry> GetGitRepositoryPermissions(
      IVssRequestContext requestContext,
      ProjectInfo project,
      bool defaultToProjectAdmin = false)
    {
      try
      {
        requestContext.TraceEnter(513215, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetGitRepositoryPermissions));
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
        List<AccessControlEntry> aces = new List<AccessControlEntry>();
        XmlDocument versionControlProcess = VersionControlProcessTemplateUtility.GetVersionControlProcess(requestContext, project);
        if (versionControlProcess != null)
        {
          foreach (XmlNode selectNode in versionControlProcess.DocumentElement.SelectNodes("//tasks/task[@id='VersionControlTask']/taskXml/git/permission"))
          {
            string csv1 = selectNode.Attributes["allow"]?.Value ?? string.Empty;
            string csv2 = selectNode.Attributes["denies"]?.Value ?? string.Empty;
            string identity = selectNode.Attributes["identity"]?.Value;
            string identityName;
            if (VersionControlProcessTemplateUtility.TryParseIdentity(requestContext, identity, project, out identityName))
            {
              string[] array1 = VersionControlProcessTemplateUtility.SplitAndTrim(csv1);
              string[] array2 = VersionControlProcessTemplateUtility.SplitAndTrim(csv2);
              aces.Add(new AccessControlEntry()
              {
                Descriptor = new IdentityDescriptor("Microsoft.TeamFoundation.UnauthenticatedIdentity", identityName),
                Allow = VersionControlUtil.TranslatePermission(typeof (GitRepositoryPermissions), array1, 524286),
                Deny = VersionControlUtil.TranslatePermission(typeof (GitRepositoryPermissions), array2, 524286)
              });
            }
          }
        }
        if (aces.Count < 1 & defaultToProjectAdmin)
        {
          requestContext.Trace(513216, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Process template permissions could not be retrieved.  Default to adding the Project Admin Group with Git repository allow All permissions for project: {0} (projectId: {1})", (object) project.Name, (object) project.Id);
          AccessControlEntry accessControlEntry = new AccessControlEntry()
          {
            Descriptor = VersionControlProcessTemplateUtility.GetProjectAdminGroupIdentity(requestContext, project.Uri).Descriptor,
            Allow = 491382,
            Deny = 0
          };
          aces.Add(accessControlEntry);
        }
        GitServerUtils.TranslateLegacyPermissionsToCurrentPermissions((IEnumerable<IAccessControlEntry>) aces);
        return aces;
      }
      finally
      {
        requestContext.TraceLeave(513217, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetGitRepositoryPermissions));
      }
    }

    private static XmlDocument GetVersionControlProcess(
      IVssRequestContext requestContext,
      ProjectInfo project)
    {
      IProcessTemplate processTemplate = (IProcessTemplate) null;
      XmlDocument vcProcessXml = (XmlDocument) null;
      if (VersionControlProcessTemplateUtility.TryGetActiveProcessTemplate(requestContext, project, out processTemplate) && VersionControlProcessTemplateUtility.TryGetVersionControlProcessXml(processTemplate, out vcProcessXml))
      {
        requestContext.Trace(513220, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Successfully retrieved the version control process XML from the project's active process template of type: {0} for project: {1} (projectId: {2})", (object) processTemplate.Descriptor.TypeId, (object) project.Name, (object) project.Id);
        return vcProcessXml;
      }
      if (!VersionControlProcessTemplateUtility.TryGetProcessTemplate(requestContext, ProcessTemplateTypeIdentifiers.MsfAgileSoftwareDevelopment, out processTemplate) || !VersionControlProcessTemplateUtility.TryGetVersionControlProcessXml(processTemplate, out vcProcessXml))
        return vcProcessXml;
      requestContext.Trace(513221, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Successfully retrieved the version control process XML from the default Agile process template of type: {0} for project: {1} (projectId: {2})", (object) processTemplate.Descriptor.TypeId, (object) project.Name, (object) project.Id);
      return vcProcessXml;
    }

    private static bool TryGetActiveProcessTemplate(
      IVssRequestContext requestContext,
      ProjectInfo project,
      out IProcessTemplate processTemplate)
    {
      requestContext.Trace(513222, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Attempting to get the active process template for project: {0} (projectId: {1})", (object) project.Name, (object) project.Id);
      processTemplate = (IProcessTemplate) null;
      ProjectProperty projectProperty = project.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (prop => prop.Name.Equals(ProcessTemplateIdPropertyNames.ProcessTemplateType, StringComparison.Ordinal)));
      Guid result;
      return projectProperty != null && Guid.TryParse((string) projectProperty.Value, out result) && VersionControlProcessTemplateUtility.TryGetProcessTemplate(requestContext, result, out processTemplate);
    }

    private static bool TryGetProcessTemplate(
      IVssRequestContext requestContext,
      Guid processTemplateType,
      out IProcessTemplate processTemplate)
    {
      requestContext.Trace(513223, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "Attempting to get the process template of type: {0}", (object) processTemplateType);
      processTemplate = (IProcessTemplate) null;
      ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
      ProcessDescriptor descriptor;
      if (!service.TryGetProcessDescriptor(requestContext, processTemplateType, out descriptor))
        return false;
      processTemplate = service.GetLegacyProcess(requestContext, descriptor);
      return true;
    }

    private static bool TryGetVersionControlProcessXml(
      IProcessTemplate template,
      out XmlDocument vcProcessXml)
    {
      vcProcessXml = (XmlDocument) null;
      using (StreamReader streamReader1 = new StreamReader(template.GetResource("ProcessTemplate.xml")))
      {
        XmlNode xmlNode = XmlUtility.GetDocument(streamReader1.ReadToEnd()).DocumentElement.SelectSingleNode("//groups/group[@id='VersionControl']/taskList/@filename");
        if (xmlNode != null)
        {
          using (StreamReader streamReader2 = new StreamReader(template.GetResource(xmlNode.Value)))
          {
            vcProcessXml = XmlUtility.GetDocument(streamReader2.ReadToEnd());
            return true;
          }
        }
      }
      return false;
    }

    private static bool TryParseIdentity(
      IVssRequestContext requestContext,
      string identity,
      ProjectInfo project,
      out string identityName)
    {
      requestContext.Trace(513224, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "TryParseIdentity for identity: {0} for project: {1} (projectId: {2})", (object) identity, (object) project.Name, (object) project.Id);
      identityName = (string) null;
      string[] source = identity.Split(new string[1]{ "\\" }, StringSplitOptions.RemoveEmptyEntries);
      if (2 == source.Length)
      {
        string strB1 = ((IEnumerable<string>) source).First<string>().TrimStart('[').TrimEnd(']');
        string strB2 = ((IEnumerable<string>) source).Last<string>();
        ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
        if (string.CompareOrdinal("$$PROJECTADMINGROUP$$", strB2) == 0 && (string.CompareOrdinal("$$PROJECTNAME$$", strB1) == 0 || string.CompareOrdinal(project.Name, strB1) == 0))
        {
          TeamFoundationIdentity foundationIdentity = service.ReadIdentity(requestContext, IdentitySearchFactor.AdministratorsGroup, project.Uri);
          if (foundationIdentity != null)
          {
            identityName = foundationIdentity.DisplayName;
            return true;
          }
        }
        if (string.CompareOrdinal("$$PROJECTNAME$$", strB1) == 0)
        {
          identity = identity.Replace("$$PROJECTNAME$$", project.Name);
          TeamFoundationIdentity foundationIdentity = service.ReadIdentity(requestContext, IdentitySearchFactor.AccountName, identity, MembershipQuery.None, ReadIdentityOptions.IncludeReadFromSource, (IEnumerable<string>) null);
          if (foundationIdentity != null)
          {
            identityName = foundationIdentity.DisplayName;
            return true;
          }
        }
      }
      requestContext.Trace(513225, TraceLevel.Verbose, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, "TryParseIdentity failed for identity: {0} for project: {1} (projectId: {2})", (object) identity, (object) project.Name, (object) project.Id);
      return false;
    }

    private static string[] SplitAndTrim(string csv) => ((IEnumerable<string>) csv.Split(new string[1]
    {
      ","
    }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (x => x.Trim())).ToArray<string>();

    private static TeamFoundationIdentity GetProjectAdminGroupIdentity(
      IVssRequestContext requestContext,
      string teamProjectUri)
    {
      return requestContext.GetService<ITeamFoundationIdentityService>().ReadIdentity(requestContext, IdentitySearchFactor.AdministratorsGroup, teamProjectUri) ?? throw new Microsoft.VisualStudio.Services.Identity.IdentityNotFoundException("Couldn't read the AdministratorsGroup identity for the team project:" + teamProjectUri + ".");
    }
  }
}
