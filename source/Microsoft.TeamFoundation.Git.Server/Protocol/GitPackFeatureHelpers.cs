// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.GitPackFeatureHelpers
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server.Protocol
{
  internal static class GitPackFeatureHelpers
  {
    public static GitPackFeature FeatureFromString(string featureName)
    {
      string str = featureName.Trim();
      if (str != null)
      {
        switch (str.Length)
        {
          case 5:
            if (str == "quiet")
              return GitPackFeature.Quiet;
            break;
          case 7:
            switch (str[0])
            {
              case 'n':
                if (str == "no-done")
                  return GitPackFeature.NoDone;
                break;
              case 's':
                if (str == "shallow")
                  return GitPackFeature.Shallow;
                break;
            }
            break;
          case 9:
            switch (str[0])
            {
              case 'm':
                if (str == "multi_ack")
                  return GitPackFeature.MultiAck;
                break;
              case 'o':
                if (str == "ofs-delta")
                  return GitPackFeature.OfsDelta;
                break;
              case 's':
                if (str == "side-band")
                  return GitPackFeature.SideBand;
                break;
              case 't':
                if (str == "thin-pack")
                  return GitPackFeature.ThinPack;
                break;
            }
            break;
          case 11:
            switch (str[0])
            {
              case 'd':
                if (str == "delete-refs")
                  return GitPackFeature.DeleteRefs;
                break;
              case 'i':
                if (str == "include-tag")
                  return GitPackFeature.IncludeTag;
                break;
              case 'n':
                if (str == "no-progress")
                  return GitPackFeature.NoProgress;
                break;
            }
            break;
          case 13:
            switch (str[0])
            {
              case 'r':
                if (str == "report-status")
                  return GitPackFeature.ReportStatus;
                break;
              case 's':
                if (str == "side-band-64k")
                  return GitPackFeature.SideBand | GitPackFeature.SideBand64K;
                break;
            }
            break;
          case 18:
            if (str == "multi_ack_detailed")
              return GitPackFeature.MultiAck | GitPackFeature.MultiAckDetailed;
            break;
        }
      }
      return GitPackFeature.None;
    }
  }
}
