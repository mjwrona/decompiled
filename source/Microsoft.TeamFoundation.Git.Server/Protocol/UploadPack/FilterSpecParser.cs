// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.FilterSpecParser
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack
{
  internal class FilterSpecParser
  {
    private const string c_CombinePrefix = "combine:";
    private const string c_BlobPrefix = "blob:";
    private const string c_TreePrefix = "tree:";

    internal static GitObjectFilter Parse(string filterSpec)
    {
      GitObjectFilter gitObjectFilter = new GitObjectFilter();
      ArgumentUtility.CheckStringForNullOrWhiteSpace(filterSpec, nameof (filterSpec));
      filterSpec = filterSpec.Trim();
      if (filterSpec.Length >= "combine:".Length && (int) filterSpec[0] == (int) "combine:"[0] && (int) filterSpec[1] == (int) "combine:"[1] && (int) filterSpec[2] == (int) "combine:"[2] && (int) filterSpec[3] == (int) "combine:"[3] && (int) filterSpec[4] == (int) "combine:"[4] && (int) filterSpec[5] == (int) "combine:"[5] && (int) filterSpec[6] == (int) "combine:"[6] && (int) filterSpec[7] == (int) "combine:"[7])
      {
        filterSpec = filterSpec.Substring("combine:".Length);
        ((IEnumerable<string>) filterSpec.Split('+')).ForEach<string>((Action<string>) (f => FilterSpecParser.ParseIndividualFilter(f, gitObjectFilter)));
      }
      else
        FilterSpecParser.ParseIndividualFilter(filterSpec, gitObjectFilter);
      return gitObjectFilter;
    }

    private static void ParseIndividualFilter(string filterSpec, GitObjectFilter filter)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(filterSpec, nameof (filterSpec));
      if (filterSpec.Length >= "blob:".Length && (int) filterSpec[0] == (int) "blob:"[0] && (int) filterSpec[1] == (int) "blob:"[1] && (int) filterSpec[2] == (int) "blob:"[2] && (int) filterSpec[3] == (int) "blob:"[3] && (int) filterSpec[4] == (int) "blob:"[4])
      {
        if (!(filterSpec.Substring("blob:".Length) == "none"))
          throw new GitProtocolException("Unsupported blob filter: " + filterSpec);
        filter.AllowBlobs = false;
      }
      else
      {
        int result;
        if (filterSpec.Length < "tree:".Length || (int) filterSpec[0] != (int) "tree:"[0] || (int) filterSpec[1] != (int) "tree:"[1] || (int) filterSpec[2] != (int) "tree:"[2] || (int) filterSpec[3] != (int) "tree:"[3] || (int) filterSpec[4] != (int) "tree:"[4] || !int.TryParse(filterSpec.Substring("tree:".Length), out result))
          throw new GitProtocolException("Unsupported filter spec: " + filterSpec);
        if (result != 0)
          throw new GitProtocolException("Unsupported tree depth: " + result.ToString());
        filter.AllowTrees = false;
      }
    }
  }
}
