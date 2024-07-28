// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.CommonProjectConfiguration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SettingsPropertyName("TFS.WorkItemTracking.CommonPorjectConfiguration")]
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  public class CommonProjectConfiguration : ISupportValidation
  {
    public const string SettingsPropertyName = "TFS.WorkItemTracking.CommonPorjectConfiguration";

    public CommonProjectConfiguration()
    {
      this.TypeFields = Array.Empty<TypeField>();
      this.Weekends = Array.Empty<DayOfWeek>();
      this.RequirementWorkItems = new WorkItemCategory();
      this.TaskWorkItems = new WorkItemCategory();
    }

    public TypeField[] TypeFields { get; set; }

    public WorkItemCategory RequirementWorkItems { get; set; }

    [XmlElement("TaskWorkItems")]
    public WorkItemCategory TaskWorkItems { get; set; }

    [XmlElement("FeedbackRequestWorkItems")]
    public WorkItemCategory FeedbackRequestWorkItems { get; set; }

    [XmlElement("FeedbackResponseWorkItems")]
    public WorkItemCategory FeedbackResponseWorkItems { get; set; }

    [XmlElement("FeedbackWorkItems")]
    public WorkItemCategory FeedbackWorkItems { get; set; }

    [XmlElement("BugWorkItems")]
    public WorkItemCategory BugWorkItems { get; set; }

    public IEnumerable<WorkItemCategory> GetCategories() => (IEnumerable<WorkItemCategory>) ((IEnumerable<WorkItemCategory>) new WorkItemCategory[6]
    {
      this.RequirementWorkItems,
      this.BugWorkItems,
      this.TaskWorkItems,
      this.FeedbackWorkItems,
      this.FeedbackResponseWorkItems,
      this.FeedbackRequestWorkItems
    }).Where<WorkItemCategory>((Func<WorkItemCategory, bool>) (c => c != null)).ToArray<WorkItemCategory>();

    public virtual DayOfWeek[] Weekends { get; set; }

    [XmlIgnore]
    public virtual TypeField TeamField
    {
      get
      {
        TypeField teamField = this.GetField(FieldTypeEnum.Team);
        if (teamField == null)
        {
          TypeField typeField = new TypeField();
          typeField.Name = "System.AreaPath";
          typeField.Type = FieldTypeEnum.Team;
          teamField = typeField;
        }
        return teamField;
      }
    }

    [XmlIgnore]
    public TypeField EffortField => this.GetField(FieldTypeEnum.Effort);

    [XmlIgnore]
    public TypeField ClosedDateField => this.GetField(FieldTypeEnum.ClosedDate);

    [XmlIgnore]
    public TypeField OrderByField => this.GetField(FieldTypeEnum.Order);

    [XmlIgnore]
    public TypeField RemainingWorkField => this.GetField(FieldTypeEnum.RemainingWork);

    [XmlIgnore]
    public TypeField ActivityField => this.GetField(FieldTypeEnum.Activity);

    [XmlIgnore]
    public TypeField RequestorField => this.GetField(FieldTypeEnum.Requestor);

    [XmlIgnore]
    public TypeField ApplicationTypeField => this.GetField(FieldTypeEnum.ApplicationType);

    [XmlIgnore]
    public TypeField ApplicationStartInformation => this.GetField(FieldTypeEnum.ApplicationStartInformation);

    [XmlIgnore]
    public TypeField ApplicationLaunchInstructions => this.GetField(FieldTypeEnum.ApplicationLaunchInstructions);

    [XmlIgnore]
    public TypeField FeedbackNotes => this.GetField(FieldTypeEnum.FeedbackNotes);

    public virtual bool IsTeamFieldAreaPath() => TFStringComparer.WorkItemFieldReferenceName.Equals("System.AreaPath", this.TeamField.Name);

    public TypeField GetField(FieldTypeEnum type) => ((IEnumerable<TypeField>) this.TypeFields).Where<TypeField>((Func<TypeField, bool>) (c => c.Type == type)).FirstOrDefault<TypeField>();

    public string GetFieldName(FieldTypeEnum type)
    {
      TypeField field = this.GetField(type);
      return field == null ? string.Empty : field.Name;
    }

    internal bool IsDefault => (this.TypeFields == null || this.TypeFields.Length == 0) && (this.Weekends == null || this.Weekends.Length == 0) && (this.RequirementWorkItems == null || string.IsNullOrEmpty(this.RequirementWorkItems.CategoryName)) && (this.TaskWorkItems == null || string.IsNullOrEmpty(this.TaskWorkItems.CategoryName)) && (this.FeedbackRequestWorkItems == null || string.IsNullOrEmpty(this.FeedbackRequestWorkItems.CategoryName)) && (this.FeedbackResponseWorkItems == null || string.IsNullOrEmpty(this.FeedbackResponseWorkItems.CategoryName)) && (this.FeedbackWorkItems == null || string.IsNullOrEmpty(this.FeedbackWorkItems.CategoryName)) && (this.BugWorkItems == null || string.IsNullOrEmpty(this.BugWorkItems.CategoryName)) && (this.ApplicationTypeField == null || this.ApplicationTypeField.TypeFieldValues == null || this.ApplicationTypeField.TypeFieldValues.Length == 0) && this.ApplicationLaunchInstructions == null && this.ApplicationStartInformation == null;

    public void Validate(
      IVssRequestContext requestContext,
      string projectUri,
      bool correctWarnings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
      new CommonSettingsValidator(this).Validate(requestContext, projectUri, correctWarnings);
    }

    public void Validate(
      IVssRequestContext requestContext,
      string projectUri,
      bool correctWarnings,
      CommonProjectConfiguration.OptionalFeatures featureToValidate = CommonProjectConfiguration.OptionalFeatures.None)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
      new CommonSettingsValidator(this).Validate(requestContext, projectUri, correctWarnings, featureToValidate);
    }

    public void ValidateStructure() => new CommonSettingsValidator(this).ValidateBasicStructure();

    [Flags]
    public enum OptionalFeatures
    {
      None = 0,
      Feedback = 1,
      MyWork = 2,
      All = 65535, // 0x0000FFFF
    }
  }
}
