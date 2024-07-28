// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsOperation
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal abstract class CsdlSemanticsOperation : 
    CsdlSemanticsElement,
    IEdmOperation,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmFullNamedElement
  {
    private readonly string fullName;
    private readonly CsdlOperation operation;
    private readonly Cache<CsdlSemanticsOperation, IEdmPathExpression> entitySetPathCache = new Cache<CsdlSemanticsOperation, IEdmPathExpression>();
    private static readonly Func<CsdlSemanticsOperation, IEdmPathExpression> ComputeEntitySetPathFunc = (Func<CsdlSemanticsOperation, IEdmPathExpression>) (me => me.ComputeEntitySetPath());
    private readonly Cache<CsdlSemanticsOperation, IEdmOperationReturn> returnCache = new Cache<CsdlSemanticsOperation, IEdmOperationReturn>();
    private static readonly Func<CsdlSemanticsOperation, IEdmOperationReturn> ComputeReturnFunc = (Func<CsdlSemanticsOperation, IEdmOperationReturn>) (me => me.ComputeReturn());
    private readonly Cache<CsdlSemanticsOperation, IEnumerable<IEdmOperationParameter>> parametersCache = new Cache<CsdlSemanticsOperation, IEnumerable<IEdmOperationParameter>>();
    private static readonly Func<CsdlSemanticsOperation, IEnumerable<IEdmOperationParameter>> ComputeParametersFunc = (Func<CsdlSemanticsOperation, IEnumerable<IEdmOperationParameter>>) (me => me.ComputeParameters());

    public CsdlSemanticsOperation(CsdlSemanticsSchema context, CsdlOperation operation)
      : base((CsdlElement) operation)
    {
      this.Context = context;
      this.operation = operation;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(this.Context?.Namespace, this.operation?.Name);
    }

    public abstract EdmSchemaElementKind SchemaElementKind { get; }

    public override CsdlSemanticsModel Model => this.Context.Model;

    public string Name => this.operation.Name;

    public string FullName => this.fullName;

    public override CsdlElement Element => (CsdlElement) this.operation;

    public string Namespace => this.Context.Namespace;

    public bool IsBound => this.operation.IsBound;

    public IEdmPathExpression EntitySetPath => this.entitySetPathCache.GetValue(this, CsdlSemanticsOperation.ComputeEntitySetPathFunc, (Func<CsdlSemanticsOperation, IEdmPathExpression>) null);

    public IEdmTypeReference ReturnType => this.operation.Return == null ? (IEdmTypeReference) null : this.Return.Type;

    public IEdmOperationReturn Return => this.returnCache.GetValue(this, CsdlSemanticsOperation.ComputeReturnFunc, (Func<CsdlSemanticsOperation, IEdmOperationReturn>) null);

    public IEnumerable<IEdmOperationParameter> Parameters => this.parametersCache.GetValue(this, CsdlSemanticsOperation.ComputeParametersFunc, (Func<CsdlSemanticsOperation, IEnumerable<IEdmOperationParameter>>) null);

    public CsdlSemanticsSchema Context { get; private set; }

    public IEdmOperationParameter FindParameter(string name) => this.Parameters.SingleOrDefault<IEdmOperationParameter>((Func<IEdmOperationParameter, bool>) (p => p.Name == name));

    protected override IEnumerable<IEdmVocabularyAnnotation> ComputeInlineVocabularyAnnotations() => this.Model.WrapInlineVocabularyAnnotations((CsdlSemanticsElement) this, this.Context);

    private IEdmPathExpression ComputeEntitySetPath()
    {
      if (this.operation.EntitySetPath == null)
        return (IEdmPathExpression) null;
      return (IEdmPathExpression) new CsdlSemanticsOperation.OperationPathExpression(this.operation.EntitySetPath)
      {
        Location = this.Location
      };
    }

    private IEdmOperationReturn ComputeReturn() => this.operation.Return == null ? (IEdmOperationReturn) null : (IEdmOperationReturn) new CsdlSemanticsOperationReturn(this, this.operation.Return);

    private IEnumerable<IEdmOperationParameter> ComputeParameters()
    {
      List<IEdmOperationParameter> parameters1 = new List<IEdmOperationParameter>();
      foreach (CsdlOperationParameter parameter in this.operation.Parameters)
      {
        if (parameter.IsOptional)
          parameters1.Add((IEdmOperationParameter) new CsdlSemanticsOptionalParameter(this, parameter, parameter.DefaultValue));
        else
          parameters1.Add((IEdmOperationParameter) new CsdlSemanticsOperationParameter(this, parameter));
      }
      string str1 = this.Namespace + "." + this.Name;
      string str2 = CsdlSemanticsOperation.ParameterizedTargetName((IList<IEdmOperationParameter>) parameters1);
      List<IEdmOperationParameter> parameters2 = new List<IEdmOperationParameter>(parameters1.Count);
      foreach (IEdmOperationParameter operationParameter1 in parameters1)
      {
        string defaultValue;
        if (this.TryGetOptionalParameterOutOfLineAnnotation(str1 + str2 + "/" + operationParameter1.Name, str1 + "/" + operationParameter1.Name, out defaultValue))
        {
          CsdlSemanticsOperationParameter operationParameter2 = (CsdlSemanticsOperationParameter) operationParameter1;
          parameters2.Add((IEdmOperationParameter) new CsdlSemanticsOptionalParameter(this, (CsdlOperationParameter) operationParameter2.Element, defaultValue));
        }
        else
          parameters2.Add(operationParameter1);
      }
      return (IEnumerable<IEdmOperationParameter>) parameters2;
    }

    private bool TryGetOptionalParameterOutOfLineAnnotation(
      string fullTargetName,
      string targetName,
      out string defaultValue)
    {
      defaultValue = (string) null;
      bool ofLineAnnotation = false;
      List<CsdlSemanticsAnnotations> semanticsAnnotationsList;
      if (this.Model.OutOfLineAnnotations.TryGetValue(fullTargetName, out semanticsAnnotationsList) || this.Model.OutOfLineAnnotations.TryGetValue(targetName, out semanticsAnnotationsList))
      {
        foreach (CsdlSemanticsAnnotations semanticsAnnotations in semanticsAnnotationsList)
        {
          CsdlAnnotation csdlAnnotation = semanticsAnnotations.Annotations.Annotations.FirstOrDefault<CsdlAnnotation>((Func<CsdlAnnotation, bool>) (a => a.Term == CoreVocabularyModel.OptionalParameterTerm.ShortQualifiedName() || a.Term == CoreVocabularyModel.OptionalParameterTerm.FullName()));
          if (csdlAnnotation != null)
          {
            ofLineAnnotation = true;
            if (csdlAnnotation.Expression is CsdlRecordExpression expression1)
            {
              using (IEnumerator<CsdlPropertyValue> enumerator = expression1.PropertyValues.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  CsdlPropertyValue current = enumerator.Current;
                  if (current.Property == "DefaultValue" && current.Expression is CsdlConstantExpression expression)
                    defaultValue = expression.Value;
                }
                break;
              }
            }
            else
              break;
          }
        }
      }
      return ofLineAnnotation;
    }

    internal static string ParameterizedTargetName(IList<IEdmOperationParameter> parameters)
    {
      int num1 = 0;
      int num2 = parameters.Count<IEdmOperationParameter>();
      StringBuilder stringBuilder = new StringBuilder("(");
      foreach (IEdmOperationParameter parameter in (IEnumerable<IEdmOperationParameter>) parameters)
      {
        string str = parameter.Type != null ? (!parameter.Type.IsCollection() ? (!parameter.Type.IsEntityReference() ? parameter.Type.FullName() : "Ref(" + parameter.Type.AsEntityReference().EntityType().FullName() + ")") : "Collection(" + parameter.Type.AsCollection().ElementType().FullName() + ")") : "Edm.Untyped";
        stringBuilder.Append(str);
        ++num1;
        if (num1 < num2)
          stringBuilder.Append(", ");
      }
      stringBuilder.Append(")");
      return stringBuilder.ToString();
    }

    private sealed class OperationPathExpression : EdmPathExpression, IEdmLocatable
    {
      internal OperationPathExpression(string path)
        : base(path)
      {
      }

      public EdmLocation Location { get; set; }
    }
  }
}
