// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.TaskReference
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class TaskReference : BaseSecuredObject
  {
    public TaskReference()
    {
    }

    public TaskReference(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    protected TaskReference(TaskReference taskToBeCloned)
      : base((ISecuredObject) taskToBeCloned)
    {
      this.Id = taskToBeCloned.Id;
      this.Name = taskToBeCloned.Name;
      this.Version = taskToBeCloned.Version;
    }

    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Version { get; set; }

    public virtual TaskReference Clone() => new TaskReference(this);
  }
}
