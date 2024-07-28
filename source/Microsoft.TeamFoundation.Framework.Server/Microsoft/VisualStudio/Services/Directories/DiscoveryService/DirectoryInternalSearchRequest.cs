// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectoryInternalSearchRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class DirectoryInternalSearchRequest : DirectoryInternalRequest
  {
    internal string Query { get; set; }

    internal IEnumerable<string> TypesToSearch { get; set; }

    internal IEnumerable<string> FilterByAncestorEntityIds { get; set; }

    internal IEnumerable<string> FilterByEntityIds { get; set; }

    internal IEnumerable<string> PropertiesToSearch { get; set; }

    internal IEnumerable<string> PropertiesToReturn { get; set; }

    internal int MaxResults { get; set; }

    internal string PagingToken { get; set; }

    internal QueryType QueryType { get; set; }

    internal Guid ScopeId { get; set; }
  }
}
