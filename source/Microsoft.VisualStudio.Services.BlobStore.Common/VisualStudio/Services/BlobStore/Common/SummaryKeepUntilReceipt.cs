// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.SummaryKeepUntilReceipt
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class SummaryKeepUntilReceipt
  {
    private static readonly Pool<SHA256CryptoServiceProvider> PoolSha256 = new Pool<SHA256CryptoServiceProvider>((Func<SHA256CryptoServiceProvider>) (() => new SHA256CryptoServiceProvider()), (Action<SHA256CryptoServiceProvider>) (sha256 => sha256.Initialize()), 4 * Environment.ProcessorCount);
    public readonly KeepUntilBlobReference?[] KeepUntils;
    public readonly byte[] Signature;

    public SummaryKeepUntilReceipt(KeepUntilBlobReference?[] keepUntils, byte[] signature)
    {
      this.KeepUntils = keepUntils;
      this.Signature = signature;
    }

    public SummaryKeepUntilReceipt(params KeepUntilReceipt[] receipts)
    {
      this.KeepUntils = ((IEnumerable<KeepUntilReceipt>) receipts).Select<KeepUntilReceipt, KeepUntilBlobReference?>((Func<KeepUntilReceipt, KeepUntilBlobReference?>) (r => r?.KeepUntil)).ToArray<KeepUntilBlobReference?>();
      this.Signature = SummaryKeepUntilReceipt.ComputeSummarySignature(((IEnumerable<KeepUntilReceipt>) receipts).Select<KeepUntilReceipt, byte[]>((Func<KeepUntilReceipt, byte[]>) (r => r?.Signature)));
    }

    internal static byte[] ComputeSummarySignature(IEnumerable<byte[]> signatures)
    {
      using (MemoryStream output = new MemoryStream())
      {
        using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
        {
          using (Pool<SHA256CryptoServiceProvider>.PoolHandle poolHandle = SummaryKeepUntilReceipt.PoolSha256.Get())
          {
            foreach (byte[] signature in signatures)
            {
              if (signature != null)
                binaryWriter.Write(signature);
              else
                binaryWriter.Write(KeepUntilReceipt.NullSignature);
            }
            binaryWriter.Flush();
            byte[] array = output.ToArray();
            return poolHandle.Value.ComputeHash(array, 0, array.Length);
          }
        }
      }
    }
  }
}
