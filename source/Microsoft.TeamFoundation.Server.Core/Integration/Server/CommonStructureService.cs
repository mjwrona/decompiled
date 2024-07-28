// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.CommonStructureService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace Microsoft.TeamFoundation.Integration.Server
{
  public sealed class CommonStructureService : ICommonStructureService, IVssFrameworkService
  {
    private ICommonStructureServiceProvider m_serviceProvider;
    private static readonly string s_area = nameof (CommonStructureService);
    private static readonly string s_layer = nameof (CommonStructureService);
    private Dictionary<string, XslCompiledTransform> m_projectMappingXslt = new Dictionary<string, XslCompiledTransform>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private object m_projectMappingLock = new object();

    internal CommonStructureService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_serviceProvider = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.GetExtension<ICommonStructureServiceProvider>() : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      if (this.m_serviceProvider == null)
      {
        systemRequestContext.Trace(5500190, TraceLevel.Error, CommonStructureService.s_area, CommonStructureService.s_layer, "Failed to load common structure service provider.");
        throw new NotSupportedException();
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public CatalogNode QueryProjectCatalogNode(IVssRequestContext requestContext, string projectUri)
    {
      if (string.IsNullOrEmpty(projectUri))
      {
        requestContext.Trace(5500123, TraceLevel.Error, CommonStructureService.s_area, CommonStructureService.s_layer, Microsoft.TeamFoundation.Server.Core.Resources.EmptyProjectUri());
        throw new ArgumentNullException(projectUri, Microsoft.TeamFoundation.Server.Core.Resources.EmptyProjectUri());
      }
      IVssRequestContext vssRequestContext = requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.To(TeamFoundationHostType.Application) : throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      CatalogNode catalogNode = TeamFoundationCatalogHelper.QueryCollectionCatalogNode(vssRequestContext, requestContext.ServiceHost.InstanceId, CatalogQueryOptions.None);
      ITeamFoundationCatalogService service = vssRequestContext.GetService<ITeamFoundationCatalogService>();
      KeyValuePair<string, string>[] propertyFilters = new KeyValuePair<string, string>[1]
      {
        new KeyValuePair<string, string>("ProjectUri", projectUri)
      };
      return service.QueryNodes(vssRequestContext, catalogNode.FullPath + CatalogConstants.SingleRecurseStar, CatalogResourceTypes.TeamProject, (IEnumerable<KeyValuePair<string, string>>) propertyFilters).FirstOrDefault<CatalogNode>();
    }

    public CommonStructureProjectInfo[] GetProjects(IVssRequestContext requestContext) => CommonStructureProjectInfo.ConvertProjectInfo((IList<ProjectInfo>) requestContext.GetService<IProjectService>().GetProjects(requestContext).ToList<ProjectInfo>());

    public CommonStructureProjectInfo[] GetWellFormedProjects(IVssRequestContext requestContext) => this.GetProjectsFromState(requestContext, CommonStructureProjectState.WellFormed);

    internal CommonStructureProjectInfo[] GetProjectsFromState(
      IVssRequestContext requestContext,
      CommonStructureProjectState state)
    {
      return CommonStructureProjectInfo.ConvertProjectInfo((IList<ProjectInfo>) requestContext.GetService<IProjectService>().GetProjects(requestContext, CommonStructureProjectInfo.ConvertToProjectState(state)).ToList<ProjectInfo>());
    }

    public CommonStructureProjectInfo GetProject(
      IVssRequestContext requestContext,
      string projectUri)
    {
      Guid id;
      projectUri = CommonStructureUtils.CheckAndNormalizeUri(projectUri, nameof (projectUri), true, out id);
      return CommonStructureProjectInfo.ConvertProjectInfo(requestContext.GetService<IProjectService>().GetProject(requestContext, id));
    }

    public CommonStructureProjectInfo GetProject(IVssRequestContext requestContext, Guid projectId) => CommonStructureProjectInfo.ConvertProjectInfo(requestContext.GetService<IProjectService>().GetProject(requestContext, projectId));

    public CommonStructureProjectInfo GetProjectFromName(
      IVssRequestContext requestContext,
      string projectName)
    {
      return CommonStructureProjectInfo.ConvertProjectInfo(requestContext.GetService<IProjectService>().GetProject(requestContext, projectName));
    }

    internal Guid ReserveProject(
      IVssRequestContext requestContext,
      string projectName,
      Guid? desiredProjectGuid = null)
    {
      return requestContext.GetService<IProjectService>().ReserveProject(requestContext, projectName, desiredProjectGuid);
    }

    internal void DeleteReservedProject(IVssRequestContext requestContext, Guid pendingProjectGuid) => requestContext.GetService<IProjectService>().DeleteReservedProject(requestContext, pendingProjectGuid);

    public CommonStructureProjectInfo CreateProject(
      IVssRequestContext requestContext,
      string projectName,
      XmlElement structure,
      Guid? pendingProjectGuid = null)
    {
      return this.CreateProject(requestContext, projectName, CommonStructureUtils.ParseNodesStructure(structure, nameof (structure)), pendingProjectGuid, (string) null, ProjectVisibility.Private);
    }

    public CommonStructureProjectInfo CreateProject(
      IVssRequestContext requestContext,
      string projectName,
      XmlNode[] nodes,
      Guid? pendingProjectGuid = null,
      string projectDescription = null,
      ProjectVisibility projectVisibility = ProjectVisibility.Private)
    {
      bool flag = false;
      try
      {
        if (!pendingProjectGuid.HasValue)
        {
          pendingProjectGuid = new Guid?(this.ReserveProject(requestContext, projectName));
          flag = true;
        }
        ProjectInfo project = requestContext.GetService<IProjectService>().CreateProject(requestContext, pendingProjectGuid.Value, projectName, projectVisibility, projectDescription: projectDescription);
        this.EnsureTeamProjectsExistsInCatalog(requestContext, (IEnumerable<CommonStructureProjectInfo>) new List<CommonStructureProjectInfo>()
        {
          CommonStructureProjectInfo.ConvertProjectInfo(project)
        });
        requestContext.GetService<IDataspaceService>().CreateDataspaces(requestContext, new string[2]
        {
          "Integration",
          "TestManagement"
        }, project.Id);
        TaggingSecurityHelper.SetTeamProjectTaggingPermissions(requestContext, project.Uri);
        if (nodes.Length != 0)
          this.CreateInitialNodesAndDefaultTeam(requestContext, project.Name, project.Id, project.Uri, nodes);
        return CommonStructureProjectInfo.ConvertProjectInfo(project);
      }
      catch
      {
        if (flag)
        {
          try
          {
            this.DeleteReservedProject(requestContext, pendingProjectGuid.Value);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(5500122, CommonStructureService.s_area, CommonStructureService.s_layer, ex);
          }
        }
        throw;
      }
    }

    public void EnsureTeamProjectsExistsInCatalog(
      IVssRequestContext requestContext,
      IEnumerable<CommonStructureProjectInfo> projects)
    {
      this.EnsureTeamProjectsExistsInCatalog(requestContext, projects, (ITFLogger) null);
    }

    public void EnsureTeamProjectsExistsInCatalog(
      IVssRequestContext requestContext,
      IEnumerable<CommonStructureProjectInfo> projects,
      ITFLogger logger)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      CatalogTransactionContext transactionContext = vssRequestContext.GetService<ITeamFoundationCatalogService>().CreateTransactionContext();
      if (transactionContext.IsReadOnly(vssRequestContext))
        return;
      if (vssRequestContext.ExecutionEnvironment.IsHostedDeployment && vssRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        requestContext.Trace(5500120, TraceLevel.Error, CommonStructureService.s_area, CommonStructureService.s_layer, "Collection is not associated with a hosting account. Cannot create a team project: {0}", (object) requestContext.ServiceHost);
        throw new InvalidParentHostException(requestContext.ServiceHost, vssRequestContext.ServiceHost);
      }
      TeamFoundationIdentityService service1 = vssRequestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentityService service2 = requestContext.GetService<TeamFoundationIdentityService>();
      ITeamFoundationSecurityService service3 = requestContext.GetService<ITeamFoundationSecurityService>();
      CatalogNode catalogNode1 = TeamFoundationCatalogHelper.QueryCollectionCatalogNode(vssRequestContext, requestContext.ServiceHost.InstanceId, CatalogQueryOptions.None);
      if (catalogNode1 == null)
        throw new CatalogNodeDoesNotExistException();
      List<CatalogNode> catalogNodeList = new List<CatalogNode>();
      logger?.Info(string.Format("Processing project(s). Count: {0}", (object) projects.Count<CommonStructureProjectInfo>()));
      foreach (CommonStructureProjectInfo project1 in projects)
      {
        logger?.Info(string.Format("Processing {0} project. Status: {1}", (object) project1.Name, (object) project1.Status));
        if (project1.Status != CommonStructureProjectState.CreatePending)
        {
          CatalogNode catalogNode2 = this.QueryProjectCatalogNode(requestContext, project1.Uri);
          if (catalogNode2 == null)
          {
            logger?.Info("Creating catalog entry...");
            catalogNode2 = catalogNode1.CreateChild(vssRequestContext, TeamProjectCatalogConstants.ResourceType, project1.Name);
          }
          Guid id;
          if (CommonStructureUtils.TryParseProjectUri(project1.Uri, out id))
          {
            catalogNode2.Resource.Properties["ProjectName"] = project1.Name;
            catalogNode2.Resource.Properties["ProjectUri"] = project1.Uri;
            catalogNode2.Resource.Properties["ProjectId"] = id.ToString();
            catalogNode2.Resource.Properties["ProjectState"] = project1.Status.ToString();
            IProjectService service4 = requestContext.GetService<IProjectService>();
            try
            {
              ProjectInfo project2 = service4.GetProject(requestContext, id);
              project2.PopulateProperties(requestContext, "System.SourceControlCapabilityFlags", "System.SourceControlTfvcEnabled", "System.SourceControlGitEnabled");
              if (!string.IsNullOrEmpty(project2.Description))
                catalogNode2.Resource.Description = project2.Description;
              ProjectProperty projectProperty1 = project2.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (x => x.Name.Equals("System.SourceControlCapabilityFlags")));
              if (projectProperty1 != null)
                catalogNode2.Resource.Properties["SourceControlCapabilityFlags"] = (string) projectProperty1.Value;
              ProjectProperty projectProperty2 = project2.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (x => x.Name.Equals("System.SourceControlTfvcEnabled")));
              if (projectProperty2 != null)
                catalogNode2.Resource.Properties["SourceControlTfvcEnabled"] = (string) projectProperty2.Value;
              ProjectProperty projectProperty3 = project2.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (x => x.Name.Equals("System.SourceControlGitEnabled")));
              if (projectProperty3 != null)
                catalogNode2.Resource.Properties["SourceControlGitEnabled"] = (string) projectProperty3.Value;
            }
            catch (ProjectDoesNotExistException ex)
            {
              requestContext.Trace(5500121, TraceLevel.Info, CommonStructureService.s_area, CommonStructureService.s_layer, "CommonStructureService.EnsureTeamProjectsExistsInCatalog() - project {0} was not found in the ProjectService", (object) id);
            }
            transactionContext.AttachNode(catalogNode2);
            catalogNodeList.Add(catalogNode2);
          }
        }
      }
      transactionContext.Save(vssRequestContext, false);
      TeamFoundationIdentity foundationIdentity1 = service2.ReadIdentity(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, MembershipQuery.None, ReadIdentityOptions.TrueSid);
      if (foundationIdentity1 == null)
      {
        logger?.Info("Could not find collection admin group.");
        throw new IdentityNotFoundException(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup);
      }
      TeamFoundationIdentity foundationIdentity2 = service2.ReadIdentity(requestContext, GroupWellKnownIdentityDescriptors.ServiceUsersGroup, MembershipQuery.None, ReadIdentityOptions.TrueSid);
      if (foundationIdentity2 == null)
      {
        logger?.Info("Could not find collection service users group.");
        throw new IdentityNotFoundException(GroupWellKnownIdentityDescriptors.ServiceUsersGroup);
      }
      IdentityDescriptor foundationDescriptor = IdentityHelper.CreateTeamFoundationDescriptor(SharePointConstants.SharePointServiceAccountsGroup);
      TeamFoundationIdentity foundationIdentity3 = service1.ReadIdentity(vssRequestContext, foundationDescriptor, MembershipQuery.None, ReadIdentityOptions.TrueSid);
      IVssSecurityNamespace securityNamespace = service3.GetSecurityNamespace(requestContext, AuthorizationSecurityConstants.ProjectSecurityGuid);
      List<AccessControlList> accessControlListList = new List<AccessControlList>();
      foreach (CatalogNode catalogNode3 in catalogNodeList)
      {
        string property1 = catalogNode3.Resource.Properties["ProjectName"];
        string property2 = catalogNode3.Resource.Properties["ProjectUri"];
        logger?.Info("Processing team project " + property1 + ". Project Uri: " + property2);
        TeamFoundationIdentity foundationIdentity4 = service2.ReadIdentity(requestContext, IdentitySearchFactor.AdministratorsGroup, property2);
        if (foundationIdentity4 == null)
        {
          logger?.Error("Failed to find admin group for the following team project: " + property1 + ". ProjectUri: " + property2);
          throw new IdentityNotFoundException(FrameworkResources.IdentityNotFoundSimpleMessage());
        }
        AccessControlList acl = new AccessControlList(catalogNode3.FullPath, false);
        acl.SetPermissions(foundationIdentity4.Descriptor, 15, 0, false);
        acl.SetPermissions(foundationIdentity1.Descriptor, 15, 0, false);
        acl.SetPermissions(foundationIdentity2.Descriptor, 15, 0, false);
        if (foundationIdentity3 != null)
          acl.SetPermissions(foundationIdentity3.Descriptor, 1, 0, false);
        foreach (IAccessControlEntry accessControlEntry in securityNamespace.QueryAccessControlList(requestContext, AuthorizationSecurityConstants.ProjectSecurityPrefix + catalogNode3.Resource.Properties["ProjectUri"], (IEnumerable<IdentityDescriptor>) null, false).AccessControlEntries)
        {
          int projectPermissions1 = this.GetCatalogPermissionsFromProjectPermissions(accessControlEntry.Allow);
          int projectPermissions2 = this.GetCatalogPermissionsFromProjectPermissions(accessControlEntry.Deny);
          if (projectPermissions1 != 0 || projectPermissions2 != 0)
            acl.SetPermissions((service2.ReadIdentity(requestContext, accessControlEntry.Descriptor, MembershipQuery.None, ReadIdentityOptions.TrueSid) ?? throw new IdentityNotFoundException(accessControlEntry.Descriptor)).Descriptor, projectPermissions1, projectPermissions2, true);
        }
        accessControlListList.Add(acl);
      }
      vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.CatalogNamespaceId).SetAccessControlLists(vssRequestContext.Elevate(), (IEnumerable<IAccessControlList>) accessControlListList, false);
    }

    public void TransformProjectMappingProperty(
      string xsltVersionSpecifier,
      CommonStructureProjectProperty[] properties)
    {
      XslCompiledTransform xslTransform;
      lock (this.m_projectMappingLock)
      {
        if (!this.m_projectMappingXslt.TryGetValue(xsltVersionSpecifier, out xslTransform))
        {
          Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(xsltVersionSpecifier);
          XmlReaderSettings settings = new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null
          };
          using (StreamReader streamReader = new StreamReader(manifestResourceStream))
          {
            string end = streamReader.ReadToEnd();
            xslTransform = new XslCompiledTransform();
            using (XmlReader stylesheet = XmlReader.Create((TextReader) new StringReader(end), settings))
              xslTransform.Load(stylesheet);
            this.m_projectMappingXslt.Add(xsltVersionSpecifier, xslTransform);
          }
        }
      }
      CommonStructureService.TransformProjectMappingProperty(xslTransform, properties);
    }

    public CommonStructureNodeInfo GetNode(IVssRequestContext requestContext, string nodeUri)
    {
      Guid id;
      nodeUri = CommonStructureUtils.CheckAndNormalizeUri(nodeUri, nameof (nodeUri), false, out id);
      if (!requestContext.IsSystemContext)
        this.CheckGlobalPermission(requestContext, "GENERIC_READ");
      CommonStructureNodeInfo node = this.m_serviceProvider.GetNode(requestContext, id);
      if (!requestContext.IsSystemContext)
        this.CheckProjectPermission(requestContext, node.ProjectUri, "GENERIC_READ");
      return node;
    }

    public List<CommonStructureNodeInfo> GetNodes(
      IVssRequestContext requestContext,
      List<string> nodesUris)
    {
      if (!requestContext.IsSystemContext)
        this.CheckGlobalPermission(requestContext, "GENERIC_READ");
      List<Guid> guidList = new List<Guid>();
      foreach (string nodesUri in nodesUris)
      {
        Guid id;
        CommonStructureUtils.CheckAndNormalizeUri(nodesUri, "nodeUri", false, out id);
        guidList.Add(id);
      }
      List<CommonStructureNodeInfo> nodes = new List<CommonStructureNodeInfo>();
      foreach (Guid nodeId in guidList)
      {
        try
        {
          nodes.Add(this.m_serviceProvider.GetNode(requestContext, nodeId));
        }
        catch (NodeDoesNotExistException ex)
        {
        }
      }
      if (!requestContext.IsSystemContext)
      {
        foreach (CommonStructureNodeInfo structureNodeInfo in nodes)
          this.CheckProjectPermission(requestContext, structureNodeInfo.ProjectUri, "GENERIC_READ");
      }
      return nodes;
    }

    public bool TryGetNodeFromPath(
      IVssRequestContext requestContext,
      string nodePath,
      out CommonStructureNodeInfo node)
    {
      node = (CommonStructureNodeInfo) null;
      try
      {
        node = this.GetNodeFromPath(requestContext, nodePath);
        return true;
      }
      catch (NodePathDoesNotExistException ex)
      {
        return false;
      }
    }

    public CommonStructureNodeInfo GetNodeFromPath(
      IVssRequestContext requestContext,
      string nodePath)
    {
      nodePath = CommonStructureUtils.NormalizeNodePath(nodePath, nameof (nodePath));
      if (!requestContext.IsSystemContext)
        this.CheckGlobalPermission(requestContext, "GENERIC_READ");
      CommonStructureNodeInfo node = this.m_serviceProvider.GetNode(requestContext, nodePath);
      if (!requestContext.IsSystemContext)
        this.CheckProjectPermission(requestContext, node.ProjectUri, "GENERIC_READ");
      return node;
    }

    public XmlElement GetNodesXml(
      IVssRequestContext requestContext,
      string[] nodeUris,
      bool includeChildren)
    {
      XmlDocument document = TeamFoundationSerializationUtility.SerializeToDocument("<Nodes></Nodes>");
      XmlElement documentElement = document.DocumentElement;
      foreach (string nodeId in CommonStructureUtils.ExtractNodeIds(ref nodeUris, nameof (nodeUris)))
      {
        Dictionary<string, List<CommonStructureNodeInfo>> parents;
        CommonStructureNodeInfo nodeInfo;
        if (includeChildren)
        {
          nodeInfo = this.m_serviceProvider.GetNodeWithDescendants(requestContext, Guid.Parse(nodeId), out parents);
        }
        else
        {
          nodeInfo = this.m_serviceProvider.GetNode(requestContext, Guid.Parse(nodeId));
          parents = new Dictionary<string, List<CommonStructureNodeInfo>>();
          parents.Add(nodeInfo.Uri, new List<CommonStructureNodeInfo>());
        }
        documentElement.AppendChild((XmlNode) CommonStructureService.NodeInfoToXml(nodeInfo, parents, document));
      }
      if (!requestContext.IsSystemContext)
      {
        foreach (XmlElement childNode in documentElement.ChildNodes)
          this.CheckProjectPermission(requestContext, childNode.Attributes["ProjectID"].Value, "GENERIC_READ");
      }
      return documentElement;
    }

    public void GetNodes(
      IVssRequestContext requestContext,
      string nodeUri,
      out CommonStructureNodeInfo nodeInfo,
      out Dictionary<string, List<CommonStructureNodeInfo>> parents)
    {
      string nodeId = CommonStructureUtils.ExtractNodeId(ref nodeUri, nameof (nodeUri));
      Dictionary<string, List<CommonStructureNodeInfo>> parents1 = (Dictionary<string, List<CommonStructureNodeInfo>>) null;
      CommonStructureNodeInfo nodeWithDescendants = this.m_serviceProvider.GetNodeWithDescendants(requestContext, Guid.Parse(nodeId), out parents1);
      this.CheckProjectPermission(requestContext, nodeWithDescendants.ProjectUri, "GENERIC_READ");
      nodeInfo = nodeWithDescendants;
      parents = parents1;
    }

    public CommonStructureNodeInfo[] GetRootNodes(
      IVssRequestContext requestContext,
      string projectUri)
    {
      Guid id;
      projectUri = CommonStructureUtils.CheckAndNormalizeUri(projectUri, nameof (projectUri), true, out id);
      if (!requestContext.IsSystemContext)
        this.CheckProjectPermission(requestContext, projectUri, "GENERIC_READ");
      return this.m_serviceProvider.GetRootNodes(requestContext, id);
    }

    public XmlElement GetDeletedNodes(
      IVssRequestContext requestContext,
      string projectUri,
      DateTime since)
    {
      Guid id;
      projectUri = CommonStructureUtils.CheckAndNormalizeUri(projectUri, nameof (projectUri), true, out id);
      if (since < SqlDateTime.MinValue.Value)
        since = SqlDateTime.MinValue.Value;
      if (!requestContext.IsSystemContext)
      {
        this.CheckGlobalPermission(requestContext, "SYNCHRONIZE_READ");
        this.CheckProjectPermission(requestContext, projectUri, "GENERIC_READ");
      }
      List<DeletedNodeInfo> deletedNodes = this.m_serviceProvider.GetDeletedNodes(requestContext, id, since);
      XmlDocument document = TeamFoundationSerializationUtility.SerializeToDocument("<DeletedNodes></DeletedNodes>");
      XmlElement documentElement = document.DocumentElement;
      foreach (DeletedNodeInfo deletedNodeInfo in deletedNodes)
      {
        XmlElement element1 = document.CreateElement("Node");
        element1.SetAttribute("NodeID", deletedNodeInfo.Uri);
        XmlElement element2 = document.CreateElement("ReclassifyUri");
        element2.InnerText = deletedNodeInfo.ReclassifyUri;
        element1.AppendChild((XmlNode) element2);
        XmlElement element3 = document.CreateElement("DeletedUser");
        element3.InnerText = deletedNodeInfo.User;
        element1.AppendChild((XmlNode) element3);
        XmlElement element4 = document.CreateElement("DeletedTime");
        element4.InnerText = deletedNodeInfo.Time.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
        element1.AppendChild((XmlNode) element4);
        documentElement.AppendChild((XmlNode) element1);
      }
      return documentElement;
    }

    public string GetChangedNodes(IVssRequestContext requestContext, int startSequenceId)
    {
      if (startSequenceId < 0)
        throw new ArgumentException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_NEGATIVE_ARGUMENT((object) "firstSequenceId"), "firstSequenceId");
      if (!requestContext.IsSystemContext)
        this.CheckGlobalPermission(requestContext, "SYNCHRONIZE_READ");
      int endSequenceId = 0;
      List<ChangedNodeInfo> changedNode = this.m_serviceProvider.GetChangedNode(requestContext, startSequenceId, false, out endSequenceId, out List<ChangedProjectInfo> _);
      int fMore = 0;
      StringBuilder output = new StringBuilder();
      using (XmlWriter writer = XmlWriter.Create(output))
      {
        writer.WriteStartDocument();
        this.WriteChangedNodesAndProjects(writer, changedNode, (List<ChangedProjectInfo>) null, endSequenceId, fMore);
        writer.WriteEndDocument();
        writer.Close();
      }
      return output.ToString();
    }

    public string GetChangedNodesAndProjects(IVssRequestContext requestContext, int startSequenceId)
    {
      if (!requestContext.IsSystemContext)
        this.CheckGlobalPermission(requestContext, "SYNCHRONIZE_READ");
      if (startSequenceId < 0)
        throw new ArgumentException(Microsoft.Azure.Boards.CssNodes.ServerResources.CSS_NEGATIVE_ARGUMENT((object) "firstSequenceId"), "firstSequenceId");
      List<ChangedProjectInfo> deletedProjects = (List<ChangedProjectInfo>) null;
      int endSequenceId = 0;
      List<ChangedNodeInfo> changedNode = this.m_serviceProvider.GetChangedNode(requestContext, startSequenceId, true, out endSequenceId, out deletedProjects);
      int fMore = 0;
      StringBuilder output = new StringBuilder();
      using (XmlWriter writer = XmlWriter.Create(output))
      {
        writer.WriteStartDocument();
        this.WriteChangedNodesAndProjects(writer, changedNode, deletedProjects, endSequenceId, fMore);
        writer.WriteEndDocument();
        writer.Close();
      }
      return output.ToString();
    }

    public string CreateNode(
      IVssRequestContext requestContext,
      string nodeName,
      string parentUri,
      DateTime? startDate,
      DateTime? finishDate)
    {
      Guid id1;
      parentUri = CommonStructureUtils.CheckAndNormalizeUri(parentUri, "parentNodeUri", false, out id1);
      Guid id2 = Guid.NewGuid();
      Guid projectId = Guid.Parse(LinkingUtilities.DecodeUri(this.m_serviceProvider.GetNode(requestContext, id1).ProjectUri).ToolSpecificId);
      CommonStructureNode commonStructureNode = new CommonStructureNode()
      {
        Id = id2,
        Name = nodeName,
        ParentId = id1,
        StartDate = startDate,
        FinishDate = finishDate
      };
      this.m_serviceProvider.CreateNodes(requestContext, projectId, (IEnumerable<CommonStructureNode>) new CommonStructureNode[1]
      {
        commonStructureNode
      });
      return CommonStructureUtils.GetNodeUri(id2);
    }

    public string CreateNode(IVssRequestContext requestContext, string nodeName, string parentUri) => this.CreateNode(requestContext, nodeName, parentUri, new DateTime?(), new DateTime?());

    public void CreateInitialNodesAndDefaultTeam(
      IVssRequestContext requestContext,
      string projectName,
      Guid projectId,
      string projectUri,
      XmlElement structure)
    {
      this.CreateInitialNodesAndDefaultTeam(requestContext, projectName, projectId, projectUri, CommonStructureUtils.ParseNodesStructure(structure, nameof (structure)));
    }

    public void CreateInitialNodesAndDefaultTeam(
      IVssRequestContext requestContext,
      string projectName,
      Guid projectId,
      string projectUri,
      XmlNode[] nodes)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      if (!requestContext.IsSystemContext)
        this.CheckGlobalPermission(requestContext, "CREATE_PROJECTS");
      properties.Add("AfterPermissionElapsedTime", (double) stopwatch.ElapsedMilliseconds);
      List<CommonStructureNode> nodes1 = CommonStructureUtils.ConvertPcwXmlToNodes("ignored", nodes);
      properties.Add("AfterConvertElapsedTime", (double) stopwatch.ElapsedMilliseconds);
      this.m_serviceProvider.CreateNodes(requestContext, projectId, (IEnumerable<CommonStructureNode>) nodes1);
      properties.Add("AfterNodeCreationElapsedTime", (double) stopwatch.ElapsedMilliseconds);
      ITeamService service = requestContext.GetService<ITeamService>();
      string name = FrameworkResources.ProjectDefaultTeam((object) projectName);
      if (!Microsoft.Azure.Devops.Teams.Service.TeamsUtility.IsValidTeamName(name))
        name = projectName;
      WebApiTeam team = service.CreateTeam(requestContext, projectUri, name, FrameworkResources.ProjectDefaultTeamDescription());
      properties.Add("AfterTeamCreationElapsedTime", (double) stopwatch.ElapsedMilliseconds);
      service.SetDefaultTeamId(requestContext, projectId, team.Id);
      properties.Add("AfterSettingDefaultTeamElapsedTime", (double) stopwatch.ElapsedMilliseconds);
      stopwatch.Stop();
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ProjectCreatePerf", nameof (CreateInitialNodesAndDefaultTeam), properties);
    }

    public void DeleteBranches(
      IVssRequestContext requestContext,
      string[] nodeUris,
      string reclassifyUri)
    {
      CommonStructureUtils.CheckNullOrEmptyArrayArgument((object[]) nodeUris, nameof (nodeUris));
      List<Guid> guidList = new List<Guid>();
      foreach (string nodeUri in nodeUris)
      {
        Guid id;
        CommonStructureUtils.CheckAndNormalizeUri(nodeUri, nameof (nodeUris), false, out id);
        guidList.Add(id);
      }
      Guid id1;
      reclassifyUri = CommonStructureUtils.CheckAndNormalizeUri(reclassifyUri, nameof (reclassifyUri), false, out id1);
      Guid projectId = Guid.Parse(LinkingUtilities.DecodeUri(this.m_serviceProvider.GetNode(requestContext, guidList.First<Guid>()).ProjectUri).ToolSpecificId);
      this.m_serviceProvider.DeleteNodes(requestContext, projectId, (IEnumerable<Guid>) guidList, id1);
    }

    public void MoveBranch(IVssRequestContext requestContext, string nodeUri, string newParentUri)
    {
      Guid id1;
      nodeUri = CommonStructureUtils.CheckAndNormalizeUri(nodeUri, nameof (nodeUri), false, out id1);
      Guid id2;
      newParentUri = CommonStructureUtils.CheckAndNormalizeUri(newParentUri, "newParentNodeUri", false, out id2);
      Guid projectId = Guid.Parse(LinkingUtilities.DecodeUri(this.m_serviceProvider.GetNode(requestContext, id1).ProjectUri).ToolSpecificId);
      this.m_serviceProvider.MoveNode(requestContext, projectId, id1, id2);
    }

    public void RenameNode(IVssRequestContext requestContext, string nodeUri, string newName)
    {
      Guid id;
      nodeUri = CommonStructureUtils.CheckAndNormalizeUri(nodeUri, nameof (nodeUri), false, out id);
      Guid projectId = Guid.Parse(LinkingUtilities.DecodeUri(this.m_serviceProvider.GetNode(requestContext, id).ProjectUri).ToolSpecificId);
      this.m_serviceProvider.RenameNode(requestContext, projectId, id, newName);
    }

    public void SetIterationDates(
      IVssRequestContext requestContext,
      string nodeUri,
      DateTime? startDate,
      DateTime? finishDate)
    {
      Guid id;
      nodeUri = CommonStructureUtils.CheckAndNormalizeUri(nodeUri, nameof (nodeUri), false, out id);
      Guid projectId = Guid.Parse(LinkingUtilities.DecodeUri(this.m_serviceProvider.GetNode(requestContext, id).ProjectUri).ToolSpecificId);
      this.m_serviceProvider.UpdateIterationDates(requestContext, projectId, id, startDate, finishDate);
    }

    public void ReorderNode(IVssRequestContext requestContext, string nodeUri, int moveBy) => throw new NotSupportedException();

    public void HardDeleteProject(IVssRequestContext requestContext, string projectUri)
    {
      requestContext.TraceEnter(5500110, CommonStructureService.s_area, CommonStructureService.s_layer, "DeleteProject");
      try
      {
        Guid id;
        projectUri = CommonStructureUtils.CheckAndNormalizeUri(projectUri, nameof (projectUri), true, out id);
        try
        {
          requestContext.GetService<ITeamFoundationTaggingService>().DeleteTagDefinitionsInScope(requestContext.Elevate(), id);
        }
        catch (NotSupportedException ex)
        {
          requestContext.TraceCatch(5500111, CommonStructureService.s_area, CommonStructureService.s_layer, (Exception) ex);
        }
        requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TaggingNamespaceId)?.RemoveAccessControlLists(requestContext, (IEnumerable<string>) new string[1]
        {
          TaggingService.GetSecurityToken(new Guid?(id))
        }, true);
        requestContext.GetService<PlatformProjectService>().HardDeleteProject(requestContext, id);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        ITeamFoundationCatalogService service = vssRequestContext.GetService<ITeamFoundationCatalogService>();
        CatalogNode catalogNode = this.QueryProjectCatalogNode(requestContext, projectUri);
        if (catalogNode == null)
          return;
        CatalogTransactionContext transactionContext = service.CreateTransactionContext();
        foreach (CatalogNode nodeReference in catalogNode.Resource.NodeReferences)
          transactionContext.AttachDelete(nodeReference, true);
        transactionContext.Save(vssRequestContext, false);
      }
      catch (SecurityObjectDoesNotExistException ex)
      {
        throw new ProjectDoesNotExistException(new Uri(projectUri));
      }
      finally
      {
        requestContext.TraceEnter(5500112, CommonStructureService.s_area, CommonStructureService.s_layer, "DeleteProject");
      }
    }

    internal int GetCatalogPermissionsFromProjectPermissions(int projectPermissions)
    {
      int projectPermissions1 = 0;
      if ((projectPermissions & AuthorizationProjectPermissions.GenericRead) != 0)
        projectPermissions1 |= 1;
      if ((projectPermissions & AuthorizationProjectPermissions.GenericWrite) != 0)
        projectPermissions1 |= 14;
      return projectPermissions1;
    }

    internal string GetNodeSecurityToken(IVssRequestContext requestContext, Guid nodeId)
    {
      string securityToken = (string) null;
      this.m_serviceProvider.TryGetNodeSecurityToken(requestContext, nodeId, out securityToken);
      return securityToken;
    }

    private static void TransformProjectMappingProperty(
      XslCompiledTransform xslTransform,
      CommonStructureProjectProperty[] properties)
    {
      foreach (CommonStructureProjectProperty property in properties)
      {
        if (property != null && TFStringComparer.CssProjectPropertyName.Equals(property.Name, "System.MSPROJ") && !string.IsNullOrEmpty(property.Value))
        {
          XmlReaderSettings settings = new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null
          };
          using (MemoryStream w = new MemoryStream())
          {
            XmlTextWriter results = new XmlTextWriter((Stream) w, Encoding.UTF8);
            results.WriteStartDocument();
            using (XmlReader input = XmlReader.Create((TextReader) new StringReader(property.Value), settings))
              xslTransform.Transform(input, (XmlWriter) results);
            w.Seek(0L, SeekOrigin.Begin);
            using (StreamReader streamReader = new StreamReader((Stream) w))
            {
              property.Value = streamReader.ReadToEnd();
              break;
            }
          }
        }
      }
    }

    private static XmlElement NodeInfoToXml(
      CommonStructureNodeInfo nodeInfo,
      Dictionary<string, List<CommonStructureNodeInfo>> parentDictionary,
      XmlDocument doc)
    {
      XmlElement element1 = doc.CreateElement("Node");
      element1.SetAttribute("NodeID", nodeInfo.Uri);
      element1.SetAttribute("Name", nodeInfo.Name);
      if (nodeInfo.ParentUri != null)
        element1.SetAttribute("ParentID", nodeInfo.ParentUri);
      element1.SetAttribute("Path", nodeInfo.Path);
      element1.SetAttribute("ProjectID", nodeInfo.ProjectUri);
      element1.SetAttribute("StructureType", nodeInfo.StructureType);
      DateTime? nullable = nodeInfo.StartDate;
      if (nullable.HasValue)
      {
        XmlElement xmlElement1 = element1;
        nullable = nodeInfo.StartDate;
        DateTime dateTime = nullable.Value;
        string str1 = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
        xmlElement1.SetAttribute("StartDate", str1);
        XmlElement xmlElement2 = element1;
        nullable = nodeInfo.FinishDate;
        dateTime = nullable.Value;
        string str2 = dateTime.ToString("yyyy-MM-ddTHH:mm:ss.fff", (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
        xmlElement2.SetAttribute("FinishDate", str2);
      }
      List<CommonStructureNodeInfo> parent = parentDictionary[nodeInfo.Uri];
      if (parent != null && parent.Count > 0)
      {
        XmlElement element2 = doc.CreateElement("Children");
        element1.AppendChild((XmlNode) element2);
        foreach (CommonStructureNodeInfo nodeInfo1 in parent)
          element2.AppendChild((XmlNode) CommonStructureService.NodeInfoToXml(nodeInfo1, parentDictionary, doc));
      }
      return element1;
    }

    private void WriteChangedNodesAndProjects(
      XmlWriter writer,
      List<ChangedNodeInfo> nodes,
      List<ChangedProjectInfo> projects,
      int endSequenceId,
      int fMore)
    {
      writer.WriteStartElement("StructureChanges");
      writer.WriteAttributeString("MaxSequence", endSequenceId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      writer.WriteAttributeString(nameof (fMore), fMore.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      writer.WriteStartElement("StructureElements");
      foreach (ChangedNodeInfo node in nodes)
      {
        writer.WriteStartElement("StructureElement");
        writer.WriteAttributeString("Id", node.Uri);
        writer.WriteAttributeString("ProjectId", node.ProjectUri);
        writer.WriteAttributeString("Deleted", node.Deleted.ToString());
        if (node.Deleted)
        {
          writer.WriteAttributeString("ForwardingId", node.ReclassifyUri);
        }
        else
        {
          writer.WriteAttributeString("Name", node.Name);
          writer.WriteAttributeString("StructureId", node.Type);
          if (node.ParentUri != null)
            writer.WriteAttributeString("ParentId", node.ParentUri);
        }
        DateTime? nullable = node.StartDate;
        if (nullable.HasValue)
        {
          XmlWriter xmlWriter1 = writer;
          nullable = node.StartDate;
          DateTime dateTime = nullable.Value;
          string str1 = dateTime.ToString();
          xmlWriter1.WriteAttributeString("StartDate", str1);
          XmlWriter xmlWriter2 = writer;
          nullable = node.FinishDate;
          dateTime = nullable.Value;
          string str2 = dateTime.ToString();
          xmlWriter2.WriteAttributeString("FinishDate", str2);
        }
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
      if (projects != null)
      {
        writer.WriteStartElement("Projects");
        foreach (ChangedProjectInfo project in projects)
        {
          writer.WriteStartElement("Project");
          writer.WriteAttributeString("ProjectId", project.ProjectUri);
          writer.WriteAttributeString("Deleted", project.Deleted.ToString());
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }

    private void CheckGlobalPermission(IVssRequestContext requestContext, string permissionName)
    {
      int requestedPermissions = ActionToPermissionConverter.Convert(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceId, permissionName);
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceId);
      string token = securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, FrameworkSecurity.TeamProjectCollectionNamespaceToken);
      securityNamespace.CheckPermission(requestContext, token, requestedPermissions);
    }

    private void CheckProjectPermission(
      IVssRequestContext requestContext,
      string projectUri,
      string actionId)
    {
      int permission = ActionToPermissionConverter.Convert(requestContext, FrameworkSecurity.TeamProjectNamespaceId, actionId);
      requestContext.GetService<PlatformProjectService>().CheckProjectPermission(requestContext, projectUri, permission);
    }
  }
}
