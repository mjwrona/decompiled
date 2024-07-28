// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitFileDiffsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebApi.Legacy;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientInternalUseOnly(true)]
  public class GitFileDiffsController : GitApiController
  {
    private const long c_maxFileDiffBytes = 5242880;
    private const int c_defaultMaxFileDiffs = 10;
    private static readonly RegistryQuery s_maxFileDiffBytesQuery = new RegistryQuery("/Service/GitRest/Settings/FileDiffsControllerMaxFileDiffBytes", false);
    private static readonly RegistryQuery s_maxFileDiffsQuery = new RegistryQuery("/Service/GitRest/Settings/FileDiffsControllerMaxFileDiffs", false);

    [HttpPost]
    [ClientResponseType(typeof (IEnumerable<Microsoft.TeamFoundation.SourceControl.WebApi.FileDiff>), null, null)]
    [ClientLocationId("C4C5A7E6-E9F3-4730-A92B-84BAACFF694B")]
    public HttpResponseMessage GetFileDiffs(
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      FileDiffsCriteria fileDiffsCriteria)
    {
      this.ValidateParameters(fileDiffsCriteria);
      List<Microsoft.TeamFoundation.SourceControl.WebApi.FileDiff> fileDiffList = new List<Microsoft.TeamFoundation.SourceControl.WebApi.FileDiff>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId))
      {
        GitVersionControlProvider gitVersionControlProvider = new GitVersionControlProvider(this.TfsRequestContext, tfsGitRepository);
        long maxFileDiffBytes = this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<long>(this.TfsRequestContext, in GitFileDiffsController.s_maxFileDiffBytesQuery, 5242880L);
        foreach (FileDiffParams fileDiffParam in fileDiffsCriteria.FileDiffParams)
        {
          Microsoft.TeamFoundation.SourceControl.WebApi.FileDiff fileDiff = this.GetFileDiff(gitVersionControlProvider, fileDiffParam, fileDiffsCriteria.TargetVersionCommit, fileDiffsCriteria.BaseVersionCommit, maxFileDiffBytes);
          fileDiffList.Add(fileDiff);
        }
      }
      return this.Request.CreateResponse<List<Microsoft.TeamFoundation.SourceControl.WebApi.FileDiff>>(HttpStatusCode.OK, fileDiffList);
    }

    private void ValidateParameters(FileDiffsCriteria fileDiffsCriteria)
    {
      if (fileDiffsCriteria == null)
        throw new ArgumentException(Resources.Get("FileDiffsCriteriaArgumentException"));
      if (fileDiffsCriteria.BaseVersionCommit == null || !Sha1Id.TryParse(fileDiffsCriteria.BaseVersionCommit, out Sha1Id _))
        throw new ArgumentException(Resources.Format("InvalidObjectId", (object) fileDiffsCriteria.BaseVersionCommit, (object) "BaseVersionCommit"));
      if (fileDiffsCriteria.TargetVersionCommit == null || !Sha1Id.TryParse(fileDiffsCriteria.TargetVersionCommit, out Sha1Id _))
        throw new ArgumentException(Resources.Format("InvalidObjectId", (object) fileDiffsCriteria.TargetVersionCommit, (object) "TargetVersionCommit"));
      int num1 = this.TfsRequestContext.GetService<IVssRegistryService>().GetValue<int>(this.TfsRequestContext, in GitFileDiffsController.s_maxFileDiffsQuery, 10);
      int? nullable1;
      if (fileDiffsCriteria == null)
      {
        nullable1 = new int?();
      }
      else
      {
        IEnumerable<FileDiffParams> fileDiffParams = fileDiffsCriteria.FileDiffParams;
        nullable1 = fileDiffParams != null ? new int?(fileDiffParams.ToList<FileDiffParams>().Count) : new int?();
      }
      int? nullable2 = nullable1;
      int? nullable3 = nullable2.HasValue ? nullable2 : throw new ArgumentException(Resources.Format("FileDiffsInvalidFileParamsInput", (object) num1));
      int num2 = 1;
      if (!(nullable3.GetValueOrDefault() < num2 & nullable3.HasValue))
      {
        nullable3 = nullable2;
        int num3 = num1;
        if (!(nullable3.GetValueOrDefault() > num3 & nullable3.HasValue))
        {
          List<FileDiffParams> fileDiffParamsList;
          if (fileDiffsCriteria == null)
          {
            fileDiffParamsList = (List<FileDiffParams>) null;
          }
          else
          {
            IEnumerable<FileDiffParams> fileDiffParams = fileDiffsCriteria.FileDiffParams;
            fileDiffParamsList = fileDiffParams != null ? fileDiffParams.ToList<FileDiffParams>() : (List<FileDiffParams>) null;
          }
          using (List<FileDiffParams>.Enumerator enumerator = fileDiffParamsList.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              FileDiffParams current = enumerator.Current;
              if (string.IsNullOrEmpty(current.Path) && string.IsNullOrEmpty(current.OriginalPath))
                throw new ArgumentException(Resources.Get("FileDiffsMissingPath"));
            }
          }
        }
      }
    }

    private Microsoft.TeamFoundation.SourceControl.WebApi.FileDiff GetFileDiff(
      GitVersionControlProvider gitVersionControlProvider,
      FileDiffParams fileDiffParams,
      string sourceCommit,
      string comparisonCommit,
      long maxFileDiffBytes)
    {
      FileDiffParameters diffParameters = new FileDiffParameters()
      {
        IncludeCharDiffs = false,
        LineNumbersOnly = true,
        ModifiedPath = fileDiffParams.Path,
        OriginalPath = fileDiffParams.OriginalPath,
        ModifiedVersion = "GC" + sourceCommit,
        OriginalVersion = "GC" + comparisonCommit,
        PartialDiff = false
      };
      Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileDiff fileDiffModel = gitVersionControlProvider.GetFileDiffModel(diffParameters, new long?(maxFileDiffBytes));
      List<LineDiffBlock> lineDiffBlockList = new List<LineDiffBlock>();
      foreach (FileDiffBlock block in fileDiffModel.Blocks)
      {
        LineDiffBlock lineDiffBlock = new LineDiffBlock()
        {
          ChangeType = (LineDiffBlockChangeType) block.ChangeType,
          OriginalLineNumberStart = block.OriginalLineNumberStart,
          OriginalLinesCount = block.OriginalLinesCount,
          ModifiedLineNumberStart = block.ModifiedLineNumberStart,
          ModifiedLinesCount = block.ModifiedLinesCount
        };
        lineDiffBlockList.Add(lineDiffBlock);
      }
      return new Microsoft.TeamFoundation.SourceControl.WebApi.FileDiff()
      {
        Path = fileDiffParams.Path,
        OriginalPath = fileDiffParams.OriginalPath,
        LineDiffBlocks = (IEnumerable<LineDiffBlock>) lineDiffBlockList
      };
    }
  }
}
