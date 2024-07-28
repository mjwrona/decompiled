// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmActionImport
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  public class EdmActionImport : 
    EdmOperationImport,
    IEdmActionImport,
    IEdmOperationImport,
    IEdmEntityContainerElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private const string ActionArgumentNullParameterName = "action";

    public EdmActionImport(IEdmEntityContainer container, string name, IEdmAction action)
      : this(container, name, action, (IEdmExpression) null)
    {
    }

    public EdmActionImport(
      IEdmEntityContainer container,
      string name,
      IEdmAction action,
      IEdmExpression entitySetExpression)
      : base(container, (IEdmOperation) action, name, entitySetExpression)
    {
      EdmUtil.CheckArgumentNull<IEdmAction>(action, nameof (action));
      this.Action = action;
    }

    public IEdmAction Action { get; private set; }

    public override EdmContainerElementKind ContainerElementKind => EdmContainerElementKind.ActionImport;

    protected override string OperationArgumentNullParameterName() => "action";
  }
}
