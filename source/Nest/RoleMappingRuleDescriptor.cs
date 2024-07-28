// Decompiled with JetBrains decompiler
// Type: Nest.RoleMappingRuleDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class RoleMappingRuleDescriptor : 
    DescriptorBase<RoleMappingRuleDescriptor, IDescriptor>,
    IDescriptor
  {
    public RoleMappingRuleBase DistinguishedName(string name) => (RoleMappingRuleBase) new DistinguishedNameRule(name);

    public RoleMappingRuleBase Username(string username) => (RoleMappingRuleBase) new UsernameRule(username);

    public RoleMappingRuleBase Groups(params string[] groups) => (RoleMappingRuleBase) new GroupsRule(groups);

    public RoleMappingRuleBase Groups(IEnumerable<string> groups) => (RoleMappingRuleBase) new GroupsRule(groups);

    public RoleMappingRuleBase Realm(string realm) => (RoleMappingRuleBase) new RealmRule(realm);

    public RoleMappingRuleBase Metadata(string key, object value) => (RoleMappingRuleBase) new MetadataRule(key, value);

    public RoleMappingRuleBase Any(
      Func<RoleMappingRulesDescriptor, IPromise<List<RoleMappingRuleBase>>> selector)
    {
      return (RoleMappingRuleBase) new AnyRoleMappingRule(selector != null ? (IEnumerable<RoleMappingRuleBase>) selector(new RoleMappingRulesDescriptor())?.Value : (IEnumerable<RoleMappingRuleBase>) null);
    }

    public RoleMappingRuleBase All(
      Func<RoleMappingRulesDescriptor, IPromise<List<RoleMappingRuleBase>>> selector)
    {
      return (RoleMappingRuleBase) new AllRoleMappingRule(selector != null ? (IEnumerable<RoleMappingRuleBase>) selector(new RoleMappingRulesDescriptor())?.Value : (IEnumerable<RoleMappingRuleBase>) null);
    }

    public RoleMappingRuleBase Except(
      Func<RoleMappingRuleDescriptor, RoleMappingRuleBase> selector)
    {
      return (RoleMappingRuleBase) new ExceptRoleMappingRole(selector != null ? selector(new RoleMappingRuleDescriptor()) : (RoleMappingRuleBase) null);
    }
  }
}
