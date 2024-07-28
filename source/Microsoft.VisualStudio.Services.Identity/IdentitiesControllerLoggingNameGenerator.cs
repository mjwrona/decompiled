// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitiesControllerLoggingNameGenerator
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentitiesControllerLoggingNameGenerator
  {
    private const string DescriptorsKey = "descriptors";
    private const string IdentityIdsKey = "identityIds";
    private const string SubjectDescriptorsKey = "subjectDescriptors";
    private const string SearchFilterKey = "searchFilter";
    private const string IdentityIdKey = "identityId";
    private const string QueryMembershipKey = "queryMembership";
    private const string Separator = "_";
    private const string UnknownFlavor = "Unknown";
    private const string ByDescriptorsFlavor = "Sid";
    private const string BySubjectDescriptorsFlavor = "SubjectDescriptor";
    private const string ByIDsFlavor = "Id";
    private const string BySearchFilterFlavor = "Search";
    private const string UnknownSearchFilterType = "Unknown";
    private static readonly Func<string, string> GroupNameLookup = LookupGenerator.CreateLookupWithDefault<string, string>("Group", (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) SidIdentityHelper.WellKnownGroupSidComparer)
    {
      [GroupWellKnownSidConstants.AccountCreatorGroupSid] = "AccountCreatorGroup",
      [GroupWellKnownSidConstants.AnonymousUsersGroupSid] = "AnonymousUsersGroup",
      [GroupWellKnownSidConstants.EveryoneGroupSid] = "EveryoneGroup",
      [GroupWellKnownSidConstants.InstanceAllocatorsGroup] = "InstanceAllocatorsGroup",
      [GroupWellKnownSidConstants.InvitedUsersGroupSid] = "InvitedUsersGroup",
      [GroupWellKnownSidConstants.LicensedUsersGroupSid] = "LicensedUsersGroup",
      [GroupWellKnownSidConstants.LicenseesGroupSid] = "LicenseesGroup",
      [GroupWellKnownSidConstants.NamespaceAdministratorsGroupSid] = "NamespaceAdminGroup",
      [GroupWellKnownSidConstants.UsersGroupSid] = "UsersGroup",
      [GroupWellKnownSidConstants.SecurityServiceGroupSid] = "SecurityServiceGroup",
      [GroupWellKnownSidConstants.ServicePrincipalGroupSid] = "ServicePrincipalGroup",
      [GroupWellKnownSidConstants.ServiceUsersGroupSid] = "ServiceUsersGroup",
      [GroupWellKnownSidConstants.ApplicationPrincipalsGroupSid] = "ApplicationPrincipalsGroup",
      [GroupWellKnownSidConstants.ProjectScopedUsersGroupSid] = "ProjectScopedUsersGroup"
    });
    private static readonly Func<string, Func<string, string>> IdentityTypeHandlerLookup = LookupGenerator.CreateLookupWithDefault<string, Func<string, string>>((Func<string, string>) (_ => "UnrecognizedIdentityType"), (IReadOnlyDictionary<string, Func<string, string>>) new Dictionary<string, Func<string, string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      ["Microsoft.IdentityModel.Claims.ClaimsIdentity"] = new Func<string, string>(IdentitiesControllerLoggingNameGenerator.ClaimsTypeLookup),
      ["Microsoft.TeamFoundation.BindPendingIdentity"] = new Func<string, string>(IdentitiesControllerLoggingNameGenerator.BindPendingTypeLookup),
      ["Microsoft.TeamFoundation.Identity"] = IdentitiesControllerLoggingNameGenerator.GroupNameLookup,
      ["Microsoft.TeamFoundation.ServiceIdentity"] = (Func<string, string>) (_ => "ServiceIdentity"),
      ["Microsoft.VisualStudio.Services.Identity.ServerTestIdentity"] = (Func<string, string>) (_ => "ServerTestIdentity"),
      ["Microsoft.TeamFoundation.UnauthenticatedIdentity"] = (Func<string, string>) (_ => "UnauthenticatedIdentity"),
      ["Microsoft.TeamFoundation.AggregateIdentity"] = (Func<string, string>) (_ => "AggregateIdentity"),
      ["Microsoft.VisualStudio.Services.Identity.UnknownIdentity"] = (Func<string, string>) (_ => "UnknownIdentity"),
      ["System.Security.Principal.WindowsIdentity"] = (Func<string, string>) (_ => "WindowsIdentity"),
      ["Microsoft.TeamFoundation.ImportedIdentity"] = (Func<string, string>) (_ => "ImportedIdentity"),
      ["Microsoft.TeamFoundation.Claims.CspPartnerIdentity"] = (Func<string, string>) (_ => "CspPartnerIdentityType"),
      ["Microsoft.VisualStudio.Services.Claims.AadServicePrincipal"] = (Func<string, string>) (_ => "AADServicePrincipal"),
      ["System:License"] = (Func<string, string>) (_ => "INVALID_SubjectStoreLicense"),
      ["System:ServicePrincipal"] = (Func<string, string>) (_ => "INVALID_SubjectStoreServicePrincipal"),
      ["System:Scope"] = (Func<string, string>) (_ => "INVALID_SubjectStoreScope"),
      ["System:CspPartner"] = (Func<string, string>) (_ => "INVALID_SubjectStoreCspPartner"),
      ["System:PublicAccess"] = (Func<string, string>) (_ => "INVALID_SubjectStorePublicAccess"),
      ["System:WellKnownGroup"] = (Func<string, string>) (_ => "INVALID_SubjectStoreWellKnownGroup")
    });
    private static readonly IReadOnlyCollection<string> ReadMembershipSuffixes = (IReadOnlyCollection<string>) new List<string>()
    {
      "Direct",
      "Expanded",
      "ExpandedDown",
      "ExpandedUp"
    };

    public static string GetReadIdentitiesLoggingName(
      string methodName,
      IReadOnlyDictionary<string, object> actionArguments)
    {
      return new StringBuilder(methodName).AppendReadIdentitiesFlavor(actionArguments).AppendQueryMembership(actionArguments).ToString();
    }

    public static string GetReadIdentityLoggingName(
      string methodName,
      IReadOnlyDictionary<string, object> actionArguments)
    {
      return new StringBuilder(methodName).AppendReadIdentityFlavor(actionArguments).AppendQueryMembership(actionArguments).ToString();
    }

    public static string GetReadIdentityBatchLoggingName(
      string methodName,
      IReadOnlyDictionary<string, string> queryString)
    {
      return new StringBuilder(methodName).AppendReadIdentityBatchFlavor(queryString).AppendQueryMembership(queryString).ToString();
    }

    private static StringBuilder AppendReadIdentitiesFlavor(
      this StringBuilder sb,
      IReadOnlyDictionary<string, object> actionArguments)
    {
      sb.Append("_");
      string castedValueOrDefault1 = actionArguments.GetCastedValueOrDefault<string, string>("descriptors", string.Empty);
      if (!string.IsNullOrWhiteSpace(castedValueOrDefault1))
      {
        int itemCount = IdentitiesControllerLoggingNameGenerator.CountItemsInCommaSeperatedList(castedValueOrDefault1);
        return sb.Append("Sid").Append("_").Append(itemCount == 1 ? IdentitiesControllerLoggingNameGenerator.GetSingleDescriptorInfo(castedValueOrDefault1) : IdentitiesControllerLoggingNameGenerator.GetOrderOfMagnitudeBucket(itemCount));
      }
      string castedValueOrDefault2 = actionArguments.GetCastedValueOrDefault<string, string>("identityIds", string.Empty);
      if (!string.IsNullOrWhiteSpace(castedValueOrDefault2))
      {
        int itemCount = IdentitiesControllerLoggingNameGenerator.CountItemsInCommaSeperatedList(castedValueOrDefault2);
        return sb.Append("Id").Append("_").Append(IdentitiesControllerLoggingNameGenerator.GetOrderOfMagnitudeBucket(itemCount));
      }
      string castedValueOrDefault3 = actionArguments.GetCastedValueOrDefault<string, string>("subjectDescriptors", string.Empty);
      if (!string.IsNullOrWhiteSpace(castedValueOrDefault3))
      {
        int itemCount = IdentitiesControllerLoggingNameGenerator.CountItemsInCommaSeperatedList(castedValueOrDefault3);
        return sb.Append("SubjectDescriptor").Append("_").Append(IdentitiesControllerLoggingNameGenerator.GetOrderOfMagnitudeBucket(itemCount));
      }
      string castedValueOrDefault4 = actionArguments.GetCastedValueOrDefault<string, string>("searchFilter", string.Empty);
      if (string.IsNullOrWhiteSpace(castedValueOrDefault4))
        return sb.Append("Unknown");
      IdentitySearchFilter result;
      string str = Enum.TryParse<IdentitySearchFilter>(castedValueOrDefault4, out result) ? result.ToString() : "Unknown";
      return sb.Append("Search").Append("_").Append(str);
    }

    private static StringBuilder AppendReadIdentityFlavor(
      this StringBuilder sb,
      IReadOnlyDictionary<string, object> actionArguments)
    {
      sb.Append("_");
      string castedValueOrDefault = actionArguments.GetCastedValueOrDefault<string, string>("identityId", string.Empty);
      if (string.IsNullOrWhiteSpace(castedValueOrDefault))
        return sb.Append("Unknown");
      return Guid.TryParse(castedValueOrDefault, out Guid _) ? sb.Append("Id") : sb.Append("Sid").Append("_").Append(IdentitiesControllerLoggingNameGenerator.GetSingleDescriptorInfo(castedValueOrDefault));
    }

    private static StringBuilder AppendReadIdentityBatchFlavor(
      this StringBuilder sb,
      IReadOnlyDictionary<string, string> queryString)
    {
      string valueOrDefault1 = queryString.GetValueOrDefault<string, string>("flavor");
      string valueOrDefault2 = queryString.GetValueOrDefault<string, string>("count");
      int itemCount = 0;
      ref int local = ref itemCount;
      int.TryParse(valueOrDefault2, out local);
      string ofMagnitudeBucket = IdentitiesControllerLoggingNameGenerator.GetOrderOfMagnitudeBucket(itemCount);
      switch (valueOrDefault1)
      {
        case "id":
          return sb.Append("_").Append("Id").Append("_").Append(ofMagnitudeBucket);
        case "descriptor":
          return sb.Append("_").Append("Sid").Append("_").Append(ofMagnitudeBucket);
        case "subjectDescriptor":
          return sb.Append("_").Append("SubjectDescriptor").Append("_").Append(ofMagnitudeBucket);
        default:
          return sb.Append("_").Append("Unknown");
      }
    }

    private static StringBuilder AppendQueryMembership(
      this StringBuilder sb,
      IReadOnlyDictionary<string, string> actionArguments)
    {
      QueryMembership result = QueryMembership.None;
      Enum.TryParse<QueryMembership>(actionArguments.GetValueOrDefault<string, string>("queryMembership"), out result);
      return result != QueryMembership.None ? sb.Append("_").Append((object) result) : sb;
    }

    private static StringBuilder AppendQueryMembership(
      this StringBuilder sb,
      IReadOnlyDictionary<string, object> actionArguments)
    {
      object obj = (object) null;
      QueryMembership queryMembership = QueryMembership.None;
      if (actionArguments.TryGetValue("queryMembership", out obj) && (obj is int || obj is QueryMembership))
        queryMembership = (QueryMembership) obj;
      return queryMembership != QueryMembership.None ? sb.Append("_").Append((object) queryMembership) : sb;
    }

    private static int CountItemsInCommaSeperatedList(string commaSeperatedList) => !string.IsNullOrWhiteSpace(commaSeperatedList) ? commaSeperatedList.Count<char>((Func<char, bool>) (c => c == ',')) + 1 : 0;

    private static string GetOrderOfMagnitudeBucket(int itemCount)
    {
      if (itemCount <= 0)
        return "None";
      if (itemCount == 1)
        return "Single";
      if (itemCount < 10)
        return "Few";
      if (itemCount < 100)
        return "Tens";
      return itemCount < 1000 ? "Hundreds" : "Thousands";
    }

    private static string GetSingleDescriptorInfo(string descriptor)
    {
      string[] strArray = descriptor.Split(new char[1]
      {
        ';'
      }, 3, StringSplitOptions.RemoveEmptyEntries);
      string str1;
      string str2;
      if (strArray.Length == 2)
      {
        str1 = strArray[0];
        str2 = HttpUtility.UrlDecode(strArray[1]);
      }
      else
      {
        if (strArray.Length != 1)
          return "UnrecognizedDescriptorFormat";
        str1 = "Microsoft.IdentityModel.Claims.ClaimsIdentity";
        str2 = HttpUtility.UrlDecode(strArray[0]);
      }
      return IdentitiesControllerLoggingNameGenerator.IdentityTypeHandlerLookup(str1)(str2);
    }

    private static string ClaimsTypeLookup(string identifier)
    {
      if (identifier.EndsWith("@Live.com", StringComparison.OrdinalIgnoreCase))
        return "MSA";
      if (identifier.Contains<char>('\\'))
        return "AAD";
      if (identifier.Contains<char>('@'))
        return "ServicePrincipal";
      return Guid.TryParse(identifier, out Guid _) ? "CloudBuildAgent" : "UnrecognizedClaimsType";
    }

    private static string BindPendingTypeLookup(string identifier)
    {
      string[] strArray = identifier.Split(new char[1]
      {
        '\\'
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length == 1 && identifier.StartsWith("email:", StringComparison.OrdinalIgnoreCase))
        return "InvitedLegacyMSA";
      if (strArray.Length != 2)
        return "UnknownInvitedIdentity";
      string str = strArray[0];
      if (str.Equals("upn:Windows Live ID", StringComparison.OrdinalIgnoreCase))
        return "InvitedMSA";
      return Guid.TryParse(str.Replace("upn:", ""), out Guid _) ? "InvitedAAD" : "UnknownInvitedIdentity";
    }

    public static void RenameCurrentServiceIfRequestIsMembershipRelated(
      IVssRequestContext tfsRequestContext,
      string methodName)
    {
      if (!IdentitiesControllerLoggingNameGenerator.ReadMembershipSuffixes.Any<string>(new Func<string, bool>(methodName.EndsWith)))
        return;
      tfsRequestContext.ServiceName = "Memberships";
    }

    private static class IdentityBatchTelemetryConstants
    {
      public const string QueryMembershipHint = "queryMembership";
      public const string FlavorHint = "flavor";
      public const string CountHint = "count";
      public const string ByIdFlavor = "id";
      public const string ByDescriptorFlavor = "descriptor";
      public const string BySubjectDescriptorFlavor = "subjectDescriptor";
    }
  }
}
