// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostMigrationStrongBoxUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HostMigrationStrongBoxUtil
  {
    private const int c_GuidLength = 32;
    private const int c_HostIdStart = 0;
    private const int c_MigrationIdStart = 33;
    private const int c_ContainerUriStart = 66;

    public static string GetLookupKey(Guid hostId, Guid migrationId, string containerUri) => hostId.ToString("N") + "/" + migrationId.ToString("N") + "/" + containerUri;

    public static bool GetMigrationIdFromLookupKey(string lookupKey, out Guid migrationId) => Guid.TryParse(lookupKey.Substring(33, 32), out migrationId);

    public static bool GetHostIdFromLookupKey(string lookupKey, out Guid hostId) => Guid.TryParse(lookupKey.Substring(0, 32), out hostId);

    public static bool GetContainerUriFromLookupKey(string lookupKey, out Uri containerUri) => Uri.TryCreate(lookupKey.Substring(66), UriKind.Absolute, out containerUri);

    public static int AddEntriesToStrongBox(
      IVssRequestContext requestContext,
      string stringContent,
      string entryName,
      string entryCountName,
      string drawerName,
      int chunkSize = 524288)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawer = service.UnlockOrCreateDrawer(requestContext, drawerName);
      List<string> stringList = HostMigrationStrongBoxUtil.ChunkString(stringContent, chunkSize);
      if (stringList.Count > 1)
      {
        service.AddString(requestContext, drawer, entryCountName, stringList.Count.ToString());
        for (int index = 0; index < stringList.Count; ++index)
          service.AddString(requestContext, drawer, HostMigrationStrongBoxUtil.ChunkName(entryName, index), stringList[index]);
      }
      else
        service.AddString(requestContext, drawer, entryName, stringContent);
      return stringList.Count;
    }

    public static string ReadEntriesFromStrongBox(
      IVssRequestContext requestContext,
      string entryName,
      string entryCountName,
      string drawerName)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(requestContext, drawerName, true);
      string str1;
      if (service.GetItemInfo(requestContext, drawerId, entryName, false) == null)
      {
        StrongBoxItemInfo itemInfo1 = service.GetItemInfo(requestContext, drawerId, entryCountName, true);
        int num = int.Parse(service.GetString(requestContext, itemInfo1));
        StringBuilder stringBuilder = new StringBuilder();
        for (int chunkNo = 0; chunkNo < num; ++chunkNo)
        {
          StrongBoxItemInfo itemInfo2 = service.GetItemInfo(requestContext, drawerId, HostMigrationStrongBoxUtil.ChunkName(entryName, chunkNo), true);
          string str2 = service.GetString(requestContext, itemInfo2);
          stringBuilder.Append(str2);
        }
        str1 = stringBuilder.ToString();
      }
      else
      {
        StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, drawerId, entryName, true);
        str1 = service.GetString(requestContext, itemInfo);
      }
      return str1;
    }

    public static void DeleteStrongBoxEntries(
      IVssRequestContext requestContext,
      string entryName,
      string entryCountName)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(requestContext, FrameworkServerConstants.HostMigrationStorageStrongBoxDrawer, true);
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, drawerId, entryCountName, false);
      if (itemInfo != null)
      {
        int num = int.Parse(service.GetString(requestContext, itemInfo));
        for (int chunkNo = 0; chunkNo < num; ++chunkNo)
        {
          string lookupKey = HostMigrationStrongBoxUtil.ChunkName(entryName, chunkNo);
          if (service.GetItemInfo(requestContext, drawerId, lookupKey, false) != null)
            service.DeleteItem(requestContext, drawerId, lookupKey);
        }
        service.DeleteItem(requestContext, drawerId, entryCountName);
      }
      else
      {
        if (service.GetItemInfo(requestContext, drawerId, entryName, false) == null)
          return;
        service.DeleteItem(requestContext, drawerId, entryName);
      }
    }

    internal static List<string> ChunkString(string stringContent, int chunkSize)
    {
      if (string.IsNullOrEmpty(stringContent) || chunkSize < 0)
        throw new ArgumentException("String is empty or null or chunkSize is invalid");
      return Enumerable.Range(0, (stringContent.Length + chunkSize - 1) / chunkSize).Select<int, string>((Func<int, string>) (i => stringContent.Substring(i * chunkSize, Math.Min(stringContent.Length - i * chunkSize, chunkSize)))).ToList<string>();
    }

    internal static string ChunkName(string baseName, int chunkNo)
    {
      if (chunkNo < 0)
        throw new ArgumentException("Invalid chunkNo");
      return baseName + "-" + chunkNo.ToString();
    }
  }
}
