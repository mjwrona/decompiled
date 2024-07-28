// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcChangesContinuationToken
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Newtonsoft.Json.Linq;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public sealed class TfvcChangesContinuationToken
  {
    private const string changesetIdJPropertyName = "c";
    private const string continuationPathJPropertyName = "cp";

    public TfvcChangesContinuationToken(int changesetId, string continuationPath)
    {
      this.ChangesetId = changesetId;
      this.ContinuationPath = continuationPath;
    }

    public static bool TryParseContinuationToken(
      int changesetId,
      string encodedToken,
      out TfvcChangesContinuationToken token)
    {
      token = (TfvcChangesContinuationToken) null;
      JObject rawTokenAsJObject;
      if (!ContinuationTokenHelper.TryParse(encodedToken, out rawTokenAsJObject) || rawTokenAsJObject["c"] == null || (string) rawTokenAsJObject["cp"] == null || changesetId != (int) rawTokenAsJObject["c"])
        return false;
      string continuationPath = (string) rawTokenAsJObject["cp"];
      token = new TfvcChangesContinuationToken(changesetId, continuationPath);
      return true;
    }

    public override string ToString() => ContinuationTokenHelper.EncodeToken(new JObject(new object[2]
    {
      (object) new JProperty("c", (object) this.ChangesetId),
      (object) new JProperty("cp", (object) this.ContinuationPath)
    }));

    public int ChangesetId { get; }

    public string ContinuationPath { get; }
  }
}
