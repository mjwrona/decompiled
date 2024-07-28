// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryKeyService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class GalleryKeyService : IGalleryKeyService, IVssFrameworkService
  {
    private const string KeyServiceLock = "KeyServiceLock:{0}";
    private const int MaxLookupAge = 300000;
    private HashSet<string> s_validKeyNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal)
    {
      "AccountSigningKey",
      "AssetSigningKey"
    };
    private Dictionary<string, GalleryKeyService.CachedItems> m_cachedItems = new Dictionary<string, GalleryKeyService.CachedItems>((IEqualityComparer<string>) StringComparer.Ordinal);

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void CreateKey(
      IVssRequestContext requestContext,
      string keyName,
      int keyLength,
      int expireCurrentSeconds = -1)
    {
      string base64String = Convert.ToBase64String(TokenManagement.CreateSigningKey(keyLength));
      this.VerifyKey(requestContext, keyName);
      using (requestContext.GetService<ITeamFoundationLockingService>().AcquireLock(requestContext, TeamFoundationLockMode.Exclusive, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "KeyServiceLock:{0}", (object) keyName.ToString())))
      {
        StrongBoxItemInfo strongBoxItemInfo1 = (StrongBoxItemInfo) null;
        int num = 1;
        Guid drawerId;
        List<StrongBoxItemInfo> itemsFromStrongBox = this.GetItemsFromStrongBox(requestContext, keyName, out drawerId);
        if (itemsFromStrongBox != null && itemsFromStrongBox.Count > 0)
        {
          strongBoxItemInfo1 = itemsFromStrongBox[0];
          strongBoxItemInfo1.ExpirationDate = new DateTime?(DateTime.UtcNow.AddSeconds((double) expireCurrentSeconds));
          num = int.Parse(strongBoxItemInfo1.LookupKey) + 1;
        }
        ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
        List<Tuple<StrongBoxItemInfo, string>> items = new List<Tuple<StrongBoxItemInfo, string>>();
        if (strongBoxItemInfo1 != null)
          items.Add(new Tuple<StrongBoxItemInfo, string>(strongBoxItemInfo1, service.GetString(requestContext, strongBoxItemInfo1)));
        StrongBoxItemInfo strongBoxItemInfo2 = new StrongBoxItemInfo()
        {
          LookupKey = num.ToString(),
          DrawerId = drawerId,
          ItemKind = StrongBoxItemKind.String
        };
        items.Add(new Tuple<StrongBoxItemInfo, string>(strongBoxItemInfo2, base64String));
        service.AddStrings(requestContext, items);
        this.PublishEvent(requestContext, new KeyChangeEvent()
        {
          KeyName = keyName
        });
      }
    }

    public string ReadKey(IVssRequestContext requestContext, string keyName, bool allowCachedRead = true)
    {
      string str = (string) null;
      this.VerifyKey(requestContext, keyName);
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      List<StrongBoxItemInfo> strongBoxItemInfoList = !allowCachedRead ? this.GetItemsFromStrongBox(requestContext, keyName) : this.GetItems(requestContext, keyName);
      if (strongBoxItemInfoList != null && strongBoxItemInfoList.Count > 0)
        str = service.GetString(requestContext, strongBoxItemInfoList[0].DrawerId, strongBoxItemInfoList[0].LookupKey);
      return str;
    }

    public IEnumerable<string> ReadKeys(
      IVssRequestContext requestContext,
      string keyName,
      bool allowCachedRead = true)
    {
      List<string> stringList = new List<string>();
      this.VerifyKey(requestContext, keyName);
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      List<StrongBoxItemInfo> strongBoxItemInfoList = !allowCachedRead ? this.GetItemsFromStrongBox(requestContext, keyName) : this.GetItems(requestContext, keyName);
      if (strongBoxItemInfoList != null && strongBoxItemInfoList.Count > 0)
      {
        foreach (StrongBoxItemInfo strongBoxItemInfo in strongBoxItemInfoList)
        {
          if (!strongBoxItemInfo.ExpirationDate.HasValue || strongBoxItemInfo.ExpirationDate.Value > DateTime.UtcNow)
            stringList.Add(service.GetString(requestContext, strongBoxItemInfo));
        }
      }
      return (IEnumerable<string>) stringList;
    }

    private List<StrongBoxItemInfo> GetItems(IVssRequestContext requestContext, string keyName)
    {
      GalleryKeyService.CachedItems cachedItems;
      if (!this.m_cachedItems.TryGetValue(keyName, out cachedItems) || DateTime.UtcNow.Subtract(cachedItems.CacheTime).TotalMilliseconds > 300000.0)
      {
        List<StrongBoxItemInfo> itemsFromStrongBox = this.GetItemsFromStrongBox(requestContext, keyName);
        if (itemsFromStrongBox != null && itemsFromStrongBox.Count > 0)
        {
          cachedItems = new GalleryKeyService.CachedItems()
          {
            DrawerId = itemsFromStrongBox[0].DrawerId,
            CacheTime = DateTime.UtcNow
          };
          cachedItems.Items = new List<StrongBoxItemInfo>();
          foreach (StrongBoxItemInfo strongBoxItemInfo in itemsFromStrongBox)
            cachedItems.Items.Add(strongBoxItemInfo);
          this.m_cachedItems[keyName] = cachedItems;
        }
      }
      return cachedItems.Items;
    }

    private List<StrongBoxItemInfo> GetItemsFromStrongBox(
      IVssRequestContext requestContext,
      string keyName)
    {
      return this.GetItemsFromStrongBox(requestContext, keyName, out Guid _);
    }

    private List<StrongBoxItemInfo> GetItemsFromStrongBox(
      IVssRequestContext requestContext,
      string keyName,
      out Guid drawerId)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      drawerId = service.UnlockDrawer(requestContext, keyName, true);
      List<StrongBoxItemInfo> drawerContents = service.GetDrawerContents(requestContext, drawerId);
      drawerContents?.Sort((IComparer<StrongBoxItemInfo>) new GalleryKeyService.StrongBoxItemComparer());
      return drawerContents;
    }

    private void PublishEvent(IVssRequestContext requestContext, KeyChangeEvent changeEvent)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      IMessageBusPublisherService service = vssRequestContext.GetService<IMessageBusPublisherService>();
      ServiceEvent serviceEvent = new ServiceEvent()
      {
        EventType = "KeyChangeEvent",
        Resource = (object) changeEvent,
        ResourceVersion = GalleryConstants.MessageVersions[0],
        Publisher = new Microsoft.VisualStudio.Services.WebApi.Publisher()
        {
          Name = "Gallery",
          ServiceOwnerId = new Guid("00000029-0000-8888-8000-000000000000")
        }
      };
      try
      {
        service.Publish(vssRequestContext, "Microsoft.VisualStudio.Services.KeyManagement", (object[]) new ServiceEvent[1]
        {
          serviceEvent
        });
      }
      catch
      {
        if (requestContext.ExecutionEnvironment.IsDevFabricDeployment)
          return;
        throw;
      }
    }

    private void VerifyKey(IVssRequestContext requestContext, string keyName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(keyName, nameof (keyName));
      if (!this.s_validKeyNames.Contains(keyName))
        throw new ArgumentException(GalleryResources.InvalidKeyName((object) keyName));
    }

    private class StrongBoxItemComparer : IComparer<StrongBoxItemInfo>
    {
      public int Compare(StrongBoxItemInfo x, StrongBoxItemInfo y)
      {
        int num = int.Parse(x.LookupKey);
        return int.Parse(y.LookupKey) - num;
      }
    }

    private class CachedItems
    {
      public Guid DrawerId { get; set; }

      public DateTime CacheTime { get; set; }

      public List<StrongBoxItemInfo> Items { get; set; }
    }
  }
}
