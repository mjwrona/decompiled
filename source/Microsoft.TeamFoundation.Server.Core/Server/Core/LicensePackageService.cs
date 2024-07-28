// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.LicensePackageService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class LicensePackageService : IVssFrameworkService
  {
    private LicensePackage m_package;
    private const string s_area = "TeamFoundationLicensing";
    private const string s_layer = "LicensePackageService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      systemRequestContext.GetService<TeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", SqlNotificationEventClasses.LicensingDataChanged, new SqlNotificationCallback(this.OnSettingsChanged), true);
      string serializedObject = systemRequestContext.GetService<CachedRegistryService>().GetValue<string>(systemRequestContext, (RegistryQuery) FrameworkServerConstants.LicensingPackageXmlPath, false, (string) null);
      if (string.IsNullOrEmpty(serializedObject))
        return;
      this.m_package = TeamFoundationSerializationUtility.Deserialize<LicensePackage>(serializedObject);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ImportPackage(IVssRequestContext requestContext, LicensePackage package)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<LicensePackage>(package, nameof (package));
      this.CheckWritePermission(requestContext);
      this.ValidatePackage(package);
      IVssRequestContext requestContext1 = requestContext.Elevate();
      string eventData = TeamFoundationSerializationUtility.SerializeToString<LicensePackage>(package);
      requestContext.GetService<CachedRegistryService>().SetValue<string>(requestContext1, FrameworkServerConstants.LicensingPackageXmlPath, eventData);
      this.m_package = package;
      this.OnLicensePackageChanged(requestContext1);
      requestContext.GetService<TeamFoundationSqlNotificationService>().SendNotification(requestContext1, SqlNotificationEventClasses.LicensingDataChanged, eventData);
    }

    public void ImportPackage(IVssRequestContext requestContext, string licensePackageXml)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(licensePackageXml, nameof (licensePackageXml));
      LicensePackage package = TeamFoundationSerializationUtility.Deserialize<LicensePackage>(licensePackageXml);
      this.ImportPackage(requestContext, package);
    }

    internal void ClearPackage(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.CheckWritePermission(requestContext);
      requestContext.GetService<CachedRegistryService>().DeleteEntries(requestContext, FrameworkServerConstants.LicensingPackageXmlPath);
      this.m_package = (LicensePackage) null;
      this.OnLicensePackageChanged(requestContext);
      requestContext.GetService<TeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.LicensingDataChanged, (string) null);
    }

    public void SetDefaultLicenseType(IVssRequestContext requestContext, Guid licenseTypeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(licenseTypeId, nameof (licenseTypeId));
      this.CheckWritePermission(requestContext);
      LicenseType licenseType1 = (LicenseType) null;
      LicenseType licenseType2 = (LicenseType) null;
      foreach (LicenseType licenseType3 in this.m_package.LicenseTypes)
      {
        if (licenseType3.IsDefault)
          licenseType1 = licenseType3;
        if (licenseType3.Id == licenseTypeId)
          licenseType2 = licenseType3;
      }
      if (licenseType2 == null)
        throw new ArgumentException(FrameworkResources.LicensingPackage_UnknownLicenseType((object) licenseTypeId));
      if (licenseType1 == licenseType2)
        return;
      licenseType1.IsDefault = false;
      licenseType2.IsDefault = true;
      this.ImportPackage(requestContext, this.m_package);
    }

    public virtual ILicensePackage GetLicensePackage(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.CheckReadPermission(requestContext);
      return (ILicensePackage) this.m_package;
    }

    public event EventHandler LicensePackageChanged;

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.Trace(66110, TraceLevel.Info, "TeamFoundationLicensing", nameof (LicensePackageService), "Received a setting change event with eventClass '{0}' and eventData '{1}'", (object) eventClass, (object) eventData);
      this.m_package = string.IsNullOrEmpty(eventData) ? (LicensePackage) null : TeamFoundationSerializationUtility.Deserialize<LicensePackage>(eventData);
      requestContext.Trace(66111, TraceLevel.Info, "TeamFoundationLicensing", nameof (LicensePackageService), "Deserialized eventData to package: {0}", this.m_package == null ? (object) "null" : (object) this.m_package.Serialize<LicensePackage>());
      this.OnLicensePackageChanged(requestContext);
    }

    private void OnLicensePackageChanged(IVssRequestContext requestContext)
    {
      EventHandler licensePackageChanged = this.LicensePackageChanged;
      if (licensePackageChanged == null)
        return;
      requestContext.Trace(66120, TraceLevel.Info, "TeamFoundationLicensing", nameof (LicensePackageService), "Invoking license package change event");
      licensePackageChanged((object) this, EventArgs.Empty);
    }

    private void ValidatePackage(LicensePackage package)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) package.Features, "LicensePackage.Features");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) package.LicenseTypes, "LicensePackage.LicenseTypes");
      HashSet<Guid> guidSet1 = new HashSet<Guid>();
      HashSet<Guid> guidSet2 = new HashSet<Guid>();
      for (int index = 0; index < package.Features.Length; ++index)
      {
        ArgumentUtility.CheckForEmptyGuid(package.Features[index].Id, "package.Features[" + index.ToString() + "].Id");
        ArgumentUtility.CheckStringForNullOrEmpty(package.Features[index].Name, "package.Features[" + index.ToString() + "].Name");
        if (guidSet1.Contains(package.Features[index].Id))
          throw new ArgumentException(FrameworkResources.LicensingPackageValidation_DuplicateFeatureOrLicenseType((object) package.Features[index].Name));
        guidSet1.Add(package.Features[index].Id);
      }
      ILicenseType licenseType = (ILicenseType) null;
      for (int index = 0; index < package.LicenseTypes.Length; ++index)
      {
        ArgumentUtility.CheckForEmptyGuid(package.LicenseTypes[index].Id, "package.LicenseTypes[" + index.ToString() + "].Id");
        ArgumentUtility.CheckStringForNullOrEmpty(package.LicenseTypes[index].Name, "package.LicenseTypes[" + index.ToString() + "].Name");
        if (guidSet2.Contains(package.LicenseTypes[index].Id))
          throw new ArgumentException(FrameworkResources.LicensingPackageValidation_DuplicateFeatureOrLicenseType((object) package.LicenseTypes[index].Name));
        if (package.LicenseTypes[index].IsDefault)
        {
          if (licenseType != null)
            throw new ArgumentException(FrameworkResources.LicensingPackageValidation_MultipleDefaultLicenseTypes());
          licenseType = (ILicenseType) package.LicenseTypes[index];
        }
        ArgumentUtility.CheckForNull<Guid[]>(package.LicenseTypes[index].Features, "package.LicenseTypes[" + index.ToString() + "].Features");
        HashSet<Guid> guidSet3 = new HashSet<Guid>();
        foreach (Guid feature in package.LicenseTypes[index].Features)
        {
          if (!guidSet1.Contains(feature))
            throw new ArgumentException(FrameworkResources.LicensingPackage_UnknownFeature((object) feature));
          if (guidSet3.Contains(feature))
            throw new ArgumentException(FrameworkResources.LicensingPackageValidation_DuplicateFeatureOrLicenseType((object) feature));
          guidSet3.Add(feature);
        }
        guidSet2.Add(package.LicenseTypes[index].Id);
      }
      if (licenseType == null)
        throw new ArgumentException(FrameworkResources.LicensingPackageValidation_MissingDefaultLicense());
    }

    private void CheckReadPermission(IVssRequestContext requestContext) => this.CheckPermission(requestContext, 1);

    private void CheckWritePermission(IVssRequestContext requestContext) => this.CheckPermission(requestContext, 2);

    private void CheckPermission(IVssRequestContext requestContext, int requestedPermission)
    {
      if (requestContext.IsServicingContext)
        return;
      requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, requestedPermission, false);
    }
  }
}
