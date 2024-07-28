// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.HMACHash`1
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common
{
  public abstract class HMACHash<THMAC> : IDisposable where THMAC : HMAC
  {
    private string m_content;
    private byte[] m_hash;
    private string m_hashBase32Encoded;
    private string m_hashBase64Encoded;
    private THMAC m_hashAlgorithm;

    public HMACHash(string content, byte[] key)
    {
      if (string.IsNullOrWhiteSpace(content))
        throw new ArgumentException("Content cannot be null or empty.");
      if (key == null || key.Length == 0)
        throw new ArgumentException("Key cannot be null or empty.");
      this.m_content = content;
      this.m_hashAlgorithm = (THMAC) Activator.CreateInstance(typeof (THMAC), (object[]) new byte[1][]
      {
        key
      });
    }

    public void Dispose() => this.m_hashAlgorithm.Dispose();

    public byte[] Hash
    {
      get
      {
        if (this.m_hash == null)
          this.ComputeHash();
        return this.m_hash;
      }
    }

    public string HashBase32Encoded
    {
      get
      {
        if (this.m_hash == null)
          this.ComputeHash();
        return this.m_hashBase32Encoded;
      }
    }

    public string HashBase64Encoded
    {
      get
      {
        if (this.m_hash == null)
          this.ComputeHash();
        return this.m_hashBase64Encoded;
      }
    }

    private void ComputeHash()
    {
      this.m_hash = this.m_hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(this.m_content));
      this.m_hashBase64Encoded = Convert.ToBase64String(this.m_hash);
      this.m_hashBase32Encoded = Base32Encoder.Encode(this.m_hash);
    }
  }
}
