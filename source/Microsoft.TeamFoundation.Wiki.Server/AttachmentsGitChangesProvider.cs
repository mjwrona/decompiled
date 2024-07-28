// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.AttachmentsGitChangesProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class AttachmentsGitChangesProvider : IGitChangesProvider<WikiAttachmentChange>
  {
    public IList<GitChange> GetChanges(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      string mappedPath,
      WikiAttachmentChange attachmentChange)
    {
      List<GitChange> changes = new List<GitChange>();
      GitChange gitChange = new GitChange();
      gitChange.ChangeType = VersionControlChangeType.Add;
      GitItem gitItem = new GitItem();
      gitItem.Path = attachmentChange.Path;
      gitChange.Item = gitItem;
      gitChange.NewContent = new ItemContent()
      {
        Content = attachmentChange.Content,
        ContentType = ItemContentType.Base64Encoded
      };
      changes.Add(gitChange);
      return (IList<GitChange>) changes;
    }
  }
}
