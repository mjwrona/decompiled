// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments
{
  internal class ValidationHelper
  {
    internal static void ValidateWorkItemId(int workItemId)
    {
      if (workItemId <= 0)
        throw new VssPropertyValidationException(nameof (workItemId), ResourceStrings.QueryParameterOutOfRange((object) nameof (workItemId)));
    }

    internal static void ValidateCommentId(int commentId)
    {
      if (commentId <= 0)
        throw new VssPropertyValidationException(nameof (commentId), ResourceStrings.QueryParameterOutOfRange((object) nameof (commentId)));
    }

    internal static void ValidateTop(int? top)
    {
      int? nullable1 = top;
      int num = 1;
      if (!(nullable1.GetValueOrDefault() < num & nullable1.HasValue))
      {
        int? nullable2 = top;
        int maxAllowedPageSize = WorkItemCommentService.MaxAllowedPageSize;
        if (!(nullable2.GetValueOrDefault() > maxAllowedPageSize & nullable2.HasValue))
          return;
      }
      throw new VssPropertyValidationException(nameof (top), ResourceStrings.QueryParameterOutOfRangeWithRangeValues((object) nameof (top), (object) 1, (object) WorkItemCommentService.MaxAllowedPageSize));
    }
  }
}
