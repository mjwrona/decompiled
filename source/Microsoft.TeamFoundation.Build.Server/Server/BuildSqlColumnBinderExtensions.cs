// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildSqlColumnBinderExtensions
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Server.Compatibility;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class BuildSqlColumnBinderExtensions
  {
    private static readonly XmlReaderSettings s_xmlReaderSettings = new XmlReaderSettings()
    {
      ConformanceLevel = ConformanceLevel.Fragment,
      IgnoreProcessingInstructions = true,
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = (XmlResolver) null
    };

    internal static string GetArtifactUriFromInt32(
      ref this SqlColumnBinder binder,
      SqlDataReader reader,
      string artifactTypeName,
      bool allowNull)
    {
      if (!allowNull)
        return DBHelper.CreateArtifactUri(artifactTypeName, binder.GetInt32((IDataReader) reader));
      int int32 = binder.GetInt32((IDataReader) reader, int.MinValue);
      return int32 != int.MinValue ? DBHelper.CreateArtifactUri(artifactTypeName, int32) : (string) null;
    }

    internal static string GetArtifactUriFromInt64(
      ref this SqlColumnBinder binder,
      SqlDataReader reader,
      string artifactTypeName,
      bool allowNull)
    {
      if (!allowNull)
        return DBHelper.CreateArtifactUri(artifactTypeName, binder.GetInt64((IDataReader) reader).ToString((IFormatProvider) CultureInfo.InvariantCulture));
      long int64 = binder.GetInt64((IDataReader) reader, long.MinValue);
      return int64 != long.MinValue ? DBHelper.CreateArtifactUri(artifactTypeName, int64.ToString((IFormatProvider) CultureInfo.InvariantCulture)) : (string) null;
    }

    internal static string GetArtifactUriFromString(
      ref this SqlColumnBinder binder,
      SqlDataReader reader,
      string artifactTypeName,
      bool allowNull)
    {
      string toolSpecificId = binder.GetString((IDataReader) reader, allowNull);
      return !string.IsNullOrEmpty(toolSpecificId) ? DBHelper.CreateArtifactUri(artifactTypeName, toolSpecificId) : (string) null;
    }

    internal static BuildReason GetBuildReason(
      ref this SqlColumnBinder binder,
      SqlDataReader reader)
    {
      return (BuildReason) binder.GetInt32((IDataReader) reader, 1);
    }

    internal static BuildStatus GetBuildStatus(
      ref this SqlColumnBinder binder,
      SqlDataReader reader)
    {
      return (BuildStatus) binder.GetInt32((IDataReader) reader, 32);
    }

    internal static BuildPhaseStatus GetBuildPhaseStatus(
      ref this SqlColumnBinder binder,
      SqlDataReader reader)
    {
      return (BuildPhaseStatus) binder.GetByte((IDataReader) reader, (byte) 0);
    }

    internal static BuildUpdate GetBuildUpdateFlags(
      ref this SqlColumnBinder binder,
      SqlDataReader reader)
    {
      return (BuildUpdate) binder.GetInt32((IDataReader) reader, 0);
    }

    internal static DefinitionTriggerType GetTriggerType(
      ref this SqlColumnBinder binder,
      SqlDataReader reader)
    {
      return (DefinitionTriggerType) binder.GetByte((IDataReader) reader, (byte) 1);
    }

    internal static DeleteOptions GetDeleteOptions(
      ref this SqlColumnBinder binder,
      SqlDataReader reader)
    {
      return (DeleteOptions) binder.GetInt32((IDataReader) reader, 31);
    }

    internal static WorkspaceMappingType GetWorkspaceMappingType(
      ref this SqlColumnBinder binder,
      SqlDataReader reader)
    {
      return (WorkspaceMappingType) binder.GetByte((IDataReader) reader, (byte) 0);
    }

    internal static string GetBuildItem(
      ref this SqlColumnBinder binder,
      SqlDataReader reader,
      bool allowNull)
    {
      return DBHelper.DBPathToServerPath(binder.GetString((IDataReader) reader, allowNull));
    }

    internal static string GetLocalPath(
      ref this SqlColumnBinder binder,
      SqlDataReader reader,
      bool allowNull)
    {
      return DBHelper.DBPathToLocalPath(binder.GetString((IDataReader) reader, allowNull));
    }

    internal static string GetVersionControlPath(
      ref this SqlColumnBinder binder,
      SqlDataReader reader,
      bool allowNull)
    {
      return DBHelper.DBPathToVersionControlPath(binder.GetString((IDataReader) reader, allowNull));
    }

    public static IList<int> XmlToListOfInt32(ref this SqlColumnBinder binder, SqlDataReader reader)
    {
      string s = binder.GetString((IDataReader) reader, true);
      if (string.IsNullOrEmpty(s))
        return (IList<int>) Array.Empty<int>();
      using (StringReader input = new StringReader(s))
      {
        using (XmlReader reader1 = XmlReader.Create((TextReader) input, BuildSqlColumnBinderExtensions.s_xmlReaderSettings))
        {
          reader1.Read();
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return (IList<int>) XmlUtility.ArrayOfObjectFromXml<int>(reader1, "int", false, BuildSqlColumnBinderExtensions.\u003C\u003EO.\u003C0\u003E__Int32FromXmlElement ?? (BuildSqlColumnBinderExtensions.\u003C\u003EO.\u003C0\u003E__Int32FromXmlElement = new System.Func<XmlReader, int>(XmlUtility.Int32FromXmlElement)));
        }
      }
    }

    public static IList<string> XmlToListOfString(
      ref this SqlColumnBinder binder,
      SqlDataReader reader)
    {
      string s = binder.GetString((IDataReader) reader, true);
      if (string.IsNullOrEmpty(s))
        return (IList<string>) Array.Empty<string>();
      using (StringReader input = new StringReader(s))
      {
        using (XmlReader reader1 = XmlReader.Create((TextReader) input, BuildSqlColumnBinderExtensions.s_xmlReaderSettings))
        {
          reader1.Read();
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return (IList<string>) XmlUtility.ArrayOfObjectFromXml<string>(reader1, "string", false, BuildSqlColumnBinderExtensions.\u003C\u003EO.\u003C1\u003E__StringFromXmlElement ?? (BuildSqlColumnBinderExtensions.\u003C\u003EO.\u003C1\u003E__StringFromXmlElement = new System.Func<XmlReader, string>(XmlUtility.StringFromXmlElement)));
        }
      }
    }

    public static IList<InformationField2010> XmlToListOfInformationField2010(
      ref this SqlColumnBinder binder,
      SqlDataReader reader)
    {
      string s = binder.GetString((IDataReader) reader, true);
      if (string.IsNullOrEmpty(s))
        return (IList<InformationField2010>) Array.Empty<InformationField2010>();
      using (StringReader input = new StringReader(s))
      {
        using (XmlReader reader1 = XmlReader.Create((TextReader) input, BuildSqlColumnBinderExtensions.s_xmlReaderSettings))
        {
          reader1.Read();
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return (IList<InformationField2010>) XmlUtility.ArrayOfObjectFromXml<InformationField2010>(reader1, "Field", false, BuildSqlColumnBinderExtensions.\u003C\u003EO.\u003C2\u003E__XmlToInformationField2010 ?? (BuildSqlColumnBinderExtensions.\u003C\u003EO.\u003C2\u003E__XmlToInformationField2010 = new System.Func<XmlReader, InformationField2010>(BuildSqlColumnBinderExtensions.XmlToInformationField2010)));
        }
      }
    }

    private static InformationField2010 XmlToInformationField2010(XmlReader reader)
    {
      InformationField2010 informationField2010 = new InformationField2010();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          if (reader.Name == "Name")
            informationField2010.Name = XmlUtility.StringFromXmlAttribute(reader);
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.Name == "Value")
            informationField2010.Value = XmlUtility.StringFromXmlElement(reader);
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return informationField2010;
    }

    public static IList<InformationField> XmlToListOfInformationField(
      ref this SqlColumnBinder binder,
      SqlDataReader reader)
    {
      string s = binder.GetString((IDataReader) reader, true);
      if (string.IsNullOrEmpty(s))
        return (IList<InformationField>) Array.Empty<InformationField>();
      using (StringReader input = new StringReader(s))
      {
        using (XmlReader reader1 = XmlReader.Create((TextReader) input, BuildSqlColumnBinderExtensions.s_xmlReaderSettings))
        {
          reader1.Read();
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return (IList<InformationField>) XmlUtility.ArrayOfObjectFromXml<InformationField>(reader1, "Field", false, BuildSqlColumnBinderExtensions.\u003C\u003EO.\u003C3\u003E__XmlToInformationField ?? (BuildSqlColumnBinderExtensions.\u003C\u003EO.\u003C3\u003E__XmlToInformationField = new System.Func<XmlReader, InformationField>(BuildSqlColumnBinderExtensions.XmlToInformationField)));
        }
      }
    }

    private static InformationField XmlToInformationField(XmlReader reader)
    {
      InformationField informationField = new InformationField();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          if (reader.Name == "Name")
            informationField.Name = reader.Value;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.Name == "Value")
            informationField.Value = reader.ReadElementContentAsString();
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return informationField;
    }

    public static IList<NameValueField> XmlToListOfNameValueField(
      ref this SqlColumnBinder binder,
      SqlDataReader reader)
    {
      string s = binder.GetString((IDataReader) reader, true);
      if (string.IsNullOrEmpty(s))
        return (IList<NameValueField>) Array.Empty<NameValueField>();
      using (StringReader input = new StringReader(s))
      {
        using (XmlReader reader1 = XmlReader.Create((TextReader) input, BuildSqlColumnBinderExtensions.s_xmlReaderSettings))
        {
          reader1.Read();
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          return (IList<NameValueField>) XmlUtility.ArrayOfObjectFromXml<NameValueField>(reader1, "Field", false, BuildSqlColumnBinderExtensions.\u003C\u003EO.\u003C4\u003E__XmlToNameValueField ?? (BuildSqlColumnBinderExtensions.\u003C\u003EO.\u003C4\u003E__XmlToNameValueField = new System.Func<XmlReader, NameValueField>(BuildSqlColumnBinderExtensions.XmlToNameValueField)));
        }
      }
    }

    private static NameValueField XmlToNameValueField(XmlReader reader)
    {
      NameValueField nameValueField = new NameValueField();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          if (reader.Name == "Name")
            nameValueField.Name = reader.Value;
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          if (reader.Name == "Value")
            nameValueField.Value = reader.ReadElementContentAsString();
          else
            reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return nameValueField;
    }

    public static string ToXml(NameValueField[] fields)
    {
      StringBuilder output = new StringBuilder();
      using (XmlWriter writer = XmlWriter.Create(output))
      {
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        XmlUtility.ArrayOfObjectToXml<NameValueField>(writer, fields, "Fields", "Field", false, false, BuildSqlColumnBinderExtensions.\u003C\u003EO.\u003C5\u003E__ToXml ?? (BuildSqlColumnBinderExtensions.\u003C\u003EO.\u003C5\u003E__ToXml = new Action<XmlWriter, string, NameValueField>(BuildSqlColumnBinderExtensions.ToXml)));
      }
      return output.ToString();
    }

    private static void ToXml(XmlWriter xmlWriter, string elementName, NameValueField field)
    {
      xmlWriter.WriteStartElement(elementName);
      xmlWriter.WriteAttributeString("Name", field.Name);
      xmlWriter.WriteElementString("Value", field.Value);
      xmlWriter.WriteEndElement();
    }
  }
}
