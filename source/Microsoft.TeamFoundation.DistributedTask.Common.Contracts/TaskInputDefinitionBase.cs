// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Common.Contracts.TaskInputDefinitionBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Common.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F0CB8220-D93B-49F7-B603-A5F8DA1FAAC3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.Common.Contracts.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Common.Contracts
{
  [DataContract]
  public class TaskInputDefinitionBase : BaseSecuredObject
  {
    [DataMember(Name = "Aliases", EmitDefaultValue = false)]
    private List<string> m_aliases;
    [DataMember(Name = "Options", EmitDefaultValue = false)]
    private Dictionary<string, string> m_options;
    [DataMember(Name = "Properties", EmitDefaultValue = false)]
    private Dictionary<string, string> m_properties;

    public TaskInputDefinitionBase()
    {
      this.InputType = "string";
      this.DefaultValue = string.Empty;
      this.Required = false;
      this.HelpMarkDown = string.Empty;
    }

    protected TaskInputDefinitionBase(TaskInputDefinitionBase inputDefinitionToClone)
      : this(inputDefinitionToClone, (ISecuredObject) null)
    {
    }

    protected TaskInputDefinitionBase(
      TaskInputDefinitionBase inputDefinitionToClone,
      ISecuredObject securedObject)
      : base(securedObject)
    {
      this.DefaultValue = inputDefinitionToClone.DefaultValue;
      this.InputType = inputDefinitionToClone.InputType;
      this.Label = inputDefinitionToClone.Label;
      this.Name = inputDefinitionToClone.Name;
      this.Required = inputDefinitionToClone.Required;
      this.HelpMarkDown = inputDefinitionToClone.HelpMarkDown;
      this.VisibleRule = inputDefinitionToClone.VisibleRule;
      this.GroupName = inputDefinitionToClone.GroupName;
      if (inputDefinitionToClone.Validation != null)
        this.Validation = inputDefinitionToClone.Validation.Clone(securedObject);
      if (inputDefinitionToClone.m_aliases != null)
        this.m_aliases = new List<string>((IEnumerable<string>) inputDefinitionToClone.m_aliases);
      if (inputDefinitionToClone.m_options != null)
        this.m_options = new Dictionary<string, string>((IDictionary<string, string>) inputDefinitionToClone.m_options);
      if (inputDefinitionToClone.m_properties == null)
        return;
      this.m_properties = new Dictionary<string, string>((IDictionary<string, string>) inputDefinitionToClone.m_properties);
    }

    public IList<string> Aliases
    {
      get
      {
        if (this.m_aliases == null)
          this.m_aliases = new List<string>();
        return (IList<string>) this.m_aliases;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Label { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DefaultValue { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool Required { get; set; }

    [DataMember(Name = "Type")]
    public string InputType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string HelpMarkDown { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string VisibleRule { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string GroupName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskInputValidation Validation { get; set; }

    public Dictionary<string, string> Options
    {
      get
      {
        if (this.m_options == null)
          this.m_options = new Dictionary<string, string>();
        return this.m_options;
      }
    }

    public Dictionary<string, string> Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new Dictionary<string, string>();
        return this.m_properties;
      }
      set => this.m_properties = value;
    }

    public virtual TaskInputDefinitionBase Clone(ISecuredObject securedObject) => new TaskInputDefinitionBase(this, securedObject);

    public override int GetHashCode() => this.Name.GetHashCode() ^ this.DefaultValue.GetHashCode() ^ this.Label.GetHashCode();

    public override bool Equals(object obj) => obj is TaskInputDefinitionBase inputDefinitionBase && string.Equals(this.InputType, inputDefinitionBase.InputType, StringComparison.OrdinalIgnoreCase) && string.Equals(this.Label, inputDefinitionBase.Label, StringComparison.OrdinalIgnoreCase) && string.Equals(this.Name, inputDefinitionBase.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(this.GroupName, inputDefinitionBase.GroupName, StringComparison.OrdinalIgnoreCase) && string.Equals(this.DefaultValue, inputDefinitionBase.DefaultValue, StringComparison.OrdinalIgnoreCase) && string.Equals(this.HelpMarkDown, inputDefinitionBase.HelpMarkDown, StringComparison.OrdinalIgnoreCase) && string.Equals(this.VisibleRule, inputDefinitionBase.VisibleRule, StringComparison.OrdinalIgnoreCase) && this.Required.Equals(inputDefinitionBase.Required) && this.AreListsEqual(this.Aliases, inputDefinitionBase.Aliases) && this.AreDictionariesEqual(this.Properties, inputDefinitionBase.Properties) && this.AreDictionariesEqual(this.Options, inputDefinitionBase.Options) && (this.Validation == null || inputDefinitionBase.Validation != null) && (this.Validation != null || inputDefinitionBase.Validation == null) && (this.Validation == null || inputDefinitionBase.Validation == null || this.Validation.Equals((object) inputDefinitionBase.Validation));

    private bool AreDictionariesEqual(
      Dictionary<string, string> input1,
      Dictionary<string, string> input2)
    {
      if (input1 == null && input2 == null)
        return true;
      if (input1 == null && input2 != null || input1 != null && input2 == null || input1.Count != input2.Count)
        return false;
      foreach (string key in input1.Keys)
      {
        if (!input2.ContainsKey(key) || !string.Equals(input1[key], input2[key], StringComparison.OrdinalIgnoreCase))
          return false;
      }
      return true;
    }

    private bool AreListsEqual(IList<string> list1, IList<string> list2)
    {
      if (list1.Count != list2.Count)
        return false;
      for (int index = 0; index < list1.Count; ++index)
      {
        if (!string.Equals(list1[index], list2[index], StringComparison.OrdinalIgnoreCase))
          return false;
      }
      return true;
    }
  }
}
