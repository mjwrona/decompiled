// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.QueryDisplayColumn
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class QueryDisplayColumn
  {
    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "text", EmitDefaultValue = false)]
    public string Text { get; set; }

    [DataMember(Name = "fieldId", EmitDefaultValue = false)]
    public int FieldId { get; set; }

    [DataMember(Name = "canSortBy")]
    public bool CanSortBy { get; set; }

    [DataMember(Name = "width", EmitDefaultValue = true)]
    public int Width { get; set; }

    [DataMember(Name = "isIdentity", EmitDefaultValue = true)]
    public bool IsIdentity { get; set; }

    [DataMember(Name = "fieldType", EmitDefaultValue = true)]
    public InternalFieldType FieldType { get; set; }
  }
}
