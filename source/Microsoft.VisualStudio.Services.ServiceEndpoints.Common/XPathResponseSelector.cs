// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.XPathResponseSelector
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class XPathResponseSelector : ResponseSelector
  {
    private readonly string _selector;

    public XPathResponseSelector(string selector, string resultTemplate, JToken replacementContext)
      : base(resultTemplate, (string) null, (string) null, replacementContext)
    {
      this._selector = selector;
    }

    public override void AddHeaders(HttpWebRequest request)
    {
      request.ContentType = "application/xml";
      request.Accept = "application/xml";
    }

    public static JToken ConvertElementToJSON(XElement element)
    {
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit,
        XmlResolver = (XmlResolver) null
      };
      try
      {
        XmlDocument node = new XmlDocument();
        using (StringReader input = new StringReader(element.ToString()))
        {
          using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
            node.Load(reader);
        }
        return JToken.Parse(JsonConvert.SerializeXmlNode((XmlNode) node));
      }
      catch (Exception ex)
      {
        throw new XPathJTokenParseException(ex.Message);
      }
    }

    public static XDocument RemoveDefaultNamespace(XDocument xdoc)
    {
      string str = "";
      foreach (XAttribute attribute in xdoc.Root.Attributes())
      {
        if (attribute.IsNamespaceDeclaration && string.IsNullOrWhiteSpace(attribute.Name.Namespace.ToString()))
        {
          str = attribute.Value;
          break;
        }
      }
      foreach (XElement xelement in xdoc.Root.DescendantsAndSelf())
      {
        if (xelement.Name.Namespace != XNamespace.None && xelement.Name.NamespaceName.ToString().Equals(str))
          xelement.Name = XNamespace.None.GetName(xelement.Name.LocalName);
      }
      return xdoc;
    }

    protected override ResponseSelectorResult SelectInternal(HttpResponseMessage response)
    {
      List<string> stringList = new List<string>();
      using (Stream result = response.Content.ReadAsStreamAsync().Result)
      {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          MaxCharactersInDocument = 2097152,
          XmlResolver = (XmlResolver) null
        };
        using (XmlReader reader = XmlReader.Create(result, settings))
        {
          IEnumerable<XElement> xelements = XPathResponseSelector.RemoveDefaultNamespace(XDocument.Load(reader)).XPathSelectElements(this._selector);
          if (string.IsNullOrEmpty(this.resultTemplate))
          {
            stringList.AddRange(xelements.Select<XElement, string>((Func<XElement, string>) (element => element.Value)));
          }
          else
          {
            List<string> collection = this.ApplyResultTemplate((IEnumerable<JToken>) this.ConvertXmlElementsToJson(xelements));
            stringList.AddRange((IEnumerable<string>) collection);
            stringList = stringList.Distinct<string>().ToList<string>();
          }
        }
      }
      response.Dispose();
      stringList.Sort();
      return new ResponseSelectorResult((IList<string>) stringList, (IDictionary<string, string>) null, false);
    }

    private List<JToken> ConvertXmlElementsToJson(IEnumerable<XElement> elements)
    {
      List<JToken> json = new List<JToken>();
      json.AddRange(elements.Select<XElement, JToken>((Func<XElement, JToken>) (element => XPathResponseSelector.ConvertElementToJSON(element))));
      return json;
    }
  }
}
