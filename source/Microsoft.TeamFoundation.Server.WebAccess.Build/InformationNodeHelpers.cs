// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.InformationNodeHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  internal static class InformationNodeHelpers
  {
    public static IDictionary<string, string> FieldsDict(this BuildInformationNode node) => (IDictionary<string, string>) node.Fields.ToDictionary<InformationField, string, string>((Func<InformationField, string>) (f => f.Name), (Func<InformationField, string>) (f => f.Value));

    public static int[] GetOpenedWorkItemIds(BuildInformationNode[] informationNodes) => InformationNodeHelpers.GetWorkItemIds((IEnumerable<BuildInformationNode>) informationNodes, InformationTypes.OpenedWorkItem);

    private static int[] GetWorkItemIds(
      IEnumerable<BuildInformationNode> informationNodes,
      string informationType)
    {
      HashSet<int> source = new HashSet<int>();
      foreach (BuildInformationNode node in informationNodes.Where<BuildInformationNode>((Func<BuildInformationNode, bool>) (node => string.Equals(node.Type, informationType, StringComparison.OrdinalIgnoreCase))))
      {
        int num = CommonInformationHelper.GetInt(node.FieldsDict(), InformationFields.WorkItemId, -1);
        if (num > 0)
          source.Add(num);
      }
      return source.ToArray<int>();
    }

    public static int GetChangesetId(IEnumerable<BuildInformationNode> informationNodes)
    {
      int changesetId = -1;
      BuildInformationNode node1 = informationNodes.FirstOrDefault<BuildInformationNode>((Func<BuildInformationNode, bool>) (node => string.Equals(node.Type, InformationTypes.CheckInOutcome, StringComparison.OrdinalIgnoreCase)));
      if (node1 != null)
        changesetId = CommonInformationHelper.GetInt(node1.FieldsDict(), InformationFields.ChangesetId, -1);
      return changesetId;
    }

    public static bool GetChangesetsInfo(
      IEnumerable<BuildInformationNode> informationNodes,
      out int successful,
      out int failed)
    {
      IEnumerable<BuildInformationNode> buildInformationNodes = informationNodes.Where<BuildInformationNode>((Func<BuildInformationNode, bool>) (node => string.Equals(node.Type, InformationTypes.CheckInOutcome, StringComparison.OrdinalIgnoreCase)));
      successful = 0;
      failed = 0;
      foreach (BuildInformationNode node in buildInformationNodes)
      {
        int num = CommonInformationHelper.GetInt(node.FieldsDict(), InformationFields.ChangesetId, -1);
        if (num > 0)
          ++successful;
        else if (num < 0)
          ++failed;
      }
      return successful > 0 || failed > 0;
    }

    public static void FixSorting(IEnumerable<InformationNodeModel> nodes)
    {
      ActivityPropertiesInfoModel propertiesInfoModel1 = nodes.OfType<ActivityPropertiesInfoModel>().FirstOrDefault<ActivityPropertiesInfoModel>();
      if (propertiesInfoModel1 != null)
        propertiesInfoModel1.Initial = true;
      ActivityPropertiesInfoModel propertiesInfoModel2 = nodes.OfType<ActivityPropertiesInfoModel>().LastOrDefault<ActivityPropertiesInfoModel>();
      if (propertiesInfoModel2 == null || propertiesInfoModel2.Equals((object) propertiesInfoModel1))
        return;
      propertiesInfoModel2.Final = true;
    }

    public static AssociatedWorkItem[] GetAssociatedWorkItems(
      BuildInformationNode[] informationNodes)
    {
      List<AssociatedWorkItem> source = new List<AssociatedWorkItem>();
      foreach (BuildInformationNode node in ((IEnumerable<BuildInformationNode>) informationNodes).Where<BuildInformationNode>((Func<BuildInformationNode, bool>) (node => string.Equals(node.Type, InformationTypes.AssociatedWorkItem, StringComparison.OrdinalIgnoreCase))))
      {
        AssociatedWorkItem associatedWorkItem = new AssociatedWorkItem()
        {
          Id = CommonInformationHelper.GetInt(node.FieldsDict(), InformationFields.WorkItemId, -1),
          ParentId = CommonInformationHelper.GetInt(node.FieldsDict(), "ParentWorkItemId", -1),
          Type = CommonInformationHelper.GetString(node.FieldsDict(), "Type")
        };
        if (associatedWorkItem.Id > 0)
          source.Add(associatedWorkItem);
      }
      Dictionary<int, AssociatedWorkItem> dictionary = source.ToDictionary<AssociatedWorkItem, int>((Func<AssociatedWorkItem, int>) (wi => wi.Id));
      foreach (AssociatedWorkItem associatedWorkItem in source)
        associatedWorkItem.ParentPath = InformationNodeHelpers.GetParentPath(associatedWorkItem.Id, dictionary);
      source.Sort((Comparison<AssociatedWorkItem>) ((wi1, wi2) => StringComparer.OrdinalIgnoreCase.Compare(wi1.ParentPath, wi2.ParentPath)));
      return source.ToArray();
    }

    public static CustomSummaryInformation[] GetCustomSummaryInformation(
      IEnumerable<BuildInformationNode> informationNodes)
    {
      List<CustomSummaryInformation> summaryInformationList = new List<CustomSummaryInformation>();
      foreach (BuildInformationNode node in informationNodes.Where<BuildInformationNode>((Func<BuildInformationNode, bool>) (node => string.Equals(node.Type, InformationTypes.CustomSummaryInformation, StringComparison.OrdinalIgnoreCase))))
      {
        CustomSummaryInformation summaryInformation = new CustomSummaryInformation()
        {
          Message = CommonInformationHelper.GetString(node.FieldsDict(), InformationFields.Message),
          SectionName = CommonInformationHelper.GetString(node.FieldsDict(), InformationFields.SectionName),
          SectionHeader = CommonInformationHelper.GetString(node.FieldsDict(), InformationFields.SectionHeader),
          SectionPriority = CommonInformationHelper.GetInt(node.FieldsDict(), InformationFields.SectionPriority, -1)
        };
        summaryInformationList.Add(summaryInformation);
      }
      return summaryInformationList.ToArray();
    }

    private static string GetParentPath(int workItemId, Dictionary<int, AssociatedWorkItem> dict)
    {
      AssociatedWorkItem associatedWorkItem;
      if (!dict.TryGetValue(workItemId, out associatedWorkItem))
        return associatedWorkItem.Type + associatedWorkItem.Id.ToString();
      string parentPath1 = associatedWorkItem.ParentPath;
      if (!string.IsNullOrEmpty(parentPath1))
        return parentPath1;
      string parentPath2 = associatedWorkItem.Type + associatedWorkItem.Id.ToString();
      int parentId = associatedWorkItem.ParentId;
      if (parentId > 0 && parentId != workItemId)
        parentPath2 = InformationNodeHelpers.GetParentPath(parentId, dict) + "\\" + parentPath2;
      return parentPath2;
    }
  }
}
