// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionUtil
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  public static class ExtensionUtil
  {
    private static VssJsonMediaTypeFormatter s_formatter = new VssJsonMediaTypeFormatter();
    private static JsonSerializer s_serializer = ExtensionUtil.s_formatter.CreateJsonSerializer();
    private const string s_restrictedPropertyPrefix = "::";
    private const string s_requiredRestrictedToSeparator = "+";
    private const string s_excludeRestrictedToSeparator = "-";
    private const string s_extensionLicensedFilterName = "ExtensionLicensed";

    public static Dictionary<string, object> GetExtensionProperties(
      PublishedExtensionFlags extensionFlags)
    {
      Dictionary<string, object> extensionProperties = new Dictionary<string, object>();
      int num = 0;
      if (extensionFlags.HasFlag((Enum) PublishedExtensionFlags.BuiltIn))
        num |= 1;
      if (extensionFlags.HasFlag((Enum) PublishedExtensionFlags.MultiVersion))
        num |= 2;
      if (extensionFlags.HasFlag((Enum) PublishedExtensionFlags.Paid))
        num |= 4;
      if (extensionFlags.HasFlag((Enum) PublishedExtensionFlags.Preview))
        num |= 8;
      if (extensionFlags.HasFlag((Enum) PublishedExtensionFlags.Public))
        num |= 16;
      if (extensionFlags.HasFlag((Enum) PublishedExtensionFlags.System))
        num |= 32;
      if (extensionFlags.HasFlag((Enum) PublishedExtensionFlags.Trusted))
        num |= 64;
      extensionProperties["::Attributes"] = (object) num;
      return extensionProperties;
    }

    public static ExtensionManifest LoadManifest(
      string publisherName,
      string extensionName,
      string version,
      Stream manifestStream,
      IDictionary<string, object> extraProperties = null,
      bool addLicensingConstraints = false)
    {
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      ExtensionManifest extensionManifest;
      using (StreamReader reader1 = new StreamReader(manifestStream))
      {
        using (JsonReader reader2 = (JsonReader) new JsonTextReader((TextReader) reader1))
          extensionManifest = ExtensionUtil.s_serializer.Deserialize<ExtensionManifest>(reader2);
      }
      if (extensionManifest == null)
        throw new ValueCannotBeNullException("extensionManifest");
      if (extensionManifest.ContributionTypes != null)
      {
        foreach (ContributionType contributionType in extensionManifest.ContributionTypes)
          contributionType.Id = ExtensionUtil.GetFullyQualifiedReference(fullyQualifiedName, contributionType.Id, false);
      }
      if (extensionManifest.Constraints != null)
      {
        foreach (ContributionConstraint constraint in extensionManifest.Constraints)
          constraint.Id = ExtensionUtil.GetFullyQualifiedReference(fullyQualifiedName, constraint.Id, false);
      }
      if (extensionManifest.Contributions != null)
      {
        List<string> stringList1 = extensionManifest.RestrictedTo == null ? new List<string>() : extensionManifest.RestrictedTo.Where<string>((Func<string, bool>) (rv => rv.StartsWith("+"))).Select<string, string>((Func<string, string>) (rv => rv.Substring(1))).ToList<string>();
        IEnumerable<string> validatedValues = ContributionRestriction.GetValidatedValues(ContributionRestriction.GetValidatedValues(extensionManifest.RestrictedTo, ContributionRestriction.GetDefaultValues()).Where<string>((Func<string, bool>) (rv => !rv.StartsWith("+"))), ContributionRestriction.GetDefaultValues());
        List<string> stringList2 = (List<string>) null;
        foreach (Contribution contribution in extensionManifest.Contributions)
        {
          contribution.Id = ExtensionUtil.GetFullyQualifiedReference(fullyQualifiedName, contribution.Id, false);
          if (contribution.Type != null)
            contribution.Type = ExtensionUtil.GetFullyQualifiedReference(fullyQualifiedName, contribution.Type, true);
          ExtensionUtil.FullyQualifyContributionIds(contribution.Targets, fullyQualifiedName);
          ExtensionUtil.FullyQualifyContributionIds(contribution.Includes, fullyQualifiedName);
          if (contribution.Properties != null)
          {
            foreach (string key in (IEnumerable<string>) ((IDictionary<string, JToken>) contribution.Properties).Keys)
            {
              if (key.StartsWith("::"))
              {
                if (stringList2 == null)
                  stringList2 = new List<string>();
                stringList2.Add(key);
              }
            }
            if (stringList2 != null)
            {
              foreach (string propertyName in stringList2)
                contribution.Properties.Remove(propertyName);
              stringList2.Clear();
            }
          }
          else
            contribution.Properties = new JObject();
          if (extraProperties != null)
          {
            foreach (KeyValuePair<string, object> extraProperty in (IEnumerable<KeyValuePair<string, object>>) extraProperties)
              contribution.Properties[extraProperty.Key] = JToken.FromObject(extraProperty.Value);
          }
          if (extensionManifest.BaseUri != null)
          {
            contribution.Properties["::BaseUri"] = (JToken) extensionManifest.BaseUri;
            contribution.Properties.Remove("::FallbackBaseUri");
          }
          if (extensionManifest.FallbackBaseUri != null)
            contribution.Properties["::FallbackBaseUri"] = (JToken) extensionManifest.FallbackBaseUri;
          Guid? serviceInstanceType = extensionManifest.ServiceInstanceType;
          if (serviceInstanceType.HasValue)
          {
            JObject properties = contribution.Properties;
            serviceInstanceType = extensionManifest.ServiceInstanceType;
            JToken jtoken = (JToken) serviceInstanceType.Value;
            properties["::ServiceInstanceType"] = jtoken;
          }
          if (contribution.Constraints != null)
          {
            for (int index = contribution.Constraints.Count - 1; index >= 0; --index)
            {
              ContributionConstraint constraint = contribution.Constraints[index];
              if ("ExtensionLicensed".Equals(constraint.Name, StringComparison.OrdinalIgnoreCase))
                contribution.Constraints.RemoveAt(index);
              else if (!string.IsNullOrEmpty(constraint.Id))
                constraint.Id = ExtensionUtil.GetFullyQualifiedReference(fullyQualifiedName, constraint.Id, true);
            }
          }
          List<string> second = (List<string>) null;
          if (contribution.RestrictedTo != null)
          {
            List<string> list = contribution.RestrictedTo.Where<string>((Func<string, bool>) (rv => rv.StartsWith("-"))).ToList<string>();
            contribution.RestrictedTo = contribution.RestrictedTo.Except<string>((IEnumerable<string>) list);
            second = list.Select<string, string>((Func<string, string>) (rv => rv.Substring(1))).ToList<string>();
          }
          contribution.RestrictedTo = ContributionRestriction.GetValidatedValues(contribution.RestrictedTo, validatedValues);
          if (stringList1.Count<string>() > 0)
          {
            string contributionRequiredValues = string.Join("+", second == null ? (IEnumerable<string>) stringList1 : stringList1.Except<string>((IEnumerable<string>) second));
            if (!string.IsNullOrEmpty(contributionRequiredValues))
            {
              contributionRequiredValues = "+" + contributionRequiredValues;
              contribution.RestrictedTo = contribution.RestrictedTo.Select<string, string>((Func<string, string>) (r => r + contributionRequiredValues));
            }
          }
        }
      }
      if (addLicensingConstraints || extensionManifest.Licensing != null && extensionManifest.Licensing.Overrides != null)
      {
        bool flag = false;
        Dictionary<string, LicensingOverride> dictionary1 = new Dictionary<string, LicensingOverride>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        if (extensionManifest.Licensing != null && extensionManifest.Licensing.Overrides != null)
        {
          foreach (LicensingOverride licensingOverride in extensionManifest.Licensing.Overrides)
          {
            if (!string.IsNullOrEmpty(licensingOverride.Id))
            {
              licensingOverride.Id = ExtensionUtil.GetFullyQualifiedReference(fullyQualifiedName, licensingOverride.Id, false);
              if (!dictionary1.ContainsKey(licensingOverride.Id))
                dictionary1.Add(licensingOverride.Id, licensingOverride);
            }
            else
              flag = true;
          }
          if (dictionary1.Count == 0 && !flag)
            extensionManifest.Licensing.Overrides = (IEnumerable<LicensingOverride>) null;
        }
        JObject jobject = new JObject();
        string str = fullyQualifiedName.ToString();
        jobject["extensionId"] = (JToken) str;
        Dictionary<ContributionLicensingBehaviorType, Dictionary<int, ContributionConstraint>> dictionary2 = new Dictionary<ContributionLicensingBehaviorType, Dictionary<int, ContributionConstraint>>();
        if (extensionManifest.Contributions != null)
        {
          foreach (Contribution contribution in extensionManifest.Contributions)
          {
            LicensingOverride licensingOverride = (LicensingOverride) null;
            dictionary1.TryGetValue(contribution.Id, out licensingOverride);
            ContributionLicensingBehaviorType key1 = ContributionLicensingBehaviorType.OnlyIfLicensed;
            if (licensingOverride != null)
              key1 = licensingOverride.Behavior;
            if (contribution.Constraints == null)
              contribution.Constraints = new List<ContributionConstraint>();
            if (key1 != ContributionLicensingBehaviorType.AlwaysInclude)
            {
              HashSet<int> intSet = new HashSet<int>();
              if (contribution.Constraints.Count == 0)
              {
                intSet.Add(0);
              }
              else
              {
                foreach (ContributionConstraint constraint in contribution.Constraints)
                {
                  int group = constraint != null ? constraint.Group : 0;
                  intSet.Add(group);
                }
              }
              Dictionary<int, ContributionConstraint> dictionary3 = (Dictionary<int, ContributionConstraint>) null;
              if (!dictionary2.TryGetValue(key1, out dictionary3))
              {
                dictionary3 = new Dictionary<int, ContributionConstraint>();
                dictionary2[key1] = dictionary3;
              }
              foreach (int key2 in intSet)
              {
                ContributionConstraint contributionConstraint = (ContributionConstraint) null;
                if (!dictionary3.TryGetValue(key2, out contributionConstraint))
                {
                  contributionConstraint = new ContributionConstraint()
                  {
                    Name = "ExtensionLicensed",
                    Inverse = key1 == ContributionLicensingBehaviorType.OnlyIfUnlicensed,
                    Properties = jobject,
                    Group = key2
                  };
                  dictionary3[key2] = contributionConstraint;
                }
                contribution.Constraints.Add(contributionConstraint);
              }
            }
          }
        }
      }
      return extensionManifest;
    }

    public static void FullyQualifyContributionIds(List<string> list, string extensionIdentifier)
    {
      if (list == null)
        return;
      for (int index = 0; index < list.Count; ++index)
        list[index] = ExtensionUtil.GetFullyQualifiedReference(extensionIdentifier, list[index], true);
    }

    public static string GetFullyQualifiedReference(
      string extensionIdentifier,
      string contributionId,
      bool supportsExternalReference)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(contributionId, nameof (contributionId));
      string qualifiedReference = contributionId;
      if (contributionId.Length <= extensionIdentifier.Length || contributionId[extensionIdentifier.Length] != '.' || !contributionId.StartsWith(extensionIdentifier, StringComparison.OrdinalIgnoreCase))
      {
        if (contributionId[0] == '.')
          qualifiedReference = extensionIdentifier + contributionId;
        else if (!supportsExternalReference)
          qualifiedReference = extensionIdentifier + "." + contributionId;
      }
      return qualifiedReference;
    }
  }
}
