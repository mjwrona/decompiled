// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiPagesOrderReader
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class WikiPagesOrderReader : IWikiPagesOrderReader, IDisposable
  {
    private Dictionary<GitPath, List<string>> orderedPageTitlesMap = new Dictionary<GitPath, List<string>>();
    private const int OffendingOrderFileLineLength = 1000;

    public List<string> GetOrderedPageTitles(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      GitPath orderFilePath,
      out bool createNew)
    {
      createNew = false;
      GitItemDescriptor itemDescriptor = new GitItemDescriptor()
      {
        Path = orderFilePath.Path,
        RecursionLevel = VersionControlRecursionType.None,
        Version = versionDescriptor.Version,
        VersionType = versionDescriptor.VersionType,
        VersionOptions = versionDescriptor.VersionOptions
      };
      List<string> orderedPageTitles;
      if (!this.orderedPageTitlesMap.TryGetValue(orderFilePath, out orderedPageTitles))
      {
        try
        {
          GitItem gitItem = GitItemUtility.RetrieveItemModels(requestContext, (UrlHelper) null, repository, itemDescriptor, true, false, (long) int.MaxValue).FirstOrDefault<GitItem>();
          FileContentMetadata contentMetadata = gitItem.ContentMetadata;
          int numOffendingLines = 0;
          int sumOfLengthOfOffendingLinesPerOrderFile = 0;
          using (Stream fileContentStream = GitFileUtility.GetFileContentStream(repository, new Sha1Id(gitItem.ObjectId)))
          {
            orderedPageTitles = new List<string>();
            VersionControlFileReader.ReadFileLines(fileContentStream, contentMetadata.Encoding).ForEach((Action<string>) (title =>
            {
              if (string.IsNullOrWhiteSpace(title))
                return;
              orderedPageTitles.Add(title.Trim());
              if (title.Length <= 1000)
                return;
              ++numOffendingLines;
              sumOfLengthOfOffendingLinesPerOrderFile += title.Length;
            }));
            this.orderedPageTitlesMap.Add(orderFilePath, orderedPageTitles);
          }
          if (numOffendingLines > 0)
            this.PublishOffendingLinesCi(requestContext, repository, numOffendingLines, sumOfLengthOfOffendingLinesPerOrderFile);
        }
        catch (GitItemNotFoundException ex)
        {
          createNew = true;
        }
      }
      return orderedPageTitles ?? new List<string>();
    }

    private void PublishOffendingLinesCi(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      int numOffendingLines,
      int sumOfLengthOfOffendingLinesPerOrderFile)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("projectId", (object) repository.Key.ProjectId);
      properties.Add("repositoryId", (object) repository.Key.RepoId);
      properties.Add(nameof (numOffendingLines), (double) numOffendingLines);
      properties.Add(nameof (sumOfLengthOfOffendingLinesPerOrderFile), (double) sumOfLengthOfOffendingLinesPerOrderFile);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Wiki", "LargeOrderFile", properties);
    }

    public void Dispose() => this.orderedPageTitlesMap = (Dictionary<GitPath, List<string>>) null;
  }
}
