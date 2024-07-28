// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ClientTheme
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  [DataContract]
  public class ClientTheme : WebSdkMetadata
  {
    [DataMember(Name = "id")]
    public string Id;
    [DataMember(Name = "name")]
    public string Name;
    [DataMember(Name = "extends")]
    public string Extends;
    [DataMember(Name = "data")]
    public WebSdkMetadataDictionary<string, string> Data;
    [DataMember(Name = "isDark", EmitDefaultValue = false)]
    public bool IsDark;
    [DataMember(Name = "isPreview", EmitDefaultValue = false)]
    public bool IsPreview;
  }
}
