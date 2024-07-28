// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LatestItemVersionArtifactProvider
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

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class LatestItemVersionArtifactProvider : ArtifactProvider
  {
    protected override void Execute()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (LatestItemVersionArtifactProvider), MethodType.Normal, EstimatedMethodCost.Low));
        HttpContext current = HttpContext.Current;
        current.Response.ContentType = "text/xml";
        using (XmlTextWriter writer = new XmlTextWriter(current.Response.OutputStream, Encoding.UTF8))
        {
          writer.WriteStartDocument();
          writer.WriteProcessingInstruction("xml-stylesheet", this.StyleSheetAttributes("/V1.0/Transforms/LatestItemVersion.xsl"));
          string empty = string.Empty;
          try
          {
            empty = current.Request.Params["artifactMoniker"];
            if (string.IsNullOrEmpty(empty))
              throw new ArtifactIdentifierRequiredException();
            Item obj = Item.QueryItem(this.VersionControlRequestContext, int.Parse(empty, (IFormatProvider) CultureInfo.InvariantCulture), new ChangesetVersionSpec(int.MaxValue), true);
            WebView.ConditionAndSerializeItem(this.VersionControlRequestContext, (XmlWriter) writer, obj);
          }
          catch (ResourceAccessException ex)
          {
            this.VersionControlRequestContext.RequestContext.TraceException(700020, TraceLevel.Info, TraceArea.Artifacts, TraceLayer.BusinessLogic, (Exception) ex);
            this.WriteException((Exception) new ItemNotFoundException(empty), (XmlWriter) writer);
          }
          catch (Exception ex)
          {
            this.VersionControlRequestContext.RequestContext.TraceException(700021, TraceArea.Artifacts, TraceLayer.BusinessLogic, ex);
            this.WriteException(ex, (XmlWriter) writer);
          }
        }
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
