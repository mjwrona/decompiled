// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.IconProps
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  [DataContract]
  public class IconProps : WebSdkMetadata
  {
    public const int DefaultIconType = 0;
    public const int ImageIconType = 1;

    public IconProps() => this.IconName = string.Empty;

    [DataMember(Name = "ariaLabel", EmitDefaultValue = false)]
    public string AriaLabel { get; set; }

    [DataMember(Name = "className", EmitDefaultValue = false)]
    public string ClassName { get; set; }

    [DataMember(Name = "iconName")]
    public string IconName { get; set; }

    [DataMember(Name = "iconType", EmitDefaultValue = false)]
    public int IconType { get; set; }
  }
}
