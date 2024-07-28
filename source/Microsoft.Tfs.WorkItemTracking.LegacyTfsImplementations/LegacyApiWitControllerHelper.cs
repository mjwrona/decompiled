// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations.LegacyApiWitControllerHelper
// Assembly: Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6D9A1E77-52F6-4366-807D-D0FABA8CDE81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Core.Catalog.Objects;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Directories;
using Microsoft.VisualStudio.Services.Directories.DirectoryService;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations
{
  public class LegacyApiWitControllerHelper : ILegacyApiWitControllerHelper, IVssFrameworkService
  {
    private readonly IReadOnlyDictionary<string, object> CommonEntityDescriptorProperties = (IReadOnlyDictionary<string, object>) new Dictionary<string, object>()
    {
      {
        "InvitationMethod",
        (object) "MailIdentityHelper"
      },
      {
        "RootWithActiveMembership",
        (object) true
      }
    };

    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public JsObject GetTeamProjectsInternal(
      IVssRequestContext requestContext,
      IEnumerable<object> objectProjects,
      bool includeFieldDefinition = false,
      bool includeProcessInfo = true,
      bool includeWorkItemTypes = true)
    {
      CatalogBulkData bulkData = (CatalogBulkData) null;
      IEnumerable<Project> source1 = objectProjects.Select<object, Project>((Func<object, Project>) (proj => (Project) proj));
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        bulkData = this.GetCatalogBulkData(requestContext);
      ILookup<int, IWorkItemType> workItemTypesLookupByProject = (ILookup<int, IWorkItemType>) null;
      if (includeWorkItemTypes | includeFieldDefinition)
        workItemTypesLookupByProject = requestContext.GetService<WebAccessWorkItemService>().GetWorkItemTypes(requestContext, source1.Select<Project, Guid>((Func<Project, Guid>) (p => p.Guid))).ToLookup<IWorkItemType, int>((Func<IWorkItemType, int>) (wit => wit.ProjectId));
      bool addFieldEnabled = EtagHelper.IsAddFieldEnabled(requestContext);
      List<FieldDefinition> fieldDefinitions = new List<FieldDefinition>();
      Func<Project, JsObject> ProjectToJson = (Func<Project, JsObject>) (project =>
      {
        IEnumerable<IWorkItemType> source2 = (IEnumerable<IWorkItemType>) null;
        IEnumerable<string> strings = (IEnumerable<string>) null;
        IEnumerable<int> ints = (IEnumerable<int>) null;
        if (includeWorkItemTypes | includeFieldDefinition)
          source2 = workItemTypesLookupByProject[project.Id];
        if (includeWorkItemTypes)
          strings = source2.Select<IWorkItemType, string>((Func<IWorkItemType, string>) (wit => wit.Name));
        if (includeFieldDefinition)
        {
          IEnumerable<IGrouping<int, FieldDefinition>> source3 = source2.SelectMany<IWorkItemType, FieldDefinition>((Func<IWorkItemType, IEnumerable<FieldDefinition>>) (wit => (IEnumerable<FieldDefinition>) wit.GetFields(requestContext))).GroupBy<FieldDefinition, int>((Func<FieldDefinition, int>) (fd => fd.Id));
          ints = source3.Select<IGrouping<int, FieldDefinition>, int>((Func<IGrouping<int, FieldDefinition>, int>) (fd => fd.Key));
          fieldDefinitions.AddRange((IEnumerable<FieldDefinition>) source3.Select<IGrouping<int, FieldDefinition>, FieldDefinition>((Func<IGrouping<int, FieldDefinition>, FieldDefinition>) (fd => fd.First<FieldDefinition>())).ToList<FieldDefinition>());
        }
        object obj1 = this.PopulateProjectUrls(requestContext, bulkData, project.Guid);
        Project project1 = project;
        IEnumerable<string> workItemTypeNames = strings;
        IEnumerable<int> fieldIds = ints;
        // ISSUE: reference to a compiler-generated field
        if (LegacyApiWitControllerHelper.\u003C\u003Eo__2.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          LegacyApiWitControllerHelper.\u003C\u003Eo__2.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (LegacyApiWitControllerHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target = LegacyApiWitControllerHelper.\u003C\u003Eo__2.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p1 = LegacyApiWitControllerHelper.\u003C\u003Eo__2.\u003C\u003Ep__1;
        // ISSUE: reference to a compiler-generated field
        if (LegacyApiWitControllerHelper.\u003C\u003Eo__2.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          LegacyApiWitControllerHelper.\u003C\u003Eo__2.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (LegacyApiWitControllerHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj2 = LegacyApiWitControllerHelper.\u003C\u003Eo__2.\u003C\u003Ep__0.Target((CallSite) LegacyApiWitControllerHelper.\u003C\u003Eo__2.\u003C\u003Ep__0, obj1, (object) null);
        var extras = target((CallSite) p1, obj2) ? null : new
        {
          resourceLocations = obj1
        };
        JsObject json = project1.ToJson(workItemTypeNames, fieldIds, (object) extras);
        if (includeProcessInfo)
        {
          IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
          ProcessDescriptor processDescriptor;
          if (addFieldEnabled && service.TryGetLatestProjectProcessDescriptor(requestContext, project.Guid, out processDescriptor))
            json["process"] = (object) new ProcessDescriptorModel(requestContext, processDescriptor, (ISecuredObject) null).ToJson();
        }
        return json;
      });
      List<Project> list = source1.ToList<Project>();
      list.Sort();
      JsObject projectsInternal = new JsObject();
      projectsInternal["projects"] = (object) list.Select<Project, JsObject>((Func<Project, JsObject>) (p => ProjectToJson(p)));
      if (includeFieldDefinition)
      {
        IEnumerable<JsObject> first = fieldDefinitions.GroupBy<FieldDefinition, int>((Func<FieldDefinition, int>) (fd => fd.Id)).Select<IGrouping<int, FieldDefinition>, JsObject>((Func<IGrouping<int, FieldDefinition>, JsObject>) (fd => fd.First<FieldDefinition>().ToJson()));
        IEnumerable<JsObject> ignoredCoreFields = this.GetIgnoredCoreFields(requestContext);
        projectsInternal["fields"] = first != null ? (object) first.Concat<JsObject>(ignoredCoreFields) : (object) (IEnumerable<JsObject>) null;
      }
      return projectsInternal;
    }

    public Microsoft.VisualStudio.Services.Identity.Identity[] ResolveAllIdentities(
      IVssRequestContext requestContext,
      Guid[] tfIds,
      Guid[] originIds,
      bool disallowInvitedUsers)
    {
      try
      {
        TeamFoundationIdentity[] collection = this.ReadIdentities(requestContext, tfIds);
        List<TeamFoundationIdentity> list = this.MaterializeIdentities(requestContext, originIds).ToList<TeamFoundationIdentity>();
        if (collection != null)
          list.AddRange((IEnumerable<TeamFoundationIdentity>) collection);
        if (disallowInvitedUsers && !requestContext.IsFeatureEnabled("WorkItemTracking.Server.AllowEmailingInvitedUsers"))
        {
          IEnumerable<TeamFoundationIdentity> source = list.Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (i => i.Descriptor.IsBindPendingType()));
          if (source.Any<TeamFoundationIdentity>())
            throw new InvitedUserCannotBeEmailedException(source.First<TeamFoundationIdentity>().DisplayName);
        }
        return list.Select<TeamFoundationIdentity, Microsoft.VisualStudio.Services.Identity.Identity>((Func<TeamFoundationIdentity, Microsoft.VisualStudio.Services.Identity.Identity>) (tfIdentity => IdentityUtil.Convert(tfIdentity))).ToArray<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      catch (InvitedUserCannotBeEmailedException ex)
      {
        throw;
      }
      catch (IdentityNotFoundException ex)
      {
        requestContext.TraceException(516601, "WebAccess.WorkItem", TfsTraceLayers.Controller, (Exception) ex);
        throw ex;
      }
      catch (RecipientWorkItemAccessDeniedException ex)
      {
        requestContext.TraceException(516601, "WebAccess.WorkItem", TfsTraceLayers.Controller, (Exception) ex);
        throw ex;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(516602, "WebAccess.WorkItem", TfsTraceLayers.Controller, ex);
        throw new Exception(WITResources.EmailResolveIdentityFailed());
      }
    }

    private TeamFoundationIdentity[] ReadIdentities(IVssRequestContext requestContext, Guid[] tfIds)
    {
      if (tfIds == null || ((IEnumerable<Guid>) tfIds).Count<Guid>() <= 0)
        return (TeamFoundationIdentity[]) null;
      TeamFoundationIdentity[] array = ((IEnumerable<TeamFoundationIdentity>) requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, tfIds)).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (x => x != null)).ToArray<TeamFoundationIdentity>();
      if (array.Length == tfIds.Length)
        return array;
      throw new IdentityNotFoundException();
    }

    private IReadOnlyList<TeamFoundationIdentity> MaterializeIdentities(
      IVssRequestContext requestContext,
      Guid[] originIds)
    {
      List<TeamFoundationIdentity> foundationIdentityList = new List<TeamFoundationIdentity>();
      try
      {
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment && originIds != null && ((IEnumerable<Guid>) originIds).Count<Guid>() > 0)
        {
          IVssRequestContext vssRequestContext = requestContext.Elevate();
          List<DirectoryEntityDescriptor> members = new List<DirectoryEntityDescriptor>();
          foreach (Guid originId in originIds)
            members.Add(new DirectoryEntityDescriptor("User", "aad", originId.ToString(), properties: this.CommonEntityDescriptorProperties));
          foreach (IdentityDirectoryEntityResult<TeamFoundationIdentity> addMember in (IEnumerable<IdentityDirectoryEntityResult<TeamFoundationIdentity>>) vssRequestContext.GetService<IDirectoryService>().IncludeTeamFoundationIdentities(requestContext).AddMembers(vssRequestContext, (IReadOnlyList<IDirectoryEntityDescriptor>) members, license: "Optimal"))
          {
            if (addMember.Status == "NotInScope")
              throw new RecipientWorkItemAccessDeniedException();
            if (addMember.Status != "Success")
            {
              string str = addMember.Entity == null ? "Undefined" : addMember.Entity.DisplayName;
              requestContext.Trace(516604, TraceLevel.Error, "WebAccess.WorkItem", TfsTraceLayers.Controller, "Identity " + str + " is out of scope and cannot be materialized");
              throw new Exception(WITResources.EmailResolveIdentityFailed());
            }
            foundationIdentityList.Add(addMember.Identity);
          }
        }
        return (IReadOnlyList<TeamFoundationIdentity>) foundationIdentityList;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(516605, TraceLevel.Error, "WebAccess.WorkItem", TfsTraceLayers.Controller, ex);
        throw;
      }
    }

    private CatalogBulkData GetCatalogBulkData(IVssRequestContext requestContext)
    {
      using (requestContext.TraceBlock(290064, 290065, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, nameof (GetCatalogBulkData)))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        ITeamFoundationCatalogService service = vssRequestContext.GetService<ITeamFoundationCatalogService>();
        List<Guid> settingsResourceTypes = ProjectSettingsCatalogHelper.GetProjectSettingsResourceTypes<TeamProject>();
        CatalogNode catalogNode = TeamFoundationCatalogHelper.QueryCollectionCatalogNode(vssRequestContext, requestContext.ServiceHost.InstanceId, CatalogQueryOptions.None);
        IVssRequestContext requestContext1 = vssRequestContext;
        string[] collectionPathSpecs = ProjectSettingsCatalogHelper.GetProjectCollectionPathSpecs(catalogNode.FullPath);
        List<Guid> resourceTypeFilters = settingsResourceTypes;
        List<CatalogNode> nodes = service.QueryNodes(requestContext1, (IEnumerable<string>) collectionPathSpecs, (IEnumerable<Guid>) resourceTypeFilters, CatalogQueryOptions.ExpandDependencies | CatalogQueryOptions.IncludeParents);
        settingsResourceTypes.Add(CatalogResourceTypes.OrganizationalRoot);
        List<Guid> queriedResourceTypes = settingsResourceTypes;
        return new CatalogBulkData((ICollection<CatalogNode>) nodes, (ICollection<Guid>) queriedResourceTypes);
      }
    }

    private object PopulateProjectUrls(
      IVssRequestContext requestContext,
      CatalogBulkData bulkData,
      Guid projectId)
    {
      Uri uri1 = (Uri) null;
      Uri uri2 = (Uri) null;
      Uri uri3 = (Uri) null;
      Uri uri4 = (Uri) null;
      if (bulkData != null)
      {
        TeamProject projectCatalogObject = this.GetTeamProjectCatalogObject(requestContext, bulkData, projectId);
        if (projectCatalogObject != null)
        {
          if (projectCatalogObject.Portal != null)
            uri1 = projectCatalogObject.Portal.FullUrl;
          if (projectCatalogObject.Guidance != null)
            uri2 = projectCatalogObject.Guidance.FullUrl;
          if (projectCatalogObject.ReportFolder != null)
          {
            ReportingServer reportServer = projectCatalogObject.ReportFolder.GetReportServer();
            if (reportServer != null)
            {
              uri3 = reportServer.ReportsManagerServiceLocation;
              uri4 = reportServer.ReportServerServiceLocation;
            }
          }
        }
      }
      return uri1 != (Uri) null || uri2 != (Uri) null || uri3 != (Uri) null || uri4 != (Uri) null ? (object) new
      {
        PortalPage = uri1,
        ProcessGuidance = uri2,
        ReportManagerUrl = uri3,
        ReportServiceSiteUrl = uri4
      } : (object) null;
    }

    private TeamProject GetTeamProjectCatalogObject(
      IVssRequestContext requestContext,
      CatalogBulkData bulkData,
      Guid projectId)
    {
      requestContext = requestContext.To(TeamFoundationHostType.Application);
      CatalogNode catalogNode = bulkData.Nodes.Where<CatalogNode>((Func<CatalogNode, bool>) (n => n.Resource.ResourceType.Identifier == TeamProject.ResourceTypeIdentifier && projectId == new Guid(n.Resource.Properties["ProjectId"]))).FirstOrDefault<CatalogNode>();
      if (catalogNode == null)
        return (TeamProject) null;
      TeamProject catalogObject = new CatalogObjectContext(requestContext).CreateCatalogObject<TeamProject>(catalogNode);
      catalogObject.Preload(bulkData);
      return catalogObject;
    }

    private IEnumerable<JsObject> GetIgnoredCoreFields(IVssRequestContext requestContext) => requestContext.WitContext().FieldDictionary.GetCoreFields().Where<FieldEntry>((Func<FieldEntry, bool>) (fld => fld.Usage == InternalFieldUsages.WorkItem && fld.IsIgnored)).Select<FieldEntry, JsObject>((Func<FieldEntry, JsObject>) (fld => fld.ToJson()));
  }
}
