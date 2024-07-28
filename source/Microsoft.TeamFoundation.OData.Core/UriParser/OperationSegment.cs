// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.OperationSegment
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  public sealed class OperationSegment : ODataPathSegment
  {
    private static readonly IEdmType UnknownSentinel = (IEdmType) new EdmEnumType("Sentinel", "UndeterminableTypeMarker");
    private readonly ReadOnlyCollection<IEdmOperation> operations;
    private readonly ReadOnlyCollection<OperationSegmentParameter> parameters;
    private readonly IEdmEntitySetBase entitySet;
    private readonly IEdmType computedReturnEdmType;

    public OperationSegment(IEdmOperation operation, IEdmEntitySetBase entitySet)
      : this()
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmOperation>(operation, nameof (operation));
      this.operations = new ReadOnlyCollection<IEdmOperation>((IList<IEdmOperation>) new IEdmOperation[1]
      {
        operation
      });
      this.entitySet = entitySet;
      this.computedReturnEdmType = operation.ReturnType != null ? operation.ReturnType.Definition : (IEdmType) null;
      this.EnsureTypeAndSetAreCompatable();
      if (this.computedReturnEdmType != null)
      {
        this.TargetEdmNavigationSource = (IEdmNavigationSource) entitySet;
        this.TargetEdmType = this.computedReturnEdmType;
        this.TargetKind = this.TargetEdmType.GetTargetKindFromType();
        this.SingleResult = this.computedReturnEdmType.TypeKind != EdmTypeKind.Collection;
      }
      else
      {
        this.TargetEdmNavigationSource = (IEdmNavigationSource) null;
        this.TargetEdmType = (IEdmType) null;
        this.TargetKind = RequestTargetKind.VoidOperation;
      }
    }

    public OperationSegment(
      IEdmOperation operation,
      IEnumerable<OperationSegmentParameter> parameters,
      IEdmEntitySetBase entitySet)
      : this(operation, entitySet)
    {
      this.parameters = new ReadOnlyCollection<OperationSegmentParameter>(parameters == null ? (IList<OperationSegmentParameter>) new List<OperationSegmentParameter>() : (IList<OperationSegmentParameter>) parameters.ToList<OperationSegmentParameter>());
    }

    public OperationSegment(IEnumerable<IEdmOperation> operations, IEdmEntitySetBase entitySet)
      : this()
    {
      ExceptionUtils.CheckArgumentNotNull<IEnumerable<IEdmOperation>>(operations, nameof (operations));
      this.operations = new ReadOnlyCollection<IEdmOperation>((IList<IEdmOperation>) operations.ToList<IEdmOperation>());
      ExceptionUtils.CheckArgumentCollectionNotNullOrEmpty<IEdmOperation>((ICollection<IEdmOperation>) this.operations, nameof (operations));
      IEdmType typeSoFar = this.operations.First<IEdmOperation>().ReturnType != null ? this.operations.First<IEdmOperation>().ReturnType.Definition : (IEdmType) null;
      if (typeSoFar == null)
      {
        if (this.operations.Any<IEdmOperation>((Func<IEdmOperation, bool>) (operation => operation.ReturnType != null)))
          typeSoFar = OperationSegment.UnknownSentinel;
      }
      else if (this.operations.Any<IEdmOperation>((Func<IEdmOperation, bool>) (operationImport => !typeSoFar.IsEquivalentTo(operationImport.ReturnType.Definition))))
        typeSoFar = OperationSegment.UnknownSentinel;
      this.computedReturnEdmType = typeSoFar;
      this.entitySet = entitySet;
      this.EnsureTypeAndSetAreCompatable();
    }

    public OperationSegment(
      IEnumerable<IEdmOperation> operations,
      IEnumerable<OperationSegmentParameter> parameters,
      IEdmEntitySetBase entitySet)
      : this(operations, entitySet)
    {
      this.parameters = new ReadOnlyCollection<OperationSegmentParameter>(parameters == null ? (IList<OperationSegmentParameter>) new List<OperationSegmentParameter>() : (IList<OperationSegmentParameter>) parameters.ToList<OperationSegmentParameter>());
    }

    private OperationSegment() => this.parameters = new ReadOnlyCollection<OperationSegmentParameter>((IList<OperationSegmentParameter>) new List<OperationSegmentParameter>());

    public IEnumerable<IEdmOperation> Operations => this.operations.AsEnumerable<IEdmOperation>();

    public IEnumerable<OperationSegmentParameter> Parameters => (IEnumerable<OperationSegmentParameter>) this.parameters;

    public override IEdmType EdmType => this.computedReturnEdmType != OperationSegment.UnknownSentinel ? this.computedReturnEdmType : throw new ODataException(Microsoft.OData.Strings.OperationSegment_ReturnTypeForMultipleOverloads);

    public IEdmEntitySetBase EntitySet => this.entitySet;

    public override T TranslateWith<T>(PathSegmentTranslator<T> translator)
    {
      ExceptionUtils.CheckArgumentNotNull<PathSegmentTranslator<T>>(translator, nameof (translator));
      return translator.Translate(this);
    }

    public override void HandleWith(PathSegmentHandler handler)
    {
      ExceptionUtils.CheckArgumentNotNull<PathSegmentHandler>(handler, nameof (handler));
      handler.Handle(this);
    }

    internal override bool Equals(ODataPathSegment other)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataPathSegment>(other, nameof (other));
      return other is OperationSegment operationSegment && operationSegment.Operations.SequenceEqual<IEdmOperation>(this.Operations) && operationSegment.EntitySet == this.entitySet;
    }

    private void EnsureTypeAndSetAreCompatable()
    {
      if (this.entitySet == null || this.computedReturnEdmType == OperationSegment.UnknownSentinel)
        return;
      IEdmType edmType = this.computedReturnEdmType != null ? this.computedReturnEdmType : throw new ODataException(Microsoft.OData.Strings.OperationSegment_CannotReturnNull);
      if (this.computedReturnEdmType is IEdmCollectionType computedReturnEdmType)
        edmType = computedReturnEdmType.ElementType.Definition;
      if (!this.entitySet.EntityType().IsOrInheritsFrom(edmType) && !edmType.IsOrInheritsFrom((IEdmType) this.entitySet.EntityType()))
        throw new ODataException(Microsoft.OData.Strings.OperationSegment_CannotReturnNull);
    }
  }
}
