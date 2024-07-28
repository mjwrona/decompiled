// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GuidUtils
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class GuidUtils
  {
    public static Guid GenerateGuidFromString(Guid namespaceGuid, string input) => GuidUtils.GetNewGuidInternal(namespaceGuid.ToByteArray(), Encoding.UTF8.GetBytes(input), NameBasedGuidVersion.Seven);

    internal static Guid GetNewGuidInternal(
      byte[] namespaceBytes,
      byte[] nameBytes,
      NameBasedGuidVersion uuidSha256VersionId)
    {
      byte[] hash;
      using (HashAlgorithm hashAlgorithm = (HashAlgorithm) SHA256.Create())
      {
        hashAlgorithm.TransformBlock(namespaceBytes, 0, namespaceBytes.Length, (byte[]) null, 0);
        hashAlgorithm.TransformFinalBlock(nameBytes, 0, nameBytes.Length);
        hash = hashAlgorithm.Hash;
      }
      byte[] numArray = new byte[16];
      Array.Copy((Array) hash, 0, (Array) numArray, 0, 16);
      numArray[6] = (byte) ((int) numArray[6] & 15 | (int) uuidSha256VersionId << 4);
      numArray[8] = (byte) ((int) numArray[8] & 63 | 128);
      GuidUtils.SwapByteOrder(numArray);
      return new Guid(numArray);
    }

    public static Guid Change14thCharToZero(Guid guidToChange) => new Guid(new StringBuilder(guidToChange.ToString())
    {
      [14] = '0'
    }.ToString());

    private static void SwapByteOrder(byte[] guid)
    {
      GuidUtils.SwapBytes(guid, 0, 3);
      GuidUtils.SwapBytes(guid, 1, 2);
      GuidUtils.SwapBytes(guid, 4, 5);
      GuidUtils.SwapBytes(guid, 6, 7);
    }

    private static void SwapBytes(byte[] guid, int left, int right)
    {
      byte num = guid[left];
      guid[left] = guid[right];
      guid[right] = num;
    }

    internal static unsafe int GetGuidVersion(Guid g) => (int) ((*(uint*) ((IntPtr) &g + 4) & 4026531840U) >> 28);
  }
}
