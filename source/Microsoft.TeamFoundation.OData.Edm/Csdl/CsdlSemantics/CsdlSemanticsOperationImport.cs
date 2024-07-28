// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsOperationImport
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal abstract class CsdlSemanticsOperationImport : 
    CsdlSemanticsElement,
    IEdmOperationImport,
    IEdmEntityContainerElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private readonly CsdlOperationImport operationImport;
    private readonly CsdlSemanticsEntityContainer container;
    private readonly Cache<CsdlSemanticsOperationImport, IEdmExpression> entitySetCache = new Cache<CsdlSemanticsOperationImport, IEdmExpression>();
    private static readonly Func<CsdlSemanticsOperationImport, IEdmExpression> ComputeEntitySetFunc = (Func<CsdlSemanticsOperationImport, IEdmExpression>) (me => me.ComputeEntitySet());

    protected CsdlSemanticsOperationImport(
      CsdlSemanticsEntityContainer container,
      CsdlOperationImport operationImport,
      IEdmOperation operation)
      : base((CsdlElement) operationImport)
    {
      this.container = container;
      this.operationImport = operationImport;
      this.Operation = operation;
    }

    public IEdmOperation Operation { get; private set; }

    public override CsdlSemanticsModel Model => this.container.Context.Model;

    public override CsdlElement Element => (CsdlElement) this.operationImport;

    public string Name => this.operationImport.Name;

    public IEdmEntityContainer Container => (IEdmEntityContainer) this.container;

    public IEdmExpression EntitySet => this.entitySetCache.GetValue(this, CsdlSemanticsOperationImport.ComputeEntitySetFunc, (Func<CsdlSemanticsOperationImport, IEdmExpression>) null);

    public abstract EdmContainerElementKind ContainerElementKind { get; }

    protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations() => this.Model.WrapInlineVocabularyAnnotations((CsdlSemanticsElement) this, this.container.Context);

    private IEdmExpression ComputeEntitySet()
    {
      if (this.operationImport.EntitySet == null)
        return (IEdmExpression) null;
      return (IEdmExpression) new CsdlSemanticsOperationImport.OperationImportPathExpression(this.operationImport.EntitySet)
      {
        Location = this.Location
      };
    }

    private sealed class OperationImportPathExpression : EdmPathExpression, IEdmLocatable
    {
      internal OperationImportPathExpression(string path)
        : base(path)
      {
      }

      public EdmLocation Location { get; set; }
    }
  }
}
