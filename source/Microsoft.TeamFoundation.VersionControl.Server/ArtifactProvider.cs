// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ArtifactProvider
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Xml;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public abstract class ArtifactProvider : VersionControlPage
  {
    protected void WriteException(Exception exception, XmlWriter writer)
    {
      writer.WriteStartElement("Exception");
      writer.WriteElementString("ExceptionType", exception.GetType().Name);
      writer.WriteElementString("Message", exception.Message);
      writer.WriteEndElement();
    }

    protected string StyleSheetAttributes(string pageSuffix) => new StringBuilder("type=\"text/xsl\" href=\"").Append(this.RequestContext.ServiceHost.StaticContentDirectory.TrimEnd('/')).Append("/VersionControl").Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, pageSuffix, (object) this.RequestContext.ServiceHost.GetCulture(this.RequestContext).LCID)).Append('"').ToString();

    protected void Page_Load(object sender, EventArgs e)
    {
      HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      this.Execute();
    }

    protected abstract void Execute();
  }
}
