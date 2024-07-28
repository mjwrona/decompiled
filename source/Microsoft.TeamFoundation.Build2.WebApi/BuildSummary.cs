// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.BuildSummary
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class BuildSummary
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public XamlBuildReference Build { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildReason Reason { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public BuildStatus Status { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Quality { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime StartTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime FinishTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool KeepForever { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef RequestedFor { get; set; }
  }
}
