// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitFileExtensionMetrics
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.Telemetry;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GitFileExtensionMetrics
  {
    private const string c_pipe = "|";
    private const string c_colon = ":";
    private readonly CommitMetadataKey m_commitKey;
    private readonly Dictionary<string, int> m_fileExtensionCounts = new Dictionary<string, int>();

    public GitFileExtensionMetrics(CommitMetadataKey commitKey) => this.m_commitKey = commitKey;

    public Dictionary<string, int> FileExtensionCounts => this.m_fileExtensionCounts;

    public void Add(TfsGitCommitChange change)
    {
      ArgumentUtility.CheckForNull<TfsGitCommitChange>(change, nameof (change));
      if (string.IsNullOrEmpty(change.ChildItem) || change.ObjectType != GitObjectType.Blob)
        return;
      if ((change.ChangeType & TfsGitChangeType.Delete) == TfsGitChangeType.Delete)
        return;
      string lower;
      try
      {
        lower = Path.GetExtension(change.ChildItem).ToLower();
      }
      catch (ArgumentException ex)
      {
        return;
      }
      if (!Regex.IsMatch(lower, "^\\.[A-Za-z0-9]+$"))
        return;
      int num;
      if (!this.m_fileExtensionCounts.TryGetValue(lower, out num))
        this.m_fileExtensionCounts.Add(lower, 1);
      else
        this.m_fileExtensionCounts[lower] = num + 1;
    }

    public void PublishKpi(
      IVssRequestContext requestContext,
      Guid repoId,
      Guid projectId,
      ProjectVisibility projectVisibility)
    {
      if (this.m_fileExtensionCounts.Count == 0)
        return;
      string str = string.Join("|", this.m_fileExtensionCounts.Select<KeyValuePair<string, int>, string>((Func<KeyValuePair<string, int>, string>) (x => x.Key + ":" + x.Value.ToString())));
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("Action", "VCFilesChanged");
      intelligenceData.Add("RepositoryId", repoId.ToString());
      intelligenceData.Add("PushId", (object) this.m_commitKey.PushId);
      intelligenceData.Add("FileExtensionChangeCounts", str);
      intelligenceData.AddDataspaceInformation(CustomerIntelligenceDataspaceType.Project, projectId.ToString(), ((int) projectVisibility).ToString((IFormatProvider) CultureInfo.InvariantCulture));
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, requestContext.ServiceHost.InstanceId, string.Empty, this.m_commitKey.PusherId, IdentityCuidHelper.GetCuidByVsid(requestContext, this.m_commitKey.PusherId), DateTime.UtcNow, "Microsoft.TeamFoundation.Git.Server", "TeamProject", intelligenceData);
      requestContext.PublishAppInsightsTelemetry("TFS/Git/VCFilesChanged", this.m_commitKey.PusherId, intelligenceData, (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "RepositoryId",
          "TFS.SourceControl.RepositoryId"
        },
        {
          "PushId",
          "TFS.SourceControl.PushId"
        },
        {
          "FileExtensionChangeCounts",
          "TFS.SourceControl.FileExtensionChangeCounts"
        }
      });
    }
  }
}
