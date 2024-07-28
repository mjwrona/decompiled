// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.GroupByPropertyNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.OData.UriParser.Aggregation
{
  public sealed class GroupByPropertyNode
  {
    private IList<GroupByPropertyNode> childTransformations = (IList<GroupByPropertyNode>) new List<GroupByPropertyNode>();
    private IEdmTypeReference typeReference;

    public GroupByPropertyNode(string name, SingleValueNode expression)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(name, nameof (name));
      this.Name = name;
      this.Expression = expression;
    }

    public GroupByPropertyNode(string name, SingleValueNode expression, IEdmTypeReference type)
      : this(name, expression)
    {
      this.typeReference = type;
    }

    public string Name { get; private set; }

    public SingleValueNode Expression { get; private set; }

    public IEdmTypeReference TypeReference => this.Expression == null ? (IEdmTypeReference) null : this.typeReference;

    public IList<GroupByPropertyNode> ChildTransformations
    {
      get => this.childTransformations;
      set => this.childTransformations = value;
    }
  }
}
