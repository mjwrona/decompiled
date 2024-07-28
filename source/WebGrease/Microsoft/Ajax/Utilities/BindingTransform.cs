// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.BindingTransform
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;

namespace Microsoft.Ajax.Utilities
{
  public static class BindingTransform
  {
    public static AstNode FromBinding(AstNode node) => BindingTransform.ConvertFromBinding(node);

    public static AstNode ToBinding(AstNode node) => BindingTransform.ConvertToBinding(node);

    public static AstNodeList ToParameters(AstNode node)
    {
      AstNodeList parameterList = (AstNodeList) null;
      if (node != null)
      {
        parameterList = new AstNodeList(node.Context);
        GroupingOperator groupingOperator = node as GroupingOperator;
        BindingTransform.RecurseParameters(parameterList, groupingOperator != null ? groupingOperator.Operand : node);
      }
      return parameterList;
    }

    private static AstNode ConvertFromBinding(AstNode node)
    {
      switch (node)
      {
        case BindingIdentifier bindingIdentifier:
          return (AstNode) BindingTransform.ConvertFromBindingIdentifier(bindingIdentifier);
        case ArrayLiteral bindingLiteral1:
          return (AstNode) BindingTransform.ConvertFromBindingArrayLiteral(bindingLiteral1);
        case ObjectLiteral bindingLiteral2:
          return (AstNode) BindingTransform.ConvertFromBindingObjectLiteral(bindingLiteral2);
        case ObjectLiteralProperty bindingLiteral3:
          return (AstNode) BindingTransform.ConvertFromBindingObjectProperty(bindingLiteral3);
        default:
          node.Context.HandleError(JSError.UnableToConvertFromBinding, true);
          return (AstNode) null;
      }
    }

    private static Lookup ConvertFromBindingIdentifier(BindingIdentifier bindingIdentifier)
    {
      Lookup lookup = (Lookup) null;
      if (bindingIdentifier != null)
      {
        lookup = new Lookup(bindingIdentifier.Context)
        {
          Name = bindingIdentifier.Name,
          VariableField = bindingIdentifier.VariableField
        };
        bindingIdentifier.VariableField.IfNotNull<JSVariableField>((Action<JSVariableField>) (v => v.References.Add((INameReference) lookup)));
      }
      return lookup;
    }

    private static ArrayLiteral ConvertFromBindingArrayLiteral(ArrayLiteral bindingLiteral)
    {
      ArrayLiteral arrayLiteral1 = (ArrayLiteral) null;
      if (bindingLiteral != null)
      {
        ArrayLiteral arrayLiteral2 = new ArrayLiteral(bindingLiteral.Context);
        arrayLiteral2.TerminatingContext = bindingLiteral.TerminatingContext;
        arrayLiteral1 = arrayLiteral2;
        if (bindingLiteral.Elements != null)
        {
          arrayLiteral1.Elements = new AstNodeList(bindingLiteral.Elements.Context);
          foreach (AstNode element in bindingLiteral.Elements)
            arrayLiteral1.Elements.Append(BindingTransform.ConvertFromBinding(element));
        }
      }
      return arrayLiteral1;
    }

    private static ObjectLiteral ConvertFromBindingObjectLiteral(ObjectLiteral bindingLiteral)
    {
      ObjectLiteral objectLiteral1 = (ObjectLiteral) null;
      if (bindingLiteral != null)
      {
        ObjectLiteral objectLiteral2 = new ObjectLiteral(bindingLiteral.Context);
        objectLiteral2.TerminatingContext = bindingLiteral.TerminatingContext;
        objectLiteral1 = objectLiteral2;
        if (bindingLiteral.Properties != null)
        {
          objectLiteral1.Properties = new AstNodeList(bindingLiteral.Properties.Context);
          foreach (AstNode property in bindingLiteral.Properties)
            objectLiteral1.Properties.Append(BindingTransform.ConvertFromBinding(property));
        }
      }
      return objectLiteral1;
    }

