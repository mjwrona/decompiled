// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics.Release
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Analytics
{
  public class Release
  {
    public Guid ProjectGuid { get; set; }

    public int ReleaseId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? ReleaseDefinitionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? DefinitionSnapshotRevision { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Reason { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? CreatedByGuid { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? CreatedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? ModifiedByGuid { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? ModifiedOn { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? KeepForever { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? IsDeleted { get; set; }
  }
}
