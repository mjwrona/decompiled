// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.LanguageMetadataAnalyzerJobData
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Xml;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public class LanguageMetadataAnalyzerJobData
  {
    public const int CurrentVersion = 1;

    public static XmlNode ToXml(Guid projectId, Guid repoId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(repoId, nameof (repoId));
      return TeamFoundationSerializationUtility.SerializeToXml((object) new LanguageMetadataAnalyzerJobData()
      {
        Version = new int?(1),
        ProjectId = projectId,
        RepositoryId = repoId
      });
    }

    public static LanguageMetadataAnalyzerJobData FromXml(XmlNode jobData) => TeamFoundationSerializationUtility.Deserialize<LanguageMetadataAnalyzerJobData>(jobData);

    public Guid ProjectId { get; set; }

    public Guid RepositoryId { get; set; }

    public int? Version { get; set; }
  }
}