    private static ObjectLiteralProperty ConvertFromBindingObjectProperty(
      ObjectLiteralProperty bindingLiteral)
    {
      ObjectLiteralProperty objectLiteralProperty1 = (ObjectLiteralProperty) null;
      if (bindingLiteral != null)
      {
        ObjectLiteralProperty objectLiteralProperty2 = new ObjectLiteralProperty(bindingLiteral.Context);
        objectLiteralProperty2.Name = BindingTransform.ConvertFromBindingObjectName(bindingLiteral.Name);
        objectLiteralProperty2.Value = BindingTransform.ConvertFromBinding(bindingLiteral.Value);
        objectLiteralProperty2.TerminatingContext = bindingLiteral.TerminatingContext;
        objectLiteralProperty1 = objectLiteralProperty2;
      }
      return objectLiteralProperty1;
    }

    private static ObjectLiteralField ConvertFromBindingObjectName(ObjectLiteralField bindingLiteral)
    {
      ObjectLiteralField objectLiteralField1 = (ObjectLiteralField) null;
      if (bindingLiteral != null)
      {
        ObjectLiteralField objectLiteralField2 = new ObjectLiteralField((object) bindingLiteral.Name, bindingLiteral.PrimitiveType, bindingLiteral.Context);
        objectLiteralField2.ColonContext = bindingLiteral.ColonContext;
        objectLiteralField2.IsIdentifier = bindingLiteral.IsIdentifier;
        objectLiteralField2.MayHaveIssues = bindingLiteral.MayHaveIssues;
        objectLiteralField2.TerminatingContext = bindingLiteral.TerminatingContext;
        objectLiteralField1 = objectLiteralField2;
      }
      return objectLiteralField1;
    }

    private static AstNode ConvertToBinding(AstNode node)
    {
      switch (node)
      {
        case Lookup lookup:
          return (AstNode) BindingTransform.ConvertToBindingIdentifier(lookup);
        case ArrayLiteral arrayLiteral:
          return (AstNode) BindingTransform.ConvertToBindingArrayLiteral(arrayLiteral);
        case ObjectLiteral objectLiteral:
          return (AstNode) BindingTransform.ConvertToBindingObjectLiteral(objectLiteral);
        case ObjectLiteralProperty objectProperty:
          return (AstNode) BindingTransform.ConvertToBindingObjectProperty(objectProperty);
        case ConstantWrapper binding when binding.Value == Missing.Value:
          return (AstNode) binding;
        case ImportExportSpecifier specifier:
          return (AstNode) BindingTransform.ConvertToBindingSpecifier(specifier);
        default:
          node.Context.HandleError(JSError.UnableToConvertToBinding, true);
          return (AstNode) null;
      }
    }

    private static BindingIdentifier ConvertToBindingIdentifier(Lookup lookup)
    {
      BindingIdentifier bindingIdentifier = (BindingIdentifier) null;
      if (lookup != null)
      {
        bindingIdentifier = new BindingIdentifier(lookup.Context)
        {
          Name = lookup.Name,
          VariableField = lookup.VariableField
        };
        lookup.VariableField.IfNotNull<JSVariableField>((Action<JSVariableField>) (v =>
        {
          v.Declarations.Add((INameDeclaration) bindingIdentifier);
          v.References.Remove((INameReference) lookup);
        }));
      }
      return bindingIdentifier;
    }

    private static ArrayLiteral ConvertToBindingArrayLiteral(ArrayLiteral arrayLiteral)
    {
      ArrayLiteral bindingArrayLiteral = (ArrayLiteral) null;
      if (arrayLiteral != null)
      {
        ArrayLiteral arrayLiteral1 = new ArrayLiteral(arrayLiteral.Context);
        arrayLiteral1.TerminatingContext = arrayLiteral.TerminatingContext;
        bindingArrayLiteral = arrayLiteral1;
        if (arrayLiteral.Elements != null)
        {
          bindingArrayLiteral.Elements = new AstNodeList(arrayLiteral.Elements.Context);
          foreach (AstNode element in arrayLiteral.Elements)
            bindingArrayLiteral.Elements.Append(BindingTransform.ConvertToBinding(element));
        }
      }
      return bindingArrayLiteral;
    }

