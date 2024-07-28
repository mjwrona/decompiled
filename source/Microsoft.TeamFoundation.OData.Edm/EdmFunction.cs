// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmFunction
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  public class EdmFunction : 
    EdmOperation,
    IEdmFunction,
    IEdmOperation,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    public EdmFunction(
      string namespaceName,
      string name,
      IEdmTypeReference returnType,
      bool isBound,
      IEdmPathExpression entitySetPathExpression,
      bool isComposable)
      : base(namespaceName, name, returnType, isBound, entitySetPathExpression)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(returnType, nameof (returnType));
      this.IsComposable = isComposable;
    }

    public EdmFunction(string namespaceName, string name, IEdmTypeReference returnType)
      : this(namespaceName, name, returnType, false, (IEdmPathExpression) null, false)
    {
    }

    public override EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.Function;

    public bool IsComposable { get; private set; }
  }
}
