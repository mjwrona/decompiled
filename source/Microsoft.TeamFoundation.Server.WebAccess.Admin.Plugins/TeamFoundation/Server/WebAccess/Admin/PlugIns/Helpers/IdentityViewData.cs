// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Helpers.IdentityViewData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Helpers
{
  public class IdentityViewData
  {
    public IEnumerable<GraphViewModel> Identities { get; set; }

    public bool HasMore { get; set; }

    public int TotalIdentityCount { get; set; }

    public Guid CollectionHostId { get; set; }
  }
}
