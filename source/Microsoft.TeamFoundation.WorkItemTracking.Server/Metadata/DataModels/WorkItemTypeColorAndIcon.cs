// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.WorkItemTypeColorAndIcon
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels
{
  [DataContract]
  public class WorkItemTypeColorAndIcon : ProcessReadSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public string WorkItemTypeName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Color { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Icon { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDisabled { get; set; }
  }
}
