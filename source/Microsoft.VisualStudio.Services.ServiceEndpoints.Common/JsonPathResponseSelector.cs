// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.JsonPathResponseSelector
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
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class JsonPathResponseSelector : ResponseSelector
  {
    private string _selector;
    private readonly string _keySelector;
    private static readonly string DelimiterSequence = "|";
    private static readonly string EscapeSequence = "\\";

    public JsonPathResponseSelector(
      string selector,
      string keySelector,
      string resultTemplate,
      string callbackContextTemplate,
      string callbackRequiredTemplate,
      JToken replacementContext)
      : base(resultTemplate, callbackContextTemplate, callbackRequiredTemplate, replacementContext)
    {
      this._selector = selector;
      this._keySelector = keySelector;
    }

    protected override ResponseSelectorResult SelectInternal(HttpResponseMessage response)
    {
      HttpResponseHeaders headers = response.Headers;
      List<string> stringList1 = new List<string>();
      ResponseSelectorResult responseSelectorResult = new ResponseSelectorResult();
      using (Stream result = response.Content.ReadAsStreamAsync().Result)
      {
        using (StreamReader streamReader = new StreamReader(result))
        {
          char[] buffer = new char[2097152];
          int length = streamReader.ReadBlock(buffer, 0, 2097152);
          if (!streamReader.EndOfStream)
            throw new InvalidEndpointResponseException(Resources.ResponseSizeExceeded());
          string json = new string(buffer, 0, length);
          List<string> keyList = new List<string>();
          List<string> stringList2 = new List<string>();
          IDictionary<string, string> dictionary1 = (IDictionary<string, string>) new Dictionary<string, string>();
          EndpointStringResolver endpointStringResolver = new EndpointStringResolver(this.replacementContext);
          if (!string.IsNullOrWhiteSpace(this._selector))
            this._selector = endpointStringResolver.ResolveVariablesInMustacheFormat(this._selector);
          JToken responseBody;
          try
          {
            responseBody = JToken.Parse(json);
          }
          catch (Exception ex)
          {
            throw new InvalidEndpointResponseException(Resources.InvalidJsonResponse((object) ex.Message));
          }
          if (!string.IsNullOrWhiteSpace(this._keySelector))
          {
            IEnumerable<JToken> source = responseBody.SelectTokens(this._keySelector);
            keyList.AddRange(source.Select<JToken, string>((Func<JToken, string>) (elem => elem.ToString())));
          }
          IEnumerable<JToken> jtokens = responseBody.SelectTokens(this._selector);
          if (!string.IsNullOrEmpty(this.resultTemplate))
            stringList2.AddRange((IEnumerable<string>) this.ApplyResultTemplate(jtokens));
          else
            stringList2.AddRange(jtokens.Select<JToken, string>((Func<JToken, string>) (elem => elem.ToString())));
          bool flag = this.ApplyCallbackRequiredTemplate(responseBody, headers, (IList<string>) stringList2);
          IDictionary<string, string> dictionary2 = this.ApplyContextTemplate(responseBody, headers);
          if (this._keySelector != null && keyList.Count != stringList2.Count)
            throw new InvalidJsonPathResponseSelectorException(Resources.KeyValueCountMismatch());
          List<string> list = (this._keySelector != null ? (IEnumerable<string>) this.GenerateConcatList(keyList, stringList2) : (IEnumerable<string>) stringList2).Distinct<string>().ToList<string>();
          responseSelectorResult.Result = (IList<string>) list;
          responseSelectorResult.CallbackContext = dictionary2;
          responseSelectorResult.CallbackRequired = flag;
        }
      }
      response.Dispose();
      return responseSelectorResult;
    }

    public override void AddHeaders(HttpWebRequest request)
    {
      request.ContentType = "application/json";
      request.Accept = "application/json";
    }

    private List<string> GenerateConcatList(List<string> keyList, List<string> valueList)
    {
      List<string> concatList = new List<string>();
      for (int index = 0; index < valueList.Count; ++index)
        concatList.Add(JsonPathResponseSelector.EscapeDelimiterSequence(keyList[index]) + JsonPathResponseSelector.DelimiterSequence + JsonPathResponseSelector.EscapeDelimiterSequence(valueList[index]));
      return concatList;
    }

    private static string EscapeDelimiterSequence(string input) => input.Replace(JsonPathResponseSelector.DelimiterSequence, JsonPathResponseSelector.EscapeSequence + JsonPathResponseSelector.DelimiterSequence);
  }
}
