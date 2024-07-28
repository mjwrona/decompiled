// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePool
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  [DataContract]
  public sealed class MachinePool : ICloneable
  {
    private PropertiesCollection m_properties;

    internal MachinePool()
    {
    }

    public MachinePool(string poolName) => this.PoolName = poolName;

    private MachinePool(MachinePool cloneFrom)
    {
      this.Enabled = cloneFrom.Enabled;
      this.Id = cloneFrom.Id;
      this.ImageName = cloneFrom.ImageName;
      this.ImageVersion = cloneFrom.ImageVersion;
      this.MachineCount = cloneFrom.MachineCount;
      this.MaxMachineCount = cloneFrom.MaxMachineCount;
      this.PerformanceTier = cloneFrom.PerformanceTier;
      this.PoolName = cloneFrom.PoolName;
      this.Region = cloneFrom.Region;
      this.RequestTimeout = cloneFrom.RequestTimeout;
      this.ResourceGroupIdentifier = cloneFrom.ResourceGroupIdentifier;
      this.ResourceProviderType = cloneFrom.ResourceProviderType;
      this.ResourceGroupData = cloneFrom.ResourceGroupData;
      this.RunPoolOrchestrationId = cloneFrom.RunPoolOrchestrationId;
      this.ServiceIdentityId = cloneFrom.ServiceIdentityId;
      this.State = cloneFrom.State;
      this.UseNestedVirtualization = cloneFrom.UseNestedVirtualization;
      this.DefaultInstanceParallelism = cloneFrom.DefaultInstanceParallelism;
      if (cloneFrom.m_properties == null)
        return;
      this.m_properties = new PropertiesCollection((IDictionary<string, object>) cloneFrom.m_properties);
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string PoolType { get; set; }

    [DataMember(IsRequired = true)]
    public string PoolName { get; set; }

    [DataMember(IsRequired = true)]
    public int MachineCount { get; set; }

    [DataMember(IsRequired = false)]
    public int MaxMachineCount { get; set; }

    [DataMember(IsRequired = true)]
    public string ImageName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ImageVersion { get; set; }

    [DataMember(IsRequired = false)]
    public string State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TimeSpan RequestTimeout { get; set; }

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

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string OperationName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string OperationId { get; set; }

    [IgnoreDataMember]
    internal int Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ResourceGroupIdentifier { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ResourceProviderType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ResourceGroupData { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Region { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string PerformanceTier { get; set; }

    [IgnoreDataMember]
    public Guid? ServiceIdentityId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool Enabled { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string RunPoolOrchestrationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool UseNestedVirtualization { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public short DefaultInstanceParallelism { get; set; }

    public MachinePool Clone() => new MachinePool(this);

    object ICloneable.Clone() => (object) this.Clone();
  }
}
