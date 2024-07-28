// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle.TokenSigningKeyLifecycleServiceBase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle
{
  public abstract class TokenSigningKeyLifecycleServiceBase : 
    ITokenSigningKeyLifecycleService,
    IVssFrameworkService
  {
    protected const string TraceArea = "SigningKeyLifecycle";
    protected const string TraceLayer = "Service";
    protected TokenSigningKeyNamespaceCache LocalCache;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.LocalCache = new TokenSigningKeyNamespaceCache(this.ExpirationInterval);

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.LocalCache.Clear();

    public TokenSigningKey GetSigningKey(
      IVssRequestContext requestContext,
      string signingKeyNamespaceName,
      DateTimeOffset validTo)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(signingKeyNamespaceName, nameof (signingKeyNamespaceName));
      ArgumentUtility.CheckStringExactLength(signingKeyNamespaceName, 3, nameof (signingKeyNamespaceName));
      ArgumentUtility.CheckStringCasing(signingKeyNamespaceName, nameof (signingKeyNamespaceName), true);
      requestContext.TraceEnter(10050030, "SigningKeyLifecycle", "Service", nameof (GetSigningKey));
      try
      {
        return this.GetKey(requestContext, signingKeyNamespaceName, (Func<TokenSigningKey>) (() => this.GetCachedSigningKey(requestContext, signingKeyNamespaceName, validTo)));
      }
      finally
      {
        requestContext.TraceLeave(10050039, "SigningKeyLifecycle", "Service", nameof (GetSigningKey));
      }
    }

    public TokenSigningKey GetValidationKey(
      IVssRequestContext requestContext,
      string signingKeyNamespaceName,
      int keyId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(signingKeyNamespaceName, nameof (signingKeyNamespaceName));
      ArgumentUtility.CheckStringExactLength(signingKeyNamespaceName, 3, nameof (signingKeyNamespaceName));
      ArgumentUtility.CheckStringCasing(signingKeyNamespaceName, nameof (signingKeyNamespaceName), true);
      ArgumentUtility.CheckGreaterThanZero((float) keyId, nameof (keyId));
      requestContext.TraceEnter(10050000, "SigningKeyLifecycle", "Service", nameof (GetValidationKey));
      try
      {
        return this.GetKey(requestContext, signingKeyNamespaceName, (Func<TokenSigningKey>) (() => this.GetCachedValidationKey(requestContext, signingKeyNamespaceName, keyId)));
      }
      finally
      {
        requestContext.TraceLeave(10050009, "SigningKeyLifecycle", "Service", nameof (GetValidationKey));
      }
    }

    private TokenSigningKey GetKey(
      IVssRequestContext requestContext,
      string signingKeyNamespaceName,
      Func<TokenSigningKey> getCachedKey)
    {
      this.CheckPermission(requestContext);
      TokenSigningKey key1 = getCachedKey();
      if (key1 != null)
        return key1;
      TokenSigningKeyNamespace namespaceObject = this.RetrieveNamespace(requestContext, signingKeyNamespaceName);
      if (namespaceObject == null)
      {
        string message = "Namespace data is retrieved as null for namespace: " + signingKeyNamespaceName;
        requestContext.Trace(10050003, TraceLevel.Error, "SigningKeyLifecycle", "Service", message);
        throw new TokenSigningKeyLifecycleOperationFailedException(message);
      }
      this.LocalCache.Set(namespaceObject.Name, namespaceObject);
      TokenSigningKey key2 = getCachedKey();
      if (key2 == null)
      {
        string message = "No key found with satisfying conditions for namespace: " + signingKeyNamespaceName;
        requestContext.Trace(10050003, TraceLevel.Error, "SigningKeyLifecycle", "Service", message);
        throw new TokenSigningKeyLifecycleOperationFailedException(message);
      }
      return key2;
    }

    private TokenSigningKey GetCachedSigningKey(
      IVssRequestContext requestContext,
      string signingKeyNamespaceName,
      DateTimeOffset validTo)
    {
      requestContext.TraceEnter(10050040, "SigningKeyLifecycle", "Service", nameof (GetCachedSigningKey));
      try
      {
        TokenSigningKeyNamespace signingKeyNamespace = (TokenSigningKeyNamespace) null;
        if (!this.LocalCache.TryGetValue(signingKeyNamespaceName, out signingKeyNamespace) || signingKeyNamespace == null || signingKeyNamespace.SigningKeyIds == null || signingKeyNamespace.ValidationKeys == null)
          return (TokenSigningKey) null;
        TokenSigningKeyMetadata[] array = signingKeyNamespace.SigningKeyIds.Select<int, TokenSigningKeyMetadata>((Func<int, TokenSigningKeyMetadata>) (x => signingKeyNamespace.ValidationKeys[x])).Where<TokenSigningKeyMetadata>((Func<TokenSigningKeyMetadata, bool>) (x => x.ValidationValidTo.ToUniversalTime() >= validTo.ToUniversalTime() && x.SigningValidTo.ToUniversalTime() >= DateTimeOffset.UtcNow)).ToArray<TokenSigningKeyMetadata>();
        TokenSigningKeyMetadata signingKeyMetadata = (TokenSigningKeyMetadata) null;
        if (array.Length == 1)
        {
          signingKeyMetadata = array[0];
        }
        else
        {
          if (array.Length <= 1)
            return (TokenSigningKey) null;
          using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
          {
            byte[] data = new byte[4];
            randomNumberGenerator.GetBytes(data);
            uint uint32 = BitConverter.ToUInt32(data, 0);
            signingKeyMetadata = array[(long) uint32 % (long) array.Length];
          }
        }
        return this.RetrieveKeyFromStrongBox(requestContext, signingKeyNamespaceName, signingKeyMetadata.KeyId);
      }
      finally
      {
        requestContext.TraceLeave(10050049, "SigningKeyLifecycle", "Service", nameof (GetCachedSigningKey));
      }
    }

    private TokenSigningKey GetCachedValidationKey(
      IVssRequestContext requestContext,
      string signingKeyNamespaceName,
      int keyId)
    {
      requestContext.TraceEnter(10050010, "SigningKeyLifecycle", "Service", nameof (GetCachedValidationKey));
      try
      {
        TokenSigningKeyNamespace namespaceObject = (TokenSigningKeyNamespace) null;
        TokenSigningKeyMetadata signingKeyMetadata = (TokenSigningKeyMetadata) null;
        if (!this.LocalCache.TryGetValue(signingKeyNamespaceName, out namespaceObject) || !namespaceObject.ValidationKeys.TryGetValue(keyId, out signingKeyMetadata))
          return (TokenSigningKey) null;
        if (signingKeyMetadata.ValidationValidTo.ToUniversalTime() < DateTimeOffset.UtcNow)
        {
          string message = string.Format("Validation key with id: {0} for namespace: {1} is expired on: {2}", (object) keyId, (object) signingKeyNamespaceName, (object) signingKeyMetadata.ValidationValidTo);
          requestContext.Trace(10050011, TraceLevel.Error, "SigningKeyLifecycle", "Service", message);
          throw new TokenSigningKeyLifecycleKeyExpiredException(message);
        }
        return this.RetrieveKeyFromStrongBox(requestContext, signingKeyNamespaceName, keyId);
      }
      finally
      {
        requestContext.TraceLeave(10050019, "SigningKeyLifecycle", "Service", nameof (GetCachedValidationKey));
      }
    }

    protected TokenSigningKey RetrieveKeyFromStrongBox(
      IVssRequestContext requestContext,
      string signingKeyNamespaceName,
      int keyId)
    {
      requestContext.TraceEnter(10050020, "SigningKeyLifecycle", "Service", nameof (RetrieveKeyFromStrongBox));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid drawerId = service.UnlockDrawer(vssRequestContext, this.GetDrawerName(requestContext, signingKeyNamespaceName), false);
        if (drawerId == Guid.Empty)
        {
          requestContext.Trace(10050021, TraceLevel.Verbose, "SigningKeyLifecycle", "Service", "Drawer cannot be found in strong box for key with id: {0} for namespace: {1}", (object) keyId, (object) signingKeyNamespaceName);
          return this.HandleStrongBoxMiss(requestContext, signingKeyNamespaceName, keyId, true);
        }
        try
        {
          string str = service.GetString(vssRequestContext, drawerId, keyId.ToString());
          return new TokenSigningKey()
          {
            SigningKeyNamespace = signingKeyNamespaceName,
            KeyId = keyId,
            KeyData = str
          };
        }
        catch (StrongBoxItemNotFoundException ex)
        {
          requestContext.Trace(10050021, TraceLevel.Verbose, "SigningKeyLifecycle", "Service", "Key cannot be found in strong box for key with id: {0} for namespace: {1}", (object) keyId, (object) signingKeyNamespaceName);
          return this.HandleStrongBoxMiss(requestContext, signingKeyNamespaceName, keyId, false);
        }
      }
      finally
      {
        requestContext.TraceLeave(10050029, "SigningKeyLifecycle", "Service", nameof (RetrieveKeyFromStrongBox));
      }
    }

    protected virtual string GetDrawerName(
      IVssRequestContext requestContext,
      string signingKeyNamespaceName)
    {
      return signingKeyNamespaceName;
    }

    protected internal abstract TokenSigningKey HandleStrongBoxMiss(
      IVssRequestContext requestContext,
      string signingKeyNamespaceName,
      int keyId,
      bool createDrawer);

    protected internal abstract TokenSigningKeyNamespace RetrieveNamespace(
      IVssRequestContext requestContext,
      string signingKeyNamespaceName);

    private void CheckPermission(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!requestContext.IsSystemContext)
        throw new AccessCheckException(FrameworkResources.InvalidAccessException());
    }

    protected virtual TimeSpan ExpirationInterval { get; } = TimeSpan.FromHours(1.0);
  }
}
