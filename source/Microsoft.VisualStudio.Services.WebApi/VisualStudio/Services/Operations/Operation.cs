// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Operations.Operation
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Operations
{
  [DataContract]
  public class Operation : OperationReference
  {
    public Operation()
    {
    }

    public Operation(OperationReference operationReference)
    {
      this.Id = operationReference.Id;
      this.PluginId = operationReference.PluginId;
      this.Status = operationReference.Status;
      this.Url = operationReference.Url;
      this.Links = new ReferenceLinks();
      this.Links.AddLink("self", this.Url);
    }

    [DataMember(Name = "_links", Order = 6, EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(Order = 7, EmitDefaultValue = false)]
    public string ResultMessage { get; set; }

    [DataMember(Order = 8, EmitDefaultValue = false)]
    public string DetailedMessage { get; set; }

    [DataMember(Order = 9, EmitDefaultValue = false)]
    public OperationResultReference ResultUrl { get; set; }

    public bool Completed => this.Status == OperationStatus.Succeeded || this.Status == OperationStatus.Failed || this.Status == OperationStatus.Cancelled;

    public override string ToString() => this.Status.ToString();
  }
}
