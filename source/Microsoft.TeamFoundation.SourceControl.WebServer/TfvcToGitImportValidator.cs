// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcToGitImportValidator
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  internal class TfvcToGitImportValidator
  {
    private static readonly string c_MaximumSnapshotSizeAllowedForTFVCImportInMB = "/Service/Git/Settings/Import/MaximumSnapshotSizeAllowedForTFVCImportInMB";
    private static readonly int c_DefaultMaximumSnapshotSizeAllowedForTFVCImportInMB = 1024;
    private static readonly string c_MaximumFileSizeAllowedForTFVCImportInMB = "/Service/Git/Settings/Import/MaximumFileSizeAllowedForTFVCImportInMB";
    private static readonly int c_DefaultMaximumFileSizeAllowedForTFVCImportInMB = 50;

    internal static bool ValidateTfvcToGitImportParams(
      IVssRequestContext requestContext,
      ImportRepositoryValidation remoteRepository,
      string traceArea,
      ClientTraceData ctData,
      out string errorMessage)
    {
      return TfvcToGitImportValidator.ValidateTfvcToGitImportParams(requestContext, (ITfvcToGitImportClient) new TfvcToGitImportClient(requestContext), remoteRepository, ctData, traceArea, out errorMessage);
    }

    internal static bool ValidateTfvcToGitImportParams(
      IVssRequestContext requestContext,
      ITfvcToGitImportClient gitImportTfvcClient,
      ImportRepositoryValidation remoteRepository,
      ClientTraceData ctData,
      string traceArea,
      out string errorMessage)
    {
      errorMessage = (string) null;
      try
      {
        if (string.Equals(remoteRepository.TfvcSource.Path, "$/"))
        {
          errorMessage = Resources.Get("GitImportTFVCRootNotAllowed");
          ctData.Add("TfvcValidationPathIsRoot", (object) true);
          return false;
        }
        TfvcVersionDescriptor changesetItemDescriptor1 = new TfvcVersionDescriptor()
        {
          VersionType = TfvcVersionType.Latest,
          Version = (string) null
        };
        List<TfvcBranch> list = gitImportTfvcClient.GetTfvcBranches(remoteRepository.TfvcSource.Path).ToList<TfvcBranch>().Where<TfvcBranch>((Func<TfvcBranch, bool>) (x => x.Path.StartsWith(remoteRepository.TfvcSource.Path))).ToList<TfvcBranch>();
        bool pathIsBranch = list.Any<TfvcBranch>((Func<TfvcBranch, bool>) (x => x.Path == remoteRepository.TfvcSource.Path));
        if (!TfvcToGitImportValidator.ValidateSnapShot(requestContext, gitImportTfvcClient, remoteRepository, changesetItemDescriptor1, pathIsBranch, list, false, ctData, traceArea, out errorMessage))
        {
          ctData.Add("TfvcTfvcTipValidationFailed", (object) true);
          return false;
        }
        if (remoteRepository.TfvcSource.ImportHistory)
        {
          TfvcVersionDescriptor changesetItemDescriptor2 = TfvcToGitImportValidator.GetBaseChangesetItemDescriptor(requestContext, gitImportTfvcClient, remoteRepository, pathIsBranch, list);
          if (!TfvcToGitImportValidator.ValidateSnapShot(requestContext, gitImportTfvcClient, remoteRepository, changesetItemDescriptor2, pathIsBranch, list, true, ctData, traceArea, out errorMessage))
          {
            ctData.Add("TfvcHistoryValidationFailed", (object) true);
            return false;
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        ctData.Add("TfvcValidationFileSizeValidationFailedWithException", (object) true);
        requestContext.TraceException(700219, traceArea, WebApiTraceLayers.BusinessLogic, ex);
        return false;
      }
    }

    private static TfvcVersionDescriptor GetBaseChangesetItemDescriptor(
      IVssRequestContext requestContext,
      ITfvcToGitImportClient gitImportTfvcClient,
      ImportRepositoryValidation remoteRepository,
      bool pathIsBranch,
      List<TfvcBranch> branchesList)
    {
      TfvcVersionDescriptor versionDescriptor = (TfvcVersionDescriptor) null;
      int changeSetId = gitImportTfvcClient.QueryTfvcChangesetIds(remoteRepository.TfvcSource.Path, 0, 1).ToList<int>().First<int>();
      DateTime fromDate = gitImportTfvcClient.QueryChangesetRef(changeSetId).CreatedDate.Subtract(TimeSpan.FromDays((double) remoteRepository.TfvcSource.ImportHistoryDurationInDays));
      List<int> list = gitImportTfvcClient.QueryTfvcChangesetIds(remoteRepository.TfvcSource.Path, fromDate).ToList<int>();
      bool flag = false;
      foreach (int num in list)
      {
        if (!flag)
        {
          versionDescriptor = new TfvcVersionDescriptor()
          {
            VersionType = TfvcVersionType.Changeset,
            Version = num.ToString()
          };
          using (TeamFoundationDataReader foundationDataReader = gitImportTfvcClient.QueryTfvcItems(remoteRepository.TfvcSource.Path, versionDescriptor))
          {
            foreach (ItemSet current in foundationDataReader.CurrentEnumerable<ItemSet>())
            {
              if (!flag)
              {
                foreach (Item obj in current.Items)
                {
                  if (TfvcToGitImportValidator.IsItemInValidPathAndNotPartOfAnyChildBranch(obj.ToWebApiItem(requestContext, (UrlHelper) null), pathIsBranch, branchesList))
                  {
                    flag = true;
                    break;
                  }
                }
              }
              else
                break;
            }
          }
        }
        else
          break;
      }
      return versionDescriptor;
    }

    private static bool ValidateSnapShot(
      IVssRequestContext requestContext,
      ITfvcToGitImportClient gitImportTfvcClient,
      ImportRepositoryValidation remoteRepository,
      TfvcVersionDescriptor changesetItemDescriptor,
      bool pathIsBranch,
      List<TfvcBranch> branchesList,
      bool isValidatingHistory,
      ClientTraceData ctData,
      string traceArea,
      out string errorMessage)
    {
      errorMessage = (string) null;
      bool flag = false;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int num1 = service.GetValue<int>(requestContext, (RegistryQuery) TfvcToGitImportValidator.c_MaximumSnapshotSizeAllowedForTFVCImportInMB, TfvcToGitImportValidator.c_DefaultMaximumSnapshotSizeAllowedForTFVCImportInMB);
      int num2 = service.GetValue<int>(requestContext, (RegistryQuery) TfvcToGitImportValidator.c_MaximumFileSizeAllowedForTFVCImportInMB, TfvcToGitImportValidator.c_DefaultMaximumFileSizeAllowedForTFVCImportInMB);
      using (TeamFoundationDataReader foundationDataReader = gitImportTfvcClient.QueryTfvcItems(remoteRepository.TfvcSource.Path, changesetItemDescriptor))
      {
        IEnumerable<ItemSet> itemSets = foundationDataReader.CurrentEnumerable<ItemSet>();
        long num3 = 0;
        int num4 = 0;
        try
        {
          foreach (BaseItemSet<Item> baseItemSet in itemSets)
          {
            foreach (Item obj in baseItemSet.Items)
            {
              ++num4;
              IVssRequestContext tfsRequestContext = requestContext;
              TfvcItem webApiItem = obj.ToWebApiItem(tfsRequestContext, (UrlHelper) null);
              if (TfvcToGitImportValidator.IsItemInValidPathAndNotPartOfAnyChildBranch(webApiItem, pathIsBranch, branchesList))
              {
                flag = true;
                num3 += webApiItem.Size;
                if ((double) webApiItem.Size / 1048576.0 > (double) num2)
                {
                  errorMessage = isValidatingHistory ? Resources.Get("GitImportTFVCFileTooLargeHistory") : Resources.Get("GitImportTFVCFileTooLarge");
                  if (isValidatingHistory)
                    ctData.Add("TfvcValidationFileSizeValidationHistory", (object) true);
                  else
                    ctData.Add("TfvcValidationFileSizeValidation", (object) true);
                  return false;
                }
                if ((double) num3 / 1048576.0 > (double) num1)
                {
                  errorMessage = isValidatingHistory ? Resources.Get("GitImportTFVCImportHistorySizeTooLarge") : Resources.Get("GitImportTFVCImportSizeTooLarge");
                  if (isValidatingHistory)
                    ctData.Add("TfvcValidationFullSizeValidationHistory", (object) true);
                  else
                    ctData.Add("TfvcValidationFullSizeValidation", (object) true);
                  return false;
                }
              }
            }
          }
        }
        catch
        {
          requestContext.Trace(700221, TraceLevel.Info, traceArea, TraceLayer.BusinessLogic, "Exception while validating snapshot. Iterated till {0} items. TfvcSource.Path:{1}, versionDescriptor:{2}, pathIsBranch:{3}, isValidatingHistory:{4}", (object) num4, (object) remoteRepository.TfvcSource.Path, (object) changesetItemDescriptor, (object) pathIsBranch, (object) isValidatingHistory);
          throw;
        }
        if (!flag)
        {
          ctData.Add("TfvcValidationNoImportableItemInPath", (object) true);
          return false;
        }
      }
      return true;
    }

    private static bool IsItemInValidPathAndNotPartOfAnyChildBranch(
      TfvcItem item,
      bool pathIsBranch,
      List<TfvcBranch> branches)
    {
      if (item.IsBranch || item.IsFolder)
        return false;
      return pathIsBranch || !branches.Any<TfvcBranch>((Func<TfvcBranch, bool>) (b => item.Path.ToLower().StartsWith(b.Path.ToLower())));
    }
  }
}
