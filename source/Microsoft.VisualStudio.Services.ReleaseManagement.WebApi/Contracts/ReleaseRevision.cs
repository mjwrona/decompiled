// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseRevision
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [DataContract]
  public class ReleaseRevision : ReleaseManagementSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public int ReleaseId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int DefinitionSnapshotRevision { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef ChangedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime ChangedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ChangeType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ChangeDetails { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Comment { get; set; }

    public int FileId { get; set; }
  }
}
