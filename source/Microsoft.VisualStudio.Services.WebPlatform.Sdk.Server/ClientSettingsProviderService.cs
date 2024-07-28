// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ClientSettingsProviderService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Settings;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ClientSettingsProviderService : IClientSettingsProviderService, IVssFrameworkService
  {
    private const string c_sharedDataKey = "_settings";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddSettingValues(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      SettingsUserScope userScope,
      string key)
    {
      this.AddSettingValues(requestContext, sharedData, userScope, (string) null, (string) null, key);
    }

    public void AddSettingValues(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      SettingsUserScope userScope,
      string scopeName,
      string scopeValue,
      string key)
    {
      IDictionary<string, object> values = requestContext.GetService<ISettingsService>().GetValues(requestContext, userScope, scopeName, scopeValue, key, false);
      object obj;
      WebSdkMetadataDictionary<string, IDictionary<string, IDictionary<string, object>>> metadataDictionary;
      if (sharedData.TryGetValue("_settings", out obj) && obj is WebSdkMetadataDictionary<string, IDictionary<string, IDictionary<string, object>>>)
      {
        metadataDictionary = obj as WebSdkMetadataDictionary<string, IDictionary<string, IDictionary<string, object>>>;
      }
      else
      {
        metadataDictionary = new WebSdkMetadataDictionary<string, IDictionary<string, IDictionary<string, object>>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        sharedData.Add("_settings", (object) metadataDictionary);
      }
      string key1 = userScope.ToString();
      if (!string.IsNullOrEmpty(scopeName))
        key1 += string.Format(";{0};{1}", (object) scopeName, (object) scopeValue);
      IDictionary<string, IDictionary<string, object>> dictionary;
      if (!metadataDictionary.TryGetValue(key1, out dictionary))
      {
        dictionary = (IDictionary<string, IDictionary<string, object>>) new Dictionary<string, IDictionary<string, object>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        metadataDictionary.Add(key1, dictionary);
      }
      dictionary[key] = values;
    }
  }
}
