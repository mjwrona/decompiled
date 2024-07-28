// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ClassificationNodesController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Boards.WebApi.Common.Helpers;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "classificationNodes", ResourceVersion = 2)]
  public class ClassificationNodesController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5907000;

    [HttpGet]
    [TraceFilter(5907000, 5907009)]
    [PublicProjectRequestRestrictions("5.0")]
    public IEnumerable<WorkItemClassificationNode> GetRootNodes([FromUri(Name = "$depth")] int depth = 0) => (IEnumerable<WorkItemClassificationNode>) this.WitRequestContext.TreeService.GetRootTreeNodes(this.ProjectId).OrderBy<TreeNode, TreeStructureType>((Func<TreeNode, TreeStructureType>) (x => x.Type)).Select<TreeNode, WorkItemClassificationNode>((Func<TreeNode, WorkItemClassificationNode>) (x => WorkItemClassificationNodeFactory.Create(this.TfsRequestContext, x, depth, false))).ToArray<WorkItemClassificationNode>();

    [HttpGet]
    [TraceFilter(5907010, 5907019)]
    [ValidateModel]
    [ClientLocationId("5A172953-1B41-49D3-840A-33F79C3CE89F")]
    [ClientExample("GET__wit_classificationNodes_areas__areaPath_.json", "Get an area", null, null)]
    [ClientExample("GET__wit_classificationNodes_iterations__iterationPath_.json", "Get an iteration", null, null)]
    [PublicProjectRequestRestrictions("5.0")]
    public WorkItemClassificationNode GetClassificationNode(
      TreeStructureGroup structureGroup,
      string path = null,
      [FromUri(Name = "$depth")] int depth = 0)
    {
      path = WorkItemClassificationNodeHelper.FixPath(path);
      return WorkItemClassificationNodeFactory.Create(this.TfsRequestContext, this.WitRequestContext.TreeService.GetTreeNode(this.ProjectId, path, WorkItemClassificationNodeHelper.ToInternalTreeStructureType(structureGroup)), depth, true);
    }

    [HttpGet]
    [ValidateModel]
    [ClientResourceOperation(ClientResourceOperationName.List)]
    [ClientExample("GET__wit_classificationNodes_From_Ids.json", "Get classification nodes from list of ids.", null, null)]
    [ClientExample("GET__wit_classificationNodes_From_Ids_ErrorPolicy.json", "Get classification nodes with errorPolicy parameter.", null, null)]
    [ClientExample("GET__wit_classificationNodes_areas.json", "Get the root area tree", null, null)]
    [ClientExample("GET__wit_classificationNodes_areas__depth-2.json", "Get the area tree with 2 levels of children", null, null)]
    [ClientExample("GET__wit_classificationNodes_iterations.json", "Get the root iteration tree", null, null)]
    [ClientExample("GET__wit_classificationNodes_iterations__depth-2.json", "Get the iteration tree with 2 levels of children", null, null)]
    [PublicProjectRequestRestrictions("5.0")]
    public IEnumerable<WorkItemClassificationNode> GetClassificationNodes(
      [ClientParameterAsIEnumerable(typeof (int), ',')] string ids,
      [FromUri(Name = "$depth")] int depth = 0,
      ClassificationNodesErrorPolicy errorPolicy = ClassificationNodesErrorPolicy.Fail)
    {
      return string.IsNullOrEmpty(ids) ? this.GetRootNodes(depth) : (IEnumerable<WorkItemClassificationNode>) this.GetClassificationNodesFromIds(this.ProjectId, (IEnumerable<int>) ParsingHelper.ParseIds(ids), errorPolicy).Select<TreeNode, WorkItemClassificationNode>((Func<TreeNode, WorkItemClassificationNode>) (node =>
      {
        if (node == null)
          return (WorkItemClassificationNode) null;
        try
        {
          return WorkItemClassificationNodeFactory.Create(this.TfsRequestContext, node, depth, true);
        }
        catch (Exception ex)
        {
          if (errorPolicy == ClassificationNodesErrorPolicy.Omit)
            return (WorkItemClassificationNode) null;
          throw;
        }
      })).ToList<WorkItemClassificationNode>();
    }

    [HttpPost]
    [TraceFilter(5907020, 5907029)]
    [ValidateModel]
    [ClientLocationId("5A172953-1B41-49D3-840A-33F79C3CE89F")]
    [ClientResponseType(typeof (WorkItemClassificationNode), null, null)]
    [ClientExample("POST__wit_classificationNodes_areas.json", "Create an area", null, null)]
    [ClientExample("POST__wit_classificationNodes_areas__areaParent_.json", "Move an area node", null, null)]
    [ClientExample("POST__wit_classificationNodes_iterations.json", "Create an iteration", null, null)]
    [ClientExample("POST__wit_classificationNodes_iterations__iterationParent_.json", "Move an iteration node", null, null)]
    public HttpResponseMessage CreateOrUpdateClassificationNode(
      TreeStructureGroup structureGroup,
      [FromBody] WorkItemClassificationNode postedNode,
      string path = null)
    {
      if (postedNode == null)
        throw new VssPropertyValidationException(nameof (postedNode), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ClassificationNodeRequred());
      path = WorkItemClassificationNodeHelper.FixPath(path);
      bool isUpdate = false;
      return this.PerformIfCRUDFeatureEnabled<HttpResponseMessage>((Func<HttpResponseMessage>) (() =>
      {
        TreeStructureType treeStructureType = WorkItemClassificationNodeHelper.ToInternalTreeStructureType(structureGroup);
        TreeNode treeNode = this.WitRequestContext.TreeService.GetTreeNode(this.ProjectId, path, treeStructureType);
        HttpResponseMessage classificationNode1;
        if (postedNode.Id == 0)
        {
          isUpdate = false;
          Guid nodeId = Guid.NewGuid();
          ClassificationNodeUpdate classificationNodeUpdate = new ClassificationNodeUpdate()
          {
            Name = postedNode.Name,
            ParentIdentifier = treeNode.CssNodeId,
            Identifier = nodeId,
            StructureType = treeStructureType
          };
          if (structureGroup == TreeStructureGroup.Iterations)
          {
            classificationNodeUpdate.StartDate = ClassificationNodesController.GetStartDate(postedNode);
            classificationNodeUpdate.FinishDate = ClassificationNodesController.GetFinishDate(postedNode);
          }
          WorkItemTrackingTreeService service = this.TfsRequestContext.GetService<WorkItemTrackingTreeService>();
          service.CreateNodes(this.TfsRequestContext, this.ProjectId, (ICollection<ClassificationNodeUpdate>) new ClassificationNodeUpdate[1]
          {
            classificationNodeUpdate
          });
          WorkItemClassificationNode classificationNode2 = WorkItemClassificationNodeFactory.Create(this.TfsRequestContext, service.GetTreeNode(this.TfsRequestContext, this.ProjectId, nodeId), 0, true);
          classificationNode1 = this.Request.CreateResponse<WorkItemClassificationNode>(HttpStatusCode.Created, classificationNode2);
          classificationNode1.Headers.Location = new Uri(classificationNode2.Url);
        }
        else
        {
          isUpdate = true;
          classificationNode1 = this.UpdateClassificationNodeInternal(structureGroup, postedNode, new int?(treeNode.Id));
        }
        if (string.IsNullOrEmpty(path))
          path = postedNode.Name;
        if (isUpdate)
        {
          if (!path.IsNullOrEmpty<char>())
          {
            IVssRequestContext tfsRequestContext = this.TfsRequestContext;
            string projectActionId = ProcessAuditConstants.GetProjectActionId(TreeStructureGroup.Iterations == structureGroup ? "IterationPath" : "AreaPath", "Update");
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("Path", (object) path);
            data.Add("ProjectName", (object) this.ProjectInfo.Name);
            Guid id = this.ProjectInfo.Id;
            Guid targetHostId = new Guid();
            Guid projectId = id;
            tfsRequestContext.LogAuditEvent(projectActionId, data, targetHostId, projectId);
          }
        }
        else
        {
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          string projectActionId = ProcessAuditConstants.GetProjectActionId(TreeStructureGroup.Iterations == structureGroup ? "IterationPath" : "AreaPath", "Create");
          Dictionary<string, object> data = new Dictionary<string, object>();
          data.Add("Path", (object) path);
          data.Add("ProjectName", (object) this.ProjectInfo.Name);
          Guid id = this.ProjectInfo.Id;
          Guid targetHostId = new Guid();
          Guid projectId = id;
          tfsRequestContext.LogAuditEvent(projectActionId, data, targetHostId, projectId);
        }
        return classificationNode1;
      }));
    }

    [HttpPatch]
    [TraceFilter(5907030, 5907039)]
    [ValidateModel]
    [ClientLocationId("5A172953-1B41-49D3-840A-33F79C3CE89F")]
    [ClientResponseType(typeof (WorkItemClassificationNode), null, null)]
    [ClientExample("PATCH__wit_classificationNodes_areas__areaPath_.json", "Rename an area", null, null)]
    [ClientExample("PATCH__wit_classificationNodes_iterations__iterationPath_.json", "Rename an iteration", null, null)]
    [ClientExample("PATCH__wit_classificationNodes_iterations__iterationPathNew_.json", "Change an iteration's dates", null, null)]
    public HttpResponseMessage UpdateClassificationNode(
      TreeStructureGroup structureGroup,
      [FromBody] WorkItemClassificationNode postedNode,
      string path = null)
    {
      if (postedNode == null)
        throw new VssPropertyValidationException(nameof (postedNode), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.ClassificationNodeRequred());
      path = WorkItemClassificationNodeHelper.FixPath(path);
      return this.PerformIfCRUDFeatureEnabled<HttpResponseMessage>((Func<HttpResponseMessage>) (() =>
      {
        TreeNode treeNode = this.WitRequestContext.TreeService.GetTreeNode(this.ProjectId, path, WorkItemClassificationNodeHelper.ToInternalTreeStructureType(structureGroup));
        if (postedNode.Id != 0 && postedNode.Id != treeNode.Id)
          throw new VssPropertyValidationException("Id", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.IdMismatch((object) postedNode.Id, (object) treeNode.Id));
        postedNode.Id = treeNode.Id;
        postedNode.Identifier = treeNode.CssNodeId;
        if (string.IsNullOrEmpty(path))
          path = string.IsNullOrEmpty(postedNode.Path) ? treeNode.GetSanitizedName(this.TfsRequestContext) : postedNode.Path;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string projectActionId = ProcessAuditConstants.GetProjectActionId(TreeStructureGroup.Iterations == structureGroup ? "IterationPath" : "AreaPath", "Update");
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("Path", (object) path);
        data.Add("ProjectName", (object) this.ProjectInfo.Name);
        Guid id = this.ProjectInfo.Id;
        Guid targetHostId = new Guid();
        Guid projectId = id;
        tfsRequestContext.LogAuditEvent(projectActionId, data, targetHostId, projectId);
        return this.UpdateClassificationNodeInternal(structureGroup, postedNode, new int?());
      }));
    }

    [HttpDelete]
    [TraceFilter(5907040, 5907049)]
    [ValidateModel]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("5A172953-1B41-49D3-840A-33F79C3CE89F")]
    [ClientExample("DELETE__wit_classificationNodes_areas__areaParent___reclassifyId-_rootAreaId_.json", "Delete an area node", null, null)]
    [ClientExample("DELETE__wit_classificationNodes_iterations__iterationParent___reclassifyId-_rootIterationId_.json", "Delete an iteration node", null, null)]
    public HttpResponseMessage DeleteClassificationNode(
      TreeStructureGroup structureGroup,
      [FromUri(Name = "$reclassifyId")] int? reclassifyId = null,
      string path = null)
    {
      path = WorkItemClassificationNodeHelper.FixPath(path);
      return this.PerformIfCRUDFeatureEnabled<HttpResponseMessage>((Func<HttpResponseMessage>) (() =>
      {
        TreeStructureType treeStructureType = WorkItemClassificationNodeHelper.ToInternalTreeStructureType(structureGroup);
        WorkItemTrackingTreeService service = this.TfsRequestContext.GetService<WorkItemTrackingTreeService>();
        TreeNode treeNode = this.WitRequestContext.TreeService.GetTreeNode(this.ProjectId, path, treeStructureType);
        string path1 = treeNode.GetPath(this.TfsRequestContext);
        ClassificationNodeUpdate[] nodes = new ClassificationNodeUpdate[1]
        {
          new ClassificationNodeUpdate()
          {
            Id = treeNode.Id,
            IsDeleted = true,
            ReclassifyId = reclassifyId.HasValue ? reclassifyId.Value : treeNode.ParentId
          }
        };
        service.DeleteNodes(this.TfsRequestContext, this.ProjectId, (ICollection<ClassificationNodeUpdate>) nodes);
        if (string.IsNullOrEmpty(path))
          path = path1;
        if (string.IsNullOrEmpty(path))
          path = reclassifyId.ToString();
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string projectActionId = ProcessAuditConstants.GetProjectActionId(TreeStructureGroup.Iterations == structureGroup ? "IterationPath" : "AreaPath", "Delete");
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("Path", (object) path);
        data.Add("ProjectName", (object) this.ProjectInfo.Name);
        Guid projectId1 = this.ProjectId;
        Guid targetHostId = new Guid();
        Guid projectId2 = projectId1;
        tfsRequestContext.LogAuditEvent(projectActionId, data, targetHostId, projectId2);
        return this.Request.CreateResponse(HttpStatusCode.NoContent);
      }));
    }

    private HttpResponseMessage UpdateClassificationNodeInternal(
      TreeStructureGroup structureGroup,
      WorkItemClassificationNode postedNode,
      int? parentId)
    {
      this.WitRequestContext.TreeService.GetTreeNode(this.ProjectId, postedNode.Id);
      WorkItemTrackingTreeService.IterationDates dates = (WorkItemTrackingTreeService.IterationDates) null;
      if (structureGroup == TreeStructureGroup.Iterations)
        dates = new WorkItemTrackingTreeService.IterationDates()
        {
          StartDate = ClassificationNodesController.GetStartDate(postedNode),
          FinishDate = ClassificationNodesController.GetFinishDate(postedNode)
        };
      WorkItemTrackingTreeService service = this.TfsRequestContext.GetService<WorkItemTrackingTreeService>();
      service.UpdateNode(this.TfsRequestContext, this.ProjectId, postedNode.Id, postedNode.Name, parentId, dates);
      WorkItemClassificationNode classificationNode = WorkItemClassificationNodeFactory.Create(this.TfsRequestContext, service.GetTreeNode(this.TfsRequestContext, this.ProjectId, postedNode.Id), 0, true);
      HttpResponseMessage response = this.Request.CreateResponse<WorkItemClassificationNode>(HttpStatusCode.OK, classificationNode);
      response.Headers.Location = new Uri(classificationNode.Url);
      return response;
    }

    private T PerformIfCRUDFeatureEnabled<T>(Func<T> function) => function();

    private void PerformIfCRUDFeatureEnabled(System.Action function) => this.PerformIfCRUDFeatureEnabled<int>((Func<int>) (() =>
    {
      function();
      return 0;
    }));

    private static DateTime? GetStartDate(WorkItemClassificationNode node) => ClassificationNodesController.GetAttribute<DateTime?>(node, "startDate");

    private static DateTime? GetFinishDate(WorkItemClassificationNode node) => ClassificationNodesController.GetAttribute<DateTime?>(node, "finishDate");

    private static T GetAttribute<T>(WorkItemClassificationNode node, string attributeName)
    {
      object objA;
      return node.Attributes == null || !node.Attributes.TryGetValue(attributeName, out objA) || object.Equals(objA, (object) null) || object.Equals(objA, (object) "") || !typeof (T).IsAssignableFrom(objA.GetType()) ? default (T) : CommonWITUtils.ConvertValue<T>(objA);
    }

    private IEnumerable<TreeNode> GetClassificationNodesFromIds(
      Guid projectId,
      IEnumerable<int> ids,
      ClassificationNodesErrorPolicy errorPolicy)
    {
      List<TreeNode> classificationNodesFromIds = new List<TreeNode>();
      foreach (int id in ids)
      {
        TreeNode node = (TreeNode) null;
        if (!this.WitRequestContext.TreeService.TryGetTreeNode(this.ProjectId, id, out node) && errorPolicy == ClassificationNodesErrorPolicy.Fail)
          throw new WorkItemTrackingTreeNodeNotFoundException(id);
        classificationNodesFromIds.Add(node);
      }
      return (IEnumerable<TreeNode>) classificationNodesFromIds;
    }
  }
}
