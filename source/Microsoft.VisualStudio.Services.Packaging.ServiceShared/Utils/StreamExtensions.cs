// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.StreamExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class StreamExtensions
  {
    public static byte[] ReadToByteArray(this Stream stream)
    {
      if (stream is MemoryStream memoryStream && memoryStream.Position == 0L)
        return memoryStream.ToArray();
      MemoryStream destination;
      using (destination = new MemoryStream())
      {
        stream.CopyTo((Stream) destination);
        return destination.ToArray();
      }
    }

    public static async Task ReadIntoMultipleHashAlgorithmsAsync(
      this Stream stream,
      IReadOnlyList<HashAlgorithm> hashAlgorithms,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      byte[] buffer = new byte[4096];
label_1:
      cancellationToken.ThrowIfCancellationRequested();
      int inputCount = await stream.ReadAsync(buffer, 0, 4096, cancellationToken);
      if (inputCount != 0)
      {
        using (IEnumerator<HashAlgorithm> enumerator = hashAlgorithms.GetEnumerator())
        {
          while (enumerator.MoveNext())
            enumerator.Current.TransformBlock(buffer, 0, inputCount, (byte[]) null, 0);
          goto label_1;
        }
      }
      else
      {
        foreach (HashAlgorithm hashAlgorithm in (IEnumerable<HashAlgorithm>) hashAlgorithms)
          hashAlgorithm.TransformFinalBlock(buffer, 0, 0);
        buffer = (byte[]) null;
      }
    }
  }
}
