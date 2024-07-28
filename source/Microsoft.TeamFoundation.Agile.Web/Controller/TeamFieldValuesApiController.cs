// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.TeamFieldValuesApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.Work.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "teamfieldvalues")]
  public class TeamFieldValuesApiController : TeamSettingsApiControllerBase
  {
    [HttpGet]
    [ClientExample("GET__work_teamsettings_teamfieldvalues.json", "Get team field values", null, null)]
    public TeamFieldValues GetTeamFieldValues()
    {
      this.TfsRequestContext.TraceEnter(290131, "AgileService", "AgileService", nameof (GetTeamFieldValues));
      try
      {
        return this.GetTeamFieldValuesInternal();
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290132, "AgileService", "AgileService", nameof (GetTeamFieldValues));
      }
    }

    [HttpPatch]
    [ClientExample("PATCH__work_teamsettings_teamfieldvalues.json", "Update team field values. Example 1", null, null)]
    [ClientExample("PATCH__work_teamsettings_teamfieldvalues2.json", "Update team field values. Example 2", null, null)]
    public TeamFieldValues UpdateTeamFieldValues([FromBody] TeamFieldValuesPatch patch)
    {
      this.TfsRequestContext.TraceEnter(290133, "AgileService", "AgileService", nameof (UpdateTeamFieldValues));
      try
      {
        if (patch == null)
          throw new TeamFieldArgumentNullException(nameof (patch));
        if (patch.DefaultValue == null)
          throw new TeamFieldArgumentNullException("DefaultValue");
        if (patch.Values == null)
          throw new TeamFieldArgumentNullException("Values");
        if (!patch.Values.Any<Microsoft.TeamFoundation.Work.WebApi.TeamFieldValue>())
          throw new NoTeamFieldValuesSelectedException(Microsoft.TeamFoundation.Agile.Server.AgileResources.TeamFieldValuesNoTeamFieldValuesSelected, "Values");
        ITeamConfigurationService service = this.TfsRequestContext.GetService<ITeamConfigurationService>();
        this.GetTeamSettingsInternal(true);
        ITeamFieldValue[] teamFieldValueArray = this.FromDataContract(patch.Values);
        IEnumerable<IGrouping<string, ITeamFieldValue>> source = ((IEnumerable<ITeamFieldValue>) teamFieldValueArray).GroupBy<ITeamFieldValue, string>((Func<ITeamFieldValue, string>) (x => x.Value), (IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
        if (source.Any<IGrouping<string, ITeamFieldValue>>((Func<IGrouping<string, ITeamFieldValue>, bool>) (g => g.Count<ITeamFieldValue>() > 1)))
          throw new DuplicateTeamFieldValuesException(string.Join(",", source.Where<IGrouping<string, ITeamFieldValue>>((Func<IGrouping<string, ITeamFieldValue>, bool>) (g => g.Count<ITeamFieldValue>() > 1)).Select<IGrouping<string, ITeamFieldValue>, string>((Func<IGrouping<string, ITeamFieldValue>, string>) (g => g.First<ITeamFieldValue>().Value))), "Values");
        int index = Array.FindIndex<ITeamFieldValue>(teamFieldValueArray, (Predicate<ITeamFieldValue>) (tfv => TFStringComparer.CssTreePathName.Equals(tfv.Value, patch.DefaultValue)));
        if (index == -1)
          throw new NoDefaultTeamFieldValueException(Microsoft.TeamFoundation.Agile.Server.AgileResources.TeamFieldValuesDefaultValueNotFound, "DefaultValue");
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        WebApiTeam team = this.Team;
        ITeamFieldValue[] fieldValues = teamFieldValueArray;
        int defaultValueIndex = index;
        service.SaveTeamFields(tfsRequestContext, team, fieldValues, defaultValueIndex);
        return this.GetTeamFieldValuesInternal(true);
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290134, "AgileService", "AgileService", nameof (UpdateTeamFieldValues));
      }
    }

    private TeamFieldValues GetTeamFieldValuesInternal(bool bypasscache = false)
    {
      TeamFieldValues fieldValuesInternal = new TeamFieldValues();
      ITeamSettings settingsInternal = this.GetTeamSettingsInternal(bypasscache);
      ProjectProcessConfiguration processSettings = this.GetProcessSettings();
      fieldValuesInternal.Field = new FieldReference()
      {
        ReferenceName = processSettings.TeamField.Name,
        Url = WitUrlHelper.GetFieldUrl(this.TfsRequestContext, processSettings.TeamField.Name)
      };
      int defaultValueIndex = settingsInternal.TeamFieldConfig.DefaultValueIndex;
      if (defaultValueIndex >= 0 && defaultValueIndex < settingsInternal.TeamFieldConfig.TeamFieldValues.Length)
      {
        fieldValuesInternal.DefaultValue = settingsInternal.TeamFieldConfig.TeamFieldValues[defaultValueIndex].Value;
      }
      else
      {
        ITeamFieldValue teamFieldValue = ((IEnumerable<ITeamFieldValue>) settingsInternal.TeamFieldConfig.TeamFieldValues).FirstOrDefault<ITeamFieldValue>();
        fieldValuesInternal.DefaultValue = teamFieldValue == null ? (string) null : teamFieldValue.Value;
      }
      fieldValuesInternal.Values = this.ToDataContract((IEnumerable<ITeamFieldValue>) settingsInternal.TeamFieldConfig.TeamFieldValues);
      string resourceUriString = AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, TeamSettingsApiConstants.TeamFieldValuesLocationId, this.ProjectId, this.TeamId, (object) string.Empty);
      fieldValuesInternal.Url = resourceUriString;
      if (processSettings.IsTeamFieldAreaPath())
        fieldValuesInternal.Links = this.GetReferenceLinks(resourceUriString, TeamSettingsApiControllerBase.CommonUrlLink.TeamSettings | TeamSettingsApiControllerBase.CommonUrlLink.RootClassificationNodeAreas);
      else
        fieldValuesInternal.Links = this.GetReferenceLinks(resourceUriString, TeamSettingsApiControllerBase.CommonUrlLink.TeamSettings);
      return fieldValuesInternal;
    }

    private IEnumerable<Microsoft.TeamFoundation.Work.WebApi.TeamFieldValue> ToDataContract(
      IEnumerable<ITeamFieldValue> values)
    {
      return values.Select<ITeamFieldValue, Microsoft.TeamFoundation.Work.WebApi.TeamFieldValue>((Func<ITeamFieldValue, Microsoft.TeamFoundation.Work.WebApi.TeamFieldValue>) (tfv => new Microsoft.TeamFoundation.Work.WebApi.TeamFieldValue()
      {
        Value = tfv.Value,
        IncludeChildren = tfv.IncludeChildren
      }));
    }

    private ITeamFieldValue[] FromDataContract(IEnumerable<Microsoft.TeamFoundation.Work.WebApi.TeamFieldValue> values) => (ITeamFieldValue[]) values.Select<Microsoft.TeamFoundation.Work.WebApi.TeamFieldValue, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamFieldValue>((Func<Microsoft.TeamFoundation.Work.WebApi.TeamFieldValue, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamFieldValue>) (tfv => new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamFieldValue()
    {
      IncludeChildren = tfv.IncludeChildren,
      Value = tfv.Value
    })).ToArray<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamFieldValue>();
  }
}
