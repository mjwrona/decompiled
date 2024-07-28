// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SigningServiceKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal abstract class SigningServiceKey : ISigningServiceKey
  {
    protected byte[] KeyData;
    private const string c_area = "SigningServiceKey";
    private const string c_layer = "Service";
    private SigningKeyType m_keyType;
    private Guid m_identifier;

    internal SigningServiceKey(SigningKeyType keyType, Guid identifier)
    {
      this.m_keyType = keyType;
      this.m_identifier = identifier;
    }

    SigningKeyType ISigningServiceKey.KeyType => this.m_keyType;

    Guid ISigningServiceKey.Identifier => this.m_identifier;

    ISigner ISigningServiceKey.GetSigner(
      IVssRequestContext requestContext,
      SigningAlgorithm paddingAlgorithm)
    {
      return this.GetKeySigner(requestContext, paddingAlgorithm);
    }

    void ISigningServiceKey.Delete(IVssRequestContext requestContext)
    {
      this.DeleteSigningKeyRecord(requestContext);
      this.DeleteKeyData(requestContext);
    }

    protected abstract byte[] GetKeyData(IVssRequestContext requestContext);

    protected abstract void StoreKeyData(
      IVssRequestContext requestContext,
      byte[] rawKeyData,
      bool overwriteExisting);

    protected abstract void DeleteKeyData(IVssRequestContext requestContext);

    protected virtual ISigner GetKeySigner(
      IVssRequestContext requestContext,
      SigningAlgorithm paddingAlgorithm)
    {
      return SigningManager.GetSigner(requestContext, this.GetKeyData(requestContext), paddingAlgorithm, this.m_keyType);
    }

    protected void DeleteSigningKeyRecord(IVssRequestContext requestContext)
    {
      using (SigningComponent component = requestContext.CreateComponent<SigningComponent>())
        component.RemoveKeys((IList<Guid>) new Guid[1]
        {
          this.m_identifier
        });
    }

    protected void CheckKeyDataIsNotNull()
    {
      if (this.KeyData == null)
        throw new InvalidOperationException("KeyData is not initialized.");
    }

    protected void CheckKeyDataIsNull()
    {
      if (this.KeyData != null)
        throw new InvalidOperationException("KeyData has already been initialized.");
    }

    protected SigningComponent.SigningServiceKey StoreKeyDataToDatabase(
      IVssRequestContext requestContext,
      byte[] storedKeyData,
      bool overwriteExisting)
    {
      using (SigningComponent component = requestContext.CreateComponent<SigningComponent>())
        return component.SetPrivateKey(this.m_identifier, this.m_keyType, storedKeyData, overwriteExisting);
    }

    internal static ISigningServiceKey LoadKeyData(
      IVssRequestContext requestContext,
      Guid identifier)
    {
      SigningComponent.SigningServiceKey rawKey = SigningServiceKey.LoadRawKeyFromDatabase(requestContext, identifier);
      return rawKey == null ? (ISigningServiceKey) null : SigningServiceKey.CreateFromComponentType(requestContext, rawKey);
    }

    internal static ICertificatePassthroughSigningKey LoadKeyDataRaw(
      ISqlConnectionInfo connectionInfo,
      Guid identifier,
      ITFLogger logger = null)
    {
      SigningComponent.SigningServiceKey signingServiceKey1 = SigningServiceKey.LoadRawKeyFromDatabaseRaw(connectionInfo, identifier, logger);
      if (signingServiceKey1 == null)
        return (ICertificatePassthroughSigningKey) null;
      CertificatePassthroughSigningServiceKey signingServiceKey2 = signingServiceKey1.KeyType == SigningKeyType.CertificatePassthrough ? new CertificatePassthroughSigningServiceKey(signingServiceKey1.Identifier) : throw new NotSupportedException("LoadKeyDataRaw only works for $CertificatePassthrough");
      signingServiceKey2.KeyData = signingServiceKey1.KeyData;
      return (ICertificatePassthroughSigningKey) signingServiceKey2;
    }

    internal static ISigningServiceKey CreateFromComponentType(
      IVssRequestContext requestContext,
      SigningComponent.SigningServiceKey rawKey)
    {
      SigningServiceKey fromComponentType = SigningServiceKey.InstantiateSigningKeyObject(requestContext, rawKey.KeyType, rawKey.Identifier);
      fromComponentType.KeyData = rawKey.KeyData;
      return (ISigningServiceKey) fromComponentType;
    }

    internal static Tuple<ISigningServiceKey, int> StoreKeyData(
      IVssRequestContext requestContext,
      Guid identifier,
      SigningKeyType keyType,
      byte[] rawKeyData,
      bool overwriteExisting)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
      ArgumentUtility.CheckForNull<byte[]>(rawKeyData, nameof (rawKeyData));
      int version;
      using (SigningComponent component = requestContext.CreateComponent<SigningComponent>())
        version = component.Version;
      SigningServiceKey signingServiceKey = SigningServiceKey.InstantiateSigningKeyObject(requestContext, keyType, identifier);
      signingServiceKey.StoreKeyData(requestContext, rawKeyData, overwriteExisting);
      return new Tuple<ISigningServiceKey, int>((ISigningServiceKey) signingServiceKey, version);
    }

    internal static Tuple<ISigningServiceKey, int> UpdateKeyType(
      IVssRequestContext requestContext,
      ISigningServiceKey sourceKey,
      SigningKeyType keyType)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ISigningServiceKey>(sourceKey, nameof (sourceKey));
      SigningServiceKey var = sourceKey as SigningServiceKey;
      ArgumentUtility.CheckForNull<SigningServiceKey>(var, "typedSourceKey");
      int version;
      using (SigningComponent component = requestContext.CreateComponent<SigningComponent>())
        version = component.Version;
      SigningServiceKey signingServiceKey = SigningServiceKey.InstantiateSigningKeyObject(requestContext, keyType, sourceKey.Identifier);
      signingServiceKey.StoreKeyData(requestContext, var.GetKeyData(requestContext), true);
      var.DeleteKeyData(requestContext);
      return new Tuple<ISigningServiceKey, int>((ISigningServiceKey) signingServiceKey, version);
    }

    protected static SigningComponent.SigningServiceKey LoadRawKeyFromDatabase(
      IVssRequestContext requestContext,
      Guid identifier)
    {
      using (SigningComponent component = requestContext.CreateComponent<SigningComponent>())
        return component.GetPrivateKey(identifier);
    }

    private static SigningComponent.SigningServiceKey LoadRawKeyFromDatabaseRaw(
      ISqlConnectionInfo connectionInfo,
      Guid identifier,
      ITFLogger logger)
    {
      using (SigningComponent componentRaw = connectionInfo.CreateComponentRaw<SigningComponent>(logger: logger))
      {
        componentRaw.PartitionId = DatabasePartitionConstants.DeploymentHostPartitionId;
        return componentRaw.GetPrivateKey(identifier);
      }
    }

    private static SigningServiceKey InstantiateSigningKeyObject(
      IVssRequestContext requestContext,
      SigningKeyType keyType,
      Guid identifier)
    {
      switch (keyType)
      {
        case SigningKeyType.RSAStored:
          return (SigningServiceKey) new RsaStoredSigningServiceKey(identifier);
        case SigningKeyType.CertificatePassthrough:
          return (SigningServiceKey) new CertificatePassthroughSigningServiceKey(identifier);
        case SigningKeyType.RSASecured:
          return (SigningServiceKey) new RsaSecuredSigningServiceKey(SigningKeyType.RSASecured, identifier);
        case SigningKeyType.PartitionSecured:
          return (SigningServiceKey) new PartitionSecuredSigningServiceKey(identifier);
        case SigningKeyType.DeploymentCertificateSecured:
          return (SigningServiceKey) new DeploymentCertificateSecuredSigningServiceKey(identifier);
        case SigningKeyType.RsaSecuredByKeyEncryptionKey:
          return (SigningServiceKey) new RsaSecuredSigningServiceKey(SigningKeyType.RsaSecuredByKeyEncryptionKey, identifier);
        case SigningKeyType.KeyEncryptionKey:
          return (SigningServiceKey) new KeyEncryptionKeySigningServiceKey(identifier);
        case SigningKeyType.MasterWrappingKey:
          return (SigningServiceKey) new MasterWrappingKeySigningServiceKey(identifier);
        default:
          throw new ArgumentException(string.Format("Invalid {0}: {1}", (object) "SigningKeyType", (object) keyType), nameof (keyType));
      }
    }
  }
}
