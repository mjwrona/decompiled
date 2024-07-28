// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.GraphException
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [Serializable]
  public class GraphException : Exception
  {
    public GraphException(string message)
      : base(message)
    {
      this.HttpStatusCode = HttpStatusCode.Unused;
    }

    public GraphException(HttpStatusCode statusCode, string message)
      : base(message)
    {
      this.HttpStatusCode = statusCode;
      this.ErrorMessage = message;
    }

    public GraphException(HttpStatusCode statusCode, string errorCode, string message)
      : base(message)
    {
      this.HttpStatusCode = statusCode;
      this.Code = errorCode;
      this.ErrorMessage = message;
    }

    public HttpStatusCode HttpStatusCode { get; set; }

    public string Code { get; set; }

    public string ErrorMessage { get; set; }

    public WebHeaderCollection ResponseHeaders { get; set; }

    public Dictionary<string, string> ExtendedErrors { get; set; }

    public ODataError ErrorResponse { get; set; }

    public string ResponseUri { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(base.ToString());
      stringBuilder.AppendFormat("[Url: {0}]", (object) this.ResponseUri);
      if (this.ResponseHeaders != null)
      {
        stringBuilder.Append("[Headers: ");
        foreach (string allKey in this.ResponseHeaders.AllKeys)
          stringBuilder.AppendFormat("{0}: {1},", (object) allKey, (object) this.ResponseHeaders[allKey]);
        stringBuilder.Append("]");
      }
      return stringBuilder.ToString();
    }
  }
}
