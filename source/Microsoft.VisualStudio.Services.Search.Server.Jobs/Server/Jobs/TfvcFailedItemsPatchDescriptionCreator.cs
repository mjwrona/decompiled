// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.TfvcFailedItemsPatchDescriptionCreator
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class TfvcFailedItemsPatchDescriptionCreator : 
    AbstractCodeFailedItemsPatchDescriptionCreator
  {
    private readonly TfvcHttpClientWrapper m_tfvcHttpClientWrapper;

    internal TfvcFailedItemsPatchDescriptionCreator(
      IndexingExecutionContext indexingExecutionContext,
      TraceMetaData traceMetaData)
      : this(new TfvcHttpClientWrapper((ExecutionContext) indexingExecutionContext, traceMetaData), traceMetaData)
    {
    }

    internal TfvcFailedItemsPatchDescriptionCreator(
      TfvcHttpClientWrapper tfvcHttpClientWrapper,
      TraceMetaData traceMetaData)
      : base(traceMetaData)
    {
      this.m_tfvcHttpClientWrapper = tfvcHttpClientWrapper;
    }

    internal override VersionControlType GetVersionControlType() => VersionControlType.TFVC;

    internal override void CreatePatchDescription(
      IndexingExecutionContext iexContext,
      string filePath,
      string branchName,
      out List<string> recordsToBeAdded,
      out List<string> recordsToBeDeleted,
      out string patchFile)
    {
      recordsToBeAdded = new List<string>();
      recordsToBeDeleted = new List<string>();
      patchFile = (string) null;
      string str1 = "$/" + iexContext.ProjectName + "/";
      TfvcChangesetSearchCriteria searchCriteriaObject = new TfvcChangesetSearchCriteria()
      {
        ItemPath = str1
      };
      try
      {
        string str2 = this.m_tfvcHttpClientWrapper.GetLatestChangeset(iexContext.ProjectId.Value.ToString(), searchCriteriaObject).ChangesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        TfvcVersionDescriptor versionInfo = new TfvcVersionDescriptor()
        {
          Version = str2,
          VersionType = TfvcVersionType.Changeset
        };
        bool isDeleted;
        TfvcItem tfvcItem1 = this.m_tfvcHttpClientWrapper.GetItem(filePath, versionInfo, out isDeleted);
        if (tfvcItem1 != null && tfvcItem1.IsFolder && !isDeleted)
        {
          foreach (TfvcItem tfvcItem2 in this.m_tfvcHttpClientWrapper.GetItems(filePath, versionInfo, VersionControlRecursionType.OneLevel))
          {
            if (tfvcItem2.Path != tfvcItem1.Path)
              recordsToBeAdded.Add(tfvcItem2.Path);
          }
          recordsToBeDeleted.Add(filePath);
        }
        else
          patchFile = filePath;
      }
      catch (Exception ex)
      {
        recordsToBeAdded.Clear();
        recordsToBeDeleted.Clear();
        recordsToBeAdded.Add(filePath);
        Tracer.TraceError(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Exception while creating patch description for filePath: {0}. Exception: {1}", (object) filePath, (object) ex)));
      }
    }
  }
}
