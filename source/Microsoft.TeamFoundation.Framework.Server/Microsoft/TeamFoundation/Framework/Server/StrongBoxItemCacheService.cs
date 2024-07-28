// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxItemCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StrongBoxItemCacheService : 
    VssVersionedMemoryCacheService<StrongBoxItemName, ItemCacheEntry>
  {
    private static readonly Lazy<XmlSerializer> s_itemNameSerializer = new Lazy<XmlSerializer>((Func<XmlSerializer>) (() => new XmlSerializer(typeof (List<StrongBoxItemName>))));
    private readonly ItemChacheEntryComparer c_itemChacheEntryComparer = new ItemChacheEntryComparer();
    private const int NTE_BAD_KEYSET = -2146893802;

    public StrongBoxItemCacheService()
      : base((IEqualityComparer<StrongBoxItemName>) null, new MemoryCacheConfiguration<StrongBoxItemName, ItemCacheEntry>().WithMaxSize(long.MaxValue, (ISizeProvider<StrongBoxItemName, ItemCacheEntry>) new StrongBoxItemSizeProvider()))
    {
    }

    internal void StartService(IVssRequestContext requestContext, bool forceUseOrderedCache = false) => this.ServiceStart(requestContext);

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      bool flag = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment);
      CaptureLength maxCacheLength = this.MaxCacheLength;
      IVssRegistryService registryService1 = service;
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery registryQuery = (RegistryQuery) FrameworkServerConstants.StrongBoxCacheItemCount;
      ref RegistryQuery local1 = ref registryQuery;
      int defaultValue = flag ? 5000 : 0;
      int num1 = registryService1.GetValue<int>(requestContext1, in local1, defaultValue);
      maxCacheLength.Value = num1;
      CaptureSize maxCacheSize = this.MaxCacheSize;
      IVssRegistryService registryService2 = service;
      IVssRequestContext requestContext2 = requestContext;
      registryQuery = (RegistryQuery) FrameworkServerConstants.StrongBoxCacheByteCount;
      ref RegistryQuery local2 = ref registryQuery;
      long num2 = registryService2.GetValue<long>(requestContext2, in local2, 2097152L);
      maxCacheSize.Value = num2;
      base.ServiceStart(requestContext);
    }

    internal StrongBoxItemInfo GetItem(
      IVssRequestContext requestContext,
      Guid drawerId,
      string lookupKey,
      bool forceUpdateCache,
      Func<StrongBoxItemInfo> missDelegate)
    {
      StrongBoxItemName strongBoxItemName = new StrongBoxItemName()
      {
        DrawerId = drawerId,
        LookupKey = lookupKey
      };
      ItemCacheEntry itemCacheEntry;
      if (!(!this.TryGetValue(requestContext, strongBoxItemName, out itemCacheEntry) | forceUpdateCache))
        return itemCacheEntry.Item;
      using (IVssVersionedCacheContext<StrongBoxItemName, ItemCacheEntry> context = this.CreateContext(requestContext, strongBoxItemName))
      {
        StrongBoxItemInfo strongBoxItemInfo = missDelegate();
        if (strongBoxItemInfo != null)
        {
          itemCacheEntry.Item = strongBoxItemInfo;
          CacheUpdateResult cacheUpdateResult = context.TryUpdate(requestContext, strongBoxItemName, itemCacheEntry);
          if (cacheUpdateResult != CacheUpdateResult.Success)
            requestContext.Trace(109118, TraceLevel.Verbose, "StrongBox", "Service", "Unable to add StrongBoxItemInfo ({0}, {1}, {2}) to cache. Reason:{3}", (object) drawerId, (object) lookupKey, (object) forceUpdateCache, (object) cacheUpdateResult);
        }
        return strongBoxItemInfo;
      }
    }

    public void InvalidateItem(IVssRequestContext requestContext, Guid drawerId, string lookupKey)
    {
      StrongBoxItemName strongBoxItemName = new StrongBoxItemName()
      {
        DrawerId = drawerId,
        LookupKey = lookupKey
      };
      using (IVssVersionedCacheContext<StrongBoxItemName, ItemCacheEntry> context = this.CreateContext(requestContext, strongBoxItemName))
        context.Invalidate(requestContext, strongBoxItemName);
    }

    internal byte[] GetDecryptedBytes(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      bool forceUpdateCache,
      Func<byte[]> missDelegate)
    {
      requestContext.TraceEnter(109094, "StrongBox", "Service", nameof (GetDecryptedBytes));
      try
      {
        if (item == null)
        {
          requestContext.Trace(109096, TraceLevel.Verbose, "StrongBox", "Service", "GetDecryptedBytes - Calling miss delegate - Null item");
          return missDelegate();
        }
        StrongBoxItemName strongBoxItemName = new StrongBoxItemName()
        {
          DrawerId = item.DrawerId,
          LookupKey = item.LookupKey
        };
        ItemCacheEntry itemCacheEntry;
        if (!this.TryGetValue(requestContext, strongBoxItemName, out itemCacheEntry))
        {
          requestContext.Trace(109097, TraceLevel.Verbose, "StrongBox", "Service", string.Format("GetDecryptedBytes - Calling miss delegate - Item not found DrawerId: {0}, LookupKey {1}.", (object) item.DrawerId, (object) item.LookupKey));
          return missDelegate();
        }
        if (itemCacheEntry.ProtectedContent != null && !forceUpdateCache)
          return ProtectedData.Unprotect(itemCacheEntry.ProtectedContent, (byte[]) null, DataProtectionScope.CurrentUser);
        string str = forceUpdateCache ? "Force update" : "ProtectedContent is null";
        requestContext.Trace(109098, TraceLevel.Verbose, "StrongBox", "Service", string.Format("GetDecryptedBytes - Calling miss delegate - {0} DrawerId: {1}, LookupKey {2}.", (object) str, (object) item.DrawerId, (object) item.LookupKey));
        using (IVssVersionedCacheContext<StrongBoxItemName, ItemCacheEntry> context = this.CreateContext(requestContext, strongBoxItemName))
        {
          byte[] userData = missDelegate();
          itemCacheEntry.ProtectedContent = ProtectedData.Protect(userData, (byte[]) null, DataProtectionScope.CurrentUser);
          requestContext.Trace(109099, TraceLevel.Verbose, "StrongBox", "Service", "GetDecryptedBytes - Updating cache");
          CacheUpdateResult cacheUpdateResult = context.TryUpdate(requestContext, strongBoxItemName, itemCacheEntry);
          if (cacheUpdateResult != CacheUpdateResult.Success)
            requestContext.Trace(109119, TraceLevel.Verbose, "StrongBox", "Service", "Unable to add StrongBoxItemInfo decrypted bytes ({0}, {1}, {2}) to cache. Reason:{3}", (object) item.DrawerId, (object) item.LookupKey, (object) forceUpdateCache, (object) cacheUpdateResult);
          return userData;
        }
      }
      finally
      {
        requestContext.TraceLeave(109095, "StrongBox", "Service", nameof (GetDecryptedBytes));
      }
    }

    internal X509Certificate2 GetCertificate(
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      bool exportable,
      bool expectPrivateKey,
      bool forceUpdateCache,
      Func<X509Certificate2> missDelegate)
    {
      requestContext.TraceEnter(109098, "StrongBox", "Service", nameof (GetCertificate));
      try
      {
        StrongBoxItemName strongBoxItemName = new StrongBoxItemName()
        {
          DrawerId = item.DrawerId,
          LookupKey = item.LookupKey
        };
        ItemCacheEntry itemCacheEntry;
        if (!this.TryGetValue(requestContext, strongBoxItemName, out itemCacheEntry))
        {
          requestContext.Trace(109100, TraceLevel.Verbose, "StrongBox", "Service", "GetCertificate - Calling miss delegate - item not cached");
          return missDelegate();
        }
        using (IVssVersionedCacheContext<StrongBoxItemName, ItemCacheEntry> context = this.CreateContext(requestContext, strongBoxItemName))
        {
          X509Certificate2 certificate = exportable ? itemCacheEntry.ExportableCertificate : itemCacheEntry.NonExportableCertificate;
          if (certificate != null && ((!expectPrivateKey || certificate.HasPrivateKey ? (!this.ValidatePrivateKey(requestContext, certificate) ? 1 : 0) : 1) | (forceUpdateCache ? 1 : 0)) != 0)
            certificate = (X509Certificate2) null;
          if (certificate == null)
          {
            requestContext.Trace(109100, TraceLevel.Verbose, "StrongBox", "Service", "GetCertificate - Calling miss delegate - Certificate not cached.");
            certificate = missDelegate();
            requestContext.Trace(109101, TraceLevel.Verbose, "StrongBox", "Service", "GetCertificate - Updating cache");
            if (exportable)
              itemCacheEntry.ExportableCertificate = certificate;
            else
              itemCacheEntry.NonExportableCertificate = certificate;
            CacheUpdateResult cacheUpdateResult = context.TryUpdate(requestContext, strongBoxItemName, itemCacheEntry);
            if (cacheUpdateResult != CacheUpdateResult.Success)
              requestContext.Trace(109120, TraceLevel.Verbose, "StrongBox", "Service", "Unable to add StrongBoxItemInfo certificate ({0}, {1}, {2}) to cache. Reason:{3}", (object) item.DrawerId, (object) item.LookupKey, (object) forceUpdateCache, (object) cacheUpdateResult);
          }
          return certificate;
        }
      }
      finally
      {
        requestContext.TraceLeave(109099, "StrongBox", "Service", nameof (GetCertificate));
      }
    }

    private IVssVersionedCacheContext<StrongBoxItemName, ItemCacheEntry> CreateContext(
      IVssRequestContext requestContext,
      StrongBoxItemName name)
    {
      ItemCacheEntry itemCacheEntry;
      return this.CreateVersionedContext<DateTime>(requestContext, (Func<StrongBoxItemName, DateTime>) (key => this.TryGetValue(requestContext, key, out itemCacheEntry) ? itemCacheEntry.Item.LastUpdateTime.GetValueOrDefault() : new DateTime()), name);
    }

    private bool ValidatePrivateKey(IVssRequestContext requestContext, X509Certificate2 certificate)
    {
      try
      {
        if (certificate.HasPrivateKey)
        {
          AsymmetricAlgorithm rsaPrivateKey = (AsymmetricAlgorithm) certificate.GetRSAPrivateKey();
          requestContext.Trace(749933450, TraceLevel.Verbose, "StrongBox", "Service", "StrongBox cert with thumbprint {0}: Private key signature algorithm={1}, key exchange algorithm={2}, key size={3}", (object) certificate.Thumbprint, (object) rsaPrivateKey.SignatureAlgorithm, (object) rsaPrivateKey.KeyExchangeAlgorithm, (object) rsaPrivateKey.KeySize);
        }
        else
          requestContext.Trace(749933451, TraceLevel.Verbose, "StrongBox", "Service", "StrongBoxCert with thumbprint {0}: no private key detected", (object) certificate.Thumbprint);
      }
      catch (CryptographicException ex) when (ex.HResult == -2146893802)
      {
        requestContext.TraceException(8708879, "StrongBox", "Service", (Exception) ex);
        return false;
      }
      return true;
    }
  }
}
