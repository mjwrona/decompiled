// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineInstance
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachineInstance
  {
    private PropertiesCollection m_properties;
    private string m_hubName;

    internal MachineInstance()
    {
    }

    public MachineInstance(string instanceName) => this.InstanceName = instanceName;

    [IgnoreDataMember]
    internal int PoolId { get; set; }

    [IgnoreDataMember]
    internal int Id { get; set; }

    [DataMember]
    public string InstanceName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ImageName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ImageVersion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public byte[] AccessToken { get; set; }

    [DataMember]
    public string QueueName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool Connected { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool Enabled { get; set; }

    [DataMember]
    public string State { get; set; }

    [DataMember]
    public DateTime CreatedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? ConnectedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? DisconnectedOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? RegisteredOn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public long? RequestId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? StorageAccountId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public long? FactoryDiskId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public long? OSDiskId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string HubName
    {
      get => string.IsNullOrEmpty(this.m_hubName) ? this.InstanceName : this.m_hubName;
      set => this.m_hubName = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DispatcherType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public short CurrentParallelism { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public short MaxParallelism { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public PropertiesCollection Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new PropertiesCollection();
        return this.m_properties;
      }
      internal set => this.m_properties = value;
    }

    [IgnoreDataMember]
    internal string OperationName { get; set; }

    [DataMember]
    internal string OperationId { get; set; }

    [IgnoreDataMember]
    internal DateTime? OperationQueuedOn { get; set; }

    public static int GetMachineInstanceId(string instanceName)
    {
      int num = instanceName.LastIndexOf('-');
      int result;
      int.TryParse(instanceName.Substring(num + 1), out result);
      return result;
    }
  }
}
