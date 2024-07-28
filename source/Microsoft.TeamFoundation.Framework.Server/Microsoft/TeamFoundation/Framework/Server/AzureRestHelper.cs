// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AzureRestHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class AzureRestHelper
  {
    public const string AzureXmlNamespace = "http://schemas.microsoft.com/windowsazure";

    public static XDocument Execute(
      string method,
      Uri requestUri,
      X509Certificate2 certificate,
      string version,
      string body = "")
    {
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(requestUri);
      httpWebRequest.ClientCertificates.Add((X509Certificate) certificate);
      httpWebRequest.Method = method;
      httpWebRequest.Headers.Add("x-ms-version", version);
      if (!string.IsNullOrEmpty(body))
      {
        byte[] bytes = Encoding.UTF8.GetBytes(body);
        using (Stream requestStream = httpWebRequest.GetRequestStream())
          requestStream.Write(bytes, 0, body.Length);
      }
      return XDocument.Load(httpWebRequest.GetResponse().GetResponseStream());
    }

    public static string GetAzureVip(
      string subscriptionId,
      string cloudServiceName,
      X509Certificate2 azureCert)
    {
      XDocument xdocument = AzureRestHelper.Execute("GET", new Uri("https://management.core.windows.net/" + subscriptionId + "/services/hostedservices/" + cloudServiceName + "/deploymentslots/production"), azureCert, "2012-03-01");
      XNamespace ns = (XNamespace) "http://schemas.microsoft.com/windowsazure";
      XName name = ns + "Vip";
      IEnumerable<XElement> source = xdocument.Descendants(name).Select(vip => new
      {
        vip = vip,
        endpoint = vip.ElementsBeforeSelf().FirstOrDefault<XElement>()
      }).Where(_param1 => (_param1.endpoint.Value.Equals("HttpIn") || _param1.endpoint.Value.Equals("HttpsIn")) && _param1.vip.Parent.Parent.Parent.Descendants(ns + "RoleName").Where<XElement>((Func<XElement, bool>) (role => role.Value.Equals("AT"))).Any<XElement>()).Select(_param1 => _param1.vip);
      return source.Count<XElement>() > 0 ? source.FirstOrDefault<XElement>().Value : (string) null;
    }
  }
}
