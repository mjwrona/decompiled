// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.GenericDownloadHandler
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;
using System.Web;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class GenericDownloadHandler
  {
    private const string c_area = "File";
    private const string c_layer = "GenericDownloadHandler";
    private static readonly RegistryQuery s_downloadTimeoutQuery = new RegistryQuery(FrameworkServerConstants.FileServiceHttpTimeout);

    public void DownloadFile(
      IVssRequestContext requestContext,
      DownloadContext downloadContext,
      HttpContextBase httpContextBase,
      HttpResponseBase response,
      GenericDownloadHandler.HandleErrorDelegate errorDelegate,
      bool? useCache = null)
    {
      DateTime utcNow = DateTime.UtcNow;
      TeamFoundationFileCacheService service = requestContext.GetService<TeamFoundationFileCacheService>();
      bool valueOrDefault = useCache.GetValueOrDefault(service.IsVCCacheEnabled);
      this.SetTimeout(requestContext, httpContextBase);
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      using (ISigner signer = vssRequestContext.GetService<ITeamFoundationSigningService>().GetSigner(vssRequestContext, ProxyConstants.ProxySigningKey))
      {
        downloadContext.DemandValidSignature(signer, utcNow);
        if (downloadContext.FileId == 1023)
          throw new DestroyedContentUnavailableException();
        try
        {
          using (MidTierDownloadState tierDownloadState = new MidTierDownloadState(response, requestContext))
          {
            FileInformation fileInfo = new FileInformation(requestContext.ServiceHost.InstanceId, downloadContext.FileId, (byte[]) null);
            if (valueOrDefault)
              service.RetrieveFile<FileInformation>(requestContext, fileInfo, (IDownloadState<FileInformation>) tierDownloadState, true);
            else
              service.RetrieveFileFromDatabase<FileInformation>(requestContext, fileInfo, (IDownloadState<FileInformation>) tierDownloadState, true, (Stream) null, false);
          }
        }
        catch (ClientCancelException ex)
        {
        }
        catch (FileNotFoundException ex)
        {
          requestContext.TraceException(0, "File", nameof (GenericDownloadHandler), (Exception) ex);
          errorDelegate((Exception) ex, "X-VersionControl-Exception", 404, false);
        }
        catch (FileIdNotFoundException ex)
        {
          requestContext.TraceException(0, "File", nameof (GenericDownloadHandler), (Exception) ex);
          errorDelegate((Exception) ex, "X-VersionControl-Exception", 404, false);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(0, "File", nameof (GenericDownloadHandler), ex);
          errorDelegate(ex, "X-VersionControl-Exception", 500, true);
        }
      }
    }

    internal void SetTimeout(IVssRequestContext requestContext, HttpContextBase httpContextBase)
    {
      int timeoutSeconds = Math.Max(30, requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in GenericDownloadHandler.s_downloadTimeoutQuery, true, 14400));
      httpContextBase.OverrideRequestTimeoutSeconds(timeoutSeconds);
    }

    public delegate void HandleErrorDelegate(
      Exception exception,
      string exceptionHeader,
      int statusCode,
      bool responseStarted);
  }
}
