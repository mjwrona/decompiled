// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Internals.QueryHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Internals
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class QueryHelper
  {
    public static List<InternalWorkItemRelation> ConvertLinkQueryResult(XmlElement resultsElement)
    {
      TeamFoundationTrace.Verbose("--> ConvertLinkQueryResult()");
      TeamFoundationTrace.Verbose("resultsElement: {0}", (object) resultsElement.OuterXml);
      List<InternalWorkItemRelation> workItemRelationList = new List<InternalWorkItemRelation>();
      int sourceId = InternalWorkItemRelation.MissingID;
      int targetId = InternalWorkItemRelation.MissingID;
      int linkTypeId = InternalWorkItemRelation.MissingID;
      bool isLocked = false;
      foreach (XmlElement childNode in resultsElement.ChildNodes)
      {
        if (childNode.Name.Equals("S", StringComparison.OrdinalIgnoreCase))
        {
          targetId = InternalWorkItemRelation.MissingID;
          linkTypeId = InternalWorkItemRelation.MissingID;
          isLocked = false;
          int int32 = XmlConvert.ToInt32(childNode.GetAttribute("S"));
          int num = int32;
          if (childNode.HasAttribute("E"))
            num = XmlConvert.ToInt32(childNode.GetAttribute("E"));
          for (; int32 <= num; ++int32)
            workItemRelationList.Add(new InternalWorkItemRelation(int32, targetId, linkTypeId, isLocked));
          sourceId = num;
        }
        else if (childNode.Name.Equals("T", StringComparison.OrdinalIgnoreCase))
        {
          sourceId = InternalWorkItemRelation.MissingID;
          isLocked = false;
          int int32 = XmlConvert.ToInt32(childNode.GetAttribute("S"));
          int num = int32;
          if (childNode.HasAttribute("E"))
            num = XmlConvert.ToInt32(childNode.GetAttribute("E"));
          for (; int32 <= num; ++int32)
            workItemRelationList.Add(new InternalWorkItemRelation(sourceId, int32, linkTypeId, isLocked));
          targetId = num;
        }
        else
        {
          string attribute1 = childNode.GetAttribute("S");
          if (!string.IsNullOrEmpty(attribute1))
            sourceId = XmlConvert.ToInt32(attribute1);
          string attribute2 = childNode.GetAttribute("T");
          if (!string.IsNullOrEmpty(attribute2))
            targetId = XmlConvert.ToInt32(attribute2);
          string attribute3 = childNode.GetAttribute("L");
          if (!string.IsNullOrEmpty(attribute3))
            linkTypeId = XmlConvert.ToInt32(attribute3);
          string attribute4 = childNode.GetAttribute("E");
          if (!string.IsNullOrEmpty(attribute4))
            isLocked = XmlConvert.ToInt32(attribute4) == 1;
          workItemRelationList.Add(new InternalWorkItemRelation(sourceId, targetId, linkTypeId, isLocked));
        }
      }
      TeamFoundationTrace.Verbose("<-- ConvertLinkQueryResult()");
      return workItemRelationList;
    }

    public static List<InternalWorkItemLinkInfo> SortAndConvert(
      List<IWorkItemRelation> links,
      bool isTreeQuery,
      bool? sortFlag)
    {
      TeamFoundationTrace.Verbose("--> SortAndConvert()");
      List<InternalWorkItemLinkInfo> workItemLinkInfoList = new List<InternalWorkItemLinkInfo>();
      if (isTreeQuery)
      {
        TeamFoundationTrace.Verbose("Converting query results for tree query.");
        List<InternalWorkItemLinkInfo> list = links.Select<IWorkItemRelation, InternalWorkItemLinkInfo>((Func<IWorkItemRelation, InternalWorkItemLinkInfo>) (item => new InternalWorkItemLinkInfo(item.SourceID, item.TargetID, item.LinkTypeID, item.IsLocked))).ToList<InternalWorkItemLinkInfo>();
        TeamFoundationTrace.Verbose("<-- SortAndConvert()");
        return list;
      }
      if (sortFlag.HasValue)
      {
        TeamFoundationTrace.Verbose("Sorting on ID field, Ascending: {0}", (object) sortFlag.Value.ToString());
        links.Sort((Comparison<IWorkItemRelation>) ((a, b) =>
        {
          int num = a.SourceID - b.SourceID;
          if (num == 0)
            num = a.TargetID - b.TargetID;
          return !sortFlag.Value ? -num : num;
        }));
      }
      TeamFoundationTrace.Verbose("Converting query results for links query.");
      int num1 = 0;
      int index = 0;
      for (int count = links.Count; index < count; ++index)
      {
        IWorkItemRelation link = links[index];
        if (link.SourceID != num1)
        {
          workItemLinkInfoList.Add(new InternalWorkItemLinkInfo(0, link.SourceID, 0, false));
          num1 = link.SourceID;
        }
        if (link.TargetID != 0)
          workItemLinkInfoList.Add(new InternalWorkItemLinkInfo(link.SourceID, link.TargetID, link.LinkTypeID, link.IsLocked));
      }
      TeamFoundationTrace.Verbose("<-- SortAndConvert()");
      return workItemLinkInfoList;
    }

    public static List<int> ConvertRegularQueryResult(XmlElement resultsElement)
    {
      TeamFoundationTrace.Verbose("--> ConvertRegularQueryResult()");
      TeamFoundationTrace.Verbose("resultsElement: {0}", (object) resultsElement.OuterXml);
      List<int> intList = new List<int>();
      XmlNodeList xmlNodeList = resultsElement.SelectNodes("id");
      int num1 = 0;
      foreach (XmlNode xmlNode in xmlNodeList)
      {
        XmlAttribute namedItem1 = (XmlAttribute) xmlNode.Attributes.GetNamedItem("s");
        if (namedItem1 != null)
          num1 = Convert.ToInt32(namedItem1.Value, (IFormatProvider) CultureInfo.InvariantCulture);
        XmlAttribute namedItem2 = (XmlAttribute) xmlNode.Attributes.GetNamedItem("e");
        int num2 = namedItem2 == null ? num1 : Convert.ToInt32(namedItem2.Value, (IFormatProvider) CultureInfo.InvariantCulture);
        for (int index = num1; index <= num2; ++index)
          intList.Add(index);
      }
      TeamFoundationTrace.Verbose("<-- ConvertRegularQueryResult()");
      return intList;
    }
  }
}
