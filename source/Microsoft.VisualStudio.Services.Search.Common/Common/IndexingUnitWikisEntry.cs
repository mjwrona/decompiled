// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.IndexingUnitWikisEntry
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class IndexingUnitWikisEntry
  {
    public int IndexingUnitId { get; set; }

    public List<WikiV2> Wikis { get; set; }

    public List<string> GetBranchesToBeIndexed() => this.Wikis == null ? new List<string>() : this.Wikis.Select<WikiV2, string>((Func<WikiV2, string>) (w => w.Versions != null && w.Versions.Count<GitVersionDescriptor>() > 0 && w.Versions.First<GitVersionDescriptor>().VersionType == GitVersionType.Branch ? "refs/heads/" + w.Versions.First<GitVersionDescriptor>().Version : (string) null)).Where<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x))).Distinct<string>().ToList<string>();
  }
}
