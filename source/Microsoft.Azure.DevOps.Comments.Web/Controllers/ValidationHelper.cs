// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.Web.Controllers.ValidationHelper
// Assembly: Microsoft.Azure.DevOps.Comments.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6538262-E3F2-45F5-B799-587642D68EAC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.Web.dll

using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Azure.DevOps.Comments.Web.Controllers
{
  internal class ValidationHelper
  {
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
        int maxAllowedPageSize = CommentService.MaxAllowedPageSize;
        if (!(nullable2.GetValueOrDefault() > maxAllowedPageSize & nullable2.HasValue))
          return;
      }
      throw new VssPropertyValidationException(nameof (top), ResourceStrings.QueryParameterOutOfRangeWithRangeValues((object) nameof (top), (object) 1, (object) CommentService.MaxAllowedPageSize));
    }
  }
}
