// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmEnumMemberExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmEnumMemberExpression : 
    EdmElement,
    IEdmEnumMemberExpression,
    IEdmExpression,
    IEdmElement
  {
    private readonly List<IEdmEnumMember> enumMembers;

    public EdmEnumMemberExpression(params IEdmEnumMember[] enumMembers)
    {
      EdmUtil.CheckArgumentNull<IEdmEnumMember[]>(enumMembers, "referencedEnumMember");
      this.enumMembers = ((IEnumerable<IEdmEnumMember>) enumMembers).ToList<IEdmEnumMember>();
    }

    public IEnumerable<IEdmEnumMember> EnumMembers => (IEnumerable<IEdmEnumMember>) this.enumMembers;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.EnumMember;
  }
}
