// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.ResourceProviderMessage
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class ResourceProviderMessage
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? AcknowledgedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AcknowledgedState { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? CompletedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string CompletionResult { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TimeSpan CompletionWaitTime { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? CreatedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ErrorDetails { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ErrorMessage { get; set; }

    [DataMember(IsRequired = true)]
    public Guid MessageId { get; set; }

    [DataMember(IsRequired = true)]
    public string MessageSource { get; set; }
  }
}
