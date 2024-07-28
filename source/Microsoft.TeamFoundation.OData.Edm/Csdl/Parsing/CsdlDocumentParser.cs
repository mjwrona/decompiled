// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.CsdlDocumentParser
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Csdl.Parsing.Common;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.OData.Edm.Csdl.Parsing
{
  internal class CsdlDocumentParser : EdmXmlDocumentParser<CsdlSchema>
  {
    private Version artifactVersion;
    private int entityContainerCount;

    internal CsdlDocumentParser(string documentPath, XmlReader reader)
      : base(documentPath, reader)
    {
      this.entityContainerCount = 0;
    }

    internal override IEnumerable<KeyValuePair<Version, string>> SupportedVersions => CsdlConstants.SupportedVersions.SelectMany<KeyValuePair<Version, string[]>, KeyValuePair<Version, string>>((Func<KeyValuePair<Version, string[]>, IEnumerable<KeyValuePair<Version, string>>>) (kvp => ((IEnumerable<string>) kvp.Value).Select<string, KeyValuePair<Version, string>>((Func<string, KeyValuePair<Version, string>>) (ns => new KeyValuePair<Version, string>(kvp.Key, ns)))));

    protected override bool TryGetDocumentElementParser(
      Version csdlArtifactVersion,
      XmlElementInfo rootElement,
      out XmlElementParser<CsdlSchema> parser)
    {
      EdmUtil.CheckArgumentNull<XmlElementInfo>(rootElement, nameof (rootElement));
      this.artifactVersion = csdlArtifactVersion;
      if (string.Equals(rootElement.Name, "Schema", StringComparison.Ordinal))
      {
        parser = this.CreateRootElementParser();
        return true;
      }
      parser = (XmlElementParser<CsdlSchema>) null;
      return false;
    }

    protected override void AnnotateItem(object result, XmlElementValueCollection childValues)
    {
      if (!(result is Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlElement csdlElement))
        return;
      foreach (XmlAnnotationInfo annotation in (IEnumerable<XmlAnnotationInfo>) this.currentElement.Annotations)
        csdlElement.AddAnnotation(new CsdlDirectValueAnnotation(annotation.NamespaceName, annotation.Name, annotation.Value, annotation.IsAttribute, annotation.Location));
      foreach (CsdlAnnotation annotation in childValues.ValuesOfType<CsdlAnnotation>())
        csdlElement.AddAnnotation(annotation);
    }

    private XmlElementParser<CsdlSchema> CreateRootElementParser()
    {
      XmlElementParser<CsdlTypeReference> xmlElementParser1 = this.CsdlElement<CsdlTypeReference>("ReferenceType", new Func<XmlElementInfo, XmlElementValueCollection, CsdlTypeReference>(this.OnEntityReferenceTypeElement));
      XmlElementParser<CsdlTypeReference> child = this.CsdlElement<CsdlTypeReference>("CollectionType", new Func<XmlElementInfo, XmlElementValueCollection, CsdlTypeReference>(this.OnCollectionTypeElement), (XmlElementParser) this.CsdlElement<CsdlTypeReference>("TypeRef", new Func<XmlElementInfo, XmlElementValueCollection, CsdlTypeReference>(this.OnTypeRefElement)), (XmlElementParser) xmlElementParser1);
      XmlElementParser<CsdlProperty> xmlElementParser2 = this.CsdlElement<CsdlProperty>("Property", new Func<XmlElementInfo, XmlElementValueCollection, CsdlProperty>(this.OnPropertyElement));
      XmlElementParser<CsdlExpressionBase> xmlElementParser3 = this.CsdlElement<CsdlExpressionBase>("String", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnStringConstantExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser4 = this.CsdlElement<CsdlExpressionBase>("Binary", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnBinaryConstantExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser5 = this.CsdlElement<CsdlExpressionBase>("Int", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnIntConstantExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser6 = this.CsdlElement<CsdlExpressionBase>("Float", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnFloatConstantExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser7 = this.CsdlElement<CsdlExpressionBase>("Guid", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnGuidConstantExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser8 = this.CsdlElement<CsdlExpressionBase>("Decimal", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnDecimalConstantExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser9 = this.CsdlElement<CsdlExpressionBase>("Bool", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnBoolConstantExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser10 = this.CsdlElement<CsdlExpressionBase>("Duration", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnDurationConstantExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser11 = this.CsdlElement<CsdlExpressionBase>("Date", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnDateConstantExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser12 = this.CsdlElement<CsdlExpressionBase>("TimeOfDay", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnTimeOfDayConstantExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser13 = this.CsdlElement<CsdlExpressionBase>("DateTimeOffset", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnDateTimeOffsetConstantExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser14 = this.CsdlElement<CsdlExpressionBase>("Null", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnNullConstantExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser15 = this.CsdlElement<CsdlExpressionBase>("Path", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnPathExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser16 = this.CsdlElement<CsdlExpressionBase>("PropertyPath", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnPropertyPathExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser17 = this.CsdlElement<CsdlExpressionBase>("NavigationPropertyPath", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(CsdlDocumentParser.OnNavigationPropertyPathExpression));
      XmlElementParser<CsdlExpressionBase> xmlElementParser18 = this.CsdlElement<CsdlExpressionBase>("EnumMember", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(this.OnEnumMemberExpression));
      XmlElementParser<CsdlExpressionBase> parent1 = this.CsdlElement<CsdlExpressionBase>("If", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(this.OnIfExpression));
      XmlElementParser<CsdlExpressionBase> parent2 = this.CsdlElement<CsdlExpressionBase>("Cast", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(this.OnCastExpression));
      XmlElementParser<CsdlExpressionBase> parent3 = this.CsdlElement<CsdlExpressionBase>("IsType", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(this.OnIsTypeExpression));
      XmlElementParser<CsdlPropertyValue> parent4 = this.CsdlElement<CsdlPropertyValue>("PropertyValue", new Func<XmlElementInfo, XmlElementValueCollection, CsdlPropertyValue>(this.OnPropertyValueElement));
      XmlElementParser<CsdlRecordExpression> xmlElementParser19 = this.CsdlElement<CsdlRecordExpression>("Record", new Func<XmlElementInfo, XmlElementValueCollection, CsdlRecordExpression>(this.OnRecordElement), (XmlElementParser) parent4);
      XmlElementParser<CsdlLabeledExpression> parent5 = this.CsdlElement<CsdlLabeledExpression>("LabeledElement", new Func<XmlElementInfo, XmlElementValueCollection, CsdlLabeledExpression>(this.OnLabeledElement));
      XmlElementParser<CsdlCollectionExpression> parent6 = this.CsdlElement<CsdlCollectionExpression>("Collection", new Func<XmlElementInfo, XmlElementValueCollection, CsdlCollectionExpression>(this.OnCollectionElement));
      XmlElementParser<CsdlExpressionBase> parent7 = this.CsdlElement<CsdlExpressionBase>("Apply", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(this.OnApplyElement));
      XmlElementParser<CsdlExpressionBase> xmlElementParser20 = this.CsdlElement<CsdlExpressionBase>("LabeledElementReference", new Func<XmlElementInfo, XmlElementValueCollection, CsdlExpressionBase>(this.OnLabeledElementReferenceExpression));
      XmlElementParser[] children = new XmlElementParser[25]
      {
        (XmlElementParser) xmlElementParser3,
        (XmlElementParser) xmlElementParser4,
        (XmlElementParser) xmlElementParser5,
        (XmlElementParser) xmlElementParser6,
        (XmlElementParser) xmlElementParser7,
        (XmlElementParser) xmlElementParser8,
        (XmlElementParser) xmlElementParser9,
        (XmlElementParser) xmlElementParser11,
        (XmlElementParser) xmlElementParser13,
        (XmlElementParser) xmlElementParser10,
        (XmlElementParser) xmlElementParser12,
        (XmlElementParser) xmlElementParser14,
        (XmlElementParser) xmlElementParser15,
        (XmlElementParser) xmlElementParser16,
        (XmlElementParser) xmlElementParser17,
        (XmlElementParser) parent1,
        (XmlElementParser) parent3,
        (XmlElementParser) parent2,
        (XmlElementParser) xmlElementParser19,
        (XmlElementParser) parent6,
        (XmlElementParser) xmlElementParser20,
        (XmlElementParser) parent4,
        (XmlElementParser) parent5,
        (XmlElementParser) xmlElementParser18,
        (XmlElementParser) parent7
      };
      CsdlDocumentParser.AddChildParsers((XmlElementParser) parent1, (IEnumerable<XmlElementParser>) children);
      CsdlDocumentParser.AddChildParsers((XmlElementParser) parent2, (IEnumerable<XmlElementParser>) children);
      CsdlDocumentParser.AddChildParsers((XmlElementParser) parent3, (IEnumerable<XmlElementParser>) children);
      CsdlDocumentParser.AddChildParsers((XmlElementParser) parent4, (IEnumerable<XmlElementParser>) children);
      CsdlDocumentParser.AddChildParsers((XmlElementParser) parent6, (IEnumerable<XmlElementParser>) children);
      CsdlDocumentParser.AddChildParsers((XmlElementParser) parent5, (IEnumerable<XmlElementParser>) children);
      CsdlDocumentParser.AddChildParsers((XmlElementParser) parent7, (IEnumerable<XmlElementParser>) children);
      XmlElementParser<CsdlAnnotation> xmlElementParser21 = this.CsdlElement<CsdlAnnotation>("Annotation", new Func<XmlElementInfo, XmlElementValueCollection, CsdlAnnotation>(this.OnAnnotationElement));
      CsdlDocumentParser.AddChildParsers((XmlElementParser) xmlElementParser21, (IEnumerable<XmlElementParser>) children);
      xmlElementParser2.AddChildParser((XmlElementParser) xmlElementParser21);
      child.AddChildParser((XmlElementParser) child);
      return this.CsdlElement<CsdlSchema>("Schema", new Func<XmlElementInfo, XmlElementValueCollection, CsdlSchema>(this.OnSchemaElement), (XmlElementParser) this.CsdlElement<CsdlComplexType>("ComplexType", new Func<XmlElementInfo, XmlElementValueCollection, CsdlComplexType>(this.OnComplexTypeElement), (XmlElementParser) xmlElementParser2, (XmlElementParser) this.CsdlElement<CsdlNamedElement>("NavigationProperty", new Func<XmlElementInfo, XmlElementValueCollection, CsdlNamedElement>(this.OnNavigationPropertyElement), (XmlElementParser) this.CsdlElement<CsdlReferentialConstraint>("ReferentialConstraint", new Func<XmlElementInfo, XmlElementValueCollection, CsdlReferentialConstraint>(this.OnReferentialConstraintElement)), (XmlElementParser) this.CsdlElement<CsdlOnDelete>("OnDelete", new Func<XmlElementInfo, XmlElementValueCollection, CsdlOnDelete>(this.OnDeleteActionElement)), (XmlElementParser) xmlElementParser21), (XmlElementParser) xmlElementParser21), (XmlElementParser) this.CsdlElement<CsdlEntityType>("EntityType", new Func<XmlElementInfo, XmlElementValueCollection, CsdlEntityType>(this.OnEntityTypeElement), (XmlElementParser) this.CsdlElement<CsdlKey>("Key", new Func<XmlElementInfo, XmlElementValueCollection, CsdlKey>(CsdlDocumentParser.OnEntityKeyElement), (XmlElementParser) this.CsdlElement<CsdlPropertyReference>("PropertyRef", new Func<XmlElementInfo, XmlElementValueCollection, CsdlPropertyReference>(this.OnPropertyRefElement))), (XmlElementParser) xmlElementParser2, (XmlElementParser) this.CsdlElement<CsdlNamedElement>("NavigationProperty", new Func<XmlElementInfo, XmlElementValueCollection, CsdlNamedElement>(this.OnNavigationPropertyElement), (XmlElementParser) this.CsdlElement<CsdlReferentialConstraint>("ReferentialConstraint", new Func<XmlElementInfo, XmlElementValueCollection, CsdlReferentialConstraint>(this.OnReferentialConstraintElement)), (XmlElementParser) this.CsdlElement<CsdlOnDelete>("OnDelete", new Func<XmlElementInfo, XmlElementValueCollection, CsdlOnDelete>(this.OnDeleteActionElement)), (XmlElementParser) xmlElementParser21), (XmlElementParser) xmlElementParser21), (XmlElementParser) this.CsdlElement<CsdlEnumType>("EnumType", new Func<XmlElementInfo, XmlElementValueCollection, CsdlEnumType>(this.OnEnumTypeElement), (XmlElementParser) this.CsdlElement<CsdlEnumMember>("Member", new Func<XmlElementInfo, XmlElementValueCollection, CsdlEnumMember>(this.OnEnumMemberElement), (XmlElementParser) xmlElementParser21), (XmlElementParser) xmlElementParser21), (XmlElementParser) this.CsdlElement<CsdlTypeDefinition>("TypeDefinition", new Func<XmlElementInfo, XmlElementValueCollection, CsdlTypeDefinition>(this.OnTypeDefinitionElement), (XmlElementParser) xmlElementParser21), (XmlElementParser) this.CsdlElement<CsdlAction>("Action", new Func<XmlElementInfo, XmlElementValueCollection, CsdlAction>(this.OnActionElement), (XmlElementParser) this.CsdlElement<CsdlOperationParameter>("Parameter", new Func<XmlElementInfo, XmlElementValueCollection, CsdlOperationParameter>(this.OnParameterElement), (XmlElementParser) this.CsdlElement<CsdlTypeReference>("TypeRef", new Func<XmlElementInfo, XmlElementValueCollection, CsdlTypeReference>(this.OnTypeRefElement)), (XmlElementParser) child, (XmlElementParser) xmlElementParser1, (XmlElementParser) xmlElementParser21), (XmlElementParser) this.CsdlElement<CsdlOperationReturn>("ReturnType", new Func<XmlElementInfo, XmlElementValueCollection, CsdlOperationReturn>(this.OnReturnTypeElement), (XmlElementParser) this.CsdlElement<CsdlTypeReference>("TypeRef", new Func<XmlElementInfo, XmlElementValueCollection, CsdlTypeReference>(this.OnTypeRefElement)), (XmlElementParser) child, (XmlElementParser) xmlElementParser1, (XmlElementParser) xmlElementParser21), (XmlElementParser) xmlElementParser21), (XmlElementParser) this.CsdlElement<CsdlOperation>("Function", new Func<XmlElementInfo, XmlElementValueCollection, CsdlOperation>(this.OnFunctionElement), (XmlElementParser) this.CsdlElement<CsdlOperationParameter>("Parameter", new Func<XmlElementInfo, XmlElementValueCollection, CsdlOperationParameter>(this.OnParameterElement), (XmlElementParser) this.CsdlElement<CsdlTypeReference>("TypeRef", new Func<XmlElementInfo, XmlElementValueCollection, CsdlTypeReference>(this.OnTypeRefElement)), (XmlElementParser) child, (XmlElementParser) xmlElementParser1, (XmlElementParser) xmlElementParser21), (XmlElementParser) this.CsdlElement<CsdlOperationReturn>("ReturnType", new Func<XmlElementInfo, XmlElementValueCollection, CsdlOperationReturn>(this.OnReturnTypeElement), (XmlElementParser) this.CsdlElement<CsdlTypeReference>("TypeRef", new Func<XmlElementInfo, XmlElementValueCollection, CsdlTypeReference>(this.OnTypeRefElement)), (XmlElementParser) child, (XmlElementParser) xmlElementParser1, (XmlElementParser) xmlElementParser21), (XmlElementParser) xmlElementParser21), (XmlElementParser) this.CsdlElement<CsdlTerm>("Term", new Func<XmlElementInfo, XmlElementValueCollection, CsdlTerm>(this.OnTermElement), (XmlElementParser) this.CsdlElement<CsdlTypeReference>("TypeRef", new Func<XmlElementInfo, XmlElementValueCollection, CsdlTypeReference>(this.OnTypeRefElement)), (XmlElementParser) child, (XmlElementParser) xmlElementParser1, (XmlElementParser) xmlElementParser21), (XmlElementParser) this.CsdlElement<CsdlAnnotations>("Annotations", new Func<XmlElementInfo, XmlElementValueCollection, CsdlAnnotations>(this.OnAnnotationsElement), (XmlElementParser) xmlElementParser21), (XmlElementParser) this.CsdlElement<CsdlEntityContainer>("EntityContainer", new Func<XmlElementInfo, XmlElementValueCollection, CsdlEntityContainer>(this.OnEntityContainerElement), (XmlElementParser) this.CsdlElement<CsdlEntitySet>("EntitySet", new Func<XmlElementInfo, XmlElementValueCollection, CsdlEntitySet>(this.OnEntitySetElement), (XmlElementParser) this.CsdlElement<CsdlNavigationPropertyBinding>("NavigationPropertyBinding", new Func<XmlElementInfo, XmlElementValueCollection, CsdlNavigationPropertyBinding>(this.OnNavigationPropertyBindingElement)), (XmlElementParser) xmlElementParser21), (XmlElementParser) this.CsdlElement<CsdlSingleton>("Singleton", new Func<XmlElementInfo, XmlElementValueCollection, CsdlSingleton>(this.OnSingletonElement), (XmlElementParser) this.CsdlElement<CsdlNavigationPropertyBinding>("NavigationPropertyBinding", new Func<XmlElementInfo, XmlElementValueCollection, CsdlNavigationPropertyBinding>(this.OnNavigationPropertyBindingElement)), (XmlElementParser) xmlElementParser21), (XmlElementParser) this.CsdlElement<CsdlActionImport>("ActionImport", new Func<XmlElementInfo, XmlElementValueCollection, CsdlActionImport>(this.OnActionImportElement), (XmlElementParser) xmlElementParser21), (XmlElementParser) this.CsdlElement<CsdlOperationImport>("FunctionImport", new Func<XmlElementInfo, XmlElementValueCollection, CsdlOperationImport>(this.OnFunctionImportElement), (XmlElementParser) this.CsdlElement<CsdlOperationParameter>("Parameter", new Func<XmlElementInfo, XmlElementValueCollection, CsdlOperationParameter>(this.OnFunctionImportParameterElement), (XmlElementParser) xmlElementParser21), (XmlElementParser) xmlElementParser21), (XmlElementParser) xmlElementParser21));
    }

    private CsdlSchema OnSchemaElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlSchema(this.Optional("Namespace") ?? string.Empty, this.OptionalAlias("Alias"), this.artifactVersion, childValues.ValuesOfType<CsdlStructuredType>(), childValues.ValuesOfType<CsdlEnumType>(), childValues.ValuesOfType<CsdlOperation>(), childValues.ValuesOfType<CsdlTerm>(), childValues.ValuesOfType<CsdlEntityContainer>(), childValues.ValuesOfType<CsdlAnnotations>(), childValues.ValuesOfType<CsdlTypeDefinition>(), element.Location);
    }

    private CsdlComplexType OnComplexTypeElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      string name = this.Required("Name");
      string baseTypeName = this.OptionalQualifiedName("BaseType");
      bool? nullable = this.OptionalBoolean("OpenType");
      bool isOpen = ((int) nullable ?? 0) != 0;
      nullable = this.OptionalBoolean("Abstract");
      bool isAbstract = ((int) nullable ?? 0) != 0;
      return new CsdlComplexType(name, baseTypeName, isAbstract, isOpen, childValues.ValuesOfType<CsdlProperty>(), childValues.ValuesOfType<CsdlNavigationProperty>(), element.Location);
    }

    private CsdlEntityType OnEntityTypeElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      string name = this.Required("Name");
      string baseTypeName = this.OptionalQualifiedName("BaseType");
      bool? nullable = this.OptionalBoolean("OpenType");
      bool isOpen = ((int) nullable ?? 0) != 0;
      nullable = this.OptionalBoolean("Abstract");
      bool isAbstract = ((int) nullable ?? 0) != 0;
      nullable = this.OptionalBoolean("HasStream");
      bool hasStream = ((int) nullable ?? 0) != 0;
      CsdlKey key = childValues.ValuesOfType<CsdlKey>().FirstOrDefault<CsdlKey>();
      return new CsdlEntityType(name, baseTypeName, isAbstract, isOpen, hasStream, key, childValues.ValuesOfType<CsdlProperty>(), childValues.ValuesOfType<CsdlNavigationProperty>(), element.Location);
    }

    private CsdlProperty OnPropertyElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      CsdlTypeReference typeReference = this.ParseTypeReference(this.OptionalType("Type"), childValues, element.Location, CsdlDocumentParser.Optionality.Required);
      string name = this.Required("Name");
      string defaultValue = this.Optional("DefaultValue");
      return new CsdlProperty(name, typeReference, defaultValue, element.Location);
    }

    private CsdlTerm OnTermElement(XmlElementInfo element, XmlElementValueCollection childValues)
    {
      CsdlTypeReference typeReference = this.ParseTypeReference(this.OptionalType("Type"), childValues, element.Location, CsdlDocumentParser.Optionality.Required);
      string name = this.Required("Name");
      string appliesTo = this.Optional("AppliesTo");
      string defaultValue = this.Optional("DefaultValue");
      return new CsdlTerm(name, typeReference, appliesTo, defaultValue, element.Location);
    }

    private CsdlAnnotations OnAnnotationsElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      string target = this.Required("Target");
      string qualifier = this.Optional("Qualifier");
      return new CsdlAnnotations(childValues.ValuesOfType<CsdlAnnotation>(), target, qualifier);
    }

    private CsdlAnnotation OnAnnotationElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlAnnotation(this.RequiredQualifiedName("Term"), this.Optional("Qualifier"), this.ParseAnnotationExpression(element, childValues), element.Location);
    }

    private CsdlPropertyValue OnPropertyValueElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlPropertyValue(this.Required("Property"), this.ParseAnnotationExpression(element, childValues), element.Location);
    }

    private CsdlRecordExpression OnRecordElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      string fullName = this.OptionalQualifiedName("Type");
      IEnumerable<CsdlPropertyValue> propertyValues = childValues.ValuesOfType<CsdlPropertyValue>();
      return new CsdlRecordExpression(fullName != null ? (CsdlTypeReference) new CsdlNamedTypeReference(fullName, false, element.Location) : (CsdlTypeReference) null, propertyValues, element.Location);
    }

    private CsdlCollectionExpression OnCollectionElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlCollectionExpression(this.ParseTypeReference(this.OptionalType("Type"), childValues, element.Location, CsdlDocumentParser.Optionality.Optional), childValues.ValuesOfType<CsdlExpressionBase>(), element.Location);
    }

    private CsdlLabeledExpression OnLabeledElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      string label = this.Required("Name");
      IEnumerable<CsdlExpressionBase> source = childValues.ValuesOfType<CsdlExpressionBase>();
      if (source.Count<CsdlExpressionBase>() != 1)
        this.ReportError(element.Location, EdmErrorCode.InvalidLabeledElementExpressionIncorrectNumberOfOperands, Strings.CsdlParser_InvalidLabeledElementExpressionIncorrectNumberOfOperands);
      return new CsdlLabeledExpression(label, source.ElementAtOrDefault<CsdlExpressionBase>(0), element.Location);
    }

    private CsdlApplyExpression OnApplyElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlApplyExpression(this.Optional("Function"), childValues.ValuesOfType<CsdlExpressionBase>(), element.Location);
    }

    private static void AddChildParsers(
      XmlElementParser parent,
      IEnumerable<XmlElementParser> children)
    {
      foreach (XmlElementParser child in children)
        parent.AddChildParser(child);
    }

    private static CsdlConstantExpression ConstantExpression(
      EdmValueKind kind,
      XmlElementValueCollection childValues,
      CsdlLocation location)
    {
      XmlTextValue firstText = childValues.FirstText;
      return new CsdlConstantExpression(kind, firstText != null ? firstText.TextValue : string.Empty, location);
    }

    private static CsdlConstantExpression OnIntConstantExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return CsdlDocumentParser.ConstantExpression(EdmValueKind.Integer, childValues, element.Location);
    }

    private static CsdlConstantExpression OnStringConstantExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return CsdlDocumentParser.ConstantExpression(EdmValueKind.String, childValues, element.Location);
    }

    private static CsdlConstantExpression OnBinaryConstantExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return CsdlDocumentParser.ConstantExpression(EdmValueKind.Binary, childValues, element.Location);
    }

    private static CsdlConstantExpression OnFloatConstantExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return CsdlDocumentParser.ConstantExpression(EdmValueKind.Floating, childValues, element.Location);
    }

    private static CsdlConstantExpression OnGuidConstantExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return CsdlDocumentParser.ConstantExpression(EdmValueKind.Guid, childValues, element.Location);
    }

    private static CsdlConstantExpression OnDecimalConstantExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return CsdlDocumentParser.ConstantExpression(EdmValueKind.Decimal, childValues, element.Location);
    }

    private static CsdlConstantExpression OnBoolConstantExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return CsdlDocumentParser.ConstantExpression(EdmValueKind.Boolean, childValues, element.Location);
    }

    private static CsdlConstantExpression OnDurationConstantExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return CsdlDocumentParser.ConstantExpression(EdmValueKind.Duration, childValues, element.Location);
    }

    private static CsdlConstantExpression OnDateConstantExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return CsdlDocumentParser.ConstantExpression(EdmValueKind.Date, childValues, element.Location);
    }

    private static CsdlConstantExpression OnDateTimeOffsetConstantExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return CsdlDocumentParser.ConstantExpression(EdmValueKind.DateTimeOffset, childValues, element.Location);
    }

    private static CsdlConstantExpression OnTimeOfDayConstantExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return CsdlDocumentParser.ConstantExpression(EdmValueKind.TimeOfDay, childValues, element.Location);
    }

    private static CsdlConstantExpression OnNullConstantExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return CsdlDocumentParser.ConstantExpression(EdmValueKind.Null, childValues, element.Location);
    }

    private static CsdlPathExpression OnPathExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      XmlTextValue firstText = childValues.FirstText;
      return new CsdlPathExpression(firstText != null ? firstText.TextValue : string.Empty, element.Location);
    }

    private static CsdlPropertyPathExpression OnPropertyPathExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      XmlTextValue firstText = childValues.FirstText;
      return new CsdlPropertyPathExpression(firstText != null ? firstText.TextValue : string.Empty, element.Location);
    }

    private static CsdlNavigationPropertyPathExpression OnNavigationPropertyPathExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      XmlTextValue firstText = childValues.FirstText;
      return new CsdlNavigationPropertyPathExpression(firstText != null ? firstText.TextValue : string.Empty, element.Location);
    }

    private CsdlLabeledExpressionReferenceExpression OnLabeledElementReferenceExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlLabeledExpressionReferenceExpression(this.Required("Name"), element.Location);
    }

    private CsdlEnumMemberExpression OnEnumMemberExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlEnumMemberExpression(this.RequiredEnumMemberPath(childValues.FirstText), element.Location);
    }

    private CsdlExpressionBase OnIfExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      IEnumerable<CsdlExpressionBase> source = childValues.ValuesOfType<CsdlExpressionBase>();
      if (source.Count<CsdlExpressionBase>() != 3)
        this.ReportError(element.Location, EdmErrorCode.InvalidIfExpressionIncorrectNumberOfOperands, Strings.CsdlParser_InvalidIfExpressionIncorrectNumberOfOperands);
      return (CsdlExpressionBase) new CsdlIfExpression(source.ElementAtOrDefault<CsdlExpressionBase>(0), source.ElementAtOrDefault<CsdlExpressionBase>(1), source.ElementAtOrDefault<CsdlExpressionBase>(2), element.Location);
    }

    private CsdlExpressionBase OnCastExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      CsdlTypeReference typeReference = this.ParseTypeReference(this.OptionalType("Type"), childValues, element.Location, CsdlDocumentParser.Optionality.Required);
      IEnumerable<CsdlExpressionBase> source = childValues.ValuesOfType<CsdlExpressionBase>();
      if (source.Count<CsdlExpressionBase>() != 1)
        this.ReportError(element.Location, EdmErrorCode.InvalidCastExpressionIncorrectNumberOfOperands, Strings.CsdlParser_InvalidCastExpressionIncorrectNumberOfOperands);
      return (CsdlExpressionBase) new CsdlCastExpression(typeReference, source.ElementAtOrDefault<CsdlExpressionBase>(0), element.Location);
    }

    private CsdlExpressionBase OnIsTypeExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      CsdlTypeReference typeReference = this.ParseTypeReference(this.OptionalType("Type"), childValues, element.Location, CsdlDocumentParser.Optionality.Required);
      IEnumerable<CsdlExpressionBase> source = childValues.ValuesOfType<CsdlExpressionBase>();
      if (source.Count<CsdlExpressionBase>() != 1)
        this.ReportError(element.Location, EdmErrorCode.InvalidIsTypeExpressionIncorrectNumberOfOperands, Strings.CsdlParser_InvalidIsTypeExpressionIncorrectNumberOfOperands);
      return (CsdlExpressionBase) new CsdlIsTypeExpression(typeReference, source.ElementAtOrDefault<CsdlExpressionBase>(0), element.Location);
    }

    private CsdlTypeDefinition OnTypeDefinitionElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlTypeDefinition(this.Required("Name"), this.RequiredType("UnderlyingType"), element.Location);
    }

    private CsdlExpressionBase ParseAnnotationExpression(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      CsdlExpressionBase annotationExpression = childValues.ValuesOfType<CsdlExpressionBase>().FirstOrDefault<CsdlExpressionBase>();
      if (annotationExpression != null)
        return annotationExpression;
      string path1 = this.Optional("Path");
      if (path1 != null)
        return (CsdlExpressionBase) new CsdlPathExpression(path1, element.Location);
      string path2 = this.Optional("PropertyPath");
      if (path2 != null)
        return (CsdlExpressionBase) new CsdlPropertyPathExpression(path2, element.Location);
      string path3 = this.Optional("NavigationPropertyPath");
      if (path3 != null)
        return (CsdlExpressionBase) new CsdlNavigationPropertyPathExpression(path3, element.Location);
      string path4 = this.Optional("EnumMember");
      if (path4 != null)
        return (CsdlExpressionBase) new CsdlEnumMemberExpression(this.ValidateEnumMembersPath(path4), element.Location);
      string path5 = this.Optional("AnnotationPath");
      if (path5 != null)
        return (CsdlExpressionBase) new CsdlAnnotationPathExpression(path5, element.Location);
      string str = this.Optional("String");
      EdmValueKind kind;
      if (str != null)
      {
        kind = EdmValueKind.String;
      }
      else
      {
        str = this.Optional("Bool");
        if (str != null)
        {
          kind = EdmValueKind.Boolean;
        }
        else
        {
          str = this.Optional("Int");
          if (str != null)
          {
            kind = EdmValueKind.Integer;
          }
          else
          {
            str = this.Optional("Float");
            if (str != null)
            {
              kind = EdmValueKind.Floating;
            }
            else
            {
              str = this.Optional("DateTimeOffset");
              if (str != null)
              {
                kind = EdmValueKind.DateTimeOffset;
              }
              else
              {
                str = this.Optional("Duration");
                if (str != null)
                {
                  kind = EdmValueKind.Duration;
                }
                else
                {
                  str = this.Optional("Decimal");
                  if (str != null)
                  {
                    kind = EdmValueKind.Decimal;
                  }
                  else
                  {
                    str = this.Optional("Binary");
                    if (str != null)
                    {
                      kind = EdmValueKind.Binary;
                    }
                    else
                    {
                      str = this.Optional("Guid");
                      if (str != null)
                      {
                        kind = EdmValueKind.Guid;
                      }
                      else
                      {
                        str = this.Optional("Date");
                        if (str != null)
                        {
                          kind = EdmValueKind.Date;
                        }
                        else
                        {
                          str = this.Optional("TimeOfDay");
                          if (str == null)
                            return (CsdlExpressionBase) null;
                          kind = EdmValueKind.TimeOfDay;
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
      return (CsdlExpressionBase) new CsdlConstantExpression(kind, str, element.Location);
    }

    private CsdlNamedTypeReference ParseNamedTypeReference(
      string typeName,
      bool isNullable,
      CsdlLocation parentLocation)
    {
      EdmPrimitiveTypeKind primitiveTypeKind = EdmCoreModel.Instance.GetPrimitiveTypeKind(typeName);
      switch (primitiveTypeKind)
      {
        case EdmPrimitiveTypeKind.None:
          if (string.Equals(typeName, "Edm.Untyped", StringComparison.Ordinal))
            return (CsdlNamedTypeReference) new CsdlUntypedTypeReference(typeName, parentLocation);
          break;
        case EdmPrimitiveTypeKind.Binary:
          bool Unbounded1;
          int? maxLength1;
          this.ParseBinaryFacets(out Unbounded1, out maxLength1);
          return (CsdlNamedTypeReference) new CsdlBinaryTypeReference(Unbounded1, maxLength1, typeName, isNullable, parentLocation);
        case EdmPrimitiveTypeKind.Boolean:
        case EdmPrimitiveTypeKind.Byte:
        case EdmPrimitiveTypeKind.Double:
        case EdmPrimitiveTypeKind.Guid:
        case EdmPrimitiveTypeKind.Int16:
        case EdmPrimitiveTypeKind.Int32:
        case EdmPrimitiveTypeKind.Int64:
        case EdmPrimitiveTypeKind.SByte:
        case EdmPrimitiveTypeKind.Single:
        case EdmPrimitiveTypeKind.Stream:
        case EdmPrimitiveTypeKind.Date:
        case EdmPrimitiveTypeKind.PrimitiveType:
          return (CsdlNamedTypeReference) new CsdlPrimitiveTypeReference(primitiveTypeKind, typeName, isNullable, parentLocation);
        case EdmPrimitiveTypeKind.DateTimeOffset:
        case EdmPrimitiveTypeKind.Duration:
        case EdmPrimitiveTypeKind.TimeOfDay:
          int? precision1;
          this.ParseTemporalFacets(out precision1);
          return (CsdlNamedTypeReference) new CsdlTemporalTypeReference(primitiveTypeKind, precision1, typeName, isNullable, parentLocation);
        case EdmPrimitiveTypeKind.Decimal:
          int? precision2;
          int? scale1;
          this.ParseDecimalFacets(out precision2, out scale1);
          return (CsdlNamedTypeReference) new CsdlDecimalTypeReference(precision2, scale1, typeName, isNullable, parentLocation);
        case EdmPrimitiveTypeKind.String:
          bool Unbounded2;
          int? maxLength2;
          bool? unicode1;
          this.ParseStringFacets(out Unbounded2, out maxLength2, out unicode1);
          return (CsdlNamedTypeReference) new CsdlStringTypeReference(Unbounded2, maxLength2, unicode1, typeName, isNullable, parentLocation);
        case EdmPrimitiveTypeKind.Geography:
        case EdmPrimitiveTypeKind.GeographyPoint:
        case EdmPrimitiveTypeKind.GeographyLineString:
        case EdmPrimitiveTypeKind.GeographyPolygon:
        case EdmPrimitiveTypeKind.GeographyCollection:
        case EdmPrimitiveTypeKind.GeographyMultiPolygon:
        case EdmPrimitiveTypeKind.GeographyMultiLineString:
        case EdmPrimitiveTypeKind.GeographyMultiPoint:
          int? srid1;
          this.ParseSpatialFacets(out srid1, 4326);
          return (CsdlNamedTypeReference) new CsdlSpatialTypeReference(primitiveTypeKind, srid1, typeName, isNullable, parentLocation);
        case EdmPrimitiveTypeKind.Geometry:
        case EdmPrimitiveTypeKind.GeometryPoint:
        case EdmPrimitiveTypeKind.GeometryLineString:
        case EdmPrimitiveTypeKind.GeometryPolygon:
        case EdmPrimitiveTypeKind.GeometryCollection:
        case EdmPrimitiveTypeKind.GeometryMultiPolygon:
        case EdmPrimitiveTypeKind.GeometryMultiLineString:
        case EdmPrimitiveTypeKind.GeometryMultiPoint:
          int? srid2;
          this.ParseSpatialFacets(out srid2, 0);
          return (CsdlNamedTypeReference) new CsdlSpatialTypeReference(primitiveTypeKind, srid2, typeName, isNullable, parentLocation);
      }
      bool isUnbounded;
      int? maxLength3;
      bool? unicode2;
      int? precision3;
      int? scale2;
      int? srid3;
      this.ParseTypeDefinitionFacets(out isUnbounded, out maxLength3, out unicode2, out precision3, out scale2, out srid3);
      return new CsdlNamedTypeReference(isUnbounded, maxLength3, unicode2, precision3, scale2, srid3, typeName, isNullable, parentLocation);
    }

    private CsdlTypeReference ParseTypeReference(
      string typeString,
      XmlElementValueCollection childValues,
      CsdlLocation parentLocation,
      CsdlDocumentParser.Optionality typeInfoOptionality)
    {
      bool isNullable = ((int) this.OptionalBoolean("Nullable") ?? 1) != 0;
      CsdlTypeReference typeReference = (CsdlTypeReference) null;
      if (typeString != null)
      {
        string[] source = typeString.Split('(', ')');
        string typeName = source[0];
        switch (typeName)
        {
          case "Collection":
            typeReference = (CsdlTypeReference) new CsdlExpressionTypeReference((ICsdlTypeExpression) new CsdlCollectionType((CsdlTypeReference) this.ParseNamedTypeReference(((IEnumerable<string>) source).Count<string>() > 1 ? source[1] : typeString, isNullable, parentLocation), parentLocation), isNullable, parentLocation);
            break;
          case "Ref":
            typeReference = (CsdlTypeReference) new CsdlExpressionTypeReference((ICsdlTypeExpression) new CsdlEntityReferenceType((CsdlTypeReference) this.ParseNamedTypeReference(((IEnumerable<string>) source).Count<string>() > 1 ? source[1] : typeString, isNullable, parentLocation), parentLocation), true, parentLocation);
            break;
          default:
            typeReference = (CsdlTypeReference) this.ParseNamedTypeReference(typeName, isNullable, parentLocation);
            break;
        }
      }
      else if (childValues != null)
        typeReference = childValues.ValuesOfType<CsdlTypeReference>().FirstOrDefault<CsdlTypeReference>();
      if (typeReference == null && typeInfoOptionality == CsdlDocumentParser.Optionality.Required)
      {
        if (childValues != null)
          this.ReportError(parentLocation, EdmErrorCode.MissingType, Strings.CsdlParser_MissingTypeAttributeOrElement);
        typeReference = (CsdlTypeReference) new CsdlNamedTypeReference(string.Empty, isNullable, parentLocation);
      }
      return typeReference;
    }

    private CsdlNamedElement OnNavigationPropertyElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      string name = this.Required("Name");
      string type = this.RequiredType("Type");
      bool? nullable1 = this.OptionalBoolean("Nullable");
      string partner = this.Optional("Partner");
      bool? nullable2 = this.OptionalBoolean("ContainsTarget");
      CsdlOnDelete onDelete = childValues.ValuesOfType<CsdlOnDelete>().FirstOrDefault<CsdlOnDelete>();
      IEnumerable<CsdlReferentialConstraint> list = (IEnumerable<CsdlReferentialConstraint>) childValues.ValuesOfType<CsdlReferentialConstraint>().ToList<CsdlReferentialConstraint>();
      return (CsdlNamedElement) new CsdlNavigationProperty(name, type, nullable1, partner, ((int) nullable2 ?? 0) != 0, onDelete, list, element.Location);
    }

    private static CsdlKey OnEntityKeyElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlKey((IEnumerable<CsdlPropertyReference>) new List<CsdlPropertyReference>(childValues.ValuesOfType<CsdlPropertyReference>()), element.Location);
    }

    private CsdlPropertyReference OnPropertyRefElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlPropertyReference(this.Required("Name"), element.Location);
    }

    private CsdlEnumType OnEnumTypeElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlEnumType(this.Required("Name"), this.OptionalType("UnderlyingType"), ((int) this.OptionalBoolean("IsFlags") ?? 0) != 0, childValues.ValuesOfType<CsdlEnumMember>(), element.Location);
    }

    private CsdlEnumMember OnEnumMemberElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlEnumMember(this.Required("Name"), this.OptionalLong("Value"), element.Location);
    }

    private CsdlOnDelete OnDeleteActionElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlOnDelete(this.RequiredOnDeleteAction("Action"), element.Location);
    }

    private CsdlReferentialConstraint OnReferentialConstraintElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlReferentialConstraint(this.Required("Property"), this.Required("ReferencedProperty"), element.Location);
    }

    internal CsdlAction OnActionElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      string name = this.Required("Name");
      bool isBound = ((int) this.OptionalBoolean("IsBound") ?? 0) != 0;
      string entitySetPath = this.Optional("EntitySetPath");
      IEnumerable<CsdlOperationParameter> parameters = childValues.ValuesOfType<CsdlOperationParameter>();
      CsdlOperationReturn operationReturn = childValues.ValuesOfType<CsdlOperationReturn>().FirstOrDefault<CsdlOperationReturn>();
      this.ReportOperationReadErrorsIfExist(entitySetPath, isBound, name);
      return new CsdlAction(name, parameters, operationReturn, isBound, entitySetPath, element.Location);
    }

    internal CsdlFunction OnFunctionElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      string name = this.Required("Name");
      bool? nullable = this.OptionalBoolean("IsBound");
      bool isBound = ((int) nullable ?? 0) != 0;
      string entitySetPath = this.Optional("EntitySetPath");
      nullable = this.OptionalBoolean("IsComposable");
      bool isComposable = ((int) nullable ?? 0) != 0;
      IEnumerable<CsdlOperationParameter> parameters = childValues.ValuesOfType<CsdlOperationParameter>();
      CsdlOperationReturn operationReturn = childValues.ValuesOfType<CsdlOperationReturn>().FirstOrDefault<CsdlOperationReturn>();
      this.ReportOperationReadErrorsIfExist(entitySetPath, isBound, name);
      return new CsdlFunction(name, parameters, operationReturn, isBound, entitySetPath, isComposable, element.Location);
    }

    private void ReportOperationReadErrorsIfExist(string entitySetPath, bool isBound, string name)
    {
      if (entitySetPath == null || isBound)
        return;
      this.ReportError(this.currentElement.Location, EdmErrorCode.InvalidEntitySetPath, Strings.CsdlParser_InvalidEntitySetPathWithUnboundAction((object) "Action", (object) name));
    }

    private CsdlOperationParameter OnParameterElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      string name = this.Required("Name");
      string typeString = this.OptionalType("Type");
      string defaultValue = (string) null;
      bool isOptional = false;
      CsdlTypeReference typeReference = this.ParseTypeReference(typeString, childValues, element.Location, CsdlDocumentParser.Optionality.Required);
      XmlElementValue xmlElementValue = childValues.Where<XmlElementValue>((Func<XmlElementValue, bool>) (c =>
      {
        if (!(c is XmlElementValue<CsdlAnnotation>))
          return false;
        return c.ValueAs<CsdlAnnotation>().Term == CoreVocabularyModel.OptionalParameterTerm.ShortQualifiedName() || c.ValueAs<CsdlAnnotation>().Term == CoreVocabularyModel.OptionalParameterTerm.FullName();
      })).FirstOrDefault<XmlElementValue>();
      if (xmlElementValue != null)
      {
        isOptional = true;
        if (xmlElementValue.ValueAs<CsdlAnnotation>().Expression is CsdlRecordExpression expression1)
        {
          foreach (CsdlPropertyValue propertyValue in expression1.PropertyValues)
          {
            if (propertyValue.Expression is CsdlConstantExpression expression && propertyValue.Property == "DefaultValue")
              defaultValue = expression.Value;
          }
        }
        childValues.Remove(xmlElementValue);
      }
      return new CsdlOperationParameter(name, typeReference, element.Location, isOptional, defaultValue);
    }

    private CsdlActionImport OnActionImportElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlActionImport(this.Required("Name"), this.RequiredQualifiedName("Action"), this.Optional("EntitySet"), element.Location);
    }

    private CsdlFunctionImport OnFunctionImportElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlFunctionImport(this.Required("Name"), this.RequiredQualifiedName("Function"), this.Optional("EntitySet"), ((int) this.OptionalBoolean("IncludeInServiceDocument") ?? 0) != 0, element.Location);
    }

    private CsdlOperationParameter OnFunctionImportParameterElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlOperationParameter(this.Required("Name"), this.ParseTypeReference(this.RequiredType("Type"), (XmlElementValueCollection) null, element.Location, CsdlDocumentParser.Optionality.Required), element.Location);
    }

    private CsdlTypeReference OnEntityReferenceTypeElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return (CsdlTypeReference) new CsdlExpressionTypeReference((ICsdlTypeExpression) new CsdlEntityReferenceType(this.ParseTypeReference(this.RequiredType("Type"), (XmlElementValueCollection) null, element.Location, CsdlDocumentParser.Optionality.Required), element.Location), true, element.Location);
    }

    private CsdlTypeReference OnTypeRefElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return this.ParseTypeReference(this.RequiredType("Type"), (XmlElementValueCollection) null, element.Location, CsdlDocumentParser.Optionality.Required);
    }

    private CsdlTypeReference OnCollectionTypeElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      CsdlTypeReference typeReference = this.ParseTypeReference(this.OptionalType("ElementType"), childValues, element.Location, CsdlDocumentParser.Optionality.Required);
      return (CsdlTypeReference) new CsdlExpressionTypeReference((ICsdlTypeExpression) new CsdlCollectionType(typeReference, element.Location), typeReference.IsNullable, element.Location);
    }

    private CsdlOperationReturn OnReturnTypeElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlOperationReturn(this.ParseTypeReference(this.RequiredType("Type"), childValues, element.Location, CsdlDocumentParser.Optionality.Required), element.Location);
    }

    private CsdlEntityContainer OnEntityContainerElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      string name = this.Required("Name");
      string extends = this.Optional("Extends");
      if (this.entityContainerCount++ > 0)
        this.ReportError(this.currentElement.Location, EdmErrorCode.MetadataDocumentCannotHaveMoreThanOneEntityContainer, Strings.CsdlParser_MetadataDocumentCannotHaveMoreThanOneEntityContainer);
      return new CsdlEntityContainer(name, extends, childValues.ValuesOfType<CsdlEntitySet>(), childValues.ValuesOfType<CsdlSingleton>(), childValues.ValuesOfType<CsdlOperationImport>(), element.Location);
    }

    private CsdlEntitySet OnEntitySetElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      string name = this.Required("Name");
      string elementType = this.RequiredQualifiedName("EntityType");
      bool? nullable = this.OptionalBoolean("IncludeInServiceDocument");
      return !nullable.HasValue ? new CsdlEntitySet(name, elementType, childValues.ValuesOfType<CsdlNavigationPropertyBinding>(), element.Location) : new CsdlEntitySet(name, elementType, childValues.ValuesOfType<CsdlNavigationPropertyBinding>(), element.Location, nullable.Value);
    }

    private CsdlSingleton OnSingletonElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlSingleton(this.Required("Name"), this.RequiredQualifiedName("Type"), childValues.ValuesOfType<CsdlNavigationPropertyBinding>(), element.Location);
    }

    private CsdlNavigationPropertyBinding OnNavigationPropertyBindingElement(
      XmlElementInfo element,
      XmlElementValueCollection childValues)
    {
      return new CsdlNavigationPropertyBinding(this.Required("Path"), this.Required("Target"), element.Location);
    }

    private void ParseMaxLength(out bool Unbounded, out int? maxLength)
    {
      string string1 = this.Optional("MaxLength");
      if (string1 == null)
      {
        Unbounded = false;
        maxLength = new int?();
      }
      else if (string1.EqualsOrdinalIgnoreCase("max"))
      {
        Unbounded = true;
        maxLength = new int?();
      }
      else
      {
        Unbounded = false;
        maxLength = this.OptionalMaxLength("MaxLength");
      }
    }

    private void ParseBinaryFacets(out bool Unbounded, out int? maxLength) => this.ParseMaxLength(out Unbounded, out maxLength);

    private void ParseDecimalFacets(out int? precision, out int? scale)
    {
      precision = this.OptionalInteger("Precision");
      scale = this.OptionalScale("Scale");
    }

    private void ParseStringFacets(out bool Unbounded, out int? maxLength, out bool? unicode)
    {
      this.ParseMaxLength(out Unbounded, out maxLength);
      unicode = new bool?(((int) this.OptionalBoolean("Unicode") ?? 1) != 0);
    }

    private void ParseTemporalFacets(out int? precision) => precision = new int?(this.OptionalInteger("Precision") ?? 0);

    private void ParseSpatialFacets(out int? srid, int defaultSrid) => srid = this.OptionalSrid("SRID", defaultSrid);

    private void ParseTypeDefinitionFacets(
      out bool isUnbounded,
      out int? maxLength,
      out bool? unicode,
      out int? precision,
      out int? scale,
      out int? srid)
    {
      this.ParseMaxLength(out isUnbounded, out maxLength);
      unicode = this.OptionalBoolean("Unicode");
      precision = this.OptionalInteger("Precision");
      scale = this.OptionalScale("Scale");
      srid = this.OptionalSrid("SRID", int.MinValue);
    }

    private enum Optionality
    {
      Optional,
      Required,
    }
  }
}
