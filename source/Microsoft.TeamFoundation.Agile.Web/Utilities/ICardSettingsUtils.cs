// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.ICardSettingsUtils
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public interface ICardSettingsUtils
  {
    IDictionary<string, object> GetBoardCardSettingsCIData(
      BoardCardSettings boardCardSettings,
      IAgileSettings agileSettings);

    IEnumerable<string> GetBoardFields(
      IVssRequestContext requestContext,
      BoardCardSettings boardCardSettings);

    IEnumerable<string> GetCardStyleRuleFields(BoardCardSettings boardCardSettings);

    List<string> GetCardsDisplayedFields(BoardCardSettings boardCardSettings);

    IEnumerable<ICardFieldDefinition> GetCardFieldDefinitions(
      IVssRequestContext context,
      IEnumerable<string> fields);

    void InsertRowNumberData(BoardCardSettings data);

    void SortBasedOnRowNumber(BoardCardSettings data);

    void RemoveAllRowNumberKey(BoardCardSettings data);

    HashSet<string> GetCoreFieldIdentifiers(IAgileSettings settings);

    string ValidateCardRules(
      IVssRequestContext context,
      BoardCardRules boardCardRules,
      string type,
      IDictionary<Guid, string> tagNamesReplacedbyId);

    string ValidateAndReconcileBoardCardSettingsForSET(
      IVssRequestContext context,
      BacklogLevelConfiguration backlogLevel,
      IAgileSettings settings,
      BoardCardSettings newSettings,
      Guid teamID);

    void ValidateAndReconcileSettingsForGET(
      IVssRequestContext context,
      BacklogLevelConfiguration backlogLevel,
      IAgileSettings settings,
      BoardCardSettings.ScopeType scope,
      Guid scopeId,
      BoardCardSettings savedSettings);

    void InsertDefaultCards(BoardCardSettings savedSettings, BoardCardSettings defaultSettings);

    void RemoveNonDefaultCards(BoardCardSettings savedSettings, BoardCardSettings defaultSettings);

    BoardCardSettings GetDefaultBoardCardSettings(
      IVssRequestContext context,
      BacklogLevelConfiguration backlogLevel,
      IAgileSettings settings,
      BoardCardSettings.ScopeType scope,
      Guid scopeId);

    FilterModel GetFilterModel(string wiql, IVssRequestContext requestContext);

    void AddPropertiesToField(FieldSetting field);

    List<CardRule> GetBoardCardStyles(IVssRequestContext requestContext);

    void AddWorkItemFields(
      IVssRequestContext context,
      BacklogLevelConfiguration backlogLevel,
      IAgileSettings settings,
      BoardCardSettings.ScopeType scope,
      CardSetting card,
      string workItemType);

    IEnumerable<string> GetBoardWorkItems(
      IVssRequestContext context,
      BacklogLevelConfiguration backlogLevel,
      IAgileSettings settings,
      BoardCardSettings.ScopeType scope);

    void ConvertTagIdToTagName(IVssRequestContext tfsRequestContext, List<CardRule> Rules);

    string TransformWiqlNamesToIds(IVssRequestContext requestContext, string wiql);

    List<CardRule> TransformWiqls(IVssRequestContext requestContext, List<CardRule> rules);
  }
}
