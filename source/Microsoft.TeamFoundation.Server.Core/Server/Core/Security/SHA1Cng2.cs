// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Security.SHA1Cng2
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Server.Core.Security
{
  public sealed class SHA1Cng2 : SHA1
  {
    private BCryptHashAlgorithm2 m_hashAlgorithm;

    public SHA1Cng2() => this.m_hashAlgorithm = new BCryptHashAlgorithm2(CngAlgorithm.Sha1, "Microsoft Primitive Provider");

    protected override void Dispose(bool disposing)
    {
      try
      {
        if (!disposing)
          return;
        this.m_hashAlgorithm.Dispose();
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    public override void Initialize() => this.m_hashAlgorithm.Initialize();

    protected override void HashCore(byte[] array, int ibStart, int cbSize) => this.m_hashAlgorithm.HashCore(array, ibStart, cbSize);

    protected override byte[] HashFinal() => this.m_hashAlgorithm.HashFinal();
  }
}
