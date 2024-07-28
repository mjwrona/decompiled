// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WrappingSigner
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class WrappingSigner : ISigner, IDisposable
  {
    private IVssRequestContext m_requestContext;
    private readonly string m_keyId;

    public WrappingSigner(IVssRequestContext requestContext, byte[] keyData)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.m_requestContext = requestContext;
      this.m_keyId = Encoding.UTF8.GetString(keyData);
    }

    public SigningKeyType KeyType => SigningKeyType.MasterWrappingKey;

    public byte[] Decrypt(byte[] data)
    {
      if (this.m_requestContext == null)
        throw new ObjectDisposedException(nameof (WrappingSigner));
      return this.m_requestContext.GetService<IKeyVaultWrappedKeyService>().UnwrapKey(this.m_requestContext, this.m_keyId, data);
    }

    public void Dispose() => this.m_requestContext = (IVssRequestContext) null;

    public byte[] Encrypt(byte[] data)
    {
      if (this.m_requestContext == null)
        throw new ObjectDisposedException(nameof (WrappingSigner));
      return this.m_requestContext.GetService<IKeyVaultWrappedKeyService>().WrapKey(this.m_requestContext, this.m_keyId, data);
    }

    public byte[] ExportPublicKey() => throw new InvalidOperationException("Wrapping key keys do not support signing operations.");

    public int GetKeySize() => throw new InvalidOperationException();

    public SigningAlgorithm GetSigningAlgorithm() => throw new InvalidOperationException("Wrapping key keys do not support signing operations.");

    public byte[] SignHash(byte[] hash) => throw new InvalidOperationException("Wrapping key keys do not support signing operations.");

    public bool VerifyHash(byte[] hash, byte[] signature) => throw new InvalidOperationException("Wrapping key keys do not support signing operations.");
  }
}
