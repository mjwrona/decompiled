// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.ResponseSelector
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  [InheritedExport]
  public abstract class ResponseSelector
  {
    protected const int MaxSize = 2097152;
    protected string resultTemplate;
    protected string callbackContextTemplate;
    protected string callbackRequiredTemplate;
    protected JToken replacementContext;

    public ResponseSelector()
    {
    }

    public ResponseSelector(
      string resultTemplate,
      string callbackContextTemplate,
      string callbackRequiredTemplate,
      JToken replacementContext)
    {
      this.resultTemplate = resultTemplate;
      this.callbackContextTemplate = callbackContextTemplate;
      this.callbackRequiredTemplate = callbackRequiredTemplate;
      this.replacementContext = replacementContext;
    }

    public ResponseSelectorResult Select(HttpResponseMessage response)
    {
      try
      {
        return this.SelectInternal(response);
      }
      catch (InvalidEndpointResponseException ex)
      {
        throw;
      }
      catch (InvalidJsonPathResponseSelectorException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new InvalidEndpointResponseException(Resources.SelectorParseError(), ex);
      }
    }

    protected List<string> ApplyResultTemplate(IEnumerable<JToken> resultTokens)
    {
      List<string> stringList = new List<string>();
      foreach (JToken resultToken in resultTokens)
      {
        JToken mergedContext = EndpointMustacheHelper.GetMergedContext(resultToken, this.replacementContext);
        stringList.Add(new MustacheTemplateEngine().EvaluateTemplate(this.resultTemplate, mergedContext));
      }
      return stringList;
    }

    protected IDictionary<string, string> ApplyContextTemplate(
      JToken responseBody,
      HttpResponseHeaders headersCollection)
    {
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      JToken resultHeadersToken = this.GetResultHeadersToken(headersCollection);
      JToken responseBodyToken = this.GetResponseBodyToken(responseBody);
      JToken replacementContext = (JToken) null;
      if (responseBody != null)
        replacementContext = EndpointMustacheHelper.GetMergedContext(responseBodyToken, this.replacementContext);
      if (resultHeadersToken != null && !string.IsNullOrEmpty(this.callbackContextTemplate))
        dictionary = (IDictionary<string, string>) JsonConvert.DeserializeObject<Dictionary<string, string>>(new MustacheTemplateEngine().EvaluateTemplate(this.callbackContextTemplate, EndpointMustacheHelper.GetMergedContext(resultHeadersToken, replacementContext)));
      return dictionary;
    }

    protected bool ApplyCallbackRequiredTemplate(
      JToken responseBody,
      HttpResponseHeaders headersCollection,
      IList<string> results)
    {
      bool result = false;
      JToken resultHeadersToken = this.GetResultHeadersToken(headersCollection);
      JToken responseBodyToken = this.GetResponseBodyToken(responseBody);
      JToken replacementContext = (JToken) null;
      if (responseBody != null)
        replacementContext = EndpointMustacheHelper.GetMergedContext(responseBodyToken, this.replacementContext);
      if (resultHeadersToken != null)
        replacementContext = EndpointMustacheHelper.GetMergedContext(resultHeadersToken, replacementContext);
      JToken resultCountToken = this.GetResultCountToken(results);
      if (resultCountToken != null && !string.IsNullOrEmpty(this.callbackRequiredTemplate) && replacementContext != null)
        bool.TryParse(new MustacheTemplateEngine().EvaluateTemplate(this.callbackRequiredTemplate, EndpointMustacheHelper.GetMergedContext(resultCountToken, replacementContext)), out result);
      return result;
    }

    private JToken GetResponseBodyToken(JToken responseBody)
    {
      JToken responseBodyToken = (JToken) null;
      if (responseBody != null)
        responseBodyToken = JToken.FromObject((object) new
        {
          response = responseBody
        });
      return responseBodyToken;
    }

    private JToken GetResultCountToken(IList<string> results)
    {
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      if (results != null)
        dictionary.Add("count", results.Count.ToString());
      return JToken.FromObject((object) new
      {
        result = dictionary
      });
    }

    private JToken GetResultHeadersToken(HttpResponseHeaders responseHeaders)
    {
      JToken resultHeadersToken = (JToken) null;
      IDictionary<string, string> dictionary1 = (IDictionary<string, string>) new Dictionary<string, string>();
      if (responseHeaders != null && responseHeaders.Count<KeyValuePair<string, IEnumerable<string>>>() > 0)
      {
        for (int index = 0; index < responseHeaders.Count<KeyValuePair<string, IEnumerable<string>>>(); ++index)
        {
          IDictionary<string, string> dictionary2 = dictionary1;
          KeyValuePair<string, IEnumerable<string>> keyValuePair = responseHeaders.ElementAt<KeyValuePair<string, IEnumerable<string>>>(index);
          string key = keyValuePair.Key;
          keyValuePair = responseHeaders.ElementAt<KeyValuePair<string, IEnumerable<string>>>(index);
          string str = keyValuePair.Value.First<string>();
          dictionary2.Add(key, str);
        }
        resultHeadersToken = JToken.FromObject((object) new
        {
          headers = dictionary1
        });
      }
      return resultHeadersToken;
    }

    public abstract void AddHeaders(HttpWebRequest request);

    protected abstract ResponseSelectorResult SelectInternal(HttpResponseMessage response);
  }
}
