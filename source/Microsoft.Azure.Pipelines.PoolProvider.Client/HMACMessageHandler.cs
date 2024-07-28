// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.PoolProvider.Client.HMACMessageHandler
// Assembly: Microsoft.Azure.Pipelines.PoolProvider.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4D55F5C7-EE6B-4E5B-8407-D17F3B35057D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.PoolProvider.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Pipelines.PoolProvider.Client
{
  public class HMACMessageHandler : DelegatingHandler
  {
    private HMACSHA512 m_hashAlgorithm;
    private const string c_header = "X-Azure-Signature";

    public HMACMessageHandler(byte[] sharedSecret) => this.m_hashAlgorithm = new HMACSHA512(sharedSecret);

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (request?.Content != null)
      {
        HMACSHA512 hmacshA512 = this.m_hashAlgorithm;
        byte[] hash = hmacshA512.ComputeHash(await request.Content.ReadAsStreamAsync());
        hmacshA512 = (HMACSHA512) null;
        request.Headers.Add("X-Azure-Signature", HexConverter.ToStringLowerCase(hash));
      }
      return await base.SendAsync(request, cancellationToken);
    }
  }
}
