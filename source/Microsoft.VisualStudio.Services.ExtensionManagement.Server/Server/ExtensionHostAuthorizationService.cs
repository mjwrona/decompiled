// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ExtensionHostAuthorizationService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  public class ExtensionHostAuthorizationService : 
    IExtensionHostAuthorizationService,
    IVssFrameworkService
  {
    private const string c_hostAuthorizationIdsMoniker = "HostAuthorizationIds";
    private const string c_hostAuthorizationIdItem = "$HostAuthorizationId";
    private ConcurrentDictionary<string, Guid> m_hostAuthorizationIdsCache;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      this.m_hostAuthorizationIdsCache = new ConcurrentDictionary<string, Guid>();
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", ExtensionManagementSqlNotificationClasses.InstalledExtensionChanged, new SqlNotificationHandler(this.OnExtensionChanged), requestContext.ExecutionEnvironment.IsHostedDeployment);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", ExtensionManagementSqlNotificationClasses.InstalledExtensionChanged, new SqlNotificationHandler(this.OnExtensionChanged), false);

    public void SetHostAuthorization(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid hostAuthorizationId)
    {
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      List<PropertyValue> propertyValueList1 = new List<PropertyValue>();
      ArtifactSpec artifactSpec1 = new ArtifactSpec(ExtensionManagementPropertyServiceConstants.ExtensionDataPropertiesArtifactKind, "HostAuthorizationIds", 1);
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      propertyValueList1.Add(new PropertyValue(fullyQualifiedName, (object) hostAuthorizationId.ToString()));
      IVssRequestContext requestContext1 = requestContext;
      ArtifactSpec artifactSpec2 = artifactSpec1;
      List<PropertyValue> propertyValueList2 = propertyValueList1;
      service.SetProperties(requestContext1, artifactSpec2, (IEnumerable<PropertyValue>) propertyValueList2);
      this.m_hostAuthorizationIdsCache[fullyQualifiedName] = hostAuthorizationId;
    }

    public void RemoveHostAuthorization(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      ITeamFoundationPropertyService service = requestContext.GetService<ITeamFoundationPropertyService>();
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      IVssRequestContext requestContext1 = requestContext;
      Guid propertiesArtifactKind = ExtensionManagementPropertyServiceConstants.ExtensionDataPropertiesArtifactKind;
      List<string> propertyNames = new List<string>();
      propertyNames.Add(fullyQualifiedName);
      int? maxPropertiesToDelete = new int?();
      service.DeleteProperties(requestContext1, propertiesArtifactKind, (IEnumerable<string>) propertyNames, maxPropertiesToDelete: maxPropertiesToDelete);
      this.m_hostAuthorizationIdsCache.Clear();
    }

    public bool IsRequestAuthorized(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      return this.GetHostAuthorizationIdFromRequest(requestContext) == this.GetHostAuthorizationIdFromExtension(requestContext, publisherName, extensionName);
    }

    private Guid GetHostAuthorizationIdFromExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      Guid authorizationIdFromExtension1;
      if (this.m_hostAuthorizationIdsCache.TryGetValue(fullyQualifiedName, out authorizationIdFromExtension1))
        return authorizationIdFromExtension1;
      ITeamFoundationPropertyService service1 = requestContext.GetService<ITeamFoundationPropertyService>();
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      IVssRequestContext requestContext1 = requestContext;
      ArtifactSpec[] artifactSpecArray = new ArtifactSpec[1]
      {
        new ArtifactSpec(ExtensionManagementPropertyServiceConstants.ExtensionDataPropertiesArtifactKind, "HostAuthorizationIds", 1)
      };
      using (TeamFoundationDataReader properties = service1.GetProperties(requestContext1, (IEnumerable<ArtifactSpec>) artifactSpecArray, (IEnumerable<string>) null))
      {
        foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
        {
          foreach (PropertyValue propertyValue in current.PropertyValues)
          {
            if (propertyValue != null)
            {
              KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(propertyValue.PropertyName, propertyValue.Value == null ? (string) null : propertyValue.Value.ToString());
              if (keyValuePair.Key == fullyQualifiedName)
              {
                Guid authorizationIdFromExtension2 = Guid.Parse(keyValuePair.Value);
                this.m_hostAuthorizationIdsCache[fullyQualifiedName] = authorizationIdFromExtension2;
                return authorizationIdFromExtension2;
              }
            }
          }
        }
      }
      IVssRequestContext context = requestContext.Elevate();
      IDelegatedAuthorizationService service2 = context.GetService<IDelegatedAuthorizationService>();
      Guid registrationIdForExtension = this.GetRegistrationIdForExtension(requestContext, publisherName, extensionName);
      IVssRequestContext requestContext2 = context;
      Guid clientId = registrationIdForExtension;
      Guid? newId = new Guid?();
      HostAuthorizationDecision authorizationDecision = service2.AuthorizeHost(requestContext2, clientId, newId);
      if (authorizationDecision == null)
        return Guid.Empty;
      Guid hostAuthorizationId = authorizationDecision.HostAuthorizationId;
      this.SetHostAuthorization(requestContext, publisherName, extensionName, hostAuthorizationId);
      return hostAuthorizationId;
    }

    private Guid GetHostAuthorizationIdFromRequest(IVssRequestContext requestContext)
    {
      object obj = (object) null;
      if (!requestContext.Items.TryGetValue("$HostAuthorizationId", out obj))
        throw new TokenMissingHostAuthorizationClaimException();
      return Guid.Parse(obj.ToString());
    }

    private Guid GetRegistrationIdForExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string token = requestContext.GetService<IAccountTokenService>().GetToken(requestContext);
      PublishedExtension extension = vssRequestContext.GetService<IGalleryService>().GetExtension(vssRequestContext, publisherName, extensionName, (string) null, ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeVersionProperties, token);
      InstalledExtension installedExtension = requestContext.GetService<IInstalledExtensionService>().GetInstalledExtension(requestContext, publisherName, extensionName);
      Guid result;
      return installedExtension != null && Guid.TryParse(extension.GetProperty(installedExtension.Version, "RegistrationId"), out result) && result != Guid.Empty ? result : Guid.Empty;
    }

    private void OnExtensionChanged(IVssRequestContext requestContext, NotificationEventArgs args) => this.m_hostAuthorizationIdsCache.Clear();
  }
}
