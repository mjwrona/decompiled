// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WiqlTransformUtils
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Events;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Identity.Mru;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class WiqlTransformUtils
  {
    private static HashSet<Condition> ValidReplacementConditions = new HashSet<Condition>()
    {
      Condition.Equals,
      Condition.EqualsAlias,
      Condition.Group,
      Condition.In,
      Condition.NotEquals,
      Condition.NotEqualsAlias
    };

    private static bool IsPersonField(IFieldTypeDictionary fieldsDict, string fieldName)
    {
      FieldEntry field;
      return fieldsDict.TryGetField(fieldName, out field) && field.IsPerson;
    }

    private static bool IsAreaPathField(IFieldTypeDictionary fieldsDict, string fieldName)
    {
      FieldEntry field;
      return fieldsDict.TryGetField(fieldName, out field) && field.IsAreaPath;
    }

    private static bool IsPortfolioProjectField(IFieldTypeDictionary fieldsDict, string fieldName)
    {
      FieldEntry field;
      return fieldsDict.TryGetField(fieldName, out field) && field.IsPortfolioProject;
    }

    private static bool IsIterationPathField(IFieldTypeDictionary fieldsDict, string fieldName)
    {
      FieldEntry field;
      return fieldsDict.TryGetField(fieldName, out field) && field.IsIterationPath;
    }

    private static bool IsIdentityField(IFieldTypeDictionary fieldsDict, string fieldName)
    {
      FieldEntry field;
      return fieldsDict.TryGetField(fieldName, out field) && field.IsIdentity;
    }

    internal static string TransformNamesToIds(
      IVssRequestContext requestContext,
      string queryText,
      bool ignoreInvalidPaths,
      bool transformPersonNames = true)
    {
      if (string.IsNullOrEmpty(queryText))
        return queryText;
      IFieldTypeDictionary fieldsSnapshot = requestContext.GetService<WorkItemTrackingFieldService>().GetFieldsSnapshot(requestContext);
      NodeSelect syntax = Parser.ParseSyntax(queryText);
      WiqlTransformUtils.TransfromIterationAndAreaPathToId(requestContext, syntax, fieldsSnapshot, ignoreInvalidPaths);
      if (transformPersonNames)
        WiqlTransformUtils.TransformPersonNamesToTfId(requestContext, syntax, fieldsSnapshot);
      return syntax.ToString();
    }

    private static void TransfromIterationAndAreaPathToId(
      IVssRequestContext requestContext,
      NodeSelect topNode,
      IFieldTypeDictionary fieldTypes,
      bool ignoreInvalidPaths)
    {
      StringComparer serverStringComparer = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ServerStringComparer;
      HashSet<string> areaPathDisplayNames = new HashSet<string>((IEqualityComparer<string>) serverStringComparer);
      HashSet<string> iterationPathDisplayNames = new HashSet<string>((IEqualityComparer<string>) serverStringComparer);
      WiqlTransformUtils.WalkNode(topNode.Where, (Func<NodeCondition, NodeFieldName, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>) ((conditionNode, fieldNameNode, valueNode) =>
      {
        if (!string.IsNullOrEmpty(valueNode?.ConstStringValue) && valueNode.ConstStringValue[0] != '\a')
        {
          if (WiqlTransformUtils.IsAreaPathField(fieldTypes, fieldNameNode.Value) || WiqlTransformUtils.IsPortfolioProjectField(fieldTypes, fieldNameNode.Value))
            areaPathDisplayNames.Add(valueNode.ConstStringValue);
          else if (WiqlTransformUtils.IsIterationPathField(fieldTypes, fieldNameNode.Value))
            iterationPathDisplayNames.Add(valueNode.ConstStringValue);
        }
        return valueNode;
      }));
      if (!areaPathDisplayNames.Any<string>() && !iterationPathDisplayNames.Any<string>())
        return;
      ITreeDictionary treeService = requestContext.WitContext().TreeService;
      WiqlTransformUtils.WalkNode(topNode.Where, (Func<NodeCondition, NodeFieldName, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>) ((conditionNode, fieldNameNode, valueNode) =>
      {
        if (!string.IsNullOrEmpty(valueNode?.ConstStringValue))
        {
          string str = "\a";
          if (WiqlTransformUtils.IsAreaPathField(fieldTypes, fieldNameNode.Value) || WiqlTransformUtils.IsPortfolioProjectField(fieldTypes, fieldNameNode.Value))
          {
            TreeNode treeNode;
            if (treeService.TryGetNodeFromPath(requestContext, valueNode.ConstStringValue, TreeStructureType.Area, out treeNode))
              return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(str + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ARPID:'{0}:{1:0000000000}'", (object) treeNode.ProjectId, (object) treeNode.Id));
            if (!ignoreInvalidPaths)
              throw new SyntaxException(valueNode, SyntaxError.AreaPathIsNotFoundInHierarchy);
            return valueNode;
          }
          if (WiqlTransformUtils.IsIterationPathField(fieldTypes, fieldNameNode.Value))
          {
            TreeNode treeNode;
            if (treeService.TryGetNodeFromPath(requestContext, valueNode.ConstStringValue, TreeStructureType.Iteration, out treeNode))
              return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(str + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ITPID:'{0}:{1:0000000000}'", (object) treeNode.ProjectId, (object) treeNode.Id));
            if (!ignoreInvalidPaths)
              throw new SyntaxException(valueNode, SyntaxError.IterationPathIsNotFoundInHierarchy);
            return valueNode;
          }
        }
        return valueNode;
      }));
    }

    private static void TransformPersonNamesToTfId(
      IVssRequestContext requestContext,
      NodeSelect topNode,
      IFieldTypeDictionary fieldTypes)
    {
      StringComparer serverStringComparer = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ServerStringComparer;
      HashSet<string> userDisplayNames = new HashSet<string>((IEqualityComparer<string>) serverStringComparer);
      HashSet<string> identityDisplayNames = new HashSet<string>((IEqualityComparer<string>) serverStringComparer);
      WiqlTransformUtils.WalkNode(topNode.Where, (Func<NodeCondition, NodeFieldName, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>) ((conditionNode, fieldNameNode, valueNode) =>
      {
        if (valueNode is NodeVariable)
        {
          foreach (NodeItem nodeItem in (IEnumerable<NodeItem>) (valueNode as NodeVariable).Parameters.Arguments)
          {
            if (nodeItem is NodeString && nodeItem.Value[0] != '\a')
              identityDisplayNames.Add(nodeItem.Value);
          }
        }
        if (!string.IsNullOrEmpty(valueNode?.ConstStringValue) && valueNode.ConstStringValue[0] != '\a')
        {
          if (WiqlTransformUtils.IsPersonField(fieldTypes, fieldNameNode.Value) && !WiqlTransformUtils.IsIdentityField(fieldTypes, fieldNameNode.Value))
            userDisplayNames.Add(valueNode.ConstStringValue);
          if (WiqlTransformUtils.IsIdentityField(fieldTypes, fieldNameNode.Value) && conditionNode != null && WiqlTransformUtils.ValidReplacementConditions.Contains(conditionNode.Condition))
            identityDisplayNames.Add(valueNode.ConstStringValue);
        }
        return valueNode;
      }));
      if (!userDisplayNames.Any<string>() && !identityDisplayNames.Any<string>())
        return;
      ResolvedIdentityNamesInfo resolvedNamesInfo = new ResolvedIdentityNamesInfo();
      Dictionary<string, Guid> displayNameTfIdMap = new Dictionary<string, Guid>();
      if (identityDisplayNames.Any<string>())
        resolvedNamesInfo = requestContext.GetService<IWorkItemIdentityService>().ResolveIdentityNames(requestContext.WitContext(), (IEnumerable<string>) identityDisplayNames, false);
      if (userDisplayNames.Any<string>())
        displayNameTfIdMap = WiqlTransformUtils.MapDisplayNamesToTeamFoundationId(requestContext, (IEnumerable<string>) userDisplayNames, serverStringComparer);
      WiqlTransformUtils.WalkNode(topNode.Where, (Func<NodeCondition, NodeFieldName, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>) ((conditionNode, fieldNameNode, valueNode) =>
      {
        if (valueNode is NodeVariable nodeVariable2)
        {
          foreach (NodeItem nodeItem in (IEnumerable<NodeItem>) nodeVariable2.Parameters.Arguments)
          {
            if (nodeItem is NodeString && resolvedNamesInfo.NamesLookup.ContainsKey(nodeItem.Value))
            {
              string str = "\a" + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TFID:'{0}'", (object) resolvedNamesInfo.NamesLookup[nodeItem.Value].TeamFoundationId);
              nodeItem.Value = str;
            }
          }
        }
        if (!string.IsNullOrEmpty(valueNode?.ConstStringValue))
        {
          string str = "\a";
          if (WiqlTransformUtils.IsPersonField(fieldTypes, fieldNameNode.Value) && !WiqlTransformUtils.IsIdentityField(fieldTypes, fieldNameNode.Value))
          {
            Guid guid;
            return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(!displayNameTfIdMap.TryGetValue(valueNode.ConstStringValue, out guid) ? str + valueNode.ConstStringValue : str + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TFID:'{0}'", (object) guid));
          }
          if (WiqlTransformUtils.IsIdentityField(fieldTypes, fieldNameNode.Value) && conditionNode != null && WiqlTransformUtils.ValidReplacementConditions.Contains(conditionNode.Condition))
          {
            string s;
            if (resolvedNamesInfo.NamesLookup.ContainsKey(valueNode.ConstStringValue))
              s = !(resolvedNamesInfo.NamesLookup[valueNode.ConstStringValue].TeamFoundationId != Guid.Empty) ? str + valueNode.ConstStringValue : str + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TFID:'{0}'", (object) resolvedNamesInfo.NamesLookup[valueNode.ConstStringValue].TeamFoundationId);
            else if (resolvedNamesInfo.AadNamesLookup.ContainsKey(valueNode.ConstStringValue))
            {
              string key = resolvedNamesInfo.AadNamesLookup[valueNode.ConstStringValue];
              s = !resolvedNamesInfo.IdentityMap.Value.ContainsKey(key) ? str + key : str + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TFID:'{0}'", (object) resolvedNamesInfo.IdentityMap.Value[key].Id);
            }
            else
              s = str + valueNode.ConstStringValue;
            return (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(s);
          }
        }
        return valueNode;
      }));
      WiqlTransformUtils.AddIdentitiesToMru(requestContext, resolvedNamesInfo, identityDisplayNames);
    }

    internal static void UpdateQueryTexts(
      IVssRequestContext requestContext,
      FieldTypeUpdateEventData[] fieldTypeChanges)
    {
      requestContext.TraceBlock(901930, 901939, 901938, "Queries", "Query", "WiqlTransormUtils.UpdateQueryTexts", (Action) (() =>
      {
        HashSet<string> fromPersonToKeyword = new HashSet<string>(((IEnumerable<FieldTypeUpdateEventData>) fieldTypeChanges).Where<FieldTypeUpdateEventData>((Func<FieldTypeUpdateEventData, bool>) (ftc =>
        {
          int? oldType = ftc.OldType;
          int? nullable = oldType.HasValue ? new int?(oldType.GetValueOrDefault() & 24) : new int?();
          int num7 = 24;
          if (nullable.GetValueOrDefault() == num7 & nullable.HasValue)
          {
            int? newType3 = ftc.NewType;
            nullable = newType3.HasValue ? new int?(newType3.GetValueOrDefault() & 24) : new int?();
            int num8 = 24;
            if (!(nullable.GetValueOrDefault() == num8 & nullable.HasValue))
            {
              int? newType4 = ftc.NewType;
              nullable = newType4.HasValue ? new int?(newType4.GetValueOrDefault() & 16) : new int?();
              int num9 = 16;
              return nullable.GetValueOrDefault() == num9 & nullable.HasValue;
            }
          }
          return false;
        })).Select<FieldTypeUpdateEventData, string>((Func<FieldTypeUpdateEventData, string>) (ftc => ftc.ReferenceName)), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        HashSet<string> fromKeywordToPerson = new HashSet<string>(((IEnumerable<FieldTypeUpdateEventData>) fieldTypeChanges).Where<FieldTypeUpdateEventData>((Func<FieldTypeUpdateEventData, bool>) (ftc =>
        {
          int? oldType3 = ftc.OldType;
          int? nullable = oldType3.HasValue ? new int?(oldType3.GetValueOrDefault() & 24) : new int?();
          int num10 = 24;
          if (!(nullable.GetValueOrDefault() == num10 & nullable.HasValue))
          {
            int? oldType4 = ftc.OldType;
            nullable = oldType4.HasValue ? new int?(oldType4.GetValueOrDefault() & 16) : new int?();
            int num11 = 16;
            if (nullable.GetValueOrDefault() == num11 & nullable.HasValue)
            {
              int? newType = ftc.NewType;
              nullable = newType.HasValue ? new int?(newType.GetValueOrDefault() & 24) : new int?();
              int num12 = 24;
              return nullable.GetValueOrDefault() == num12 & nullable.HasValue;
            }
          }
          return false;
        })).Select<FieldTypeUpdateEventData, string>((Func<FieldTypeUpdateEventData, string>) (ftc => ftc.ReferenceName)), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
        if (fromKeywordToPerson.Count <= 0 && fromPersonToKeyword.Count <= 0)
          return;
        StringComparer serverStringComparer = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ServerStringComparer;
        string[] array1 = fromKeywordToPerson.Concat<string>((IEnumerable<string>) fromPersonToKeyword).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName).ToArray<string>();
        IEnumerable<KeyValuePair<Guid, string>> allQueryTexts;
        using (DalSqlResourceComponent component = DalSqlResourceComponent.CreateComponent(requestContext))
          allQueryTexts = component.GetAllQueryTexts();
        HashSet<Guid> tfIdsToLook = new HashSet<Guid>();
        HashSet<string> displayNamesToLook = new HashSet<string>((IEqualityComparer<string>) serverStringComparer);
        Dictionary<Guid, NodeSelect> source = new Dictionary<Guid, NodeSelect>();
        foreach (KeyValuePair<Guid, string> keyValuePair in allQueryTexts)
        {
          string queryText = keyValuePair.Value;
          if (!string.IsNullOrEmpty(queryText) && ((IEnumerable<string>) array1).Any<string>((Func<string, bool>) (refName => TFStringComparer.WorkItemFieldReferenceName.IndexOf(queryText, refName) > 0)))
          {
            NodeSelect syntax = Parser.ParseSyntax(queryText);
            bool needsTouch = false;
            WiqlTransformUtils.WalkNode(syntax.Where, (Func<NodeCondition, NodeFieldName, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>) ((conditionNode, fieldNameNode, valueNode) =>
            {
              string constStringValue = valueNode.ConstStringValue;
              if (!string.IsNullOrEmpty(constStringValue))
              {
                if (fromPersonToKeyword.Contains(fieldNameNode.Value))
                {
                  if (constStringValue.StartsWith("\a", StringComparison.Ordinal))
                  {
                    Guid result;
                    if (constStringValue.Length >= 45 && constStringValue.StartsWith("\aTFID:'", StringComparison.Ordinal) && Guid.TryParse(constStringValue.Substring(7, 36), out result))
                      tfIdsToLook.Add(result);
                    needsTouch = true;
                  }
                }
                else if (fromKeywordToPerson.Contains(fieldNameNode.Value))
                {
                  if (constStringValue.StartsWith("\a", StringComparison.Ordinal))
                    displayNamesToLook.Add(constStringValue.Substring(1));
                  else
                    displayNamesToLook.Add(constStringValue);
                  needsTouch = true;
                }
              }
              return valueNode;
            }));
            if (needsTouch)
              source[keyValuePair.Key] = syntax;
          }
        }
        if (source.Count <= 0)
          return;
        Dictionary<Guid, string> tfIdDisplayNameMap = new Dictionary<Guid, string>();
        if (tfIdsToLook.Count > 0)
        {
          foreach (ConstantRecord constantRecord in (IEnumerable<ConstantRecord>) new DataAccessLayerImpl(requestContext).GetConstantRecordsAll(tfIdsToLook.Select<Guid, string>((Func<Guid, string>) (id => id.ToString())).ToArray<string>(), ConstantRecordSearchFactor.TeamFoundationId))
            tfIdDisplayNameMap[constantRecord.TeamFoundationId] = constantRecord.DisplayValue;
        }
        Dictionary<string, Guid> displayNameTfIdMap = displayNamesToLook.Count <= 0 ? new Dictionary<string, Guid>((IEqualityComparer<string>) serverStringComparer) : WiqlTransformUtils.MapDisplayNamesToTeamFoundationId(requestContext, (IEnumerable<string>) displayNamesToLook, serverStringComparer);
        foreach (KeyValuePair<Guid, NodeSelect> keyValuePair in source)
          WiqlTransformUtils.WalkNode(keyValuePair.Value.Where, (Func<NodeCondition, NodeFieldName, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>) ((conditionNode, fieldNameNode, valueNode) =>
          {
            string constStringValue = valueNode?.ConstStringValue;
            if (!string.IsNullOrEmpty(constStringValue))
            {
              if (fromPersonToKeyword.Contains(fieldNameNode.Value))
              {
                if (constStringValue.StartsWith("\a", StringComparison.Ordinal))
                {
                  Guid result;
                  string s;
                  return constStringValue.Length >= 45 && constStringValue.StartsWith("\aTFID:'", StringComparison.Ordinal) && Guid.TryParse(constStringValue.Substring(7, 36), out result) && tfIdDisplayNameMap.TryGetValue(result, out s) ? (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(s) : (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(constStringValue.Substring(1));
                }
              }
              else if (fromKeywordToPerson.Contains(fieldNameNode.Value))
              {
                string key = !constStringValue.StartsWith("\a", StringComparison.Ordinal) ? constStringValue : constStringValue.Substring(1);
                Guid guid;
                return displayNameTfIdMap.TryGetValue(key, out guid) ? (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString("\a" + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TFID:'{0}'", (object) guid)) : (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString("\a" + key);
              }
            }
            return valueNode;
          }));
        KeyValuePair<Guid, string>[] array2 = source.Select<KeyValuePair<Guid, NodeSelect>, KeyValuePair<Guid, string>>((Func<KeyValuePair<Guid, NodeSelect>, KeyValuePair<Guid, string>>) (pair => new KeyValuePair<Guid, string>(pair.Key, pair.Value.ToString()))).ToArray<KeyValuePair<Guid, string>>();
        using (DalSqlResourceComponent component = DalSqlResourceComponent.CreateComponent(requestContext))
          component.UpdateQueryTexts((IEnumerable<KeyValuePair<Guid, string>>) array2);
      }));
    }

    private static void AddIdentitiesToMru(
      IVssRequestContext requestContext,
      ResolvedIdentityNamesInfo resolvedNamesInfo,
      HashSet<string> identityDisplayNames)
    {
      if (!identityDisplayNames.Any<string>())
        return;
      List<Guid> source = new List<Guid>();
      source.AddRange(resolvedNamesInfo.NamesLookup.Where<KeyValuePair<string, ConstantsSearchRecord>>((Func<KeyValuePair<string, ConstantsSearchRecord>, bool>) (pair => identityDisplayNames.Contains(pair.Key))).Select<KeyValuePair<string, ConstantsSearchRecord>, Guid>((Func<KeyValuePair<string, ConstantsSearchRecord>, Guid>) (pair => pair.Value.TeamFoundationId)));
      foreach (KeyValuePair<string, ConstantsSearchRecord[]> keyValuePair in (IEnumerable<KeyValuePair<string, ConstantsSearchRecord[]>>) resolvedNamesInfo.AmbiguousNamesLookup)
      {
        if (identityDisplayNames.Contains(keyValuePair.Key))
          source.AddRange(((IEnumerable<ConstantsSearchRecord>) keyValuePair.Value).Select<ConstantsSearchRecord, Guid>((Func<ConstantsSearchRecord, Guid>) (identity => identity.TeamFoundationId)));
      }
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) resolvedNamesInfo.AadNamesLookup)
      {
        if (resolvedNamesInfo.IdentityMap.Value.ContainsKey(keyValuePair.Value))
          source.Add(resolvedNamesInfo.IdentityMap.Value[keyValuePair.Value].Id);
      }
      if (source.Count <= 0)
        return;
      IList<Guid> list = (IList<Guid>) source.Where<Guid>((Func<Guid, bool>) (g => g != Guid.Empty)).ToList<Guid>();
      if (!list.Any<Guid>())
        return;
      requestContext.GetService<IdentityMruService>().AddMruIdentities(requestContext.Elevate(), requestContext.GetUserIdentity().Id, IdentityMruService.SharedDefaultContainerId, list);
    }

    private static Dictionary<string, Guid> MapDisplayNamesToTeamFoundationId(
      IVssRequestContext requestContext,
      IEnumerable<string> displayNamesToLook,
      StringComparer itemComparer)
    {
      List<ConstantRecord> constantRecordsAll = new DataAccessLayerImpl(requestContext).GetConstantRecordsAll(displayNamesToLook.ToArray<string>(), ConstantRecordSearchFactor.DisplayName, true);
      Dictionary<string, Guid> teamFoundationId = new Dictionary<string, Guid>((IEqualityComparer<string>) itemComparer);
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) itemComparer);
      foreach (ConstantRecord constantRecord in (IEnumerable<ConstantRecord>) constantRecordsAll)
      {
        if (teamFoundationId.ContainsKey(constantRecord.DisplayValue))
        {
          teamFoundationId.Remove(constantRecord.DisplayValue);
          stringSet.Add(constantRecord.DisplayValue);
        }
        else if (constantRecord.TeamFoundationId == Guid.Empty)
          stringSet.Add(constantRecord.DisplayValue);
        else if (!stringSet.Contains(constantRecord.DisplayValue))
          teamFoundationId.Add(constantRecord.DisplayValue, constantRecord.TeamFoundationId);
      }
      return teamFoundationId;
    }

    private static void WalkNode(
      Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node,
      Func<NodeCondition, NodeFieldName, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node, Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node> visitor)
    {
      if (node == null)
        return;
      switch (node.NodeType)
      {
        case NodeType.FieldCondition:
          NodeCondition nodeCondition = (NodeCondition) node;
          NodeFieldName left = nodeCondition.Left;
          Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node right = nodeCondition.Right;
          if ((right != null ? (right.NodeType == NodeType.ValueList ? 1 : 0) : 0) != 0)
          {
            for (int i = 0; i < nodeCondition.Right.Count; ++i)
              nodeCondition.Right[i] = visitor(nodeCondition, left, nodeCondition.Right[i]);
            break;
          }
          nodeCondition.Right = visitor(nodeCondition, left, nodeCondition.Right);
          break;
        case NodeType.Not:
          WiqlTransformUtils.WalkNode(((NodeNotOperator) node).Value, visitor);
          break;
        case NodeType.Ever:
          WiqlTransformUtils.WalkNode(((NodeEverOperator) node).Value, visitor);
          break;
        case NodeType.And:
        case NodeType.Or:
          int count = node.Count;
          for (int i = 0; i < count; ++i)
            WiqlTransformUtils.WalkNode(node[i], visitor);
          break;
      }
    }
  }
}
