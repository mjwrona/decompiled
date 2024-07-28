// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.EnumMemberConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;

namespace Microsoft.AspNet.OData.Builder
{
  public class EnumMemberConfiguration
  {
    private string _name;

    public EnumMemberConfiguration(Enum member, EnumTypeConfiguration declaringType)
    {
      if (member == null)
        throw Error.ArgumentNull(nameof (member));
      if (declaringType == null)
        throw Error.ArgumentNull(nameof (declaringType));
      this.MemberInfo = member;
      this.DeclaringType = declaringType;
      this.AddedExplicitly = true;
      this._name = Enum.GetName(member.GetType(), (object) member);
    }

    public string Name
    {
      get => this._name;
      set => this._name = value != null ? value : throw Error.PropertyNull();
    }

    public EnumTypeConfiguration DeclaringType { get; private set; }

    public Enum MemberInfo { get; private set; }

    public bool AddedExplicitly { get; set; }
  }
}
