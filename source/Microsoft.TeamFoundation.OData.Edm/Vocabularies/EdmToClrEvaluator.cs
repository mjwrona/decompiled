// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmToClrEvaluator
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmToClrEvaluator : EdmExpressionEvaluator
  {
    private EdmToClrConverter edmToClrConverter = new EdmToClrConverter();

    public EdmToClrEvaluator(
      IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions)
      : base(builtInFunctions)
    {
    }

    public EdmToClrEvaluator(
      IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions,
      Func<string, IEdmValue[], IEdmValue> lastChanceOperationApplier)
      : base(builtInFunctions, lastChanceOperationApplier)
    {
    }

    public EdmToClrEvaluator(
      IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions,
      Func<string, IEdmValue[], IEdmValue> lastChanceOperationApplier,
      Func<IEdmModel, IEdmType, string, string, IEdmExpression> getAnnotationExpressionForType,
      Func<IEdmModel, IEdmType, string, string, string, IEdmExpression> getAnnotationExpressionForProperty,
      IEdmModel edmModel)
      : base(builtInFunctions, lastChanceOperationApplier, getAnnotationExpressionForType, getAnnotationExpressionForProperty, edmModel)
    {
      this.ResolveTypeFromName = new Func<string, IEdmModel, IEdmType>(this.ResolveEdmTypeFromName);
    }

    public EdmToClrConverter EdmToClrConverter
    {
      get => this.edmToClrConverter;
      set
      {
        EdmUtil.CheckArgumentNull<EdmToClrConverter>(value, nameof (value));
        this.edmToClrConverter = value;
      }
    }

    public T EvaluateToClrValue<T>(IEdmExpression expression) => this.edmToClrConverter.AsClrValue<T>(this.Evaluate(expression));

    public T EvaluateToClrValue<T>(IEdmExpression expression, IEdmStructuredValue context) => this.edmToClrConverter.AsClrValue<T>(this.Evaluate(expression, context));

    public T EvaluateToClrValue<T>(
      IEdmExpression expression,
      IEdmStructuredValue context,
      IEdmTypeReference targetType)
    {
      return this.edmToClrConverter.AsClrValue<T>(this.Evaluate(expression, context, targetType));
    }

    internal IEdmType ResolveEdmTypeFromName(string edmTypeName, IEdmModel edmModel)
    {
      string clrTypeName = (string) null;
      return this.edmToClrConverter.TryGetClrTypeNameDelegate(edmModel, edmTypeName, out clrTypeName) ? EdmExpressionEvaluator.FindEdmType(clrTypeName, edmModel) : (IEdmType) null;
    }
  }
}
