// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlReader
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.OData.Edm.Csdl
{
  public class CsdlReader
  {
    private static readonly Dictionary<string, Action> EmptyParserLookup = new Dictionary<string, Action>();
    private readonly Dictionary<string, Action> edmxParserLookup;
    private readonly Dictionary<string, Action> runtimeParserLookup;
    private readonly Dictionary<string, Action> conceptualModelsParserLookup;
    private readonly Dictionary<string, Action> dataServicesParserLookup;
    private readonly XmlReader reader;
    private readonly List<EdmError> errors;
    private readonly List<IEdmReference> edmReferences;
    private readonly CsdlParser csdlParser;
    private readonly Func<Uri, XmlReader> getReferencedModelReaderFunc;
    private bool targetParsed;
    private bool ignoreUnexpectedAttributesAndElements;
    private string source;

    private CsdlReader(XmlReader reader, Func<Uri, XmlReader> getReferencedModelReaderFunc)
    {
      this.reader = reader;
      this.getReferencedModelReaderFunc = getReferencedModelReaderFunc;
      this.errors = new List<EdmError>();
      this.edmReferences = new List<IEdmReference>();
      this.csdlParser = new CsdlParser();
      this.edmxParserLookup = new Dictionary<string, Action>()
      {
        {
          "DataServices",
          new Action(this.ParseDataServicesElement)
        },
        {
          "Reference",
          new Action(this.ParseReferenceElement)
        },
        {
          "Runtime",
          new Action(this.ParseRuntimeElement)
        }
      };
      this.dataServicesParserLookup = new Dictionary<string, Action>()
      {
        {
          "Schema",
          new Action(this.ParseSchemaElement)
        }
      };
      this.runtimeParserLookup = new Dictionary<string, Action>()
      {
        {
          "ConceptualModels",
          new Action(this.ParseConceptualModelsElement)
        }
      };
      this.conceptualModelsParserLookup = new Dictionary<string, Action>()
      {
        {
          "Schema",
          new Action(this.ParseSchemaElement)
        }
      };
    }

    public static bool TryParse(
      XmlReader reader,
      out IEdmModel model,
      out IEnumerable<EdmError> errors)
    {
      return new CsdlReader(reader, (Func<Uri, XmlReader>) null).TryParse(Enumerable.Empty<IEdmModel>(), true, out model, out errors);
    }

    public static bool TryParse(
      XmlReader reader,
      bool ignoreUnexpectedAttributesAndElements,
      out IEdmModel model,
      out IEnumerable<EdmError> errors)
    {
      return new CsdlReader(reader, (Func<Uri, XmlReader>) null)
      {
        ignoreUnexpectedAttributesAndElements = ignoreUnexpectedAttributesAndElements
      }.TryParse(Enumerable.Empty<IEdmModel>(), true, out model, out errors);
    }

    public static IEdmModel Parse(XmlReader reader)
    {
      IEdmModel model;
      IEnumerable<EdmError> errors;
      if (!CsdlReader.TryParse(reader, out model, out errors))
        throw new EdmParseException(errors);
      return model;
    }

    public static bool TryParse(
      XmlReader reader,
      Func<Uri, XmlReader> getReferencedModelReaderFunc,
      out IEdmModel model,
      out IEnumerable<EdmError> errors)
    {
      return new CsdlReader(reader, getReferencedModelReaderFunc).TryParse(Enumerable.Empty<IEdmModel>(), true, out model, out errors);
    }

    public static bool TryParse(
      XmlReader reader,
      IEnumerable<IEdmModel> references,
      out IEdmModel model,
      out IEnumerable<EdmError> errors)
    {
      return CsdlReader.TryParse(reader, references, true, out model, out errors);
    }

    public static bool TryParse(
      XmlReader reader,
      IEnumerable<IEdmModel> references,
      bool includeDefaultVocabularies,
      out IEdmModel model,
      out IEnumerable<EdmError> errors)
    {
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmModel>>(references, nameof (references));
      return new CsdlReader(reader, (Func<Uri, XmlReader>) null).TryParse(references, includeDefaultVocabularies, out model, out errors);
    }

    public static bool TryParse(
      XmlReader reader,
      IEdmModel reference,
      out IEdmModel model,
      out IEnumerable<EdmError> errors)
    {
      return new CsdlReader(reader, (Func<Uri, XmlReader>) null).TryParse((IEnumerable<IEdmModel>) new IEdmModel[1]
      {
        reference
      }, true, out model, out errors);
    }

    public static IEdmModel Parse(XmlReader reader, IEnumerable<IEdmModel> referencedModels)
    {
      IEdmModel model;
      IEnumerable<EdmError> errors;
      if (!CsdlReader.TryParse(reader, referencedModels, out model, out errors))
        throw new EdmParseException(errors);
      return model;
    }

    public static IEdmModel Parse(XmlReader reader, IEdmModel referencedModel)
    {
      IEdmModel model;
      IEnumerable<EdmError> errors;
      if (!CsdlReader.TryParse(reader, referencedModel, out model, out errors))
        throw new EdmParseException(errors);
      return model;
    }

    public static IEdmModel Parse(
      XmlReader reader,
      Func<Uri, XmlReader> getReferencedModelReaderFunc)
    {
      IEdmModel model;
      IEnumerable<EdmError> errors;
      if (!CsdlReader.TryParse(reader, getReferencedModelReaderFunc, out model, out errors))
        throw new EdmParseException(errors);
      return model;
    }

    public static bool TryParse(
      XmlReader reader,
      IEnumerable<IEdmModel> references,
      CsdlReaderSettings settings,
      out IEdmModel model,
      out IEnumerable<EdmError> errors)
    {
      EdmUtil.CheckArgumentNull<IEnumerable<IEdmModel>>(references, nameof (references));
      if (settings == null)
        return CsdlReader.TryParse(reader, references, out model, out errors);
      return new CsdlReader(reader, settings.GetReferencedModelReaderFunc)
      {
        ignoreUnexpectedAttributesAndElements = settings.IgnoreUnexpectedAttributesAndElements
      }.TryParse(references, true, out model, out errors);
    }

    private static bool TryParseVersion(string input, out Version version)
    {
      version = (Version) null;
      if (string.IsNullOrEmpty(input))
        return false;
      input = input.Trim();
      string[] strArray = input.Split('.');
      int result1;
      int result2;
      if (strArray.Length != 2 || !int.TryParse(strArray[0], out result1) || !int.TryParse(strArray[1], out result2))
        return false;
      version = new Version(result1, result2);
      return true;
    }

    private bool TryParse(
      IEnumerable<IEdmModel> referencedModels,
      bool includeDefaultVocabularies,
      out IEdmModel model,
      out IEnumerable<EdmError> parsingErrors)
    {
      Version csdlVersion;
      CsdlModel mainCsdlModel;
      this.TryParseCsdlFileToCsdlModel(out csdlVersion, out mainCsdlModel);
      if (!this.HasIntolerableError())
      {
        List<CsdlModel> referencedCsdlFiles = this.LoadAndParseReferencedCsdlFiles(csdlVersion);
        IEnumerable<EdmError> errors;
        this.csdlParser.GetResult(out mainCsdlModel, out errors);
        if (errors != null)
          this.errors.AddRange(errors.Except<EdmError>((IEnumerable<EdmError>) this.errors));
        if (!this.HasIntolerableError())
        {
          CsdlSemanticsModel csdlSemanticsModel = new CsdlSemanticsModel(mainCsdlModel, (IEdmDirectValueAnnotationsManager) new CsdlSemanticsDirectValueAnnotationsManager(), (IEnumerable<CsdlModel>) referencedCsdlFiles, includeDefaultVocabularies);
          csdlSemanticsModel.AddToReferencedModels(referencedModels);
          model = (IEdmModel) csdlSemanticsModel;
          model.SetEdmxVersion(csdlVersion);
          Version version;
          if (CsdlConstants.EdmxToEdmVersions.TryGetValue(csdlVersion, out version))
            model.SetEdmVersion(version);
        }
        else
          model = (IEdmModel) null;
      }
      else
        model = (IEdmModel) null;
      parsingErrors = (IEnumerable<EdmError>) this.errors;
      return !this.HasIntolerableError();
    }

    private List<CsdlModel> LoadAndParseReferencedCsdlFiles(Version mainCsdlVersion)
    {
      List<CsdlModel> referencedCsdlFiles = new List<CsdlModel>();
      if (this.getReferencedModelReaderFunc == null)
        return referencedCsdlFiles;
      foreach (IEdmReference edmReference in this.edmReferences)
      {
        if (!edmReference.Includes.Any<IEdmInclude>() && !edmReference.IncludeAnnotations.Any<IEdmIncludeAnnotations>())
          this.RaiseError(EdmErrorCode.ReferenceElementMustContainAtLeastOneIncludeOrIncludeAnnotationsElement, Strings.EdmxParser_InvalidReferenceIncorrectNumberOfIncludes);
        else if (!(edmReference.Uri != (Uri) null) || !edmReference.Uri.OriginalString.EndsWith("/Org.OData.Core.V1.xml", StringComparison.Ordinal) && !edmReference.Uri.OriginalString.EndsWith("/Org.OData.Capabilities.V1.xml", StringComparison.Ordinal) && !edmReference.Uri.OriginalString.EndsWith("/Org.OData.Authorization.V1.xml", StringComparison.Ordinal) && !edmReference.Uri.OriginalString.EndsWith("/Org.OData.Validation.V1.xml", StringComparison.Ordinal) && !edmReference.Uri.OriginalString.EndsWith("/Org.OData.Community.V1.xml", StringComparison.Ordinal) && !edmReference.Uri.OriginalString.EndsWith("/OData.Community.Keys.V1.xml", StringComparison.Ordinal))
        {
          XmlReader reader = this.getReferencedModelReaderFunc(edmReference.Uri);
          if (reader == null)
          {
            this.RaiseError(EdmErrorCode.UnresolvedReferenceUriInEdmxReference, Strings.EdmxParser_UnresolvedReferenceUriInEdmxReference);
          }
          else
          {
            CsdlReader csdlReader = new CsdlReader(reader, (Func<Uri, XmlReader>) null);
            csdlReader.source = edmReference.Uri != (Uri) null ? edmReference.Uri.OriginalString : (string) null;
            csdlReader.ignoreUnexpectedAttributesAndElements = this.ignoreUnexpectedAttributesAndElements;
            Version csdlVersion;
            CsdlModel csdlModel;
            if (csdlReader.TryParseCsdlFileToCsdlModel(out csdlVersion, out csdlModel))
            {
              if (!mainCsdlVersion.Equals(csdlVersion))
                this.errors.Add((EdmError) null);
              csdlModel.AddParentModelReferences(edmReference);
              referencedCsdlFiles.Add(csdlModel);
            }
            this.errors.AddRange((IEnumerable<EdmError>) csdlReader.errors);
          }
        }
      }
      return referencedCsdlFiles;
    }

    private bool TryParseCsdlFileToCsdlModel(out Version csdlVersion, out CsdlModel csdlModel)
    {
      csdlVersion = (Version) null;
      csdlModel = (CsdlModel) null;
      try
      {
        if (this.reader.NodeType != XmlNodeType.Element)
        {
          while (this.reader.Read() && this.reader.NodeType != XmlNodeType.Element)
            ;
        }
        if (this.reader.EOF)
        {
          this.RaiseEmptyFile();
          return false;
        }
        if (this.reader.LocalName != "Edmx" || !CsdlConstants.SupportedEdmxNamespaces.TryGetValue(this.reader.NamespaceURI, out csdlVersion))
        {
          this.RaiseError(EdmErrorCode.UnexpectedXmlElement, Strings.XmlParser_UnexpectedRootElement((object) this.reader.Name, (object) "Edmx"));
          return false;
        }
        csdlVersion = this.ParseEdmxElement(csdlVersion);
        IEnumerable<EdmError> errors;
        if (!this.csdlParser.GetResult(out csdlModel, out errors))
        {
          this.errors.AddRange(errors);
          if (this.HasIntolerableError())
            return false;
        }
      }
      catch (XmlException ex)
      {
        this.errors.Add(new EdmError((EdmLocation) new CsdlLocation(this.source, ex.LineNumber, ex.LinePosition), EdmErrorCode.XmlError, ex.Message));
        return false;
      }
      csdlModel.AddCurrentModelReferences((IEnumerable<IEdmReference>) this.edmReferences);
      return true;
    }

    private bool HasIntolerableError() => this.ignoreUnexpectedAttributesAndElements ? this.errors.Any<EdmError>((Func<EdmError, bool>) (error => error.ErrorCode != EdmErrorCode.UnexpectedXmlElement && error.ErrorCode != EdmErrorCode.UnexpectedXmlAttribute)) : this.errors.Any<EdmError>();

    private void ParseElement(string elementName, Dictionary<string, Action> elementParsers)
    {
      if (this.reader.IsEmptyElement)
      {
        this.reader.Read();
      }
      else
      {
        this.reader.Read();
        while (this.reader.NodeType != XmlNodeType.EndElement)
        {
          if (this.reader.NodeType == XmlNodeType.Element)
          {
            if (elementParsers.ContainsKey(this.reader.LocalName))
              elementParsers[this.reader.LocalName]();
            else
              this.ParseElement(this.reader.LocalName, CsdlReader.EmptyParserLookup);
          }
          else if (!this.reader.Read())
            break;
        }
        this.reader.Read();
      }
    }

    private Version ParseEdmxElement(Version edmxVersion)
    {
      string attributeValue = this.GetAttributeValue((string) null, "Version");
      Version version1 = (Version) null;
      if (attributeValue != null && (!CsdlReader.TryParseVersion(attributeValue, out version1) || version1.Major != edmxVersion.Major))
        this.RaiseError(EdmErrorCode.InvalidVersionNumber, Strings.EdmxParser_EdmxVersionMismatch);
      this.ParseElement("Edmx", this.edmxParserLookup);
      Version version2 = version1;
      return (object) version2 != null ? version2 : edmxVersion;
    }

    private string GetAttributeValue(string namespaceUri, string localName)
    {
      string namespaceUri1 = this.reader.NamespaceURI;
      string attributeValue = (string) null;
      for (bool flag = this.reader.MoveToFirstAttribute(); flag; flag = this.reader.MoveToNextAttribute())
      {
        if ((namespaceUri != null && this.reader.NamespaceURI == namespaceUri || string.IsNullOrEmpty(this.reader.NamespaceURI) || this.reader.NamespaceURI == namespaceUri1) && this.reader.LocalName == localName)
        {
          attributeValue = this.reader.Value;
          break;
        }
      }
      this.reader.MoveToElement();
      return attributeValue;
    }

    private void ParseRuntimeElement() => this.ParseTargetElement("Runtime", this.runtimeParserLookup);

    private void ParseDataServicesElement() => this.ParseTargetElement("DataServices", this.dataServicesParserLookup);

    private void ParseTargetElement(string elementName, Dictionary<string, Action> elementParsers)
    {
      if (!this.targetParsed)
      {
        this.targetParsed = true;
      }
      else
      {
        this.RaiseError(EdmErrorCode.UnexpectedXmlElement, Strings.EdmxParser_BodyElement((object) "DataServices"));
        elementParsers = CsdlReader.EmptyParserLookup;
      }
      this.ParseElement(elementName, elementParsers);
    }

    private void ParseConceptualModelsElement() => this.ParseElement("ConceptualModels", this.conceptualModelsParserLookup);

    private void ParseReferenceElement()
    {
      EdmReference edmReference = new EdmReference(new Uri(this.GetAttributeValue((string) null, "Uri"), UriKind.RelativeOrAbsolute));
      if (this.reader.IsEmptyElement)
      {
        this.reader.Read();
        this.edmReferences.Add((IEdmReference) edmReference);
      }
      else
      {
        this.reader.Read();
        while (this.reader.NodeType != XmlNodeType.EndElement)
        {
          do
            ;
          while (this.reader.NodeType == XmlNodeType.Whitespace && this.reader.Read());
          if (this.reader.NodeType == XmlNodeType.Element)
          {
            if (this.reader.LocalName == "Include")
            {
              IEdmInclude edmInclude = (IEdmInclude) new EdmInclude(this.GetAttributeValue((string) null, "Alias"), this.GetAttributeValue((string) null, "Namespace"));
              edmReference.AddInclude(edmInclude);
            }
            else if (this.reader.LocalName == "IncludeAnnotations")
            {
              IEdmIncludeAnnotations edmIncludeAnnotations = (IEdmIncludeAnnotations) new EdmIncludeAnnotations(this.GetAttributeValue((string) null, "TermNamespace"), this.GetAttributeValue((string) null, "Qualifier"), this.GetAttributeValue((string) null, "TargetNamespace"));
              edmReference.AddIncludeAnnotations(edmIncludeAnnotations);
            }
            else
            {
              if (this.reader.LocalName == "Annotation")
              {
                this.reader.Skip();
                this.RaiseError(EdmErrorCode.UnexpectedXmlElement, Strings.XmlParser_UnexpectedElement((object) this.reader.LocalName));
                continue;
              }
              this.RaiseError(EdmErrorCode.UnexpectedXmlElement, Strings.XmlParser_UnexpectedElement((object) this.reader.LocalName));
            }
            if (!this.reader.IsEmptyElement)
            {
              this.reader.Read();
              while (this.reader.NodeType == XmlNodeType.Whitespace && this.reader.Read())
                ;
            }
            this.reader.Read();
          }
          else
            break;
        }
        this.reader.Read();
        this.edmReferences.Add((IEdmReference) edmReference);
      }
    }

    private void ParseSchemaElement()
    {
      XmlReaderSettings settings = new XmlReaderSettings();
      if (this.reader is IXmlLineInfo reader && reader.HasLineInfo())
      {
        settings.LineNumberOffset = reader.LineNumber - 1;
        settings.LinePositionOffset = reader.LinePosition - 2;
      }
      using (StringReader input = new StringReader(this.reader.ReadOuterXml()))
      {
        using (XmlReader csdlReader = XmlReader.Create((TextReader) input, settings))
          this.csdlParser.AddReader(csdlReader, this.source);
      }
    }

    private void RaiseEmptyFile() => this.RaiseError(EdmErrorCode.EmptyFile, Strings.XmlParser_EmptySchemaTextReader);

    private CsdlLocation Location()
    {
      int number = 0;
      int position = 0;
      if (this.reader is IXmlLineInfo reader && reader.HasLineInfo())
      {
        number = reader.LineNumber;
        position = reader.LinePosition;
      }
      return new CsdlLocation(this.source, number, position);
    }

    private void RaiseError(EdmErrorCode errorCode, string errorMessage) => this.errors.Add(new EdmError((EdmLocation) this.Location(), errorCode, errorMessage));
  }
}
