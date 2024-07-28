// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ClientMenuItemProviderService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ClientMenuItemProviderService : IClientMenuItemProviderService, IVssFrameworkService
  {
    private const string c_sharedDataKey = "_menuItems";
    private const string c_area = "ClientRouteProviderService";
    private const string c_layer = "IClientMenuItemProviderService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddMenuItems(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      string targetMenuId,
      IEnumerable<ContributedMenuItem> menuItems)
    {
      object obj;
      Dictionary<string, List<ContributedMenuItem>> dictionary;
      if (sharedData.TryGetValue("_menuItems", out obj) && obj is WebSdkMetadataDictionary<string, List<ContributedMenuItem>>)
      {
        dictionary = (Dictionary<string, List<ContributedMenuItem>>) (obj as WebSdkMetadataDictionary<string, List<ContributedMenuItem>>);
      }
      else
      {
        dictionary = (Dictionary<string, List<ContributedMenuItem>>) new WebSdkMetadataDictionary<string, List<ContributedMenuItem>>();
        sharedData.Add("_menuItems", (object) dictionary);
      }
      List<ContributedMenuItem> contributedMenuItemList;
      if (!dictionary.TryGetValue(targetMenuId, out contributedMenuItemList))
      {
        contributedMenuItemList = new List<ContributedMenuItem>();
        dictionary.Add(targetMenuId, contributedMenuItemList);
      }
      if (menuItems == null)
        return;
      contributedMenuItemList.AddRange(menuItems);
    }
  }
}
