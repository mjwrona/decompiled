// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.UnresolvedOperation
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class UnresolvedOperation : 
    BadElement,
    IEdmOperation,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IUnresolvedElement,
    IEdmFullNamedElement
  {
    private readonly string namespaceName;
    private readonly string name;
    private readonly string fullName;
    private readonly IEdmTypeReference returnType;

    public UnresolvedOperation(string qualifiedName, string errorMessage, EdmLocation location)
      : base((IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(location, EdmErrorCode.BadUnresolvedOperation, errorMessage)
      })
    {
      qualifiedName = qualifiedName ?? string.Empty;
      EdmUtil.TryGetNamespaceNameFromQualifiedName(qualifiedName, out this.namespaceName, out this.name, out this.fullName);
      this.returnType = (IEdmTypeReference) new BadTypeReference(new BadType(this.Errors), true);
    }

    public string Namespace => this.namespaceName;

    public string Name => this.name;

    public string FullName => this.fullName;

    public IEdmTypeReference ReturnType => this.returnType;

    public IEnumerable<IEdmOperationParameter> Parameters => Enumerable.Empty<IEdmOperationParameter>();

    public bool IsBound => false;

    public IEdmPathExpression EntitySetPath => (IEdmPathExpression) null;

    public EdmSchemaElementKind SchemaElementKind => EdmSchemaElementKind.None;

    public IEdmOperationParameter FindParameter(string name) => (IEdmOperationParameter) null;
  }
}
