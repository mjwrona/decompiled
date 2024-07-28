// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectoryOperationHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class DirectoryOperationHelper
  {
    internal static string[] FilterDirectoriesForAadGuestUsers(
      IVssRequestContext context,
      string[] directories)
    {
      return !DirectoryUtils.IsRequestByAadGuestUser(context) || directories == null || directories.Length == 0 ? directories : new HashSet<string>(((IEnumerable<string>) directories).Select<string, string>((Func<string, string>) (dir => !dir.Equals("aad") && !dir.Equals("src") && !dir.Equals("any") ? dir : "vsd"))).ToArray<string>();
    }
  }
}
