// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenPomUtility
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Exceptions;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public static class MavenPomUtility
  {
    private static readonly XmlWriterSettings writerSettings = new XmlWriterSettings()
    {
      Encoding = Encoding.UTF8,
      OmitXmlDeclaration = true,
      CloseOutput = false
    };

    public static MavenPomMetadata Parse(Stream stream)
    {
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      try
      {
        return MavenPomUtility.ParseExecute(stream);
      }
      catch (Exception ex)
      {
        throw new MavenPomParsingException(ex);
      }
    }

    public static void Serialize(this MavenPomMetadata pomMetadata, Stream stream)
    {
      using (XmlWriter xmlWriter = XmlWriter.Create(stream, MavenPomUtility.writerSettings))
        new XmlSerializer(typeof (MavenPomMetadata)).Serialize(xmlWriter, (object) pomMetadata);
    }

    private static MavenPomMetadata ParseExecute(Stream stream)
    {
      XDocument xdocument = new MavenPreprocessingXDocumentLoader(HtmlMathMlEntities.DefinedEntities).Load(stream);
      MavenPomUtility.StripAllXmlNamespaces((XContainer) xdocument);
      MavenPomUtility.StripAllXmlComments((XContainer) xdocument);
      MavenPomMetadata execute;
      using (XmlReader reader = xdocument.CreateReader())
        execute = (MavenPomMetadata) new XmlSerializer(typeof (MavenPomMetadata)).Deserialize(reader);
      XElement root = xdocument.Root;
      if (root == null || !root.Name.LocalName.Equals("project", StringComparison.OrdinalIgnoreCase))
        throw new XmlException(Resources.Error_PomMissingRootElement((object) "project"));
      execute.Properties = MavenPomUtility.ReadProperties(xdocument);
      execute.Prerequisites = MavenPomUtility.ReadPreRequisites(xdocument);
      return execute;
    }

    private static void StripAllXmlComments(XContainer container) => container.DescendantNodes().OfType<XComment>().Remove<XComment>();

    private static void StripAllXmlNamespaces(XContainer container)
    {
      foreach (XElement descendant in container.Descendants())
      {
        descendant.Name = XNamespace.None + descendant.Name.LocalName;
        descendant.ReplaceAttributes((object) descendant.Attributes().Where<XAttribute>((Func<XAttribute, bool>) (attr => attr.Name != XNamespace.None + "xmlns" && attr.Name.Namespace != XNamespace.Xmlns)).Select<XAttribute, XAttribute>((Func<XAttribute, XAttribute>) (attr => new XAttribute(XNamespace.None + attr.Name.LocalName, (object) attr.Value))));
      }
    }

    private static Dictionary<string, string> ReadProperties(XDocument doc) => MavenPomUtility.ReadNode(doc.Root.Elements().SingleOrDefault<XElement>((Func<XElement, bool>) (x => x.Name.LocalName == "properties")));

    private static Dictionary<string, string> ReadPreRequisites(XDocument doc) => MavenPomUtility.ReadNode(doc.Root.Elements().SingleOrDefault<XElement>((Func<XElement, bool>) (x => x.Name.LocalName == "prerequisites")));

    private static Dictionary<string, string> ReadNode(XElement node)
    {
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      if (node != null)
      {
        foreach (XElement element in node.Elements())
        {
          string str = element.Value;
          string localName = element.Name.LocalName;
          if (dictionary1.ContainsKey(localName))
          {
            if (!dictionary1[localName].Equals(str, StringComparison.OrdinalIgnoreCase))
            {
              Dictionary<string, string> dictionary2 = dictionary1;
              string key = localName;
              dictionary2[key] = dictionary2[key] + ", " + str;
            }
          }
          else
            dictionary1.Add(localName, str);
        }
      }
      return dictionary1;
    }

    public static void ValidateMaxSize(IVssRequestContext requestContext, Stream stream)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      long num = requestContext.GetService<IVssRegistryService>().GetValue<long>(requestContext, (RegistryQuery) "/Configuration/Packaging/Maven/Ingestion/Pom/MaxSize", true, 524288L);
      if (stream.Length > num)
        throw new MavenPomSizeLimitExceededException(Resources.Error_PomFileSizeLimitExceeded((object) num));
    }
  }
}
