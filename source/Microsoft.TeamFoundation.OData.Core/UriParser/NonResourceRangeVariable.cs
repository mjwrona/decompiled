// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.NonResourceRangeVariable
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.OData.UriParser
{
  public sealed class NonResourceRangeVariable : RangeVariable
  {
    private readonly string name;
    private readonly CollectionNode collectionNode;
    private readonly IEdmTypeReference typeReference;

    public NonResourceRangeVariable(
      string name,
      IEdmTypeReference typeReference,
      CollectionNode collectionNode)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(name, nameof (name));
      this.name = name;
      if (typeReference != null && typeReference.Definition.TypeKind.IsStructured())
        throw new ArgumentException(Microsoft.OData.Strings.Nodes_NonentityParameterQueryNodeWithEntityType((object) typeReference.FullName()));
      this.typeReference = typeReference;
      this.collectionNode = collectionNode;
    }

    public override string Name => this.name;

    public override IEdmTypeReference TypeReference => this.typeReference;

    public CollectionNode CollectionNode => this.collectionNode;

    public override int Kind => 1;
  }
}
