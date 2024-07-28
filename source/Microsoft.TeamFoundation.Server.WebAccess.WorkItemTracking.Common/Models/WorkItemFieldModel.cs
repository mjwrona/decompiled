// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemFieldModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class WorkItemFieldModel : BaseSecuredObjectModel
  {
    public WorkItemFieldModel(FieldDefinition field, ISecuredObject securedObject)
      : base(securedObject)
    {
      this.Id = field.Id;
      this.Name = field.Name;
      this.ReferenceName = field.ReferenceName;
      this.Type = (int) field.FieldType;
      this.Flags = (int) field.Flags;
      this.Usages = (int) field.Usages;
      this.IsIdentity = field.IsIdentity;
      this.IsHistoryEnabled = field.IsHistoryEnabled;
    }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string ReferenceName { get; set; }

    [DataMember]
    public int Type { get; set; }

    [DataMember]
    public int Flags { get; set; }

    [DataMember]
    public int Usages { get; set; }

    [DataMember]
    public bool IsIdentity { get; set; }

    [DataMember]
    public bool IsHistoryEnabled { get; set; }
  }
}
