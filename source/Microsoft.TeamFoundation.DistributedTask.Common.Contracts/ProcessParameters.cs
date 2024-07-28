// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Common.Contracts.ProcessParameters
// Assembly: Microsoft.TeamFoundation.DistributedTask.Common.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F0CB8220-D93B-49F7-B603-A5F8DA1FAAC3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Common.Contracts.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Common.Contracts
{
  [DataContract]
  public class ProcessParameters : BaseSecuredObject
  {
    [DataMember(Name = "Inputs", EmitDefaultValue = false)]
    private List<TaskInputDefinitionBase> m_serializedInputs;
    [DataMember(Name = "SourceDefinitions", EmitDefaultValue = false)]
    private List<TaskSourceDefinitionBase> m_serializedSourceDefinitions;
    [DataMember(Name = "DataSourceBindings", EmitDefaultValue = false)]
    private List<DataSourceBindingBase> m_serializedDataSourceBindings;
    private List<TaskInputDefinitionBase> m_inputs;
    private List<TaskSourceDefinitionBase> m_sourceDefinitions;
    private List<DataSourceBindingBase> m_dataSourceBindings;

    public ProcessParameters()
      : this((ISecuredObject) null)
    {
    }

    public ProcessParameters(ISecuredObject securedObject)
      : this((ProcessParameters) null, securedObject)
    {
    }

    private ProcessParameters(ProcessParameters toClone, ISecuredObject securedObject)
      : base(securedObject)
    {
      if (toClone == null)
        return;
      if (toClone.Inputs.Count > 0)
        this.Inputs.AddRange<TaskInputDefinitionBase, IList<TaskInputDefinitionBase>>(toClone.Inputs.Select<TaskInputDefinitionBase, TaskInputDefinitionBase>((Func<TaskInputDefinitionBase, TaskInputDefinitionBase>) (i => i.Clone(securedObject))));
      if (toClone.SourceDefinitions.Count > 0)
        this.SourceDefinitions.AddRange<TaskSourceDefinitionBase, IList<TaskSourceDefinitionBase>>(toClone.SourceDefinitions.Select<TaskSourceDefinitionBase, TaskSourceDefinitionBase>((Func<TaskSourceDefinitionBase, TaskSourceDefinitionBase>) (sd => sd.Clone(securedObject))));
      if (toClone.DataSourceBindings.Count <= 0)
        return;
      this.DataSourceBindings.AddRange<DataSourceBindingBase, IList<DataSourceBindingBase>>(toClone.DataSourceBindings.Select<DataSourceBindingBase, DataSourceBindingBase>((Func<DataSourceBindingBase, DataSourceBindingBase>) (dsb => dsb.Clone(securedObject))));
    }

    public IList<TaskInputDefinitionBase> Inputs
    {
      get
      {
        if (this.m_inputs == null)
          this.m_inputs = new List<TaskInputDefinitionBase>();
        return (IList<TaskInputDefinitionBase>) this.m_inputs;
      }
    }

    public IList<TaskSourceDefinitionBase> SourceDefinitions
    {
      get
      {
        if (this.m_sourceDefinitions == null)
          this.m_sourceDefinitions = new List<TaskSourceDefinitionBase>();
        return (IList<TaskSourceDefinitionBase>) this.m_sourceDefinitions;
      }
    }

    public IList<DataSourceBindingBase> DataSourceBindings
    {
      get
      {
        if (this.m_dataSourceBindings == null)
          this.m_dataSourceBindings = new List<DataSourceBindingBase>();
        return (IList<DataSourceBindingBase>) this.m_dataSourceBindings;
      }
    }

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object obj)
    {
      if (!(obj is ProcessParameters processParameters))
        return false;
      if (this.Inputs == null && processParameters.Inputs == null)
        return true;
      if (this.Inputs != null && processParameters.Inputs == null || this.Inputs == null && processParameters.Inputs != null || this.Inputs.Count != processParameters.Inputs.Count)
        return false;
      IOrderedEnumerable<TaskInputDefinitionBase> source = this.Inputs.Where<TaskInputDefinitionBase>((Func<TaskInputDefinitionBase, bool>) (i => i != null)).OrderBy<TaskInputDefinitionBase, string>((Func<TaskInputDefinitionBase, string>) (i => i.Name));
      IOrderedEnumerable<TaskInputDefinitionBase> second = processParameters.Inputs.Where<TaskInputDefinitionBase>((Func<TaskInputDefinitionBase, bool>) (i => i != null)).OrderBy<TaskInputDefinitionBase, string>((Func<TaskInputDefinitionBase, string>) (i => i.Name));
      return source.OrderBy<TaskInputDefinitionBase, string>((Func<TaskInputDefinitionBase, string>) (i => i.Name)).SequenceEqual<TaskInputDefinitionBase>((IEnumerable<TaskInputDefinitionBase>) second);
    }

    public ProcessParameters Clone(ISecuredObject securedObject = null) => new ProcessParameters(this, securedObject);

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      SerializationHelper.Copy<TaskInputDefinitionBase>(ref this.m_serializedInputs, ref this.m_inputs, true);
      SerializationHelper.Copy<TaskSourceDefinitionBase>(ref this.m_serializedSourceDefinitions, ref this.m_sourceDefinitions, true);
      SerializationHelper.Copy<DataSourceBindingBase>(ref this.m_serializedDataSourceBindings, ref this.m_dataSourceBindings, true);
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      SerializationHelper.Copy<TaskInputDefinitionBase>(ref this.m_inputs, ref this.m_serializedInputs);
      SerializationHelper.Copy<TaskSourceDefinitionBase>(ref this.m_sourceDefinitions, ref this.m_serializedSourceDefinitions);
      SerializationHelper.Copy<DataSourceBindingBase>(ref this.m_dataSourceBindings, ref this.m_serializedDataSourceBindings);
    }

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context)
    {
      this.m_serializedInputs = (List<TaskInputDefinitionBase>) null;
      this.m_serializedSourceDefinitions = (List<TaskSourceDefinitionBase>) null;
      this.m_serializedDataSourceBindings = (List<DataSourceBindingBase>) null;
    }
  }
}
