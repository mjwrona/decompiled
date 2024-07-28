// Decompiled with JetBrains decompiler
// Type: Nest.XPackUser
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class XPackUser
  {
    [DataMember(Name = "email")]
    public string Email { get; internal set; }

    [DataMember(Name = "full_name")]
    public string FullName { get; internal set; }

    [DataMember(Name = "metadata")]
    public IReadOnlyDictionary<string, object> Metadata { get; internal set; } = EmptyReadOnly<string, object>.Dictionary;

    [DataMember(Name = "roles")]
    public IReadOnlyCollection<string> Roles { get; internal set; } = EmptyReadOnly<string>.Collection;

    [DataMember(Name = "username")]
    public string Username { get; internal set; }
  }
}
