// Decompiled with JetBrains decompiler
// Type: Nest.RoleMappingRulesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class RoleMappingRulesDescriptor : 
    DescriptorPromiseBase<RoleMappingRulesDescriptor, List<RoleMappingRuleBase>>
  {
    public RoleMappingRulesDescriptor()
      : base(new List<RoleMappingRuleBase>())
    {
    }

    private RoleMappingRulesDescriptor Add(RoleMappingRuleBase m)
    {
      this.PromisedValue.AddIfNotNull<RoleMappingRuleBase>(m);
      return this;
    }

    public RoleMappingRulesDescriptor DistinguishedName(IEnumerable<string> names) => this.Add((RoleMappingRuleBase) new DistinguishedNameRule(names));

    public RoleMappingRulesDescriptor DistinguishedName(string name) => this.Add((RoleMappingRuleBase) new DistinguishedNameRule(name));

    public RoleMappingRulesDescriptor Username(IEnumerable<string> usernames) => this.Add((RoleMappingRuleBase) new UsernameRule(usernames));

    public RoleMappingRulesDescriptor Username(string username) => this.Add((RoleMappingRuleBase) new UsernameRule(username));

    public RoleMappingRulesDescriptor Groups(params string[] groups) => this.Add((RoleMappingRuleBase) new GroupsRule(groups));

    public RoleMappingRulesDescriptor Groups(IEnumerable<string> groups) => this.Add((RoleMappingRuleBase) new GroupsRule(groups));

    public RoleMappingRulesDescriptor Realm(string realm) => this.Add((RoleMappingRuleBase) new RealmRule(realm));

    public RoleMappingRulesDescriptor Metadata(string key, object value) => this.Add((RoleMappingRuleBase) new MetadataRule(key, value));

    public RoleMappingRulesDescriptor Any(
      Func<RoleMappingRulesDescriptor, IPromise<List<RoleMappingRuleBase>>> selector)
    {
      return this.Add((RoleMappingRuleBase) new AnyRoleMappingRule(selector != null ? (IEnumerable<RoleMappingRuleBase>) selector(new RoleMappingRulesDescriptor())?.Value : (IEnumerable<RoleMappingRuleBase>) null));
    }

    public RoleMappingRulesDescriptor All(
      Func<RoleMappingRulesDescriptor, IPromise<List<RoleMappingRuleBase>>> selector)
    {
      return this.Add((RoleMappingRuleBase) new AllRoleMappingRule(selector != null ? (IEnumerable<RoleMappingRuleBase>) selector(new RoleMappingRulesDescriptor())?.Value : (IEnumerable<RoleMappingRuleBase>) null));
    }

    public RoleMappingRulesDescriptor Except(
      Func<RoleMappingRuleDescriptor, RoleMappingRuleBase> selector)
    {
      return this.Add((RoleMappingRuleBase) new ExceptRoleMappingRole(selector != null ? selector(new RoleMappingRuleDescriptor()) : (RoleMappingRuleBase) null));
    }
  }
}
