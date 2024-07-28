// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Common.Contracts.TaskSourceDefinitionBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Common.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F0CB8220-D93B-49F7-B603-A5F8DA1FAAC3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Common.Contracts.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Common.Contracts
{
  [DataContract]
  public class TaskSourceDefinitionBase : BaseSecuredObject
  {
    public TaskSourceDefinitionBase()
    {
      this.AuthKey = string.Empty;
      this.Endpoint = string.Empty;
      this.Selector = string.Empty;
      this.Target = string.Empty;
      this.KeySelector = string.Empty;
    }

    protected TaskSourceDefinitionBase(TaskSourceDefinitionBase inputDefinitionToClone)
      : this(inputDefinitionToClone, (ISecuredObject) null)
    {
    }

    protected TaskSourceDefinitionBase(
      TaskSourceDefinitionBase inputDefinitionToClone,
      ISecuredObject securedObject)
      : base(securedObject)
    {
      this.Endpoint = inputDefinitionToClone.Endpoint;
      this.Target = inputDefinitionToClone.Target;
      this.AuthKey = inputDefinitionToClone.AuthKey;
      this.Selector = inputDefinitionToClone.Selector;
      this.KeySelector = inputDefinitionToClone.KeySelector;
    }

    public virtual TaskSourceDefinitionBase Clone(ISecuredObject securedObject) => new TaskSourceDefinitionBase(this, securedObject);

    [DataMember(EmitDefaultValue = false)]
    public string Endpoint { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Target { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string AuthKey { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Selector { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string KeySelector { get; set; }
  }
}
