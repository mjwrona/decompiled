// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions.FieldEntryExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions
{
  public static class FieldEntryExtensions
  {
    public static FieldReference ToFieldReference(
      this FieldEntry field,
      IVssRequestContext tfsRequestContext,
      UrlHelper urlHelper)
    {
      return new FieldReference()
      {
        Id = field.FieldId,
        Name = field.Name,
        RefName = field.ReferenceName,
        Url = WitUrlHelper.GetFieldUrl(tfsRequestContext, field.FieldId, urlHelper)
      };
    }
  }
}
