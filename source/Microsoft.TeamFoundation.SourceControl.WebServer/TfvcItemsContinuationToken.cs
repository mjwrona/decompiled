// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcItemsContinuationToken
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.VersionControl.Common;
using Newtonsoft.Json.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public sealed class TfvcItemsContinuationToken
  {
    private const string scopePathJPropertyName = "sp";
    private const string continuationPathJPropertyName = "cp";
    private const string changesetIdJPropertyName = "c";

    internal TfvcItemsContinuationToken(string scopePath, string continuationPath, int changesetId)
    {
      this.ScopePath = scopePath;
      this.ContinuationPath = continuationPath;
      this.ChangesetId = changesetId;
    }

    public static bool TryParseContinuationToken(
      string scopePath,
      string encodedToken,
      out TfvcItemsContinuationToken token)
    {
      token = (TfvcItemsContinuationToken) null;
      JObject rawTokenAsJObject;
      if (!ContinuationTokenHelper.TryParse(encodedToken, out rawTokenAsJObject) || (string) rawTokenAsJObject["sp"] == null || !scopePath.Equals((string) rawTokenAsJObject["sp"]) || (string) rawTokenAsJObject["cp"] == null || rawTokenAsJObject["c"] == null)
        return false;
      string continuationPath = (string) rawTokenAsJObject["cp"];
      if (!VersionControlPath.IsSubItem(continuationPath, scopePath) || (int) rawTokenAsJObject["c"] < 0)
        return false;
      token = new TfvcItemsContinuationToken(scopePath, continuationPath, (int) rawTokenAsJObject["c"]);
      return true;
    }

    public override string ToString() => ContinuationTokenHelper.EncodeToken(new JObject(new object[3]
    {
      (object) new JProperty("sp", (object) this.ScopePath),
      (object) new JProperty("cp", (object) this.ContinuationPath),
      (object) new JProperty("c", (object) this.ChangesetId)
    }));

    internal string ScopePath { get; }

    internal string ContinuationPath { get; }

    internal int ChangesetId { get; }
  }
}
