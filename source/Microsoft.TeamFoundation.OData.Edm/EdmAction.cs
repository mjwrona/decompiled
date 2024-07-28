// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmAction
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  public class EdmAction : 
    EdmOperation,
    IEdmAction,
    IEdmOperation,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    public EdmAction(
      string namespaceName,
      string name,
      IEdmTypeReference returnType,
      bool isBound,
      IEdmPathExpression entitySetPathExpression)
      : base(namespaceName, name, returnType, isBound, entitySetPathExpression)
    {
    }

    public EdmAction(string namespaceName, string name, IEdmTypeReference returnType)
      : base(namespaceName, name, returnType)
    {
    }

    public override EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.Action;
  }
}
