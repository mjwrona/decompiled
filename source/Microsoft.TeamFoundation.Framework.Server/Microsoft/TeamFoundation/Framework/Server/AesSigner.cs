// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AesSigner
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.IO;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class AesSigner : ISigner, IDisposable
  {
    private readonly byte[] m_keyDataRaw;
    private readonly byte[] m_key;
    private readonly byte[] m_IV;

    public AesSigner(byte[] keyData, SigningKeyType keyType)
    {
      this.m_keyDataRaw = keyData;
      BinaryReader binaryReader = new BinaryReader((Stream) new MemoryStream(this.m_keyDataRaw));
      int count1 = binaryReader.ReadInt32();
      this.m_key = binaryReader.ReadBytes(count1);
      int count2 = binaryReader.ReadInt32();
      this.m_IV = binaryReader.ReadBytes(count2);
    }

    public SigningKeyType KeyType => SigningKeyType.KeyEncryptionKey;

    public byte[] Decrypt(byte[] data)
    {
      using (Aes aes = Aes.Create())
      {
        aes.Key = this.m_key;
        aes.IV = this.m_IV;
        return aes.CreateDecryptor(aes.Key, aes.IV).TransformFinalBlock(data, 0, data.Length);
      }
    }

    public void Dispose()
    {
    }

    public byte[] Encrypt(byte[] data)
    {
      using (Aes aes = Aes.Create())
      {
        aes.Key = this.m_key;
        aes.IV = this.m_IV;
        return aes.CreateEncryptor(aes.Key, aes.IV).TransformFinalBlock(data, 0, data.Length);
      }
    }

    public byte[] ExportPublicKey() => throw new InvalidOperationException("AES keys do not contain public key.");

    public int GetKeySize() => this.m_key.Length;

    public SigningAlgorithm GetSigningAlgorithm() => throw new InvalidOperationException("AES keys do not support signing operations.");

    public byte[] SignHash(byte[] hash) => throw new InvalidOperationException("AES keys do not support signing operations.");

    public bool VerifyHash(byte[] hash, byte[] signature) => throw new InvalidOperationException("AES keys do not support signing operations.");

    public static byte[] GenerateKey()
    {
      using (Aes aes = Aes.Create())
      {
        using (MemoryStream output = new MemoryStream())
        {
          using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
          {
            binaryWriter.Write(aes.Key.Length);
            binaryWriter.Write(aes.Key);
            binaryWriter.Write(aes.IV.Length);
            binaryWriter.Write(aes.IV);
            return output.ToArray();
          }
        }
      }
    }
  }
}
