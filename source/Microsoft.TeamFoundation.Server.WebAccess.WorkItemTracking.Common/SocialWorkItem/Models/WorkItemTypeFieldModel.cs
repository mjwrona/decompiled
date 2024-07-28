// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SocialWorkItem.Models.WorkItemTypeFieldModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SocialWorkItem.Models
{
  [DataContract]
  public class WorkItemTypeFieldModel : BaseSecuredObjectModel
  {
    public WorkItemTypeFieldModel(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "referenceName")]
    public string ReferenceName { get; set; }

    [DataMember(Name = "description")]
    public string Description { get; set; }

    [DataMember(Name = "type")]
    public FieldType Type { get; set; }

    [DataMember(Name = "readOnly")]
    public bool ReadOnly { get; set; }

    [DataMember(Name = "isIdentity")]
    public bool IsIdentity { get; set; }

    [DataMember(Name = "helpText")]
    public string HelpText { get; set; }

    [DataMember(Name = "alwaysRequired")]
    public bool AlwaysRequired { get; set; }

    [DataMember(Name = "defaultValue")]
    public string DefaultValue { get; set; }

    [DataMember(Name = "allowedValues")]
    public string[] AllowedValues { get; set; }
  }
}
