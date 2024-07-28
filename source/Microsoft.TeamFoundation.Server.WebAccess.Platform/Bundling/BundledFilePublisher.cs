// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundledFilePublisher
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  internal class BundledFilePublisher : 
    WaitHandleLock<Tuple<IVssRequestContext, Dictionary<string, string[]>, bool>, bool?>
  {
    private static readonly TimeSpan s_singleWaitTime = TimeSpan.FromSeconds(20.0);
    private static readonly TimeSpan s_overallWaitTime = TimeSpan.FromSeconds(60.0);
    private static readonly string s_tempBlobPrefix = "bundles/temp/";
    private ITeamFoundationFileService m_fileService;
    private IBlobProvider m_cdnBlobProvider;
    private string m_cdnContainerName;
    private BundleDefinition m_definition;
    private UrlHelper m_urlHelper;
    private BundledFile m_bundledFile;
    private string m_contentType;
    private string m_bundleNamePrefix;

    public BundledFilePublisher(
      TeamFoundationFileService fileService,
      IBlobProvider cdnBlobProvider,
      string cdnContainerName,
      BundleDefinition definition,
      UrlHelper urlHelper,
      BundledFile bundledFile,
      string contentType,
      string bundleNamePrefix)
      : base(BundledFilePublisher.s_singleWaitTime, BundledFilePublisher.s_overallWaitTime)
    {
      this.m_fileService = (ITeamFoundationFileService) fileService;
      this.m_cdnBlobProvider = cdnBlobProvider;
      this.m_cdnContainerName = cdnContainerName;
      this.m_definition = definition;
      this.m_urlHelper = urlHelper;
      this.m_bundledFile = bundledFile;
      this.m_contentType = contentType;
      this.m_bundleNamePrefix = bundleNamePrefix;
      this.Action = (Func<Tuple<IVssRequestContext, Dictionary<string, string[]>, bool>, bool?>) (args => this.EnsureBundleIsPublishedPrivate(args.Item1, args.Item2, args.Item3));
      this.TimeOutExceptionMessage = "Timed out waiting to ensure bundle is published.";
    }

    public BundledFile BundledFile => this.m_bundledFile;

    public bool IsBundlePublishCheckComplete { get; private set; }

    public bool EnsureBundleIsPublished(
      IVssRequestContext requestContext,
      Dictionary<string, string[]> contentCache,
      bool skipCdn = false)
    {
      return this.PerformAction(new Tuple<IVssRequestContext, Dictionary<string, string[]>, bool>(requestContext, contentCache, skipCdn)).Value;
    }

    private IEnumerable<IBundleStreamProvider> GetBundleStreamProviders(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("VisualStudio.Services.WebAccess.StreamBundles") ? this.m_definition.GetBundleStreamProviders(requestContext, this.m_bundledFile, this.m_urlHelper) : (IEnumerable<IBundleStreamProvider>) null;
    }

    private bool? EnsureBundleIsPublishedPrivate(
      IVssRequestContext requestContext,
      Dictionary<string, string[]> contentCache,
      bool skipCdn)
    {
      skipCdn = ((requestContext.IsAnonymousPrincipal() ? 1 : (requestContext.IsPublicUser() ? 1 : 0)) | (skipCdn ? 1 : 0)) != 0;
      bool flag = !this.HasBundleBeenPublishedToFileService(requestContext);
      if (flag)
      {
        IEnumerable<IBundleStreamProvider> bundleStreamProviders = this.GetBundleStreamProviders(requestContext);
        if (bundleStreamProviders != null)
        {
          using (new BundleStream(bundleStreamProviders))
            this.PublishBundleContent(requestContext, (string) null, bundleStreamProviders, contentCache, skipCdn);
        }
        else
        {
          string bundleContent = this.m_definition.GetBundleContent(requestContext, this.m_bundledFile, this.m_urlHelper, contentCache);
          this.PublishBundleContent(requestContext, bundleContent, (IEnumerable<IBundleStreamProvider>) null, contentCache, skipCdn);
        }
      }
      else if (this.m_cdnBlobProvider != null && !skipCdn)
        this.StartCdnUploadTask(requestContext, (byte[]) null, contentCache, this.GetBundleStreamProviders(requestContext));
      this.IsBundlePublishCheckComplete = true;
      return new bool?(flag);
    }

    private bool HasBundleBeenPublishedToFileService(IVssRequestContext requestContext) => this.m_fileService.TryGetFileId(requestContext, OwnerId.WebAccess, this.m_bundledFile.Name, out int _);

    private void PublishBundleContent(
      IVssRequestContext requestContext,
      string bundleContent,
      IEnumerable<IBundleStreamProvider> bundleStreamProviders,
      Dictionary<string, string[]> contentCache,
      bool skipCdn)
    {
      if (bundleStreamProviders == null)
      {
        if (string.IsNullOrEmpty(bundleContent))
        {
          this.m_bundledFile.IsEmpty = true;
        }
        else
        {
          byte[] bytes = Encoding.UTF8.GetBytes(bundleContent);
          if (this.m_cdnBlobProvider != null && !skipCdn)
            this.StartCdnUploadTask(requestContext, bytes, (Dictionary<string, string[]>) null, (IEnumerable<IBundleStreamProvider>) null);
          using (requestContext.AllowAnonymousOrPublicUserWrites())
            this.UploadBundleContent(requestContext, bytes, (IEnumerable<IBundleStreamProvider>) null);
        }
      }
      else if (!bundleStreamProviders.Any<IBundleStreamProvider>())
      {
        this.m_bundledFile.IsEmpty = true;
      }
      else
      {
        using (requestContext.AllowAnonymousOrPublicUserWrites())
          this.UploadBundleContent(requestContext, (byte[]) null, bundleStreamProviders);
        if (this.m_cdnBlobProvider == null || skipCdn)
          return;
        this.StartCdnUploadTask(requestContext, (byte[]) null, (Dictionary<string, string[]>) null, bundleStreamProviders);
      }
    }

    private void UploadBundleContent(
      IVssRequestContext requestContext,
      byte[] bundleContentBytes,
      IEnumerable<IBundleStreamProvider> bundleStreamProviders)
    {
      string methodName = nameof (UploadBundleContent);
      requestContext.TraceEnter(15060001, BundlingService.s_area, BundlingService.s_layer, methodName);
      try
      {
        string fileName = "bundle-" + Guid.NewGuid().ToString();
        DateTime now = DateTime.Now;
        bool flag = bundleStreamProviders != null;
        long fileId;
        if (flag)
        {
          using (Stream content = (Stream) new BundleStream(bundleStreamProviders))
            fileId = this.m_fileService.UploadFile64(requestContext, content, OwnerId.WebAccess, Guid.Empty, fileName);
        }
        else
        {
          using (Stream content = (Stream) new MemoryStream(bundleContentBytes))
            fileId = this.m_fileService.UploadFile64(requestContext, content, OwnerId.WebAccess, Guid.Empty, fileName);
        }
        TimeSpan timeSpan = DateTime.Now - now;
        BundlingService.HttpContextInfoForCi contextInfoForCi = this.GetHttpContextInfoForCi();
        CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
        CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
        intelligenceData.Add("action", "UploadBundleToFileService");
        intelligenceData.Add("bundleFileName", this.m_bundledFile.Name);
        intelligenceData.Add("bundleLength", (double) this.m_bundledFile.ContentLength);
        intelligenceData.Add("bundleIntegrity", this.m_bundledFile.Integrity ?? string.Empty);
        intelligenceData.Add("tempFileName", fileName);
        intelligenceData.Add("url", (object) contextInfoForCi.Url);
        intelligenceData.Add("routeAction", contextInfoForCi.Action);
        intelligenceData.Add("controller", contextInfoForCi.Controller);
        intelligenceData.Add("duration", (double) (long) timeSpan.TotalMilliseconds);
        intelligenceData.Add("useStreaming", flag);
        IVssRequestContext requestContext1 = requestContext;
        string layer = BundlingService.s_layer;
        string area = BundlingService.s_area;
        CustomerIntelligenceData properties = intelligenceData;
        service.Publish(requestContext1, layer, area, properties);
        try
        {
          this.m_fileService.RenameFile(requestContext, fileId, this.m_bundledFile.Name);
        }
        catch (DuplicateFileNameException ex)
        {
          this.m_fileService.DeleteFile(requestContext, fileId);
          requestContext.Trace(15060002, TraceLevel.Error, BundlingService.s_area, BundlingService.s_layer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Bundle {0} already exists, bundle deleted and using the existing bundle.", (object) this.m_bundledFile.Name));
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15060003, BundlingService.s_area, BundlingService.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(15060004, BundlingService.s_area, BundlingService.s_layer, methodName);
      }
    }

    private void StartCdnUploadTask(
      IVssRequestContext requestContext,
      byte[] bundleContentBytes,
      Dictionary<string, string[]> contentCache,
      IEnumerable<IBundleStreamProvider> bundleStreamProviders)
    {
      BundledFilePublisher.UploadBundleContentToCdnArgs taskArgs = new BundledFilePublisher.UploadBundleContentToCdnArgs()
      {
        BundleContentBytes = bundleContentBytes,
        HttpContextInfo = this.GetHttpContextInfoForCi(),
        ContentCache = contentCache,
        BundleStreamProviders = bundleStreamProviders
      };
      requestContext.GetService<TeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.UploadBundleContentToCdn), (object) taskArgs, 0));
    }

    private void UploadBundleContentToCdn(IVssRequestContext requestContext, object argsObject)
    {
      requestContext.TraceEnter(15060021, BundlingService.s_area, BundlingService.s_layer, "UploadBundleContentToCDN");
      try
      {
        BundledFilePublisher.UploadBundleContentToCdnArgs contentToCdnArgs = argsObject as BundledFilePublisher.UploadBundleContentToCdnArgs;
        string str1 = this.m_bundleNamePrefix + this.m_bundledFile.Name;
        requestContext.Trace(15060013, TraceLevel.Info, BundlingService.s_area, BundlingService.s_layer, "Ensuring bundle {0} exists in the CDN blob storage.", (object) this.m_bundledFile.Name);
        if (!this.m_cdnBlobProvider.BlobExists(requestContext, this.m_cdnContainerName, str1))
        {
          string str2 = BundledFilePublisher.s_tempBlobPrefix + Guid.NewGuid().ToString();
          requestContext.Trace(15060014, TraceLevel.Info, BundlingService.s_area, BundlingService.s_layer, "Bundle {0} does not exists in the CDN blob storage. Starting upload to {1}.", (object) this.m_bundledFile.Name, (object) str2);
          bool flag = contentToCdnArgs.BundleStreamProviders != null;
          TimeSpan timeSpan;
          if (flag)
          {
            using (MemoryStream memoryStream = new MemoryStream())
            {
              using (GZipStream destination = new GZipStream((Stream) memoryStream, CompressionMode.Compress, true))
              {
                using (BundleStream bundleStream = new BundleStream(contentToCdnArgs.BundleStreamProviders))
                  bundleStream.CopyTo((Stream) destination);
              }
              memoryStream.Seek(0L, SeekOrigin.Begin);
              DateTime now = DateTime.Now;
              this.m_cdnBlobProvider.PutStream(requestContext, this.m_cdnContainerName, str2, (Stream) memoryStream, (IDictionary<string, string>) null);
              timeSpan = DateTime.Now - now;
            }
          }
          else
          {
            byte[] buffer = contentToCdnArgs.BundleContentBytes == null ? Encoding.UTF8.GetBytes(this.m_definition.GetBundleContent(requestContext, this.m_bundledFile, this.m_urlHelper, contentToCdnArgs.ContentCache)) : contentToCdnArgs.BundleContentBytes;
            using (MemoryStream memoryStream1 = new MemoryStream())
            {
              using (GZipStream destination = new GZipStream((Stream) memoryStream1, CompressionMode.Compress, true))
              {
                using (MemoryStream memoryStream2 = new MemoryStream(buffer))
                  memoryStream2.CopyTo((Stream) destination);
              }
              memoryStream1.Seek(0L, SeekOrigin.Begin);
              DateTime now = DateTime.Now;
              this.m_cdnBlobProvider.PutStream(requestContext, this.m_cdnContainerName, str2, (Stream) memoryStream1, (IDictionary<string, string>) null);
              timeSpan = DateTime.Now - now;
            }
          }
          CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
          CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
          intelligenceData.Add("action", "UploadBundleToCdnBlobStorage");
          intelligenceData.Add("bundleFileName", this.m_bundledFile.Name);
          intelligenceData.Add("bundleLength", (double) this.m_bundledFile.ContentLength);
          intelligenceData.Add("bundleIntegrity", this.m_bundledFile.Integrity ?? string.Empty);
          intelligenceData.Add("tempBlobName", str2);
          intelligenceData.Add("containerName", this.m_cdnContainerName);
          intelligenceData.Add("url", (object) contentToCdnArgs.HttpContextInfo.Url);
          intelligenceData.Add("routeAction", contentToCdnArgs.HttpContextInfo.Action);
          intelligenceData.Add("controller", contentToCdnArgs.HttpContextInfo.Controller);
          intelligenceData.Add("duration", (double) (long) timeSpan.TotalMilliseconds);
          intelligenceData.Add("useStreaming", flag);
          IVssRequestContext requestContext1 = requestContext;
          string layer = BundlingService.s_layer;
          string area = BundlingService.s_area;
          CustomerIntelligenceData properties = intelligenceData;
          service.Publish(requestContext1, layer, area, properties);
          requestContext.Trace(15060026, TraceLevel.Info, BundlingService.s_area, BundlingService.s_layer, "Setting headers on blob: {0}", (object) str1);
          this.m_cdnBlobProvider.SetBlobHeaders(requestContext, this.m_cdnContainerName, str2, "public, max-age=31536000", this.m_contentType, (string) null, "gzip", (string) null);
          requestContext.Trace(15060015, TraceLevel.Info, BundlingService.s_area, BundlingService.s_layer, "Renaming temp blob {0} back to {1}.", (object) str2, (object) str1);
          try
          {
            this.m_cdnBlobProvider.RenameBlob(requestContext, this.m_cdnContainerName, str2, str1);
            this.m_bundledFile.CDNRelativeUri = this.GetBundleCdnUri();
            requestContext.Trace(15060016, TraceLevel.Info, BundlingService.s_area, BundlingService.s_layer, "Bundle {0} successfully uploaded to CDN blob storage", (object) this.m_bundledFile.Name);
          }
          catch (Exception ex1)
          {
            requestContext.Trace(15060017, TraceLevel.Info, BundlingService.s_area, BundlingService.s_layer, "Error renaming temp blob {0} back to {1}: {2}", (object) str2, (object) str1, (object) ex1);
            if (this.m_cdnBlobProvider.BlobExists(requestContext, this.m_cdnContainerName, str1))
            {
              this.m_bundledFile.CDNRelativeUri = this.GetBundleCdnUri();
              requestContext.Trace(15060018, TraceLevel.Info, BundlingService.s_area, BundlingService.s_layer, "Deleting temporary blob: {0}", (object) str2);
              try
              {
                this.m_cdnBlobProvider.DeleteBlob(requestContext, this.m_cdnContainerName, str2);
              }
              catch (Exception ex2)
              {
                requestContext.TraceException(15060019, TraceLevel.Info, BundlingService.s_area, BundlingService.s_layer, ex2);
              }
            }
            else
              throw;
          }
        }
        else
        {
          this.m_bundledFile.CDNRelativeUri = this.GetBundleCdnUri();
          requestContext.Trace(15060013, TraceLevel.Info, BundlingService.s_area, BundlingService.s_layer, "Bundle {0} already exists in the CDN blob storage.", (object) this.m_bundledFile.Name);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15060021, BundlingService.s_area, BundlingService.s_layer, ex);
      }
      finally
      {
        requestContext.TraceLeave(15060022, BundlingService.s_area, BundlingService.s_layer, "UploadBundleContentToCDN");
      }
    }

    private BundlingService.HttpContextInfoForCi GetHttpContextInfoForCi()
    {
      BundlingService.HttpContextInfoForCi contextInfoForCi = new BundlingService.HttpContextInfoForCi();
      if (HttpContext.Current != null && HttpContext.Current.Request != null)
      {
        HttpRequest request = HttpContext.Current.Request;
        if (request.RequestContext != null && request.RequestContext.RouteData != null && request.RequestContext.RouteData.Values != null)
        {
          object obj1;
          request.RequestContext.RouteData.Values.TryGetValue("action", out obj1);
          if (obj1 != null)
            contextInfoForCi.Action = obj1.ToString();
          object obj2;
          request.RequestContext.RouteData.Values.TryGetValue("controller", out obj2);
          if (obj2 != null)
            contextInfoForCi.Controller = obj2.ToString();
        }
        contextInfoForCi.Url = !(contextInfoForCi.Controller == BundlingService.s_bundlingControllerName) ? request.Url : request.UrlReferrer;
      }
      return contextInfoForCi;
    }

    private string GetBundleCdnUri() => this.m_bundleNamePrefix + this.m_bundledFile.Name;

    private class UploadBundleContentToCdnArgs
    {
      public byte[] BundleContentBytes { get; set; }

      public BundlingService.HttpContextInfoForCi HttpContextInfo { get; set; }

      public Dictionary<string, string[]> ContentCache { get; set; }

      public IEnumerable<IBundleStreamProvider> BundleStreamProviders { get; set; }
    }
  }
}
