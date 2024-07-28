// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssSigningCredentials
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public abstract class VssSigningCredentials
  {
    private const int c_minKeySize = 2048;
    private readonly DateTime m_effectiveDate;

    protected VssSigningCredentials() => this.m_effectiveDate = DateTime.UtcNow;

    public abstract bool CanSignData { get; }

    public abstract int KeySize { get; }

    public virtual DateTime ValidFrom => this.m_effectiveDate;

    public virtual DateTime ValidTo => DateTime.MaxValue;

    public virtual string KeyId { get; }

    public abstract JWTAlgorithm SignatureAlgorithm { get; }

    public virtual byte[] SignData(byte[] input)
    {
      if (!this.CanSignData)
        throw new InvalidOperationException();
      return this.GetSignature(input);
    }

    protected abstract byte[] GetSignature(byte[] input);

    public abstract bool VerifySignature(byte[] input, byte[] signature);

    public static VssSigningCredentials Create(X509Certificate2 certificate)
    {
      ArgumentUtility.CheckForNull<X509Certificate2>(certificate, nameof (certificate));
      if (certificate.HasPrivateKey)
      {
        if (!(certificate.PrivateKey is RSACryptoServiceProvider privateKey))
          throw new SignatureAlgorithmUnsupportedException(certificate.PrivateKey.SignatureAlgorithm);
        if (privateKey.CspKeyContainerInfo.ProviderType != 24)
          throw new SignatureAlgorithmUnsupportedException(privateKey.CspKeyContainerInfo.ProviderType);
        if (privateKey.KeySize < 2048)
          throw new InvalidCredentialsException(JwtResources.SigningTokenKeyTooSmall());
      }
      return (VssSigningCredentials) new VssSigningCredentials.X509Certificate2SigningToken(certificate);
    }

    public static VssSigningCredentials Create(Func<RSACryptoServiceProvider> factory)
    {
      ArgumentUtility.CheckForNull<Func<RSACryptoServiceProvider>>(factory, nameof (factory));
      using (RSACryptoServiceProvider cryptoServiceProvider = factory())
      {
        if (cryptoServiceProvider == null)
          throw new InvalidCredentialsException(JwtResources.SignatureAlgorithmUnsupportedException((object) "None"));
        if (cryptoServiceProvider.KeySize < 2048)
          throw new InvalidCredentialsException(JwtResources.SigningTokenKeyTooSmall());
        return (VssSigningCredentials) new VssSigningCredentials.RSASigningToken(factory, cryptoServiceProvider.KeySize);
      }
    }

    public static VssSigningCredentials Create(byte[] key)
    {
      ArgumentUtility.CheckForNull<byte[]>(key, nameof (key));
      return (VssSigningCredentials) new VssSigningCredentials.SymmetricKeySigningToken(key);
    }

    private class SymmetricKeySigningToken : VssSigningCredentials
    {
      private readonly byte[] m_key;

      public SymmetricKeySigningToken(byte[] key)
      {
        this.m_key = new byte[key.Length];
        Buffer.BlockCopy((Array) key, 0, (Array) this.m_key, 0, this.m_key.Length);
      }

      public override bool CanSignData => true;

      public override int KeySize => this.m_key.Length * 8;

      public override JWTAlgorithm SignatureAlgorithm => JWTAlgorithm.HS256;

      protected override byte[] GetSignature(byte[] input)
      {
        using (HMACSHA256 hmacshA256 = new HMACSHA256(this.m_key))
          return hmacshA256.ComputeHash(input);
      }

      public override bool VerifySignature(byte[] input, byte[] signature) => SecureCompare.TimeInvariantEquals(this.SignData(input), signature);
    }

    private abstract class AsymmetricKeySigningToken : VssSigningCredentials
    {
      private bool? m_hasPrivateKey;

      protected abstract bool HasPrivateKey();

      public override JWTAlgorithm SignatureAlgorithm => JWTAlgorithm.RS256;

      public override bool CanSignData
      {
        get
        {
          if (!this.m_hasPrivateKey.HasValue)
            this.m_hasPrivateKey = new bool?(this.HasPrivateKey());
          return this.m_hasPrivateKey.Value;
        }
      }
    }

    private class X509Certificate2SigningToken : 
      VssSigningCredentials.AsymmetricKeySigningToken,
      IJsonWebTokenHeaderProvider
    {
      private readonly X509Certificate2 m_certificate;

      public X509Certificate2SigningToken(X509Certificate2 certificate) => this.m_certificate = certificate;

      public override int KeySize => this.m_certificate.PublicKey.Key.KeySize;

      public override DateTime ValidFrom => this.m_certificate.NotBefore;

      public override DateTime ValidTo => this.m_certificate.NotAfter;

      public override string KeyId => this.m_certificate.Thumbprint;

      public override bool VerifySignature(byte[] input, byte[] signature)
      {
        using (SHA256 halg = SHA256.Create())
          return (this.m_certificate.PublicKey.Key as RSACryptoServiceProvider).VerifyData(input, (object) halg, signature);
      }

      protected override byte[] GetSignature(byte[] input)
      {
        using (SHA256 halg = SHA256.Create())
          return (this.m_certificate.PrivateKey as RSACryptoServiceProvider).SignData(input, (object) halg);
      }

      protected override bool HasPrivateKey() => this.m_certificate.HasPrivateKey;

      void IJsonWebTokenHeaderProvider.SetHeaders(IDictionary<string, object> headers) => headers["x5t"] = (object) this.m_certificate.GetCertHash().ToBase64StringNoPadding();
    }

    private class RSASigningToken : VssSigningCredentials.AsymmetricKeySigningToken
    {
      private readonly int m_keySize;
      private readonly Func<RSACryptoServiceProvider> m_factory;

      public RSASigningToken(Func<RSACryptoServiceProvider> factory, int keySize)
      {
        this.m_keySize = keySize;
        this.m_factory = factory;
      }

      public override int KeySize => this.m_keySize;

      protected override byte[] GetSignature(byte[] input)
      {
        using (RSACryptoServiceProvider cryptoServiceProvider = this.m_factory())
        {
          using (SHA256CryptoServiceProvider halg = new SHA256CryptoServiceProvider())
            return cryptoServiceProvider.SignData(input, (object) halg);
        }
      }

      protected override bool HasPrivateKey()
      {
        try
        {
          this.GetSignature(new byte[1]{ (byte) 1 });
          return true;
        }
        catch (CryptographicException ex)
        {
          return false;
        }
      }

      public override bool VerifySignature(byte[] input, byte[] signature)
      {
        using (RSACryptoServiceProvider cryptoServiceProvider = this.m_factory())
        {
          using (SHA256CryptoServiceProvider halg = new SHA256CryptoServiceProvider())
            return cryptoServiceProvider.VerifyData(input, (object) halg, signature);
        }
      }
    }
  }
}
