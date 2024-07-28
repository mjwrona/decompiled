// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenHashAlgorithmInfo
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public class MavenHashAlgorithmInfo
  {
    public static readonly IEnumerable<MavenHashAlgorithmInfo> Algorithms = (IEnumerable<MavenHashAlgorithmInfo>) new MavenHashAlgorithmInfo[4]
    {
      new MavenHashAlgorithmInfo(MavenHashAlgorithm.MD5, HashType.MD5, ".md5", new Func<HashAlgorithm>(MD5.Create)),
      new MavenHashAlgorithmInfo(MavenHashAlgorithm.SHA1, HashType.SHA1, ".sha1", new Func<HashAlgorithm>(SHA1.Create)),
      new MavenHashAlgorithmInfo(MavenHashAlgorithm.SHA256, HashType.SHA256, ".sha256", new Func<HashAlgorithm>(SHA256.Create)),
      new MavenHashAlgorithmInfo(MavenHashAlgorithm.SHA512, HashType.SHA512, ".sha512", new Func<HashAlgorithm>(SHA512.Create))
    };

    public static bool TryGet(MavenHashAlgorithm id, out MavenHashAlgorithmInfo algorithm) => MavenHashAlgorithmInfo.TryGet((Func<MavenHashAlgorithmInfo, bool>) (a => a.Id == id), out algorithm);

    public static bool TryGet(HashType hashType, out MavenHashAlgorithmInfo algorithm) => MavenHashAlgorithmInfo.TryGet((Func<MavenHashAlgorithmInfo, bool>) (a => a.HashType == hashType), out algorithm);

    public static bool TryGet(string fileExtension, out MavenHashAlgorithmInfo algorithm) => MavenHashAlgorithmInfo.TryGet((Func<MavenHashAlgorithmInfo, bool>) (a => VssStringComparer.FilePath.Equals(a.FileExtension, fileExtension)), out algorithm);

    public MavenHashAlgorithm Id { get; }

    public HashType HashType { get; }

    public string FileExtension { get; }

    private Func<HashAlgorithm> Creator { get; }

    public MavenHashAlgorithmInfo(
      MavenHashAlgorithm id,
      HashType hashType,
      string fileExtension,
      Func<HashAlgorithm> hashAlgorithm)
    {
      this.Id = id;
      this.HashType = hashType;
      this.FileExtension = fileExtension;
      this.Creator = hashAlgorithm;
    }

    public HashAlgorithm Create() => this.Creator();

    private static bool TryGet(
      Func<MavenHashAlgorithmInfo, bool> test,
      out MavenHashAlgorithmInfo algorithm)
    {
      algorithm = MavenHashAlgorithmInfo.Algorithms.FirstOrDefault<MavenHashAlgorithmInfo>(test);
      return algorithm != null;
    }
  }
}
