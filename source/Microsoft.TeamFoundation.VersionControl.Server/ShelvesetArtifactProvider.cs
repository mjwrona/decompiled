// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ShelvesetArtifactProvider
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ShelvesetArtifactProvider : ArtifactProvider
  {
    protected override void Execute()
    {
      try
      {
        this.EnterMethod(new MethodInformation(nameof (ShelvesetArtifactProvider), MethodType.Normal, EstimatedMethodCost.Low));
        string shelvesetName = string.Empty;
        string shelvesetOwner = string.Empty;
        HttpContext current = HttpContext.Current;
        current.Response.ContentType = "text/xml";
        using (XmlTextWriter writer = new XmlTextWriter(current.Response.OutputStream, Encoding.UTF8))
        {
          writer.WriteStartDocument();
          writer.WriteProcessingInstruction("xml-stylesheet", this.StyleSheetAttributes("/V1.0/Transforms/{0}/shelveset.xsl"));
          try
          {
            string shelvesetArtifactName = current.Request.Params["artifactMoniker"];
            if (string.IsNullOrEmpty(shelvesetArtifactName))
              throw new ArtifactIdentifierRequiredException();
            UriType type;
            ShelvesetUri.Decode(shelvesetArtifactName, out shelvesetName, out shelvesetOwner, out type);
            if (type == UriType.Normal)
            {
              List<Shelveset> shelvesetList = Shelveset.Query(this.VersionControlRequestContext, shelvesetOwner, shelvesetName, 0);
              if (shelvesetList.Count < 1)
                throw new ShelvesetNotFoundException(shelvesetName, shelvesetOwner);
              new XmlSerializer(typeof (Shelveset)).Serialize((XmlWriter) writer, (object) shelvesetList[0]);
            }
            else
            {
              ShelvesetEvent o = Shelveset.QueryShelveset(this.VersionControlRequestContext, shelvesetName, shelvesetOwner, 0, true, this.VersionControlRequestContext.VersionControlService.GetShelvesetArtifactChangesLimit(this.VersionControlRequestContext));
              new XmlSerializer(typeof (ShelvesetEvent)).Serialize((XmlWriter) writer, (object) o);
            }
          }
          catch (ResourceAccessException ex)
          {
            this.VersionControlRequestContext.RequestContext.TraceException(700024, TraceLevel.Info, TraceArea.Artifacts, TraceLayer.BusinessLogic, (Exception) ex);
            this.WriteException((Exception) new ShelvesetNotFoundException(shelvesetName, shelvesetOwner), (XmlWriter) writer);
          }
          catch (Exception ex)
          {
            this.VersionControlRequestContext.RequestContext.TraceException(700025, TraceArea.Artifacts, TraceLayer.BusinessLogic, ex);
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
