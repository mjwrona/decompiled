// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Sitemap.BlobStorageService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Sitemap
{
  internal class BlobStorageService : IStorageService, IVssFrameworkService
  {
    public int UploadFile(IVssRequestContext requestContext, Stream content, string fileName)
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
      string str = fileName;
      Guid empty = Guid.Empty;
      string fileName1 = str;
      if (service.UploadFile(requestContext1, ref local, content1, hashValue, length1, length2, 0L, CompressionType.None, OwnerId.Gallery, empty, fileName1, true))
        return num;
      throw new UploadFailedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to upload file"));
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
        requestContext.TraceException(12061025, "Gallery", "Sitemap", (Exception) ex);
      }
      return (Stream) null;
    }

    public void DeleteFile(IVssRequestContext requestContext, int fileId) => requestContext.GetService<TeamFoundationFileService>().DeleteFile(requestContext, (long) fileId);

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
