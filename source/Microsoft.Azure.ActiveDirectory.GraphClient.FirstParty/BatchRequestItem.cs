// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.BatchRequestItem
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using System;
using System.Net;
using System.Text;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class BatchRequestItem
  {
    public BatchRequestItem(
      string httpMethod,
      bool isChangesetRequired,
      Uri requestUri,
      WebHeaderCollection headers,
      string body)
    {
      this.Method = httpMethod;
      this.IsChangesetRequired = isChangesetRequired;
      this.RequestUri = requestUri;
      this.Headers = headers;
      this.Body = body;
    }

    public string Method { get; set; }

    public Uri RequestUri { get; set; }

    public Guid RequestId { get; set; }

    public bool IsChangesetRequired { get; set; }

    public Guid ChangeSetId { get; set; }

    public WebHeaderCollection Headers { get; set; }

    public string Body { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("--batch_{0}", (object) this.RequestId.ToString());
      stringBuilder.AppendLine();
      if (this.IsChangesetRequired)
      {
        stringBuilder.AppendFormat("Content-Type: multipart/mixed; boundary=changeset_{0}", (object) this.ChangeSetId.ToString());
        stringBuilder.AppendLine();
        stringBuilder.AppendLine();
        stringBuilder.AppendFormat("--changeset_{0}", (object) this.ChangeSetId.ToString());
        stringBuilder.AppendLine();
      }
      stringBuilder.Append("Content-Type: application/http");
      stringBuilder.AppendLine();
      stringBuilder.Append("Content-Transfer-Encoding: binary");
      stringBuilder.AppendLine();
      stringBuilder.AppendLine();
      stringBuilder.AppendFormat("{0} {1} HTTP/1.1", (object) this.Method, (object) this.RequestUri);
      stringBuilder.AppendLine();
      if (this.Headers == null)
        this.Headers = new WebHeaderCollection();
      this.Headers[HttpRequestHeader.ContentType] = "application/json;odata=minimalmetadata";
      this.Headers[HttpRequestHeader.Accept] = "application/json;odata=minimalmetadata";
      this.Headers["Prefer"] = "return-content";
      this.Headers[HttpRequestHeader.AcceptCharset] = "UTF-8";
      foreach (string key in this.Headers.Keys)
      {
        stringBuilder.AppendFormat("{0}: {1}", (object) key, (object) this.Headers[key]);
        stringBuilder.AppendLine();
      }
      if (!string.IsNullOrEmpty(this.Body))
      {
        stringBuilder.AppendLine();
        stringBuilder.Append(this.Body);
      }
      stringBuilder.AppendLine();
      return stringBuilder.ToString();
    }
  }
}
