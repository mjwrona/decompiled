// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TcmAttachmentsDownloadHandler
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using System;
using System.Collections.Specialized;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class TcmAttachmentsDownloadHandler
  {
    public TcmAttachmentsDownloadHandler(
      IVssRequestContext context,
      NameValueCollection requestParams)
    {
      this.RequestContext = context;
      this.RequestParams = requestParams;
    }

    public bool TryDownloadAndCopyTo(HttpResponseBase response)
    {
      if (!this.ShouldRetrieveFromTcm)
        return false;
      this.RequestContext.GetClient<TestResultsHttpClient>().GetTestResultAttachmentContentAsync(this.ProjectId, this.TestRunId, this.TestResultId, this.AttachmentId, (object) null, new CancellationToken()).Result.CopyTo(response.OutputStream);
      return true;
    }

    private int AttachmentId => int.Parse(this.RequestParams["attachmentId"]);

    private string ProjectId => this.RequestParams["project"];

    private bool ShouldRetrieveFromTcm
    {
      get
      {
        string requestParam = this.RequestParams["storedIn"];
        return !string.IsNullOrEmpty(requestParam) && requestParam.Equals("tcm", StringComparison.OrdinalIgnoreCase);
      }
    }

    private int TestResultId => int.Parse(this.RequestParams["testResultId"]);

    private int TestRunId => int.Parse(this.RequestParams["testRunId"]);

    private IVssRequestContext RequestContext { get; }

    private NameValueCollection RequestParams { get; }
  }
}
