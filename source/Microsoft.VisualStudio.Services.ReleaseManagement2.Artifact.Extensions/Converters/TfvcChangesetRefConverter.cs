// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Converters.TfvcChangesetRefConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Converters
{
  public static class TfvcChangesetRefConverter
  {
    public static IList<Change> ToReleaseChanges(IList<TfvcChangesetRef> tfvcChangesets) => (IList<Change>) tfvcChangesets.Select<TfvcChangesetRef, Change>(TfvcChangesetRefConverter.\u003C\u003EO.\u003C0\u003E__ToReleaseChange ?? (TfvcChangesetRefConverter.\u003C\u003EO.\u003C0\u003E__ToReleaseChange = new Func<TfvcChangesetRef, Change>(TfvcChangesetRefConverter.ToReleaseChange))).ToList<Change>();

    private static Change ToReleaseChange(this TfvcChangesetRef tfvcChangeset) => new Change()
    {
      Id = tfvcChangeset.ChangesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture),
      Message = tfvcChangeset.Comment,
      ChangeType = "TfsVersionControl",
      Author = tfvcChangeset.Author,
      Timestamp = new DateTime?(tfvcChangeset.CreatedDate),
      Location = new Uri(tfvcChangeset.Url),
      DisplayUri = new Uri(tfvcChangeset.Url),
      Pusher = tfvcChangeset.CheckedInBy.Id,
      PushedBy = tfvcChangeset.CheckedInBy
    };
  }
}
