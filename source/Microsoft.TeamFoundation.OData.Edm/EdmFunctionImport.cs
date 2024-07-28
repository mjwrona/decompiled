// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmFunctionImport
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  public class EdmFunctionImport : 
    EdmOperationImport,
    IEdmFunctionImport,
    IEdmOperationImport,
    IEdmEntityContainerElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private const string FunctionArgumentNullParameterName = "function";

    public EdmFunctionImport(IEdmEntityContainer container, string name, IEdmFunction function)
      : this(container, name, function, (IEdmExpression) null, false)
    {
    }

    public EdmFunctionImport(
      IEdmEntityContainer container,
      string name,
      IEdmFunction function,
      IEdmExpression entitySetExpression,
      bool includeInServiceDocument)
      : base(container, (IEdmOperation) function, name, entitySetExpression)
    {
      EdmUtil.CheckArgumentNull<IEdmFunction>(function, nameof (function));
      this.Function = function;
      this.IncludeInServiceDocument = includeInServiceDocument;
    }

    public IEdmFunction Function { get; private set; }

    public override EdmContainerElementKind ContainerElementKind => EdmContainerElementKind.FunctionImport;

    public bool IncludeInServiceDocument { get; private set; }

    protected override string OperationArgumentNullParameterName() => "function";
  }
}
