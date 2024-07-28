// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.AadObjectCompareHelpers
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.IdentityModel.Tokens;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Aad.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory
{
  internal static class AadObjectCompareHelpers
  {
    public static string CompareAndGetDifferenceAadUser(AadUser baseUser, AadUser toCompareUser)
    {
      if (baseUser == null && toCompareUser == null)
        return (string) null;
      if (baseUser == null)
        return "Baseline user is null.";
      if (toCompareUser == null)
        return "User to compare with is null.";
      StringBuilder sb = new StringBuilder();
      if (baseUser.ObjectId != toCompareUser.ObjectId)
        sb.AppendLine(string.Format("ObjectId does not match. Expected {0}, Actual {1}", (object) baseUser.ObjectId, (object) toCompareUser.ObjectId));
      if (baseUser.AccountEnabled != toCompareUser.AccountEnabled)
        sb.AppendLine(string.Format("AccountEnabled does not match. Expected {0}, Actual {1}", (object) baseUser.AccountEnabled, (object) toCompareUser.AccountEnabled));
      if (baseUser.DisplayName != toCompareUser.DisplayName)
        sb.AppendLine("DisplayName does not match. Expected " + baseUser.DisplayName + ", Actual " + toCompareUser.DisplayName);
      if (baseUser.Mail != toCompareUser.Mail)
        sb.AppendLine("Mail does not match. Expected " + baseUser.Mail + ", Actual " + toCompareUser.Mail);
      string countOfIenumerables = AadObjectCompareHelpers.CompareAndGetDifferenceForCountOfIEnumerables<string>(baseUser.OtherMails, toCompareUser.OtherMails, "OtherMails");
      if (string.IsNullOrEmpty(countOfIenumerables))
      {
        if (baseUser.OtherMails != null && toCompareUser.OtherMails != null && !baseUser.OtherMails.SequenceEqual<string>(toCompareUser.OtherMails))
          sb.AppendLine("OtherMails does not match. Expected " + string.Join(",", baseUser.OtherMails) + ", Actual " + string.Join(",", toCompareUser.OtherMails));
      }
      else
        sb.AppendLine(countOfIenumerables);
      if (baseUser.MailNickname != toCompareUser.MailNickname)
        sb.AppendLine("MailNickname does not match. Expected " + baseUser.MailNickname + ", Actual " + toCompareUser.MailNickname);
      if (baseUser.UserPrincipalName != toCompareUser.UserPrincipalName)
        sb.AppendLine("UserPrincipalName does not match. Expected " + baseUser.UserPrincipalName + ", Actual " + toCompareUser.UserPrincipalName);
      if (baseUser.SignInAddress != toCompareUser.SignInAddress)
        sb.AppendLine("SignInAddress does not match. Expected " + baseUser.SignInAddress + ", Actual " + toCompareUser.SignInAddress);
      if (baseUser.JobTitle != toCompareUser.JobTitle)
        sb.AppendLine("JobTitle does not match. Expected " + baseUser.JobTitle + ", Actual " + toCompareUser.JobTitle);
      if (baseUser.Department != toCompareUser.Department)
        sb.AppendLine("Department does not match. Expected " + baseUser.Department + ", Actual " + toCompareUser.Department);
      if (baseUser.PhysicalDeliveryOfficeName != toCompareUser.PhysicalDeliveryOfficeName)
        sb.AppendLine("PhysicalDeliveryOfficeName does not match. Expected " + baseUser.PhysicalDeliveryOfficeName + ", Actual " + toCompareUser.PhysicalDeliveryOfficeName);
      if (baseUser.Surname != toCompareUser.Surname)
        sb.AppendLine("Surname does not match. Expected " + baseUser.Surname + ", Actual " + toCompareUser.Surname);
      if (baseUser.UserType != toCompareUser.UserType)
        sb.AppendLine("UserType does not match. Expected " + baseUser.UserType + ", Actual " + toCompareUser.UserType);
      if (baseUser.OnPremisesSecurityIdentifier != toCompareUser.OnPremisesSecurityIdentifier)
        sb.AppendLine("OnPremisesSecurityIdentifier does not match. Expected " + baseUser.OnPremisesSecurityIdentifier + ", Actual " + toCompareUser.OnPremisesSecurityIdentifier);
      if (baseUser.ImmutableId != toCompareUser.ImmutableId)
        sb.AppendLine("ImmutableId does not match. Expected " + baseUser.ImmutableId + ", Actual " + toCompareUser.ImmutableId);
      if (baseUser.TelephoneNumber != toCompareUser.TelephoneNumber)
        sb.AppendLine("TelephoneNumber does not match. Expected " + baseUser.TelephoneNumber + ", Actual " + toCompareUser.TelephoneNumber);
      if (baseUser.Country != toCompareUser.Country)
        sb.AppendLine("Country does not match. Expected " + baseUser.Country + ", Actual " + toCompareUser.Country);
      if (baseUser.UsageLocation != toCompareUser.UsageLocation)
        sb.AppendLine("UsageLocation does not match. Expected " + baseUser.UsageLocation + ", Actual " + toCompareUser.UsageLocation);
      sb.AppendLineIfNotNullOrEmpty(AadObjectCompareHelpers.CompareAndGetDifferenceAadUser(baseUser.Manager, toCompareUser.Manager));
      sb.AppendLineIfNotNullOrEmpty(AadObjectCompareHelpers.CompareAndGetDifferenceAadUsers(baseUser.DirectReports, toCompareUser.DirectReports));
      return sb.Length <= 0 ? (string) null : sb.ToString();
    }

    public static string CompareAndGetDifferenceAadGroup(
      AadGroup baseGroup,
      AadGroup toCompareGroup)
    {
      if (baseGroup == null && toCompareGroup == null)
        return (string) null;
      if (baseGroup == null)
        return "Baseline group is null.";
      if (toCompareGroup == null)
        return "Group to compare with is null.";
      StringBuilder stringBuilder = new StringBuilder();
      if (baseGroup.ObjectId != toCompareGroup.ObjectId)
        stringBuilder.AppendLine(string.Format("ObjectId does not match. Expected {0}, Actual {1}", (object) baseGroup.ObjectId, (object) toCompareGroup.ObjectId));
      if (baseGroup.Description != toCompareGroup.Description)
        stringBuilder.AppendLine("Description does not match. Expected " + baseGroup.Description + ", Actual " + toCompareGroup.Description);
      if (baseGroup.DisplayName != toCompareGroup.DisplayName)
        stringBuilder.AppendLine("DisplayName does not match. Expected " + baseGroup.DisplayName + ", Actual " + toCompareGroup.DisplayName);
      if (baseGroup.MailNickname != toCompareGroup.MailNickname)
        stringBuilder.AppendLine("MailNickname does not match. Expected " + baseGroup.MailNickname + ", Actual " + toCompareGroup.MailNickname);
      if (baseGroup.Mail != toCompareGroup.Mail)
        stringBuilder.AppendLine("Mail does not match. Expected " + baseGroup.Mail + ", Actual " + toCompareGroup.Mail);
      if (baseGroup.OnPremisesSecurityIdentifier != toCompareGroup.OnPremisesSecurityIdentifier)
        stringBuilder.AppendLine("OnPremisesSecurityIdentifier does not match. Expected " + baseGroup.OnPremisesSecurityIdentifier + ", Actual " + toCompareGroup.OnPremisesSecurityIdentifier);
      return stringBuilder.Length <= 0 ? (string) null : stringBuilder.ToString();
    }

    private static string CompareAndGetDifferenceAadServicePrincipal(
      AadServicePrincipal baseServicePrincipal,
      AadServicePrincipal toCompareServicePrincipal)
    {
      if (baseServicePrincipal == null && toCompareServicePrincipal == null)
        return (string) null;
      if (baseServicePrincipal == null)
        return "Baseline ServicePrincipal is null.";
      if (toCompareServicePrincipal == null)
        return "ServicePrincipal to compare with is null.";
      StringBuilder stringBuilder = new StringBuilder();
      if (baseServicePrincipal.ObjectId != toCompareServicePrincipal.ObjectId)
        stringBuilder.AppendLine(string.Format("ObjectId does not match. Expected {0}, Actual {1}", (object) baseServicePrincipal.ObjectId, (object) toCompareServicePrincipal.ObjectId));
      if (baseServicePrincipal.DisplayName != toCompareServicePrincipal.DisplayName)
        stringBuilder.AppendLine("DisplayName does not match. Expected " + baseServicePrincipal.DisplayName + ", Actual " + toCompareServicePrincipal.DisplayName);
      if (baseServicePrincipal.AppId != toCompareServicePrincipal.AppId)
        stringBuilder.AppendLine(string.Format("AppId does not match. Expected {0}, Actual {1}", (object) baseServicePrincipal.AppId, (object) toCompareServicePrincipal.AppId));
      if (baseServicePrincipal.ServicePrincipalType != toCompareServicePrincipal.ServicePrincipalType)
        stringBuilder.AppendLine("ServicePrincipalType does not match. Expected " + baseServicePrincipal.ServicePrincipalType + ", Actual " + toCompareServicePrincipal.ServicePrincipalType);
      return stringBuilder.Length <= 0 ? (string) null : stringBuilder.ToString();
    }

    public static string CompareAndGetDifferenceAadTenant(
      AadTenant baseTenant,
      AadTenant toCompareTenant)
    {
      if (baseTenant == null && toCompareTenant == null)
        return (string) null;
      if (baseTenant == null)
        return "Baseline tenant is null.";
      if (toCompareTenant == null)
        return "Tenant to compare with is null.";
      StringBuilder sb = new StringBuilder();
      if (baseTenant.ObjectId != toCompareTenant.ObjectId)
        sb.AppendLine(string.Format("ObjectId does not match. Expected {0}, Actual {1}. ", (object) baseTenant.ObjectId, (object) toCompareTenant.ObjectId));
      if (baseTenant.DisplayName != toCompareTenant.DisplayName)
        sb.AppendLine("DisplayName does not match.");
      bool? dirSyncEnabled1 = baseTenant.DirSyncEnabled;
      bool? dirSyncEnabled2 = toCompareTenant.DirSyncEnabled;
      if (!(dirSyncEnabled1.GetValueOrDefault() == dirSyncEnabled2.GetValueOrDefault() & dirSyncEnabled1.HasValue == dirSyncEnabled2.HasValue))
        sb.AppendLine(string.Format("DirSyncEnabled does not match. Expected {0}, Actual {1}. ", (object) baseTenant.DirSyncEnabled, (object) toCompareTenant.DirSyncEnabled));
      sb.AppendLineIfNotNullOrEmpty(AadObjectCompareHelpers.CompareAndGetDifferenceAadDomains(baseTenant.VerifiedDomains, toCompareTenant.VerifiedDomains));
      return sb.Length <= 0 ? (string) null : sb.ToString();
    }

    public static string CompareAndGetDifferenceDescendantId(
      Tuple<Guid, AadObjectType> baseDescendantId,
      Tuple<Guid, AadObjectType> toCompareDescendantId)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (baseDescendantId.Item1 != toCompareDescendantId.Item1)
        stringBuilder.AppendLine(string.Format("DescendantId does not match. Expected {0}, Actual {1}", (object) baseDescendantId.Item1, (object) toCompareDescendantId.Item1));
      if (baseDescendantId.Item2 != toCompareDescendantId.Item2)
        stringBuilder.AppendLine(string.Format("DescendantId type does not match. Expected {0}, Actual {1}", (object) baseDescendantId.Item2, (object) toCompareDescendantId.Item2));
      return stringBuilder.Length <= 0 ? (string) null : stringBuilder.ToString();
    }

    public static string CompareAndGetDifferenceAadDomain(
      AadDomain baseDomain,
      AadDomain toCompareDomain)
    {
      if (baseDomain == null && toCompareDomain == null)
        return (string) null;
      if (baseDomain == null)
        return "Baseline domain is null.";
      if (toCompareDomain == null)
        return "Domain to compare with is null.";
      StringBuilder stringBuilder = new StringBuilder();
      if (baseDomain.Name != toCompareDomain.Name)
        stringBuilder.AppendLine("Name does not match. Expected " + baseDomain.Name + ", Actual " + toCompareDomain.Name);
      if (baseDomain.IsDefault != toCompareDomain.IsDefault)
        stringBuilder.AppendLine(string.Format("IsDefault does not match. Expected {0}, Actual {1}", (object) baseDomain.IsDefault, (object) toCompareDomain.IsDefault));
      return stringBuilder.Length <= 0 ? (string) null : stringBuilder.ToString();
    }

    public static string CompareAndGetDifferenceAadObject(
      AadObject baseObject,
      AadObject toCompareObject)
    {
      if (baseObject == null && toCompareObject == null)
        return (string) null;
      if (baseObject == null)
        return "Baseline Aad Object is null.";
      if (toCompareObject == null)
        return "Aad Object to compare with is null.";
      StringBuilder sb = new StringBuilder();
      if (baseObject.GetType() != toCompareObject.GetType())
      {
        sb.AppendLine(string.Format("Type is different. Expected {0}, actual {1}", (object) baseObject.GetType(), (object) toCompareObject.GetType()));
      }
      else
      {
        switch (baseObject)
        {
          case AadUser baseUser:
            sb.AppendLineIfNotNullOrEmpty(AadObjectCompareHelpers.CompareAndGetDifferenceAadUser(baseUser, toCompareObject as AadUser));
            break;
          case AadGroup baseGroup:
            sb.AppendLineIfNotNullOrEmpty(AadObjectCompareHelpers.CompareAndGetDifferenceAadGroup(baseGroup, toCompareObject as AadGroup));
            break;
          case AadServicePrincipal baseServicePrincipal:
            sb.AppendLineIfNotNullOrEmpty(AadObjectCompareHelpers.CompareAndGetDifferenceAadServicePrincipal(baseServicePrincipal, toCompareObject as AadServicePrincipal));
            break;
          default:
            sb.Append(string.Format("Unknown type of AadObject : {0}", (object) baseObject.GetType()));
            break;
        }
      }
      return sb.Length <= 0 ? (string) null : sb.ToString();
    }

    public static string CompareAndGetDifferenceDeletedObject<TIdentifier, TType>(
      KeyValuePair<TIdentifier, GetSoftDeletedObjectResponse<TType>> expectedObject,
      KeyValuePair<TIdentifier, GetSoftDeletedObjectResponse<TType>> actualObject)
      where TType : AadObject
    {
      if ((object) expectedObject.Key == null)
        return "Expected Identifier is null";
      if (!((object) expectedObject.Key).Equals((object) actualObject.Key))
        return string.Format("Identifier is different. Expected {0}, actual {1}", (object) expectedObject.Key, (object) actualObject.Key);
      string empty = string.Empty;
      GetSoftDeletedObjectResponse<TType> deletedObjectResponse1 = expectedObject.Value;
      bool flag1 = (object) (deletedObjectResponse1 != null ? deletedObjectResponse1.Object : default (TType)) == null;
      GetSoftDeletedObjectResponse<TType> deletedObjectResponse2 = actualObject.Value;
      bool flag2 = (object) (deletedObjectResponse2 != null ? deletedObjectResponse2.Object : default (TType)) == null;
      if (flag1 ^ flag2)
        empty += string.Join("", new string[6]
        {
          "Expected soft deleted object is ",
          flag1 ? "" : "not ",
          string.Format("null for Identifier '{0}';", (object) expectedObject.Key),
          "Actual soft deleted object is ",
          flag2 ? "" : "not ",
          string.Format("null for Identifier '{0}'. ", (object) actualObject.Key)
        });
      bool flag3 = expectedObject.Value?.Exception == null;
      bool flag4 = actualObject.Value?.Exception == null;
      if (flag3 ^ flag4)
        empty += string.Join("", new string[6]
        {
          "Expected soft deleted object Exception is ",
          flag3 ? "" : "not ",
          string.Format("null for Identifier '{0}';", (object) expectedObject.Key),
          "Actual soft deleted object Exception is ",
          flag4 ? "" : "not ",
          string.Format("null for Identifier '{0}'. ", (object) actualObject.Key)
        });
      return empty.IsNullOrEmpty<char>() ? AadObjectCompareHelpers.CompareAndGetDifferenceAadObject((AadObject) expectedObject.Value.Object, (AadObject) actualObject.Value.Object) : empty;
    }

    public static string CompareAndGetDifferenceDeletedObjects<TIdentifier, TType>(
      IDictionary<TIdentifier, GetSoftDeletedObjectResponse<TType>> baseObject,
      IDictionary<TIdentifier, GetSoftDeletedObjectResponse<TType>> toCompareObject)
      where TType : AadObject
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AadObjectCompareHelpers.CompareAndGetDifferenceObjects<KeyValuePair<TIdentifier, GetSoftDeletedObjectResponse<TType>>>(AadObjectCompareHelpers.\u003CCompareAndGetDifferenceDeletedObjects\u003EO__8_0<TIdentifier, TType>.\u003C0\u003E__CompareAndGetDifferenceDeletedObject ?? (AadObjectCompareHelpers.\u003CCompareAndGetDifferenceDeletedObjects\u003EO__8_0<TIdentifier, TType>.\u003C0\u003E__CompareAndGetDifferenceDeletedObject = new Func<KeyValuePair<TIdentifier, GetSoftDeletedObjectResponse<TType>>, KeyValuePair<TIdentifier, GetSoftDeletedObjectResponse<TType>>, string>(AadObjectCompareHelpers.CompareAndGetDifferenceDeletedObject<TIdentifier, TType>)), "SoftDeletedObject", (IEnumerable<KeyValuePair<TIdentifier, GetSoftDeletedObjectResponse<TType>>>) baseObject, (IEnumerable<KeyValuePair<TIdentifier, GetSoftDeletedObjectResponse<TType>>>) toCompareObject, (Func<KeyValuePair<TIdentifier, GetSoftDeletedObjectResponse<TType>>, object>) (kvp => (object) kvp.Key));
    }

    public static string CompareAndGetDifferenceAncestorIds<T>(
      KeyValuePair<T, ISet<Guid>> baseObject,
      KeyValuePair<T, ISet<Guid>> toCompareObject)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if ((object) baseObject.Key == null)
        stringBuilder.AppendLine("Expected Identifier is null");
      if (!((object) baseObject.Key).Equals((object) toCompareObject.Key))
        stringBuilder.AppendLine(string.Format("Identifier is different. Expected {0}, actual {1}", (object) baseObject.Key, (object) toCompareObject.Key));
      ISet<Guid> guidSet1 = baseObject.Value ?? (ISet<Guid>) new HashSet<Guid>();
      ISet<Guid> guidSet2 = toCompareObject.Value ?? (ISet<Guid>) new HashSet<Guid>();
      if (!guidSet1.SetEquals((IEnumerable<Guid>) guidSet2))
      {
        IEnumerable<Guid> guids1 = guidSet1.Except<Guid>((IEnumerable<Guid>) guidSet2);
        IEnumerable<Guid> guids2 = guidSet2.Except<Guid>((IEnumerable<Guid>) guidSet1);
        stringBuilder.Append("Ancestor Ids are different, following only exist in expected '" + guids1.Serialize<IEnumerable<Guid>>() + "', following only exists in actual '" + guids2.Serialize<IEnumerable<Guid>>() + "'");
      }
      if (baseObject.Value == null)
        stringBuilder.AppendLine(string.Format("Expected ancestors of Identifier '{0}' is null.", (object) baseObject.Key));
      if (toCompareObject.Value == null)
        stringBuilder.AppendLine(string.Format("Actual ancestors of Identifier '{0}' is null.", (object) toCompareObject.Key));
      return stringBuilder.Length <= 0 ? (string) null : stringBuilder.ToString();
    }

    public static string CompareAndGetDifferenceAadUsers(
      IEnumerable<AadUser> baseUsers,
      IEnumerable<AadUser> toCompareUsers)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AadObjectCompareHelpers.CompareAndGetDifferenceObjects<AadUser>(AadObjectCompareHelpers.\u003C\u003EO.\u003C0\u003E__CompareAndGetDifferenceAadUser ?? (AadObjectCompareHelpers.\u003C\u003EO.\u003C0\u003E__CompareAndGetDifferenceAadUser = new Func<AadUser, AadUser, string>(AadObjectCompareHelpers.CompareAndGetDifferenceAadUser)), "AadUser", baseUsers, toCompareUsers, (Func<AadUser, object>) (kvp => (object) kvp.ObjectId));
    }

    public static string CompareAndGetDifferenceAadGroups(
      IEnumerable<AadGroup> baseGroups,
      IEnumerable<AadGroup> toCompareGroups)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AadObjectCompareHelpers.CompareAndGetDifferenceObjects<AadGroup>(AadObjectCompareHelpers.\u003C\u003EO.\u003C1\u003E__CompareAndGetDifferenceAadGroup ?? (AadObjectCompareHelpers.\u003C\u003EO.\u003C1\u003E__CompareAndGetDifferenceAadGroup = new Func<AadGroup, AadGroup, string>(AadObjectCompareHelpers.CompareAndGetDifferenceAadGroup)), "AadGroup", baseGroups, toCompareGroups, (Func<AadGroup, object>) (kvp => (object) kvp.ObjectId));
    }

    public static string CompareAndGetDifferenceAadDirectoryRolesDict(
      IDictionary<Guid, AadDirectoryRole> baseDirectoryRoles,
      IDictionary<Guid, AadDirectoryRole> toCompareDirectoryRoles)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AadObjectCompareHelpers.CompareAndGetDifferenceObjects<KeyValuePair<Guid, AadDirectoryRole>>(AadObjectCompareHelpers.\u003C\u003EO.\u003C2\u003E__CompareAndGetDifferenceAadDirectoryRoleDict ?? (AadObjectCompareHelpers.\u003C\u003EO.\u003C2\u003E__CompareAndGetDifferenceAadDirectoryRoleDict = new Func<KeyValuePair<Guid, AadDirectoryRole>, KeyValuePair<Guid, AadDirectoryRole>, string>(AadObjectCompareHelpers.CompareAndGetDifferenceAadDirectoryRoleDict)), "AadDirectoryRole", (IEnumerable<KeyValuePair<Guid, AadDirectoryRole>>) baseDirectoryRoles, (IEnumerable<KeyValuePair<Guid, AadDirectoryRole>>) toCompareDirectoryRoles, (Func<KeyValuePair<Guid, AadDirectoryRole>, object>) (kvp => (object) kvp.Key));
    }

    public static string CompareAndGetDifferenceDirectoryRoleMembers(
      ISet<AadObject> baseMembers,
      ISet<AadObject> toCompareMembers)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AadObjectCompareHelpers.CompareAndGetDifferenceObjects<AadObject>(AadObjectCompareHelpers.\u003C\u003EO.\u003C3\u003E__CompareAndGetDifferenceAadObject ?? (AadObjectCompareHelpers.\u003C\u003EO.\u003C3\u003E__CompareAndGetDifferenceAadObject = new Func<AadObject, AadObject, string>(AadObjectCompareHelpers.CompareAndGetDifferenceAadObject)), "RoleMember", (IEnumerable<AadObject>) baseMembers, (IEnumerable<AadObject>) toCompareMembers, (Func<AadObject, object>) (kvp => (object) kvp.ObjectId));
    }

    public static string CompareAndGetDifferenceUserRolesAndGroups(
      ISet<Guid> baseMembers,
      ISet<Guid> toCompareMembers)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AadObjectCompareHelpers.CompareAndGetDifferenceObjects<Guid>(AadObjectCompareHelpers.\u003C\u003EO.\u003C4\u003E__CompareAndGetDifferenceUserRolesAndGroups ?? (AadObjectCompareHelpers.\u003C\u003EO.\u003C4\u003E__CompareAndGetDifferenceUserRolesAndGroups = new Func<Guid, Guid, string>(AadObjectCompareHelpers.CompareAndGetDifferenceUserRolesAndGroups)), "UserRolesAndGroups", (IEnumerable<Guid>) baseMembers, (IEnumerable<Guid>) toCompareMembers, (Func<Guid, object>) (kvp => (object) kvp));
    }

    public static string CompareAndGetDifferenceDescendantIds(
      IEnumerable<Tuple<Guid, AadObjectType>> baseDescendantIds,
      IEnumerable<Tuple<Guid, AadObjectType>> toCompareDescendantIds)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AadObjectCompareHelpers.CompareAndGetDifferenceObjects<Tuple<Guid, AadObjectType>>(AadObjectCompareHelpers.\u003C\u003EO.\u003C5\u003E__CompareAndGetDifferenceDescendantId ?? (AadObjectCompareHelpers.\u003C\u003EO.\u003C5\u003E__CompareAndGetDifferenceDescendantId = new Func<Tuple<Guid, AadObjectType>, Tuple<Guid, AadObjectType>, string>(AadObjectCompareHelpers.CompareAndGetDifferenceDescendantId)), "DescendantId", baseDescendantIds, toCompareDescendantIds, (Func<Tuple<Guid, AadObjectType>, object>) (kvp => (object) kvp.Item1));
    }

    public static string CompareAndGetDifferenceAadObjects(
      IEnumerable<AadObject> baseAadObjects,
      IEnumerable<AadObject> toCompareAadObjects)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AadObjectCompareHelpers.CompareAndGetDifferenceObjects<AadObject>(AadObjectCompareHelpers.\u003C\u003EO.\u003C3\u003E__CompareAndGetDifferenceAadObject ?? (AadObjectCompareHelpers.\u003C\u003EO.\u003C3\u003E__CompareAndGetDifferenceAadObject = new Func<AadObject, AadObject, string>(AadObjectCompareHelpers.CompareAndGetDifferenceAadObject)), "AadObject", baseAadObjects, toCompareAadObjects, (Func<AadObject, object>) (kvp => (object) kvp.ObjectId));
    }

    public static string CompareandGetDifferenceAncestorIdsDictionary<TIdentifier>(
      IDictionary<TIdentifier, ISet<Guid>> baseAncestorIds,
      IDictionary<TIdentifier, ISet<Guid>> toCompareAncestorIds)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AadObjectCompareHelpers.CompareAndGetDifferenceObjects<KeyValuePair<TIdentifier, ISet<Guid>>>(AadObjectCompareHelpers.\u003CCompareandGetDifferenceAncestorIdsDictionary\u003EO__17_0<TIdentifier>.\u003C0\u003E__CompareAndGetDifferenceAncestorIds ?? (AadObjectCompareHelpers.\u003CCompareandGetDifferenceAncestorIdsDictionary\u003EO__17_0<TIdentifier>.\u003C0\u003E__CompareAndGetDifferenceAncestorIds = new Func<KeyValuePair<TIdentifier, ISet<Guid>>, KeyValuePair<TIdentifier, ISet<Guid>>, string>(AadObjectCompareHelpers.CompareAndGetDifferenceAncestorIds<TIdentifier>)), "AncestorIds", (IEnumerable<KeyValuePair<TIdentifier, ISet<Guid>>>) baseAncestorIds, (IEnumerable<KeyValuePair<TIdentifier, ISet<Guid>>>) toCompareAncestorIds, (Func<KeyValuePair<TIdentifier, ISet<Guid>>, object>) (kvp => (object) kvp.Key));
    }

    public static string CompareAndGetDifferenceAadDirectoryRoleDict(
      KeyValuePair<Guid, AadDirectoryRole> baseDirectoryRole,
      KeyValuePair<Guid, AadDirectoryRole> toCompareDirectoryRoles)
    {
      Guid key = baseDirectoryRole.Key;
      if (baseDirectoryRole.Value == null && toCompareDirectoryRoles.Value == null)
        return (string) null;
      if (baseDirectoryRole.Value == null)
        return "Baseline DirectoryRole is null.";
      if (toCompareDirectoryRoles.Value == null)
        return "DirectoryRole to compare with is null.";
      StringBuilder stringBuilder = new StringBuilder();
      if (!baseDirectoryRole.Key.Equals(toCompareDirectoryRoles.Key))
        stringBuilder.AppendLine(string.Format("Key does not match. Expected {0}, Actual {1}", (object) baseDirectoryRole.Key, (object) toCompareDirectoryRoles.Key));
      if (baseDirectoryRole.Value.ObjectId != toCompareDirectoryRoles.Value.ObjectId)
        stringBuilder.AppendLine(string.Format("ObjectId does not match. Expected {0}, Actual {1}", (object) baseDirectoryRole.Value.ObjectId, (object) toCompareDirectoryRoles.Value.ObjectId));
      if (baseDirectoryRole.Value.Description != toCompareDirectoryRoles.Value.Description)
        stringBuilder.AppendLine("Description does not match. Expected " + baseDirectoryRole.Value.Description + ", Actual " + toCompareDirectoryRoles.Value.Description);
      if (baseDirectoryRole.Value.DisplayName != toCompareDirectoryRoles.Value.DisplayName)
        stringBuilder.AppendLine("DisplayName does not match. Expected " + baseDirectoryRole.Value.DisplayName + ", Actual " + toCompareDirectoryRoles.Value.DisplayName);
      if (baseDirectoryRole.Value.RoleTemplateId != toCompareDirectoryRoles.Value.RoleTemplateId)
        stringBuilder.AppendLine("RoleTemplateId does not match. Expected " + baseDirectoryRole.Value.RoleTemplateId + ", Actual " + toCompareDirectoryRoles.Value.RoleTemplateId);
      return stringBuilder.Length <= 0 ? (string) null : stringBuilder.ToString();
    }

    public static string CompareAndGetDifferenceUserRolesAndGroups(
      Guid baseMember,
      Guid toCompareMember)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (!baseMember.Equals(toCompareMember))
        stringBuilder.AppendLine(string.Format("UserRolesAndGroups does not match. Expected {0}, Actual {1}", (object) baseMember, (object) toCompareMember));
      return stringBuilder.Length <= 0 ? (string) null : stringBuilder.ToString();
    }

    private static string CompareAndGetDifferenceAadDomains(
      IEnumerable<AadDomain> baseDomains,
      IEnumerable<AadDomain> toCompareDomains)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AadObjectCompareHelpers.CompareAndGetDifferenceObjects<AadDomain>(AadObjectCompareHelpers.\u003C\u003EO.\u003C6\u003E__CompareAndGetDifferenceAadDomain ?? (AadObjectCompareHelpers.\u003C\u003EO.\u003C6\u003E__CompareAndGetDifferenceAadDomain = new Func<AadDomain, AadDomain, string>(AadObjectCompareHelpers.CompareAndGetDifferenceAadDomain)), "AadDomain", baseDomains, toCompareDomains);
    }

    private static string CompareAndGetDifferenceObjectWithTIdentifier<TIdentifier, V>(
      KeyValuePair<TIdentifier, V> baseObject,
      KeyValuePair<TIdentifier, V> toCompareObject)
    {
      if ((object) baseObject.Key == null && (object) toCompareObject.Key == null)
        return (string) null;
      if ((object) baseObject.Value == null && (object) toCompareObject.Value == null)
        return (string) null;
      if ((object) baseObject.Value == null)
        return "Baseline Object with TIdentifier is null.";
      if ((object) baseObject.Value == null)
        return "Object with TIdentifier to compare with is null.";
      StringBuilder sb = new StringBuilder();
      if (!baseObject.Key.Equals((object) toCompareObject.Key))
        sb.AppendLine(string.Format("Key does not match. Expected {0}, Actual {1}", (object) baseObject.Key, (object) toCompareObject.Key));
      V v = baseObject.Value;
      Type type1 = v.GetType();
      v = toCompareObject.Value;
      Type type2 = v.GetType();
      if (type1 != type2)
      {
        StringBuilder stringBuilder = sb;
        v = baseObject.Value;
        Type type3 = v.GetType();
        v = toCompareObject.Value;
        Type type4 = v.GetType();
        string str = string.Format("Type is different. Expected {0}, actual {1}", (object) type3, (object) type4);
        stringBuilder.AppendLine(str);
      }
      else
      {
        v = baseObject.Value;
        switch (v)
        {
          case AadUser baseUser:
            sb.AppendLineIfNotNullOrEmpty(AadObjectCompareHelpers.CompareAndGetDifferenceAadUser(baseUser, (object) toCompareObject.Value as AadUser));
            break;
          case AadGroup baseGroup:
            sb.AppendLineIfNotNullOrEmpty(AadObjectCompareHelpers.CompareAndGetDifferenceAadGroup(baseGroup, (object) toCompareObject.Value as AadGroup));
            break;
          default:
            sb.Append(string.Format("Unknown type of AadObject : {0}", (object) baseObject.GetType()));
            break;
        }
      }
      return sb.Length <= 0 ? (string) null : sb.ToString();
    }

    public static string CompareAndGetDifferenceDictionaryTIdentifierAadGroups<TIdentifier>(
      IDictionary<TIdentifier, AadGroup> baseGroups,
      IDictionary<TIdentifier, AadGroup> toCompareGroups)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AadObjectCompareHelpers.CompareAndGetDifferenceObjects<KeyValuePair<TIdentifier, AadGroup>>(AadObjectCompareHelpers.\u003CCompareAndGetDifferenceDictionaryTIdentifierAadGroups\u003EO__22_0<TIdentifier>.\u003C0\u003E__CompareAndGetDifferenceObjectWithTIdentifier ?? (AadObjectCompareHelpers.\u003CCompareAndGetDifferenceDictionaryTIdentifierAadGroups\u003EO__22_0<TIdentifier>.\u003C0\u003E__CompareAndGetDifferenceObjectWithTIdentifier = new Func<KeyValuePair<TIdentifier, AadGroup>, KeyValuePair<TIdentifier, AadGroup>, string>(AadObjectCompareHelpers.CompareAndGetDifferenceObjectWithTIdentifier<TIdentifier, AadGroup>)), "AadGroup", (IEnumerable<KeyValuePair<TIdentifier, AadGroup>>) baseGroups, (IEnumerable<KeyValuePair<TIdentifier, AadGroup>>) toCompareGroups, (Func<KeyValuePair<TIdentifier, AadGroup>, object>) (kvp => (object) kvp.Key));
    }

    public static string CompareAndGetDifferenceDictionaryTIdentifierAadUsers<TIdentifier, AadUser>(
      IDictionary<TIdentifier, AadUser> baseUsers,
      IDictionary<TIdentifier, AadUser> toCompareUsers)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AadObjectCompareHelpers.CompareAndGetDifferenceObjects<KeyValuePair<TIdentifier, AadUser>>(AadObjectCompareHelpers.\u003CCompareAndGetDifferenceDictionaryTIdentifierAadUsers\u003EO__23_0<TIdentifier, AadUser>.\u003C0\u003E__CompareAndGetDifferenceObjectWithTIdentifier ?? (AadObjectCompareHelpers.\u003CCompareAndGetDifferenceDictionaryTIdentifierAadUsers\u003EO__23_0<TIdentifier, AadUser>.\u003C0\u003E__CompareAndGetDifferenceObjectWithTIdentifier = new Func<KeyValuePair<TIdentifier, AadUser>, KeyValuePair<TIdentifier, AadUser>, string>(AadObjectCompareHelpers.CompareAndGetDifferenceObjectWithTIdentifier<TIdentifier, AadUser>)), nameof (AadUser), (IEnumerable<KeyValuePair<TIdentifier, AadUser>>) baseUsers, (IEnumerable<KeyValuePair<TIdentifier, AadUser>>) toCompareUsers, (Func<KeyValuePair<TIdentifier, AadUser>, object>) (kvp => (object) kvp.Key));
    }

    public static string CompareUserThumbnail(
      byte[] baseUserThumbnail,
      byte[] toCompareUserThumbnail)
    {
      if (baseUserThumbnail == null && toCompareUserThumbnail == null)
        return (string) null;
      if (baseUserThumbnail == null)
        return "Baseline user thumbnail is null (and thumbnail to compare is not).";
      if (toCompareUserThumbnail == null)
        return "To compare user thumbnail is null (and baseline thumbnail is not).";
      if (baseUserThumbnail.Length != toCompareUserThumbnail.Length)
        return string.Format("Length of user thumbnail mismatch, Expected '{0}', Actual '{1}'.", (object) baseUserThumbnail.Length, (object) toCompareUserThumbnail.Length);
      return !((IEnumerable<byte>) baseUserThumbnail).SequenceEqual<byte>((IEnumerable<byte>) toCompareUserThumbnail) ? "Content of user thumbnail does not match (length matches)." : (string) null;
    }

    private static string CompareAndGetDifferenceDirectoryRoleMember(
      AadDirectoryRole baseMember,
      AadDirectoryRole toCompareMember)
    {
      if (baseMember == null && toCompareMember == null)
        return (string) null;
      if (baseMember == null)
        return "Baseline DirectoryRoleMember is null.";
      if (toCompareMember == null)
        return "DirectoryRoleMember to compare with is null.";
      StringBuilder stringBuilder = new StringBuilder();
      if (baseMember.ObjectId != toCompareMember.ObjectId)
        stringBuilder.AppendLine(string.Format("ObjectId does not match. Expected {0}, Actual {1}", (object) baseMember.ObjectId, (object) toCompareMember.ObjectId));
      if (baseMember.DisplayName != toCompareMember.DisplayName)
        stringBuilder.AppendLine("DisplayName does not match. Expected " + baseMember.DisplayName + ", Actual " + toCompareMember.DisplayName);
      if (baseMember.Description != toCompareMember.Description)
        stringBuilder.AppendLine("Description does not match. Expected " + baseMember.Description + ", Actual " + toCompareMember.Description);
      if (baseMember.RoleTemplateId != toCompareMember.RoleTemplateId)
        stringBuilder.AppendLine("RoleTemplateId does not match. Expected " + baseMember.RoleTemplateId + ", Actual " + toCompareMember.RoleTemplateId);
      return stringBuilder.Length <= 0 ? (string) null : stringBuilder.ToString();
    }

    public static string CompareAndGetDifferenceDirectoryRoles(
      IEnumerable<AadDirectoryRole> baseMembers,
      IEnumerable<AadDirectoryRole> toCompareMembers)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return AadObjectCompareHelpers.CompareAndGetDifferenceObjects<AadDirectoryRole>(AadObjectCompareHelpers.\u003C\u003EO.\u003C7\u003E__CompareAndGetDifferenceDirectoryRoleMember ?? (AadObjectCompareHelpers.\u003C\u003EO.\u003C7\u003E__CompareAndGetDifferenceDirectoryRoleMember = new Func<AadDirectoryRole, AadDirectoryRole, string>(AadObjectCompareHelpers.CompareAndGetDifferenceDirectoryRoleMember)), "RoleMember", baseMembers, toCompareMembers, (Func<AadDirectoryRole, object>) (kvp => (object) kvp.ObjectId));
    }

    internal static string CompareAndGetDifferenceGetTenants(
      GetTenantsResponse baseTenants,
      GetTenantsResponse toCompareTenants)
    {
      if (baseTenants == null && toCompareTenants == null)
        return (string) null;
      bool flag1 = baseTenants == null;
      bool flag2 = toCompareTenants == null;
      if (flag1 != flag2)
        return string.Empty + string.Join("", new string[6]
        {
          "Expected Tenants Response is ",
          flag1 ? "" : "not ",
          "null;",
          "Actual Tenants Response is ",
          flag2 ? "" : "not ",
          "null;'. "
        });
      StringBuilder sb = new StringBuilder();
      if (sb.AppendLineIfNotNullOrEmpty(AadObjectCompareHelpers.CompareAndGetDifferenceAadTenant(baseTenants.HomeTenant, toCompareTenants.HomeTenant)))
        sb.AppendLine(" - HomeTenant has differences ");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (sb.AppendLineIfNotNullOrEmpty(AadObjectCompareHelpers.CompareAndGetDifferenceObjects<AadTenant>(AadObjectCompareHelpers.\u003C\u003EO.\u003C8\u003E__CompareAndGetDifferenceAadTenant ?? (AadObjectCompareHelpers.\u003C\u003EO.\u003C8\u003E__CompareAndGetDifferenceAadTenant = new Func<AadTenant, AadTenant, string>(AadObjectCompareHelpers.CompareAndGetDifferenceAadTenant)), "ForeignTenants", baseTenants.ForeignTenants, toCompareTenants.ForeignTenants, (Func<AadTenant, object>) (kvp => (object) kvp.ObjectId))))
        sb.AppendLine(" - ForeignTenants have differences ");
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (sb.AppendLineIfNotNullOrEmpty(AadObjectCompareHelpers.CompareAndGetDifferenceObjects<AadTenant>(AadObjectCompareHelpers.\u003C\u003EO.\u003C8\u003E__CompareAndGetDifferenceAadTenant ?? (AadObjectCompareHelpers.\u003C\u003EO.\u003C8\u003E__CompareAndGetDifferenceAadTenant = new Func<AadTenant, AadTenant, string>(AadObjectCompareHelpers.CompareAndGetDifferenceAadTenant)), "Tenants", baseTenants.Tenants, toCompareTenants.Tenants, (Func<AadTenant, object>) (kvp => (object) kvp.ObjectId))))
        sb.AppendLine(" - Tenants have differences ");
      return sb.Length <= 0 ? (string) null : sb.ToString();
    }

    private static string CompareAndGetDifferenceForCountOfIEnumerables<T>(
      IEnumerable<T> baseItems,
      IEnumerable<T> toCompareItems,
      string itemName)
    {
      int num1 = baseItems != null ? baseItems.Count<T>() : 0;
      int num2 = toCompareItems != null ? toCompareItems.Count<T>() : 0;
      return num1 != num2 ? string.Format("Count of '{0}' mismatch, Expected '{1}', Actual '{2}'.", (object) itemName, (object) num1, (object) num2) : (string) null;
    }

    private static bool AppendLineIfNotNullOrEmpty(this StringBuilder sb, string textToAppend)
    {
      if (string.IsNullOrEmpty(textToAppend))
        return false;
      sb.AppendLine(textToAppend);
      return true;
    }

    private static string CompareAndGetDifferenceObjects<T>(
      Func<T, T, string> comparer,
      string objectType,
      IEnumerable<T> baseObjects,
      IEnumerable<T> toCompareObjects,
      Func<T, object> orderSelector = null)
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLineIfNotNullOrEmpty(AadObjectCompareHelpers.CompareAndGetDifferenceForCountOfIEnumerables<T>(baseObjects, toCompareObjects, objectType));
      if (baseObjects != null && toCompareObjects != null)
      {
        if (orderSelector != null)
        {
          baseObjects = (IEnumerable<T>) baseObjects.OrderBy<T, object>(orderSelector);
          toCompareObjects = (IEnumerable<T>) toCompareObjects.OrderBy<T, object>(orderSelector);
        }
        using (IEnumerator<T> enumerator1 = baseObjects.GetEnumerator())
        {
          using (IEnumerator<T> enumerator2 = toCompareObjects.GetEnumerator())
          {
            while (enumerator1.MoveNext())
            {
              if (enumerator2.MoveNext())
                sb.AppendLineIfNotNullOrEmpty(comparer(enumerator1.Current, enumerator2.Current));
              else
                break;
            }
          }
        }
      }
      return sb.Length <= 0 ? (string) null : sb.ToString();
    }
  }
}
