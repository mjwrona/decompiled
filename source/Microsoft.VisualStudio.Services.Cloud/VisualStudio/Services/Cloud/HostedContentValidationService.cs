// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostedContentValidationService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.ContentValidation;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal sealed class HostedContentValidationService : 
    IContentValidationService,
    IVssFrameworkService
  {
    private string m_regionName;
    private HostedContentValidationService.CvsWrapper m_cvs;

    public bool IsEnabled(IVssRequestContext rc, string regionName)
    {
      string regionName1;
      if (rc.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        ArgumentUtility.CheckStringForNullOrEmpty(regionName, nameof (regionName));
        regionName1 = regionName;
      }
      else
      {
        if (!rc.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          throw new UnexpectedHostTypeException(rc.ServiceHost.HostType);
        if (regionName != null)
          throw new ArgumentException("regionName can only be specified at the deployment level.");
        regionName1 = this.m_regionName;
      }
      return this.m_cvs.InitIfEnabledForRegion(rc, regionName1);
    }

    public void ServiceStart(IVssRequestContext systemRc)
    {
      if (systemRc.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        this.m_regionName = systemRc.GetService<IHostRegionService>().GetHostRegion(systemRc);
      this.m_cvs = systemRc.To(TeamFoundationHostType.Deployment).GetService<HostedContentValidationService.CvsWrapper>();
    }

    public void ServiceEnd(IVssRequestContext systemRc)
    {
    }

    public Task SubmitAsync(
      IVssRequestContext rc,
      Guid projectId,
      IEnumerable<ContentValidationKey> toScan,
      Microsoft.VisualStudio.Services.Identity.Identity contentCreator,
      string creatorIpAddress)
    {
      return this.m_cvs.SubmitAsync(rc, projectId, this.m_regionName, toScan, contentCreator, creatorIpAddress);
    }

    public Task SubmitAsync(
      IVssRequestContext rc,
      Guid projectId,
      string base64EncodedContent,
      ContentValidationScanType scanType,
      string fileName,
      Microsoft.VisualStudio.Services.Identity.Identity contentCreator,
      string creatorIpAddress)
    {
      return this.m_cvs.SubmitAsync(rc, projectId, this.m_regionName, base64EncodedContent, scanType, fileName, contentCreator, creatorIpAddress);
    }

    public Task SubmitAsync(
      IVssRequestContext rc,
      Guid projectId,
      byte[] content,
      Microsoft.VisualStudio.Services.Identity.Identity contentCreator,
      string creatorIpAddress,
      string fileName,
      ContentValidationScanType? scanType)
    {
      return this.m_cvs.SubmitAsync(rc, projectId, this.m_regionName, content, contentCreator, creatorIpAddress, fileName, scanType);
    }

    public Task SubmitProfileImageAsync(
      IVssRequestContext deploymentRc,
      byte[] profileImageContent,
      Microsoft.VisualStudio.Services.Identity.Identity profileIdentity,
      string lastUsedIpAddress,
      string regionName)
    {
      return this.m_cvs.SubmitImageAsync(deploymentRc, profileImageContent, profileIdentity, lastUsedIpAddress, regionName, ContentValidationTakedownTarget.UserProfileImage);
    }

    public Task SubmitImageAsync(
      IVssRequestContext deploymentRc,
      byte[] imageContent,
      Microsoft.VisualStudio.Services.Identity.Identity contentAuthor,
      string lastUsedIpAddress,
      string regionName,
      ContentValidationTakedownTarget takedownTarget)
    {
      return this.m_cvs.SubmitImageAsync(deploymentRc, imageContent, contentAuthor, lastUsedIpAddress, regionName, takedownTarget);
    }

    internal sealed class CvsWrapper : IVssFrameworkService
    {
      private ContentValidator m_validator;
      private static int s_handlersCreated;

      public void ServiceStart(IVssRequestContext systemRc)
      {
        systemRc.CheckServiceHostType(TeamFoundationHostType.Deployment);
        systemRc.GetService<IVssRegistryService>().RegisterNotification(systemRc, new RegistrySettingsChangedCallback(this.OnRegistryChanged), true, (IEnumerable<RegistryQuery>) new RegistryQuery[1]
        {
          HostedContentValidationConstants.CvsApiEndpointQuery
        });
      }

      public void ServiceEnd(IVssRequestContext systemRc)
      {
        systemRc.CheckServiceHostType(TeamFoundationHostType.Deployment);
        if (this.m_validator != null)
        {
          this.m_validator.Dispose();
          this.m_validator = (ContentValidator) null;
        }
        systemRc.GetService<IVssRegistryService>().UnregisterNotification(systemRc, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      }

      private void OnRegistryChanged(IVssRequestContext rc, RegistryEntryCollection changedEntries) => this.DoInitNewValidator(rc, this.m_validator);

      public bool InitIfEnabledForRegion(IVssRequestContext rc, string regionName)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(regionName, nameof (regionName));
        bool flag = false;
        if (rc.IsFeatureEnabled("VisualStudio.Services.SubmitToContentValidationService"))
        {
          flag = ContentValidationRegions.IsEnabled(regionName);
          if (flag && this.m_validator == null)
            this.DoInitNewValidator(rc.To(TeamFoundationHostType.Deployment).Elevate(), (ContentValidator) null);
        }
        rc.Trace(15289008, TraceLevel.Info, nameof (CvsWrapper), nameof (InitIfEnabledForRegion), "{0} enabled for region {1}? {2}", (object) nameof (CvsWrapper), (object) regionName, (object) flag);
        return flag;
      }

      private void DoInitNewValidator(
        IVssRequestContext sysDeploymentRc,
        ContentValidator expectedPrevValidator)
      {
        ContentValidator contentValidator = new ContentValidator(sysDeploymentRc, sysDeploymentRc.GetService<ILocationService>(), sysDeploymentRc.GetService<IVssRegistryService>(), sysDeploymentRc.GetService<ITeamFoundationStrongBoxService>(), sysDeploymentRc.GetService<IInstanceManagementService>(), (Func<WebRequestHandler>) (() => (WebRequestHandler) new LoggingWebRequestHandler("ContentValidator_" + (++HostedContentValidationService.CvsWrapper.s_handlersCreated).ToString())), 1000);
        if (Interlocked.CompareExchange<ContentValidator>(ref this.m_validator, contentValidator, expectedPrevValidator) == expectedPrevValidator)
          return;
        contentValidator.Dispose();
      }

      public Task SubmitAsync(
        IVssRequestContext rc,
        Guid projectId,
        string regionName,
        IEnumerable<ContentValidationKey> toScan,
        Microsoft.VisualStudio.Services.Identity.Identity contentCreator,
        string creatorIpAddress)
      {
        rc.CheckProjectCollectionRequestContext();
        return this.InitIfEnabledForRegion(rc, regionName) ? this.m_validator.SubmitAsync(rc, projectId, toScan, contentCreator, creatorIpAddress) : Task.CompletedTask;
      }

      public Task SubmitAsync(
        IVssRequestContext rc,
        Guid projectId,
        string regionName,
        string base64EncodedContent,
        ContentValidationScanType scanType,
        string fileName,
        Microsoft.VisualStudio.Services.Identity.Identity contentCreator,
        string creatorIpAddress)
      {
        rc.CheckProjectCollectionRequestContext();
        return this.InitIfEnabledForRegion(rc, regionName) ? this.m_validator.SubmitAsync(rc, new Guid?(projectId), base64EncodedContent, scanType, fileName, contentCreator, creatorIpAddress, ContentValidationTakedownTarget.AllProjectsInCollection) : Task.CompletedTask;
      }

      public Task SubmitAsync(
        IVssRequestContext rc,
        Guid projectId,
        string regionName,
        byte[] content,
        Microsoft.VisualStudio.Services.Identity.Identity contentCreator,
        string creatorIpAddress,
        string fileName,
        ContentValidationScanType? scanType)
      {
        rc.CheckProjectCollectionRequestContext();
        return this.InitIfEnabledForRegion(rc, regionName) ? this.m_validator.SubmitAsync(rc, projectId, content, contentCreator, creatorIpAddress, fileName, scanType) : Task.CompletedTask;
      }

      public Task SubmitImageAsync(
        IVssRequestContext deploymentRc,
        byte[] imageContent,
        Microsoft.VisualStudio.Services.Identity.Identity author,
        string lastUsedIpAddress,
        string regionName,
        ContentValidationTakedownTarget takedownTarget)
      {
        deploymentRc.CheckDeploymentRequestContext();
        return this.InitIfEnabledForRegion(deploymentRc, regionName) ? this.m_validator.SubmitAsync(deploymentRc, new Guid?(), Convert.ToBase64String(imageContent, Base64FormattingOptions.None), ContentValidationScanType.Image, (string) null, author, lastUsedIpAddress, takedownTarget) : Task.CompletedTask;
      }
    }
  }
}
