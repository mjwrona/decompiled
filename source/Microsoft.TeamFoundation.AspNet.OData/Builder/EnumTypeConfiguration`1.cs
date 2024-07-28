// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.EnumTypeConfiguration`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Builder
{
  public class EnumTypeConfiguration<TEnumType>
  {
    private EnumTypeConfiguration _configuration;

    internal EnumTypeConfiguration(EnumTypeConfiguration configuration) => this._configuration = configuration;

    public IEnumerable<EnumMemberConfiguration> Members => this._configuration.Members;

    public string FullName => this._configuration.FullName;

    public string Namespace
    {
      get => this._configuration.Namespace;
      set => this._configuration.Namespace = value;
    }

    public string Name
    {
      get => this._configuration.Name;
      set => this._configuration.Name = value;
    }

    public virtual void RemoveMember(TEnumType member) => this._configuration.RemoveMember((Enum) (object) member);

    public EnumMemberConfiguration Member(TEnumType enumMember) => this._configuration.AddMember((Enum) (object) enumMember);
  }
}
