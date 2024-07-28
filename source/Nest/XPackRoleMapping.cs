// Decompiled with JetBrains decompiler
// Type: Nest.XPackRoleMapping
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class XPackRoleMapping
  {
    [DataMember(Name = "enabled")]
    public bool Enabled { get; private set; }

    [DataMember(Name = "metadata")]
    public IDictionary<string, object> Metadata { get; private set; }

    [DataMember(Name = "roles")]
    public IReadOnlyCollection<string> Roles { get; private set; } = EmptyReadOnly<string>.Collection;

    [DataMember(Name = "rules")]
    public RoleMappingRuleBase Rules { get; private set; }
  }
}
