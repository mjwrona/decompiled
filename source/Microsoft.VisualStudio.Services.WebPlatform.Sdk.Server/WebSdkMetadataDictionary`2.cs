// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.WebSdkMetadataDictionary`2
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class WebSdkMetadataDictionary<TKey, TValue> : 
    Dictionary<TKey, TValue>,
    IWebSdkMetadata,
    ISecuredObject
  {
    public WebSdkMetadataDictionary()
    {
    }

    public WebSdkMetadataDictionary(IEqualityComparer<TKey> comparer)
      : base(comparer)
    {
    }

    [IgnoreDataMember]
    public Guid NamespaceId => WebPlatformSecurityConstants.NamespaceId;

    [IgnoreDataMember]
    public int RequiredPermissions => 1;

    public string GetToken() => "/WebPlatformMetadata";
  }
}
