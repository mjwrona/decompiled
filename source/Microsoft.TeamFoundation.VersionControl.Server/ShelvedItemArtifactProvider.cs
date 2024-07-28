// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ShelvedItemArtifactProvider
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Diagnostics;
using System.Text;
using System.Web;
using System.Xml;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ShelvedItemArtifactProvider : ArtifactProvider
  {
    protected override void Execute()
    {
      HttpContext current = HttpContext.Current;
      current.Response.ContentType = "text/xml";
      using (XmlTextWriter writer = new XmlTextWriter(current.Response.OutputStream, Encoding.UTF8))
      {
        writer.WriteStartDocument();
        writer.WriteProcessingInstruction("xml-stylesheet", this.StyleSheetAttributes("/V1.0/Transforms/shelvedItem.xsl"));
        string empty = string.Empty;
        try
        {
          empty = current.Request.Params["artifactMoniker"];
          if (string.IsNullOrEmpty(empty))
            throw new ArtifactIdentifierRequiredException();
          string serverItem;
          string shelvesetName;
          string shelvesetOwner;
          UriType type;
          ShelvedItemUri.Decode(empty, out serverItem, out shelvesetName, out shelvesetOwner, out type);
          ItemSpec itemSpec = new ItemSpec(serverItem, RecursionType.None, 0);
          this.VersionControlRequestContext.Validation.check((IValidatable) itemSpec, "serverPath", false);
          if (VersionControlPath.IsWildcard(serverItem))
            throw new WildcardNotAllowedException("WildcardNotAllowedException", new object[1]
            {
              (object) "artifactMoniker"
            });
          using (CommandQueryPendingSets queryPendingSets = new CommandQueryPendingSets(this.VersionControlRequestContext))
          {
            queryPendingSets.Execute((Workspace) null, shelvesetOwner, new ItemSpec[1]
            {
              itemSpec
            }, shelvesetName, PendingSetType.Shelveset, false);
            if (!queryPendingSets.PendingSets.MoveNext() || !queryPendingSets.PendingSets.Current.PendingChanges.MoveNext())
              throw new ItemNotFoundException(empty);
            WebView.ConditionAndSerializeShelvedItem(this.VersionControlRequestContext, type == UriType.Extended, (XmlWriter) writer, shelvesetName, shelvesetOwner, queryPendingSets.PendingSets.Current.PendingChanges.Current);
          }
        }
        catch (ResourceAccessException ex)
        {
          this.VersionControlRequestContext.RequestContext.TraceException(700026, TraceLevel.Info, TraceArea.Artifacts, TraceLayer.BusinessLogic, (Exception) ex);
          this.WriteException((Exception) new ItemNotFoundException(empty), (XmlWriter) writer);
        }
        catch (Exception ex)
        {
          this.VersionControlRequestContext.RequestContext.TraceException(700027, TraceArea.Artifacts, TraceLayer.BusinessLogic, ex);
          this.WriteException(ex, (XmlWriter) writer);
        }
      }
    }
  }
}
