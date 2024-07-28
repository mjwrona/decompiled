// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskRequestMessage.StrongBoxBasedEncryptionKeyProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskRequestMessage
{
  internal class StrongBoxBasedEncryptionKeyProvider : IAesEncryptionKeyProvider
  {
    private static string m_drawerName;
    private static string m_lookupKey;
    private static bool m_createKeyIfNotFound;
    private static readonly string m_leaseName = "SetStrongBoxEncryptionKeyLease";
    private const int leaseTime = 5;
    private const int leaseAcquireTimeout = 5;

    public StrongBoxBasedEncryptionKeyProvider(
      string drawerName,
      string lookupKey,
      bool createKeyIfNotFound)
    {
      StrongBoxBasedEncryptionKeyProvider.m_drawerName = drawerName;
      StrongBoxBasedEncryptionKeyProvider.m_lookupKey = lookupKey;
      StrongBoxBasedEncryptionKeyProvider.m_createKeyIfNotFound = createKeyIfNotFound;
    }

    public byte[] GetKey(IVssRequestContext requestContext)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, StrongBoxBasedEncryptionKeyProvider.m_drawerName, StrongBoxBasedEncryptionKeyProvider.m_lookupKey, false);
      if (itemInfo != null)
      {
        string s = service.GetString(requestContext.Elevate(), itemInfo);
        if (!string.IsNullOrEmpty(s))
          return Convert.FromBase64String(s);
      }
      if (StrongBoxBasedEncryptionKeyProvider.m_createKeyIfNotFound)
        return this.GenerateKey(requestContext);
      requestContext.TraceError(10016002, "DistributedTask", TaskResources.EncryptionKeyNotFound((object) StrongBoxBasedEncryptionKeyProvider.m_drawerName, (object) StrongBoxBasedEncryptionKeyProvider.m_lookupKey));
      throw new InvalidOperationException(TaskResources.UnableToCompleteOperationSecurely());
    }

    private byte[] GenerateKey(IVssRequestContext requestContext)
    {
      byte[] key;
      using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
      {
        cryptoServiceProvider.KeySize = 256;
        cryptoServiceProvider.Mode = CipherMode.CBC;
        cryptoServiceProvider.Padding = PaddingMode.PKCS7;
        cryptoServiceProvider.GenerateKey();
        key = cryptoServiceProvider.Key;
      }
      ILeaseInfo leaseInfo = (ILeaseInfo) null;
      try
      {
        ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
        if (!requestContext.GetService<ILeaseService>().TryAcquireLease(requestContext, StrongBoxBasedEncryptionKeyProvider.m_leaseName, TimeSpan.FromMinutes(5.0), TimeSpan.FromMinutes(5.0), out leaseInfo))
        {
          requestContext.TraceError(10016002, "DistributedTask", TaskResources.UnableToAcquireLease((object) StrongBoxBasedEncryptionKeyProvider.m_leaseName), (object) StrongBoxBasedEncryptionKeyProvider.m_leaseName);
          throw new InvalidOperationException(TaskResources.UnableToAcquireLease((object) StrongBoxBasedEncryptionKeyProvider.m_leaseName));
        }
        try
        {
          Guid drawer = service.CreateDrawer(requestContext.Elevate(), StrongBoxBasedEncryptionKeyProvider.m_drawerName);
          service.AddString(requestContext.Elevate(), drawer, StrongBoxBasedEncryptionKeyProvider.m_lookupKey, Convert.ToBase64String(key));
        }
        catch (StrongBoxDrawerExistsException ex)
        {
          Guid drawerId = service.UnlockDrawer(requestContext.Elevate(), StrongBoxBasedEncryptionKeyProvider.m_drawerName, false);
          string s = service.GetString(requestContext.Elevate(), drawerId, StrongBoxBasedEncryptionKeyProvider.m_lookupKey);
          if (!string.IsNullOrEmpty(s))
            return Convert.FromBase64String(s);
          service.AddString(requestContext.Elevate(), drawerId, StrongBoxBasedEncryptionKeyProvider.m_lookupKey, Convert.ToBase64String(key));
        }
        return key;
      }
      catch (Exception ex)
      {
        requestContext.TraceError(10016002, "DistributedTask", "Error while storing enryption key in strong box", (object) TeamFoundationExceptionFormatter.FormatException(ex, false));
        throw new InvalidOperationException(TaskResources.UnableToCompleteOperationSecurely());
      }
      finally
      {
        leaseInfo?.Dispose();
      }
    }
  }
}
