// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationOnPremLicensingService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationOnPremLicensingService : TeamFoundationLicensingServiceBase
  {
    private Dictionary<Guid, IdentityDescriptor> m_licenseToLicenseGroup;
    private Dictionary<Guid, List<IdentityDescriptor>> m_featureToLicenseGroups;
    protected new static readonly string s_area = "TeamFoundationLicensing";
    protected new static readonly string s_layer = nameof (TeamFoundationOnPremLicensingService);

    public override bool IsFeatureSupported(
      IVssRequestContext requestContext,
      Guid featureId,
      IdentityDescriptor userContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(featureId, nameof (featureId));
      if (userContext == (IdentityDescriptor) null)
        return false;
      if (IdentityDescriptorComparer.Instance.Equals(userContext, requestContext.ServiceHost.SystemDescriptor()))
        return true;
      try
      {
        this.EnsureSettingsLoaded(requestContext);
        TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
        List<IdentityDescriptor> identityDescriptorList;
        if (this.m_featureToLicenseGroups.TryGetValue(featureId, out identityDescriptorList))
        {
          foreach (IdentityDescriptor identityDescriptor in identityDescriptorList)
          {
            if (service.IsMember(requestContext, identityDescriptor, userContext) || this.IsFeatureSupportedNoAdminLockout(requestContext, identityDescriptor, userContext))
              return true;
          }
          if (this.m_defaultFeatures.Contains(featureId))
          {
            foreach (IdentityDescriptor groupDescriptor in this.m_licenseToLicenseGroup.Values)
            {
              if (service.IsMember(requestContext, groupDescriptor, userContext))
              {
                requestContext.Trace(66003, TraceLevel.Warning, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, "User is already in a license group, so default is not used.");
                return false;
              }
            }
            return true;
          }
        }
        else
          requestContext.Trace(66002, TraceLevel.Warning, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, "Could not find license groups for feature {0}.", (object) featureId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(66010, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, ex);
      }
      return false;
    }

    public override bool IsFeatureInAdvertisingMode(
      IVssRequestContext requestContext,
      Guid featureId)
    {
      return false;
    }

    public override ILicenseType[] GetLicensesForUser(
      IVssRequestContext requestContext,
      IdentityDescriptor userDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(userDescriptor, nameof (userDescriptor));
      this.EnsureSettingsLoaded(requestContext);
      TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
      List<ILicenseType> licenses = new List<ILicenseType>();
      foreach (KeyValuePair<Guid, IdentityDescriptor> keyValuePair in this.m_licenseToLicenseGroup)
      {
        Guid key = keyValuePair.Key;
        IdentityDescriptor groupDescriptor = keyValuePair.Value;
        if (service.IsMember(requestContext, groupDescriptor, userDescriptor))
          licenses.Add(this.LocalizeLicenseType(this.m_licenseToLicenseType[key]));
      }
      if (licenses.Count == 0 && this.m_defaultLicense != null)
        licenses.Add(this.LocalizeLicenseType(this.m_defaultLicense));
      this.GetLicensesNoAdminLockout(requestContext, licenses, userDescriptor);
      return licenses.ToArray();
    }

    private bool IsFeatureSupportedNoAdminLockout(
      IVssRequestContext requestContext,
      IdentityDescriptor group,
      IdentityDescriptor user)
    {
      if (!requestContext.IsFeatureEnabled("Licensing.NoAdminLockout") || !IdentityDescriptorComparer.Instance.Equals(group, this.m_licenseToLicenseGroup[OnPremLicenseName.Standard]) || !TeamFoundationOnPremLicensingService.IsDeploymentAdmin(requestContext, user))
        return false;
      if (requestContext.IsTracing(66011, TraceLevel.Verbose, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer))
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Group={0}, User={1}, StackTrace={2}", (object) group, (object) user, (object) Environment.StackTrace);
        requestContext.Trace(66011, TraceLevel.Verbose, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, message);
      }
      return true;
    }

    private void GetLicensesNoAdminLockout(
      IVssRequestContext requestContext,
      List<ILicenseType> licenses,
      IdentityDescriptor user)
    {
      if (!requestContext.IsFeatureEnabled("Licensing.NoAdminLockout") || !licenses.All<ILicenseType>((Func<ILicenseType, bool>) (license => license.Id.Equals(OnPremLicenseName.Limited))) || !TeamFoundationOnPremLicensingService.IsDeploymentAdmin(requestContext, user))
        return;
      if (requestContext.IsTracing(66012, TraceLevel.Verbose, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer))
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "User={0}, StackTrace={1}", (object) user, (object) Environment.StackTrace);
        requestContext.Trace(66012, TraceLevel.Verbose, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, message);
      }
      ILicenseType licenseType;
      if (!this.m_licenseToLicenseType.TryGetValue(OnPremLicenseName.Standard, out licenseType))
        return;
      licenses.Add(this.LocalizeLicenseType(licenseType));
    }

    private static bool IsDeploymentAdmin(
      IVssRequestContext requestContext,
      IdentityDescriptor user)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IdentityService>().IsMember(vssRequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, user);
    }

    protected override void EnsureSettingsLoaded(IVssRequestContext requestContext)
    {
      if (this.m_settingsLoaded)
        return;
      lock (this.m_settingsLock)
      {
        if (this.m_settingsLoaded)
          return;
        requestContext = requestContext.Elevate();
        Dictionary<Guid, ILicenseFeature> dictionary1 = new Dictionary<Guid, ILicenseFeature>();
        Dictionary<Guid, ILicenseType> dictionary2 = new Dictionary<Guid, ILicenseType>();
        Dictionary<Guid, IdentityDescriptor> dictionary3 = new Dictionary<Guid, IdentityDescriptor>();
        Dictionary<Guid, List<IdentityDescriptor>> dictionary4 = new Dictionary<Guid, List<IdentityDescriptor>>();
        HashSet<Guid> guidSet1 = new HashSet<Guid>();
        HashSet<Guid> guidSet2 = new HashSet<Guid>();
        ILicenseType licenseType1 = (ILicenseType) null;
        try
        {
          ILicensePackage licensePackage = this.m_licensePackageService.GetLicensePackage(requestContext.To(TeamFoundationHostType.Deployment));
          if (licensePackage == null)
          {
            requestContext.Trace(66008, TraceLevel.Warning, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, "No license package found");
          }
          else
          {
            TeamFoundationLockingService service1 = requestContext.GetService<TeamFoundationLockingService>();
            TeamFoundationIdentityService service2 = requestContext.GetService<TeamFoundationIdentityService>();
            CachedRegistryService service3 = requestContext.GetService<CachedRegistryService>();
            string str1 = "-" + requestContext.ServiceHost.InstanceId.ToString("D");
            Guid guid1 = requestContext.ServiceHost.InstanceId;
            string str2 = string.Format("LicensingLock_{0}", (object) guid1.ToString("n"));
            IVssRequestContext requestContext1 = requestContext;
            string resource = str2;
            using (service1.AcquireLock(requestContext1, TeamFoundationLockMode.Exclusive, resource))
            {
              RegistryEntryCollection registryEntryCollection = service3.ReadEntries(requestContext, (RegistryQuery) (FrameworkServerConstants.LicensingRootPath + "/**"));
              dictionary1 = new Dictionary<Guid, ILicenseFeature>();
              foreach (ILicenseFeature feature in licensePackage.Features)
                dictionary1[feature.Id] = feature;
              dictionary2 = new Dictionary<Guid, ILicenseType>();
              foreach (ILicenseType licenseType2 in licensePackage.LicenseTypes)
              {
                dictionary2[licenseType2.Id] = licenseType2;
                string path = string.Format((IFormatProvider) CultureInfo.InvariantCulture, FrameworkServerConstants.LicensingGroupPath, (object) licenseType2.Id);
                Guid guid2 = registryEntryCollection[path].GetValue<Guid>();
                TeamFoundationIdentity foundationIdentity;
                if (guid2 == Guid.Empty)
                {
                  TeamFoundationIdentityService foundationIdentityService1 = service2;
                  IVssRequestContext requestContext2 = requestContext;
                  guid1 = licenseType2.Id;
                  string factorValue = guid1.ToString() + str1;
                  foundationIdentity = foundationIdentityService1.ReadIdentity(requestContext2, IdentitySearchFactor.AccountName, factorValue);
                  if (foundationIdentity == null)
                  {
                    TeamFoundationIdentityService foundationIdentityService2 = service2;
                    IVssRequestContext requestContext3 = requestContext;
                    guid1 = licenseType2.Id;
                    string groupName = guid1.ToString() + str1;
                    foundationIdentity = foundationIdentityService2.CreateApplicationGroup(requestContext3, (string) null, groupName, (string) null, false, true);
                  }
                  service3.SetValue<Guid>(requestContext, path, foundationIdentity.TeamFoundationId);
                }
                else
                {
                  foundationIdentity = service2.ReadIdentities(requestContext, new Guid[1]
                  {
                    guid2
                  })[0];
                  if (foundationIdentity == null)
                  {
                    requestContext.Trace(66004, TraceLevel.Warning, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, "Couldn't read identity with id {0}", (object) guid2);
                    continue;
                  }
                }
                foreach (Guid feature in licenseType2.Features)
                {
                  List<IdentityDescriptor> identityDescriptorList;
                  if (!dictionary4.TryGetValue(feature, out identityDescriptorList))
                  {
                    identityDescriptorList = new List<IdentityDescriptor>();
                    dictionary4.Add(feature, identityDescriptorList);
                  }
                  if (licenseType2.IsDefault)
                    guidSet1.Add(feature);
                  identityDescriptorList.Add(foundationIdentity.Descriptor);
                }
                if (licenseType2.IsDefault)
                  licenseType1 = licenseType2;
                dictionary3.Add(licenseType2.Id, foundationIdentity.Descriptor);
              }
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(60001, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, ex);
        }
        finally
        {
          this.m_featureToLicenseFeature = dictionary1;
          this.m_licenseToLicenseType = dictionary2;
          this.m_licenseToLicenseGroup = dictionary3;
          this.m_featureToLicenseGroups = dictionary4;
          this.m_defaultFeatures = guidSet1;
          this.m_previewFeatures = guidSet2;
          this.m_defaultLicense = licenseType1;
          this.m_settingsLoaded = true;
        }
      }
    }

    public void AddUserLicense(
      IVssRequestContext requestContext,
      Guid licenseTypeId,
      IdentityDescriptor userDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(licenseTypeId, nameof (licenseTypeId));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(userDescriptor, nameof (userDescriptor));
      this.CheckWritePermission(requestContext);
      this.EnsureSettingsLoaded(requestContext);
      TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
      IdentityDescriptor groupDescriptor;
      if (this.m_licenseToLicenseGroup.TryGetValue(licenseTypeId, out groupDescriptor))
      {
        try
        {
          service.AddMemberToApplicationGroup(requestContext, groupDescriptor, userDescriptor);
        }
        catch (AddMemberIdentityAlreadyMemberException ex)
        {
          requestContext.Trace(66009, TraceLevel.Info, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, "Tried to add a license {0} twice for {1} -- can probably ignore", (object) licenseTypeId, (object) userDescriptor.Identifier);
        }
      }
      else
      {
        requestContext.Trace(66005, TraceLevel.Warning, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, "No license group found for licenseType {0}", (object) licenseTypeId);
        throw new ArgumentException(FrameworkResources.LicensingPackage_UnknownLicenseType((object) licenseTypeId));
      }
    }

    public void RemoveUserLicense(
      IVssRequestContext requestContext,
      Guid licenseTypeId,
      IdentityDescriptor userDescriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(licenseTypeId, nameof (licenseTypeId));
      ArgumentUtility.CheckForNull<IdentityDescriptor>(userDescriptor, nameof (userDescriptor));
      this.CheckWritePermission(requestContext);
      this.EnsureSettingsLoaded(requestContext);
      TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
      IdentityDescriptor groupDescriptor;
      if (this.m_licenseToLicenseGroup.TryGetValue(licenseTypeId, out groupDescriptor))
      {
        try
        {
          service.RemoveMemberFromApplicationGroup(requestContext, groupDescriptor, userDescriptor);
        }
        catch (RemoveGroupMemberNotMemberException ex)
        {
          requestContext.Trace(66009, TraceLevel.Info, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, "Tried to remove a license {0} twice for {1} -- can probably ignore", (object) licenseTypeId, (object) userDescriptor.Identifier);
        }
      }
      else
      {
        requestContext.Trace(66006, TraceLevel.Warning, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, "No license group found for licenseType {0}", (object) licenseTypeId);
        throw new ArgumentException(FrameworkResources.LicensingPackage_UnknownLicenseType((object) licenseTypeId));
      }
    }

    internal TeamFoundationIdentity[] GetLicenseGroups(
      IVssRequestContext requestContext,
      Guid[] licenseTypeIds,
      MembershipQuery membershipQuery)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Guid[]>(licenseTypeIds, nameof (licenseTypeIds));
      this.CheckReadPermission(requestContext);
      this.EnsureSettingsLoaded(requestContext);
      IdentityDescriptor[] descriptors = new IdentityDescriptor[licenseTypeIds.Length];
      for (int index = 0; index < licenseTypeIds.Length; ++index)
      {
        Guid licenseTypeId = licenseTypeIds[index];
        if (this.m_licenseToLicenseGroup.ContainsKey(licenseTypeId))
          descriptors[index] = this.m_licenseToLicenseGroup[licenseTypeId];
        else
          requestContext.Trace(66005, TraceLevel.Warning, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, "No license type found for licenseType {0}", (object) licenseTypeId);
      }
      return requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext.Elevate(), descriptors, membershipQuery, ReadIdentityOptions.None, (IEnumerable<string>) null);
    }

    internal TeamFoundationIdentity GetLicenseGroup(
      IVssRequestContext requestContext,
      Guid licenseTypeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(licenseTypeId, nameof (licenseTypeId));
      this.CheckReadPermission(requestContext);
      TeamFoundationIdentity licenseGroup = this.GetLicenseGroups(requestContext, new Guid[1]
      {
        licenseTypeId
      }, MembershipQuery.None)[0];
      if (licenseGroup != null)
        return licenseGroup;
      requestContext.Trace(66005, TraceLevel.Warning, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, "No license type found for licenseType {0}", (object) licenseTypeId);
      throw new ArgumentException(FrameworkResources.LicensingPackage_UnknownLicenseType((object) licenseTypeId));
    }

    internal ICollection<IdentityDescriptor> GetLicenseUsers(
      IVssRequestContext requestContext,
      Guid licenseTypeId,
      MembershipQuery membershipQuery)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(licenseTypeId, nameof (licenseTypeId));
      this.CheckReadPermission(requestContext);
      this.EnsureSettingsLoaded(requestContext);
      TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
      IdentityDescriptor descriptor;
      if (this.m_licenseToLicenseGroup.TryGetValue(licenseTypeId, out descriptor))
        return service.ReadIdentity(requestContext.Elevate(), descriptor, membershipQuery, ReadIdentityOptions.None).Members;
      requestContext.Trace(66006, TraceLevel.Warning, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, "No license group found for licenseType {0}", (object) licenseTypeId);
      throw new ArgumentException(FrameworkResources.LicensingPackage_UnknownLicenseType((object) licenseTypeId));
    }

    internal Dictionary<IdentityDescriptor, int> ExportUserLicenses(
      IVssRequestContext requestContext,
      out ILicenseType[] licenseTypes)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      licenseTypes = this.GetAllLicenses(requestContext);
      Dictionary<IdentityDescriptor, int> dictionary = new Dictionary<IdentityDescriptor, int>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      for (int index = 0; index < licenseTypes.Length; ++index)
      {
        foreach (IdentityDescriptor licenseUser in (IEnumerable<IdentityDescriptor>) this.GetLicenseUsers(requestContext, licenseTypes[index].Id, MembershipQuery.Expanded))
        {
          if (!string.Equals(licenseUser.IdentityType, "Microsoft.TeamFoundation.Identity"))
          {
            if (!dictionary.ContainsKey(licenseUser))
              dictionary.Add(licenseUser, 0);
            dictionary[licenseUser] |= 1 << index;
          }
        }
      }
      return dictionary;
    }

    internal bool DeleteLicensingData(IVssRequestContext requestContext, ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (logger == null)
        logger = (ITFLogger) new ServerTraceLogger();
      if (requestContext.ServiceHost.IsOnly(TeamFoundationHostType.Application))
        this.m_licensePackageService.LicensePackageChanged -= new EventHandler(((TeamFoundationLicensingServiceBase) this).OnLicensePackageChanged);
      TeamFoundationIdentityService service1 = requestContext.GetService<TeamFoundationIdentityService>();
      CachedRegistryService service2 = requestContext.GetService<CachedRegistryService>();
      string str = "-" + requestContext.ServiceHost.InstanceId.ToString("D");
      int num = 0;
      foreach (RegistryEntry readEntry in service2.ReadEntries(requestContext, (RegistryQuery) (FrameworkServerConstants.LicensingRootPath + "/**")))
      {
        if (readEntry.Path.EndsWith("/Group", StringComparison.OrdinalIgnoreCase))
        {
          Guid guid = readEntry.GetValue<Guid>();
          TeamFoundationIdentity readIdentity = service1.ReadIdentities(requestContext, new Guid[1]
          {
            guid
          })[0];
          if (readIdentity != null)
          {
            if (readIdentity.DisplayName.EndsWith(str, StringComparison.OrdinalIgnoreCase))
            {
              if (readIdentity.IsActive)
              {
                service1.DeleteApplicationGroup(requestContext, readIdentity.Descriptor);
                ++num;
              }
            }
            else
              requestContext.Trace(0, TraceLevel.Warning, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, "Possible licensing group {0} doesn't end with expected suffix {1}.", (object) readIdentity.DisplayName, (object) str);
          }
        }
      }
      service2.DeleteEntries(requestContext, FrameworkServerConstants.LicensingRootPath + "/**");
      ILicensePackage licensePackage = this.m_licensePackageService.GetLicensePackage(requestContext.To(TeamFoundationHostType.Deployment));
      if (licensePackage != null)
      {
        foreach (ILicenseType licenseType in licensePackage.LicenseTypes)
        {
          TeamFoundationIdentityService foundationIdentityService = service1;
          IVssRequestContext requestContext1 = requestContext;
          string[] factorValues = new string[1]
          {
            licenseType.Id.ToString() + str
          };
          foreach (TeamFoundationIdentity foundationIdentity in foundationIdentityService.ReadIdentities(requestContext1, IdentitySearchFactor.AccountName, factorValues)[0])
          {
            if (foundationIdentity != null && foundationIdentity.IsActive)
            {
              service1.DeleteApplicationGroup(requestContext, foundationIdentity.Descriptor);
              ++num;
            }
          }
        }
      }
      else
      {
        requestContext.Trace(0, TraceLevel.Warning, TeamFoundationOnPremLicensingService.s_area, TeamFoundationOnPremLicensingService.s_layer, "No license package found");
        logger.Warning("No license package found.");
      }
      logger.Info("{0} licensing application groups deleted.", (object) num);
      return num > 0;
    }
  }
}
