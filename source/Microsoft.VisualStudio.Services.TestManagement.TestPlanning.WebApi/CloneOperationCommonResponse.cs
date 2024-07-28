// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.CloneOperationCommonResponse
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  [DataContract]
  public class CloneOperationCommonResponse
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CloneStatistics cloneStatistics { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime completionDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime creationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string message { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int opId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public CloneOperationState state { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ReferenceLinks links { get; set; }
  }
}
