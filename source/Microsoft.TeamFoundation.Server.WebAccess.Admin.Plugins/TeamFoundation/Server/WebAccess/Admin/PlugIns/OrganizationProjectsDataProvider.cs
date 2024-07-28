// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.OrganizationProjectsDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class OrganizationProjectsDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.OrganizationProjects";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      int topValue;
      int skipValue;
      OrganizationProjectsDataProvider.GetOrganizationProjectsParams(providerContext, out topValue, out skipValue);
      IEnumerable<ProjectInfo> projectInfos1 = requestContext.GetService<IProjectService>().GetProjects(requestContext, ProjectState.WellFormed).Skip<ProjectInfo>(skipValue);
      if (topValue != -1)
        projectInfos1 = projectInfos1.Take<ProjectInfo>(topValue);
      IEnumerable<ProjectInfo> projectInfos2 = projectInfos1.PopulateProperties(requestContext, TeamConstants.DefaultTeamPropertyName, ProcessTemplateIdPropertyNames.CurrentProcessTemplateId, ProcessTemplateIdPropertyNames.ProcessTemplateType);
      IEnumerable<ProjectInfo> projectInfos3 = requestContext.GetService<PlatformProjectService>().GetSoftDeletedProjects(requestContext).Skip<ProjectInfo>(skipValue);
      if (topValue != -1)
        projectInfos3 = projectInfos3.Take<ProjectInfo>(topValue);
      IEnumerable<ProjectInfo> projectInfos4 = projectInfos3.PopulateProperties(requestContext, TeamConstants.DefaultTeamPropertyName, ProcessTemplateIdPropertyNames.CurrentProcessTemplateId, ProcessTemplateIdPropertyNames.ProcessTemplateType, "System.SoftDeletedProjectName");
      IClientLocationProviderService service = requestContext.GetService<IClientLocationProviderService>();
      service.AddLocation(requestContext, providerContext.SharedData, ServiceInstanceTypes.SPS, new TeamFoundationHostType?(TeamFoundationHostType.ProjectCollection));
      service.AddLocation(requestContext, providerContext.SharedData, ServiceInstanceTypes.SPS, new TeamFoundationHostType?(TeamFoundationHostType.Application));
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
      List<OrganizationProjectInfo> organizationProjectInfoList1 = new List<OrganizationProjectInfo>();
      List<OrganizationProjectInfo> organizationProjectInfoList2 = new List<OrganizationProjectInfo>();
      List<string> tokens1 = new List<string>();
      List<string> tokens2 = new List<string>();
      try
      {
        foreach (ProjectInfo projectInfo in projectInfos2)
          tokens1.Add(securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, projectInfo.Uri));
        IEnumerator<bool> enumerator1 = securityNamespace.HasPermission(requestContext, (IEnumerable<string>) tokens1, TeamProjectPermissions.Delete).GetEnumerator();
        IEnumerator<bool> enumerator2 = securityNamespace.HasPermission(requestContext, (IEnumerable<string>) tokens1, TeamProjectPermissions.Rename).GetEnumerator();
        foreach (ProjectInfo projectInfo in projectInfos2)
        {
          enumerator1.MoveNext();
          enumerator2.MoveNext();
          OrganizationProjectInfo organizationProjectInfo = new OrganizationProjectInfo()
          {
            ProjectId = projectInfo.Id,
            HasDeletePermission = enumerator1.Current,
            HasRenamePermission = enumerator2.Current,
            ProcessTemplateName = OrganizationProjectsDataProvider.GetProcessName(requestContext, projectInfo),
            TeamProjectReference = projectInfo.ToTeamProjectReference(requestContext, true)
          };
          organizationProjectInfoList1.Add(organizationProjectInfo);
        }
        foreach (ProjectInfo projectInfo in projectInfos4)
          tokens2.Add(securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, projectInfo.Uri));
        IEnumerator<bool> enumerator3 = securityNamespace.HasPermission(requestContext, (IEnumerable<string>) tokens2, TeamProjectPermissions.Delete).GetEnumerator();
        IEnumerator<bool> enumerator4 = securityNamespace.HasPermission(requestContext, (IEnumerable<string>) tokens2, TeamProjectPermissions.Rename).GetEnumerator();
        foreach (ProjectInfo projectInfo in projectInfos4)
        {
          enumerator3.MoveNext();
          enumerator4.MoveNext();
          ProjectProperty projectProperty = projectInfo.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (prop => prop.Name.Equals("System.SoftDeletedProjectName", StringComparison.OrdinalIgnoreCase)));
          TeamProjectReference projectReference = projectInfo.ToTeamProjectReference(requestContext, true);
          if (projectProperty != null)
            projectReference.Name = projectProperty.Value.ToString();
          OrganizationProjectInfo organizationProjectInfo = new OrganizationProjectInfo()
          {
            ProjectId = projectInfo.Id,
            HasDeletePermission = enumerator3.Current,
            HasRenamePermission = enumerator4.Current,
            ProcessTemplateName = OrganizationProjectsDataProvider.GetProcessName(requestContext, projectInfo),
            TeamProjectReference = projectReference
          };
          organizationProjectInfoList2.Add(organizationProjectInfo);
        }
      }
      catch (Exception ex)
      {
        organizationProjectInfoList1 = (List<OrganizationProjectInfo>) null;
        organizationProjectInfoList2 = (List<OrganizationProjectInfo>) null;
        requestContext.TraceException(10050084, "OrgSettingsProjects", "DataProvider", ex);
      }
      return (object) new OrganizationProjectsData()
      {
        Projects = organizationProjectInfoList1,
        DeletedProjects = organizationProjectInfoList2
      };
    }

    private static string GetProcessName(IVssRequestContext requestContext, ProjectInfo project)
    {
      ProjectProperty projectProperty = project.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (property => property.Name.Equals(ProcessTemplateIdPropertyNames.ProcessTemplateType, StringComparison.OrdinalIgnoreCase)));
      Guid result;
      ProcessDescriptor descriptor;
      return project.Properties != null && projectProperty != null && Guid.TryParse((string) projectProperty.Value, out result) && requestContext.GetService<ITeamFoundationProcessService>().TryGetProcessDescriptor(requestContext, result, out descriptor) ? descriptor.Name : string.Empty;
    }

    private static void GetOrganizationProjectsParams(
      DataProviderContext providerContext,
      out int topValue,
      out int skipValue)
    {
      int? nullable1 = new int?();
      int? nullable2 = new int?();
      if (providerContext.Properties.ContainsKey("top") && providerContext.Properties["top"] != null)
        nullable1 = new int?(Convert.ToInt32(providerContext.Properties["top"]));
      if (providerContext.Properties.ContainsKey("skip") && providerContext.Properties["skip"] != null)
        nullable2 = new int?(Convert.ToInt32(providerContext.Properties["skip"]));
      ref int local1 = ref topValue;
      int? nullable3;
      int num1;
      if (nullable1.HasValue)
      {
        nullable3 = nullable1;
        int num2 = 1;
        if (!(nullable3.GetValueOrDefault() < num2 & nullable3.HasValue))
        {
          num1 = nullable1.Value;
          goto label_8;
        }
      }
      num1 = -1;
label_8:
      local1 = num1;
      ref int local2 = ref skipValue;
      int num3;
      if (nullable2.HasValue)
      {
        nullable3 = nullable2;
        int num4 = 0;
        if (!(nullable3.GetValueOrDefault() < num4 & nullable3.HasValue))
        {
          num3 = nullable2.Value;
          goto label_12;
        }
      }
      num3 = 0;
label_12:
      local2 = num3;
    }

    private static class Constants
    {
      public const int TopMin = 1;
      public const int SkipMin = 0;
      public const int DefaultTop = -1;
      public const int DefaultSkip = 0;
    }
  }
}
