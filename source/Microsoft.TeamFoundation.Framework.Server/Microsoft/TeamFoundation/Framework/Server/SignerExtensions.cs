// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SignerExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SignerExtensions
  {
    public static byte[] GenerateHash(this ISigner signer, string encodedMessage)
    {
      using (HashAlgorithm hashAlgorithm = HashAlgorithm.Create(SigningManager.GetHashAlgorithmName(signer.GetSigningAlgorithm()).ToString()))
        return hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(encodedMessage));
    }

    public static void CheckCanEncrypt(this ISigner signer, byte[] data)
    {
      if (!signer.CanEncrypt(data))
        throw new SignerCantEncryptException(signer.GetMaxDataLength());
    }

    public static bool CanEncrypt(this ISigner signer, byte[] data) => data.Length <= signer.GetMaxDataLength();

    public static int GetMaxDataLength(this ISigner signer) => signer.GetKeySize() / 8 - 2 * SigningManager.GetHashSize(signer.GetSigningAlgorithm()) / 8 - 2;
  }
}
