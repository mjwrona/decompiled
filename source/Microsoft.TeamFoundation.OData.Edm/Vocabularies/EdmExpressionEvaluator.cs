// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmExpressionEvaluator
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmExpressionEvaluator
  {
    private readonly IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions;
    private readonly Dictionary<IEdmLabeledExpression, EdmExpressionEvaluator.DelayedValue> labeledValues = new Dictionary<IEdmLabeledExpression, EdmExpressionEvaluator.DelayedValue>();
    private readonly Func<string, IEdmValue[], IEdmValue> lastChanceOperationApplier;
    private readonly Func<IEdmModel, IEdmType, string, string, IEdmExpression> getAnnotationExpressionForType;
    private readonly Func<IEdmModel, IEdmType, string, string, string, IEdmExpression> getAnnotationExpressionForProperty;
    private readonly IEdmModel edmModel;
    private Func<string, IEdmModel, IEdmType> resolveTypeFromName = (Func<string, IEdmModel, IEdmType>) ((typeName, edmModel) => EdmExpressionEvaluator.FindEdmType(typeName, edmModel));

    public EdmExpressionEvaluator(
      IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions)
    {
      this.builtInFunctions = builtInFunctions;
    }

    public EdmExpressionEvaluator(
      IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions,
      Func<string, IEdmValue[], IEdmValue> lastChanceOperationApplier)
      : this(builtInFunctions)
    {
      this.lastChanceOperationApplier = lastChanceOperationApplier;
    }

    public EdmExpressionEvaluator(
      IDictionary<IEdmOperation, Func<IEdmValue[], IEdmValue>> builtInFunctions,
      Func<string, IEdmValue[], IEdmValue> lastChanceOperationApplier,
      Func<IEdmModel, IEdmType, string, string, IEdmExpression> getAnnotationExpressionForType,
      Func<IEdmModel, IEdmType, string, string, string, IEdmExpression> getAnnotationExpressionForProperty,
      IEdmModel edmModel)
      : this(builtInFunctions, lastChanceOperationApplier)
    {
      this.getAnnotationExpressionForType = getAnnotationExpressionForType;
      this.getAnnotationExpressionForProperty = getAnnotationExpressionForProperty;
      this.edmModel = edmModel;
    }

    protected Func<string, IEdmModel, IEdmType> ResolveTypeFromName
    {
      get => this.resolveTypeFromName;
      set => this.resolveTypeFromName = value;
    }

    public IEdmValue Evaluate(IEdmExpression expression)
    {
      EdmUtil.CheckArgumentNull<IEdmExpression>(expression, nameof (expression));
      return this.Eval(expression, (IEdmStructuredValue) null);
    }

    public IEdmValue Evaluate(IEdmExpression expression, IEdmStructuredValue context)
    {
      EdmUtil.CheckArgumentNull<IEdmExpression>(expression, nameof (expression));
      return this.Eval(expression, context);
    }

    public IEdmValue Evaluate(
      IEdmExpression expression,
      IEdmStructuredValue context,
      IEdmTypeReference targetType)
    {
      EdmUtil.CheckArgumentNull<IEdmExpression>(expression, nameof (expression));
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(targetType, nameof (targetType));
      return EdmExpressionEvaluator.Cast(targetType, this.Eval(expression, context));
    }

    protected static IEdmType FindEdmType(string edmTypeName, IEdmModel edmModel) => (IEdmType) edmModel.FindDeclaredType(edmTypeName);

    private static bool InRange(long value, long min, long max) => value >= min && value <= max;

    private static bool FitsInSingle(double value) => value >= -3.4028234663852886E+38 && value <= 3.4028234663852886E+38;

    private static bool MatchesType(IEdmTypeReference targetType, IEdmValue operand) => EdmExpressionEvaluator.MatchesType(targetType, operand, true);

    private static bool MatchesType(
      IEdmTypeReference targetType,
      IEdmValue operand,
      bool testPropertyTypes)
    {
      IEdmTypeReference type = operand.Type;
      EdmValueKind valueKind = operand.ValueKind;
      if (type != null && valueKind != EdmValueKind.Null && type.Definition.IsOrInheritsFrom(targetType.Definition))
        return true;
      switch (valueKind)
      {
        case EdmValueKind.Binary:
          if (targetType.IsBinary())
          {
            IEdmBinaryTypeReference binaryTypeReference = targetType.AsBinary();
            return binaryTypeReference.IsUnbounded || !binaryTypeReference.MaxLength.HasValue || binaryTypeReference.MaxLength.Value >= ((IEdmBinaryValue) operand).Value.Length;
          }
          break;
        case EdmValueKind.Boolean:
          return targetType.IsBoolean();
        case EdmValueKind.Collection:
          if (targetType.IsCollection())
          {
            IEdmTypeReference targetType1 = targetType.AsCollection().ElementType();
            foreach (IEdmDelayedValue element in ((IEdmCollectionValue) operand).Elements)
            {
              if (!EdmExpressionEvaluator.MatchesType(targetType1, element.Value))
                return false;
            }
            return true;
          }
          break;
        case EdmValueKind.DateTimeOffset:
          return targetType.IsDateTimeOffset();
        case EdmValueKind.Decimal:
          return targetType.IsDecimal();
        case EdmValueKind.Enum:
          return operand.Type.Definition.IsEquivalentTo(targetType.Definition);
        case EdmValueKind.Floating:
          if (targetType.IsDouble())
            return true;
          return targetType.IsSingle() && EdmExpressionEvaluator.FitsInSingle(((IEdmFloatingValue) operand).Value);
        case EdmValueKind.Guid:
          return targetType.IsGuid();
        case EdmValueKind.Integer:
          if (targetType.TypeKind() == EdmTypeKind.Primitive)
          {
            switch (ExtensionMethods.PrimitiveKind(targetType.AsPrimitive()))
            {
              case EdmPrimitiveTypeKind.Byte:
                return EdmExpressionEvaluator.InRange(((IEdmIntegerValue) operand).Value, 0L, (long) byte.MaxValue);
              case EdmPrimitiveTypeKind.Double:
              case EdmPrimitiveTypeKind.Int64:
              case EdmPrimitiveTypeKind.Single:
                return true;
              case EdmPrimitiveTypeKind.Int16:
                return EdmExpressionEvaluator.InRange(((IEdmIntegerValue) operand).Value, (long) short.MinValue, (long) short.MaxValue);
              case EdmPrimitiveTypeKind.Int32:
                return EdmExpressionEvaluator.InRange(((IEdmIntegerValue) operand).Value, (long) int.MinValue, (long) int.MaxValue);
              case EdmPrimitiveTypeKind.SByte:
                return EdmExpressionEvaluator.InRange(((IEdmIntegerValue) operand).Value, (long) sbyte.MinValue, (long) sbyte.MaxValue);
            }
          }
          else
            break;
          break;
        case EdmValueKind.Null:
          return targetType.IsNullable;
        case EdmValueKind.String:
          if (targetType.IsString())
          {
            IEdmStringTypeReference stringTypeReference = targetType.AsString();
            return stringTypeReference.IsUnbounded || !stringTypeReference.MaxLength.HasValue || stringTypeReference.MaxLength.Value >= ((IEdmStringValue) operand).Value.Length;
          }
          break;
        case EdmValueKind.Structured:
          if (targetType.IsStructured())
            return EdmExpressionEvaluator.AssertOrMatchStructuredType(targetType.AsStructured(), (IEdmStructuredValue) operand, testPropertyTypes, (List<IEdmPropertyValue>) null);
          break;
        case EdmValueKind.Duration:
          return targetType.IsDuration();
        case EdmValueKind.Date:
          return targetType.IsDate();
        case EdmValueKind.TimeOfDay:
          return targetType.IsTimeOfDay();
      }
      return false;
    }

    private static IEdmValue Cast(IEdmTypeReference targetType, IEdmValue operand)
    {
      IEdmTypeReference type = operand.Type;
      EdmValueKind valueKind = operand.ValueKind;
      if (type != null && valueKind != EdmValueKind.Null && type.Definition.IsOrInheritsFrom(targetType.Definition) || targetType.TypeKind() == EdmTypeKind.None)
        return operand;
      bool flag;
      switch (valueKind)
      {
        case EdmValueKind.Collection:
          if (targetType.IsCollection())
            return (IEdmValue) new EdmExpressionEvaluator.CastCollectionValue(targetType.AsCollection(), (IEdmCollectionValue) operand);
          flag = false;
          break;
        case EdmValueKind.Structured:
          if (targetType.IsStructured())
          {
            IEdmStructuredTypeReference structuredTypeReference = targetType.AsStructured();
            List<IEdmPropertyValue> edmPropertyValueList = new List<IEdmPropertyValue>();
            flag = EdmExpressionEvaluator.AssertOrMatchStructuredType(structuredTypeReference, (IEdmStructuredValue) operand, true, edmPropertyValueList);
            if (flag)
              return (IEdmValue) new EdmStructuredValue(structuredTypeReference, (IEnumerable<IEdmPropertyValue>) edmPropertyValueList);
            break;
          }
          flag = false;
          break;
        default:
          flag = EdmExpressionEvaluator.MatchesType(targetType, operand);
          break;
      }
      if (!flag)
        throw new InvalidOperationException(Strings.Edm_Evaluator_FailedTypeAssertion((object) targetType.ToTraceString()));
      return operand;
    }

    private static bool AssertOrMatchStructuredType(
      IEdmStructuredTypeReference structuredTargetType,
      IEdmStructuredValue structuredValue,
      bool testPropertyTypes,
      List<IEdmPropertyValue> newProperties)
    {
      IEdmTypeReference type = structuredValue.Type;
      if (type != null && !structuredTargetType.StructuredDefinition().InheritsFrom(type.AsStructured().StructuredDefinition()))
        return false;
      HashSetInternal<IEdmPropertyValue> hashSetInternal = new HashSetInternal<IEdmPropertyValue>();
      foreach (IEdmProperty structuralProperty in structuredTargetType.StructuralProperties())
      {
        IEdmPropertyValue propertyValue = structuredValue.FindPropertyValue(structuralProperty.Name);
        if (propertyValue == null)
          return false;
        hashSetInternal.Add(propertyValue);
        if (testPropertyTypes)
        {
          if (newProperties != null)
            newProperties.Add((IEdmPropertyValue) new EdmPropertyValue(propertyValue.Name, EdmExpressionEvaluator.Cast(structuralProperty.Type, propertyValue.Value)));
          else if (!EdmExpressionEvaluator.MatchesType(structuralProperty.Type, propertyValue.Value))
            return false;
        }
      }
      if (structuredTargetType.IsEntity())
      {
        foreach (IEdmNavigationProperty navigationProperty in structuredTargetType.AsEntity().NavigationProperties())
        {
          IEdmPropertyValue propertyValue = structuredValue.FindPropertyValue(navigationProperty.Name);
          if (propertyValue == null || testPropertyTypes && !EdmExpressionEvaluator.MatchesType(navigationProperty.Type, propertyValue.Value, false))
            return false;
          hashSetInternal.Add(propertyValue);
          newProperties?.Add(propertyValue);
        }
      }
      if (newProperties != null)
      {
        foreach (IEdmPropertyValue propertyValue in structuredValue.PropertyValues)
        {
          if (!hashSetInternal.Contains(propertyValue))
            newProperties.Add(propertyValue);
        }
      }
      return true;
    }

    private IEdmValue Eval(IEdmExpression expression, IEdmStructuredValue context)
    {
      switch (expression.ExpressionKind)
      {
        case EdmExpressionKind.BinaryConstant:
          return (IEdmValue) expression;
        case EdmExpressionKind.BooleanConstant:
          return (IEdmValue) expression;
        case EdmExpressionKind.DateTimeOffsetConstant:
          return (IEdmValue) expression;
        case EdmExpressionKind.DecimalConstant:
          return (IEdmValue) expression;
        case EdmExpressionKind.FloatingConstant:
          return (IEdmValue) expression;
        case EdmExpressionKind.GuidConstant:
          return (IEdmValue) expression;
        case EdmExpressionKind.IntegerConstant:
          return (IEdmValue) expression;
        case EdmExpressionKind.StringConstant:
          return (IEdmValue) expression;
        case EdmExpressionKind.DurationConstant:
          return (IEdmValue) expression;
        case EdmExpressionKind.Null:
          return (IEdmValue) expression;
        case EdmExpressionKind.Record:
          IEdmRecordExpression recordExpression = (IEdmRecordExpression) expression;
          EdmExpressionEvaluator.DelayedExpressionContext context1 = new EdmExpressionEvaluator.DelayedExpressionContext(this, context);
          List<IEdmPropertyValue> propertyValues = new List<IEdmPropertyValue>();
          foreach (IEdmPropertyConstructor property in recordExpression.Properties)
            propertyValues.Add((IEdmPropertyValue) new EdmExpressionEvaluator.DelayedRecordProperty(context1, property));
          return (IEdmValue) new EdmStructuredValue(recordExpression.DeclaredType != null ? recordExpression.DeclaredType.AsStructured() : (IEdmStructuredTypeReference) null, (IEnumerable<IEdmPropertyValue>) propertyValues);
        case EdmExpressionKind.Collection:
          IEdmCollectionExpression collectionExpression = (IEdmCollectionExpression) expression;
          EdmExpressionEvaluator.DelayedExpressionContext delayedContext = new EdmExpressionEvaluator.DelayedExpressionContext(this, context);
          List<IEdmDelayedValue> elements1 = new List<IEdmDelayedValue>();
          foreach (IEdmExpression element in collectionExpression.Elements)
            elements1.Add(this.MapLabeledExpressionToDelayedValue(element, delayedContext, context));
          return (IEdmValue) new EdmCollectionValue(collectionExpression.DeclaredType != null ? collectionExpression.DeclaredType.AsCollection() : (IEdmCollectionTypeReference) null, (IEnumerable<IEdmDelayedValue>) elements1);
        case EdmExpressionKind.Path:
          if (context == null)
            throw new InvalidOperationException(Strings.Edm_Evaluator_NoContextPath);
          IEdmPathExpression edmPathExpression1 = (IEdmPathExpression) expression;
          IEdmValue context2 = (IEdmValue) context;
          foreach (string pathSegment in edmPathExpression1.PathSegments)
          {
            if (pathSegment.Contains("@"))
            {
              string[] strArray1 = pathSegment.Split('@');
              string str1 = strArray1[0];
              string str2 = strArray1[1];
              IEdmExpression expression1 = (IEdmExpression) null;
              if (!string.IsNullOrWhiteSpace(str2))
              {
                string[] strArray2 = str2.Split('#');
                if (strArray2.Length <= 2)
                {
                  string str3 = strArray2[0];
                  string str4 = strArray2.Length == 2 ? strArray2[1] : (string) null;
                  if (string.IsNullOrWhiteSpace(str1) && this.getAnnotationExpressionForType != null)
                    expression1 = this.getAnnotationExpressionForType(this.edmModel, context.Type.Definition, str3, str4);
                  else if (!string.IsNullOrWhiteSpace(str1) && this.getAnnotationExpressionForProperty != null)
                    expression1 = this.getAnnotationExpressionForProperty(this.edmModel, context.Type.Definition, str1, str3, str4);
                }
              }
              if (expression1 == null)
              {
                context2 = (IEdmValue) null;
                break;
              }
              context2 = this.Eval(expression1, context);
            }
            else if (pathSegment == "$count")
            {
              if (context2 is IEdmCollectionValue edmCollectionValue)
              {
                context2 = (IEdmValue) new EdmIntegerConstant((long) edmCollectionValue.Elements.Count<IEdmDelayedValue>());
              }
              else
              {
                context2 = (IEdmValue) null;
                break;
              }
            }
            else
            {
              if (pathSegment.Contains("."))
              {
                if (this.edmModel == null)
                  throw new InvalidOperationException(Strings.Edm_Evaluator_TypeCastNeedsEdmModel);
                IEdmType edmType = this.resolveTypeFromName(pathSegment, this.edmModel);
                if (edmType == null)
                {
                  context2 = (IEdmValue) null;
                  break;
                }
                IEdmTypeReference type = context2.Type;
                EdmValueKind valueKind = context2.ValueKind;
                switch (valueKind)
                {
                  case EdmValueKind.Collection:
                    List<IEdmDelayedValue> elements2 = new List<IEdmDelayedValue>();
                    foreach (IEdmDelayedValue element in (context2 as IEdmCollectionValue).Elements)
                    {
                      if (element.Value.Type.Definition.IsOrInheritsFrom(edmType))
                        elements2.Add(element);
                    }
                    context2 = (IEdmValue) new EdmCollectionValue((IEdmCollectionTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) new EdmCollectionType(edmType.GetTypeReference(false))), (IEnumerable<IEdmDelayedValue>) elements2);
                    continue;
                  case EdmValueKind.Structured:
                    if (valueKind != EdmValueKind.Structured || type.Definition.IsOrInheritsFrom(edmType))
                      continue;
                    break;
                }
                context2 = (IEdmValue) null;
                break;
              }
              context2 = EdmExpressionEvaluator.FindProperty(pathSegment, context2);
              if (context2 == null)
                throw new InvalidOperationException(Strings.Edm_Evaluator_UnboundPath((object) pathSegment));
            }
          }
          return context2;
        case EdmExpressionKind.If:
          IEdmIfExpression edmIfExpression = (IEdmIfExpression) expression;
          return ((IEdmBooleanValue) this.Eval(edmIfExpression.TestExpression, context)).Value ? this.Eval(edmIfExpression.TrueExpression, context) : this.Eval(edmIfExpression.FalseExpression, context);
        case EdmExpressionKind.Cast:
          IEdmCastExpression edmCastExpression = (IEdmCastExpression) expression;
          IEdmValue operand1 = this.Eval(edmCastExpression.Operand, context);
          return EdmExpressionEvaluator.Cast(edmCastExpression.Type, operand1);
        case EdmExpressionKind.IsType:
          IEdmIsTypeExpression isTypeExpression = (IEdmIsTypeExpression) expression;
          IEdmValue operand2 = this.Eval(isTypeExpression.Operand, context);
          return (IEdmValue) new EdmBooleanConstant(EdmExpressionEvaluator.MatchesType(isTypeExpression.Type, operand2));
        case EdmExpressionKind.FunctionApplication:
          IEdmApplyExpression edmApplyExpression = (IEdmApplyExpression) expression;
          IEdmFunction appliedFunction = edmApplyExpression.AppliedFunction;
          if (appliedFunction != null)
          {
            IList<IEdmExpression> list = (IList<IEdmExpression>) edmApplyExpression.Arguments.ToList<IEdmExpression>();
            IEdmValue[] edmValueArray = new IEdmValue[list.Count<IEdmExpression>()];
            int num = 0;
            foreach (IEdmExpression expression2 in (IEnumerable<IEdmExpression>) list)
              edmValueArray[num++] = this.Eval(expression2, context);
            Func<IEdmValue[], IEdmValue> func;
            if (this.builtInFunctions.TryGetValue((IEdmOperation) appliedFunction, out func))
              return func(edmValueArray);
            if (this.lastChanceOperationApplier != null)
              return this.lastChanceOperationApplier(appliedFunction.FullName(), edmValueArray);
          }
          throw new InvalidOperationException(Strings.Edm_Evaluator_UnboundFunction(appliedFunction != null ? (object) appliedFunction.ToTraceString() : (object) string.Empty));
        case EdmExpressionKind.LabeledExpressionReference:
          return this.MapLabeledExpressionToDelayedValue((IEdmExpression) ((IEdmLabeledExpressionReferenceExpression) expression).ReferencedLabeledExpression, (EdmExpressionEvaluator.DelayedExpressionContext) null, context).Value;
        case EdmExpressionKind.Labeled:
          return this.MapLabeledExpressionToDelayedValue(expression, new EdmExpressionEvaluator.DelayedExpressionContext(this, context), context).Value;
        case EdmExpressionKind.PropertyPath:
        case EdmExpressionKind.NavigationPropertyPath:
          EdmUtil.CheckArgumentNull<IEdmStructuredValue>(context, nameof (context));
          IEdmPathExpression edmPathExpression2 = (IEdmPathExpression) expression;
          IEdmValue context3 = (IEdmValue) context;
          foreach (string pathSegment in edmPathExpression2.PathSegments)
          {
            context3 = EdmExpressionEvaluator.FindProperty(pathSegment, context3);
            if (context3 == null)
              throw new InvalidOperationException(Strings.Edm_Evaluator_UnboundPath((object) pathSegment));
          }
          return context3;
        case EdmExpressionKind.DateConstant:
          return (IEdmValue) expression;
        case EdmExpressionKind.TimeOfDayConstant:
          return (IEdmValue) expression;
        case EdmExpressionKind.EnumMember:
          IEdmEnumMemberExpression memberExpression = (IEdmEnumMemberExpression) expression;
          List<IEdmEnumMember> list1 = memberExpression.EnumMembers.ToList<IEdmEnumMember>();
          IEdmEnumType declaringType = list1.First<IEdmEnumMember>().DeclaringType;
          IEdmEnumTypeReference type1 = (IEdmEnumTypeReference) new EdmEnumTypeReference(declaringType, false);
          if (list1.Count<IEdmEnumMember>() == 1)
            return (IEdmValue) new EdmEnumValue(type1, memberExpression.EnumMembers.Single<IEdmEnumMember>());
          if (!declaringType.IsFlags || !EdmEnumValueParser.IsEnumIntegerType(declaringType))
            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Type {0} cannot be assigned with multi-values.", new object[1]
            {
              (object) declaringType.FullName()
            }));
          long num1 = 0;
          foreach (IEdmEnumMember edmEnumMember in list1)
          {
            long num2 = edmEnumMember.Value.Value;
            num1 |= num2;
          }
          return (IEdmValue) new EdmEnumValue(type1, (IEdmEnumMemberValue) new EdmEnumMemberValue(num1));
        default:
          throw new InvalidOperationException(Strings.Edm_Evaluator_UnrecognizedExpressionKind((object) ((int) expression.ExpressionKind).ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      }
    }

    private IEdmDelayedValue MapLabeledExpressionToDelayedValue(
      IEdmExpression expression,
      EdmExpressionEvaluator.DelayedExpressionContext delayedContext,
      IEdmStructuredValue context)
    {
      if (!(expression is IEdmLabeledExpression key))
        return (IEdmDelayedValue) new EdmExpressionEvaluator.DelayedCollectionElement(delayedContext, expression);
      EdmExpressionEvaluator.DelayedValue delayedValue1;
      if (this.labeledValues.TryGetValue(key, out delayedValue1))
        return (IEdmDelayedValue) delayedValue1;
      EdmExpressionEvaluator.DelayedValue delayedValue2 = (EdmExpressionEvaluator.DelayedValue) new EdmExpressionEvaluator.DelayedCollectionElement(delayedContext ?? new EdmExpressionEvaluator.DelayedExpressionContext(this, context), key.Expression);
      this.labeledValues[key] = delayedValue2;
      return (IEdmDelayedValue) delayedValue2;
    }

    private static IEdmValue FindProperty(string name, IEdmValue context)
    {
      IEdmValue property = (IEdmValue) null;
      if (context is IEdmStructuredValue edmStructuredValue)
      {
        IEdmPropertyValue propertyValue = edmStructuredValue.FindPropertyValue(name);
        if (propertyValue != null)
          property = propertyValue.Value;
      }
      return property;
    }

    private class DelayedExpressionContext
    {
      private readonly EdmExpressionEvaluator expressionEvaluator;
      private readonly IEdmStructuredValue context;

      public DelayedExpressionContext(
        EdmExpressionEvaluator expressionEvaluator,
        IEdmStructuredValue context)
      {
        this.expressionEvaluator = expressionEvaluator;
        this.context = context;
      }

      public IEdmValue Eval(IEdmExpression expression) => this.expressionEvaluator.Eval(expression, this.context);
    }

    private abstract class DelayedValue : IEdmDelayedValue
    {
      private readonly EdmExpressionEvaluator.DelayedExpressionContext context;
      private IEdmValue value;

      public DelayedValue(
        EdmExpressionEvaluator.DelayedExpressionContext context)
      {
        this.context = context;
      }

      public abstract IEdmExpression Expression { get; }

      public IEdmValue Value
      {
        get
        {
          if (this.value == null)
            this.value = this.context.Eval(this.Expression);
          return this.value;
        }
      }
    }

    private class DelayedRecordProperty : 
      EdmExpressionEvaluator.DelayedValue,
      IEdmPropertyValue,
      IEdmDelayedValue
    {
      private readonly IEdmPropertyConstructor constructor;

      public DelayedRecordProperty(
        EdmExpressionEvaluator.DelayedExpressionContext context,
        IEdmPropertyConstructor constructor)
        : base(context)
      {
        this.constructor = constructor;
      }

      public string Name => this.constructor.Name;

      public override IEdmExpression Expression => this.constructor.Value;
    }

    private class DelayedCollectionElement : EdmExpressionEvaluator.DelayedValue
    {
      private readonly IEdmExpression expression;

      public DelayedCollectionElement(
        EdmExpressionEvaluator.DelayedExpressionContext context,
        IEdmExpression expression)
        : base(context)
      {
        this.expression = expression;
      }

      public override IEdmExpression Expression => this.expression;
    }

    private class CastCollectionValue : 
      EdmElement,
      IEdmCollectionValue,
      IEdmValue,
      IEdmElement,
      IEnumerable<IEdmDelayedValue>,
      IEnumerable
    {
      private readonly IEdmCollectionTypeReference targetCollectionType;
      private readonly IEdmCollectionValue collectionValue;

      public CastCollectionValue(
        IEdmCollectionTypeReference targetCollectionType,
        IEdmCollectionValue collectionValue)
      {
        this.targetCollectionType = targetCollectionType;
        this.collectionValue = collectionValue;
      }

      IEnumerable<IEdmDelayedValue> IEdmCollectionValue.Elements => (IEnumerable<IEdmDelayedValue>) this;

      IEdmTypeReference IEdmValue.Type => (IEdmTypeReference) this.targetCollectionType;

      EdmValueKind IEdmValue.ValueKind => EdmValueKind.Collection;

      IEnumerator<IEdmDelayedValue> IEnumerable<IEdmDelayedValue>.GetEnumerator() => (IEnumerator<IEdmDelayedValue>) new EdmExpressionEvaluator.CastCollectionValue.CastCollectionValueEnumerator(this);

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) new EdmExpressionEvaluator.CastCollectionValue.CastCollectionValueEnumerator(this);

      private class CastCollectionValueEnumerator : 
        IEnumerator<IEdmDelayedValue>,
        IDisposable,
        IEnumerator
      {
        private readonly EdmExpressionEvaluator.CastCollectionValue value;
        private readonly IEnumerator<IEdmDelayedValue> enumerator;

        public CastCollectionValueEnumerator(EdmExpressionEvaluator.CastCollectionValue value)
        {
          this.value = value;
          this.enumerator = value.collectionValue.Elements.GetEnumerator();
        }

        public IEdmDelayedValue Current => (IEdmDelayedValue) new EdmExpressionEvaluator.CastCollectionValue.CastCollectionValueEnumerator.DelayedCast(this.value.targetCollectionType.ElementType(), this.enumerator.Current);

        object IEnumerator.Current => (object) this.Current;

        bool IEnumerator.MoveNext() => this.enumerator.MoveNext();

        void IEnumerator.Reset() => this.enumerator.Reset();

        void IDisposable.Dispose() => this.enumerator.Dispose();

        private class DelayedCast : IEdmDelayedValue
        {
          private readonly IEdmDelayedValue delayedValue;
          private readonly IEdmTypeReference targetType;
          private IEdmValue value;

          public DelayedCast(IEdmTypeReference targetType, IEdmDelayedValue value)
          {
            this.delayedValue = value;
            this.targetType = targetType;
          }

          public IEdmValue Value
          {
            get
            {
              if (this.value == null)
                this.value = EdmExpressionEvaluator.Cast(this.targetType, this.delayedValue.Value);
              return this.value;
            }
          }
        }
      }
    }
  }
}
