// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Remote.ExtensionManagement.ExtensionManagementClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Remote.ExtensionManagement
{
  internal class ExtensionManagementClient : IExtensionManagementClient, IDisposable
  {
    private readonly WrappedHttpClient<ExtensionManagementHttpClient> _wrappedEmsClient;
    private readonly GalleryKPIHelpers _kpiHelpers;

    public ExtensionManagementClient(
      WrappedHttpClient<ExtensionManagementHttpClient> wrappedEmsClient,
      GalleryKPIHelpers kpiHelpers)
    {
      this._wrappedEmsClient = wrappedEmsClient;
      this._kpiHelpers = kpiHelpers;
    }

    public RequestedExtension RequestExtensionSync(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string requestMessage,
      object userState = null)
    {
      RequestedExtension requestedExtension;
      try
      {
        requestedExtension = this._wrappedEmsClient.Client.RequestExtensionAsync(publisherName, extensionName, requestMessage, userState).SyncResult<RequestedExtension>();
      }
      catch (Exception ex)
      {
        this._kpiHelpers.LogS2SKPI(requestContext, "S2SFailureEMS");
        requestContext.TraceException(12060030, "GalleryKPIHelpers", "Service", ex);
        throw;
      }
      this._kpiHelpers.LogS2SKPI(requestContext, "S2SSuccessEMS");
      return requestedExtension;
    }

    public InstalledExtension InstallExtensionByNameSync(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      object userState = null)
    {
      InstalledExtension installedExtension;
      try
      {
        installedExtension = this._wrappedEmsClient.Client.InstallExtensionByNameAsync(publisherName, extensionName, userState: userState).SyncResult<InstalledExtension>();
      }
      catch (Exception ex)
      {
        this._kpiHelpers.LogS2SKPI(requestContext, "S2SFailureEMS");
        requestContext.TraceException(12060030, "GalleryKPIHelpers", "Service", ex);
        throw;
      }
      this._kpiHelpers.LogS2SKPI(requestContext, "S2SSuccessEMS");
      return installedExtension;
    }

    public InstalledExtension GetInstalledExtensionSync(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      InstalledExtension installedExtensionSync;
      try
      {
        installedExtensionSync = this._wrappedEmsClient.Client.GetInstalledExtensionByNameAsync(publisherName, extensionName).SyncResult<InstalledExtension>();
      }
      catch (InstalledExtensionNotFoundException ex)
      {
        this._kpiHelpers.LogS2SKPI(requestContext, "S2SSuccessEMS");
        throw;
      }
      catch (Exception ex)
      {
        this._kpiHelpers.LogS2SKPI(requestContext, "S2SFailureEMS");
        requestContext.TraceException(12060030, "GalleryKPIHelpers", "Service", ex);
        throw;
      }
      this._kpiHelpers.LogS2SKPI(requestContext, "S2SSuccessEMS");
      return installedExtensionSync;
    }

    public UserExtensionPolicy GetPoliciesSync(
      IVssRequestContext requestContext,
      string userId,
      object userState = null)
    {
      UserExtensionPolicy policiesSync;
      try
      {
        policiesSync = this._wrappedEmsClient.Client.GetPoliciesAsync(userId, userState).SyncResult<UserExtensionPolicy>();
      }
      catch (Exception ex)
      {
        this._kpiHelpers.LogS2SKPI(requestContext, "S2SFailureEMS");
        requestContext.TraceException(12060030, "GalleryKPIHelpers", "Service", ex);
        throw;
      }
      this._kpiHelpers.LogS2SKPI(requestContext, "S2SSuccessEMS");
      return policiesSync;
    }

    public List<RequestedExtension> GetRequestsSync(
      IVssRequestContext requestContext,
      object userState = null)
    {
      List<RequestedExtension> requestsSync;
      try
      {
        requestsSync = this._wrappedEmsClient.Client.GetRequestsAsync(userState).SyncResult<List<RequestedExtension>>();
      }
      catch (Exception ex)
      {
        this._kpiHelpers.LogS2SKPI(requestContext, "S2SFailureEMS");
        requestContext.TraceException(12060030, "GalleryKPIHelpers", "Service", ex);
        throw;
      }
      this._kpiHelpers.LogS2SKPI(requestContext, "S2SSuccessEMS");
      return requestsSync;
    }

    public AcquisitionOptions GetAcquisitionOptionsSync(
      IVssRequestContext requestContext,
      string itemName,
      bool isAccountOwner,
      bool isLinked)
    {
      AcquisitionOptions acquisitionOptionsSync;
      try
      {
        acquisitionOptionsSync = this._wrappedEmsClient.Client.GetAcquisitionOptionsAsync(itemName, new bool?(false), new bool?(true), new bool?(isAccountOwner), new bool?(isLinked), new bool?(false), new bool?(true)).SyncResult<AcquisitionOptions>();
      }
      catch (Exception ex)
      {
        this._kpiHelpers.LogS2SKPI(requestContext, "S2SFailureEMS");
        requestContext.TraceException(12060030, "GalleryKPIHelpers", "Service", ex);
        throw;
      }
      this._kpiHelpers.LogS2SKPI(requestContext, "S2SSuccessEMS");
      return acquisitionOptionsSync;
    }

    public void Dispose() => this._wrappedEmsClient.Dispose();
  }
}
