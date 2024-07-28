// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardsAdminDataSource
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class BoardsAdminDataSource
  {
    public static object GetIterations(IVssRequestContext tfsRequestContext, string projectName)
    {
      TFCommonUtil.CheckProjectUri(ref projectName, false);
      return tfsRequestContext.GetService<ILegacyCssUtilsService>().GetTreeValues(tfsRequestContext, projectName, TreeStructureType.Iteration, out int _);
    }

    public static JsObject HasPermissions(
      IVssRequestContext tfsRequestContext,
      Guid nodeId,
      bool genericReadAccess = false,
      bool genericWriteAccess = false,
      bool createChildrenAccess = false,
      bool deleteAccess = false)
    {
      bool flag = false;
      string str = (string) null;
      try
      {
        string nodeUri = CommonStructureUtils.GetNodeUri(nodeId);
        IntegrationSecurityManager service = tfsRequestContext.GetService<IntegrationSecurityManager>();
        if (genericReadAccess)
          service.CheckObjectPermission(tfsRequestContext, nodeUri, "GENERIC_READ");
        if (genericWriteAccess)
          service.CheckObjectPermission(tfsRequestContext, nodeUri, "GENERIC_WRITE");
        if (createChildrenAccess)
          service.CheckObjectPermission(tfsRequestContext, nodeUri, "CREATE_CHILDREN");
        if (deleteAccess)
          service.CheckObjectPermission(tfsRequestContext, nodeUri, "DELETE");
      }
      catch (Exception ex)
      {
        flag = true;
        str = ex.Message;
        switch (ex)
        {
          case UnauthorizedAccessException _:
          case GroupSecuritySubsystemServiceException _:
            break;
          default:
            TeamFoundationTrace.TraceException(ex);
            break;
        }
      }
      JsObject jsObject = new JsObject();
      jsObject["hasPermission"] = (object) !flag;
      jsObject["errorMessage"] = (object) str;
      return jsObject;
    }

    public static IEnumerable<JsObject> MigrateProjectsProcess(
      IVssRequestContext tfsRequestContext,
      IEnumerable<ProcessMigrationModel> migratingProjects)
    {
      return tfsRequestContext.GetService<IWorkItemTrackingProcessService>().MigrateProjectsProcess(tfsRequestContext, migratingProjects).Select<ProcessMigrationResult, JsObject>((Func<ProcessMigrationResult, JsObject>) (r => r.ToJson()));
    }
  }
}
