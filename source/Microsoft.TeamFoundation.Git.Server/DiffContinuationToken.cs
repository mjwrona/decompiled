// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.DiffContinuationToken
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class DiffContinuationToken
  {
    private const string baseTreeJPropertyName = "b";
    private const string targetTreeJPropertyName = "t";
    private const string pathJPropertyName = "p";
    private const string objectTypeJPropertyName = "o";
    private const string changeTypeJPropertyName = "c";

    internal DiffContinuationToken(
      Sha1Id baseTreeId,
      Sha1Id targetTreeId,
      NormalizedGitPath path,
      GitObjectType objectType,
      TfsGitChangeType changeType)
    {
      this.BaseTree = baseTreeId;
      this.TargetTree = targetTreeId;
      this.Path = path;
      this.ObjectType = objectType;
      this.ChangeType = changeType;
    }

    public static bool TryParseContinuationToken(
      Sha1Id baseTreeId,
      Sha1Id targetTreeId,
      string encodedToken,
      out DiffContinuationToken token,
      out string parseFailureReason)
    {
      JObject jobject;
      try
      {
        jobject = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(encodedToken)));
      }
      catch (Exception ex)
      {
        parseFailureReason = Resources.Format("InvalidDiffContinuationToken_InvalidEncoded", (object) (ex.GetType().Name + ": " + ex.Message));
        goto label_14;
      }
      string str1 = (string) jobject["b"];
      if (!baseTreeId.ToString().Equals(str1))
      {
        parseFailureReason = Resources.Format("InvalidDiffContinuationToken_BaseTreeDoesNotMatch", (object) baseTreeId, (object) str1);
      }
      else
      {
        string str2 = (string) jobject["t"];
        if (!targetTreeId.ToString().Equals(str2))
          parseFailureReason = Resources.Format("InvalidDiffContinuationToken_TargetTreeDoesNotMatch", (object) targetTreeId, (object) str2);
        else if ((string) jobject["p"] == null)
        {
          parseFailureReason = Resources.Get("InvalidDiffContinuationToken_PathMissing");
        }
        else
        {
          JToken jtoken1 = jobject["o"];
          GitObjectType result1;
          if (jtoken1 == null || !Enum.TryParse<GitObjectType>(jtoken1.ToString(), out result1) || result1 <= GitObjectType.Bad || result1 > GitObjectType.Blob)
          {
            parseFailureReason = Resources.Format("InvalidDiffContinuationToken_InvalidObjectType", (object) jtoken1);
          }
          else
          {
            JToken jtoken2 = jobject["c"];
            TfsGitChangeType result2;
            if (jtoken2 == null || !Enum.TryParse<TfsGitChangeType>(jtoken2.ToString(), out result2) || result2 < TfsGitChangeType.Add || result2 > TfsGitChangeType.Delete)
            {
              parseFailureReason = Resources.Format("InvalidDiffContinuationToken_InvalidChangeType", (object) jtoken2);
            }
            else
            {
              token = new DiffContinuationToken(baseTreeId, targetTreeId, new NormalizedGitPath((string) jobject["p"]), result1, result2);
              parseFailureReason = (string) null;
              return true;
            }
          }
        }
      }
label_14:
      token = (DiffContinuationToken) null;
      return false;
    }

    public override string ToString() => Convert.ToBase64String(Encoding.UTF8.GetBytes(new JObject(new object[5]
    {
      (object) new JProperty("b", (object) this.BaseTree.ToString()),
      (object) new JProperty("t", (object) this.TargetTree.ToString()),
      (object) new JProperty("p", (object) this.Path.ToString()),
      (object) new JProperty("o", (object) (int) this.ObjectType),
      (object) new JProperty("c", (object) (int) this.ChangeType)
    }).ToString(Formatting.None)));

    internal bool Matches(TfsGitDiffEntry diffEntry) => diffEntry != null && this.Path.ToString().Equals(diffEntry.RelativePath) && this.ChangeType == diffEntry.ChangeType && this.ObjectType == (!diffEntry.NewObjectId.HasValue ? diffEntry.OldObjectType : diffEntry.NewObjectType);

    internal void Validate(Sha1Id baseId, Sha1Id targetId)
    {
      if (!this.BaseTree.Equals(baseId) || !this.TargetTree.Equals(targetId))
        throw new ArgumentException("Invalid continuation token supplied.");
    }

    internal Sha1Id BaseTree { get; }

    internal Sha1Id TargetTree { get; }

    internal NormalizedGitPath Path { get; }

    internal GitObjectType ObjectType { get; }

    internal TfsGitChangeType ChangeType { get; }
  }
}
