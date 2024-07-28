// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings.DuplicateFlowWorkItemType
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings
{
  [DataContract]
  public class DuplicateFlowWorkItemType
  {
    [DataMember(Name = "workItemTypeName", EmitDefaultValue = false)]
    public string WorkItemTypeName { get; set; }

    [DataMember(Name = "resolveAsPrimary", EmitDefaultValue = false)]
    public DuplicateFlowWorkFieldValue[] ResolveAsPrimary { get; set; }

    [DataMember(Name = "resolveAsDuplicate", EmitDefaultValue = false)]
    public DuplicateFlowWorkFieldValue[] ResolveAsDuplicate { get; set; }

    [DataMember(Name = "unlink", EmitDefaultValue = false)]
    public DuplicateFlowWorkFieldValue[] Unlink { get; set; }
  }
}
