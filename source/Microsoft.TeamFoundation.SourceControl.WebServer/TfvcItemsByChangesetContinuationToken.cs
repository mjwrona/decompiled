// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcItemsByChangesetContinuationToken
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.VersionControl.Common;
using Newtonsoft.Json.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public sealed class TfvcItemsByChangesetContinuationToken
  {
    private const string scopePathJPropertyName = "sp";
    private const string continuationPathJPropertyName = "cp";
    private const string baseChangesetIdJPropertyName = "bc";
    private const string targetChangesetIdJPropertyName = "tc";

    internal TfvcItemsByChangesetContinuationToken(
      string scopePath,
      string continuationPath,
      int baseChangesetId,
      int targetChangesetId)
    {
      this.ScopePath = scopePath;
      this.ContinuationPath = continuationPath;
      this.BaseChangesetId = baseChangesetId;
      this.TargetChangesetId = targetChangesetId;
    }

    public static bool TryParseContinuationToken(
      string scopePath,
      string encodedToken,
      out TfvcItemsByChangesetContinuationToken token)
    {
      token = (TfvcItemsByChangesetContinuationToken) null;
      JObject rawTokenAsJObject;
      if (!ContinuationTokenHelper.TryParse(encodedToken, out rawTokenAsJObject) || (string) rawTokenAsJObject["sp"] == null || !scopePath.Equals((string) rawTokenAsJObject["sp"]) || (string) rawTokenAsJObject["cp"] == null || !((int?) rawTokenAsJObject["bc"]).HasValue || !((int?) rawTokenAsJObject["tc"]).HasValue)
        return false;
      string continuationPath = (string) rawTokenAsJObject["cp"];
      if (!VersionControlPath.IsSubItem(continuationPath, scopePath))
        return false;
      int num = (int) rawTokenAsJObject["bc"];
      if (num <= 0 || (int) rawTokenAsJObject["tc"] < num)
        return false;
      token = new TfvcItemsByChangesetContinuationToken(scopePath, continuationPath, (int) rawTokenAsJObject["bc"], (int) rawTokenAsJObject["tc"]);
      return true;
    }

    public override string ToString() => ContinuationTokenHelper.EncodeToken(new JObject(new object[4]
    {
      (object) new JProperty("sp", (object) this.ScopePath),
      (object) new JProperty("cp", (object) this.ContinuationPath),
      (object) new JProperty("bc", (object) this.BaseChangesetId),
      (object) new JProperty("tc", (object) this.TargetChangesetId)
    }));

    internal string ScopePath { get; }

    internal string ContinuationPath { get; }

    internal int BaseChangesetId { get; }

    internal int TargetChangesetId { get; }
  }
}
