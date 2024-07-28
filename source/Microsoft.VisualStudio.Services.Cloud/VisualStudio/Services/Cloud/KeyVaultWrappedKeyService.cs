// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KeyVaultWrappedKeyService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Rest;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class KeyVaultWrappedKeyService : IKeyVaultWrappedKeyService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public byte[] WrapKey(IVssRequestContext requestContext, string keyId, byte[] key)
    {
      using (requestContext.TraceBlock(90003321, 90003323, KeyVaultWrappedKeyConstants.TracePoints.Area, KeyVaultWrappedKeyConstants.TracePoints.Layer, nameof (WrapKey)))
      {
        KeyIdentifier keyIdentifier = KeyVaultWrappedKeyService.CreateKeyIdentifier(keyId, true);
        try
        {
          IKeyVaultClientAdapter keyVaultClient = this.GetKeyVaultClient(requestContext);
          return requestContext.RunSynchronously<KeyOperationResult>((Func<Task<KeyOperationResult>>) (() => keyVaultClient.WrapKeyAsync(keyIdentifier.Vault, keyIdentifier.Name, keyIdentifier.Version, key, new CancellationToken()))).Result;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(90003322, KeyVaultWrappedKeyConstants.TracePoints.Area, KeyVaultWrappedKeyConstants.TracePoints.Layer, ex);
          throw new KeyVaultWrappedKeyOperationFailedException(nameof (WrapKey), (object) keyIdentifier, ex);
        }
      }
    }

    public byte[] UnwrapKey(IVssRequestContext requestContext, string keyId, byte[] encryptedKey)
    {
      using (requestContext.TraceBlock(90003331, 90003333, KeyVaultWrappedKeyConstants.TracePoints.Area, KeyVaultWrappedKeyConstants.TracePoints.Layer, nameof (UnwrapKey)))
      {
        KeyIdentifier keyIdentifier = KeyVaultWrappedKeyService.CreateKeyIdentifier(keyId, true);
        try
        {
          requestContext.TraceAlways(90003324, TraceLevel.Info, KeyVaultWrappedKeyConstants.TracePoints.Area, KeyVaultWrappedKeyConstants.TracePoints.Layer, "Unwrapping content using keyId {0}", (object) keyId);
          IKeyVaultClientAdapter keyVaultClient = this.GetKeyVaultClient(requestContext);
          return requestContext.RunSynchronously<KeyOperationResult>((Func<Task<KeyOperationResult>>) (() => keyVaultClient.UnwrapKeyAsync(keyIdentifier.Vault, keyIdentifier.Name, keyIdentifier.Version, encryptedKey, new CancellationToken()))).Result;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(90003332, KeyVaultWrappedKeyConstants.TracePoints.Area, KeyVaultWrappedKeyConstants.TracePoints.Layer, ex);
          throw new KeyVaultWrappedKeyOperationFailedException(nameof (UnwrapKey), (object) keyIdentifier, ex);
        }
      }
    }

    private static KeyIdentifier CreateKeyIdentifier(string keyId, bool requireVersion = false)
    {
      try
      {
        KeyIdentifier keyIdentifier = new KeyIdentifier(keyId);
        if (requireVersion && string.IsNullOrEmpty(keyIdentifier.Version))
          throw new ArgumentNullException(string.Format("Key version is required: {0}", (object) keyIdentifier));
        return keyIdentifier;
      }
      catch (Exception ex)
      {
        throw new KeyVaultWrappedKeyInvalidKeyIdException(keyId, ex);
      }
    }

    private IKeyVaultClientAdapter GetKeyVaultClient(IVssRequestContext requestContext) => KeyVaultWrappedKeyService.CreateKeyVaultClient(requestContext);

    private static IKeyVaultClientAdapter CreateKeyVaultClient(IVssRequestContext requestContext)
    {
      int num = requestContext.IsFeatureEnabled("AzureDevOps.Services.ManagedIdentity.UseForKeyVaultAccess") ? 1 : 0;
      TraceLogger traceLogger = new TraceLogger(requestContext, KeyVaultWrappedKeyConstants.TracePoints.Area, KeyVaultWrappedKeyConstants.TracePoints.Layer);
      TraceLogger logger = traceLogger;
      return KeyVaultClientAdapterFactory.GetKeyVaultClientAdapter((ServiceClientCredentials) new DefaultKeyVaultCredentials(num != 0, (ITFLogger) logger), true, (ITFLogger) traceLogger);
    }
  }
}
