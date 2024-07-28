// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Metadata.ODataAtomErrorDeserializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Xml;

namespace Microsoft.OData.Metadata
{
  internal sealed class ODataAtomErrorDeserializer
  {
    internal static ODataError ReadErrorElement(
      BufferingXmlReader xmlReader,
      int maxInnerErrorDepth)
    {
      ODataError odataError = new ODataError();
      ODataAtomErrorDeserializer.DuplicateErrorElementPropertyBitMask elementsFoundBitField = ODataAtomErrorDeserializer.DuplicateErrorElementPropertyBitMask.None;
      if (!xmlReader.IsEmptyElement)
      {
        xmlReader.Read();
        do
        {
          switch (xmlReader.NodeType)
          {
            case XmlNodeType.Element:
              if (xmlReader.NamespaceEquals(xmlReader.ODataMetadataNamespace))
              {
                switch (xmlReader.LocalName)
                {
                  case "code":
                    ODataAtomErrorDeserializer.VerifyErrorElementNotFound(ref elementsFoundBitField, ODataAtomErrorDeserializer.DuplicateErrorElementPropertyBitMask.Code, "code");
                    odataError.ErrorCode = xmlReader.ReadElementValue();
                    goto label_9;
                  case "message":
                    ODataAtomErrorDeserializer.VerifyErrorElementNotFound(ref elementsFoundBitField, ODataAtomErrorDeserializer.DuplicateErrorElementPropertyBitMask.Message, "message");
                    odataError.Message = xmlReader.ReadElementValue();
                    goto label_9;
                  case "innererror":
                    ODataAtomErrorDeserializer.VerifyErrorElementNotFound(ref elementsFoundBitField, ODataAtomErrorDeserializer.DuplicateErrorElementPropertyBitMask.InnerError, "innererror");
                    odataError.InnerError = ODataAtomErrorDeserializer.ReadInnerErrorElement(xmlReader, 0, maxInnerErrorDepth);
                    goto label_9;
                  default:
                    goto label_8;
                }
              }
              else
                goto default;
            case XmlNodeType.EndElement:
label_9:
              continue;
            default:
label_8:
              xmlReader.Skip();
              goto case XmlNodeType.EndElement;
          }
        }
        while (xmlReader.NodeType != XmlNodeType.EndElement);
      }
      return odataError;
    }

    private static void VerifyErrorElementNotFound(
      ref ODataAtomErrorDeserializer.DuplicateErrorElementPropertyBitMask elementsFoundBitField,
      ODataAtomErrorDeserializer.DuplicateErrorElementPropertyBitMask elementFoundBitMask,
      string elementName)
    {
      if ((elementsFoundBitField & elementFoundBitMask) == elementFoundBitMask)
        throw new ODataException(Strings.ODataAtomErrorDeserializer_MultipleErrorElementsWithSameName((object) elementName));
      elementsFoundBitField |= elementFoundBitMask;
    }

    private static void VerifyInnerErrorElementNotFound(
      ref ODataAtomErrorDeserializer.DuplicateInnerErrorElementPropertyBitMask elementsFoundBitField,
      ODataAtomErrorDeserializer.DuplicateInnerErrorElementPropertyBitMask elementFoundBitMask,
      string elementName)
    {
      if ((elementsFoundBitField & elementFoundBitMask) == elementFoundBitMask)
        throw new ODataException(Strings.ODataAtomErrorDeserializer_MultipleInnerErrorElementsWithSameName((object) elementName));
      elementsFoundBitField |= elementFoundBitMask;
    }

    private static ODataInnerError ReadInnerErrorElement(
      BufferingXmlReader xmlReader,
      int recursionDepth,
      int maxInnerErrorDepth)
    {
      ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, maxInnerErrorDepth);
      ODataInnerError odataInnerError = new ODataInnerError();
      ODataAtomErrorDeserializer.DuplicateInnerErrorElementPropertyBitMask elementsFoundBitField = ODataAtomErrorDeserializer.DuplicateInnerErrorElementPropertyBitMask.None;
      if (!xmlReader.IsEmptyElement)
      {
        xmlReader.Read();
        do
        {
          switch (xmlReader.NodeType)
          {
            case XmlNodeType.Element:
              if (xmlReader.NamespaceEquals(xmlReader.ODataMetadataNamespace))
              {
                switch (xmlReader.LocalName)
                {
                  case "message":
                    ODataAtomErrorDeserializer.VerifyInnerErrorElementNotFound(ref elementsFoundBitField, ODataAtomErrorDeserializer.DuplicateInnerErrorElementPropertyBitMask.Message, "message");
                    odataInnerError.Message = xmlReader.ReadElementValue();
                    goto label_10;
                  case "type":
                    ODataAtomErrorDeserializer.VerifyInnerErrorElementNotFound(ref elementsFoundBitField, ODataAtomErrorDeserializer.DuplicateInnerErrorElementPropertyBitMask.TypeName, "type");
                    odataInnerError.TypeName = xmlReader.ReadElementValue();
                    goto label_10;
                  case "stacktrace":
                    ODataAtomErrorDeserializer.VerifyInnerErrorElementNotFound(ref elementsFoundBitField, ODataAtomErrorDeserializer.DuplicateInnerErrorElementPropertyBitMask.StackTrace, "stacktrace");
                    odataInnerError.StackTrace = xmlReader.ReadElementValue();
                    goto label_10;
                  case "internalexception":
                    ODataAtomErrorDeserializer.VerifyInnerErrorElementNotFound(ref elementsFoundBitField, ODataAtomErrorDeserializer.DuplicateInnerErrorElementPropertyBitMask.InternalException, "internalexception");
                    odataInnerError.InnerError = ODataAtomErrorDeserializer.ReadInnerErrorElement(xmlReader, recursionDepth, maxInnerErrorDepth);
                    goto label_10;
                  default:
                    goto label_9;
                }
              }
              else
                goto default;
            case XmlNodeType.EndElement:
label_10:
              continue;
            default:
label_9:
              xmlReader.Skip();
              goto case XmlNodeType.EndElement;
          }
        }
        while (xmlReader.NodeType != XmlNodeType.EndElement);
      }
      xmlReader.Read();
      return odataInnerError;
    }

    [Flags]
    private enum DuplicateErrorElementPropertyBitMask
    {
      None = 0,
      Code = 1,
      Message = 2,
      InnerError = 4,
    }

    [Flags]
    private enum DuplicateInnerErrorElementPropertyBitMask
    {
      None = 0,
      Message = 1,
      TypeName = 2,
      StackTrace = 4,
      InternalException = 8,
    }
  }
}
