// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationSigningService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationSigningService))]
  public interface ITeamFoundationSigningService : IVssFrameworkService
  {
    void CreatePassthroughKey(
      IVssRequestContext requestContext,
      Guid identifier,
      X509Certificate2 certificate);

    byte[] Decrypt(
      IVssRequestContext requestContext,
      Guid identifier,
      byte[] encryptedData,
      SigningAlgorithm algorithm = SigningAlgorithm.SHA1);

    byte[] Encrypt(
      IVssRequestContext requestContext,
      Guid identifier,
      byte[] rawData,
      SigningAlgorithm algorithm = SigningAlgorithm.SHA1,
      bool useAKVWrapping = false);

    Guid GetDatabaseSigningKey(IVssRequestContext requestContext);

    List<string> GetThumbprintsInUse(IVssRequestContext requestContext);

    byte[] GetPublicKey(IVssRequestContext requestContext, Guid identifier, out int keyLength);

    ISigner GetSigner(
      IVssRequestContext requestContext,
      Guid identifier,
      SigningAlgorithm algorithm = SigningAlgorithm.SHA1,
      bool useAKVWrapping = false);

    SigningKeyType GetSigningKeyType(IVssRequestContext requestContext, Guid identifier);

    SigningKeyType GetDefaultKeyType(IVssRequestContext requestContext);

    ReencryptResults ReencryptConsumers(IVssRequestContext requestContext);

    Guid GenerateKey(IVssRequestContext requestContext, SigningKeyType keyType);

    void RegenerateKey(
      IVssRequestContext requestContext,
      Guid identifier,
      SigningKeyType requestedKeyType = SigningKeyType.Default);

    void SetDatabaseSigningKey(IVssRequestContext requestContext, Guid identifier);

    byte[] Sign(
      IVssRequestContext requestContext,
      Guid identifier,
      byte[] message,
      SigningAlgorithm algorithm = SigningAlgorithm.SHA1);

    Guid FindCertificateKey(IVssRequestContext requestContext, string thumbprint);

    void RemoveKeys(IVssRequestContext requestContext, IEnumerable<Guid> keys);

    void RemoveKeysNotInUse(IVssRequestContext requestContext);

    HashSet<Guid> GetSigningKeysInUse(IVssRequestContext requestContext);

    Guid FindMasterWrappingKeyId(IVssRequestContext requestContext, string masterWrappingKeyUrl);
  }
}
