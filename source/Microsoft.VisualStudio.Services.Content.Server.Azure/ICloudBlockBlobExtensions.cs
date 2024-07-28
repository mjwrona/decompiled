// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.ICloudBlockBlobExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public static class ICloudBlockBlobExtensions
  {
    public static async Task<byte[]> DownloadAsByteArrayAsync(
      this ICloudBlockBlob blob,
      VssRequestPump.Processor processor,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      Exception innerException = (Exception) null;
      for (int i = 0; i < 3; ++i)
      {
        byte[] array;
        try
        {
          long? length = blob.Properties?.Length;
          long? nullable = !length.HasValue || length.Value > 0L ? length : new long?();
          using (MemoryStream ms = !nullable.HasValue ? new MemoryStream() : new MemoryStream((int) nullable.Value))
          {
            await blob.DownloadToStreamNeedsRetryAsync(processor, (Stream) ms, accessCondition, options, operationContext).ConfigureAwait(false);
            array = ms.ToArray();
          }
        }
        catch (StorageException ex) when (
        {
          // ISSUE: unable to correctly present filter
          int num;
          if (accessCondition == null)
          {
            StorageException exc = ex;
            HttpStatusCode[] httpStatusCodeArray = new HttpStatusCode[1]
            {
              HttpStatusCode.PreconditionFailed
            };
            num = exc.ContainsStorageExceptionWithHttpStatus(httpStatusCodeArray) ? 1 : 0;
          }
          else
            num = 0;
          if ((uint) num > 0U)
          {
            SuccessfulFiltering;
          }
          else
            throw;
        }
        )
        {
          innerException = (Exception) ex;
          continue;
        }
        return array;
      }
      throw new TimeoutException(string.Format("Could not download blob due to concurrent modifications despite {0} retries.", (object) 3), innerException);
    }
  }
}
