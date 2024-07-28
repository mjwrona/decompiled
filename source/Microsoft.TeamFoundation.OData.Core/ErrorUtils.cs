// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ErrorUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Xml;

namespace Microsoft.OData
{
  internal static class ErrorUtils
  {
    internal const string ODataErrorMessageDefaultLanguage = "en-US";

    internal static void GetErrorDetails(ODataError error, out string code, out string message)
    {
      code = error.ErrorCode ?? string.Empty;
      message = error.Message ?? string.Empty;
    }

    internal static void WriteXmlError(
      XmlWriter writer,
      ODataError error,
      bool includeDebugInformation,
      int maxInnerErrorDepth)
    {
      string code;
      string message;
      ErrorUtils.GetErrorDetails(error, out code, out message);
      ODataInnerError innerError = includeDebugInformation ? error.InnerError : (ODataInnerError) null;
      ErrorUtils.WriteXmlError(writer, code, message, innerError, maxInnerErrorDepth);
    }

    private static void WriteXmlError(
      XmlWriter writer,
      string code,
      string message,
      ODataInnerError innerError,
      int maxInnerErrorDepth)
    {
      writer.WriteStartElement("m", "error", "http://docs.oasis-open.org/odata/ns/metadata");
      writer.WriteElementString("m", nameof (code), "http://docs.oasis-open.org/odata/ns/metadata", code);
      writer.WriteElementString("m", nameof (message), "http://docs.oasis-open.org/odata/ns/metadata", message);
      if (innerError != null)
        ErrorUtils.WriteXmlInnerError(writer, innerError, "innererror", 0, maxInnerErrorDepth);
      writer.WriteEndElement();
    }

    private static void WriteXmlInnerError(
      XmlWriter writer,
      ODataInnerError innerError,
      string innerErrorElementName,
      int recursionDepth,
      int maxInnerErrorDepth)
    {
      ++recursionDepth;
      if (recursionDepth > maxInnerErrorDepth)
        throw new ODataException(Strings.ValidationUtils_RecursionDepthLimitReached((object) maxInnerErrorDepth));
      writer.WriteStartElement("m", innerErrorElementName, "http://docs.oasis-open.org/odata/ns/metadata");
      string text1 = innerError.Message ?? string.Empty;
      writer.WriteStartElement("message", "http://docs.oasis-open.org/odata/ns/metadata");
      writer.WriteString(text1);
      writer.WriteEndElement();
      string text2 = innerError.TypeName ?? string.Empty;
      writer.WriteStartElement("type", "http://docs.oasis-open.org/odata/ns/metadata");
      writer.WriteString(text2);
      writer.WriteEndElement();
      string text3 = innerError.StackTrace ?? string.Empty;
      writer.WriteStartElement("stacktrace", "http://docs.oasis-open.org/odata/ns/metadata");
      writer.WriteString(text3);
      writer.WriteEndElement();
      if (innerError.InnerError != null)
        ErrorUtils.WriteXmlInnerError(writer, innerError.InnerError, "internalexception", recursionDepth, maxInnerErrorDepth);
      writer.WriteEndElement();
    }
  }
}
