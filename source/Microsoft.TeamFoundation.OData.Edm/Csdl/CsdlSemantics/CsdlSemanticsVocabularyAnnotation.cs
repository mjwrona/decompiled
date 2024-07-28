// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsVocabularyAnnotation
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsVocabularyAnnotation : 
    CsdlSemanticsElement,
    IEdmVocabularyAnnotation,
    IEdmElement,
    IEdmCheckable
  {
    protected readonly CsdlAnnotation Annotation;
    private readonly CsdlSemanticsSchema schema;
    private readonly string qualifier;
    private readonly IEdmVocabularyAnnotatable targetContext;
    private readonly CsdlSemanticsAnnotations annotationsContext;
    private readonly Cache<CsdlSemanticsVocabularyAnnotation, IEdmExpression> valueCache = new Cache<CsdlSemanticsVocabularyAnnotation, IEdmExpression>();
    private static readonly Func<CsdlSemanticsVocabularyAnnotation, IEdmExpression> ComputeValueFunc = (Func<CsdlSemanticsVocabularyAnnotation, IEdmExpression>) (me => me.ComputeValue());
    private readonly Cache<CsdlSemanticsVocabularyAnnotation, IEdmTerm> termCache = new Cache<CsdlSemanticsVocabularyAnnotation, IEdmTerm>();
    private static readonly Func<CsdlSemanticsVocabularyAnnotation, IEdmTerm> ComputeTermFunc = (Func<CsdlSemanticsVocabularyAnnotation, IEdmTerm>) (me => me.ComputeTerm());
    private readonly Cache<CsdlSemanticsVocabularyAnnotation, IEdmVocabularyAnnotatable> targetCache = new Cache<CsdlSemanticsVocabularyAnnotation, IEdmVocabularyAnnotatable>();
    private static readonly Func<CsdlSemanticsVocabularyAnnotation, IEdmVocabularyAnnotatable> ComputeTargetFunc = (Func<CsdlSemanticsVocabularyAnnotation, IEdmVocabularyAnnotatable>) (me => me.ComputeTarget());

    public CsdlSemanticsVocabularyAnnotation(
      CsdlSemanticsSchema schema,
      IEdmVocabularyAnnotatable targetContext,
      CsdlSemanticsAnnotations annotationsContext,
      CsdlAnnotation annotation,
      string qualifier)
      : base((CsdlElement) annotation)
    {
      this.schema = schema;
      this.Annotation = annotation;
      this.qualifier = qualifier ?? annotation.Qualifier;
      this.targetContext = targetContext;
      this.annotationsContext = annotationsContext;
    }

    public CsdlSemanticsSchema Schema => this.schema;

    public override CsdlElement Element => (CsdlElement) this.Annotation;

    public string Qualifier => this.qualifier;

    public override CsdlSemanticsModel Model => this.schema.Model;

    public IEdmTerm Term => this.termCache.GetValue(this, CsdlSemanticsVocabularyAnnotation.ComputeTermFunc, (Func<CsdlSemanticsVocabularyAnnotation, IEdmTerm>) null);

    public IEdmVocabularyAnnotatable Target => this.targetCache.GetValue(this, CsdlSemanticsVocabularyAnnotation.ComputeTargetFunc, (Func<CsdlSemanticsVocabularyAnnotation, IEdmVocabularyAnnotatable>) null);

    public IEnumerable<EdmError> Errors
    {
      get
      {
        if (this.Term is IUnresolvedElement)
          return this.Term.Errors();
        return this.Target is IUnresolvedElement ? this.Target.Errors() : Enumerable.Empty<EdmError>();
      }
    }

    public IEdmEntityType TargetBindingContext
    {
      get
      {
        switch (this.Target)
        {
          case IEdmNavigationSource navigationSource:
            pattern_0 = navigationSource.EntityType();
            break;
        }
        return pattern_0;
      }
    }

    public IEdmExpression Value => this.valueCache.GetValue(this, CsdlSemanticsVocabularyAnnotation.ComputeValueFunc, (Func<CsdlSemanticsVocabularyAnnotation, IEdmExpression>) null);

    protected IEdmTerm ComputeTerm() => this.Schema.FindTerm(this.Annotation.Term) ?? (IEdmTerm) new UnresolvedVocabularyTerm(this.Schema.UnresolvedName(this.Annotation.Term));

    private IEdmExpression ComputeValue() => CsdlSemanticsModel.WrapExpression(this.Annotation.Expression, this.TargetBindingContext, this.Schema);

    private IEdmVocabularyAnnotatable ComputeTarget()
    {
      if (this.targetContext != null)
        return this.targetContext;
      string target = this.annotationsContext.Annotations.Target;
      string[] source = target.Split('/');
      switch (((IEnumerable<string>) source).Count<string>())
      {
        case 1:
          string str1 = source[0];
          return (IEdmVocabularyAnnotatable) this.schema.FindType(str1) ?? (IEdmVocabularyAnnotatable) this.schema.FindTerm(str1) ?? (IEdmVocabularyAnnotatable) this.FindParameterizedOperation(str1, new Func<string, IEnumerable<IEdmOperation>>(this.Schema.FindOperations), new Func<IEnumerable<IEdmOperation>, IEdmOperation>(this.CreateAmbiguousOperation)) ?? (IEdmVocabularyAnnotatable) this.schema.FindEntityContainer(str1) ?? (IEdmVocabularyAnnotatable) new UnresolvedType(this.Schema.UnresolvedName(source[0]), this.Location);
        case 2:
          IEdmEntityContainer entityContainer1 = this.schema.FindEntityContainer(source[0]);
          if (entityContainer1 != null)
            return (IEdmVocabularyAnnotatable) entityContainer1.FindEntitySetExtended(source[1]) ?? (IEdmVocabularyAnnotatable) CsdlSemanticsVocabularyAnnotation.FindParameterizedOperationImport(source[1], new Func<string, IEnumerable<IEdmOperationImport>>(((ExtensionMethods) entityContainer1).FindOperationImportsExtended), new Func<IEnumerable<IEdmOperationImport>, IEdmOperationImport>(this.CreateAmbiguousOperationImport)) ?? (IEdmVocabularyAnnotatable) new UnresolvedEntitySet(source[1], entityContainer1, this.Location);
          switch (this.schema.FindType(source[0]))
          {
            case IEdmStructuredType declaringType1:
              return (IEdmVocabularyAnnotatable) declaringType1.FindProperty(source[1]) ?? (IEdmVocabularyAnnotatable) new UnresolvedProperty(declaringType1, source[1], this.Location);
            case IEdmEnumType declaringType2:
              foreach (IEdmEnumMember member in declaringType2.Members)
              {
                if (string.Equals(member.Name, source[1], StringComparison.OrdinalIgnoreCase))
                  return (IEdmVocabularyAnnotatable) member;
              }
              return (IEdmVocabularyAnnotatable) new UnresolvedEnumMember(source[1], declaringType2, this.Location);
            default:
              IEdmOperation parameterizedOperation = this.FindParameterizedOperation(source[0], new Func<string, IEnumerable<IEdmOperation>>(this.Schema.FindOperations), new Func<IEnumerable<IEdmOperation>, IEdmOperation>(this.CreateAmbiguousOperation));
              if (parameterizedOperation == null)
                return (IEdmVocabularyAnnotatable) new UnresolvedProperty((IEdmStructuredType) new UnresolvedEntityType(this.Schema.UnresolvedName(source[0]), this.Location), source[1], this.Location);
              if (!(source[1] == "$ReturnType"))
                return (IEdmVocabularyAnnotatable) parameterizedOperation.FindParameter(source[1]) ?? (IEdmVocabularyAnnotatable) new UnresolvedParameter(parameterizedOperation, source[1], this.Location);
              return parameterizedOperation.ReturnType != null ? (IEdmVocabularyAnnotatable) parameterizedOperation.GetReturn() : (IEdmVocabularyAnnotatable) new UnresolvedReturn(parameterizedOperation, this.Location);
          }
        case 3:
          string qualifiedName = source[0];
          string parameterizedName = source[1];
          string name = source[2];
          IEdmEntityContainer entityContainer2 = this.Model.FindEntityContainer(qualifiedName);
          if (entityContainer2 != null)
          {
            IEdmOperationImport parameterizedOperationImport = CsdlSemanticsVocabularyAnnotation.FindParameterizedOperationImport(parameterizedName, new Func<string, IEnumerable<IEdmOperationImport>>(((ExtensionMethods) entityContainer2).FindOperationImportsExtended), new Func<IEnumerable<IEdmOperationImport>, IEdmOperationImport>(this.CreateAmbiguousOperationImport));
            if (parameterizedOperationImport != null)
            {
              if (!(name == "$ReturnType"))
                return (IEdmVocabularyAnnotatable) parameterizedOperationImport.Operation.FindParameter(name) ?? (IEdmVocabularyAnnotatable) new UnresolvedParameter(parameterizedOperationImport.Operation, name, this.Location);
              return parameterizedOperationImport.Operation.ReturnType != null ? (IEdmVocabularyAnnotatable) parameterizedOperationImport.Operation.GetReturn() : (IEdmVocabularyAnnotatable) new UnresolvedReturn(parameterizedOperationImport.Operation, this.Location);
            }
          }
          string str2 = qualifiedName + "/" + parameterizedName;
          UnresolvedOperation declaringOperation = new UnresolvedOperation(str2, Strings.Bad_UnresolvedOperation((object) str2), this.Location);
          return name == "$ReturnType" ? (IEdmVocabularyAnnotatable) new UnresolvedReturn((IEdmOperation) declaringOperation, this.Location) : (IEdmVocabularyAnnotatable) new UnresolvedParameter((IEdmOperation) declaringOperation, name, this.Location);
        default:
          return (IEdmVocabularyAnnotatable) new BadElement((IEnumerable<EdmError>) new EdmError[1]
          {
            new EdmError(this.Location, EdmErrorCode.ImpossibleAnnotationsTarget, Strings.CsdlSemantics_ImpossibleAnnotationsTarget((object) target))
          });
      }
    }

    private static IEdmOperationImport FindParameterizedOperationImport(
      string parameterizedName,
      Func<string, IEnumerable<IEdmOperationImport>> findFunctions,
      Func<IEnumerable<IEdmOperationImport>, IEdmOperationImport> ambiguityCreator)
    {
      IEnumerable<IEdmOperationImport> source = findFunctions(parameterizedName);
      if (source.Count<IEdmOperationImport>() == 0)
        return (IEdmOperationImport) null;
      return source.Count<IEdmOperationImport>() == 1 ? source.First<IEdmOperationImport>() : ambiguityCreator(source);
    }

    private IEdmOperation FindParameterizedOperation(
      string parameterizedName,
      Func<string, IEnumerable<IEdmOperation>> findFunctions,
      Func<IEnumerable<IEdmOperation>, IEdmOperation> ambiguityCreator)
    {
      int length = parameterizedName.IndexOf('(');
      int num = parameterizedName.LastIndexOf(')');
      if (length < 0)
        return (IEdmOperation) null;
      string str = parameterizedName.Substring(0, length);
      string[] parameters = parameterizedName.Substring(length + 1, num - (length + 1)).Split(new string[1]
      {
        ", "
      }, StringSplitOptions.RemoveEmptyEntries);
      IEnumerable<IEdmOperation> operationFromList = this.FindParameterizedOperationFromList(findFunctions(str).Cast<IEdmOperation>(), parameters);
      if (operationFromList.Count<IEdmOperation>() == 0)
        return (IEdmOperation) null;
      return operationFromList.Count<IEdmOperation>() == 1 ? operationFromList.First<IEdmOperation>() : ambiguityCreator(operationFromList);
    }

    private IEdmOperationImport CreateAmbiguousOperationImport(
      IEnumerable<IEdmOperationImport> operations)
    {
      IEnumerator<IEdmOperationImport> enumerator = operations.GetEnumerator();
      enumerator.MoveNext();
      IEdmOperationImport current1 = enumerator.Current;
      enumerator.MoveNext();
      IEdmOperationImport current2 = enumerator.Current;
      AmbiguousOperationImportBinding ambiguousOperationImport = new AmbiguousOperationImportBinding(current1, current2);
      while (enumerator.MoveNext())
        ambiguousOperationImport.AddBinding(enumerator.Current);
      return (IEdmOperationImport) ambiguousOperationImport;
    }

    private IEdmOperation CreateAmbiguousOperation(IEnumerable<IEdmOperation> operations)
    {
      IEnumerator<IEdmOperation> enumerator = operations.GetEnumerator();
      enumerator.MoveNext();
      IEdmOperation current1 = enumerator.Current;
      enumerator.MoveNext();
      IEdmOperation current2 = enumerator.Current;
      AmbiguousOperationBinding ambiguousOperation = new AmbiguousOperationBinding(current1, current2);
      while (enumerator.MoveNext())
        ambiguousOperation.AddBinding(enumerator.Current);
      return (IEdmOperation) ambiguousOperation;
    }

    private IEnumerable<IEdmOperation> FindParameterizedOperationFromList(
      IEnumerable<IEdmOperation> operations,
      string[] parameters)
    {
      List<IEdmOperation> operationFromList = new List<IEdmOperation>();
      foreach (IEdmOperation operation in operations)
      {
        if (operation.Parameters.Count<IEdmOperationParameter>() == ((IEnumerable<string>) parameters).Count<string>())
        {
          bool flag = true;
          IEnumerator<string> enumerator = ((IEnumerable<string>) parameters).GetEnumerator();
          foreach (IEdmOperationParameter parameter in operation.Parameters)
          {
            enumerator.MoveNext();
            string[] strArray = enumerator.Current.Split('(', ')');
            switch (strArray[0])
            {
              case "Collection":
                flag = parameter.Type.IsCollection() && this.Schema.FindType(strArray[1]).IsEquivalentTo(parameter.Type.AsCollection().ElementType().Definition);
                break;
              case "Ref":
                flag = parameter.Type.IsEntityReference() && this.Schema.FindType(strArray[1]).IsEquivalentTo((IEdmType) parameter.Type.AsEntityReference().EntityType());
                break;
              default:
                flag = EdmCoreModel.Instance.FindDeclaredType(enumerator.Current).IsEquivalentTo(parameter.Type.Definition) || this.Schema.FindType(enumerator.Current).IsEquivalentTo(parameter.Type.Definition);
                break;
            }
            if (!flag)
              break;
          }
          if (flag)
            operationFromList.Add(operation);
        }
      }
      return (IEnumerable<IEdmOperation>) operationFromList;
    }
  }
}
