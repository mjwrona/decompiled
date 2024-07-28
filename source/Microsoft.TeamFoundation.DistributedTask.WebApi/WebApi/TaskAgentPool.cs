// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPool
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgentPool : TaskAgentPoolReference
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Properties")]
    private PropertiesCollection m_properties;

    internal TaskAgentPool()
    {
    }

    public TaskAgentPool(string name) => this.Name = name;

    private TaskAgentPool(TaskAgentPool poolToBeCloned)
    {
      this.AutoProvision = poolToBeCloned.AutoProvision;
      IdentityRef createdBy = poolToBeCloned.CreatedBy;
      this.CreatedBy = createdBy != null ? createdBy.Clone() : (IdentityRef) null;
      this.CreatedOn = poolToBeCloned.CreatedOn;
      this.Id = poolToBeCloned.Id;
      this.IsHosted = poolToBeCloned.IsHosted;
      this.Name = poolToBeCloned.Name;
      this.Scope = poolToBeCloned.Scope;
      this.Size = poolToBeCloned.Size;
      this.PoolType = poolToBeCloned.PoolType;
      IdentityRef owner = poolToBeCloned.Owner;
      this.Owner = owner != null ? owner.Clone() : (IdentityRef) null;
      this.AgentCloudId = poolToBeCloned.AgentCloudId;
      this.TargetSize = poolToBeCloned.TargetSize;
      this.IsLegacy = poolToBeCloned.IsLegacy;
      this.AutoUpdate = poolToBeCloned.AutoUpdate;
      this.Options = poolToBeCloned.Options;
      IdentityRef administratorsGroup = poolToBeCloned.AdministratorsGroup;
      this.AdministratorsGroup = administratorsGroup != null ? administratorsGroup.Clone() : (IdentityRef) null;
      this.GroupScopeId = poolToBeCloned.GroupScopeId;
      this.Provisioned = poolToBeCloned.Provisioned;
      IdentityRef serviceAccountsGroup = poolToBeCloned.ServiceAccountsGroup;
      this.ServiceAccountsGroup = serviceAccountsGroup != null ? serviceAccountsGroup.Clone() : (IdentityRef) null;
      if (poolToBeCloned.m_properties == null)
        return;
      this.m_properties = new PropertiesCollection((IDictionary<string, object>) poolToBeCloned.m_properties);
    }

    [DataMember]
    public DateTime CreatedOn { get; internal set; }

    [DataMember]
    public bool? AutoProvision { get; set; }

    [DataMember]
    public bool? AutoUpdate { get; set; }

    [DataMember]
    public bool? AutoSize { get; set; }

    [DataMember]
    public int? TargetSize { get; set; }

    [DataMember]
    public int? AgentCloudId { get; set; }

    [DataMember]
    public IdentityRef CreatedBy { get; set; }

    [DataMember]
    public IdentityRef Owner { get; set; }

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

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This property is no longer used and will be removed in a future version.", false)]
    public Guid GroupScopeId { get; internal set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This property is no longer used and will be removed in a future version.", false)]
    public bool Provisioned { get; internal set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This property is no longer used and will be removed in a future version.", false)]
    public IdentityRef AdministratorsGroup { get; internal set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This property is no longer used and will be removed in a future version.", false)]
    public IdentityRef ServiceAccountsGroup { get; internal set; }

    public TaskAgentPool Clone() => new TaskAgentPool(this);
  }
}
