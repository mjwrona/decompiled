// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ResourceGovernance.Utils.RUValidationUtils
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud.ResourceGovernance.Utils
{
  public static class RUValidationUtils
  {
    public static bool SetRUThresholdWithValidations(
      IVssRequestContext requestContext,
      string ruleName,
      string entity,
      long? flag,
      long? tarpit,
      long? block,
      double? dpMagnitude,
      string note,
      DateTime? expiration,
      bool? allowMerge,
      bool? checkExpirationOnly,
      out string errorMessage,
      out int affectedRecords)
    {
      errorMessage = (string) null;
      affectedRecords = 0;
      using (ResourceUtilizationComponent component = requestContext.CreateComponent<ResourceUtilizationComponent>())
      {
        bool? nullable = allowMerge;
        bool flag1 = true;
        if (!(nullable.GetValueOrDefault() == flag1 & nullable.HasValue))
        {
          RUThreshold matchingThreshold = RUValidationUtils.FindMatchingThreshold(component, ruleName, entity);
          if (matchingThreshold != null)
          {
            nullable = checkExpirationOnly;
            bool flag2 = true;
            if (nullable.GetValueOrDefault() == flag2 & nullable.HasValue)
            {
              if (RUValidationUtils.DoesThresholdHaveMergeConflict(matchingThreshold, matchingThreshold.Flag, matchingThreshold.Tarpit, matchingThreshold.Block, matchingThreshold.DPMagnitude, matchingThreshold.Note, expiration, out errorMessage))
                return false;
            }
            else if (RUValidationUtils.DoesThresholdHaveMergeConflict(matchingThreshold, flag, tarpit, block, dpMagnitude, note, expiration, out errorMessage))
              return false;
          }
        }
        affectedRecords = component.SetRUThreshold(ruleName, entity, flag, tarpit, block, dpMagnitude, note, expiration);
        return true;
      }
    }

    public static RUThreshold FindMatchingThreshold(
      ResourceUtilizationComponent comp,
      string ruleName,
      string entity)
    {
      List<RUThreshold> thresholds;
      comp.ReadRURulesAndThresholds(out List<RUMacro> _, out thresholds);
      return thresholds.Where<RUThreshold>((Func<RUThreshold, bool>) (threshold => (threshold.Entity ?? string.Empty) == (entity ?? string.Empty) && threshold.RuleName == ruleName)).FirstOrDefault<RUThreshold>();
    }

    public static bool DoesThresholdHaveMergeConflict(
      RUThreshold existingThreshold,
      long? flag,
      long? tarpit,
      long? block,
      double? dpMagnitude,
      string note,
      DateTime? expiration,
      out string errorMessage)
    {
      errorMessage = (string) null;
      if (existingThreshold != null)
      {
        errorMessage = RUValidationUtils.CheckForMergeConflict<long?>("Flag", existingThreshold.Flag, flag) + RUValidationUtils.CheckForMergeConflict<long?>("Tarpit", existingThreshold.Tarpit, tarpit) + RUValidationUtils.CheckForMergeConflict<long?>("Block", existingThreshold.Block, block) + RUValidationUtils.CheckForMergeConflict<double?>("DPMagnitude", existingThreshold.DPMagnitude, dpMagnitude) + RUValidationUtils.CheckForMergeConflict<string>("Note", existingThreshold.Note, note) + RUValidationUtils.CheckForMergeConflict("Expiration", existingThreshold.Expiration, expiration);
        if (!string.IsNullOrEmpty(errorMessage))
        {
          errorMessage = "Some of the values are not explicitly set for the threshold but the threshold with the same key params (entity, rulename, hostid) already exists. If you want to merge the values passed into cmdlet into existing threshold, please add -AllowMerge flag. Otherwise please set params explicitly. " + errorMessage;
          return true;
        }
      }
      return false;
    }

    private static string CheckForMergeConflict(
      string paramName,
      DateTime? existingValue,
      DateTime? newValue)
    {
      if (existingValue.HasValue)
      {
        DateTime? nullable = existingValue;
        DateTime minValue = DateTime.MinValue;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != minValue ? 1 : 0) : 0) : 1) != 0 && !newValue.HasValue)
          return RUValidationUtils.GetConflictErrorMessage<DateTime?>(paramName, existingValue);
      }
      return string.Empty;
    }

    private static string CheckForMergeConflict<T>(string paramName, T existingValue, T newValue) => (object) existingValue != null && (object) newValue == null ? RUValidationUtils.GetConflictErrorMessage<T>(paramName, existingValue) : string.Empty;

    private static string GetConflictErrorMessage<T>(string paramName, T existingValue) => string.Format("{0} parameter is set for existing threshold with value: {1}, but no new explicit value is passed. ", (object) paramName, (object) existingValue);
  }
}
