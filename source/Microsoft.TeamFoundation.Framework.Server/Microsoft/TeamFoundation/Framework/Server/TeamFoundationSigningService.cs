// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationSigningService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationSigningService : 
    VssBaseService,
    ITeamFoundationSigningService,
    IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private const string s_Area = "TeamFoundationSigningService";
    private const string s_Layer = "Service";

    public void CreatePassthroughKey(
      IVssRequestContext requestContext,
      Guid identifier,
      X509Certificate2 certificate)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108116, nameof (TeamFoundationSigningService), "Service", nameof (CreatePassthroughKey));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        requestContext.CheckSystemRequestContext();
        ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
        ArgumentUtility.CheckForNull<X509Certificate2>(certificate, nameof (certificate));
        byte[] bytes = Encoding.ASCII.GetBytes(certificate.Thumbprint);
        requestContext.GetService<TeamFoundationSigningService.SigningServiceCache>().WriteThrough(requestContext, identifier, SigningKeyType.CertificatePassthrough, bytes, true);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108117, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108118, nameof (TeamFoundationSigningService), "Service", nameof (CreatePassthroughKey));
      }
    }

    public void CreateMasterWrappingKey(
      IVssRequestContext requestContext,
      Guid identifier,
      string keyvaultKeyIdentifier)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108116, nameof (TeamFoundationSigningService), "Service", nameof (CreateMasterWrappingKey));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        requestContext.CheckSystemRequestContext();
        ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
        requestContext.TraceAlways(108119, TraceLevel.Info, nameof (TeamFoundationSigningService), "Service", "Creating master wrapping key Id: {0} KeyvaultId:{1}", (object) identifier, (object) keyvaultKeyIdentifier);
        byte[] bytes = Encoding.UTF8.GetBytes(keyvaultKeyIdentifier);
        requestContext.GetService<TeamFoundationSigningService.SigningServiceCache>().WriteThrough(requestContext, identifier, SigningKeyType.MasterWrappingKey, bytes, true);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108117, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108118, nameof (TeamFoundationSigningService), "Service", nameof (CreateMasterWrappingKey));
      }
    }

    public void CreateDeploymentSecuredKey(
      IVssRequestContext requestContext,
      Guid identifier,
      string thumbprint)
    {
      this.ValidateRequestContext(requestContext);
      using (requestContext.TraceBlock(108107, 108018, nameof (TeamFoundationSigningService), "Service", nameof (CreateDeploymentSecuredKey)))
      {
        requestContext.CheckProjectCollectionOrOrganizationRequestContext();
        ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
        ArgumentUtility.CheckStringForNullOrEmpty(thumbprint, nameof (thumbprint));
        byte[] bytes = Encoding.ASCII.GetBytes(thumbprint);
        requestContext.GetService<TeamFoundationSigningService.SigningServiceCache>().WriteThrough(requestContext, identifier, SigningKeyType.DeploymentCertificateSecured, bytes, true);
      }
    }

    public byte[] Decrypt(
      IVssRequestContext requestContext,
      Guid identifier,
      byte[] encryptedData,
      SigningAlgorithm algorithm = SigningAlgorithm.SHA1)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108119, nameof (TeamFoundationSigningService), "Service", nameof (Decrypt));
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        requestContext.CheckSystemRequestContext();
        ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
        ArgumentUtility.CheckForNull<byte[]>(encryptedData, nameof (encryptedData));
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceDecryptsPerSec").Increment();
        using (ISigner signer = this.GetSigner(requestContext, identifier, algorithm, false))
        {
          requestContext.Trace(108105, TraceLevel.Info, nameof (TeamFoundationSigningService), "Service", "Decrypting data for host {0} with key {1} of type {2}.", (object) requestContext.ServiceHost.InstanceId, (object) identifier, (object) signer.KeyType);
          return signer.Decrypt(encryptedData);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108120, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceAvgDecryptTime").IncrementMilliseconds(stopwatch.ElapsedMilliseconds);
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceAvgDecryptTimeBase").Increment();
        requestContext.TraceLeave(108121, nameof (TeamFoundationSigningService), "Service", nameof (Decrypt));
      }
    }

    public byte[] Encrypt(
      IVssRequestContext requestContext,
      Guid identifier,
      byte[] rawData,
      SigningAlgorithm algorithm = SigningAlgorithm.SHA1,
      bool useAKVWrapping = false)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108122, nameof (TeamFoundationSigningService), "Service", nameof (Encrypt));
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        requestContext.CheckSystemRequestContext();
        ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
        ArgumentUtility.CheckForNull<byte[]>(rawData, nameof (rawData));
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceEncryptsPerSec").Increment();
        using (ISigner signer = this.GetSigner(requestContext, identifier, algorithm, useAKVWrapping))
        {
          requestContext.Trace(108103, TraceLevel.Info, nameof (TeamFoundationSigningService), "Service", "Encrypting data for host {0} with key {1} of type {2}.", (object) requestContext.ServiceHost.InstanceId, (object) identifier, (object) signer.KeyType);
          this.CheckKeyTypeAllowedForEncryption(requestContext, identifier, signer);
          return signer.Encrypt(rawData);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108123, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceAvgEncryptTime").IncrementMilliseconds(stopwatch.ElapsedMilliseconds);
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceAvgEncryptTimeBase").Increment();
        requestContext.TraceLeave(108124, nameof (TeamFoundationSigningService), "Service", nameof (Encrypt));
      }
    }

    public Guid GetDatabaseSigningKey(IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108125, nameof (TeamFoundationSigningService), "Service", nameof (GetDatabaseSigningKey));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        requestContext.CheckSystemRequestContext();
        CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
        Guid frameworkSigningKey = FrameworkServerConstants.FrameworkSigningKey;
        IVssRequestContext requestContext1 = requestContext;
        // ISSUE: explicit reference operation
        ref RegistryQuery local = @(RegistryQuery) FrameworkServerConstants.DefaultSigningKeyPath;
        Guid defaultValue = frameworkSigningKey;
        return service.GetValue<Guid>(requestContext1, in local, defaultValue);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108126, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108127, nameof (TeamFoundationSigningService), "Service", nameof (GetDatabaseSigningKey));
      }
    }

    public List<string> GetThumbprintsInUse(IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108128, nameof (TeamFoundationSigningService), "Service", nameof (GetThumbprintsInUse));
      try
      {
        requestContext.CheckSystemRequestContext();
        if (requestContext.IsFeatureEnabled(FrameworkServerConstants.UseBatchedSigningKeyRetrievalForGetThumbprintsInUse))
        {
          HashSet<Guid> signingKeysInUse = this.GetSigningKeysInUse(requestContext);
          List<ISigningServiceKey> serviceKeysByIds = this.GetSigningServiceKeysByIds(requestContext, (ICollection<Guid>) signingKeysInUse);
          return this.GetThumbprintsForSigningKeys(requestContext, (IEnumerable<ISigningServiceKey>) serviceKeysByIds);
        }
        Func<Guid, string> selector = !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? (Func<Guid, string>) (identifier => !(this.GetSigningKey(requestContext, identifier, SigningKeyType.DeploymentCertificateSecured, false) is DeploymentCertificateSecuredSigningServiceKey signingKey1) ? (string) null : signingKey1.Thumbprint) : (Func<Guid, string>) (identifier => !(this.GetSigningKey(requestContext, identifier, SigningKeyType.CertificatePassthrough, false) is ICertificatePassthroughSigningKey signingKey2) ? (string) null : signingKey2.Thumbprint);
        return this.GetSigningKeysInUse(requestContext).Select<Guid, string>(selector).Where<string>((Func<string, bool>) (x => x != null)).ToList<string>();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108129, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108130, nameof (TeamFoundationSigningService), "Service", nameof (GetThumbprintsInUse));
      }
    }

    private List<ISigningServiceKey> GetSigningServiceKeysByIds(
      IVssRequestContext requestContext,
      ICollection<Guid> signingKeyIds)
    {
      int count1 = 10000;
      int count2 = 0;
      List<ISigningServiceKey> serviceKeysByIds = new List<ISigningServiceKey>();
      using (SigningComponent component = requestContext.CreateComponent<SigningComponent>())
      {
        for (; count2 < signingKeyIds.Count; count2 += count1)
        {
          IEnumerable<Guid> identifiers = signingKeyIds.Skip<Guid>(count2).Take<Guid>(count1);
          serviceKeysByIds.AddRange(component.GetPrivateKeysByIds(identifiers).Select<SigningComponent.SigningServiceKey, ISigningServiceKey>((Func<SigningComponent.SigningServiceKey, ISigningServiceKey>) (ssk => SigningServiceKey.CreateFromComponentType(requestContext, ssk))));
        }
      }
      return serviceKeysByIds;
    }

    private List<string> GetThumbprintsForSigningKeys(
      IVssRequestContext requestContext,
      IEnumerable<ISigningServiceKey> signingKeys)
    {
      List<string> thumbprintsForSigningKeys = new List<string>();
      List<SigningKeyType> source = new List<SigningKeyType>();
      Func<ISigningServiceKey, string> func;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        source.Add(SigningKeyType.CertificatePassthrough);
        func = (Func<ISigningServiceKey, string>) (signingKey => !(signingKey is ICertificatePassthroughSigningKey passthroughSigningKey) ? (string) null : passthroughSigningKey.Thumbprint);
      }
      else
      {
        source.Add(SigningKeyType.DeploymentCertificateSecured);
        source.Add(SigningKeyType.PartitionSecured);
        func = (Func<ISigningServiceKey, string>) (signingKey => !(signingKey is DeploymentCertificateSecuredSigningServiceKey signingServiceKey) ? (string) null : signingServiceKey.Thumbprint);
      }
      string str1 = string.Join(",", source.Select<SigningKeyType, string>((Func<SigningKeyType, string>) (keyType => keyType.ToString())));
      foreach (ISigningServiceKey signingKey in signingKeys)
      {
        if (!source.Contains(signingKey.KeyType))
          requestContext.TraceAlways(1234124, TraceLevel.Warning, nameof (TeamFoundationSigningService), "Service", string.Format("The key with identifier {0} does not match an expected key type. Expected: {1}, Actual: {2}", (object) signingKey.Identifier, (object) str1, (object) signingKey.KeyType));
        string str2 = func(signingKey);
        if (str2 != null)
          thumbprintsForSigningKeys.Add(str2);
      }
      return thumbprintsForSigningKeys;
    }

    public byte[] GetPublicKey(
      IVssRequestContext requestContext,
      Guid identifier,
      out int keyLength)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108131, nameof (TeamFoundationSigningService), "Service", nameof (GetPublicKey));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
        requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);
        using (ISigner signer = this.GetSigner(requestContext.Elevate(), identifier, SigningAlgorithm.SHA512, false))
        {
          keyLength = signer.GetKeySize();
          return signer.ExportPublicKey();
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108132, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108133, nameof (TeamFoundationSigningService), "Service", nameof (GetPublicKey));
      }
    }

    public ISigner GetSigner(
      IVssRequestContext requestContext,
      Guid identifier,
      SigningAlgorithm algorithm = SigningAlgorithm.SHA1,
      bool useAKVWrapping = false)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108134, nameof (TeamFoundationSigningService), "Service", nameof (GetSigner));
      try
      {
        requestContext.CheckSystemRequestContext();
        ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
        return (useAKVWrapping ? this.GetSigningKeyV2(requestContext, identifier) : this.GetSigningKey(requestContext, identifier)).GetSigner(requestContext, algorithm);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108135, nameof (TeamFoundationSigningService), "Service", ex);
        if (ex is CertificateNotFoundException && requestContext.ExecutionEnvironment.IsDevFabricDeployment)
          this.TraceCertificateChanges(requestContext);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108136, nameof (TeamFoundationSigningService), "Service", nameof (GetSigner));
      }
    }

    public SigningKeyType GetSigningKeyType(IVssRequestContext requestContext, Guid identifier)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108174, nameof (TeamFoundationSigningService), "Service", nameof (GetSigningKeyType));
      try
      {
        requestContext.CheckSystemRequestContext();
        ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
        return this.GetSigningKey(requestContext, identifier).KeyType;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108175, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108176, nameof (TeamFoundationSigningService), "Service", nameof (GetSigningKeyType));
      }
    }

    public ReencryptResults ReencryptConsumers(IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108140, nameof (TeamFoundationSigningService), "Service", nameof (ReencryptConsumers));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        requestContext.CheckSystemRequestContext();
        ReencryptResults reencryptResults = new ReencryptResults();
        Guid databaseSigningKey = this.GetDatabaseSigningKey(requestContext);
        using (IDisposableReadOnlyList<ISigningServiceConsumer> extensions = requestContext.GetExtensions<ISigningServiceConsumer>(throwOnError: true))
        {
          foreach (ISigningServiceConsumer signingServiceConsumer in (IEnumerable<ISigningServiceConsumer>) extensions)
          {
            ReencryptResults results = signingServiceConsumer.Reencrypt(requestContext, databaseSigningKey);
            requestContext.Trace(108101, TraceLevel.Info, nameof (TeamFoundationSigningService), "Service", "{0} re-encryption results: {1}", (object) signingServiceConsumer.GetType().Name, (object) results);
            reencryptResults.Merge(results);
          }
        }
        return reencryptResults;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108141, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108142, nameof (TeamFoundationSigningService), "Service", nameof (ReencryptConsumers));
      }
    }

    public Guid GenerateKey(IVssRequestContext requestContext, SigningKeyType keyType)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108171, nameof (TeamFoundationSigningService), "Service", nameof (GenerateKey));
      try
      {
        requestContext.CheckSystemRequestContext();
        if (keyType == SigningKeyType.Default)
          keyType = this.GetDefaultKeyType(requestContext);
        if (keyType != SigningKeyType.RSASecured && keyType != SigningKeyType.RSAStored && keyType != SigningKeyType.PartitionSecured && keyType != SigningKeyType.KeyEncryptionKey && keyType != SigningKeyType.RsaSecuredByKeyEncryptionKey)
          throw new NotSupportedException(FrameworkResources.KeyTypeMustBeRSASecuredOrStored((object) keyType));
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && keyType != SigningKeyType.MasterWrappingKey && keyType != SigningKeyType.KeyEncryptionKey)
          throw new NotSupportedException(FrameworkResources.KeyTypeNotAllowedForHost((object) requestContext.ServiceHost.InstanceId, (object) Guid.Empty.ToString("D"), (object) keyType));
        Guid identifier = Guid.NewGuid();
        byte[] key = SigningManager.GenerateKey(keyType, this.GetDefaultKeyLength(requestContext));
        requestContext.GetService<TeamFoundationSigningService.SigningServiceCache>().WriteThrough(requestContext, identifier, keyType, key, true);
        return identifier;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108172, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108173, nameof (TeamFoundationSigningService), "Service", nameof (GenerateKey));
      }
    }

    public void ConvertToDefaultKeyType(IVssRequestContext requestContext, Guid identifier)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108181, nameof (TeamFoundationSigningService), "Service", nameof (ConvertToDefaultKeyType));
      try
      {
        requestContext.CheckSystemRequestContext();
        requestContext.CheckProjectCollectionOrOrganizationRequestContext();
        requestContext.CheckHostedDeployment();
        TeamFoundationSigningService.SigningServiceCache service = requestContext.GetService<TeamFoundationSigningService.SigningServiceCache>();
        ISigningServiceKey sourceKey;
        if (!service.TryGetValue(requestContext, identifier, out sourceKey))
        {
          sourceKey = SigningServiceKey.LoadKeyData(requestContext, identifier);
          if (sourceKey == null)
            throw new SigningKeyNotFoundException(identifier);
        }
        if (sourceKey.KeyType != SigningKeyType.RSASecured && sourceKey.KeyType != SigningKeyType.PartitionSecured && sourceKey.KeyType != SigningKeyType.RSAStored)
          throw new NotSupportedException(FrameworkResources.KeyTypeNotAllowed((object) identifier.ToString("D")));
        SigningKeyType defaultKeyType = this.GetDefaultKeyType(requestContext);
        if (sourceKey.KeyType == defaultKeyType)
          return;
        requestContext.Trace(108184, TraceLevel.Verbose, nameof (TeamFoundationSigningService), "Service", "Converted signing key {0} from {1} to {2}", (object) identifier.ToString("D"), (object) sourceKey.KeyType, (object) defaultKeyType);
        service.WriteThrough(requestContext, sourceKey, defaultKeyType);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108182, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108183, nameof (TeamFoundationSigningService), "Service", nameof (ConvertToDefaultKeyType));
      }
    }

    public void RegenerateKey(
      IVssRequestContext requestContext,
      Guid identifier,
      SigningKeyType requestedKeyType)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108143, nameof (TeamFoundationSigningService), "Service", nameof (RegenerateKey));
      try
      {
        requestContext.CheckSystemRequestContext();
        ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          throw new NotSupportedException(FrameworkResources.KeyTypeNotAllowedForHost((object) requestContext.ServiceHost.InstanceId, (object) identifier.ToString("D"), (object) SigningKeyType.RSAStored));
        SigningKeyType keyType = requestedKeyType;
        if (requestedKeyType == SigningKeyType.Default)
          keyType = this.GetDefaultKeyType(requestContext);
        byte[] keyPair = SigningManager.GenerateKeyPair(this.GetDefaultKeyLength(requestContext));
        requestContext.GetService<TeamFoundationSigningService.SigningServiceCache>().WriteThrough(requestContext, identifier, keyType, keyPair, true);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108144, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108145, nameof (TeamFoundationSigningService), "Service", nameof (RegenerateKey));
      }
    }

    public void SetDatabaseSigningKey(IVssRequestContext requestContext, Guid identifier)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108146, nameof (TeamFoundationSigningService), "Service", nameof (SetDatabaseSigningKey));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        requestContext.CheckSystemRequestContext();
        ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
        requestContext.GetService<CachedRegistryService>().SetValue<Guid>(requestContext, FrameworkServerConstants.DefaultSigningKeyPath, identifier);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108147, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108148, nameof (TeamFoundationSigningService), "Service", nameof (SetDatabaseSigningKey));
      }
    }

    public byte[] Sign(
      IVssRequestContext requestContext,
      Guid identifier,
      byte[] message,
      SigningAlgorithm algorithm = SigningAlgorithm.SHA1)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108153, nameof (TeamFoundationSigningService), "Service", nameof (Sign));
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        requestContext.CheckSystemRequestContext();
        ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
        ArgumentUtility.CheckForNull<byte[]>(message, nameof (message));
        using (ISigner signer = this.GetSigner(requestContext, identifier, algorithm, false))
          return this.SignHash(requestContext, message, identifier, signer);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108154, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceAvgSigningTime").IncrementMilliseconds(stopwatch.ElapsedMilliseconds);
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceAvgSigningTimeBase").Increment();
        requestContext.TraceLeave(108155, nameof (TeamFoundationSigningService), "Service", nameof (Sign));
      }
    }

    public Guid FindMasterWrappingKeyId(
      IVssRequestContext requestContext,
      string masterWrappingKeyUrl)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108200, nameof (TeamFoundationSigningService), "Service", nameof (FindMasterWrappingKeyId));
      try
      {
        requestContext.TraceAlways(108201, TraceLevel.Info, nameof (TeamFoundationSigningService), "Service", "Looking up the Master wrapping key with {0}", (object) masterWrappingKeyUrl);
        requestContext.CheckSystemRequestContext();
        ArgumentUtility.CheckStringForNullOrEmpty(masterWrappingKeyUrl, nameof (masterWrappingKeyUrl));
        Func<ISigningServiceKey, bool> predicate = (Func<ISigningServiceKey, bool>) (key => key is MasterWrappingKeySigningServiceKey signingServiceKey1 && string.Equals(masterWrappingKeyUrl, signingServiceKey1.GetKeyUrl(), StringComparison.OrdinalIgnoreCase));
        ISigningServiceKey signingServiceKey2 = this.GetSigningKeys(requestContext).ToList<ISigningServiceKey>().FirstOrDefault<ISigningServiceKey>(predicate);
        return signingServiceKey2 != null ? signingServiceKey2.Identifier : Guid.Empty;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108200, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108200, nameof (TeamFoundationSigningService), "Service", nameof (FindMasterWrappingKeyId));
      }
    }

    public Guid FindCertificateKey(IVssRequestContext requestContext, string thumbprint)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108162, nameof (TeamFoundationSigningService), "Service", nameof (FindCertificateKey));
      try
      {
        requestContext.CheckSystemRequestContext();
        ArgumentUtility.CheckStringForNullOrEmpty(thumbprint, nameof (thumbprint));
        Func<ISigningServiceKey, bool> predicate;
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          predicate = (Func<ISigningServiceKey, bool>) (k => k is ICertificatePassthroughSigningKey passthroughSigningKey && string.Equals(passthroughSigningKey.Thumbprint, thumbprint, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
          requestContext.CheckProjectCollectionOrOrganizationRequestContext();
          predicate = (Func<ISigningServiceKey, bool>) (k => k is DeploymentCertificateSecuredSigningServiceKey signingServiceKey && string.Equals(signingServiceKey.Thumbprint, thumbprint, StringComparison.OrdinalIgnoreCase));
        }
        ISigningServiceKey signingServiceKey1 = this.GetSigningKeys(requestContext).FirstOrDefault<ISigningServiceKey>(predicate);
        return signingServiceKey1 != null ? signingServiceKey1.Identifier : Guid.Empty;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108163, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108164, nameof (TeamFoundationSigningService), "Service", nameof (FindCertificateKey));
      }
    }

    public virtual void RemoveKeys(IVssRequestContext requestContext, IEnumerable<Guid> keys)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108165, nameof (TeamFoundationSigningService), "Service", nameof (RemoveKeys));
      try
      {
        requestContext.CheckSystemRequestContext();
        ArgumentUtility.CheckForNull<IEnumerable<Guid>>(keys, nameof (keys));
        TeamFoundationSigningService.SigningServiceCache service = requestContext.GetService<TeamFoundationSigningService.SigningServiceCache>();
        foreach (Guid key in keys)
        {
          ISigningServiceKey signingServiceKey = SigningServiceKey.LoadKeyData(requestContext, key);
          if (signingServiceKey != null)
          {
            signingServiceKey.Delete(requestContext);
            service.Invalidate(requestContext, signingServiceKey.Identifier);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108166, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108167, nameof (TeamFoundationSigningService), "Service", nameof (RemoveKeys));
      }
    }

    public void RemoveKeysNotInUse(IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.TraceEnter(108168, nameof (TeamFoundationSigningService), "Service", nameof (RemoveKeysNotInUse));
      try
      {
        requestContext.CheckSystemRequestContext();
        IEnumerable<ISigningServiceKey> signingKeys = this.GetSigningKeys(requestContext);
        HashSet<Guid> signingKeysInUse = this.GetSigningKeysInUse(requestContext);
        IEnumerable<Guid> source = signingKeys.Select<ISigningServiceKey, Guid>((Func<ISigningServiceKey, Guid>) (key => key.Identifier)).Except<Guid>((IEnumerable<Guid>) signingKeysInUse);
        IEnumerable<Guid> guids = source;
        int count = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.MaxSigningKeysToRemovePath, int.MaxValue);
        if (count != int.MaxValue)
          guids = source.Take<Guid>(count);
        if (requestContext.IsTracing(108177, TraceLevel.Info, nameof (TeamFoundationSigningService), "Service"))
          requestContext.Trace(108177, TraceLevel.Info, nameof (TeamFoundationSigningService), "Service", "Removing {0} signing keys that are unused out of {1} total unused, {2} are in use", (object) guids.Count<Guid>(), (object) source.Count<Guid>(), (object) signingKeysInUse.Count);
        this.RemoveKeys(requestContext, guids);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(108169, nameof (TeamFoundationSigningService), "Service", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(108170, nameof (TeamFoundationSigningService), "Service", nameof (RemoveKeysNotInUse));
      }
    }

    public static void CreatePassthroughKeyRaw(
      ISqlConnectionInfo connectionInfo,
      Guid identifier,
      X509Certificate2 certificate)
    {
      ArgumentUtility.CheckForNull<X509Certificate2>(certificate, nameof (certificate));
      byte[] bytes = Encoding.ASCII.GetBytes(certificate.Thumbprint);
      using (SigningComponent componentRaw = connectionInfo.CreateComponentRaw<SigningComponent>())
      {
        componentRaw.PartitionId = DatabasePartitionConstants.DeploymentHostPartitionId;
        componentRaw.SetPrivateKey(identifier, SigningKeyType.CertificatePassthrough, bytes, true);
      }
    }

    public static byte[] DecryptRaw(
      ISqlConnectionInfo connectionInfo,
      Guid identifier,
      byte[] encryptedData,
      SigningAlgorithm algorithm)
    {
      ArgumentUtility.CheckForNull<ISqlConnectionInfo>(connectionInfo, nameof (connectionInfo));
      ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
      ArgumentUtility.CheckForNull<byte[]>(encryptedData, nameof (encryptedData));
      using (ISigner signer = SigningManager.GetSigner(SigningServiceKey.LoadKeyDataRaw(connectionInfo, identifier).LoadCertificate(), algorithm))
      {
        TeamFoundationTracingService.TraceRaw(108106, TraceLevel.Info, nameof (TeamFoundationSigningService), "Service", "Decrypting data for database {0};{1} on partition {2} with key {3} of type {4}.", (object) connectionInfo.DataSource, (object) connectionInfo.InitialCatalog, (object) DatabasePartitionConstants.DeploymentHostPartitionId, (object) identifier, (object) signer.KeyType);
        return signer.Decrypt(encryptedData);
      }
    }

    public static byte[] EncryptRaw(
      ISqlConnectionInfo connectionInfo,
      Guid identifier,
      byte[] rawData,
      SigningAlgorithm algorithm)
    {
      ArgumentUtility.CheckForNull<ISqlConnectionInfo>(connectionInfo, nameof (connectionInfo));
      ArgumentUtility.CheckForEmptyGuid(identifier, nameof (identifier));
      ArgumentUtility.CheckForNull<byte[]>(rawData, nameof (rawData));
      using (ISigner signer = SigningManager.GetSigner(SigningServiceKey.LoadKeyDataRaw(connectionInfo, identifier).LoadCertificate(), algorithm))
      {
        TeamFoundationTracingService.TraceRaw(108104, TraceLevel.Info, nameof (TeamFoundationSigningService), "Service", "Encrypting data for database {0};{1} on partition {2} with key {3} of type {4}.", (object) connectionInfo.DataSource, (object) connectionInfo.InitialCatalog, (object) DatabasePartitionConstants.DeploymentHostPartitionId, (object) identifier, (object) signer.KeyType);
        return signer.KeyType == SigningKeyType.CertificatePassthrough ? signer.Encrypt(rawData) : throw new NotSupportedException(FrameworkResources.KeyTypeNotAllowed((object) identifier.ToString("D")));
      }
    }

    public static Guid GetDatabaseSigningKeyRaw(ISqlConnectionInfo connectionInfo)
    {
      ArgumentUtility.CheckForNull<ISqlConnectionInfo>(connectionInfo, nameof (connectionInfo));
      return RegistryHelpers.GetValueRaw<Guid>(connectionInfo, DatabasePartitionConstants.DeploymentHostPartitionId, FrameworkServerConstants.DefaultSigningKeyPath, FrameworkServerConstants.FrameworkSigningKey);
    }

    internal virtual IEnumerable<ISigningServiceKey> GetSigningKeys(
      IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      using (SigningComponent component = requestContext.CreateComponent<SigningComponent>())
      {
        if (!(component is SigningComponent5 signingComponent5))
          throw new ServiceVersionNotSupportedException(string.Format("GetPrivateKeys is available from SigningComponent starting at service level 5.  Current service level = {0}", (object) component.Version));
        return signingComponent5.GetPrivateKeys().Select<SigningComponent.SigningServiceKey, ISigningServiceKey>((Func<SigningComponent.SigningServiceKey, ISigningServiceKey>) (ssk => SigningServiceKey.CreateFromComponentType(requestContext, ssk)));
      }
    }

    public virtual HashSet<Guid> GetSigningKeysInUse(IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      HashSet<Guid> hashSet;
      using (IDisposableReadOnlyList<ISigningServiceConsumer> extensions = requestContext.GetExtensions<ISigningServiceConsumer>(throwOnError: true))
        hashSet = extensions.SelectMany<ISigningServiceConsumer, Guid>((Func<ISigningServiceConsumer, IEnumerable<Guid>>) (consumer => consumer.GetSigningKeysInUse(requestContext))).ToHashSet<Guid>();
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        hashSet.Add(this.GetDatabaseSigningKey(requestContext));
      return hashSet;
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_serviceHostId = systemRequestContext.ServiceHost.InstanceId;
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.SigningKeyChanged, new SqlNotificationCallback(this.OnSigningKeyChanged), true);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.SigningKeyChanged, new SqlNotificationCallback(this.OnSigningKeyChanged), true);

    private void CheckKeyTypeAllowedForEncryption(
      IVssRequestContext requestContext,
      Guid identifier,
      ISigner signer)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && signer.KeyType != SigningKeyType.CertificatePassthrough && signer.KeyType != SigningKeyType.KeyEncryptionKey && signer.KeyType != SigningKeyType.MasterWrappingKey)
        throw new NotSupportedException(FrameworkResources.KeyTypeNotAllowedForHost((object) requestContext.ServiceHost.InstanceId, (object) identifier.ToString("D"), (object) signer.KeyType));
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment && (signer.KeyType == SigningKeyType.PartitionSecured || signer.KeyType == SigningKeyType.DeploymentCertificateSecured))
        throw new NotSupportedException(FrameworkResources.KeyTypeMustBeRSASecuredOrStored((object) signer.KeyType));
    }

    internal virtual ISigningServiceKey GetSigningKey(
      IVssRequestContext requestContext,
      Guid identifier,
      SigningKeyType keyType = SigningKeyType.Default,
      bool createIfNotExists = true)
    {
      ISigningServiceKey signingKey1 = (ISigningServiceKey) null;
      TeamFoundationSigningService.SigningServiceCache service = requestContext.GetService<TeamFoundationSigningService.SigningServiceCache>();
      if (service.TryGetValue(requestContext, identifier, out signingKey1))
        return signingKey1;
      ISigningServiceKey signingKey2 = SigningServiceKey.LoadKeyData(requestContext, identifier);
      if (signingKey2 == null)
      {
        if (!createIfNotExists)
          return signingKey2;
        if (keyType == SigningKeyType.Default)
          keyType = this.GetDefaultKeyType(requestContext);
        if (keyType == SigningKeyType.CertificatePassthrough || keyType == SigningKeyType.DeploymentCertificateSecured)
          throw new NotSupportedException(FrameworkResources.KeyTypeNotAllowedForHost((object) requestContext.ServiceHost.InstanceId, (object) identifier.ToString("D"), (object) keyType));
        byte[] keyPair = SigningManager.GenerateKeyPair(this.GetDefaultKeyLength(requestContext));
        signingKey2 = service.WriteThrough(requestContext, identifier, keyType, keyPair, false);
      }
      else if (keyType != SigningKeyType.Default && signingKey2.KeyType != keyType)
        requestContext.TraceAlways(1234123, TraceLevel.Warning, nameof (TeamFoundationSigningService), "Service", string.Format("The key with identifier {0} does not match requested key type. Requested: {1}, Actual: {2}", (object) identifier, (object) keyType, (object) signingKey2.KeyType));
      return signingKey2;
    }

    internal virtual ISigningServiceKey GetSigningKeyV2(
      IVssRequestContext requestContext,
      Guid identifier,
      SigningKeyType keyType = SigningKeyType.Default,
      bool createIfNotExists = true)
    {
      TeamFoundationSigningService.SigningServiceCache service = requestContext.GetService<TeamFoundationSigningService.SigningServiceCache>();
      ISigningServiceKey signingKeyV2_1;
      if (service.TryGetValue(requestContext, identifier, out signingKeyV2_1))
        return signingKeyV2_1;
      ISigningServiceKey signingKeyV2_2 = SigningServiceKey.LoadKeyData(requestContext, identifier);
      if (signingKeyV2_2 == null)
      {
        if (!createIfNotExists)
          return signingKeyV2_2;
        if (keyType == SigningKeyType.Default)
          keyType = this.GetDefaultKeyTypeV2(requestContext);
        byte[] key = SigningManager.GenerateKey(keyType, this.GetDefaultKeyLength(requestContext));
        signingKeyV2_2 = service.WriteThrough(requestContext, identifier, keyType, key, false);
      }
      else if (keyType != SigningKeyType.Default && signingKeyV2_2.KeyType != keyType)
        throw new ArgumentException(string.Format("The key with identifier {0} does not match requested key type. Requested: {1}, Actual: {2}", (object) identifier, (object) keyType, (object) signingKeyV2_2.KeyType));
      return signingKeyV2_2;
    }

    private void OnSigningKeyChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      Guid result;
      if (!Guid.TryParse(eventData, out result))
        return;
      requestContext.GetService<TeamFoundationSigningService.SigningServiceCache>().Invalidate(requestContext, result);
    }

    private byte[] SignHash(
      IVssRequestContext requestContext,
      byte[] hash,
      Guid identifier,
      ISigner signer)
    {
      this.CheckKeyTypeAllowedForEncryption(requestContext, identifier, signer);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceSignsPerSec").Increment();
      return signer.SignHash(hash);
    }

    internal virtual void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!this.m_serviceHostId.Equals(requestContext.ServiceHost.InstanceId))
        throw new InvalidRequestContextHostException(FrameworkResources.CacheServiceRequestContextHostMessage((object) this.m_serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }

    public SigningKeyType GetDefaultKeyTypeV2(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return SigningKeyType.RSAStored;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return SigningKeyType.KeyEncryptionKey;
      return TeamFoundationSigningService.ShouldCreatePartitionSecuredKeys(requestContext) ? SigningKeyType.PartitionSecured : SigningKeyType.RsaSecuredByKeyEncryptionKey;
    }

    public SigningKeyType GetDefaultKeyType(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return SigningKeyType.RSAStored;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return SigningKeyType.CertificatePassthrough;
      return TeamFoundationSigningService.ShouldCreatePartitionSecuredKeys(requestContext) ? SigningKeyType.PartitionSecured : SigningKeyType.RSASecured;
    }

    private static bool ShouldCreatePartitionSecuredKeys(IVssRequestContext requestContext)
    {
      HostMigrationSigningState migrationSigningState = MigrationRegistryUtil.GetMigrationSigningState(requestContext);
      return migrationSigningState == HostMigrationSigningState.Preparing || migrationSigningState == HostMigrationSigningState.Migrating;
    }

    private bool VerifyHash(
      IVssRequestContext requestContext,
      byte[] hash,
      byte[] signature,
      ISigner signer)
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_SigningServiceVerifiesPerSec").Increment();
      return signer.VerifyHash(hash, signature);
    }

    protected int GetDefaultKeyLength(IVssRequestContext requestContext)
    {
      requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.DefaultSigningKeyLengthPath, 2048);
    }

    private void TraceCertificateChanges(IVssRequestContext requestContext)
    {
      using (EventLogReader eventLogReader = new EventLogReader("%SystemRoot%\\System32\\Winevt\\Logs\\Microsoft-Windows-CertificateServicesClient-Lifecycle-System%4Operational.evtx", PathType.FilePath))
      {
        EventRecord eventRecord;
        while ((eventRecord = eventLogReader.ReadEvent()) != null)
        {
          using (eventRecord)
          {
            if (eventRecord.Id != 1004)
            {
              if (eventRecord.Id != 1006)
                continue;
            }
            if (eventRecord is EventLogRecord eventLogRecord)
              requestContext.Trace(27822051, TraceLevel.Error, nameof (TeamFoundationSigningService), "Service", "{0} {1}: {2}", (object) eventLogRecord.TimeCreated, (object) eventLogRecord.Id, (object) eventLogRecord.ToXml());
          }
        }
      }
    }

    internal class SigningServiceCache : VssVersionedMemoryCacheService<Guid, ISigningServiceKey>
    {
      public ISigningServiceKey WriteThrough(
        IVssRequestContext requestContext,
        Guid identifier,
        SigningKeyType keyType,
        byte[] rawKeyData,
        bool overwriteExisting)
      {
        Tuple<ISigningServiceKey, int> tuple;
        using (IVssVersionedCacheContext<Guid, ISigningServiceKey> versionedContext = this.CreateVersionedContext(requestContext))
        {
          tuple = SigningServiceKey.StoreKeyData(requestContext, identifier, keyType, rawKeyData, overwriteExisting);
          int num = (int) versionedContext.TryUpdate(requestContext, identifier, tuple.Item1);
        }
        this.FireNotificationIfNecessary(requestContext, identifier, tuple.Item2);
        return tuple.Item1;
      }

      public ISigningServiceKey WriteThrough(
        IVssRequestContext requestContext,
        ISigningServiceKey sourceKey,
        SigningKeyType keyType)
      {
        Tuple<ISigningServiceKey, int> tuple;
        using (IVssVersionedCacheContext<Guid, ISigningServiceKey> versionedContext = this.CreateVersionedContext(requestContext))
        {
          tuple = SigningServiceKey.UpdateKeyType(requestContext, sourceKey, keyType);
          int num = (int) versionedContext.TryUpdate(requestContext, sourceKey.Identifier, tuple.Item1);
        }
        this.FireNotificationIfNecessary(requestContext, sourceKey.Identifier, tuple.Item2);
        return tuple.Item1;
      }

      public void Invalidate(IVssRequestContext requestContext, Guid identifier)
      {
        using (IVssVersionedCacheContext<Guid, ISigningServiceKey> versionedContext = this.CreateVersionedContext(requestContext))
          versionedContext.Invalidate(requestContext, identifier);
      }

      private void FireNotificationIfNecessary(
        IVssRequestContext requestContext,
        Guid identifier,
        int componentVersion)
      {
        if (componentVersion > 3)
          return;
        requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.SigningKeyChanged, identifier.ToString());
      }
    }
  }
}
