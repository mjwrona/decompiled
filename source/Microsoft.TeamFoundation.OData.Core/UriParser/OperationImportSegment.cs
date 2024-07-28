// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.OperationImportSegment
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
  public sealed class OperationImportSegment : ODataPathSegment
  {
    private static readonly IEdmType UnknownSentinel = (IEdmType) new EdmEnumType("Sentinel", "UndeterminableTypeMarker");
    private readonly ReadOnlyCollection<IEdmOperationImport> operationImports;
    private readonly ReadOnlyCollection<OperationSegmentParameter> parameters;
    private readonly IEdmEntitySetBase entitySet;
    private readonly IEdmType computedReturnEdmType;

    public OperationImportSegment(IEdmOperationImport operationImport, IEdmEntitySetBase entitySet)
      : this()
    {
      ExceptionUtils.CheckArgumentNotNull<IEdmOperationImport>(operationImport, nameof (operationImport));
      this.operationImports = new ReadOnlyCollection<IEdmOperationImport>((IList<IEdmOperationImport>) new IEdmOperationImport[1]
      {
        operationImport
      });
      this.Identifier = operationImport.Name;
      this.entitySet = entitySet;
      this.computedReturnEdmType = operationImport.Operation.ReturnType != null ? operationImport.Operation.ReturnType.Definition : (IEdmType) null;
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

    public OperationImportSegment(
      IEdmOperationImport operationImport,
      IEdmEntitySetBase entitySet,
      IEnumerable<OperationSegmentParameter> parameters)
      : this(operationImport, entitySet)
    {
      this.parameters = new ReadOnlyCollection<OperationSegmentParameter>(parameters == null ? (IList<OperationSegmentParameter>) new List<OperationSegmentParameter>() : (IList<OperationSegmentParameter>) parameters.ToList<OperationSegmentParameter>());
    }

    public OperationImportSegment(
      IEnumerable<IEdmOperationImport> operationImports,
      IEdmEntitySetBase entitySet)
      : this()
    {
      ExceptionUtils.CheckArgumentNotNull<IEnumerable<IEdmOperationImport>>(operationImports, nameof (operationImports));
      this.operationImports = new ReadOnlyCollection<IEdmOperationImport>((IList<IEdmOperationImport>) operationImports.ToList<IEdmOperationImport>());
      ExceptionUtils.CheckArgumentCollectionNotNullOrEmpty<IEdmOperationImport>((ICollection<IEdmOperationImport>) this.operationImports, "operations");
      this.Identifier = this.operationImports.First<IEdmOperationImport>().Name;
      IEdmType typeSoFar = this.operationImports.First<IEdmOperationImport>().Operation.ReturnType != null ? this.operationImports.First<IEdmOperationImport>().Operation.ReturnType.Definition : (IEdmType) null;
      if (typeSoFar == null)
      {
        if (this.operationImports.Any<IEdmOperationImport>((Func<IEdmOperationImport, bool>) (operation => operation.Operation.ReturnType != null)))
          typeSoFar = OperationImportSegment.UnknownSentinel;
      }
      else if (this.operationImports.Any<IEdmOperationImport>((Func<IEdmOperationImport, bool>) (operationImport => !typeSoFar.IsEquivalentTo(operationImport.Operation.ReturnType.Definition))))
        typeSoFar = OperationImportSegment.UnknownSentinel;
      this.computedReturnEdmType = typeSoFar;
      this.entitySet = entitySet;
      this.EnsureTypeAndSetAreCompatable();
    }

    public OperationImportSegment(
      IEnumerable<IEdmOperationImport> operationImports,
      IEdmEntitySetBase entitySet,
      IEnumerable<OperationSegmentParameter> parameters)
      : this(operationImports, entitySet)
    {
      this.parameters = new ReadOnlyCollection<OperationSegmentParameter>(parameters == null ? (IList<OperationSegmentParameter>) new List<OperationSegmentParameter>() : (IList<OperationSegmentParameter>) parameters.ToList<OperationSegmentParameter>());
    }

    private OperationImportSegment() => this.parameters = new ReadOnlyCollection<OperationSegmentParameter>((IList<OperationSegmentParameter>) new List<OperationSegmentParameter>());

    public IEnumerable<IEdmOperationImport> OperationImports => this.operationImports.AsEnumerable<IEdmOperationImport>();

    public IEnumerable<OperationSegmentParameter> Parameters => (IEnumerable<OperationSegmentParameter>) this.parameters;

    public override IEdmType EdmType => this.computedReturnEdmType != OperationImportSegment.UnknownSentinel ? this.computedReturnEdmType : throw new ODataException(Microsoft.OData.Strings.OperationSegment_ReturnTypeForMultipleOverloads);

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
      return other is OperationImportSegment operationImportSegment && operationImportSegment.OperationImports.SequenceEqual<IEdmOperationImport>(this.OperationImports) && operationImportSegment.EntitySet == this.entitySet;
    }

    private void EnsureTypeAndSetAreCompatable()
    {
      if (this.entitySet == null || this.computedReturnEdmType == OperationImportSegment.UnknownSentinel)
        return;
      IEdmType edmType = this.computedReturnEdmType != null ? this.computedReturnEdmType : throw new ODataException(Microsoft.OData.Strings.OperationSegment_CannotReturnNull);
      if (this.computedReturnEdmType is IEdmCollectionType computedReturnEdmType)
        edmType = computedReturnEdmType.ElementType.Definition;
      if (!this.entitySet.EntityType().IsOrInheritsFrom(edmType) && !edmType.IsOrInheritsFrom((IEdmType) this.entitySet.EntityType()))
        throw new ODataException(Microsoft.OData.Strings.OperationSegment_CannotReturnNull);
    }
  }
}
