// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemReference
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Common;
using System.Runtime.Serialization;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model
{
  [DataContract]
  public class WorkItemReference
  {
    public WorkItemReference()
    {
    }

    public WorkItemReference(
      int workItemId,
      IVssRequestContext tfsRequestContext,
      UrlHelper urlHelper)
    {
      ITswaServerHyperlinkService service = tfsRequestContext.GetService<ITswaServerHyperlinkService>();
      this.Id = workItemId;
      this.Url = WitUrlHelper.GetWorkItemUrl(tfsRequestContext, workItemId, urlHelper);
      this.WebUrl = service.GetWorkItemEditorUrl(tfsRequestContext, workItemId).AbsoluteUri;
    }

    [DataMember]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Rev { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string WebUrl { get; set; }
  }
}
