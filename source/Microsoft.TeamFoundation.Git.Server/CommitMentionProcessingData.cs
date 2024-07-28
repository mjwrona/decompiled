// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CommitMentionProcessingData
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Xml;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class CommitMentionProcessingData
  {
    public int PullRequestId { get; set; }

    public static XmlNode CreateCommitMentionProcessingDataXml(int pullRequestId) => TeamFoundationSerializationUtility.SerializeToXml((object) new CommitMentionProcessingData()
    {
      PullRequestId = pullRequestId
    });

    public static void ParseCommitMentionProcessingDataXml(XmlNode xmlData, out int pullRequestId)
    {
      ArgumentUtility.CheckForNull<XmlNode>(xmlData, nameof (xmlData));
      CommitMentionProcessingData mentionProcessingData = TeamFoundationSerializationUtility.Deserialize<CommitMentionProcessingData>(xmlData);
      pullRequestId = mentionProcessingData.PullRequestId;
    }
  }
}
