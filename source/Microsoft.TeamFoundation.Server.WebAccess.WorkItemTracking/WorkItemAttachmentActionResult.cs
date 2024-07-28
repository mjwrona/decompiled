// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.WorkItemAttachmentActionResult
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  public class WorkItemAttachmentActionResult : FileResult
  {
    public WorkItemAttachmentActionResult(
      IVssRequestContext tfsRequestContext,
      int fileId,
      string fileName,
      bool contentOnly)
      : base("application/octet-stream")
    {
      this.TfsRequestContext = tfsRequestContext;
      this.FileId = fileId;
      bool flag = true;
      if (contentOnly)
      {
        string mimeMapping = Microsoft.TeamFoundation.Framework.Server.MimeMapping.GetMimeMapping(fileName);
        if (mimeMapping.StartsWith("image/", StringComparison.OrdinalIgnoreCase) && !mimeMapping.StartsWith("image/svg", StringComparison.OrdinalIgnoreCase) || mimeMapping.StartsWith("audio/", StringComparison.OrdinalIgnoreCase) || mimeMapping.StartsWith("video/", StringComparison.OrdinalIgnoreCase) || string.Equals(mimeMapping, "text/plain", StringComparison.OrdinalIgnoreCase))
        {
          flag = false;
          this.ContentTypeOverride = mimeMapping;
        }
      }
      if (!flag)
        return;
      this.FileDownloadName = fileName;
    }

    public IVssRequestContext TfsRequestContext { get; private set; }

    public int FileId { get; private set; }

    private string ContentTypeOverride { get; set; }

    protected override void WriteFile(HttpResponseBase response)
    {
      if (!string.IsNullOrEmpty(this.ContentTypeOverride))
        response.ContentType = this.ContentTypeOverride;
      response.BufferOutput = false;
      this.TfsRequestContext.GetService<WebAccessWorkItemService>().GetFileAttachment(this.TfsRequestContext, response.OutputStream, this.FileId);
    }
  }
}
