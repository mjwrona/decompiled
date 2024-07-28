// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.IAccountCache
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  public interface IAccountCache
  {
    IEnumerable<IAccountCacheItem> GetItems();

    Task<IEnumerable<IAccountCacheItem>> GetItemsAsync();

    Task<IAccountCacheItem> AcquireTokenSilentAsync(
      string[] scopes,
      string userIdentifier,
      string tenantId = null);

    Task DeleteItemAsync(IAccountCacheItem token);

    Task<IAccountCacheItem> AcquireTokenInteractiveAsync(
      string[] scopes,
      Prompt prompt = default (Prompt),
      string userIdentifier = null,
      string tenantId = null);

    Task<string> GetAnyUserIdentifierAsync();

    IEnumerable<IAccountCacheItem> GetVsoEndpointToken(IAccountCacheItem tokenCacheItem);

    Task<IEnumerable<IAccountCacheItem>> GetVsoEndpointTokenAsync(IAccountCacheItem tokenCacheItem);
  }
}
