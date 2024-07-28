// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.IMentionParser
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Mention.Server
{
  [InheritedExport]
  public interface IMentionParser
  {
    MentionContentType SupportedContentTypes { get; }

    IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> FindMentions(
      IVssRequestContext requestContext,
      IEnumerable<MentionCandidate> candidates);
  }
}
