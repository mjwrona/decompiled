// Decompiled with JetBrains decompiler
// Type: Nest.PutRoleMappingResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class PutRoleMappingResponse : ResponseBase
  {
    public bool Created
    {
      get
      {
        PutRoleMappingStatus roleMapping = this.RoleMapping;
        return roleMapping != null && roleMapping.Created;
      }
    }

    [DataMember(Name = "role_mapping")]
    public PutRoleMappingStatus RoleMapping { get; internal set; }
  }
}
