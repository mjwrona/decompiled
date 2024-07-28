// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.ReleaseDefinitionVariableGroupValidations
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public class ReleaseDefinitionVariableGroupValidations
  {
    private readonly Func<IVssRequestContext, Guid, int, ReleaseDefinition> getReleaseDefinition;

    internal ReleaseDefinitionVariableGroupValidations(
      Func<IVssRequestContext, Guid, int, ReleaseDefinition> getReleaseDefinition)
    {
      this.getReleaseDefinition = getReleaseDefinition;
    }

    public ReleaseDefinitionVariableGroupValidations()
      : this(ReleaseDefinitionVariableGroupValidations.\u003C\u003EO.\u003C0\u003E__GetReleaseDefinition ?? (ReleaseDefinitionVariableGroupValidations.\u003C\u003EO.\u003C0\u003E__GetReleaseDefinition = new Func<IVssRequestContext, Guid, int, ReleaseDefinition>(ReleaseDefinitionVariableGroupValidations.GetReleaseDefinition)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    private static ReleaseDefinition GetReleaseDefinition(
      IVssRequestContext context,
      Guid projectId,
      int id)
    {
      return context.GetService<ReleaseDefinitionsService>().GetReleaseDefinition(context, projectId, id);
    }

    public void ValidateVariableGroups(
      IVssRequestContext context,
      Guid projectId,
      ReleaseDefinition releaseDefinition,
      bool isUpdate)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (releaseDefinition == null)
        throw new ArgumentNullException(nameof (releaseDefinition));
      if (releaseDefinition.VariableGroups != null && releaseDefinition.VariableGroups.Count > 0 && releaseDefinition.VariableGroups.Count != releaseDefinition.VariableGroups.Distinct<int>().Count<int>())
        throw new InvalidRequestException(Resources.ReleaseDefinitionHasDuplicateVariableGroupsError);
      List<int> intList1 = new List<int>();
      List<int> intList2 = new List<int>();
      if (releaseDefinition.VariableGroups != null)
        intList1 = releaseDefinition.VariableGroups.ToList<int>();
      if (releaseDefinition.Environments != null)
      {
        intList2.AddRange(releaseDefinition.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (env => env?.VariableGroups != null)).SelectMany<DefinitionEnvironment, int>((Func<DefinitionEnvironment, IEnumerable<int>>) (env => (IEnumerable<int>) env.VariableGroups)));
        intList2 = intList2.Distinct<int>().ToList<int>();
      }
      IList<VariableGroup> variableGroups1 = context.GetService<IVariableGroupService>().GetVariableGroups(context.Elevate(), projectId, string.Empty, top: -1);
      ReleaseDefinitionVariableGroupValidations.CheckVariableGroupsLinkedToBothReleaseDefinitionAndEnvironments(variableGroups1, (IList<int>) intList1, (IList<int>) intList2);
      List<int> variableGroups2 = new List<int>();
      variableGroups2.AddRange((IEnumerable<int>) intList1);
      variableGroups2.AddRange((IEnumerable<int>) intList2);
      ReleaseDefinitionVariableGroupValidations.CheckVariableGroupsExist(variableGroups1, (IList<int>) variableGroups2);
      this.CheckUsePermissionOnNewlyUsedVariableGroups(context, projectId, variableGroups1, releaseDefinition, isUpdate);
    }

    private static void CheckVariableGroupsLinkedToBothReleaseDefinitionAndEnvironments(
      IList<VariableGroup> allVariableGroups,
      IList<int> releaseDefinitionVariableGroups,
      IList<int> environmentVariableGroups)
    {
      if (!releaseDefinitionVariableGroups.Any<int>() || !environmentVariableGroups.Any<int>())
        return;
      foreach (int environmentVariableGroup in (IEnumerable<int>) environmentVariableGroups)
      {
        int variableGroupId = environmentVariableGroup;
        if (releaseDefinitionVariableGroups.Contains(variableGroupId))
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.VariableGroupLinkedToBothReleaseAndEnvironmentDefinitions, (object) allVariableGroups.FirstOrDefault<VariableGroup>((Func<VariableGroup, bool>) (vg => vg.Id == variableGroupId))?.Name, (object) variableGroupId));
      }
    }

    private static void CheckVariableGroupsExist(
      IList<VariableGroup> allVariableGroups,
      IList<int> variableGroups)
    {
      if (variableGroups.IsNullOrEmpty<int>())
        return;
      IEnumerable<int> ints = variableGroups.Where<int>((Func<int, bool>) (id => allVariableGroups.All<VariableGroup>((Func<VariableGroup, bool>) (vg => vg.Id != id)))).Select<int, int>((Func<int, int>) (id => id));
      if (!ints.IsNullOrEmpty<int>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDefinitionLinkedVariableGroupsDoesNotExist, (object) string.Join<int>(", ", ints)));
    }

    private static void CheckDuplicateVariableGroupsLinkedToAnyEnvironment(
      List<Tuple<int, string, IList<int>>> filteredEnvironmentVariableGroups)
    {
      foreach (Tuple<int, string, IList<int>> environmentVariableGroup in filteredEnvironmentVariableGroups)
      {
        if (environmentVariableGroup.Item3.Count > 0 && environmentVariableGroup.Item3.Count != environmentVariableGroup.Item3.Distinct<int>().Count<int>())
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseDefinitionEnvironmentHasDuplicateVariableGroupsError, (object) environmentVariableGroup.Item2));
      }
    }

    private void CheckUsePermissionOnNewlyUsedVariableGroups(
      IVssRequestContext context,
      Guid projectId,
      IList<VariableGroup> allVariableGroups,
      ReleaseDefinition releaseDefinition,
      bool isUpdate)
    {
      List<int> filteredReleaseDefinitionVariableGroups;
      List<Tuple<int, string, IList<int>>> filteredEnvironmentVariableGroups;
      this.FetchFilteredVariableGroupsForPermissionCheck(context, projectId, releaseDefinition, isUpdate, out filteredReleaseDefinitionVariableGroups, out filteredEnvironmentVariableGroups);
      if (!filteredReleaseDefinitionVariableGroups.Any<int>() && !filteredEnvironmentVariableGroups.Any<Tuple<int, string, IList<int>>>())
        return;
      IList<VariableGroup> variableGroups = context.GetService<IVariableGroupService>().GetVariableGroups(context, projectId, string.Empty, VariableGroupActionFilter.Use, top: -1);
      foreach (int num in filteredReleaseDefinitionVariableGroups)
      {
        int variableGroupId = num;
        if (variableGroups.All<VariableGroup>((Func<VariableGroup, bool>) (group => group.Id != variableGroupId)))
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UserNotHavingUsePermissionOnReleaseDefinitionVariableGroup, (object) allVariableGroups.FirstOrDefault<VariableGroup>((Func<VariableGroup, bool>) (vg => vg.Id == variableGroupId))?.Name, (object) variableGroupId));
      }
      foreach (Tuple<int, string, IList<int>> tuple in filteredEnvironmentVariableGroups)
      {
        foreach (int num in (IEnumerable<int>) tuple.Item3)
        {
          int variableGroupId = num;
          if (variableGroups.All<VariableGroup>((Func<VariableGroup, bool>) (variableGroup => variableGroup.Id != variableGroupId)))
            throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.UserNotHavingUsePermissionOnEnvironmentVariableGroup, (object) allVariableGroups.FirstOrDefault<VariableGroup>((Func<VariableGroup, bool>) (vg => vg.Id == variableGroupId))?.Name, (object) variableGroupId, (object) tuple.Item2));
        }
      }
    }

    private void FetchFilteredVariableGroupsForPermissionCheck(
      IVssRequestContext context,
      Guid projectId,
      ReleaseDefinition releaseDefinition,
      bool isUpdate,
      out List<int> filteredReleaseDefinitionVariableGroups,
      out List<Tuple<int, string, IList<int>>> filteredEnvironmentVariableGroups)
    {
      filteredReleaseDefinitionVariableGroups = new List<int>();
      filteredEnvironmentVariableGroups = new List<Tuple<int, string, IList<int>>>();
      if (releaseDefinition.VariableGroups != null && releaseDefinition.VariableGroups.Count > 0)
        filteredReleaseDefinitionVariableGroups.AddRange((IEnumerable<int>) releaseDefinition.VariableGroups);
      if (releaseDefinition.Environments != null)
        filteredEnvironmentVariableGroups.AddRange(releaseDefinition.Environments.Where<DefinitionEnvironment>((Func<DefinitionEnvironment, bool>) (env =>
        {
          if (env == null)
            return false;
          int? count = env.VariableGroups?.Count;
          int num = 0;
          return count.GetValueOrDefault() > num & count.HasValue;
        })).Select<DefinitionEnvironment, Tuple<int, string, IList<int>>>((Func<DefinitionEnvironment, Tuple<int, string, IList<int>>>) (env => new Tuple<int, string, IList<int>>(env.Id, env.Name, (IList<int>) env.VariableGroups.ToList<int>()))));
      ReleaseDefinitionVariableGroupValidations.CheckDuplicateVariableGroupsLinkedToAnyEnvironment(filteredEnvironmentVariableGroups);
      if (!filteredReleaseDefinitionVariableGroups.Any<int>() && !filteredEnvironmentVariableGroups.Any<Tuple<int, string, IList<int>>>() || !isUpdate)
        return;
      ReleaseDefinition releaseDefinition1 = this.getReleaseDefinition(context, projectId, releaseDefinition.Id);
      IList<int> variableGroups = releaseDefinition1.VariableGroups;
      filteredReleaseDefinitionVariableGroups = filteredReleaseDefinitionVariableGroups.Except<int>((IEnumerable<int>) variableGroups).ToList<int>();
      foreach (DefinitionEnvironment environment in (IEnumerable<DefinitionEnvironment>) releaseDefinition1.Environments)
      {
        DefinitionEnvironment existingEnvironment = environment;
        Tuple<int, string, IList<int>> tuple = filteredEnvironmentVariableGroups.FirstOrDefault<Tuple<int, string, IList<int>>>((Func<Tuple<int, string, IList<int>>, bool>) (env => env.Item1 == existingEnvironment.Id));
        if (tuple != null)
        {
          foreach (int variableGroup in (IEnumerable<int>) existingEnvironment.VariableGroups)
            tuple.Item3.Remove(variableGroup);
        }
      }
    }
  }
}
