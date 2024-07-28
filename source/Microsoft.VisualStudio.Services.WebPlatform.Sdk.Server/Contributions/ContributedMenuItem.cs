// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContributedMenuItem
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  [DataContract]
  public class ContributedMenuItem : WebSdkMetadata
  {
    [DataMember(Name = "componentType", EmitDefaultValue = false)]
    public string ComponentType { get; set; }

    [DataMember(Name = "itemType", EmitDefaultValue = false)]
    public int ItemType { get; set; }

    [DataMember(Name = "componentProperties", EmitDefaultValue = false)]
    public object ComponentProperties { get; set; }

    [DataMember(Name = "key")]
    public string Key { get; set; }

    [DataMember(Name = "href", EmitDefaultValue = false)]
    public string Href { get; set; }

    [DataMember(Name = "iconProps", EmitDefaultValue = false)]
    public IconProps IconProps { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "disabled", EmitDefaultValue = false)]
    public bool Disabled { get; set; }

    [DataMember(Name = "title", EmitDefaultValue = false)]
    public string Title { get; set; }

    [DataMember(Name = "commandId", EmitDefaultValue = false)]
    public string CommandId { get; set; }

    [DataMember(Name = "groupKey", EmitDefaultValue = false)]
    public string GroupKey { get; set; }

    [DataMember(Name = "command", EmitDefaultValue = false)]
    public ContributedCommand Command { get; set; }

    [DataMember(Name = "rank", EmitDefaultValue = false)]
    public int Rank { get; set; }

    [DataMember(Name = "items", EmitDefaultValue = false)]
    public IEnumerable<ContributedMenuItem> Items { get; set; }

    [DataMember(Name = "subMenuId", EmitDefaultValue = false)]
    public string MenuId { get; set; }
  }
}
