// Decompiled with JetBrains decompiler
// Type: Nest.SecurityUsage
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class SecurityUsage : XPackUsage
  {
    [DataMember(Name = "anonymous")]
    public SecurityUsage.SecurityFeatureToggle Anonymous { get; internal set; }

    [DataMember(Name = "audit")]
    public SecurityUsage.AuditUsage Audit { get; internal set; }

    [DataMember(Name = "ipfilter")]
    public SecurityUsage.IpFilterUsage IpFilter { get; internal set; }

    [DataMember(Name = "realms")]
    public IReadOnlyDictionary<string, SecurityUsage.RealmUsage> Realms { get; internal set; } = EmptyReadOnly<string, SecurityUsage.RealmUsage>.Dictionary;

    [DataMember(Name = "role_mapping")]
    public IReadOnlyDictionary<string, SecurityUsage.RoleMappingUsage> RoleMapping { get; internal set; } = EmptyReadOnly<string, SecurityUsage.RoleMappingUsage>.Dictionary;

    [DataMember(Name = "roles")]
    public IReadOnlyDictionary<string, SecurityUsage.RoleUsage> Roles { get; internal set; } = EmptyReadOnly<string, SecurityUsage.RoleUsage>.Dictionary;

    [DataMember(Name = "ssl")]
    public SecurityUsage.SslUsage Ssl { get; internal set; }

    [DataMember(Name = "system_key")]
    public SecurityUsage.SecurityFeatureToggle SystemKey { get; internal set; }

    public class RoleMappingUsage
    {
      [DataMember(Name = "enabled")]
      public int Enabled { get; internal set; }

      [DataMember(Name = "size")]
      public int Size { get; internal set; }
    }

    public class AuditUsage : SecurityUsage.SecurityFeatureToggle
    {
      [DataMember(Name = "outputs")]
      public IReadOnlyCollection<string> Outputs { get; internal set; } = EmptyReadOnly<string>.Collection;
    }

    public class IpFilterUsage
    {
      [DataMember(Name = "http")]
      public bool Http { get; internal set; }

      [DataMember(Name = "transport")]
      public bool Transport { get; internal set; }
    }

    public class RealmUsage : XPackUsage
    {
      [DataMember(Name = "name")]
      public IReadOnlyCollection<string> Name { get; internal set; } = EmptyReadOnly<string>.Collection;

      [DataMember(Name = "order")]
      public IReadOnlyCollection<long> Order { get; internal set; } = EmptyReadOnly<long>.Collection;

      [DataMember(Name = "size")]
      public IReadOnlyCollection<long> Size { get; internal set; } = EmptyReadOnly<long>.Collection;
    }

    public class RoleUsage
    {
      [DataMember(Name = "dls")]
      public bool Dls { get; internal set; }

      [DataMember(Name = "fls")]
      public bool Fls { get; internal set; }

      [DataMember(Name = "size")]
      public long Size { get; internal set; }
    }

    public class SslUsage
    {
      [DataMember(Name = "http")]
      public SecurityUsage.SecurityFeatureToggle Http { get; internal set; }

      [DataMember(Name = "transport")]
      public SecurityUsage.SecurityFeatureToggle Transport { get; internal set; }
    }

    public class SecurityFeatureToggle
    {
      [DataMember(Name = "enabled")]
      public bool Enabled { get; internal set; }
    }
  }
}
