// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildChangesContinuationToken
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build2.Server;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  public class BuildChangesContinuationToken
  {
    private BuildChangesContinuationToken(int? nextChangeId) => this.NextChangeId = nextChangeId;

    public BuildChangesContinuationToken(GetChangesResult changesResult) => this.NextChangeId = changesResult.NextChangeId;

    public int? NextChangeId { get; private set; }

    public override string ToString() => this.NextChangeId.HasValue ? this.NextChangeId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture) : string.Empty;

    public static bool TryParse(string value, out BuildChangesContinuationToken token)
    {
      token = (BuildChangesContinuationToken) null;
      int result = 0;
      if (!string.IsNullOrEmpty(value) && !int.TryParse(value, out result))
        return false;
      token = new BuildChangesContinuationToken(new int?(result));
      return true;
    }
  }
}
