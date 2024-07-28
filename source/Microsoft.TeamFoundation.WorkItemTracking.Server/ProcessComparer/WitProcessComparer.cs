// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.WitProcessComparer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer
{
  public class WitProcessComparer
  {
    private const string defaultFieldsXml = "\r\n<FIELDS>    \r\n    <FIELD name='Iteration ID' refname='System.IterationId' type='Integer' />\r\n    <FIELD name='External Link Count' refname='System.ExternalLinkCount' type='Integer' />\r\n    <FIELD name='Team Project' refname='System.TeamProject' type='String' reportable='dimension' />\r\n    <FIELD name='Hyperlink Count' refname='System.HyperLinkCount' type='Integer' />\r\n    <FIELD name='Attached File Count' refname='System.AttachedFileCount' type='Integer' />\r\n    <FIELD name='Node Name' refname='System.NodeName' type='String' />\r\n    <FIELD name='Revised Date' refname='System.RevisedDate' type='DateTime' reportable='detail' />\r\n    <FIELD name='Changed Date' refname='System.ChangedDate' type='DateTime' reportable='dimension' />\r\n    <FIELD name='ID' refname='System.Id' type='Integer' reportable='dimension' />\r\n    <FIELD name='Area ID' refname='System.AreaId' type='Integer' />\r\n    <FIELD name='Authorized As' refname='System.AuthorizedAs' type='String' syncnamechanges='true' />\r\n    <FIELD name='Authorized Date' refname='System.AuthorizedDate' type='DateTime' />\r\n    <FIELD name='Watermark' refname='System.Watermark' type='Integer' />\r\n    <FIELD name='Rev' refname='System.Rev' type='Integer' reportable='dimension' />\r\n    <FIELD name='Changed By' refname='System.ChangedBy' type='String' syncnamechanges='true' reportable='dimension' />\r\n    <FIELD name='Work Item Type' refname='System.WorkItemType' type='String' reportable='dimension' />\r\n    <FIELD name='Created Date' refname='System.CreatedDate' type='DateTime' reportable='dimension' />\r\n    <FIELD name='Created By' refname='System.CreatedBy' type='String' syncnamechanges='true' reportable='dimension' />\r\n    <FIELD name='Description' refname='System.Description' type='HTML' />\r\n    <FIELD name='Related Link Count' refname='System.RelatedLinkCount' type='Integer' />\r\n    <FIELD name='Tags' refname='System.Tags' type='PlainText' />\r\n    <FIELD name='Board Column' refname='System.BoardColumn' type='String' reportable='dimension' />\r\n    <FIELD name='Board Column Done' refname='System.BoardColumnDone' type='Boolean' reportable='dimension' />\r\n    <FIELD name='Board Lane' refname='System.BoardLane' type='String' reportable='dimension' />\r\n    <FIELD name='History' refname='System.History' type='History' />\r\n    <FIELD name='Comment Count' refname='System.CommentCount' type='Integer' />\r\n    <FIELD name='Remote Link Count' refname='System.RemoteLinkCount' type='Integer' />\r\n    <FIELD name='Parent' refname='System.Parent' type='Integer' />\r\n</FIELDS>\r\n";
    private static readonly List<WorkItemFieldDeclaration> s_defaultFieldDeclarations = new List<WorkItemFieldDeclaration>();
    private static readonly LinkTypeDeclaration[] s_defaultLinkTypes = new LinkTypeDeclaration[1]
    {
      new LinkTypeDeclaration()
      {
        ReferenceName = "Microsoft.VSTS.Common.Affects",
        ForwardName = "Affects",
        ReverseName = "Affected By",
        Topology = "Dependency"
      }
    };
    private static ISet<string> s_fieldRuleWhitelist = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName)
    {
      "Microsoft.VSTS.Common.ResolvedReason",
      "Microsoft.VSTS.Common.ClosedDate",
      "Microsoft.VSTS.Common.ClosedBy",
      "Microsoft.VSTS.Common.ActivatedBy",
      "Microsoft.VSTS.Common.ActivatedDate",
      "Microsoft.VSTS.Common.ResolvedDate",
      "Microsoft.VSTS.Common.ResolvedBy",
      "Microsoft.VSTS.Scheduling.RemainingWork"
    };
    private static Dictionary<string, Dictionary<string, ISet<WorkItemRuleName>>> s_specificFieldRuleWhiteListForWorkItem = new Dictionary<string, Dictionary<string, ISet<WorkItemRuleName>>>((IEqualityComparer<string>) TFStringComparer.WorkItemType)
    {
      {
        "Test Case",
        new Dictionary<string, ISet<WorkItemRuleName>>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName)
        {
          {
            "Microsoft.VSTS.Common.Priority",
            (ISet<WorkItemRuleName>) new HashSet<WorkItemRuleName>()
            {
              WorkItemRuleName.Default
            }
          }
        }
      }
    };

    private static IReadOnlyCollection<WorkItemFieldDeclaration> GetDefaultFieldDecalarations()
    {
      if (!WitProcessComparer.s_defaultFieldDeclarations.Any<WorkItemFieldDeclaration>())
      {
        XElement xelement = XElement.Parse("\r\n<FIELDS>    \r\n    <FIELD name='Iteration ID' refname='System.IterationId' type='Integer' />\r\n    <FIELD name='External Link Count' refname='System.ExternalLinkCount' type='Integer' />\r\n    <FIELD name='Team Project' refname='System.TeamProject' type='String' reportable='dimension' />\r\n    <FIELD name='Hyperlink Count' refname='System.HyperLinkCount' type='Integer' />\r\n    <FIELD name='Attached File Count' refname='System.AttachedFileCount' type='Integer' />\r\n    <FIELD name='Node Name' refname='System.NodeName' type='String' />\r\n    <FIELD name='Revised Date' refname='System.RevisedDate' type='DateTime' reportable='detail' />\r\n    <FIELD name='Changed Date' refname='System.ChangedDate' type='DateTime' reportable='dimension' />\r\n    <FIELD name='ID' refname='System.Id' type='Integer' reportable='dimension' />\r\n    <FIELD name='Area ID' refname='System.AreaId' type='Integer' />\r\n    <FIELD name='Authorized As' refname='System.AuthorizedAs' type='String' syncnamechanges='true' />\r\n    <FIELD name='Authorized Date' refname='System.AuthorizedDate' type='DateTime' />\r\n    <FIELD name='Watermark' refname='System.Watermark' type='Integer' />\r\n    <FIELD name='Rev' refname='System.Rev' type='Integer' reportable='dimension' />\r\n    <FIELD name='Changed By' refname='System.ChangedBy' type='String' syncnamechanges='true' reportable='dimension' />\r\n    <FIELD name='Work Item Type' refname='System.WorkItemType' type='String' reportable='dimension' />\r\n    <FIELD name='Created Date' refname='System.CreatedDate' type='DateTime' reportable='dimension' />\r\n    <FIELD name='Created By' refname='System.CreatedBy' type='String' syncnamechanges='true' reportable='dimension' />\r\n    <FIELD name='Description' refname='System.Description' type='HTML' />\r\n    <FIELD name='Related Link Count' refname='System.RelatedLinkCount' type='Integer' />\r\n    <FIELD name='Tags' refname='System.Tags' type='PlainText' />\r\n    <FIELD name='Board Column' refname='System.BoardColumn' type='String' reportable='dimension' />\r\n    <FIELD name='Board Column Done' refname='System.BoardColumnDone' type='Boolean' reportable='dimension' />\r\n    <FIELD name='Board Lane' refname='System.BoardLane' type='String' reportable='dimension' />\r\n    <FIELD name='History' refname='System.History' type='History' />\r\n    <FIELD name='Comment Count' refname='System.CommentCount' type='Integer' />\r\n    <FIELD name='Remote Link Count' refname='System.RemoteLinkCount' type='Integer' />\r\n    <FIELD name='Parent' refname='System.Parent' type='Integer' />\r\n</FIELDS>\r\n");
        WitProcessComparer.s_defaultFieldDeclarations.AddRange(xelement.Elements((XName) "FIELD").Select<XElement, WorkItemFieldDeclaration>((Func<XElement, WorkItemFieldDeclaration>) (e => new WorkItemFieldDeclaration(e, (Action<string>) (s =>
        {
          throw new Exception(s);
        })))));
      }
      return (IReadOnlyCollection<WorkItemFieldDeclaration>) WitProcessComparer.s_defaultFieldDeclarations;
    }

    public static void CompareAndValidate(
      WorkItemTrackingTemplateArtifacts systemArtifacts,
      WorkItemTrackingTemplateArtifacts customArtifacts,
      Action<string> logError,
      Action<string> logWarning = null)
    {
      WitProcessComparer.CompareAndValidateLinkTypeCollections((IEnumerable<LinkTypeDeclaration>) systemArtifacts.LinkTypes, (IEnumerable<LinkTypeDeclaration>) customArtifacts.LinkTypes, logError, logWarning);
      WitProcessComparer.CompareAndValidateTypeCategoryCollections((IEnumerable<WorkItemTypeCategoryDeclaration>) systemArtifacts.Categories, (IEnumerable<WorkItemTypeCategoryDeclaration>) customArtifacts.Categories, logError, logWarning);
      WitProcessComparer.CompareAndValidateTypeCollections((IEnumerable<WorkItemTypeDeclaration>) systemArtifacts.Types, (IEnumerable<WorkItemTypeDeclaration>) customArtifacts.Types, logError, logWarning);
      WitProcessComparer.CompareAndValidateProcessConfiguration(systemArtifacts.ProcessConfiguration, customArtifacts.ProcessConfiguration, logError, logWarning);
      WitProcessComparer.CompareAndValidateExternalGlobalLists(customArtifacts.GlobalLists, logError);
    }

    private static void CompareAndValidateLinkTypeCollections(
      IEnumerable<LinkTypeDeclaration> systemLinks,
      IEnumerable<LinkTypeDeclaration> customLinks,
      Action<string> logError,
      Action<string> logWarning = null)
    {
      Dictionary<string, LinkTypeDeclaration> dictionary1 = systemLinks.ToDictionary<LinkTypeDeclaration, string>((Func<LinkTypeDeclaration, string>) (lt => lt.ReferenceName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, LinkTypeDeclaration> dictionary2 = customLinks.ToDictionary<LinkTypeDeclaration, string>((Func<LinkTypeDeclaration, string>) (lt => lt.ReferenceName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (LinkTypeDeclaration defaultLinkType in WitProcessComparer.s_defaultLinkTypes)
      {
        if (!dictionary1.ContainsKey(defaultLinkType.ReferenceName))
          dictionary1.Add(defaultLinkType.ReferenceName, defaultLinkType);
        if (!dictionary2.ContainsKey(defaultLinkType.ReferenceName))
          dictionary2.Add(defaultLinkType.ReferenceName, defaultLinkType);
      }
      foreach (LinkTypeDeclaration linkTypeDeclaration1 in dictionary1.Values)
      {
        LinkTypeDeclaration linkTypeDeclaration2;
        if (!dictionary2.TryGetValue(linkTypeDeclaration1.ReferenceName, out linkTypeDeclaration2))
          WitProcessComparer.LogErrorOrWarning("Custom process does not contain link type '" + linkTypeDeclaration1.ReferenceName + "'.", "The link type will be created, if this process is otherwise found to be a match.", logError, logWarning);
        else if (!StringComparer.Ordinal.Equals(linkTypeDeclaration1.Topology, linkTypeDeclaration2.Topology))
          logError("Custom process changed link type topology '" + linkTypeDeclaration1.ReferenceName + "'. Expected: " + linkTypeDeclaration1.Topology + ", Found: " + linkTypeDeclaration2.Topology + ".");
      }
      foreach (LinkTypeDeclaration linkTypeDeclaration in dictionary2.Values)
      {
        if (!dictionary1.ContainsKey(linkTypeDeclaration.ReferenceName))
          logError("Custom process added a link type '" + linkTypeDeclaration.ReferenceName + "'.");
      }
    }

    private static void CompareAndValidateTypeCategoryCollections(
      IEnumerable<WorkItemTypeCategoryDeclaration> systemArtifacts,
      IEnumerable<WorkItemTypeCategoryDeclaration> customArtifacts,
      Action<string> logError,
      Action<string> logWarning = null)
    {
      Dictionary<string, WorkItemTypeCategoryDeclaration> dictionary1 = systemArtifacts.ToDictionary<WorkItemTypeCategoryDeclaration, string>((Func<WorkItemTypeCategoryDeclaration, string>) (cat => cat.ReferenceName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, WorkItemTypeCategoryDeclaration> dictionary2 = customArtifacts.ToDictionary<WorkItemTypeCategoryDeclaration, string>((Func<WorkItemTypeCategoryDeclaration, string>) (cat => cat.ReferenceName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (WorkItemTypeCategoryDeclaration categoryDeclaration1 in dictionary1.Values)
      {
        WorkItemTypeCategoryDeclaration categoryDeclaration2;
        if (!dictionary2.TryGetValue(categoryDeclaration1.ReferenceName, out categoryDeclaration2))
        {
          WitProcessComparer.LogErrorOrWarning("Custom process does not contain category '" + categoryDeclaration1.ReferenceName + "'.", "The category will be created, if this process is otherwise found to be a match.", logError, logWarning);
        }
        else
        {
          if (!StringComparer.OrdinalIgnoreCase.Equals(categoryDeclaration1.DefaultWorkItemTypeName, categoryDeclaration2.DefaultWorkItemTypeName))
            WitProcessComparer.LogErrorOrWarning("Custom process changed default work item type in category '" + categoryDeclaration1.ReferenceName + "'. Expected: " + categoryDeclaration1.DefaultWorkItemTypeName + ", Found: " + categoryDeclaration2.DefaultWorkItemTypeName + ".", "This will be reset, if this process is otherwise found to be a match.", logError, logWarning);
          if (!categoryDeclaration1.WorkItemTypeNames.SetEquals((IEnumerable<string>) categoryDeclaration2.WorkItemTypeNames))
            WitProcessComparer.LogErrorOrWarning("Custom process changed default work item type list in category '" + categoryDeclaration1.ReferenceName + "'. Expected: [" + string.Join(",", (IEnumerable<string>) categoryDeclaration1.WorkItemTypeNames) + "], Found: [" + string.Join(", ", (IEnumerable<string>) categoryDeclaration2.WorkItemTypeNames) + "].", "This will be reset, if this process is otherwise found to be a match.", logError, logWarning);
        }
      }
      foreach (WorkItemTypeCategoryDeclaration categoryDeclaration in dictionary2.Values)
      {
        if (!dictionary1.ContainsKey(categoryDeclaration.ReferenceName))
          logError("Custom process added a new type category '" + categoryDeclaration.ReferenceName + "'.");
      }
    }

    private static Dictionary<string, WorkItemTypeDeclaration> ConvertWitdeclarationToDictionary(
      IEnumerable<WorkItemTypeDeclaration> source,
      Action<string> logError)
    {
      Dictionary<string, WorkItemTypeDeclaration> dictionary = new Dictionary<string, WorkItemTypeDeclaration>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      foreach (WorkItemTypeDeclaration itemTypeDeclaration in source)
      {
        if (!dictionary.ContainsKey(itemTypeDeclaration.Name))
          dictionary.Add(itemTypeDeclaration.Name, itemTypeDeclaration);
        else
          logError(ServerResources.DuplicatedWorkItemType((object) itemTypeDeclaration.Name));
      }
      return dictionary;
    }

    private static void CompareAndValidateTypeCollections(
      IEnumerable<WorkItemTypeDeclaration> systemArtifacts,
      IEnumerable<WorkItemTypeDeclaration> customArtifacts,
      Action<string> logError,
      Action<string> logWarning = null)
    {
      Dictionary<string, WorkItemTypeDeclaration> dictionary1 = WitProcessComparer.ConvertWitdeclarationToDictionary(systemArtifacts, logError);
      Dictionary<string, WorkItemTypeDeclaration> dictionary2 = WitProcessComparer.ConvertWitdeclarationToDictionary(customArtifacts, logError);
      foreach (WorkItemTypeDeclaration systemType in dictionary1.Values)
      {
        WorkItemTypeDeclaration customType;
        if (!dictionary2.TryGetValue(systemType.Name, out customType))
          WitProcessComparer.LogErrorOrWarning("Custom process does not contain type '" + systemType.ReferenceName + "'.", "The type will be created, if this process is otherwise found to be a match.", logError, logWarning);
        else
          WitProcessComparer.CompareTypes(systemType, customType, logError, logWarning);
      }
      foreach (WorkItemTypeDeclaration workItemType in dictionary2.Values)
      {
        if (!dictionary1.ContainsKey(workItemType.Name))
          logError("Custom process added a new type '" + workItemType.ReferenceName + "'.");
        WitProcessComparer.ValidateWorkItemTypeGlobalLists(workItemType, logError);
      }
    }

    private static void CompareTypes(
      WorkItemTypeDeclaration systemType,
      WorkItemTypeDeclaration customType,
      Action<string> logError,
      Action<string> logWarning = null)
    {
      WitProcessComparer.CompareFieldCollections(systemType.Name, (IEnumerable<WorkItemFieldDeclaration>) systemType.Fields, (IEnumerable<WorkItemFieldDeclaration>) customType.Fields, logError, logWarning);
      WitProcessComparer.CompareStateCollections(systemType.Name, (IEnumerable<WorkItemStateDeclaration>) systemType.States, (IEnumerable<WorkItemStateDeclaration>) customType.States, logError, logWarning);
      WitProcessComparer.CompareStateTransitionCollections(systemType.Name, (IEnumerable<WorkItemTransitionDeclaration>) systemType.Transitions, (IEnumerable<WorkItemTransitionDeclaration>) customType.Transitions, logError, logWarning);
    }

    private static void CompareFieldCollections(
      string workItemTypeName,
      IEnumerable<WorkItemFieldDeclaration> systemFields,
      IEnumerable<WorkItemFieldDeclaration> customFields,
      Action<string> logError,
      Action<string> logWarning = null)
    {
      Dictionary<string, WorkItemFieldDeclaration> dictionary1 = systemFields.ToDictionary<WorkItemFieldDeclaration, string>((Func<WorkItemFieldDeclaration, string>) (field => field.ReferenceName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, WorkItemFieldDeclaration> dictionary2 = customFields.ToDictionary<WorkItemFieldDeclaration, string>((Func<WorkItemFieldDeclaration, string>) (field => field.ReferenceName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (WorkItemFieldDeclaration fieldDecalaration in (IEnumerable<WorkItemFieldDeclaration>) WitProcessComparer.GetDefaultFieldDecalarations())
      {
        if (!dictionary1.ContainsKey(fieldDecalaration.ReferenceName))
          dictionary1.Add(fieldDecalaration.ReferenceName, fieldDecalaration);
        if (!dictionary2.ContainsKey(fieldDecalaration.ReferenceName))
          dictionary2.Add(fieldDecalaration.ReferenceName, fieldDecalaration);
      }
      foreach (WorkItemFieldDeclaration fieldDeclaration1 in dictionary1.Values)
      {
        WorkItemFieldDeclaration fieldDeclaration2;
        if (!dictionary2.TryGetValue(fieldDeclaration1.ReferenceName, out fieldDeclaration2))
        {
          WitProcessComparer.LogErrorOrWarning("Custom type '" + workItemTypeName + "' is missing the field " + fieldDeclaration1.ReferenceName + ".", "The field will be added back, if this process is otherwise found to be a match.", logError, logWarning);
        }
        else
        {
          if (!StringComparer.Ordinal.Equals(fieldDeclaration1.Type, fieldDeclaration2.Type))
            logError("Custom type '" + workItemTypeName + "' changed the type of field " + fieldDeclaration1.ReferenceName + ". Expected: " + fieldDeclaration1.Type + ", Found: " + fieldDeclaration2.Type);
          if (fieldDeclaration1.SyncNameChanges != fieldDeclaration2.SyncNameChanges)
            WitProcessComparer.LogErrorOrWarning(string.Format("Custom type '{0}' changed the syncnamechanges property of field {1}. Expected: {2}, Found: {3}", (object) workItemTypeName, (object) fieldDeclaration1.ReferenceName, (object) fieldDeclaration1.SyncNameChanges, (object) fieldDeclaration2.SyncNameChanges), "This will be automatically fixed, if this process is otherwise found to be a match.", logError, logWarning);
          WitProcessComparer.CompareFieldRules(workItemTypeName, workItemTypeName, fieldDeclaration1.ReferenceName, (IEnumerable<WorkItemRuleDeclaration>) fieldDeclaration1.Rules.Children, (IEnumerable<WorkItemRuleDeclaration>) fieldDeclaration2.Rules.Children, logError, logWarning);
        }
      }
      foreach (WorkItemFieldDeclaration fieldDeclaration3 in dictionary2.Values)
      {
        WorkItemFieldDeclaration fieldDeclaration4 = (WorkItemFieldDeclaration) null;
        if (!dictionary1.TryGetValue(fieldDeclaration3.ReferenceName, out fieldDeclaration4))
        {
          logError("Custom type '" + workItemTypeName + "' added a new field '" + fieldDeclaration3.ReferenceName + "'.");
        }
        else
        {
          if (string.Compare(fieldDeclaration4.Name, fieldDeclaration3.Name, StringComparison.OrdinalIgnoreCase) != 0)
            logError("Custom type '" + workItemTypeName + "' changed name of the field '" + fieldDeclaration4.ReferenceName + "' from '" + fieldDeclaration4.Name + "' to '" + fieldDeclaration3.Name + "'");
          if (string.Compare(fieldDeclaration4.Type, fieldDeclaration3.Type, StringComparison.Ordinal) != 0)
            logError("Custom type '" + workItemTypeName + "' changed type of the field '" + fieldDeclaration4.ReferenceName + "' from '" + fieldDeclaration4.Type + "' to '" + fieldDeclaration3.Type + "'");
        }
      }
    }

    private static void CompareFieldRules(
      string workItemTypeName,
      string levelName,
      string fieldName,
      IEnumerable<WorkItemRuleDeclaration> systemRules,
      IEnumerable<WorkItemRuleDeclaration> customRules,
      Action<string> logError,
      Action<string> logWarning = null)
    {
      if (BlockRuleDeclaration.Equals(systemRules, customRules, true, (Func<WorkItemRuleDeclaration, bool>) (r => r.Name != WorkItemRuleName.AllowExistingValue && r.Name != WorkItemRuleName.HelpText && !WitProcessComparer.IsRuleWhiteListed(r, fieldName, workItemTypeName))) || logWarning != null && WitProcessComparer.s_fieldRuleWhitelist.Contains(fieldName))
        return;
      logError("Rules for field '" + fieldName + "' do not match at level '" + levelName + "'.");
    }

    private static void CompareFieldRuleCollections(
      string workItemTypeName,
      string levelName,
      IEnumerable<WorkItemFieldRuleDeclarations> systemRules,
      IEnumerable<WorkItemFieldRuleDeclarations> customRules,
      Action<string> logError,
      Action<string> logWarning = null)
    {
      Dictionary<string, WorkItemFieldRuleDeclarations> dictionary1 = systemRules.ToDictionary<WorkItemFieldRuleDeclarations, string>((Func<WorkItemFieldRuleDeclarations, string>) (r => r.Field), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, WorkItemFieldRuleDeclarations> dictionary2 = customRules.ToDictionary<WorkItemFieldRuleDeclarations, string>((Func<WorkItemFieldRuleDeclarations, string>) (r => r.Field), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (WorkItemFieldRuleDeclarations ruleDeclarations1 in dictionary1.Values)
      {
        WorkItemFieldRuleDeclarations ruleDeclarations2;
        if (!dictionary2.TryGetValue(ruleDeclarations1.Field, out ruleDeclarations2))
        {
          if (logWarning == null || !WitProcessComparer.s_fieldRuleWhitelist.Contains(ruleDeclarations1.Field))
            WitProcessComparer.LogErrorOrWarning("Custom type '" + workItemTypeName + "' is missing rules for field '" + ruleDeclarations1.Field + "' at level '" + levelName + "'.", "The rules will be added back, if this process is otherwise found to be a match.", logError, logWarning);
        }
        else
          WitProcessComparer.CompareFieldRules(workItemTypeName, workItemTypeName + "/" + levelName, ruleDeclarations1.Field, (IEnumerable<WorkItemRuleDeclaration>) ruleDeclarations1.Children, (IEnumerable<WorkItemRuleDeclaration>) ruleDeclarations2.Children, logError, logWarning);
      }
      foreach (WorkItemFieldRuleDeclarations fieldRuleDeclaration in dictionary2.Values)
      {
        if (!dictionary1.ContainsKey(fieldRuleDeclaration.Field) && (logWarning == null || !levelName.Contains("state") && !levelName.Contains("transition") || !WitProcessComparer.s_fieldRuleWhitelist.Contains(fieldRuleDeclaration.Field) && !WitProcessComparer.IsRuleWhiteListed(fieldRuleDeclaration, workItemTypeName)))
          logError("Custom type '" + workItemTypeName + "' added rules for field '" + fieldRuleDeclaration.Field + "' at level '" + levelName + "'.");
      }
    }

    private static void CompareStateCollections(
      string workItemTypeName,
      IEnumerable<WorkItemStateDeclaration> systemStates,
      IEnumerable<WorkItemStateDeclaration> customStates,
      Action<string> logError,
      Action<string> logWarning = null)
    {
      Dictionary<string, WorkItemStateDeclaration> dictionary1 = systemStates.ToDictionary<WorkItemStateDeclaration, string>((Func<WorkItemStateDeclaration, string>) (state => state.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, WorkItemStateDeclaration> dictionary2 = customStates.ToDictionary<WorkItemStateDeclaration, string>((Func<WorkItemStateDeclaration, string>) (state => state.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (WorkItemStateDeclaration stateDeclaration1 in dictionary1.Values)
      {
        WorkItemStateDeclaration stateDeclaration2;
        if (!dictionary2.TryGetValue(stateDeclaration1.Name, out stateDeclaration2))
          WitProcessComparer.LogErrorOrWarning("Custom type '" + workItemTypeName + "' is missing the state '" + stateDeclaration1.Name + "'.", "The state will be added, if this process is otherwise found to be a match.", logError, logWarning);
        else
          WitProcessComparer.CompareFieldRuleCollections(workItemTypeName, "state[" + stateDeclaration1.Name + "]", (IEnumerable<WorkItemFieldRuleDeclarations>) stateDeclaration1.Rules, (IEnumerable<WorkItemFieldRuleDeclarations>) stateDeclaration2.Rules, logError, logWarning);
      }
      foreach (WorkItemStateDeclaration stateDeclaration in dictionary2.Values)
      {
        if (!dictionary1.ContainsKey(stateDeclaration.Name))
          logError("Custom type '" + workItemTypeName + "' added a new state definition '" + stateDeclaration.Name + "'.");
      }
    }

    private static string TransitionKey(WorkItemTransitionDeclaration transition) => transition == null ? "*->*" : WitProcessComparer.TransitionKey(transition.From, transition.To);

    private static string TransitionKey(string from, string to) => from + "->" + to;

    private static void CompareStateTransitionCollections(
      string workItemTypeName,
      IEnumerable<WorkItemTransitionDeclaration> systemTransitions,
      IEnumerable<WorkItemTransitionDeclaration> customTransitions,
      Action<string> logError,
      Action<string> logWarning = null)
    {
      Dictionary<string, WorkItemTransitionDeclaration> dictionary1 = systemTransitions.ToDictionary<WorkItemTransitionDeclaration, string>((Func<WorkItemTransitionDeclaration, string>) (tr => WitProcessComparer.TransitionKey(tr)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, WorkItemTransitionDeclaration> dictionary2 = customTransitions.ToDictionary<WorkItemTransitionDeclaration, string>((Func<WorkItemTransitionDeclaration, string>) (tr => WitProcessComparer.TransitionKey(tr)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (WorkItemTransitionDeclaration transition in dictionary1.Values)
      {
        string str = WitProcessComparer.TransitionKey(transition);
        WorkItemTransitionDeclaration transitionDeclaration;
        if (!dictionary2.TryGetValue(str, out transitionDeclaration))
        {
          WitProcessComparer.LogErrorOrWarning("Custom type '" + workItemTypeName + "' is missing the state transition '" + str + "'.", "The transition will be added back, if this process is otherwise found to be a match.", logError, logWarning);
        }
        else
        {
          WitProcessComparer.CompareFieldRuleCollections(workItemTypeName, "transition[" + str + "]", (IEnumerable<WorkItemFieldRuleDeclarations>) transition.Rules, (IEnumerable<WorkItemFieldRuleDeclarations>) transitionDeclaration.Rules, logError, logWarning);
          WitProcessComparer.CompareStateTransitionReasonCollections(workItemTypeName, str, (IEnumerable<WorkItemTransitionReasonDeclaration>) transition.Reasons, (IEnumerable<WorkItemTransitionReasonDeclaration>) transitionDeclaration.Reasons, logError, logWarning);
        }
      }
      foreach (WorkItemTransitionDeclaration transition in dictionary2.Values)
      {
        string key = WitProcessComparer.TransitionKey(transition);
        if (!dictionary1.ContainsKey(key) && (logWarning == null || !TFStringComparer.WorkItemTypeName.Equals(workItemTypeName, "Bug") || !key.Equals(WitProcessComparer.TransitionKey("", "Active"), StringComparison.OrdinalIgnoreCase)))
          logError("Custom type '" + workItemTypeName + "' added a new state transition '" + key + "'.");
      }
    }

    private static void CompareStateTransitionReasonCollections(
      string workItemTypeName,
      string transition,
      IEnumerable<WorkItemTransitionReasonDeclaration> systemReasons,
      IEnumerable<WorkItemTransitionReasonDeclaration> customReasons,
      Action<string> logError,
      Action<string> logWarning = null)
    {
      Dictionary<string, WorkItemTransitionReasonDeclaration> dictionary1 = systemReasons.ToDictionary<WorkItemTransitionReasonDeclaration, string>((Func<WorkItemTransitionReasonDeclaration, string>) (reason => reason.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, WorkItemTransitionReasonDeclaration> dictionary2 = customReasons.ToDictionary<WorkItemTransitionReasonDeclaration, string>((Func<WorkItemTransitionReasonDeclaration, string>) (reason => reason.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (WorkItemTransitionReasonDeclaration reasonDeclaration1 in dictionary1.Values)
      {
        WorkItemTransitionReasonDeclaration reasonDeclaration2;
        if (!dictionary2.TryGetValue(reasonDeclaration1.Name, out reasonDeclaration2))
        {
          WitProcessComparer.LogErrorOrWarning("Custom type '" + workItemTypeName + "' is missing state transition reason '" + transition + "/" + reasonDeclaration1.Name + "'.", "The transition reason will be added back, if this process is otherwise found to be a match.", logError, logWarning);
        }
        else
        {
          if (reasonDeclaration1.IsDefault && !reasonDeclaration2.IsDefault)
            WitProcessComparer.LogErrorOrWarning("Custom type '" + workItemTypeName + "' changed the default state transition reason '" + transition + "/" + reasonDeclaration1.Name + "'.", "This will be reset, if this process is otherwise found to be a match.", logError, logWarning);
          WitProcessComparer.CompareFieldRuleCollections(workItemTypeName, "transition[" + transition + "]/reason[" + reasonDeclaration1.Name + "]", (IEnumerable<WorkItemFieldRuleDeclarations>) reasonDeclaration1.Rules, (IEnumerable<WorkItemFieldRuleDeclarations>) reasonDeclaration2.Rules, logError, logWarning);
        }
      }
      foreach (WorkItemTransitionReasonDeclaration reasonDeclaration in dictionary2.Values)
      {
        if (!dictionary1.ContainsKey(reasonDeclaration.Name))
          logError("Custom type '" + workItemTypeName + "' added a new state transition reason '" + transition + "/" + reasonDeclaration.Name + "'.");
      }
    }

    private static void ValidateWorkItemTypeGlobalLists(
      WorkItemTypeDeclaration workItemType,
      Action<string> logError)
    {
      if (workItemType.GlobalLists.Count <= 0)
        return;
      logError("Global lists not allowed in work item types. Found in " + workItemType.Name + ".");
    }

    private static void CompareAndValidateProcessConfiguration(
      ProcessConfigurationDeclaration systemProcessConfig,
      ProcessConfigurationDeclaration customProcessConfig,
      Action<string> logError,
      Action<string> logWarning = null)
    {
      if (systemProcessConfig == null && customProcessConfig == null)
        return;
      if (customProcessConfig == null)
        logError("Custom processConfiguration is not defined");
      else if (systemProcessConfig == null)
        logError("System processConfiguration is not defined");
      else if (customProcessConfig.TypeFields == null)
      {
        logError("Custom processConfiguration typeField is empty");
      }
      else
      {
        if (systemProcessConfig.TypeFields.Length != customProcessConfig.TypeFields.Length)
          logError(string.Format("Custom processConfiguration typeField count '{0}' is different than system '{1}'.", (object) customProcessConfig.TypeFields.Length, (object) systemProcessConfig.TypeFields.Length));
        Dictionary<string, TypeField> dictionary1 = ((IEnumerable<TypeField>) systemProcessConfig.TypeFields).ToDictionary<TypeField, string>((Func<TypeField, string>) (tf => tf.Name), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (TypeField typeField1 in customProcessConfig.TypeFields)
        {
          TypeField typeField2;
          if (!dictionary1.TryGetValue(typeField1.Name, out typeField2))
          {
            logError(string.Format("Custom processConfiguration contain additional typeField '{0}:{1}'.", (object) typeField1.Type, (object) typeField1.Name));
          }
          else
          {
            if (typeField2.Type != typeField1.Type)
              logError(string.Format("Custom processConfiguration changed refname for type '{0}'. Expected: {1}, Found: {2}.", (object) typeField2.Type, (object) typeField2.Name, (object) typeField1.Name));
            if (!StringComparer.OrdinalIgnoreCase.Equals(typeField2.Format, typeField1.Format))
              logError(string.Format("Custom processConfiguration changed typeField format for type '{0}'. Expected: {1}, Found: {2}.", (object) typeField2.Type, (object) typeField2.Format, (object) typeField1.Format));
            if (typeField1.TypeFieldValues == null && typeField2.TypeFieldValues != null)
              logError("Custom processConfiguration cleared the typeFieldValues.");
            if (typeField1.TypeFieldValues != null)
            {
              if (typeField2.TypeFieldValues == null || typeField2.TypeFieldValues.Length != typeField1.TypeFieldValues.Length)
                logError("Custom processConfiguration changed typeFieldValues count does not match.");
              Dictionary<string, TypeFieldValue> dictionary2 = ((IEnumerable<TypeFieldValue>) typeField2.TypeFieldValues).ToDictionary<TypeFieldValue, string>((Func<TypeFieldValue, string>) (tf => tf.Type), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
              foreach (TypeFieldValue typeFieldValue1 in typeField1.TypeFieldValues)
              {
                TypeFieldValue typeFieldValue2;
                if (!dictionary2.TryGetValue(typeFieldValue1.Type, out typeFieldValue2))
                  logError(string.Format("Custom processConfiguration added typeFieldValues for type '{0}'. {1}", (object) typeField1.Type, (object) typeFieldValue1.Type));
                if (!StringComparer.OrdinalIgnoreCase.Equals(typeFieldValue1.Value, typeFieldValue2.Value))
                  logError(string.Format("Custom processConfiguration changed typeFieldValues for type '{0}'. Expected: {1}, Found: {2}.", (object) typeField1.Type, (object) typeFieldValue2.Value, (object) typeFieldValue1.Value));
              }
            }
          }
        }
        if (customProcessConfig.PortfolioBacklogs != null)
        {
          if (systemProcessConfig.PortfolioBacklogs == null)
          {
            logError("System processConfiguration is missing portfolio backlogs");
            return;
          }
          if (customProcessConfig.PortfolioBacklogs.Length != systemProcessConfig.PortfolioBacklogs.Length)
            WitProcessComparer.LogErrorOrWarning(string.Format("Custom processConfiguration portfolio backlog count does not match. Custom: {0}, System: {1}", (object) customProcessConfig.PortfolioBacklogs.Length, (object) systemProcessConfig.PortfolioBacklogs.Length), "The missing backlog(s) will be created, if this process is otherwise found to be a match.", logError, logWarning);
          Dictionary<string, BacklogCategoryConfiguration> dictionary3 = ((IEnumerable<BacklogCategoryConfiguration>) systemProcessConfig.PortfolioBacklogs).ToDictionary<BacklogCategoryConfiguration, string>((Func<BacklogCategoryConfiguration, string>) (pb => pb.CategoryReferenceName));
          foreach (BacklogCategoryConfiguration portfolioBacklog in customProcessConfig.PortfolioBacklogs)
          {
            BacklogCategoryConfiguration systemBacklog;
            if (!dictionary3.TryGetValue(portfolioBacklog.CategoryReferenceName, out systemBacklog))
            {
              logError("Custom processConfiguration defines a portfolio backlog '" + portfolioBacklog.CategoryReferenceName + "' not found in system");
            }
            else
            {
              if (!StringComparer.OrdinalIgnoreCase.Equals(systemBacklog.ParentCategoryReferenceName, portfolioBacklog.ParentCategoryReferenceName) && (logWarning == null || !string.IsNullOrWhiteSpace(portfolioBacklog.ParentCategoryReferenceName)))
                WitProcessComparer.LogErrorOrWarning("Custom processConfiguration category '" + portfolioBacklog.CategoryReferenceName + "' does not match with system ParentCategoryReferenceName. Expected: '" + systemBacklog.ParentCategoryReferenceName + "',  Found: '" + portfolioBacklog.ParentCategoryReferenceName + "'.", "The system value will be used, if this process is otherwise found to be a match.", logError, logWarning);
              WitProcessComparer.CompareProcessConfigBacklog((CategoryConfiguration) systemBacklog, (CategoryConfiguration) portfolioBacklog, logError, logWarning);
            }
          }
        }
        if (systemProcessConfig.BugWorkItems == null && customProcessConfig.BugWorkItems != null)
          logError("Custom process added work item type BugWorkItems");
        else if (customProcessConfig.BugWorkItems != null)
          WitProcessComparer.CompareProcessConfigBacklog(systemProcessConfig.BugWorkItems, customProcessConfig.BugWorkItems, logError, logWarning);
        if (customProcessConfig.FeedbackRequestWorkItems != null)
          WitProcessComparer.CompareProcessConfigBacklog(systemProcessConfig.FeedbackRequestWorkItems, customProcessConfig.FeedbackRequestWorkItems, logError, logWarning);
        if (customProcessConfig.FeedbackResponseWorkItems != null)
          WitProcessComparer.CompareProcessConfigBacklog(systemProcessConfig.FeedbackResponseWorkItems, customProcessConfig.FeedbackResponseWorkItems, logError, logWarning);
        if (customProcessConfig.FeedbackWorkItems != null)
          WitProcessComparer.CompareProcessConfigBacklog(systemProcessConfig.FeedbackWorkItems, customProcessConfig.FeedbackWorkItems, logError, logWarning);
        if (customProcessConfig.ReleaseStageWorkItems != null)
          WitProcessComparer.CompareProcessConfigBacklog(systemProcessConfig.ReleaseStageWorkItems, customProcessConfig.ReleaseStageWorkItems, logError, logWarning);
        if (customProcessConfig.ReleaseWorkItems != null)
          WitProcessComparer.CompareProcessConfigBacklog(systemProcessConfig.ReleaseWorkItems, customProcessConfig.ReleaseWorkItems, logError, logWarning);
        if (customProcessConfig.RequirementBacklog != null)
          WitProcessComparer.CompareProcessConfigBacklog((CategoryConfiguration) systemProcessConfig.RequirementBacklog, (CategoryConfiguration) customProcessConfig.RequirementBacklog, logError, logWarning);
        if (customProcessConfig.StageSignoffTaskWorkItems != null)
          WitProcessComparer.CompareProcessConfigBacklog(systemProcessConfig.StageSignoffTaskWorkItems, customProcessConfig.StageSignoffTaskWorkItems, logError, logWarning);
        if (customProcessConfig.TaskBacklog != null)
          WitProcessComparer.CompareProcessConfigBacklog((CategoryConfiguration) systemProcessConfig.TaskBacklog, (CategoryConfiguration) customProcessConfig.TaskBacklog, logError, logWarning);
        if (customProcessConfig.TestPlanWorkItems != null)
          WitProcessComparer.CompareProcessConfigBacklog(systemProcessConfig.TestPlanWorkItems, customProcessConfig.TestPlanWorkItems, logError, logWarning);
        if (customProcessConfig.TestSuiteWorkItems != null)
          WitProcessComparer.CompareProcessConfigBacklog(systemProcessConfig.TestSuiteWorkItems, customProcessConfig.TestSuiteWorkItems, logError, logWarning);
        if (customProcessConfig.WorkItemColors == null)
          return;
        Dictionary<string, WorkItemColor> dictionary4 = ((IEnumerable<WorkItemColor>) systemProcessConfig.WorkItemColors).ToDictionary<WorkItemColor, string>((Func<WorkItemColor, string>) (witColor => witColor.WorkItemTypeName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (WorkItemColor workItemColor1 in customProcessConfig.WorkItemColors)
        {
          WorkItemColor workItemColor2;
          if (!dictionary4.TryGetValue(workItemColor1.WorkItemTypeName, out workItemColor2))
            logError("Custom processConfig has a WorkItem Color defined that is not found in system for type: " + workItemColor1.WorkItemTypeName);
          else if (!StringComparer.OrdinalIgnoreCase.Equals(workItemColor2.PrimaryColor, workItemColor1.PrimaryColor))
            WitProcessComparer.LogErrorOrWarning("Custom processConfig has changed the primary color for work item type '" + workItemColor1.WorkItemTypeName + "'. Expected: '" + workItemColor2.PrimaryColor + "', Found: '" + workItemColor1.PrimaryColor + "'", "The color will be reset, if this process is otherwise found to be a match.", logError, logWarning);
        }
      }
    }

    private static void CompareProcessConfigBacklog(
      CategoryConfiguration systemBacklog,
      CategoryConfiguration customBacklog,
      Action<string> logError,
      Action<string> logWarning = null)
    {
      if (systemBacklog != null && customBacklog != null)
      {
        if (StringComparer.OrdinalIgnoreCase.Equals(systemBacklog.PluralName, customBacklog.PluralName) || logWarning != null && string.IsNullOrWhiteSpace(customBacklog.PluralName))
          return;
        WitProcessComparer.LogErrorOrWarning("Custom processConfiguration category '" + customBacklog.CategoryReferenceName + "' does not match with system PluralName. Expected: '" + systemBacklog.PluralName + "',  Found: '" + customBacklog.PluralName + "'.", "The system value will be used, if this process is otherwise found to be a match.", logError, logWarning);
      }
      else if (systemBacklog == null && customBacklog != null)
      {
        logError("System processConfiguration category not found for matching '" + customBacklog.CategoryReferenceName + "' ");
      }
      else
      {
        if (systemBacklog == null || customBacklog != null)
          return;
        WitProcessComparer.LogErrorOrWarning("Custom processConfiguration category not found for matching '" + systemBacklog.CategoryReferenceName + "'", "This processConfiguration category will be recreated, if this process is otherwise found to be a match.", logError, logWarning);
      }
    }

    private static void CompareAndValidateExternalGlobalLists(
      IReadOnlyCollection<GlobalListDeclaration> globalLists,
      Action<string> logError)
    {
      if (globalLists.Count == 0)
        return;
      logError("Global lists are not supported and cannot be imported. Found in 'WorkItem Tracking\\GlobalList.xml'.");
    }

    private static void LogErrorOrWarning(
      string baseMessage,
      string additionalWarningMessage,
      Action<string> logError,
      Action<string> logWarning = null)
    {
      if (logWarning != null)
        logWarning(baseMessage + " " + additionalWarningMessage);
      else
        logError(baseMessage);
    }

    private static bool IsRuleWhiteListed(
      WorkItemFieldRuleDeclarations fieldRuleDeclaration,
      string workItemType)
    {
      if (fieldRuleDeclaration == null || fieldRuleDeclaration.Field == null || fieldRuleDeclaration.Children == null)
        return false;
      string fieldRefName = fieldRuleDeclaration.Field;
      return fieldRuleDeclaration.Children.TrueForAll((Predicate<WorkItemRuleDeclaration>) (rule => WitProcessComparer.IsRuleWhiteListed(rule, fieldRefName, workItemType)));
    }

    private static bool IsRuleWhiteListed(
      WorkItemRuleDeclaration rule,
      string fieldRefName,
      string workItemType)
    {
      return workItemType != null && WitProcessComparer.s_specificFieldRuleWhiteListForWorkItem.ContainsKey(workItemType) && fieldRefName != null && WitProcessComparer.s_specificFieldRuleWhiteListForWorkItem[workItemType].ContainsKey(fieldRefName) && rule != null && WitProcessComparer.s_specificFieldRuleWhiteListForWorkItem[workItemType][fieldRefName].Contains(rule.Name);
    }
  }
}