    private static ObjectLiteral ConvertToBindingObjectLiteral(ObjectLiteral objectLiteral)
    {
      ObjectLiteral bindingObjectLiteral = (ObjectLiteral) null;
      if (objectLiteral != null)
      {
        ObjectLiteral objectLiteral1 = new ObjectLiteral(objectLiteral.Context);
        objectLiteral1.TerminatingContext = objectLiteral.TerminatingContext;
        bindingObjectLiteral = objectLiteral1;
        if (objectLiteral.Properties != null)
        {
          bindingObjectLiteral.Properties = new AstNodeList(objectLiteral.Properties.Context);
          foreach (AstNode property in objectLiteral.Properties)
            bindingObjectLiteral.Properties.Append(BindingTransform.ConvertToBinding(property));
        }
      }
      return bindingObjectLiteral;
    }

    private static ObjectLiteralProperty ConvertToBindingObjectProperty(
      ObjectLiteralProperty objectProperty)
    {
      ObjectLiteralProperty bindingObjectProperty = (ObjectLiteralProperty) null;
      if (objectProperty != null)
      {
        ObjectLiteralProperty objectLiteralProperty = new ObjectLiteralProperty(objectProperty.Context);
        objectLiteralProperty.Name = BindingTransform.ConvertToBindingObjectName(objectProperty.Name);
        objectLiteralProperty.Value = BindingTransform.ConvertToBinding(objectProperty.Value);
        objectLiteralProperty.TerminatingContext = objectProperty.TerminatingContext;
        bindingObjectProperty = objectLiteralProperty;
      }
      return bindingObjectProperty;
    }

    private static ObjectLiteralField ConvertToBindingObjectName(ObjectLiteralField objectName)
    {
      ObjectLiteralField bindingObjectName = (ObjectLiteralField) null;
      if (objectName != null)
      {
        ObjectLiteralField objectLiteralField = new ObjectLiteralField((object) objectName.Name, objectName.PrimitiveType, objectName.Context);
        objectLiteralField.IsIdentifier = objectName.IsIdentifier;
        objectLiteralField.ColonContext = objectName.ColonContext;
        objectLiteralField.MayHaveIssues = objectName.MayHaveIssues;
        objectLiteralField.TerminatingContext = objectName.TerminatingContext;
        bindingObjectName = objectLiteralField;
      }
      return bindingObjectName;
    }

    private static ImportExportSpecifier ConvertToBindingSpecifier(ImportExportSpecifier specifier)
    {
      if (specifier != null && specifier.LocalIdentifier != null)
        specifier.LocalIdentifier = BindingTransform.ConvertToBinding(specifier.LocalIdentifier);
      return specifier;
    }

    private static void RecurseParameters(AstNodeList parameterList, AstNode node)
    {
      if (node == null)
        return;
      if (node is BinaryOperator binaryOperator && binaryOperator.OperatorToken == JSToken.Comma)
      {
        BindingTransform.RecurseParameters(parameterList, binaryOperator.Operand1);
        if (binaryOperator.Operand2 is AstNodeList operand2)
        {
          foreach (AstNode child in operand2.Children)
            parameterList.Append((AstNode) BindingTransform.ConvertToParameter(child, parameterList.Count));
        }
        else
          parameterList.Append((AstNode) BindingTransform.ConvertToParameter(binaryOperator.Operand2, parameterList.Count));
      }
      else
        parameterList.Append((AstNode) BindingTransform.ConvertToParameter(node, 0));
    }

    private static ParameterDeclaration ConvertToParameter(AstNode node, int position)
    {
      ParameterDeclaration parameterDeclaration = new ParameterDeclaration(node.Context)
      {
        Position = position
      };
      switch (node)
      {
        case UnaryOperator unaryOperator when unaryOperator.OperatorToken == JSToken.RestSpread:
          parameterDeclaration.HasRest = true;
          parameterDeclaration.RestContext = unaryOperator.OperatorContext;
          parameterDeclaration.Binding = BindingTransform.ConvertToBinding(unaryOperator.Operand);
          break;
        case BinaryOperator binaryOperator when binaryOperator.OperatorToken == JSToken.Assign:
          parameterDeclaration.AssignContext = binaryOperator.OperatorContext;
          parameterDeclaration.Initializer = binaryOperator.Operand2;
          parameterDeclaration.Binding = BindingTransform.ConvertToBinding(binaryOperator.Operand1);
          break;
        default:
          parameterDeclaration.Binding = BindingTransform.ConvertToBinding(node);
          break;
      }
      return parameterDeclaration.Binding == null ? (ParameterDeclaration) null : parameterDeclaration;
    }
  }
}
