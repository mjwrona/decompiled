// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionedItemArtifactProvider
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
  public class VersionedItemArtifactProvider : ArtifactProvider
  {
    protected override void Execute()
    {
      HttpContext current1 = HttpContext.Current;
      current1.Response.ContentType = "text/xml";
      using (XmlTextWriter writer = new XmlTextWriter(current1.Response.OutputStream, Encoding.UTF8))
      {
        writer.WriteStartDocument();
        writer.WriteProcessingInstruction("xml-stylesheet", this.StyleSheetAttributes("/V1.0/Transforms/VersionedItem.xsl"));
        string empty = string.Empty;
        try
        {
          empty = current1.Request.Params["artifactMoniker"];
          if (string.IsNullOrEmpty(empty))
            throw new ArtifactIdentifierRequiredException();
          string serverItem;
          int changeset;
          int deletionId;
          UriType type;
          VersionedItemUri.Decode(empty, out serverItem, out changeset, out deletionId, out type);
          ItemSpec itemSpec = new ItemSpec(serverItem, RecursionType.None, deletionId);
          this.VersionControlRequestContext.Validation.check((IValidatable) itemSpec, "serverPath", false);
          if (VersionControlPath.IsWildcard(serverItem))
            throw new WildcardNotAllowedException("WildcardNotAllowedException", new object[1]
            {
              (object) "artifactMoniker"
            });
          using (CommandQueryItems<ItemSet, Item> commandQueryItems = new CommandQueryItems<ItemSet, Item>(this.VersionControlRequestContext))
          {
            commandQueryItems.Execute((Workspace) null, new ItemSpec[1]
            {
              itemSpec
            }, (VersionSpec) new ChangesetVersionSpec(changeset), (DeletedState) (deletionId == 0 ? 0 : 1), ItemType.Any, true, 0);
            StreamingCollection<ItemSet> itemSets = commandQueryItems.ItemSets;
            ItemSet itemSet = itemSets.MoveNext() ? itemSets.Current : throw new ItemNotFoundException(empty);
            if (!itemSet.Items.MoveNext())
              throw new ItemNotFoundException(empty);
            Item current2 = itemSet.Items.Current;
            if (deletionId != current2.DeletionId)
              throw new ItemNotFoundException(empty);
            WebView.ConditionAndSerializeItem(this.VersionControlRequestContext, type == UriType.Extended, (XmlWriter) writer, current2);
          }
        }
        catch (ResourceAccessException ex)
        {
          this.VersionControlRequestContext.RequestContext.TraceException(700022, TraceLevel.Info, TraceArea.Artifacts, TraceLayer.BusinessLogic, (Exception) ex);
          this.WriteException((Exception) new ItemNotFoundException(empty), (XmlWriter) writer);
        }
        catch (Exception ex)
        {
          this.VersionControlRequestContext.RequestContext.TraceException(700023, TraceArea.Artifacts, TraceLayer.BusinessLogic, ex);
          this.WriteException(ex, (XmlWriter) writer);
        }
      }
    }
  }
}
