// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.IdentityManagementHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Admin;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class IdentityManagementHelpers
  {
    private static int s_readFilteredIdentitiesPageSize = 100;
    private static int s_maxReadFilteredIdentitiesPageSize = 1000;
    private const int WebAccessExceptionEaten = 599999;

    public static T GetIdentityViewModel<T>(TeamFoundationIdentity identity, bool isTeam = false) where T : IdentityViewModelBase => IdentityManagementHelpers.GetIdentityViewModel(identity, isTeam) as T;

    public static IdentityViewModelBase GetIdentityViewModel(
      TeamFoundationIdentity identity,
      bool isTeam = false)
    {
      return IdentityImageUtility.GetIdentityViewModel(identity, isTeam: isTeam);
    }

    public static JsObject BuildFilteredIdentitiesJsonViewModel(
      IEnumerable<TeamFoundationIdentity> filteredIdentities,
      bool hasMoreItems,
      int totalItems)
    {
      IEnumerable<JsObject> jsObjects = Enumerable.Empty<JsObject>();
      if (filteredIdentities != null)
        jsObjects = filteredIdentities.Select<TeamFoundationIdentity, IdentityViewModelBase>((Func<TeamFoundationIdentity, IdentityViewModelBase>) (s => IdentityManagementHelpers.GetIdentityViewModel(s))).Select<IdentityViewModelBase, JsObject>((Func<IdentityViewModelBase, JsObject>) (s => s.ToJson()));
      JsObject jsObject = new JsObject();
      jsObject.Add("identities", (object) jsObjects);
      jsObject.Add("hasMore", (object) hasMoreItems);
      jsObject.Add("totalIdentityCount", (object) totalItems);
      return jsObject;
    }

    public static JsObject BuildFilteredIdentitiesJsonViewModel(
      TeamFoundationFilteredIdentitiesList filteredIdentities)
    {
      IEnumerable<JsObject> jsObjects = Enumerable.Empty<JsObject>();
      if (filteredIdentities.Items != null)
        jsObjects = ((IEnumerable<TeamFoundationIdentity>) filteredIdentities.Items).Select<TeamFoundationIdentity, IdentityViewModelBase>((Func<TeamFoundationIdentity, IdentityViewModelBase>) (s => IdentityManagementHelpers.GetIdentityViewModel(s))).Select<IdentityViewModelBase, JsObject>((Func<IdentityViewModelBase, JsObject>) (s => s.ToJson()));
      JsObject jsObject = new JsObject();
      jsObject.Add("identities", (object) jsObjects);
      jsObject.Add("hasMore", (object) filteredIdentities.HasMoreItems);
      jsObject.Add("totalIdentityCount", (object) filteredIdentities.TotalItems);
      return jsObject;
    }

    public static int GetPageSize(int? pageSize)
    {
      ref int? local = ref pageSize;
      int? nullable = pageSize;
      int num = nullable ?? IdentityManagementHelpers.s_readFilteredIdentitiesPageSize;
      local = new int?(num);
      nullable = pageSize;
      int identitiesPageSize = IdentityManagementHelpers.s_maxReadFilteredIdentitiesPageSize;
      return !(nullable.GetValueOrDefault() > identitiesPageSize & nullable.HasValue) ? pageSize.Value : IdentityManagementHelpers.s_maxReadFilteredIdentitiesPageSize;
    }

    public static MembershipModel GetMembershipModel(
      ITfsController tfsController,
      string newUsersJson,
      string existingUsersJson,
      out IList<TeamFoundationIdentity> existingIdentities,
      out IList<TeamFoundationIdentity> newUsersIdentities)
    {
      Guid[] tfidsJson = IdentityManagementHelpers.ParseTfidsJson(existingUsersJson);
      existingIdentities = IdentityManagementHelpers.ResolveIdentities(tfsController, tfidsJson);
      MembershipModel membershipModel = new MembershipModel();
      membershipModel.EditMembers = false;
      newUsersIdentities = IdentityManagementHelpers.ParseNewUsersJson(tfsController, newUsersJson, membershipModel);
      return membershipModel;
    }

    public static Guid[] ParseTfidsJson(string tfidsJson)
    {
      try
      {
        return string.IsNullOrWhiteSpace(tfidsJson) ? Array.Empty<Guid>() : JsonExtensions.FromJson<Guid[]>(tfidsJson);
      }
      catch (FormatException ex)
      {
        throw new ArgumentException(nameof (ParseTfidsJson));
      }
    }

    public static IList<TeamFoundationIdentity> ResolveIdentities(
      ITfsController tfsController,
      Guid[] Ids)
    {
      TeamFoundationIdentity[] foundationIdentityArray1 = tfsController.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(tfsController.TfsRequestContext, Ids);
      IList<Guid> source = (IList<Guid>) new List<Guid>();
      IDictionary<Guid, int> dictionary = (IDictionary<Guid, int>) new Dictionary<Guid, int>();
      for (int index = 0; index < foundationIdentityArray1.Length; ++index)
      {
        if (foundationIdentityArray1[index] == null)
        {
          source.Add(Ids[index]);
          dictionary.Add(Ids[index], index);
        }
      }
      if (source.Any<Guid>())
      {
        if (!tfsController.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new IdentityNotFoundException(source.First<Guid>());
        IVssRequestContext vssRequestContext = tfsController.TfsRequestContext.To(TeamFoundationHostType.Application);
        TeamFoundationIdentity[] foundationIdentityArray2 = vssRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(vssRequestContext, source.ToArray<Guid>(), MembershipQuery.None, ReadIdentityOptions.TrueSid, (IEnumerable<string>) null);
        for (int index1 = 0; index1 < foundationIdentityArray2.Length; ++index1)
        {
          if (foundationIdentityArray2[index1] == null)
            throw new IdentityNotFoundException(source[index1]);
          int index2 = dictionary[source[index1]];
          foundationIdentityArray1[index2] = foundationIdentityArray2[index1];
        }
      }
      return (IList<TeamFoundationIdentity>) foundationIdentityArray1;
    }

    public static IList<TeamFoundationIdentity> ParseNewUsersJson(
      ITfsController tfsController,
      string newUsersJson,
      MembershipModel membershipModel)
    {
      try
      {
        string[] newUsers = !tfsController.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? (string.IsNullOrWhiteSpace(newUsersJson) ? Array.Empty<string>() : new JavaScriptSerializer().Deserialize<string[]>(newUsersJson)) : (string.IsNullOrWhiteSpace(newUsersJson) ? Array.Empty<string>() : JsonConvert.DeserializeObject<string[]>(newUsersJson));
        return (IList<TeamFoundationIdentity>) IdentityManagementHelpers.ProcessNewUsers(tfsController, newUsers, membershipModel);
      }
      catch (FormatException ex)
      {
        throw new ArgumentException(nameof (newUsersJson));
      }
    }

    private static string GetUserDomain(ITfsController tfsController)
    {
      Guid organizationAadTenantId = tfsController.TfsRequestContext.GetOrganizationAadTenantId();
      return !organizationAadTenantId.Equals(Guid.Empty) ? organizationAadTenantId.ToString() : "Windows Live ID";
    }

    public static List<TeamFoundationIdentity> ProcessNewUsers(
      ITfsController tfsController,
      string[] newUsers,
      MembershipModel membershipModel)
    {
      ArgumentUtility.CheckForNull<string[]>(newUsers, nameof (newUsers));
      IVssRequestContext vssRequestContext = tfsController.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationIdentityService service = vssRequestContext.GetService<TeamFoundationIdentityService>();
      List<TeamFoundationIdentity> foundationIdentityList = new List<TeamFoundationIdentity>(newUsers.Length);
      foreach (string newUser in newUsers)
      {
        string str = string.IsNullOrEmpty(newUser) ? newUser : newUser.Trim();
        TeamFoundationIdentity foundationIdentity;
        if (tfsController.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          if (ArgumentUtility.IsValidEmailAddress(str))
          {
            string userDomain = IdentityManagementHelpers.GetUserDomain(tfsController);
            foundationIdentity = IdentityUtil.Convert(IdentityHelper.GetOrCreateBindPendingIdentity(tfsController.TfsRequestContext, userDomain, str, callerName: nameof (ProcessNewUsers)));
          }
          else
          {
            UserIdentityViewModel identityViewModel = new UserIdentityViewModel();
            identityViewModel.DisplayName = str;
            identityViewModel.Errors.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.InvalidEmailWithEmail, (object) str));
            membershipModel.FailedAddedIdentities.Add((IdentityViewModelBase) identityViewModel);
            continue;
          }
        }
        else
        {
          foundationIdentity = (TeamFoundationIdentity) null;
          try
          {
            foundationIdentity = service.ReadIdentity(vssRequestContext, IdentitySearchFactor.AccountName, str, MembershipQuery.None, ReadIdentityOptions.IncludeReadFromSource, (IEnumerable<string>) null);
          }
          catch (TeamFoundationServerException ex)
          {
            tfsController.TraceException(599999, (Exception) ex);
          }
          if (foundationIdentity == null)
            foundationIdentity = service.ReadIdentity(vssRequestContext, IdentitySearchFactor.DisplayName, str, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null);
          if (foundationIdentity == null || foundationIdentity.IsContainer && string.Equals(foundationIdentity.Descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
          {
            UserIdentityViewModel identityViewModel = new UserIdentityViewModel();
            identityViewModel.DisplayName = newUser;
            identityViewModel.Errors.Add(string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.UnableToFindWindowsIdentity, (object) str));
            membershipModel.FailedAddedIdentities.Add((IdentityViewModelBase) identityViewModel);
            continue;
          }
        }
        foundationIdentityList.Add(foundationIdentity);
      }
      return foundationIdentityList;
    }
  }
}
