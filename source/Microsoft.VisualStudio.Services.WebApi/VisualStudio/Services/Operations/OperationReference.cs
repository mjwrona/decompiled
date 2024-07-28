// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Operations.OperationReference
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Operations
{
  [DataContract]
  public class OperationReference
  {
    [DataMember(Order = 0, EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(Order = 1)]
    public OperationStatus Status { get; set; }

    [DataMember(Order = 2, EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(Order = 3, EmitDefaultValue = false)]
    public Guid PluginId { get; set; }

    public override string ToString() => this.Url;
  }
}
