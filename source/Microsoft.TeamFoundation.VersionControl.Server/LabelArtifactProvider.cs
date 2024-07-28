// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LabelArtifactProvider
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class LabelArtifactProvider : ArtifactProvider
  {
    protected override void Execute()
    {
      HttpContext current = HttpContext.Current;
      current.Response.ContentType = "text/xml";
      using (XmlTextWriter writer = new XmlTextWriter(current.Response.OutputStream, Encoding.UTF8))
      {
        writer.WriteStartDocument();
        writer.WriteProcessingInstruction("xml-stylesheet", this.StyleSheetAttributes("/V1.0/Transforms/{0}/Label.xsl"));
        string empty = string.Empty;
        try
        {
          empty = current.Request.QueryString["artifactMoniker"];
          if (string.IsNullOrEmpty(empty))
            throw new ArtifactIdentifierRequiredException();
          VersionControlLabel labelByLabelId = VersionControlLabel.FindLabelByLabelId(this.VersionControlRequestContext, int.Parse(empty, (IFormatProvider) CultureInfo.InvariantCulture));
          new XmlSerializer(typeof (VersionControlLabel)).Serialize((XmlWriter) writer, (object) labelByLabelId);
        }
        catch (ResourceAccessException ex)
        {
          this.VersionControlRequestContext.RequestContext.TraceException(700018, TraceLevel.Info, TraceArea.Artifacts, TraceLayer.BusinessLogic, (Exception) ex);
          this.WriteException((Exception) new LabelNotFoundException(empty), (XmlWriter) writer);
        }
        catch (Exception ex)
        {
          this.VersionControlRequestContext.RequestContext.TraceException(700019, TraceArea.Artifacts, TraceLayer.BusinessLogic, ex);
          this.WriteException(ex, (XmlWriter) writer);
        }
      }
    }
  }
}
