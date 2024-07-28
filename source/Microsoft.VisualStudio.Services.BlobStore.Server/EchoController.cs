// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.EchoController
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "dedup", ResourceName = "echo", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [SetActivityLogAnonymousIdentifier]
  [ClientIgnore]
  public sealed class EchoController : BlobControllerBase
  {
    private static readonly Microsoft.VisualStudio.Services.BlobStore.Common.BlobBlockHash OfZeroBlock = Enumerable.Repeat<byte>((byte) 0, 2097152).ToArray<byte>().CalculateBlockHash((IBlobHasher) Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.Instance);

    [HttpPost]
    [ControllerMethodTraceFilter(5707090)]
    public async Task<HttpResponseMessage> EchoAsync(
      bool hash = false,
      bool base64 = false,
      bool echo = true,
      bool vsoHash = false,
      bool storeInBlobStore = false)
    {
      EchoController echoController = this;
      HttpResponseMessage httpResponseMessage;
      using (IPoolHandle<byte[]> blobBuffer = Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.BorrowBlockBuffer())
      {
        int blobSize = 0;
        using (echoController.TfsRequestContext.RequestTimer.CreateTimeToFirstPageExclusionBlock())
        {
          using (Stream stream = await echoController.Request.Content.ReadAsStreamAsync())
          {
            while (true)
            {
              int num;
              if ((num = await stream.ReadAsync(blobBuffer.Value, blobSize, blobBuffer.Value.Length - blobSize)) != 0)
                blobSize += num;
              else
                break;
            }
          }
        }
        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
        if (hash)
          response.Headers.Add("X-CHUNKID", ChunkDedupIdentifier.CalculateIdentifier(blobBuffer.Value, 0, blobSize).ValueString);
        if (vsoHash)
        {
          using (MemoryStream memoryStream = new MemoryStream(blobBuffer.Value, 0, blobSize))
            response.Headers.Add("X-BLOBID", Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.CalculateBlobIdentifierWithBlocks((Stream) memoryStream).BlobId.ValueString);
        }
        if (storeInBlobStore)
        {
          if (blobSize != 2097152)
            throw new ArgumentException();
          await echoController.TfsRequestContext.GetService<IBlobStore>().PutBlobBlockAsync(echoController.TfsRequestContext, WellKnownDomainIds.DefaultDomainId, new BlobIdentifier(BlobIdentifier.MaxValue.AlgorithmResultBytes, (byte) 0), blobBuffer.Value, blobSize, EchoController.OfZeroBlock);
        }
        if (base64)
        {
          string base64String = Convert.ToBase64String(blobBuffer.Value, 0, blobSize);
          response.Headers.Add("X-BASE64LENGTH", base64String.Length.ToString());
          if (echo)
            response.Content = (HttpContent) new StringContent(base64String);
        }
        else if (echo)
        {
          byte[] numArray = new byte[blobSize];
          System.Buffer.BlockCopy((Array) blobBuffer.Value, 0, (Array) numArray, 0, blobSize);
          response.Content = (HttpContent) new ByteArrayContent(numArray);
        }
        httpResponseMessage = response;
      }
      return httpResponseMessage;
    }
  }
}
