// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.EndpointMustacheHelper
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public static class EndpointMustacheHelper
  {
    public const string DefaultResultKeyName = "defaultResultKey";

    public static JToken CreateReplacementContext(
      ServiceEndpoint serviceEndpoint,
      Dictionary<string, string> parameters,
      Guid projectId = default (Guid),
      OAuthConfiguration oAuthConfiguration = null,
      string initialContextTemplate = null,
      ServiceEndpointType serviceEndpointType = null,
      string[] whiteListedUrls = null)
    {
      IDictionary<string, string> dictionary1 = (IDictionary<string, string>) new Dictionary<string, string>();
      IList<string> stringList = (IList<string>) new List<string>();
      if (serviceEndpoint != null)
      {
        dictionary1 = (IDictionary<string, string>) new Dictionary<string, string>(serviceEndpoint.Data, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (serviceEndpointType != null && serviceEndpointType.InputDescriptors != null)
        {
          foreach (InputDescriptor inputDescriptor in serviceEndpointType.InputDescriptors)
          {
            if (inputDescriptor.IsConfidential && dictionary1.ContainsKey(inputDescriptor.Id))
              dictionary1.Remove(inputDescriptor.Id);
          }
        }
        string str = string.Empty;
        if (oAuthConfiguration != null && oAuthConfiguration.Url != (Uri) null)
          str = oAuthConfiguration.Url.AbsoluteUri;
        else if (serviceEndpoint.Url != (Uri) null)
          str = serviceEndpoint.Url.AbsoluteUri;
        if (!string.IsNullOrWhiteSpace(str))
        {
          dictionary1.Add("url", str);
          stringList.Add(str);
        }
      }
      IDictionary<string, string> dictionary2 = (IDictionary<string, string>) new Dictionary<string, string>();
      dictionary2.Add("teamProject", projectId.ToString());
      IDictionary<string, string> dictionary3 = (IDictionary<string, string>) new Dictionary<string, string>();
      if (!string.IsNullOrEmpty(initialContextTemplate))
      {
        initialContextTemplate = new EndpointStringResolver(JToken.FromObject((object) new
        {
          system = new
          {
            utcNow = DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffff'Z'")
          }
        })).ResolveVariablesInMustacheFormat(initialContextTemplate);
        dictionary3 = (IDictionary<string, string>) JsonConvert.DeserializeObject<Dictionary<string, string>>(initialContextTemplate);
      }
      if (whiteListedUrls != null)
      {
        foreach (string whiteListedUrl in whiteListedUrls)
        {
          if (!string.IsNullOrWhiteSpace(whiteListedUrl))
            stringList.Add(whiteListedUrl);
        }
      }
      IList<string> list = (IList<string>) dictionary3.Keys.ToList<string>();
      JToken o = serviceEndpoint == null ? JToken.FromObject((object) new
      {
        configuration = oAuthConfiguration,
        system = dictionary2,
        systemWhiteListedUrlList = stringList
      }) : JToken.FromObject((object) new
      {
        endpoint = dictionary1,
        configuration = oAuthConfiguration,
        system = dictionary2,
        systemWhiteListedUrlList = stringList
      });
      if (parameters != null)
      {
        foreach (string key in (IEnumerable<string>) list)
        {
          if (parameters.ContainsKey(key))
            dictionary3[key] = parameters[key];
        }
        foreach (KeyValuePair<string, string> parameter in parameters)
          o.Last.AddAfterSelf((object) new JProperty(parameter.Key, (object) parameter.Value));
        foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) dictionary3)
        {
          if (!parameters.ContainsKey(keyValuePair.Key))
            o.Last.AddAfterSelf((object) new JProperty(keyValuePair.Key, (object) keyValuePair.Value));
        }
      }
      return JToken.FromObject((object) o);
    }

    public static JToken GetMergedContext(JToken result, JToken replacementContext)
    {
      JToken mergedContext;
      if (replacementContext?.Last != null)
      {
        JToken jtoken = replacementContext.DeepClone();
        if (EndpointMustacheHelper.IsResultTypePrimitive(result.Type))
          result = EndpointMustacheHelper.ConvertToJsonObject(result.ToString());
        foreach (JToken content in (IEnumerable<JToken>) result)
        {
          if (content.Type == JTokenType.Property && jtoken[(object) ((JProperty) content).Name] != null)
            jtoken[(object) ((JProperty) content).Name].Parent.Remove();
          jtoken.Last.AddAfterSelf((object) content);
        }
        mergedContext = jtoken;
      }
      else
        mergedContext = result;
      return mergedContext;
    }

    private static JToken ConvertToJsonObject(string resultValue) => JToken.FromObject((object) new Dictionary<string, string>()
    {
      {
        "defaultResultKey",
        resultValue
      }
    });

    private static bool IsResultTypePrimitive(JTokenType resulTokenType) => !resulTokenType.Equals((object) JTokenType.Object) && !resulTokenType.Equals((object) JTokenType.Array);
  }
}
