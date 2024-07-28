// Decompiled with JetBrains decompiler
// Type: Nest.RoleMappingRuleBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  [JsonFormatter(typeof (RoleMappingRuleBaseFormatter))]
  public abstract class RoleMappingRuleBase
  {
    [DataMember(Name = "all")]
    protected internal IEnumerable<RoleMappingRuleBase> AllRules { get; set; }

    [DataMember(Name = "any")]
    protected internal IEnumerable<RoleMappingRuleBase> AnyRules { get; set; }

    [DataMember(Name = "except")]
    protected internal RoleMappingRuleBase ExceptRule { get; set; }

    [DataMember(Name = "field")]
    protected internal FieldRuleBase FieldRule { get; set; }

    public static AnyRoleMappingRule operator |(
      RoleMappingRuleBase leftContainer,
      RoleMappingRuleBase rightContainer)
    {
      return RoleMappingRuleBase.CombineAny(leftContainer, rightContainer);
    }

    public static AllRoleMappingRule operator &(
      RoleMappingRuleBase leftContainer,
      RoleMappingRuleBase rightContainer)
    {
      return RoleMappingRuleBase.CombineAll(leftContainer, rightContainer);
    }

    public static AllRoleMappingRule operator +(
      RoleMappingRuleBase leftContainer,
      RoleMappingRuleBase rightContainer)
    {
      return RoleMappingRuleBase.CombineAll(leftContainer, rightContainer);
    }

    public static ExceptRoleMappingRole operator !(RoleMappingRuleBase leftContainer) => new ExceptRoleMappingRole(leftContainer);

    private static AnyRoleMappingRule CombineAny(
      RoleMappingRuleBase left,
      RoleMappingRuleBase right)
    {
      List<RoleMappingRuleBase> roleMappingRuleBaseList = new List<RoleMappingRuleBase>();
      roleMappingRuleBaseList.AddRangeIfNotNull<RoleMappingRuleBase>(RoleMappingRuleBase.AnyOrSelf(left));
      roleMappingRuleBaseList.AddRangeIfNotNull<RoleMappingRuleBase>(RoleMappingRuleBase.AnyOrSelf(right));
      return new AnyRoleMappingRule((IEnumerable<RoleMappingRuleBase>) roleMappingRuleBaseList);
    }

    private static AllRoleMappingRule CombineAll(
      RoleMappingRuleBase left,
      RoleMappingRuleBase right)
    {
      List<RoleMappingRuleBase> roleMappingRuleBaseList = new List<RoleMappingRuleBase>();
      roleMappingRuleBaseList.AddRangeIfNotNull<RoleMappingRuleBase>(RoleMappingRuleBase.AllOrSelf(left));
      roleMappingRuleBaseList.AddRangeIfNotNull<RoleMappingRuleBase>(RoleMappingRuleBase.AllOrSelf(right));
      return new AllRoleMappingRule((IEnumerable<RoleMappingRuleBase>) roleMappingRuleBaseList);
    }

    public static IEnumerable<RoleMappingRuleBase> AllOrSelf(RoleMappingRuleBase rule)
    {
      if (rule is AllRoleMappingRule allRoleMappingRule)
        return allRoleMappingRule.AllRules;
      return (IEnumerable<RoleMappingRuleBase>) new RoleMappingRuleBase[1]
      {
        rule
      };
    }

    public static IEnumerable<RoleMappingRuleBase> AnyOrSelf(RoleMappingRuleBase rule)
    {
      if (rule is AnyRoleMappingRule anyRoleMappingRule)
        return anyRoleMappingRule.AnyRules;
      return (IEnumerable<RoleMappingRuleBase>) new RoleMappingRuleBase[1]
      {
        rule
      };
    }

    public static implicit operator RoleMappingRuleBase(UsernameRule rule) => (RoleMappingRuleBase) new FieldRoleMappingRule((FieldRuleBase) rule);

    public static implicit operator RoleMappingRuleBase(DistinguishedNameRule rule) => (RoleMappingRuleBase) new FieldRoleMappingRule((FieldRuleBase) rule);

    public static implicit operator RoleMappingRuleBase(GroupsRule rule) => (RoleMappingRuleBase) new FieldRoleMappingRule((FieldRuleBase) rule);

    public static implicit operator RoleMappingRuleBase(MetadataRule rule) => (RoleMappingRuleBase) new FieldRoleMappingRule((FieldRuleBase) rule);

    public static implicit operator RoleMappingRuleBase(RealmRule rule) => (RoleMappingRuleBase) new FieldRoleMappingRule((FieldRuleBase) rule);
  }
}
