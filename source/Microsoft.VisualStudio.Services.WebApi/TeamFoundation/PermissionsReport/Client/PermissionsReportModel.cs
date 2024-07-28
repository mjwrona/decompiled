// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.Client.PermissionsReportModel
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.PermissionsReport.Client
{
  [DataContract]
  public class PermissionsReportModel
  {
    public PermissionsReportModel()
    {
    }

    public PermissionsReportModel(
      string identityDescriptor,
      Guid identityId,
      string displayName,
      string accountName,
      PermissionsReportResource resource,
      IEnumerable<SimplePermissionModel> simplePermissionModels,
      string error)
    {
      this.Descriptor = identityDescriptor;
      this.Id = identityId;
      this.DisplayName = displayName;
      this.AccountName = accountName;
      this.Resource = resource;
      this.SimplePermissionModels = simplePermissionModels;
      this.Error = error;
    }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public string Descriptor { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public Guid Id { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public string AccountName { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public string DisplayName { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public PermissionsReportResource Resource { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true, Name = "Permissions")]
    public IEnumerable<SimplePermissionModel> SimplePermissionModels { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public string Error { get; set; }
  }
}
