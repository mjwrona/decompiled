// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.TaskAgentPoolReference
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class TaskAgentPoolReference : BaseSecuredObject
  {
    public TaskAgentPoolReference()
    {
    }

    public TaskAgentPoolReference(int id)
      : this(id, (ISecuredObject) null)
    {
    }

    internal TaskAgentPoolReference(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    internal TaskAgentPoolReference(int id, ISecuredObject securedObject)
      : base(securedObject)
    {
      this.Id = id;
    }

    [DataMember(EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsHosted { get; set; }
  }
}
