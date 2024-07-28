// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.FormatterExtensions
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.Spatial
{
  public static class FormatterExtensions
  {
    public static string Write(
      this SpatialFormatter<TextReader, TextWriter> formatter,
      ISpatial spatial)
    {
      Util.CheckArgumentNull((object) formatter, nameof (formatter));
      StringBuilder sb = new StringBuilder();
      using (TextWriter writerStream = (TextWriter) new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture))
        formatter.Write(spatial, writerStream);
      return sb.ToString();
    }

    public static string Write(
      this SpatialFormatter<XmlReader, XmlWriter> formatter,
      ISpatial spatial)
    {
      Util.CheckArgumentNull((object) formatter, nameof (formatter));
      StringBuilder output = new StringBuilder();
      XmlWriterSettings settings = new XmlWriterSettings()
      {
        OmitXmlDeclaration = true
      };
      using (XmlWriter writerStream = XmlWriter.Create(output, settings))
        formatter.Write(spatial, writerStream);
      return output.ToString();
    }
  }
}
