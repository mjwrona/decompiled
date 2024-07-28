// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels.AddPanelViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels
{
  [DataContract]
  public class AddPanelViewModel
  {
    [DataMember(Name = "projectId", EmitDefaultValue = true)]
    public string ProjectId { get; set; }

    [DataMember(Name = "teamId", EmitDefaultValue = true)]
    public Guid TeamId { get; set; }

    [DataMember(Name = "workItemTypes", EmitDefaultValue = true)]
    public string[] WorkItemTypes { get; set; }

    [DataMember(Name = "defaultWorkItemType", EmitDefaultValue = true)]
    public string DefaultWorkItemType { get; set; }

    [DataMember(Name = "fieldRefNames", EmitDefaultValue = true)]
    public string[] FieldRefNames { get; set; }
  }
}
