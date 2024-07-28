// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmOperationImport
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  public abstract class EdmOperationImport : 
    EdmNamedElement,
    IEdmOperationImport,
    IEdmEntityContainerElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    protected EdmOperationImport(
      IEdmEntityContainer container,
      IEdmOperation operation,
      string name,
      IEdmExpression entitySet)
      : base(name)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityContainer>(container, nameof (container));
      EdmUtil.CheckArgumentNull<IEdmOperation>(operation, this.OperationArgumentNullParameterName());
      this.Container = container;
      this.Operation = operation;
      this.EntitySet = entitySet;
    }

    public IEdmOperation Operation { get; private set; }

    public IEdmExpression EntitySet { get; private set; }

    public abstract EdmContainerElementKind ContainerElementKind { get; }

    public IEdmEntityContainer Container { get; private set; }

    protected abstract string OperationArgumentNullParameterName();
  }
}
