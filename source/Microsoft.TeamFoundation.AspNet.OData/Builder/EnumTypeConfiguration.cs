// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.EnumTypeConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.AspNet.OData.Builder
{
  public class EnumTypeConfiguration : IEdmTypeConfiguration
  {
    private string _namespace;
    private string _name;
    private NullableEnumTypeConfiguration nullableEnumTypeConfiguration;

    public EnumTypeConfiguration(ODataModelBuilder builder, Type clrType)
    {
      if (builder == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (builder));
      if (clrType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (clrType));
      this.ClrType = TypeHelper.IsEnum(clrType) ? clrType : throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (clrType), SRResources.TypeCannotBeEnum, (object) clrType.FullName);
      this.IsFlags = ((IEnumerable<object>) TypeHelper.AsMemberInfo(clrType).GetCustomAttributes(typeof (FlagsAttribute), false)).Any<object>();
      this.UnderlyingType = Enum.GetUnderlyingType(clrType);
      this.ModelBuilder = builder;
      this._name = clrType.EdmName();
      this._namespace = builder.HasAssignedNamespace ? builder.Namespace : clrType.Namespace ?? builder.Namespace;
      this.ExplicitMembers = (IDictionary<Enum, EnumMemberConfiguration>) new Dictionary<Enum, EnumMemberConfiguration>();
      this.RemovedMembers = (IList<Enum>) new List<Enum>();
    }

    public EdmTypeKind Kind => EdmTypeKind.Enum;

    public bool IsFlags { get; private set; }

    public Type ClrType { get; private set; }

    public Type UnderlyingType { get; private set; }

    public string FullName => this.Namespace + "." + this.Name;

    public string Namespace
    {
      get => this._namespace;
      set
      {
        this._namespace = value != null ? value : throw Microsoft.AspNet.OData.Common.Error.PropertyNull();
        this.AddedExplicitly = true;
      }
    }

    public string Name
    {
      get => this._name;
      set
      {
        this._name = value != null ? value : throw Microsoft.AspNet.OData.Common.Error.PropertyNull();
        this.AddedExplicitly = true;
      }
    }

    public IEnumerable<EnumMemberConfiguration> Members => (IEnumerable<EnumMemberConfiguration>) this.ExplicitMembers.Values;

    public ReadOnlyCollection<Enum> IgnoredMembers => new ReadOnlyCollection<Enum>(this.RemovedMembers);

    public bool AddedExplicitly { get; set; }

    public ODataModelBuilder ModelBuilder { get; private set; }

    protected internal IList<Enum> RemovedMembers { get; private set; }

    protected internal IDictionary<Enum, EnumMemberConfiguration> ExplicitMembers { get; private set; }

    public EnumMemberConfiguration AddMember(Enum member)
    {
      if (member == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (member));
      if (member.GetType() != this.ClrType)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (member), SRResources.PropertyDoesNotBelongToType, (object) member.ToString(), (object) this.ClrType.FullName);
      if (this.RemovedMembers.Contains(member))
        this.RemovedMembers.Remove(member);
      EnumMemberConfiguration memberConfiguration;
      if (this.ExplicitMembers.ContainsKey(member))
      {
        memberConfiguration = this.ExplicitMembers[member];
      }
      else
      {
        memberConfiguration = new EnumMemberConfiguration(member, this);
        this.ExplicitMembers[member] = memberConfiguration;
      }
      return memberConfiguration;
    }

    public void RemoveMember(Enum member)
    {
      if (member == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (member));
      if (member.GetType() != this.ClrType)
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (member), SRResources.PropertyDoesNotBelongToType, (object) member.ToString(), (object) this.ClrType.FullName);
      if (this.ExplicitMembers.ContainsKey(member))
        this.ExplicitMembers.Remove(member);
      if (this.RemovedMembers.Contains(member))
        return;
      this.RemovedMembers.Add(member);
    }

    internal NullableEnumTypeConfiguration GetNullableEnumTypeConfiguration()
    {
      if (this.nullableEnumTypeConfiguration == null)
        this.nullableEnumTypeConfiguration = new NullableEnumTypeConfiguration(this);
      return this.nullableEnumTypeConfiguration;
    }
  }
}
