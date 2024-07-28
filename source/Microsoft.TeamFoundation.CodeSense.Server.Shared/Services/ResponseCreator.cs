// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Services.ResponseCreator
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server.Services
{
  public static class ResponseCreator
  {
    public static VersionedContent CreateV3Response(
      IVssRequestContext requestContext,
      string details,
      DateTime timeStamp,
      int aggregateVersion,
      List<BranchLinkData> branchLinks,
      CodeElementIdentityCollectionV3 codeElements,
      SourceControlDataV3 sourceControlData,
      bool summary,
      string path,
      Guid projectGuid,
      ProjectMapCache projectMapCache)
    {
      FileDetailsResultV3 fileDetailsResultV3 = JsonConvert.DeserializeObject<FileDetailsResultV3>(details, CodeSenseSerializationSettings.JsonSerializerSettings);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int retentionPeriod1 = service.GetRetentionPeriod(requestContext);
      List<BranchLinkDataV3> list = branchLinks.Select<BranchLinkData, BranchLinkDataV3>((Func<BranchLinkData, BranchLinkDataV3>) (l => new BranchLinkDataV3(l))).ToList<BranchLinkDataV3>();
      FilterUtilities.ByRetentionV3(retentionPeriod1, sourceControlData, codeElements, fileDetailsResultV3.BranchList, list, (IDictionary<string, HashSet<int>>) null);
      if (summary)
        sourceControlData.RemoveChangesetDetails();
      if (aggregateVersion != 7)
        ConverterUtilities.ReplaceServerPathsWithGuid<BranchDetailsResultV3>(requestContext, fileDetailsResultV3.BranchList, projectMapCache);
      sourceControlData.RemoveWorkItemDetails();
      fileDetailsResultV3.RemoveRestrictedBranches(requestContext);
      fileDetailsResultV3.BranchList.UpdateServerPaths<BranchDetailsResultV3>(requestContext, path.GetProjectName(), projectGuid, projectMapCache);
      list.UpdateBranchMapPaths<BranchLinkDataV3>(requestContext, path.GetProjectName(), projectGuid, projectMapCache);
      bool indexComplete;
      DateTime indexCompleteTo1;
      service.GetIndexingStatus(requestContext, out indexComplete, out indexCompleteTo1);
      DateTime timeStamp1 = timeStamp;
      int num1 = retentionPeriod1;
      int num2 = indexComplete ? 1 : 0;
      DateTime? indexCompleteTo2 = !indexComplete ? new DateTime?(indexCompleteTo1) : new DateTime?();
      int retentionPeriod2 = num1;
      List<BranchLinkDataV3> branchMap = list;
      List<BranchDetailsResultV3> branchList = fileDetailsResultV3.BranchList;
      CodeElementIdentityCollectionV3 codeElements1 = codeElements;
      SourceControlDataV3 sourceControlData1 = sourceControlData;
      return new VersionedContent(JsonConvert.SerializeObject((object) new DetailsResponseV3(timeStamp1, "3.0", num2 != 0, indexCompleteTo2, retentionPeriod2, (IEnumerable<BranchLinkDataV3>) branchMap, (IList<BranchDetailsResultV3>) branchList, codeElements1, sourceControlData1), CodeSenseSerializationSettings.JsonSerializerSettings), CodeSenseResourceVersion.Dev12Update3);
    }
  }
}
