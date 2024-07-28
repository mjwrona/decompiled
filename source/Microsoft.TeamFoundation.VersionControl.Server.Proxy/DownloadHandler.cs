// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Proxy.DownloadHandler
// Assembly: Microsoft.TeamFoundation.VersionControl.Server.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3F3DC329-13F2-42E8-9562-94C7348523BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.Proxy.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Proxy
{
  public class DownloadHandler : ProxyHttpHandler
  {
    protected override void Execute()
    {
      HttpContext current = HttpContext.Current;
      HttpRequestBase httpRequestBase = (HttpRequestBase) new HttpRequestWrapper(current.Request);
      HttpResponseBase response = (HttpResponseBase) new HttpResponseWrapper(current.Response);
      try
      {
        this.EnterMethod(new MethodInformation("ProxyDownloadHandler", MethodType.Normal, EstimatedMethodCost.VeryLow));
        DateTime utcNow = DateTime.UtcNow;
        DownloadContext downloadContext = new DownloadContext(httpRequestBase.QueryString);
        if (downloadContext.FileId == FrameworkServerConstants.VersionControlDestroyedContentFileId)
          throw new DestroyedContentUnavailableException();
        TeamFoundationFileCacheService service = this.RequestContext.GetService<TeamFoundationFileCacheService>();
        Repository repository = this.RequestContext.GetService<TeamFoundationProxyRepositoryService>().GetRepository((IProxyConfiguration) service.Configuration, downloadContext.RepositoryInfo);
        try
        {
          downloadContext.DemandValidSignature(repository.TicketSigner, utcNow);
        }
        catch (DownloadTicketValidationException ex)
        {
          repository.InvalidateRepositoryProperties();
          downloadContext.DemandValidSignature(repository.TicketSigner, utcNow);
        }
        using (ProxyDownloadState proxyDownloadState = new ProxyDownloadState(response, this.RequestContext, downloadContext))
        {
          FileInformation fileInfo = new FileInformation(new Guid(downloadContext.RepositoryInfo.RepositoryId), downloadContext.FileId, (byte[]) null);
          service.RetrieveFile<FileInformation>(this.RequestContext, fileInfo, (IDownloadState<FileInformation>) proxyDownloadState, true);
        }
      }
      catch (Exception ex1)
      {
        if (!(ex1 is HttpException))
        {
          try
          {
            if (ex1 is TeamFoundationServiceException serviceException && serviceException.LogException)
              TeamFoundationEventLog.Default.LogException(httpRequestBase.Params.ToString(), ex1, TeamFoundationEventId.DefaultExceptionEventId, EventLogEntryType.Error);
            response.StatusCode = 500;
            response.TrySkipIisCustomErrors = true;
            response.AddHeader("X-VersionControl-Exception", ex1.GetType().Name);
            response.ContentType = "text/plain";
            string s = SecretUtility.ScrubSecrets(ex1.Message);
            response.Write(s);
            return;
          }
          catch (Exception ex2)
          {
          }
        }
        try
        {
          response.Close();
        }
        catch (Exception ex3)
        {
        }
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
