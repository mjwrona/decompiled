// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings.DuplicateFlowWorkFieldValue
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings
{
  [DataContract]
  public class DuplicateFlowWorkFieldValue
  {
    [DataMember(Name = "referenceName", EmitDefaultValue = false)]
    public string ReferenceName { get; set; }

    [DataMember(Name = "value", EmitDefaultValue = false)]
    public string Value { get; set; }
  }
}
