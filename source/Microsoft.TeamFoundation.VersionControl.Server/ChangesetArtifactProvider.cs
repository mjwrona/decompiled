// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ChangesetArtifactProvider
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ChangesetArtifactProvider : ArtifactProvider
  {
    protected override void Execute()
    {
      HttpContext current = HttpContext.Current;
      int changeset1 = 0;
      current.Response.ContentType = "text/xml";
      using (XmlTextWriter writer = new XmlTextWriter(current.Response.OutputStream, Encoding.UTF8))
      {
        writer.WriteStartDocument();
        writer.WriteProcessingInstruction("xml-stylesheet", this.StyleSheetAttributes("/V1.0/Transforms/{0}/changeset.xsl"));
        try
        {
          string s = current.Request.Params["artifactMoniker"];
          if (string.IsNullOrEmpty(s))
            throw new ArtifactIdentifierRequiredException();
          if (!WebView.IsWebViewQuery(this.VersionControlRequestContext))
          {
            changeset1 = int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
            using (CommandQueryChangeset commandQueryChangeset = new CommandQueryChangeset(this.VersionControlRequestContext))
            {
              commandQueryChangeset.Execute(changeset1, true, false, this.VersionControlRequestContext.VersionControlService.GetChangesetArtifactChangesLimit(this.VersionControlRequestContext));
              Changeset changeset2 = commandQueryChangeset.Changeset;
              new XmlSerializer(typeof (Changeset)).Serialize((XmlWriter) writer, (object) changeset2);
            }
          }
          else
          {
            CheckinEvent o = Changeset.QueryCheckin(this.VersionControlRequestContext, int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture), true, this.VersionControlRequestContext.VersionControlService.GetChangesetArtifactChangesLimit(this.VersionControlRequestContext));
            new XmlSerializer(typeof (CheckinEvent)).Serialize((XmlWriter) writer, (object) o);
          }
        }
        catch (ResourceAccessException ex)
        {
          this.VersionControlRequestContext.RequestContext.TraceException(700017, TraceLevel.Info, TraceArea.Artifacts, TraceLayer.BusinessLogic, (Exception) ex);
          this.WriteException((Exception) new ChangesetNotFoundException(changeset1), (XmlWriter) writer);
        }
        catch (Exception ex)
        {
          this.VersionControlRequestContext.RequestContext.TraceException(700016, TraceArea.Artifacts, TraceLayer.BusinessLogic, ex);
          this.WriteException(ex, (XmlWriter) writer);
        }
      }
    }
  }
}
