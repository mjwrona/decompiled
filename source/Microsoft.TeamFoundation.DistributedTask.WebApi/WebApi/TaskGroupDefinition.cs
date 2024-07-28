// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskGroupDefinition
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskGroupDefinition
  {
    [DataMember(Name = "Tags", EmitDefaultValue = false)]
    private List<string> m_serializedTags;
    private List<string> m_tags;

    public TaskGroupDefinition() => this.IsExpanded = false;

    private TaskGroupDefinition(TaskGroupDefinition inputDefinitionToClone)
    {
      this.IsExpanded = inputDefinitionToClone.IsExpanded;
      this.Name = inputDefinitionToClone.Name;
      this.DisplayName = inputDefinitionToClone.DisplayName;
      this.VisibleRule = inputDefinitionToClone.VisibleRule;
      if (inputDefinitionToClone.m_tags == null)
        return;
      this.m_tags = new List<string>((IEnumerable<string>) inputDefinitionToClone.m_tags);
    }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool IsExpanded { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string VisibleRule { get; set; }

    public IList<string> Tags
    {
      get
      {
        if (this.m_tags == null)
          this.m_tags = new List<string>();
        return (IList<string>) this.m_tags;
      }
    }

    public TaskGroupDefinition Clone() => new TaskGroupDefinition(this);

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<string>(ref this.m_serializedTags, ref this.m_tags, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<string>(ref this.m_tags, ref this.m_serializedTags);

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedTags = (List<string>) null;
  }
}
