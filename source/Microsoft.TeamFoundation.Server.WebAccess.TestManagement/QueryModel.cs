// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class QueryModel
  {
    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "itemType", EmitDefaultValue = false)]
    public string ItemType { get; set; }

    [DataMember(Name = "isFolder", EmitDefaultValue = false)]
    public bool IsFolder { get; set; }

    [DataMember(Name = "parentId", EmitDefaultValue = false)]
    public string ParentId { get; set; }

    [DataMember(Name = "filter", EmitDefaultValue = false)]
    public FilterModel Filter { get; set; }

    [DataMember(Name = "columns", EmitDefaultValue = false)]
    public QueryColumnInfoModel Columns { get; set; }
  }
}
