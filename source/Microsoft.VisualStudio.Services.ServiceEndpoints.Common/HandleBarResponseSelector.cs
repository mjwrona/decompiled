// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.HandleBarResponseSelector
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
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
  public class HandleBarResponseSelector : ResponseSelector
  {
    public HandleBarResponseSelector(string resultTemplate, JToken replacementContext)
      : base(resultTemplate, (string) null, (string) null, replacementContext)
    {
    }

    protected override ResponseSelectorResult SelectInternal(HttpResponseMessage response)
    {
      List<string> stringList = new List<string>();
      ResponseSelectorResult responseSelectorResult = new ResponseSelectorResult();
      using (Stream result = response.Content.ReadAsStreamAsync().Result)
      {
        using (StreamReader input = new StreamReader(result))
        {
          JToken json;
          if (!response.Headers.GetEnumerator().MoveNext() && response.Headers.Contains("Content-Type") && !string.IsNullOrEmpty(response.Headers.GetValues("Content-Type").FirstOrDefault<string>()) && string.Equals(response.Headers.GetValues("Content-Type").FirstOrDefault<string>(), "application/xml", StringComparison.OrdinalIgnoreCase))
          {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
              DtdProcessing = DtdProcessing.Prohibit,
              MaxCharactersInDocument = 2097152,
              XmlResolver = (XmlResolver) null
            };
            using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
              json = XPathResponseSelector.ConvertElementToJSON(XPathResponseSelector.RemoveDefaultNamespace(XDocument.Load(reader)).Root);
          }
          else
          {
            char[] buffer = new char[2097152];
            int length = input.ReadBlock(buffer, 0, 2097152);
            if (!input.EndOfStream)
              throw new InvalidEndpointResponseException(Resources.ResponseSizeExceeded());
            json = JToken.Parse(new string(buffer, 0, length));
          }
          string template = new MustacheTemplateEngine().EvaluateTemplate(this.resultTemplate, EndpointMustacheHelper.GetMergedContext(json, this.replacementContext));
          stringList.Add(template);
        }
      }
      response.Dispose();
      responseSelectorResult.Result = (IList<string>) stringList;
      return responseSelectorResult;
    }

    public override void AddHeaders(HttpWebRequest request)
    {
      request.ContentType = "application/json";
      request.Accept = "application/json";
    }
  }
}
