// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.SharedPermissionCollection
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  [DataContract]
  public class SharedPermissionCollection : WebSdkMetadataDictionary<Guid, Dictionary<string, int>>
  {
    public void AddPermissions(Guid namespaceId, string token, int permissions)
    {
      if (!this.ContainsKey(namespaceId))
        this.Add(namespaceId, new Dictionary<string, int>());
      Dictionary<string, int> dictionary = this[namespaceId];
      if (dictionary.ContainsKey(token))
        return;
      dictionary.Add(token, permissions);
    }
  }
}
