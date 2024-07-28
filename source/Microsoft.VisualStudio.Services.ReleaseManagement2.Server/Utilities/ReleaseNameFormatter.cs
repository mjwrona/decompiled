// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ReleaseNameFormatter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Required for unit testing")]
  public static class ReleaseNameFormatter
  {
    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Required for unit testing")]
    private static Func<DateTime> dateTimeGetter = (Func<DateTime>) (() => DateTime.UtcNow);

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Required for unit testing")]
    static ReleaseNameFormatter() => ReleaseNameFormatter.Initialize((Func<DateTime>) (() => DateTime.UtcNow));

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Required for unit testing")]
    public static void Initialize(Func<DateTime> dateTimeGetterParameter) => ReleaseNameFormatter.dateTimeGetter = dateTimeGetterParameter;

    public static void FillReleaseNameWhileStartingRelease(
      Release release,
      IVssRequestContext requestContext,
      string projectName)
    {
      if (release == null)
        return;
      string releaseDefinitionName = release.ReleaseDefinitionName;
      int artifactSourceId;
      release.TryGetPrimaryArtifactSource(out artifactSourceId);
      string nameFormatMask = string.IsNullOrEmpty(release.ReleaseNameFormat) ? release.Name : release.ReleaseNameFormat;
      release.Name = ReleaseNameFormatter.ExpandReleaseName(release, nameFormatMask, releaseDefinitionName, artifactSourceId, requestContext, projectName);
    }

    public static void FillReleaseNameWhileCreatingRelease(
      Release release,
      ReleaseDefinition definition,
      IVssRequestContext requestContext,
      string projectName)
    {
      if (release == null || definition == null)
        return;
      string name = definition.Name;
      int artifactSourceId;
      definition.TryGetPrimaryArtifactSource(requestContext, out artifactSourceId);
      release.Name = ReleaseNameFormatter.ExpandReleaseName(release, ReleaseNameFormatter.GetFormatMaskBasedOnStatus(release), name, artifactSourceId, requestContext, projectName);
      if (release.Status != ReleaseStatus.Draft)
        return;
      release.PartiallyExpandedReleaseNameFormat = ReleaseNameFormatter.ExpandReleaseName(release, release.ReleaseNameFormat, name, artifactSourceId, requestContext, projectName);
    }

    private static string ExpandReleaseName(
      Release release,
      string nameFormatMask,
      string definitionName,
      int primaryArtifactSourceId,
      IVssRequestContext requestContext,
      string projectName)
    {
      if (release == null)
        return string.Empty;
      TimeZoneInfo locationTimeZone = ReleaseNameFormatter.GetCurrentLocationTimeZone(requestContext);
      DateTime currentDateTime = TimeZoneInfo.ConvertTimeFromUtc(ReleaseNameFormatter.dateTimeGetter(), locationTimeZone);
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      dictionary["Release.DefinitionName"] = definitionName;
      dictionary["System.TeamProject"] = projectName;
      dictionary["Release.ReleaseDescription"] = release.Description.Length > 50 ? release.Description.Substring(0, 50) + "..." : release.Description;
      dictionary["DayOfMonth"] = currentDateTime.Day.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      dictionary["DayOfYear"] = currentDateTime.DayOfYear.ToString((IFormatProvider) CultureInfo.InvariantCulture).PadLeft(3, '0');
      dictionary["Month"] = currentDateTime.Month.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      dictionary["Year:yy"] = currentDateTime.ToString("yy", (IFormatProvider) CultureInfo.InvariantCulture);
      dictionary["Year:yyyy"] = currentDateTime.ToString("yyyy", (IFormatProvider) CultureInfo.InvariantCulture);
      dictionary["Hours"] = currentDateTime.Hour.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      dictionary["Minutes"] = currentDateTime.Minute.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      dictionary["Seconds"] = currentDateTime.Second.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      ReleaseNameFormatter.PopulateTokenValuesFromCustomVariables(dictionary, release);
      ReleaseNameFormatter.PopulateTokenValuesFromArtifacts(release, primaryArtifactSourceId, dictionary, requestContext);
      string releaseName = BuildCommonUtil.ExpandEnvironmentVariables(nameFormatMask, (IDictionary<string, string>) dictionary, (Func<string, string, string>) ((matchingTokenName, expandedValue) => ReleaseNameFormatter.ProcessTokenExpansion(matchingTokenName, expandedValue, currentDateTime)));
      ReleaseNameFormatter.ValidateReleaseName(releaseName);
      return releaseName;
    }

    private static string GetFormatMaskBasedOnStatus(Release release) => release.Status == ReleaseStatus.Draft ? "Draft-$(Rev:r)" : release.ReleaseNameFormat;

    private static void ValidateReleaseName(string releaseName)
    {
      if (!string.IsNullOrEmpty(releaseName) && releaseName.IndexOfAny(ReleaseNameFormatTokensConstants.ReleaseNameFormatInvalidCharacters.ToArray<char>()) >= 0)
        throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidReleaseName, (object) releaseName));
    }

    private static void PopulateTokenValuesFromArtifacts(
      Release release,
      int primaryArtifactSourceId,
      Dictionary<string, string> releaseNameFormatTokens,
      IVssRequestContext requestContext)
    {
      ReleaseNameFormatter.PopulateArtifactTokenValues(primaryArtifactSourceId != -1 ? release.GetArtifact(primaryArtifactSourceId) : ReleaseNameFormatter.CreateNullArtifact(), releaseNameFormatTokens, requestContext);
    }

    private static void PopulateTokenValuesFromCustomVariables(
      Dictionary<string, string> tokens,
      Release release)
    {
      foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) release.Variables)
      {
        if (!variable.Value.IsSecret)
          tokens.TryAdd<string, string>(variable.Key, variable.Value.Value);
      }
      foreach (VariableGroup variableGroup in (IEnumerable<VariableGroup>) release.VariableGroups)
      {
        foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) variableGroup.Variables)
        {
          if (!variable.Value.IsSecret)
            tokens.TryAdd<string, string>(variable.Key, variable.Value.Value);
        }
      }
    }

    private static void PopulateArtifactTokenValues(
      ArtifactSource artifactSource,
      Dictionary<string, string> tokens,
      IVssRequestContext requestContext)
    {
      Dictionary<string, InputValue> sourceData = artifactSource.SourceData;
      IDictionary<string, string> artifactInstance = requestContext.GetService<ArtifactTypeServiceBase>().GetArtifactType(requestContext, artifactSource.ArtifactTypeId).GetFormatMaskTokensFromReleaseArtifactInstance((IDictionary<string, InputValue>) sourceData);
      string sourceBranchName = artifactSource.GetSourceBranchName();
      if (!sourceBranchName.IsNullOrEmpty<char>())
      {
        artifactInstance["Build.SourceBranch"] = ArtifactTypeBase.GetBranchName(sourceBranchName);
        artifactInstance["Build.SourceBranchFullName"] = ReleaseNameFormatter.GetSourceBranchFullName(sourceBranchName);
      }
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) artifactInstance)
        tokens[keyValuePair.Key] = keyValuePair.Value;
    }

    private static string GetSourceBranchFullName(string sourceBranchName)
    {
      if (string.IsNullOrEmpty(sourceBranchName))
        return sourceBranchName;
      sourceBranchName = sourceBranchName.Replace("/", "_");
      return sourceBranchName;
    }

    private static string ProcessTokenExpansion(
      string matchingTokenName,
      string expandedValue,
      DateTime curreDateTime)
    {
      if (expandedValue != null)
        return expandedValue;
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$({0})", (object) "Release.ReleaseId");
      if (matchingTokenName.Equals(str, StringComparison.OrdinalIgnoreCase))
        return "$$RELEASEID$$";
      if (ReleaseNameFormatter.TryExpandDateTimeToken(matchingTokenName, curreDateTime, out expandedValue))
        return expandedValue;
      ReleaseNameFormatter.TryExpandRevisionToken(matchingTokenName, out expandedValue);
      return expandedValue;
    }

    private static bool TryExpandRevisionToken(string matchingTokenName, out string expandedValue)
    {
      expandedValue = (string) null;
      string variableFormat;
      if (!ReleaseNameFormatter.TryGetVariableFormat(matchingTokenName, "$(rev:", out string _, out variableFormat))
        return false;
      int num1 = variableFormat.IndexOf("r", StringComparison.OrdinalIgnoreCase);
      if (num1 < 0)
        return false;
      string str = variableFormat.Substring(0, num1);
      int num2 = Math.Min(variableFormat.Substring(num1).Length, 9);
      expandedValue = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}$$REV{1}$$", (object) str, (object) num2);
      return true;
    }

    private static bool TryExpandDateTimeToken(
      string matchingTokenName,
      DateTime currentDateTime,
      out string expandedValue)
    {
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "$({0}", (object) "Date:");
      expandedValue = (string) null;
      if (!matchingTokenName.StartsWith(str, StringComparison.OrdinalIgnoreCase) || !matchingTokenName.EndsWith(")", StringComparison.OrdinalIgnoreCase))
        return false;
      string format = matchingTokenName.Substring(str.Length, matchingTokenName.Length - (str.Length + ")".Length));
      if (string.IsNullOrWhiteSpace(format))
        return false;
      expandedValue = currentDateTime.ToString(format, (IFormatProvider) DateTimeFormatInfo.InvariantInfo);
      return true;
    }

    private static ArtifactSource CreateNullArtifact() => new ArtifactSource()
    {
      ArtifactTypeId = "nullArtifact"
    };

    private static bool TryGetVariableFormat(
      string searchString,
      string variablePrefix,
      out string fullToken,
      out string variableFormat)
    {
      int startIndex = searchString.IndexOf(variablePrefix, StringComparison.OrdinalIgnoreCase);
      if (startIndex >= 0)
      {
        int num = searchString.IndexOf(')', startIndex);
        if (num >= 0)
        {
          fullToken = searchString.Substring(startIndex, num - startIndex + 1);
          variableFormat = searchString.Substring(startIndex + variablePrefix.Length, num - (startIndex + variablePrefix.Length));
          return true;
        }
      }
      fullToken = (string) null;
      variableFormat = (string) null;
      return false;
    }

    private static TimeZoneInfo GetCurrentLocationTimeZone(IVssRequestContext requestContext) => requestContext.GetService<ICollectionPreferencesService>().GetCollectionTimeZone(requestContext) ?? (requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? TimeZoneInfo.Local : TimeZoneInfo.Utc);
  }
}
