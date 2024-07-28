// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentCloudType
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.FormInput;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class TaskAgentCloudType
  {
    [DataMember(EmitDefaultValue = false, Name = "InputDescriptors")]
    private List<InputDescriptor> m_inputDescriptors;

    [DataMember]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    public List<InputDescriptor> InputDescriptors
    {
      get => this.m_inputDescriptors ?? (this.m_inputDescriptors = new List<InputDescriptor>());
      set => this.m_inputDescriptors = value;
    }
  }
}
