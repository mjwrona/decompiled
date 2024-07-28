// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Services.TeamFoundationFileServiceWrapper
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Microsoft.TeamFoundation.CodeSense.Server.Services
{
  internal class TeamFoundationFileServiceWrapper : IFileService
  {
    private static Lazy<IFileService> s_instance = new Lazy<IFileService>((Func<IFileService>) (() => (IFileService) new TeamFoundationFileServiceWrapper()), LazyThreadSafetyMode.ExecutionAndPublication);

    public static IFileService GetInstance() => TeamFoundationFileServiceWrapper.s_instance.Value;

    private TeamFoundationFileServiceWrapper()
    {
    }

    public Stream RetrieveCachedFile(IVssRequestContext requestContext, int fileId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      TeamFoundationFileCacheService service = requestContext.GetService<TeamFoundationFileCacheService>();
      bool compressOutput = false;
      using (RestApiDownloadState apiDownloadState = new RestApiDownloadState(requestContext, new HttpResponseMessage(HttpStatusCode.OK)))
      {
        apiDownloadState.PushResponseStream = (Stream) new MemoryStream();
        FileInformation fileInfo = new FileInformation(requestContext.ServiceHost.InstanceId, fileId, (byte[]) null);
        if (service.IsVCCacheEnabled)
        {
          using (new CodeSenseTraceWatch(requestContext, 1024041, true, TraceLayer.ExternalFileService, "RetrieveFile {0} from cache", new object[1]
          {
            (object) fileId
          }))
          {
            if (service.RetrieveFileFromCache<FileInformation>(requestContext, fileInfo, (IDownloadState<FileInformation>) apiDownloadState, compressOutput))
              return apiDownloadState.Response.Content.ReadAsStreamAsync().Result;
          }
        }
        using (new CodeSenseTraceWatch(requestContext, 1024042, true, TraceLayer.ExternalFileService, "RetrieveFile {0} from storage", new object[1]
        {
          (object) fileId
        }))
        {
          Stream databaseStream = apiDownloadState.CacheMiss((FileCacheService) null, fileInfo, compressOutput);
          long position = databaseStream.Position;
          service.RetrieveFileFromDatabase<FileInformation>(requestContext, fileInfo, (IDownloadState<FileInformation>) apiDownloadState, compressOutput, databaseStream, false);
          databaseStream.Position = position;
          return databaseStream;
        }
      }
    }

    public Stream RetrieveFile(IVssRequestContext requestContext, int fileId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      try
      {
        return requestContext.GetService<TeamFoundationFileService>().RetrieveFile(requestContext, (long) fileId, false, out byte[] _, out long _, out CompressionType _);
      }
      catch (FileIdNotFoundException ex)
      {
        requestContext.TraceException(1024044, "CodeSense", TraceLayer.ExternalFramework, (Exception) ex);
      }
      return (Stream) null;
    }

    public int UploadFile(IVssRequestContext requestContext, Stream content)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Stream>(content, nameof (content));
      if (!content.CanSeek)
        throw new ArgumentException("The content stream must be able to seek.");
      TeamFoundationFileService service = requestContext.GetService<TeamFoundationFileService>();
      int num = 0;
      byte[] md5 = MD5Util.CalculateMD5(content, true);
      IVssRequestContext requestContext1 = requestContext;
      ref int local = ref num;
      Stream content1 = content;
      byte[] hashValue = md5;
      long length1 = content.Length;
      long length2 = content.Length;
      Guid empty = Guid.Empty;
      if (service.UploadFile(requestContext1, ref local, content1, hashValue, length1, length2, 0L, CompressionType.None, OwnerId.CodeSense, empty, (string) null, true))
        return num;
      throw new UploadFailedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to upload file"));
    }

    public void DeleteFiles(IVssRequestContext requestContext, IEnumerable<int> fileIds) => requestContext.GetService<TeamFoundationFileService>().DeleteFiles(requestContext, fileIds);
  }
}
