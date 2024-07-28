// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.AdminBaseClassificationController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Boards.Settings;
using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [DemandFeature("65AC9DB3-BB0A-42fe-B584-A690FB0D817B", true)]
  public class AdminBaseClassificationController : AdminAreaController
  {
    private const int WorkItemTrackingSyncTimeoutMilliseconds = 5000;

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(501060, 501070)]
    public ActionResult CreateClassificationNode([ModelBinder(typeof (JsonModelBinder))] ClassificationNodeOperationData operationData)
    {
      bool flag = true;
      string str = string.Empty;
      string structureType = string.Empty;
      string nodeUri1 = string.Empty;
      try
      {
        ArgumentUtility.CheckForNull<ClassificationNodeOperationData>(operationData, nameof (operationData));
        ArgumentUtility.CheckForEmptyGuid(operationData.ParentId, "operationData.ParentId ");
        CommonStructureService service = this.TfsRequestContext.GetService<CommonStructureService>();
        string nodeUri2 = CommonStructureUtils.GetNodeUri(operationData.ParentId);
        CommonStructureNodeInfo node = service.GetNode(this.TfsRequestContext, nodeUri2);
        ArgumentUtility.CheckForNull<CommonStructureNodeInfo>(node, "parentNode");
        structureType = node.StructureType;
        DateTime? startDate = new DateTime?();
        DateTime? finishDate = new DateTime?();
        if (AdminBaseClassificationController.IsIterationNode(node))
        {
          DateTime? nullable = operationData.IterationStartDate;
          if (nullable.HasValue)
          {
            ref DateTime? local = ref startDate;
            nullable = operationData.IterationStartDate;
            DateTime dateTime = nullable.Value;
            local = new DateTime?(dateTime);
          }
          nullable = operationData.IterationEndDate;
          if (nullable.HasValue)
          {
            ref DateTime? local = ref finishDate;
            nullable = operationData.IterationEndDate;
            DateTime dateTime = nullable.Value;
            local = new DateTime?(dateTime);
          }
        }
        nodeUri1 = service.CreateNode(this.TfsRequestContext, operationData.NodeName, nodeUri2, startDate, finishDate);
        operationData.NodeId = new Guid(CommonStructureUtils.ExtractNodeId(ref nodeUri1, "nodeUri"));
      }
      catch (Exception ex)
      {
        flag = false;
        str = ex.Message;
        TeamFoundationTrace.Warning(new string[1]
        {
          TraceKeywords.TSWebAccess
        }, "ApiAdminAreaIterationsController: Exception occurred while updating classification node: {0}", (object) ex);
      }
      if (flag)
      {
        string enumerable = !operationData.NodeName.IsNullOrEmpty<char>() ? operationData.NodeName : this.TfsRequestContext.GetService<CommonStructureService>().GetNode(this.TfsRequestContext, nodeUri1).Path;
        if (!enumerable.IsNullOrEmpty<char>())
        {
          IVssRequestContext tfsRequestContext = this.TfsRequestContext;
          string projectActionId = ProcessAuditConstants.GetProjectActionId("ProjectLifecycle" == structureType ? "IterationPath" : "AreaPath", "Create");
          Dictionary<string, object> data = new Dictionary<string, object>();
          data.Add("Path", (object) enumerable);
          data.Add("ProjectName", (object) this.TfsWebContext.ProjectContext?.Name);
          ContextIdentifier projectContext = this.TfsWebContext.ProjectContext;
          Guid guid = projectContext != null ? projectContext.Id : Guid.Empty;
          Guid targetHostId = new Guid();
          Guid projectId = guid;
          tfsRequestContext.LogAuditEvent(projectActionId, data, targetHostId, projectId);
        }
      }
      return (ActionResult) this.Json((object) new
      {
        success = flag,
        message = str,
        node = this.CreateResultNode(operationData, structureType)
      });
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [TfsTraceFilter(501080, 501090)]
    public ActionResult UpdateClassificationNode(
      [ModelBinder(typeof (JsonModelBinder))] ClassificationNodeOperationData operationData,
      bool syncWorkItemTracking = false)
    {
      bool flag = true;
      string str1 = string.Empty;
      string structureType = string.Empty;
      string str2 = "";
      try
      {
        ArgumentUtility.CheckForNull<ClassificationNodeOperationData>(operationData, nameof (operationData));
        ArgumentUtility.CheckForEmptyGuid(operationData.NodeId, "operationData.NodeId");
        string nodeUri1 = CommonStructureUtils.GetNodeUri(operationData.NodeId);
        CommonStructureService service = this.TfsRequestContext.GetService<CommonStructureService>();
        CommonStructureNodeInfo node = service.GetNode(this.TfsRequestContext, nodeUri1);
        structureType = node.StructureType;
        str2 = !operationData.NodeName.IsNullOrEmpty<char>() ? operationData.NodeName : node.Path;
        if (!operationData.ParentId.Equals(Guid.Empty) && !TFStringComparer.IterationName.Equals(node.Name, operationData.NodeName))
          service.RenameNode(this.TfsRequestContext, nodeUri1, operationData.NodeName);
        Guid id = new Guid();
        if (node.ParentUri != null)
          CommonStructureUtils.TryParseNodeUri(node.ParentUri, out id);
        if (!id.Equals(operationData.ParentId))
        {
          string nodeUri2 = CommonStructureUtils.GetNodeUri(operationData.ParentId);
          service.MoveBranch(this.TfsRequestContext, nodeUri1, nodeUri2);
        }
        if (AdminBaseClassificationController.IsIterationNode(node))
        {
          if (this.DatesChanged(node, operationData.IterationStartDate, operationData.IterationEndDate))
            this.UpdateIterationDates(service, nodeUri1, operationData.IterationStartDate, operationData.IterationEndDate);
        }
      }
      catch (Exception ex)
      {
        flag = false;
        str1 = ex.Message;
        TeamFoundationTrace.Warning(new string[1]
        {
          TraceKeywords.TSWebAccess
        }, "ApiAdminAreaIterationsController: Exception occurred while updating classification node: {0}", (object) ex);
      }
      if (flag)
      {
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string projectActionId = ProcessAuditConstants.GetProjectActionId("ProjectLifecycle" == structureType ? "IterationPath" : "AreaPath", "Update");
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("Path", (object) str2);
        data.Add("ProjectName", (object) this.TfsWebContext.ProjectContext?.Name);
        ContextIdentifier projectContext = this.TfsWebContext.ProjectContext;
        Guid guid = projectContext != null ? projectContext.Id : Guid.Empty;
        Guid targetHostId = new Guid();
        Guid projectId = guid;
        tfsRequestContext.LogAuditEvent(projectActionId, data, targetHostId, projectId);
      }
      return (ActionResult) this.Json((object) new
      {
        success = flag,
        message = str1,
        node = this.CreateResultNode(operationData, structureType),
        witSynced = true
      });
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Delete)]
    [TfsTraceFilter(501100, 501110)]
    public ActionResult DeleteClassificationNode([ModelBinder(typeof (JsonModelBinder))] ClassificationNodeOperationData operationData)
    {
      bool flag = true;
      string str1 = string.Empty;
      try
      {
        ArgumentUtility.CheckForNull<ClassificationNodeOperationData>(operationData, nameof (operationData));
        ArgumentUtility.CheckForEmptyGuid(operationData.NodeId, "operationData.NodeId");
        ArgumentUtility.CheckForEmptyGuid(operationData.ParentId, "operationData.ParentId");
        string nodeUri1 = CommonStructureUtils.GetNodeUri(operationData.NodeId);
        string nodeUri2 = CommonStructureUtils.GetNodeUri(operationData.ParentId);
        CommonStructureService service = this.TfsRequestContext.GetService<CommonStructureService>();
        CommonStructureNodeInfo node = service.GetNode(this.TfsRequestContext, nodeUri1);
        service.DeleteBranches(this.TfsRequestContext, new string[1]
        {
          nodeUri1
        }, nodeUri2);
        string str2 = !operationData.NodeName.IsNullOrEmpty<char>() ? operationData.NodeName : node.Path;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string projectActionId = ProcessAuditConstants.GetProjectActionId("ProjectLifecycle" == node.StructureType ? "IterationPath" : "AreaPath", "Delete");
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("Path", (object) str2);
        data.Add("ProjectName", (object) this.TfsWebContext.ProjectContext?.Name);
        ContextIdentifier projectContext = this.TfsWebContext.ProjectContext;
        Guid guid = projectContext != null ? projectContext.Id : Guid.Empty;
        Guid targetHostId = new Guid();
        Guid projectId = guid;
        tfsRequestContext.LogAuditEvent(projectActionId, data, targetHostId, projectId);
      }
      catch (GroupSecuritySubsystemServiceException ex)
      {
        flag = false;
        str1 = ex.Message;
        TeamFoundationTrace.Warning(new string[1]
        {
          TraceKeywords.TSWebAccess
        }, "ApiAdminAreaIterationsController: Exception occurred while updating classification node: {0}", (object) ex);
      }
      return (ActionResult) this.Json((object) new
      {
        success = flag,
        message = str1
      });
    }

    private bool DatesChanged(
      CommonStructureNodeInfo currentNode,
      DateTime? start,
      DateTime? finish)
    {
      return !Nullable.Equals<DateTime>(currentNode.StartDate, start) || !Nullable.Equals<DateTime>(currentNode.FinishDate, finish);
    }

    private void UpdateIterationDates(
      CommonStructureService cssManager,
      string nodeUri,
      DateTime? startDate,
      DateTime? endDate)
    {
      ArgumentUtility.CheckForNull<CommonStructureService>(cssManager, nameof (cssManager));
      ArgumentUtility.CheckStringForNullOrEmpty(nodeUri, nameof (nodeUri));
      cssManager.SetIterationDates(this.TfsRequestContext, nodeUri, startDate, endDate);
    }

    private static bool IsIterationNode(CommonStructureNodeInfo node)
    {
      ArgumentUtility.CheckForNull<CommonStructureNodeInfo>(node, nameof (node));
      return TFStringComparer.CssStructureType.Equals("ProjectLifecycle", node.StructureType);
    }

    private object CreateResultNode(
      ClassificationNodeOperationData operationData,
      string structureType)
    {
      if (operationData == null)
        return (object) null;
      List<object> objectList = new List<object>(3);
      objectList.Add((object) operationData.NodeName);
      if (TFStringComparer.CssStructureType.Equals("ProjectLifecycle", structureType))
      {
        objectList.Add((object) operationData.IterationStartDate);
        objectList.Add((object) operationData.IterationEndDate);
      }
      ILegacyCssUtilsService service = this.TfsRequestContext.GetService<ILegacyCssUtilsService>();
      Guid guid = operationData.NodeId;
      string nodeId = guid.ToString();
      string nodeName = operationData.NodeName;
      guid = operationData.ParentId;
      string parentId = guid.ToString();
      List<object> values = objectList;
      object[] children = new object[0];
      return service.CreateNode(nodeId, nodeName, parentId, (ICollection<object>) values, (IEnumerable<object>) children);
    }

    protected T GetTeamUserSettingValue<T>(string settingName, T defaultValue)
    {
      settingName = this.Team.Id.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture) + settingName;
      using (WebUserSettingsHive userSettingsHive = new WebUserSettingsHive(this.TfsWebContext.TfsRequestContext))
        return userSettingsHive.ReadSetting<T>("/" + settingName, defaultValue);
    }
  }
}
