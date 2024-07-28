// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Navigation.HeaderAction
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Navigation
{
  [DataContract]
  public class HeaderAction : WebSdkMetadata
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public bool TargetSelf { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Text { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Title { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Icon { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string FabricIconName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string CommandId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string GroupKey { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public HeaderCommand Command { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int ItemType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Rank { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<HeaderAction> Items { get; set; }

    [DataMember(Name = "subMenuId", EmitDefaultValue = false)]
    public string MenuId { get; set; }

    public string ComponentType { get; set; }

    public object ComponentProperties { get; set; }
  }
}
