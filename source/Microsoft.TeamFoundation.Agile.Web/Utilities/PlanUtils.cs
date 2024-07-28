// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.PlanUtils
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Web.Data;
using Microsoft.TeamFoundation.Agile.Web.Services;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public static class PlanUtils
  {
    private static IReadOnlyCollection<string> MandatoryCardFields = (IReadOnlyCollection<string>) new ReadOnlyCollection<string>((IList<string>) new List<string>()
    {
      "System.Title"
    });
    internal const string c_fieldReferenceNameKey = "fieldIdentifier";
    internal const string c_displayFormatKey = "displayFormat";
    internal const string c_rowNumberKey = "rowNumber";
    internal const string c_displayTypeKey = "displayType";
    internal const string c_showEmptyFieldsKey = "showEmptyFields";
    internal const string c_showChildRollupKey = "showChildRollup";
    internal const string c_additionalDisplayType = "ADDITIONAL";
    internal static readonly IEnumerable<string> s_defaultCardCoreFields = (IEnumerable<string>) new List<string>()
    {
      "System.AssignedTo",
      "System.State",
      "System.Tags"
    };

    internal static Plan ToPlan(
      this ScaledAgileView view,
      IVssRequestContext requestContext,
      Guid projectId,
      bool includeCardSettings,
      PlanPermissionsBitFlags bitFlagPermissions,
      IdentityRef createdByIdentity,
      IdentityRef modifiedByIdentity)
    {
      if (view == null)
        return (Plan) null;
      return new Plan()
      {
        Id = view.Id,
        Name = view.Name,
        Type = view.Type,
        CreatedDate = view.CreatedDate,
        CreatedByIdentity = createdByIdentity,
        ModifiedDate = view.ModifiedDate,
        ModifiedByIdentity = modifiedByIdentity,
        Description = view.Description,
        UserPermissions = (PlanUserPermissions) bitFlagPermissions,
        Revision = view.Revision,
        LastAccessed = view.LastAccessed,
        Properties = (object) PlanViewDataFactory.GetPlanProperties(requestContext, projectId, view, includeCardSettings)
      };
    }

    internal static Plan ToShallowPlan(
      ShallowScaledAgileViewRecord viewRecord,
      PlanPermissionsBitFlags bitFlagPermissions,
      IdentityRef createdByIdentity,
      IdentityRef modifiedByIdentity)
    {
      return new Plan()
      {
        Id = viewRecord.Id,
        Name = viewRecord.Name,
        Type = (PlanType) viewRecord.Type,
        CreatedDate = viewRecord.CreatedDate,
        CreatedByIdentity = createdByIdentity,
        ModifiedDate = viewRecord.ModifiedDate,
        ModifiedByIdentity = modifiedByIdentity,
        Description = viewRecord.Description,
        UserPermissions = (PlanUserPermissions) bitFlagPermissions,
        Revision = viewRecord.Revision,
        LastAccessed = viewRecord.LastAccessed
      };
    }

    internal static ScaledAgileView ToScaledAgileView(
      this CreatePlan createPlanDefinition,
      IVssRequestContext requestContext)
    {
      return new ScaledAgileView()
      {
        Name = createPlanDefinition.Name,
        Type = createPlanDefinition.Type,
        Configuration = PlanUtils.GetAndValidatePlanConfigurationByType(requestContext, Guid.Empty, createPlanDefinition.Type, createPlanDefinition.Properties),
        Description = createPlanDefinition.Description
      };
    }

    internal static ScaledAgileView ToScaledAgileView(
      this UpdatePlan updatePlanDefinition,
      IVssRequestContext requestContext,
      Guid planId)
    {
      return new ScaledAgileView()
      {
        Revision = updatePlanDefinition.Revision,
        Name = updatePlanDefinition.Name,
        Type = updatePlanDefinition.Type,
        Configuration = PlanUtils.GetAndValidatePlanConfigurationByType(requestContext, planId, updatePlanDefinition.Type, updatePlanDefinition.Properties),
        Description = updatePlanDefinition.Description
      };
    }

    internal static CardSettings GetCardSettings(this UpdatePlan updatePlanDefinition) => updatePlanDefinition.Type == PlanType.DeliveryTimelineView ? DeliveryTimelineDataProvider.ConvertPlanPropertiesToDeliveryViewProperty(updatePlanDefinition.Properties).CardSettings : (CardSettings) null;

    internal static ScaledAgileViewConfiguration GetAndValidatePlanConfigurationByType(
      IVssRequestContext requestContext,
      Guid planId,
      PlanType planType,
      object properties)
    {
      return planType != PlanType.DeliveryTimelineView ? (ScaledAgileViewConfiguration) null : DeliveryTimelineDataProvider.GetAndValidatePlanProperties(requestContext, planId, properties);
    }

    internal static ScaledAgileCardSettings GetCardSettingsByType(
      Guid planId,
      PlanType planType,
      CardSettings cardSettings)
    {
      ScaledAgileCardSettings cardSettingsByType = new ScaledAgileCardSettings(PlanUtils.GetCardSettingsScopeType(planType), planId);
      if (cardSettings != null && cardSettings.Fields != null)
      {
        CardSetting cardFieldSetting = PlanUtils.GetCardFieldSetting(cardSettings.Fields);
        cardSettingsByType.addCard(cardFieldSetting);
      }
      return cardSettingsByType;
    }

    internal static CardSetting GetCardFieldSetting(CardFieldSettings cardFieldSettings)
    {
      ArgumentUtility.CheckForNull<CardFieldSettings>(cardFieldSettings, nameof (cardFieldSettings));
      CardSetting cardFieldSetting = new CardSetting("");
      cardFieldSetting.AddRange(PlanUtils.MandatoryCardFields.Select<string, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting>((Func<string, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting>) (fieldRefName =>
      {
        return new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting()
        {
          {
            "fieldIdentifier",
            fieldRefName
          }
        };
      })));
      if (cardFieldSettings.ShowId)
      {
        CardSetting cardSetting = cardFieldSetting;
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting fieldSetting = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting();
        fieldSetting.Add("fieldIdentifier", "System.Id");
        cardSetting.Add(fieldSetting);
      }
      if (cardFieldSettings.ShowState)
      {
        CardSetting cardSetting = cardFieldSetting;
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting fieldSetting = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting();
        fieldSetting.Add("fieldIdentifier", "System.State");
        cardSetting.Add(fieldSetting);
      }
      if (cardFieldSettings.ShowTags)
      {
        CardSetting cardSetting = cardFieldSetting;
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting fieldSetting = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting();
        fieldSetting.Add("fieldIdentifier", "System.Tags");
        cardSetting.Add(fieldSetting);
      }
      if (cardFieldSettings.ShowParent)
      {
        CardSetting cardSetting = cardFieldSetting;
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting fieldSetting = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting();
        fieldSetting.Add("fieldIdentifier", "System.Parent");
        cardSetting.Add(fieldSetting);
      }
      if (cardFieldSettings.ShowEmptyFields)
      {
        CardSetting cardSetting = cardFieldSetting;
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting fieldSetting = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting();
        fieldSetting.Add("showEmptyFields", bool.TrueString);
        cardSetting.Add(fieldSetting);
      }
      if (cardFieldSettings.ShowChildRollup)
      {
        CardSetting cardSetting = cardFieldSetting;
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting fieldSetting = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting();
        fieldSetting.Add("showChildRollup", bool.TrueString);
        cardSetting.Add(fieldSetting);
      }
      if (cardFieldSettings.ShowAssignedTo)
      {
        CardSetting cardSetting = cardFieldSetting;
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting fieldSetting = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting();
        fieldSetting.Add("fieldIdentifier", "System.AssignedTo");
        fieldSetting.Add("displayFormat", cardFieldSettings.AssignedToDisplayFormat.ToString());
        cardSetting.Add(fieldSetting);
      }
      if (cardFieldSettings.AdditionalFields != null && cardFieldSettings.AdditionalFields.Count<FieldInfo>() > 0)
      {
        IEnumerable<string> source = cardFieldSettings.AdditionalFields.Select<FieldInfo, string>((Func<FieldInfo, string>) (x => x.ReferenceName));
        for (int index = 0; index < source.Count<string>(); ++index)
        {
          string str = source.ElementAt<string>(index);
          if (!string.IsNullOrEmpty(str))
          {
            CardSetting cardSetting = cardFieldSetting;
            Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting fieldSetting = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting();
            fieldSetting.Add("fieldIdentifier", str);
            fieldSetting.Add("rowNumber", index.ToString());
            fieldSetting.Add("displayType", "ADDITIONAL");
            cardSetting.Add(fieldSetting);
          }
        }
      }
      return cardFieldSetting;
    }

    internal static Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings.ScopeType GetCardSettingsScopeType(
      PlanType planType)
    {
      if (planType == PlanType.DeliveryTimelineView)
        return Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings.ScopeType.DELIVERYTIMELINE;
      throw new NotSupportedException();
    }

    internal static ScaledAgileView DTOToScaledAgileView(
      FullScaledAgileViewRecord2 viewRecord,
      IEnumerable<ScaledAgileViewPropertyRecord2> viewProperties)
    {
      return new ScaledAgileView()
      {
        Id = viewRecord.Id,
        OwnerId = viewRecord.OwnerId,
        Name = viewRecord.Name,
        Type = (PlanType) viewRecord.Type,
        CreatedDate = viewRecord.CreatedDate,
        CreatedBy = viewRecord.CreatedBy,
        ModifiedDate = viewRecord.ModifiedDate,
        ModifiedBy = viewRecord.ModifiedBy,
        Configuration = PlanUtils.CreateConfiguration(viewRecord, viewProperties),
        Description = viewRecord.Description,
        Revision = viewRecord.Revision,
        LastAccessed = viewRecord.LastAccessed
      };
    }

    internal static ScaledAgileView DTOToScaledAgileView(
      FullScaledAgileViewRecord viewRecord,
      IEnumerable<ScaledAgileViewPropertyRecord2> viewProperties)
    {
      return new ScaledAgileView()
      {
        Id = viewRecord.Id,
        OwnerId = viewRecord.OwnerId,
        Name = viewRecord.Name,
        Type = (PlanType) viewRecord.Type,
        CreatedDate = viewRecord.CreatedDate,
        CreatedBy = viewRecord.CreatedBy,
        ModifiedDate = viewRecord.ModifiedDate,
        ModifiedBy = viewRecord.ModifiedBy,
        Configuration = PlanUtils.CreateConfiguration(viewRecord, viewProperties),
        Description = viewRecord.Description,
        Revision = viewRecord.Revision
      };
    }

    internal static ScaledAgileView DTOToScaledAgileView(
      ShallowScaledAgileViewRecord viewRecord,
      IEnumerable<ScaledAgileViewPropertyRecord2> viewProperties)
    {
      return new ScaledAgileView()
      {
        Id = viewRecord.Id,
        OwnerId = viewRecord.OwnerId,
        Name = viewRecord.Name,
        Type = (PlanType) viewRecord.Type,
        CreatedDate = viewRecord.CreatedDate,
        CreatedBy = viewRecord.CreatedBy,
        ModifiedDate = viewRecord.ModifiedDate,
        ModifiedBy = viewRecord.ModifiedBy,
        Configuration = PlanUtils.CreateConfiguration(viewRecord, viewProperties),
        Description = viewRecord.Description,
        Revision = viewRecord.Revision
      };
    }

    private static ScaledAgileViewConfiguration CreateConfiguration(
      ShallowScaledAgileViewRecord viewRecord,
      IEnumerable<ScaledAgileViewPropertyRecord2> viewProperties)
    {
      return PlanUtils.DTOToScaledAgileViewConfiguration(viewProperties, (string) null, (string) null);
    }

    private static ScaledAgileViewConfiguration CreateConfiguration(
      FullScaledAgileViewRecord viewRecord,
      IEnumerable<ScaledAgileViewPropertyRecord2> viewProperties)
    {
      return PlanUtils.DTOToScaledAgileViewConfiguration(viewProperties, viewRecord.Criteria, (string) null);
    }

    private static ScaledAgileViewConfiguration CreateConfiguration(
      FullScaledAgileViewRecord2 viewRecord,
      IEnumerable<ScaledAgileViewPropertyRecord2> viewProperties)
    {
      return PlanUtils.DTOToScaledAgileViewConfiguration(viewProperties, viewRecord.Criteria, viewRecord.Markers);
    }

    internal static ScaledAgileView DTOToScaledAgileView(
      ShallowScaledAgileViewRecord viewRecord,
      IEnumerable<ScaledAgileViewPropertyRecord> viewProperties)
    {
      return new ScaledAgileView()
      {
        Id = viewRecord.Id,
        OwnerId = viewRecord.OwnerId,
        Name = viewRecord.Name,
        Type = (PlanType) viewRecord.Type,
        CreatedDate = viewRecord.CreatedDate,
        Configuration = new ScaledAgileViewConfiguration()
        {
          TeamBacklogMappings = (IEnumerable<ScaledAgileViewProperty>) viewProperties.Select<ScaledAgileViewPropertyRecord, ScaledAgileViewProperty>((Func<ScaledAgileViewPropertyRecord, ScaledAgileViewProperty>) (p => new ScaledAgileViewProperty()
          {
            TeamId = p.TeamId,
            CategoryReferenceName = p.WorkItemTypeName
          })).ToList<ScaledAgileViewProperty>()
        }
      };
    }

    internal static ScaledAgileViewConfiguration DTOToScaledAgileViewConfiguration(
      IEnumerable<ScaledAgileViewPropertyRecord2> viewProperties,
      string criteria,
      string markers)
    {
      return new ScaledAgileViewConfiguration()
      {
        TeamBacklogMappings = (IEnumerable<ScaledAgileViewProperty>) viewProperties.Select<ScaledAgileViewPropertyRecord2, ScaledAgileViewProperty>((Func<ScaledAgileViewPropertyRecord2, ScaledAgileViewProperty>) (p => new ScaledAgileViewProperty()
        {
          TeamId = p.TeamId,
          CategoryReferenceName = p.CategoryReferenceName
        })).ToList<ScaledAgileViewProperty>(),
        Criteria = criteria,
        Markers = markers
      };
    }

    internal static ScaledAgileCardSettings DTOToPlansCardFields(
      Guid viewId,
      PlanType type,
      IEnumerable<BoardCardSettingRow> cardSettingRows)
    {
      ScaledAgileCardSettings plansCardFields = (ScaledAgileCardSettings) null;
      if (cardSettingRows != null && cardSettingRows.Any<BoardCardSettingRow>())
      {
        if (type != PlanType.DeliveryTimelineView)
          throw new NotSupportedException();
        plansCardFields = new ScaledAgileCardSettings(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings.ScopeType.DELIVERYTIMELINE, viewId, cardSettingRows);
      }
      return plansCardFields;
    }

    internal static CardSettings ToDefaultCardSettings(
      ScaledAgileCardSettingsValidator cardSettingsValidator)
    {
      CardFieldSettings cardFieldSettings = new CardFieldSettings()
      {
        ShowAssignedTo = true,
        AssignedToDisplayFormat = IdentityDisplayFormat.AvatarAndFullName,
        ShowState = true,
        ShowTags = true
      };
      cardFieldSettings.CoreFields = cardSettingsValidator.GetSanitizedCoreFields(PlanUtils.s_defaultCardCoreFields);
      return new CardSettings()
      {
        Fields = cardFieldSettings
      };
    }

    internal static CardSettings ToPlanCardSettings(
      ScaledAgileCardSettings scaledAgileCardSettings,
      ScaledAgileCardSettingsValidator cardSettingsValidator)
    {
      ArgumentUtility.CheckForNull<ScaledAgileCardSettings>(scaledAgileCardSettings, nameof (scaledAgileCardSettings));
      CardFieldSettings cardFieldSettings = (CardFieldSettings) null;
      List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting> card = scaledAgileCardSettings.Cards?[""];
      if (card != null)
      {
        cardFieldSettings = new CardFieldSettings();
        List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting> source1 = new List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting>();
        List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting> source2 = new List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting>();
        foreach (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting fieldSetting in card)
        {
          if (fieldSetting != null)
          {
            if (!string.IsNullOrEmpty(fieldSetting.FieldIdentifier))
            {
              if (TFStringComparer.WorkItemFieldReferenceName.Equals(fieldSetting.FieldIdentifier, "System.Id"))
              {
                cardFieldSettings.ShowId = true;
                source2.Add(fieldSetting);
              }
              else if (TFStringComparer.WorkItemFieldReferenceName.Equals(fieldSetting.FieldIdentifier, "System.State"))
              {
                cardFieldSettings.ShowState = true;
                source2.Add(fieldSetting);
              }
              else if (TFStringComparer.WorkItemFieldReferenceName.Equals(fieldSetting.FieldIdentifier, "System.Tags"))
              {
                cardFieldSettings.ShowTags = true;
                source2.Add(fieldSetting);
              }
              else if (TFStringComparer.WorkItemFieldReferenceName.Equals(fieldSetting.FieldIdentifier, "System.AssignedTo"))
              {
                string str;
                IdentityDisplayFormat result;
                if (fieldSetting.TryGetValue("displayFormat", out str) && Enum.TryParse<IdentityDisplayFormat>(str, out result))
                {
                  cardFieldSettings.ShowAssignedTo = true;
                  cardFieldSettings.AssignedToDisplayFormat = result;
                  source2.Add(fieldSetting);
                }
              }
              else if (TFStringComparer.WorkItemFieldReferenceName.Equals(fieldSetting.FieldIdentifier, "System.Parent"))
              {
                cardFieldSettings.ShowParent = true;
                source2.Add(fieldSetting);
              }
              else
              {
                string a;
                if (fieldSetting.TryGetValue("displayType", out a) && string.Equals(a, "ADDITIONAL") && fieldSetting.ContainsKey("rowNumber"))
                  source1.Add(fieldSetting);
              }
            }
            else
            {
              string str1;
              bool result1;
              if (fieldSetting.TryGetValue("showEmptyFields", out str1) && bool.TryParse(str1, out result1))
                cardFieldSettings.ShowEmptyFields = result1;
              string str2;
              bool result2;
              if (fieldSetting.TryGetValue("showChildRollup", out str2) && bool.TryParse(str2, out result2))
                cardFieldSettings.ShowChildRollup = result2;
            }
          }
        }
        if (source2.Any<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting>())
          cardFieldSettings.CoreFields = cardSettingsValidator.GetSanitizedCoreFields((IEnumerable<string>) source2.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting, string>) (s => s.FieldIdentifier)).ToList<string>());
        if (source1.Any<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting>())
        {
          List<string> list = source1.OrderBy<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting, int>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting, int>) (fieldSetting => Convert.ToInt32(fieldSetting["rowNumber"]))).Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting, string>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting, string>) (s => s.FieldIdentifier)).ToList<string>();
          cardFieldSettings.AdditionalFields = (IEnumerable<FieldInfo>) cardSettingsValidator.GetSanitizedAdditionalFields((IEnumerable<string>) list).ToList<FieldInfo>();
        }
      }
      return new CardSettings()
      {
        Fields = cardFieldSettings
      };
    }

    public static Microsoft.TeamFoundation.Work.WebApi.Rule ToWebApiRule(CardRule r)
    {
      Microsoft.TeamFoundation.Work.WebApi.Rule webApiRule = new Microsoft.TeamFoundation.Work.WebApi.Rule();
      webApiRule.name = r.Name;
      webApiRule.isEnabled = r.IsEnabled.ToString();
      webApiRule.filter = r.QueryText.IsNullOrEmpty<char>() ? (string) null : r.QueryText;
      webApiRule.settings = new attribute();
      webApiRule.Clauses = (ICollection<Microsoft.TeamFoundation.Work.WebApi.FilterClause>) null;
      if (r.QueryExpression != null)
      {
        webApiRule.Clauses = (ICollection<Microsoft.TeamFoundation.Work.WebApi.FilterClause>) new List<Microsoft.TeamFoundation.Work.WebApi.FilterClause>();
        foreach (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause clause in (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.FilterClause>) r.QueryExpression.Clauses)
        {
          Microsoft.TeamFoundation.Work.WebApi.FilterClause filterClause = new Microsoft.TeamFoundation.Work.WebApi.FilterClause(clause.FieldName, clause.Index, clause.LogicalOperator, clause.Operator, clause.Value);
          webApiRule.Clauses.Add(filterClause);
        }
      }
      if (r.StyleAttributes != null)
      {
        foreach (KeyValuePair<string, string> styleAttribute in r.StyleAttributes)
          webApiRule.settings[styleAttribute.Key] = styleAttribute.Value;
      }
      else
        webApiRule.settings = (attribute) null;
      return webApiRule;
    }

    public static List<string> GetFieldsForStyleRules(DeliveryViewFilter timelineFilter)
    {
      List<string> source = new List<string>();
      foreach (List<Microsoft.TeamFoundation.Work.WebApi.FilterClause> filterClause1 in (IEnumerable<List<Microsoft.TeamFoundation.Work.WebApi.FilterClause>>) timelineFilter.FilterClauses)
      {
        foreach (Microsoft.TeamFoundation.Work.WebApi.FilterClause filterClause2 in filterClause1)
          source.Add(filterClause2.FieldName);
      }
      return source.Distinct<string>().ToList<string>();
    }
  }
}
