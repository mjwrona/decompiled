// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CssUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class CssUtils
  {
    public static Guid GetId(this CommonStructureProjectInfo project)
    {
      ArgumentUtility.CheckForNull<CommonStructureProjectInfo>(project, nameof (project));
      return new Guid(LinkingUtilities.DecodeUri(project.Uri.Trim()).ToolSpecificId);
    }

    public static Guid GetId(this CommonStructureNodeInfo node)
    {
      ArgumentUtility.CheckForNull<CommonStructureNodeInfo>(node, nameof (node));
      return new Guid(LinkingUtilities.DecodeUri(node.Uri.Trim()).ToolSpecificId);
    }

    public static int NodeComparison(TreeNode x, TreeNode y)
    {
      ArgumentUtility.CheckForNull<TreeNode>(x, nameof (x));
      ArgumentUtility.CheckForNull<TreeNode>(y, nameof (y));
      int num = DateTime.Compare(CssUtils.DateComparisonValue(x.StartDate), CssUtils.DateComparisonValue(y.StartDate));
      if (num == 0)
      {
        num = DateTime.Compare(CssUtils.DateComparisonValue(x.FinishDate), CssUtils.DateComparisonValue(y.FinishDate));
        if (num == 0)
          num = TFStringComparer.CssTreePathName.Compare(x.Name, y.Name);
      }
      return num;
    }

    private static DateTime DateComparisonValue(DateTime? date) => !date.HasValue ? DateTime.MaxValue.Date : date.Value.Date;

    public static string GetWitPath(this CommonStructureNodeInfo node)
    {
      ArgumentUtility.CheckForNull<CommonStructureNodeInfo>(node, nameof (node));
      return CssUtils.GetWitPathFromCssPath(node.Path);
    }

    public static string GetWitPathFromCssPath(string cssPath)
    {
      if (string.IsNullOrEmpty(cssPath))
        return cssPath;
      return string.Join("\\", ((IEnumerable<string>) cssPath.Split(new string[1]
      {
        "\\"
      }, StringSplitOptions.RemoveEmptyEntries)).Where<string>((Func<string, int, bool>) ((s, index) => index != 1)));
    }

    public static CommonStructureProjectInfo GetProjectFromUri(
      IVssRequestContext requestContext,
      string projectUri)
    {
      Guid id;
      CommonStructureUtils.CheckAndNormalizeUri(projectUri, nameof (projectUri), true, out id);
      return CommonStructureProjectInfo.ConvertProjectInfo(requestContext.GetService<IProjectService>().GetProject(requestContext, id));
    }

    public static CommonStructureProjectInfo GetProjectFromId(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return CommonStructureProjectInfo.ConvertProjectInfo(requestContext.GetService<IProjectService>().GetProject(requestContext, projectId));
    }
  }
}
