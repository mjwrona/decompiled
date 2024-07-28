// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitItemsContinuationToken
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public sealed class GitItemsContinuationToken
  {
    private const string rootTreeJPropertyName = "r";
    private const string scopePathJPropertyName = "sp";
    private const string continuationPathJPropertyName = "cp";
    private const string continuationObjectTypeJPropertyName = "o";

    internal GitItemsContinuationToken(
      Sha1Id rootTreeId,
      NormalizedGitPath scopePath,
      NormalizedGitPath path,
      GitObjectType objectType)
    {
      this.RootTree = rootTreeId;
      this.ScopePath = scopePath;
      this.ContinuationPath = path;
      this.ContinuationObjectType = objectType;
    }

    public static bool TryParseContinuationToken(
      Sha1Id rootTreeId,
      string scopePath,
      string encodedToken,
      out GitItemsContinuationToken token)
    {
      token = (GitItemsContinuationToken) null;
      JObject rawTokenAsJObject;
      GitObjectType result;
      if (!ContinuationTokenHelper.TryParse(encodedToken, out rawTokenAsJObject) || !rootTreeId.ToString().Equals((string) rawTokenAsJObject["r"]) || (string) rawTokenAsJObject["sp"] == null || !scopePath.Equals((string) rawTokenAsJObject["sp"]) || (string) rawTokenAsJObject["cp"] == null || rawTokenAsJObject["o"] == null || !Enum.TryParse<GitObjectType>(rawTokenAsJObject["o"].ToString(), out result) || !GitItemsContinuationToken.IsValidObjectType(result))
        return false;
      string path = (string) rawTokenAsJObject["cp"];
      if (!scopePath.Equals("/") && !path.Equals(scopePath) && !path.StartsWith(scopePath + "/"))
        return false;
      token = new GitItemsContinuationToken(rootTreeId, new NormalizedGitPath(scopePath), new NormalizedGitPath(path), result);
      return true;
    }

    public override string ToString() => ContinuationTokenHelper.EncodeToken(new JObject(new object[4]
    {
      (object) new JProperty("r", (object) this.RootTree.ToString()),
      (object) new JProperty("sp", this.ScopePath.IsRoot ? (object) "/" : (object) this.ScopePath.ToString()),
      (object) new JProperty("cp", (object) this.ContinuationPath.ToString()),
      (object) new JProperty("o", (object) (int) this.ContinuationObjectType)
    }));

    private static bool IsValidObjectType(GitObjectType objType) => objType == GitObjectType.Blob || objType == GitObjectType.Tree || objType == GitObjectType.Commit;

    internal Sha1Id RootTree { get; }

    internal NormalizedGitPath ScopePath { get; }

    internal NormalizedGitPath ContinuationPath { get; }

    internal GitObjectType ContinuationObjectType { get; }
  }
}
