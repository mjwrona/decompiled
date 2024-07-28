// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VariableGroupReference : ResourceReference, IVariable
  {
    public VariableGroupReference()
    {
    }

    private VariableGroupReference(VariableGroupReference referenceToCopy)
      : base((ResourceReference) referenceToCopy)
    {
      this.Id = referenceToCopy.Id;
      this.GroupType = referenceToCopy.GroupType;
      this.SecretStore = referenceToCopy.SecretStore?.Clone();
    }

    [DataMember(EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string GroupType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public SecretStoreConfiguration SecretStore { get; set; }

    public VariableGroupReference Clone() => new VariableGroupReference(this);

    public override string ToString() => base.ToString() ?? this.Id.ToString();

    [DataMember(Name = "Type")]
    VariableType IVariable.Type => VariableType.Group;
  }
}
