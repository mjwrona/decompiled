// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.PlainTextResponseSelector
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class PlainTextResponseSelector : ResponseSelector
  {
    protected override ResponseSelectorResult SelectInternal(HttpResponseMessage response)
    {
      IList<string> result1 = (IList<string>) new List<string>();
      using (Stream result2 = response.Content.ReadAsStreamAsync().Result)
      {
        using (StreamReader streamReader = new StreamReader(result2))
        {
          char[] buffer = new char[2097152];
          int length = streamReader.ReadBlock(buffer, 0, 2097152);
          if (!streamReader.EndOfStream)
            throw new InvalidEndpointResponseException(Resources.ResponseSizeExceeded());
          string str = new string(buffer, 0, length);
          result1.Add(str);
        }
      }
      return new ResponseSelectorResult(result1, (IDictionary<string, string>) null, false);
    }

    public override void AddHeaders(HttpWebRequest request)
    {
      request.ContentType = "text/plain";
      request.Accept = "application/json";
    }
  }
}
