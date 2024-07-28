// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenChecksumUtility
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public static class MavenChecksumUtility
  {
    public static byte[] ComputeChecksum(byte[] buffer, MavenHashAlgorithm mavenHashAlgorithm)
    {
      ArgumentUtility.CheckForNull<byte[]>(buffer, nameof (buffer));
      MavenHashAlgorithmInfo algorithm;
      if (!MavenHashAlgorithmInfo.TryGet(mavenHashAlgorithm, out algorithm))
        throw new ArgumentException(string.Format("Unknown hash algorithm: {0}", (object) mavenHashAlgorithm));
      using (HashAlgorithm hashAlgorithm = algorithm.Create())
        return MavenChecksumUtility.GetHexHashCode(hashAlgorithm.ComputeHash(buffer));
    }

    public static IEnumerable<(MavenHashAlgorithmInfo AlgorithmInfo, string HashValue)> ComputeTextChecksums(
      Stream content)
    {
      ArgumentUtility.CheckForNull<Stream>(content, nameof (content));
      foreach (MavenHashAlgorithmInfo algorithm in MavenHashAlgorithmInfo.Algorithms)
      {
        content.Position = 0L;
        using (HashAlgorithm hashAlgorithm = algorithm.Create())
        {
          string str = HexConverter.ToString(hashAlgorithm.ComputeHash(content));
          yield return (algorithm, str);
        }
      }
    }

    public static IEnumerable<(MavenHashAlgorithmInfo AlgorithmInfo, string HashValue)> ComputeTextChecksums(
      byte[] content)
    {
      ArgumentUtility.CheckForNull<byte[]>(content, nameof (content));
      return MavenHashAlgorithmInfo.Algorithms.Select<MavenHashAlgorithmInfo, (MavenHashAlgorithmInfo, string)>((Func<MavenHashAlgorithmInfo, (MavenHashAlgorithmInfo, string)>) (algorithmInfo =>
      {
        using (HashAlgorithm hashAlgorithm = algorithmInfo.Create())
        {
          string str = HexConverter.ToString(hashAlgorithm.ComputeHash(content));
          return (algorithmInfo, str);
        }
      }));
    }

    private static byte[] GetHexHashCode(byte[] hashCode) => Encoding.UTF8.GetBytes(HexConverter.ToString(hashCode));
  }
}
