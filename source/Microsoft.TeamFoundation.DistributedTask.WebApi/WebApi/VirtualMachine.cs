// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.VirtualMachine
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class VirtualMachine
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Name = "Tags")]
    private IList<string> tags;

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public TaskAgent Agent { get; set; }

    public IList<string> Tags
    {
      get
      {
        if (this.tags == null)
          this.tags = (IList<string>) new List<string>();
        return this.tags;
      }
      set => this.tags = value;
    }
  }
}
