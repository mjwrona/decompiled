// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.AmbiguousOperationImportBinding
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  internal class AmbiguousOperationImportBinding : 
    AmbiguousBinding<IEdmOperationImport>,
    IEdmOperationImport,
    IEdmEntityContainerElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private readonly IEdmOperationImport first;

    public AmbiguousOperationImportBinding(IEdmOperationImport first, IEdmOperationImport second)
      : base(first, second)
    {
      this.first = first;
    }

    public IEdmOperation Operation => this.first.Operation;

    public IEdmEntityContainer Container => this.first.Container;

    public EdmContainerElementKind ContainerElementKind => this.first.ContainerElementKind;

    public IEdmExpression EntitySet => (IEdmExpression) null;
  }
}
