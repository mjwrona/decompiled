// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AttachmentPage
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class AttachmentPage : TeamFoundationPage
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      TfsTestManagementRequestContext context = (TfsTestManagementRequestContext) null;
      try
      {
        this.EnterMethod(new MethodInformation("TCMAttachmentPage", MethodType.Normal, EstimatedMethodCost.Low));
        string toolSpecificId = this.Request["artifactMoniker"];
        if (string.IsNullOrEmpty(toolSpecificId))
        {
          this.Response.StatusCode = 400;
        }
        else
        {
          context = new TfsTestManagementRequestContext(this.RequestContext);
          int testRunId;
          int testResultId;
          int attachmentId;
          int sessionId;
          if (!ArtifactHelper.ParseTestCaseResultAttachmentId(toolSpecificId, out testRunId, out testResultId, out attachmentId, out sessionId))
            return;
          AttachmentDownloadHelper.ProcessDownload((TestManagementRequestContext) context, testRunId, testResultId, attachmentId, sessionId, this.Response);
        }
      }
      catch (Exception ex)
      {
        if (context != null)
          context.TraceException("Pages", ex);
        this.Response.StatusCode = 500;
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
