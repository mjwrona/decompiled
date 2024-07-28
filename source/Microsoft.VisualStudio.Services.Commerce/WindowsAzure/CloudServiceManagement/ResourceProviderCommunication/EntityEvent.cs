// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication.EntityEvent
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication
{
  [DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Cis.DevExp.Services.Rdfe.ServiceManagement")]
  [ExcludeFromCodeCoverage]
  public class EntityEvent
  {
    [DataMember(EmitDefaultValue = false, IsRequired = true, Order = 0)]
    public string EventId { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 1)]
    public string ListenerId { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 2)]
    public string EntityType { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 3)]
    public EntityState EntityState { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 4)]
    public EntityId EntityId { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 5)]
    public string OperationId { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 6)]
    public bool IsAsync { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 7)]
    public EntityProperty[] Properties { get; set; }

    public EntityEvent CreateCopy(string listenerId)
    {
      EntityEvent copy = this.MemberwiseClone() as EntityEvent;
      copy.ListenerId = listenerId;
      return copy;
    }
  }
}
