// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.TeamFoundationProcessService
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.ProcessTemplates.Events;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class TeamFoundationProcessService : ITeamFoundationProcessService, IVssFrameworkService
  {
    private Guid? m_notificationAuthor;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationSqlNotificationService notificationService = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? systemRequestContext.GetService<ITeamFoundationSqlNotificationService>() : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.m_notificationAuthor = new Guid?(notificationService.Author);
      notificationService.RegisterNotification(systemRequestContext, "Default", ProcessConstants.Notifications.Reset, new SqlNotificationCallback(this.OnReset), true);
      notificationService.RegisterNotification(systemRequestContext, "Default", ProcessConstants.Notifications.ProcessAdded, new SqlNotificationCallback(this.OnProcessAdded), true);
      notificationService.RegisterNotification(systemRequestContext, "Default", ProcessConstants.Notifications.ProcessChanged, new SqlNotificationCallback(this.OnProcessChanged), true);
      notificationService.RegisterNotification(systemRequestContext, "Default", ProcessConstants.Notifications.ProcessDeleted, new SqlNotificationCallback(this.OnProcessDeleted), true);
      notificationService.RegisterNotification(systemRequestContext, "Default", ProcessConstants.Notifications.ProcessEnabledDisabled, new SqlNotificationCallback(this.OnProcessEnabledDisabled), true);
      notificationService.RegisterNotification(systemRequestContext, "Default", ProcessConstants.Notifications.DefaultProcessChanged, new SqlNotificationCallback(this.OnDefaultProcessChanged), true);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(systemRequestContext, "Default", ProcessConstants.Notifications.ProcessAdded, new SqlNotificationCallback(this.OnProcessAdded), true);
      service.UnregisterNotification(systemRequestContext, "Default", ProcessConstants.Notifications.ProcessChanged, new SqlNotificationCallback(this.OnProcessChanged), true);
      service.UnregisterNotification(systemRequestContext, "Default", ProcessConstants.Notifications.ProcessDeleted, new SqlNotificationCallback(this.OnProcessDeleted), true);
      service.UnregisterNotification(systemRequestContext, "Default", ProcessConstants.Notifications.ProcessEnabledDisabled, new SqlNotificationCallback(this.OnProcessEnabledDisabled), true);
      service.UnregisterNotification(systemRequestContext, "Default", ProcessConstants.Notifications.DefaultProcessChanged, new SqlNotificationCallback(this.OnDefaultProcessChanged), true);
      service.UnregisterNotification(systemRequestContext, "Default", ProcessConstants.Notifications.Reset, new SqlNotificationCallback(this.OnReset), true);
    }

    private void OnProcessChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (string.IsNullOrEmpty(eventData))
        return;
      ProcessDescriptorNotificationRecord[] source = TeamFoundationSerializationUtility.Deserialize<ProcessDescriptorNotificationRecord[]>(eventData, new XmlRootAttribute("descriptors"));
      requestContext.GetService<ProcessDescriptorCacheService>().InvalidateByIds(requestContext, ((IEnumerable<ProcessDescriptorNotificationRecord>) source).SelectMany<ProcessDescriptorNotificationRecord, Guid>((Func<ProcessDescriptorNotificationRecord, IEnumerable<Guid>>) (d => (IEnumerable<Guid>) new Guid[2]
      {
        d.SpecificId,
        d.TypeId
      })));
    }

    private void OnProcessAdded(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
    }

    private void OnProcessDeleted(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (string.IsNullOrEmpty(eventData))
        return;
      ProcessDescriptorNotificationRecord[] source = TeamFoundationSerializationUtility.Deserialize<ProcessDescriptorNotificationRecord[]>(eventData, new XmlRootAttribute("descriptors"));
      requestContext.GetService<ProcessDescriptorCacheService>().RemoveByIds(requestContext, ((IEnumerable<ProcessDescriptorNotificationRecord>) source).SelectMany<ProcessDescriptorNotificationRecord, Guid>((Func<ProcessDescriptorNotificationRecord, IEnumerable<Guid>>) (d => (IEnumerable<Guid>) new Guid[2]
      {
        d.SpecificId,
        d.TypeId
      })));
    }

    private void OnProcessEnabledDisabled(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.GetService<ProcessDescriptorCacheService>().ClearDisabledProcessIdSet(requestContext);
    }

    private void OnDefaultProcessChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      ProcessDescriptorCacheService service = requestContext.GetService<ProcessDescriptorCacheService>();
      Guid result;
      if (!string.IsNullOrEmpty(eventData) && Guid.TryParse(eventData, out result) && result != Guid.Empty)
        service.SetDefaultProcessTypeId(requestContext, result);
      else
        service.ClearDefaultProcessId(requestContext);
    }

    private void OnReset(IVssRequestContext requestContext, Guid eventClass, string eventData) => requestContext.GetService<ProcessDescriptorCacheService>().Clear(requestContext);

    public ProcessDescriptor CreateOrUpdateLegacyProcess(
      IVssRequestContext requestContext,
      Stream zipContentStream)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Stream>(zipContentStream, nameof (zipContentStream));
      return requestContext.TraceBlock<ProcessDescriptor>(10005001, 10005002, "ProcessTemplate", nameof (TeamFoundationProcessService), nameof (CreateOrUpdateLegacyProcess), (Func<ProcessDescriptor>) (() =>
      {
        this.CheckMaxTemplateFileSizeLimitRespected(requestContext, zipContentStream);
        string name;
        string description;
        ProcessVersion version;
        Guid typeId;
        using (ZipArchiveProcessTemplatePackage processTemplatePackage = new ZipArchiveProcessTemplatePackage(zipContentStream, true, true))
        {
          name = processTemplatePackage.Name;
          description = processTemplatePackage.Description;
          version = processTemplatePackage.Version;
          typeId = processTemplatePackage.TypeId;
        }
        zipContentStream.Seek(0L, SeekOrigin.Begin);
        this.CheckValidProcessName(name);
        TeamFoundationProcessService.CheckDescriptionText(description);
        ProcessDescriptor descriptor1;
        if (this.TryGetProcessDescriptor(requestContext, typeId, out descriptor1, true, false) && descriptor1.Scope == ProcessScope.Deployment && !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          throw new ProcessInvalidOverrideException(descriptor1, ProcessScope.Collection, typeId, version);
        this.CheckDeploymentLevelNameCollisions(requestContext, name, (string) null);
        if (descriptor1 == null)
        {
          this.CheckMaxProcessLimitRespected(requestContext);
          this.CheckRootCreatePermission(requestContext);
        }
        else
          this.CheckProcessPermission(requestContext, descriptor1, 1, true);
        ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
        byte[] md5 = MD5Util.CalculateMD5(zipContentStream, true);
        int fileId = 0;
        long length = zipContentStream.Length;
        IVssRequestContext requestContext1 = requestContext;
        ref int local = ref fileId;
        Stream content = zipContentStream;
        byte[] hashValue = md5;
        long compressedLength = length;
        long uncompressedLength = length;
        Guid empty = Guid.Empty;
        int num = requestContext.ExecutionEnvironment.IsCloudDeployment ? 1 : 0;
        service.UploadFile(requestContext1, ref local, content, hashValue, compressedLength, uncompressedLength, 0L, CompressionType.None, OwnerId.ProcessTemplate, empty, "", false, num != 0);
        Guid identityIdInternal = this.GetUserIdentityIdInternal(requestContext);
        ProcessDescriptor descriptor2;
        using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
          descriptor2 = (ProcessDescriptor) new ProcessDescriptorImpl(component.CreateOrUpdateLegacyProcess(typeId, name, description, version, md5, fileId, identityIdInternal));
        this.PublishSqlNotification(requestContext, ProcessConstants.Notifications.ProcessChanged, TeamFoundationSerializationUtility.SerializeToString<ProcessDescriptorNotificationRecord[]>(new ProcessDescriptorNotificationRecord[1]
        {
          new ProcessDescriptorNotificationRecord()
          {
            SpecificId = descriptor2.RowId,
            TypeId = descriptor2.TypeId
          }
        }, new XmlRootAttribute("descriptors")));
        this.AddProcessPermission(requestContext, descriptor2, 15);
        requestContext.GetService<ProcessDescriptorCacheService>().Set(requestContext, descriptor2);
        return descriptor2;
      }));
    }

    public void CheckMaxProcessLimitRespected(
      IVssRequestContext requestContext,
      int numberOfNewProcesses = 1)
    {
      if (this.GetProcessCountLimit(requestContext) < requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptors(requestContext, false).Count + numberOfNewProcesses)
        throw new ProcessLimitExceededException();
    }

    public void CheckValidProcessName(string name)
    {
      if (string.IsNullOrEmpty(name) || name.Length > ProcessConstants.MaxNameLength || name.IndexOfAny(ProcessConstants.IllegalProcessNameChars) != -1)
      {
        string illegalChars = string.Join<char>("", (IEnumerable<char>) ProcessConstants.IllegalProcessNameChars);
        throw new ProcessNameInvalidException(ProcessConstants.MaxNameLength, illegalChars);
      }
      if (ProcessConstants.ReservedProcessNames.Any<string>((Func<string, bool>) (n => TFStringComparer.ProcessReferenceName.Equals(n, name))))
        throw new ProcessNameNotSupportedException(name);
    }

    public virtual void UpdateProcessStatuses(
      IVssRequestContext requestContext,
      IEnumerable<Guid> typeIds,
      ProcessStatus processState)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(typeIds, nameof (typeIds));
      if (!typeIds.Any<Guid>())
        return;
      ILookup<Guid, ProcessDescriptor> lookup = this.GetProcessDescriptors(requestContext, false, false).ToLookup<ProcessDescriptor, Guid>((Func<ProcessDescriptor, Guid>) (d => d.TypeId));
      List<ProcessDescriptorImpl> processDescriptorImplList = new List<ProcessDescriptorImpl>();
      foreach (Guid typeId in typeIds)
      {
        ProcessDescriptor processDescriptor = lookup[typeId].FirstOrDefault<ProcessDescriptor>();
        if (processDescriptor == null)
          throw new ProcessNotFoundByIdException(typeId);
        this.CheckProcessPermission(requestContext, processDescriptor, 1, true);
        if (!TeamFoundationProcessService.IsValidProcessStateTransition(processDescriptor.ProcessStatus, processState))
          throw new ProcessInvalidStateTransitionException(typeId, processDescriptor.ProcessStatus, processState);
        ProcessDescriptorImpl processDescriptorImpl = new ProcessDescriptorImpl(processDescriptor, processState);
        processDescriptorImplList.Add(processDescriptorImpl);
      }
      using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
        component.UpdateProcessStatuses(processDescriptorImplList.Select<ProcessDescriptorImpl, Guid>((Func<ProcessDescriptorImpl, Guid>) (d => d.TypeId)), processState);
      ProcessDescriptorNotificationRecord[] array = typeIds.Select<Guid, ProcessDescriptorNotificationRecord>((Func<Guid, ProcessDescriptorNotificationRecord>) (id => new ProcessDescriptorNotificationRecord()
      {
        SpecificId = Guid.Empty,
        TypeId = id
      })).ToArray<ProcessDescriptorNotificationRecord>();
      this.PublishSqlNotification(requestContext, ProcessConstants.Notifications.ProcessChanged, TeamFoundationSerializationUtility.SerializeToString<ProcessDescriptorNotificationRecord[]>(array, new XmlRootAttribute("descriptors")));
      requestContext.GetService<ProcessDescriptorCacheService>().Set(requestContext, (IEnumerable<ProcessDescriptor>) processDescriptorImplList);
    }

    public virtual void UpdateProcessStatus(
      IVssRequestContext requestContext,
      Guid typeId,
      ProcessStatus processState)
    {
      this.UpdateProcessStatuses(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        typeId
      }, processState);
    }

    public Guid GetSpecificProcessDescriptorIdByIntegerId(
      IVssRequestContext requestContext,
      int integerId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      Guid descriptorId;
      if (!this.TryGetSpecificProcessDescriptorIdByIntegerId(requestContext, integerId, out descriptorId))
        throw new ProcessNotFoundByIntegerIdException(integerId);
      return descriptorId;
    }

    public virtual bool TryGetSpecificProcessDescriptorIdByIntegerId(
      IVssRequestContext requestContext,
      int integerId,
      out Guid descriptorId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      Guid temporaryDescriptorId = Guid.Empty;
      requestContext.TraceBlock(10005003, 10005004, "ProcessTemplate", nameof (TeamFoundationProcessService), nameof (TryGetSpecificProcessDescriptorIdByIntegerId), (Action) (() =>
      {
        using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
          temporaryDescriptorId = component.GetProcessDescriptorSpecificId(integerId).GetValueOrDefault();
        if (!(temporaryDescriptorId == Guid.Empty) || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          return;
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<ITeamFoundationProcessService>().TryGetSpecificProcessDescriptorIdByIntegerId(vssRequestContext, integerId, out temporaryDescriptorId);
      }));
      descriptorId = temporaryDescriptorId;
      return descriptorId != Guid.Empty;
    }

    public Guid GetSpecificProcessDescriptorIdByVersion(
      IVssRequestContext requestContext,
      Guid processTypeId,
      int majorVersion,
      int minorVersion)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processTypeId, nameof (processTypeId));
      Guid descriptorId;
      if (!this.TryGetSpecificProcessDescriptorIdByVersion(requestContext, processTypeId, majorVersion, minorVersion, out descriptorId))
        throw new ProcessNotFoundByVersionException(processTypeId, majorVersion, minorVersion);
      return descriptorId;
    }

    public virtual bool TryGetSpecificProcessDescriptorIdByVersion(
      IVssRequestContext requestContext,
      Guid processTypeId,
      int majorVersion,
      int minorVersion,
      out Guid descriptorId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processTypeId, nameof (processTypeId));
      Guid temporaryDescriptorId = Guid.Empty;
      requestContext.TraceBlock(10005005, 10005006, "ProcessTemplate", nameof (TeamFoundationProcessService), nameof (TryGetSpecificProcessDescriptorIdByVersion), (Action) (() =>
      {
        using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
          temporaryDescriptorId = component.GetProcessDescriptorSpecificId(processTypeId, majorVersion, minorVersion).GetValueOrDefault();
        if (!(temporaryDescriptorId == Guid.Empty) || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          return;
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<ITeamFoundationProcessService>().TryGetSpecificProcessDescriptorIdByVersion(vssRequestContext, processTypeId, majorVersion, minorVersion, out temporaryDescriptorId);
      }));
      descriptorId = temporaryDescriptorId;
      return descriptorId != Guid.Empty;
    }

    public virtual IReadOnlyCollection<ProcessDescriptor> ReadProcessDescriptors(
      IVssRequestContext requestContext,
      bool fallThrough = true,
      ServiceLevel serviceLevel = null,
      bool bypassCache = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return (IReadOnlyCollection<ProcessDescriptor>) requestContext.TraceBlock<List<ProcessDescriptor>>(10005007, 10005008, "ProcessTemplate", nameof (TeamFoundationProcessService), nameof (ReadProcessDescriptors), (Func<List<ProcessDescriptor>>) (() =>
      {
        List<ProcessDescriptor> source = new List<ProcessDescriptor>();
        if (fallThrough && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        {
          ServiceLevel serviceLevel1 = this.GetServiceLevel(requestContext);
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          TeamFoundationProcessService service = vssRequestContext.GetService<TeamFoundationProcessService>();
          source.AddRange((IEnumerable<ProcessDescriptor>) service.ReadProcessDescriptors(vssRequestContext, false, serviceLevel1));
        }
        IReadOnlyCollection<ProcessDescriptor> descriptors = (IReadOnlyCollection<ProcessDescriptor>) null;
        ProcessDescriptorCacheService service1 = requestContext.GetService<ProcessDescriptorCacheService>();
        if (bypassCache || !service1.TryGetTipList(requestContext, out descriptors))
        {
          IEnumerable<ProcessTemplateDescriptorEntry> processDescriptors;
          using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
            processDescriptors = (IEnumerable<ProcessTemplateDescriptorEntry>) component.GetAllProcessDescriptors(serviceLevel);
          descriptors = (IReadOnlyCollection<ProcessDescriptor>) new List<ProcessDescriptor>((IEnumerable<ProcessDescriptor>) processDescriptors.Select<ProcessTemplateDescriptorEntry, ProcessDescriptorImpl>((Func<ProcessTemplateDescriptorEntry, ProcessDescriptorImpl>) (entry => new ProcessDescriptorImpl(entry))));
          service1.SetTipList(requestContext, descriptors);
        }
        HashSet<string> stringSet = new HashSet<string>(source.Select<ProcessDescriptor, string>((Func<ProcessDescriptor, string>) (d => d.Name)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (ProcessDescriptor descriptorToCopy in (IEnumerable<ProcessDescriptor>) descriptors)
        {
          if (stringSet.Contains(descriptorToCopy.Name))
            source.Add((ProcessDescriptor) new ProcessDescriptorImpl(descriptorToCopy, descriptorToCopy.ProcessStatus, Resources.ProcessTemplateNameDisambugiated((object) descriptorToCopy.Name)));
          else
            source.Add(descriptorToCopy);
        }
        return source;
      }));
    }

    public virtual IReadOnlyCollection<ProcessDescriptor> GetProcessDescriptors(
      IVssRequestContext requestContext,
      bool fallThrough = true,
      bool bypassCache = false)
    {
      return this.ReadProcessDescriptors(requestContext, fallThrough, bypassCache: bypassCache);
    }

    public virtual Guid GetDefaultProcessTypeId(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ProcessDescriptorCacheService service = requestContext.GetService<ProcessDescriptorCacheService>();
      Guid defaultProcessTypeId;
      if (!service.TryGetDefaultProcessId(requestContext, out defaultProcessTypeId))
      {
        using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
          defaultProcessTypeId = component.GetDefaultProcessTypeId();
        service.SetDefaultProcessTypeId(requestContext, defaultProcessTypeId);
      }
      if (!(defaultProcessTypeId == Guid.Empty) || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return defaultProcessTypeId;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<ITeamFoundationProcessService>().GetDefaultProcessTypeId(vssRequestContext);
    }

    public virtual ISet<Guid> GetDisabledProcessTypeIds(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      HashSet<Guid> disabledProcessTypeIds1 = new HashSet<Guid>();
      ProcessDescriptorCacheService service1 = requestContext.GetService<ProcessDescriptorCacheService>();
      ISet<Guid> disabledProcessTypeIds2;
      if (service1.TryGetDisabledProcessIdSet(requestContext, out disabledProcessTypeIds2))
      {
        disabledProcessTypeIds1.UnionWith((IEnumerable<Guid>) disabledProcessTypeIds2);
      }
      else
      {
        ISet<Guid> guidSet;
        using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
        {
          guidSet = (ISet<Guid>) new HashSet<Guid>((IEnumerable<Guid>) component.GetDisabledProcessTypeIds());
          disabledProcessTypeIds1.UnionWith((IEnumerable<Guid>) guidSet);
        }
        service1.SetDisabledProcessIdSet(requestContext, guidSet);
      }
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        ITeamFoundationProcessService service2 = vssRequestContext.GetService<ITeamFoundationProcessService>();
        disabledProcessTypeIds1.UnionWith((IEnumerable<Guid>) service2.GetDisabledProcessTypeIds(vssRequestContext));
      }
      return (ISet<Guid>) disabledProcessTypeIds1;
    }

    public Stream GetLegacyProcessPackageContent(
      IVssRequestContext requestContext,
      ProcessDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessDescriptor>(descriptor, nameof (descriptor));
      if (descriptor.IsDerived)
      {
        ProcessDescriptor processDescriptor = this.GetProcessDescriptor(requestContext, descriptor.Inherits, true, false);
        return this.GetLegacyProcessPackageContent(requestContext, processDescriptor);
      }
      if (descriptor.Scope == ProcessScope.Deployment)
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<ITeamFoundationProcessService>().GetLegacyProcessPackageContent(vssRequestContext, descriptor);
        }
      }
      else if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(TeamFoundationHostType.ProjectCollection);
      TeamFoundationFileCacheService service = requestContext.GetService<TeamFoundationFileCacheService>();
      using (FileStreamDownloadState streamDownloadState = new FileStreamDownloadState(requestContext))
      {
        FileInformation fileInfo = new FileInformation(requestContext.ServiceHost.InstanceId, descriptor.FileId, (byte[]) null);
        service.RetrieveFile<FileInformation>(requestContext, fileInfo, (IDownloadState<FileInformation>) streamDownloadState, false);
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("skipped", true);
          properties.Add("processName", descriptor.Name);
          properties.Add("typeId", (object) descriptor.TypeId);
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ProcessTemplate", "RemoveUnsupportedPlugins", properties);
          return (Stream) new FileStreamWrapper((Stream) streamDownloadState.FileStream);
        }
        MemoryStream memoryStream = new MemoryStream();
        streamDownloadState.FileStream.CopyTo((Stream) memoryStream);
        this.RemovePluginsNotSupported(requestContext, (Stream) memoryStream);
        memoryStream.Seek(0L, SeekOrigin.Begin);
        return (Stream) new FileStreamWrapper((Stream) memoryStream);
      }
    }

    private void RemovePluginsNotSupported(IVssRequestContext requestContext, Stream fileStream)
    {
      requestContext.GetService<ITeamFoundationProcessService>();
      using (ZipArchiveProcessTemplatePackage processTemplatePackage = new ZipArchiveProcessTemplatePackage(fileStream, true, true, true))
        processTemplatePackage.RemovePluginsNotSupported(requestContext.ExecutionEnvironment.IsHostedDeployment);
    }

    public Stream GetLegacyProcessPackageContent(
      IVssRequestContext requestContext,
      Guid legacyProcessTypeId)
    {
      return this.GetLegacyProcessPackageContent(requestContext, this.GetProcessDescriptor(requestContext, legacyProcessTypeId, true, false));
    }

    public Stream GetSpecificLegacyProcessPackageContent(
      IVssRequestContext requestContext,
      Guid legacyProcessSpecificId)
    {
      return this.GetLegacyProcessPackageContent(requestContext, this.GetSpecificProcessDescriptor(requestContext, legacyProcessSpecificId, true));
    }

    public IProcessTemplate GetLegacyProcess(
      IVssRequestContext requestContext,
      ProcessDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessDescriptor>(descriptor, nameof (descriptor));
      return requestContext.TraceBlock<IProcessTemplate>(10005009, 10005010, "ProcessTemplate", nameof (TeamFoundationProcessService), nameof (GetLegacyProcess), (Func<IProcessTemplate>) (() =>
      {
        if (descriptor.IsDerived)
          return (IProcessTemplate) (this.GetLegacyProcess(requestContext, this.GetProcessDescriptor(requestContext, descriptor.Inherits, true, false)) as LegacyProcessPackage).CreateClone(descriptor);
        if (descriptor.Scope == ProcessScope.Deployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          return vssRequestContext.GetService<ITeamFoundationProcessService>().GetLegacyProcess(vssRequestContext, descriptor);
        }
        LegacyProcessCacheService service1 = requestContext.GetService<LegacyProcessCacheService>();
        IProcessTemplate legacyProcess;
        if (!service1.TryGetValue(requestContext, descriptor.RowId, out legacyProcess))
        {
          TeamFoundationFileCacheService service2 = requestContext.GetService<TeamFoundationFileCacheService>();
          using (FileStreamDownloadState streamDownloadState = new FileStreamDownloadState(requestContext))
          {
            FileInformation fileInfo = new FileInformation(requestContext.ServiceHost.InstanceId, descriptor.FileId, (byte[]) null);
            service2.RetrieveFile<FileInformation>(requestContext, fileInfo, (IDownloadState<FileInformation>) streamDownloadState, false);
            using (FileStreamWrapper zipContentStream = new FileStreamWrapper((Stream) streamDownloadState.FileStream))
              legacyProcess = (IProcessTemplate) LegacyProcessPackage.Load(descriptor, (Stream) zipContentStream);
          }
          service1.Set(requestContext, descriptor.RowId, legacyProcess);
        }
        return legacyProcess;
      }));
    }

    public virtual bool TryGetProcessDescriptor(
      IVssRequestContext requestContext,
      Guid processTypeId,
      out ProcessDescriptor descriptor,
      bool fallThrough = true,
      bool bypassCache = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processTypeId, nameof (processTypeId));
      ProcessDescriptor descriptorFound = (ProcessDescriptor) null;
      int cachedProcessCount = -1;
      int dbProcessCount = -1;
      requestContext.TraceBlock(10005011, 10005012, "ProcessTemplate", nameof (TeamFoundationProcessService), nameof (TryGetProcessDescriptor), (Action) (() =>
      {
        if (bypassCache || !this.TryGetCachedDescriptor(requestContext, processTypeId, out descriptorFound, fallThrough))
        {
          IReadOnlyCollection<ProcessDescriptor> processDescriptors = this.GetProcessDescriptors(requestContext, fallThrough, bypassCache);
          descriptorFound = processDescriptors != null ? processDescriptors.FirstOrDefault<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (d => d.TypeId == processTypeId)) : (ProcessDescriptor) null;
          cachedProcessCount = processDescriptors != null ? processDescriptors.Count : 0;
        }
        if (descriptorFound != null || bypassCache)
          return;
        IReadOnlyCollection<ProcessDescriptor> processDescriptors1 = this.GetProcessDescriptors(requestContext, fallThrough, true);
        descriptorFound = processDescriptors1 != null ? processDescriptors1.FirstOrDefault<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (d => d.TypeId == processTypeId)) : (ProcessDescriptor) null;
        dbProcessCount = processDescriptors1 != null ? processDescriptors1.Count : 0;
      }));
      descriptor = descriptorFound;
      if (descriptor == null)
      {
        requestContext.Trace(10005121, TraceLevel.Warning, "ProcessTemplate", "GetLegacyProcess", string.Format("Unable to get process with it {0}", (object) processTypeId));
        if (processTypeId == ProcessTemplateTypeIdentifiers.MsfHydroProcess)
          requestContext.Trace(10005122, TraceLevel.Error, "ProcessTemplate", nameof (TryGetProcessDescriptor), string.Format("Hydro process not found, process type id: {0}, bypassCache: {1}, isDeploymentService: {2}, cachedProcessCount: {3}, databaseProcessCount: {4}", (object) processTypeId, (object) bypassCache, (object) requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment), (object) cachedProcessCount, (object) dbProcessCount));
      }
      return descriptor != null;
    }

    public virtual ProcessDescriptor GetProcessDescriptor(
      IVssRequestContext requestContext,
      Guid processTypeId,
      bool fallThrough = true,
      bool bypassCache = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processTypeId, nameof (processTypeId));
      ProcessDescriptor descriptor;
      if (!this.TryGetProcessDescriptor(requestContext, processTypeId, out descriptor, fallThrough, bypassCache))
        throw new ProcessNotFoundByTypeIdException(processTypeId);
      return descriptor;
    }

    public void SetProcessAsDefault(IVssRequestContext requestContext, Guid processTypeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processTypeId, nameof (processTypeId));
      requestContext.TraceBlock(10005013, 10005014, "ProcessTemplate", nameof (TeamFoundationProcessService), nameof (SetProcessAsDefault), (Action) (() =>
      {
        ProcessDescriptor processDescriptor = this.GetProcessDescriptor(requestContext, processTypeId, true, false);
        if (this.GetDisabledProcessTypeIds(requestContext).Contains(processDescriptor.TypeId))
          throw new ProcessInvalidDefaultOnDisabledException();
        this.CheckProcessPermission(requestContext, processDescriptor, 1, false);
        Guid identityIdInternal = this.GetUserIdentityIdInternal(requestContext);
        using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
          component.SetDefaultProcessType(processTypeId, identityIdInternal);
        this.PublishSqlNotification(requestContext, ProcessConstants.Notifications.DefaultProcessChanged, processTypeId.ToString());
        requestContext.GetService<ProcessDescriptorCacheService>().SetDefaultProcessTypeId(requestContext, processTypeId);
      }));
    }

    public virtual bool TryGetSpecificProcessDescriptor(
      IVssRequestContext requestContext,
      Guid processSpecificId,
      out ProcessDescriptor descriptor,
      bool fallThrough = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processSpecificId, nameof (processSpecificId));
      ProcessDescriptor specificDescriptor = (ProcessDescriptor) null;
      requestContext.TraceBlock(10005015, 10005016, "ProcessTemplate", nameof (TeamFoundationProcessService), nameof (TryGetSpecificProcessDescriptor), (Action) (() =>
      {
        if (this.TryGetCachedDescriptor(requestContext, processSpecificId, out specificDescriptor, fallThrough))
          return;
        ProcessTemplateDescriptorEntry entry = (ProcessTemplateDescriptorEntry) null;
        using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
          entry = component.GetSpecificProcessDescriptor(processSpecificId);
        if (entry != null)
        {
          specificDescriptor = (ProcessDescriptor) new ProcessDescriptorImpl(entry);
          requestContext.GetService<ProcessDescriptorCacheService>().Set(requestContext, specificDescriptor);
        }
        else
        {
          if (!fallThrough || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
            return;
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          ProcessDescriptor descriptor1;
          if (!vssRequestContext.GetService<ITeamFoundationProcessService>().TryGetSpecificProcessDescriptor(vssRequestContext, processSpecificId, out descriptor1))
            return;
          specificDescriptor = descriptor1;
        }
      }));
      descriptor = specificDescriptor;
      return descriptor != null;
    }

    public virtual ProcessDescriptor GetSpecificProcessDescriptor(
      IVssRequestContext requestContext,
      Guid processSpecificId,
      bool fallThrough = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processSpecificId, nameof (processSpecificId));
      ProcessDescriptor descriptor;
      if (!this.TryGetSpecificProcessDescriptor(requestContext, processSpecificId, out descriptor, fallThrough))
        throw new ProcessNotFoundByIdException(processSpecificId);
      return descriptor;
    }

    public IReadOnlyCollection<ProcessDescriptor> GetProcessDescriptorHistory(
      IVssRequestContext requestContext,
      Guid processTypeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processTypeId, nameof (processTypeId));
      return requestContext.TraceBlock<IReadOnlyCollection<ProcessDescriptor>>(10005019, 10005020, "ProcessTemplate", nameof (TeamFoundationProcessService), nameof (GetProcessDescriptorHistory), (Func<IReadOnlyCollection<ProcessDescriptor>>) (() =>
      {
        IEnumerable<ProcessTemplateDescriptorEntry> source = (IEnumerable<ProcessTemplateDescriptorEntry>) null;
        using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
          source = (IEnumerable<ProcessTemplateDescriptorEntry>) component.GetProcessHistory(processTypeId);
        if (source.Any<ProcessTemplateDescriptorEntry>() || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          return (IReadOnlyCollection<ProcessDescriptor>) source.Select<ProcessTemplateDescriptorEntry, ProcessDescriptorImpl>((Func<ProcessTemplateDescriptorEntry, ProcessDescriptorImpl>) (entry => new ProcessDescriptorImpl(entry))).ToList<ProcessDescriptorImpl>();
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return vssRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptorHistory(vssRequestContext, processTypeId);
      }));
    }

    public void DeleteProcess(IVssRequestContext requestContext, Guid processTypeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processTypeId, nameof (processTypeId));
      requestContext.TraceBlock(10005017, 10005018, "ProcessTemplate", nameof (TeamFoundationProcessService), nameof (DeleteProcess), (Action) (() =>
      {
        ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
        ProcessDescriptor descriptor = service.GetProcessDescriptor(requestContext, processTypeId, false);
        if (descriptor.ProcessStatus != ProcessStatus.Ready)
        {
          if (descriptor.IsSystem || this.GetSubscribedProjectsForProcess(requestContext, processTypeId).Count != 0)
            throw new ProcessCannotDeleteNonReady();
          this.UpdateProcessStatus(requestContext, descriptor.TypeId, ProcessStatus.Ready);
        }
        if (descriptor.TypeId == service.GetDefaultProcessTypeId(requestContext))
          throw new ProcessCannotDeleteDefault();
        service.CheckProcessPermission(requestContext, descriptor, 2);
        Action<bool> action = (Action<bool>) (revertProcessStatus =>
        {
          if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && this.GetSubscribedProjectsForProcess(requestContext, descriptor.TypeId).Any<ProjectInfo>())
          {
            if (revertProcessStatus)
              this.UpdateProcessStatus(requestContext, descriptor.TypeId, ProcessStatus.Ready);
            throw new ProcessCannotDeleteWithActiveProjects();
          }
        });
        if (this.IsProcessEnabled(requestContext))
        {
          action(false);
          this.UpdateProcessStatus(requestContext, descriptor.TypeId, ProcessStatus.Deleting);
          action(true);
        }
        using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
          component.DeleteProcess(descriptor.RowId, descriptor.TypeId);
        this.PublishSqlNotification(requestContext, ProcessConstants.Notifications.ProcessDeleted, TeamFoundationSerializationUtility.SerializeToString<ProcessDescriptorNotificationRecord[]>(new ProcessDescriptorNotificationRecord[1]
        {
          new ProcessDescriptorNotificationRecord()
          {
            SpecificId = descriptor.RowId,
            TypeId = descriptor.TypeId
          }
        }, new XmlRootAttribute("descriptors")));
        requestContext.GetService<ProcessDescriptorCacheService>().Remove(requestContext, descriptor);
        requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) new ProcessChangedEvent()
        {
          ProcessTypeId = descriptor.TypeId,
          ChangeType = ProcessChangeType.ProcessDeleted
        });
      }));
    }

    public ProcessDescriptor UpdateProcessNameAndDescription(
      IVssRequestContext requestContext,
      Guid processTypeId,
      string newName,
      string newDescription)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processTypeId, nameof (processTypeId));
      if (newName == null && newDescription == null)
        throw new ProcessInvalidInheritedProcessUpdateInputException();
      if (newName != null)
      {
        ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
        newName = newName.Trim();
        string name = newName;
        service.CheckValidProcessName(name);
      }
      TeamFoundationProcessService.CheckDescriptionText(newDescription);
      return requestContext.TraceBlock<ProcessDescriptor>(10005102, 10005103, "ProcessTemplate", nameof (TeamFoundationProcessService), nameof (UpdateProcessNameAndDescription), (Func<ProcessDescriptor>) (() =>
      {
        ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
        ProcessDescriptor processDescriptor1 = service.GetProcessDescriptor(requestContext, processTypeId);
        service.CheckProcessPermission(requestContext, processDescriptor1, 1);
        if (StringComparer.Ordinal.Equals(newName ?? processDescriptor1.Name, processDescriptor1.Name) && StringComparer.Ordinal.Equals(newDescription ?? processDescriptor1.Description, processDescriptor1.Description))
          return processDescriptor1;
        if (!TFStringComparer.ProcessName.Equals(newName ?? processDescriptor1.Name, processDescriptor1.Name))
          this.CheckDeploymentLevelNameCollisions(requestContext, newName, (string) null);
        Guid identityIdInternal = this.GetUserIdentityIdInternal(requestContext);
        ProcessDescriptor processDescriptor2;
        using (ProcessTemplateComponent component = this.CreateComponent(requestContext))
          processDescriptor2 = (ProcessDescriptor) new ProcessDescriptorImpl(component.UpdateProcessNameAndDescription(processDescriptor1, newName, newDescription, identityIdInternal));
        this.PublishSqlNotification(requestContext, ProcessConstants.Notifications.ProcessChanged, TeamFoundationSerializationUtility.SerializeToString<ProcessDescriptorNotificationRecord[]>(new ProcessDescriptorNotificationRecord[1]
        {
          new ProcessDescriptorNotificationRecord()
          {
            SpecificId = processDescriptor2.RowId,
            TypeId = processDescriptor2.TypeId
          }
        }, new XmlRootAttribute("descriptors")));
        requestContext.GetService<ProcessDescriptorCacheService>().Set(requestContext, processDescriptor2);
        foreach (ProjectInfo project in (IEnumerable<ProjectInfo>) this.GetSubscribedProjectsForProcess(requestContext, processDescriptor2.TypeId))
          this.UpdateProjectProcess(requestContext, project, processDescriptor2);
        return processDescriptor2;
      }));
    }

    public void UpdateProjectProcess(
      IVssRequestContext requestContext,
      ProjectInfo project,
      ProcessDescriptor targetProcess)
    {
      IProjectService service = requestContext.GetService<IProjectService>();
      List<ProjectProperty> projectPropertyList1 = new List<ProjectProperty>()
      {
        new ProjectProperty()
        {
          Name = ProcessTemplateIdPropertyNames.CurrentProcessTemplateId,
          Value = (object) targetProcess.RowId.ToString("D")
        },
        new ProjectProperty()
        {
          Name = ProcessTemplateIdPropertyNames.ProcessTemplateType,
          Value = (object) targetProcess.TypeId.ToString("D")
        }
      };
      List<ProjectProperty> projectPropertyList2 = projectPropertyList1;
      ProjectProperty projectProperty = project.Properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (p => p.Name == ProcessTemplateIdPropertyNames.OriginalProcessTemplateId));
      if (projectProperty == null)
        projectProperty = new ProjectProperty()
        {
          Name = ProcessTemplateIdPropertyNames.OriginalProcessTemplateId,
          Value = (object) targetProcess.RowId.ToString("D")
        };
      projectPropertyList2.Add(projectProperty);
      service.CheckProjectPermission(requestContext, ProjectInfo.GetProjectUri(project.Id), TeamProjectPermissions.GenericWrite);
      service.SetProjectProperties(requestContext.Elevate(), project.Id, (IEnumerable<ProjectProperty>) projectPropertyList1);
    }

    public IReadOnlyCollection<ProjectProcessDescriptorMapping> GetProjectProcessDescriptorMappings(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds = null,
      bool expectUnmappedProjects = false,
      bool expectInvalidProcessIds = false,
      ProjectState projectStateFilter = ProjectState.WellFormed)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (projectIds != null)
      {
        foreach (Guid projectId in projectIds)
          ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectIds));
      }
      List<ProjectProcessDescriptorMapping> descriptorMappings = new List<ProjectProcessDescriptorMapping>();
      if (projectIds == null || projectIds.Any<Guid>())
      {
        IEnumerable<ProjectInfo> projects = requestContext.GetService<IProjectService>().GetProjects(requestContext, projectStateFilter);
        IEnumerable<ProjectInfo> source = requestContext.GetService<ILegacyProjectPropertiesReaderService>().PopulateProperties(projects, requestContext, ProcessTemplateIdPropertyNames.ProcessTemplateType);
        if (projectIds != null && source.Any<ProjectInfo>())
        {
          Dictionary<Guid, ProjectInfo> dictionary = source.ToDictionary<ProjectInfo, Guid>((Func<ProjectInfo, Guid>) (p => p.Id));
          List<ProjectInfo> projectInfoList = new List<ProjectInfo>();
          foreach (Guid key in projectIds.Distinct<Guid>())
          {
            ProjectInfo projectInfo;
            if (!dictionary.TryGetValue(key, out projectInfo))
              throw new ProjectDoesNotExistException(key.ToString());
            projectInfoList.Add(projectInfo);
          }
          source = (IEnumerable<ProjectInfo>) projectInfoList;
        }
        Dictionary<Guid, ProcessDescriptor> dictionary1 = new Dictionary<Guid, ProcessDescriptor>();
        if (source.Any<ProjectInfo>())
        {
          foreach (ProcessDescriptor processDescriptor in (IEnumerable<ProcessDescriptor>) requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptors(requestContext))
          {
            dictionary1[processDescriptor.TypeId] = processDescriptor;
            dictionary1[processDescriptor.RowId] = processDescriptor;
          }
        }
        foreach (ProjectInfo projectInfo in source)
        {
          ProcessDescriptor processDescriptor = (ProcessDescriptor) null;
          IList<ProjectProperty> properties = projectInfo.Properties;
          string input = properties != null ? (string) properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (p => StringComparer.OrdinalIgnoreCase.Equals(p.Name, ProcessTemplateIdPropertyNames.ProcessTemplateType)))?.Value : (string) (object) null;
          Guid result;
          if (!string.IsNullOrEmpty(input) && Guid.TryParse(input, out result))
          {
            if (!dictionary1.TryGetValue(result, out processDescriptor))
            {
              if (!expectInvalidProcessIds)
                throw new ProcessProjectWithInvalidProcessException(projectInfo.Name);
              processDescriptor = (ProcessDescriptor) null;
            }
          }
          else if (!expectUnmappedProjects)
            throw new ProcessProjectWithInvalidProcessException(projectInfo.Name);
          descriptorMappings.Add(new ProjectProcessDescriptorMapping()
          {
            Project = projectInfo,
            Descriptor = processDescriptor
          });
        }
      }
      return (IReadOnlyCollection<ProjectProcessDescriptorMapping>) descriptorMappings;
    }

    public virtual bool HasProcessPermission(
      IVssRequestContext requestContext,
      int requestedPermission,
      ProcessDescriptor descriptor = null,
      bool checkDescriptorScope = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckGreaterThanOrEqualToZero((float) requestedPermission, nameof (requestedPermission));
      if (requestContext.IsSystemContext)
        return true;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        requestContext.Trace(10005104, TraceLevel.Info, "ProcessTemplate", nameof (TeamFoundationProcessService), "Process permission denied because request context was deployment level and call was not system context.");
        return false;
      }
      if (checkDescriptorScope && descriptor != null && descriptor.Scope == ProcessScope.Deployment)
      {
        requestContext.Trace(10005104, TraceLevel.Info, "ProcessTemplate", nameof (TeamFoundationProcessService), "Process permission denied because request context was collection level and descriptor was deployment level.");
        return false;
      }
      bool flag;
      if (this.IsProcessEnabled(requestContext))
      {
        Guid processNamespaceId = FrameworkSecurity.ProcessNamespaceId;
        string securityToken = this.GetSecurityToken(requestContext, descriptor);
        int permission = requestedPermission;
        flag = this.HasPermission(requestContext, processNamespaceId, securityToken, permission);
      }
      else
        flag = this.HasManageTemplatesPermissionOnProcessTemplatesNamespace(requestContext);
      return flag;
    }

    public void CheckProcessPermission(
      IVssRequestContext requestContext,
      ProcessDescriptor descriptor,
      int requestedPermission,
      bool checkDescriptorScope = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessDescriptor>(descriptor, nameof (descriptor));
      if (!this.HasProcessPermission(requestContext, requestedPermission, descriptor, checkDescriptorScope))
        throw new ProcessPermissionException();
    }

    public bool HasRootCreatePermission(IVssRequestContext requestContext) => this.HasProcessPermission(requestContext, 4, (ProcessDescriptor) null, true);

    public virtual void AddProcessPermission(
      IVssRequestContext requestContext,
      ProcessDescriptor descriptor,
      int requestedPermission = 15)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.ProcessNamespaceId);
      if (securityNamespace == null)
        return;
      AccessControlEntry accessControlEntry = new AccessControlEntry(requestContext.GetAuthenticatedDescriptor(), requestedPermission, 0);
      string securityToken = this.ComputeSecurityToken(requestContext, descriptor);
      securityNamespace.SetAccessControlEntry(requestContext, securityToken, (IAccessControlEntry) accessControlEntry, false);
    }

    private bool TryGetCachedDescriptor(
      IVssRequestContext requestContext,
      Guid id,
      out ProcessDescriptor descriptor,
      bool fallThrough = true)
    {
      if (requestContext.GetService<ProcessDescriptorCacheService>().TryGetDescriptor(requestContext, id, out descriptor))
        return true;
      if (!fallThrough || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return false;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<TeamFoundationProcessService>().TryGetCachedDescriptor(vssRequestContext, id, out descriptor, false);
    }

    private bool HasManageTemplatesPermissionOnProcessTemplatesNamespace(
      IVssRequestContext requestContext)
    {
      Guid templatesNamespaceId = FrameworkSecurity.ProcessTemplatesNamespaceId;
      string templateNamespaceToken = FrameworkSecurity.ProcessTemplateNamespaceToken;
      int permission = 32;
      return this.HasPermission(requestContext, templatesNamespaceId, templateNamespaceToken, permission);
    }

    private void CheckRootCreatePermission(IVssRequestContext requestContext)
    {
      if (!this.HasRootCreatePermission(requestContext))
        throw new ProcessPermissionException();
      if (!this.IsProcessEnabled(requestContext))
      {
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          if (requestContext.IsSystemContext)
            return;
        }
        else if (requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, WitProvisionSecurity.NamespaceId).HasPermission(requestContext, "$", 1, false))
          return;
        throw new ProcessProvisioningPermissionException();
      }
    }

    private bool HasPermission(
      IVssRequestContext requestContext,
      Guid namespaceId,
      string token,
      int permission,
      bool checkActionDefined = false)
    {
      ITeamFoundationSecurityService service = requestContext.GetService<ITeamFoundationSecurityService>();
      IVssSecurityNamespace securityNamespace = service.GetSecurityNamespace(requestContext, namespaceId);
      if (securityNamespace == null)
      {
        requestContext.ServiceHost.ServiceHostInternal().FlushNotificationQueue(requestContext);
        securityNamespace = service.GetSecurityNamespace(requestContext, namespaceId);
      }
      return securityNamespace.HasPermission(requestContext, token, permission);
    }

    public virtual string GetSecurityToken(
      IVssRequestContext requestContext,
      ProcessDescriptor descriptor = null)
    {
      string securityToken = this.ComputeSecurityToken(requestContext, descriptor);
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        securityToken = securityToken + ProcessConstants.ProcessTokenSuffix.ToString() + ProcessConstants.ProcessSecurityTokenSeparator;
      return securityToken;
    }

    public bool IsProcessEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy") || requestContext.IsFeatureEnabled("WebAccess.Process.ProcessUpload");

    private string ComputeSecurityToken(
      IVssRequestContext requestContext,
      ProcessDescriptor descriptor = null)
    {
      if (descriptor == null)
        return PermissionNamespaces.Process;
      string str;
      if (descriptor.IsDerived)
      {
        ProcessDescriptor processDescriptor = requestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptor(requestContext, descriptor.Inherits);
        str = this.ComputeSecurityToken(requestContext, processDescriptor);
      }
      else
        str = PermissionNamespaces.Process;
      return str + descriptor.TypeId.ToString() + ProcessConstants.ProcessSecurityTokenSeparator;
    }

    private void PublishSqlNotification(
      IVssRequestContext requestContext,
      Guid eventId,
      string eventData)
    {
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, eventId, eventData);
    }

    private static void CheckDescriptionText(string description)
    {
      if (description != null && description.Length > ProcessConstants.MaxDescriptionLength)
        throw new ProcessDescriptionLengthExceededException(description.Length);
    }

    private IReadOnlyCollection<ProjectInfo> GetSubscribedProjectsForProcess(
      IVssRequestContext requestContext,
      Guid processTypeId)
    {
      IProjectService projectService = requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.GetService<IProjectService>() : throw new UnexpectedHostTypeException(TeamFoundationHostType.ProjectCollection);
      IVssRequestContext requestContext1 = requestContext.Elevate();
      IVssRequestContext requestContext2 = requestContext1;
      IEnumerable<ProjectInfo> projects = projectService.GetProjects(requestContext2);
      return (IReadOnlyCollection<ProjectInfo>) requestContext.GetService<ILegacyProjectPropertiesReaderService>().PopulateProperties(projects, requestContext1, ProcessTemplateIdPropertyNames.ProcessTemplateType).Where<ProjectInfo>((Func<ProjectInfo, bool>) (project =>
      {
        switch (project.State)
        {
          case ProjectState.New:
          case ProjectState.WellFormed:
          case ProjectState.CreatePending:
            IList<ProjectProperty> properties = project.Properties;
            Guid result;
            return Guid.TryParse(properties != null ? (string) properties.FirstOrDefault<ProjectProperty>((Func<ProjectProperty, bool>) (prop => StringComparer.OrdinalIgnoreCase.Equals(prop.Name, ProcessTemplateIdPropertyNames.ProcessTemplateType)))?.Value : (string) (object) null, out result) && result == processTypeId;
          default:
            return false;
        }
      })).ToList<ProjectInfo>();
    }

    private static bool IsValidProcessStateTransition(
      ProcessStatus stateFrom,
      ProcessStatus stateTo)
    {
      return stateFrom == ProcessStatus.Ready || stateTo == ProcessStatus.Ready;
    }

    protected virtual Guid GetUserIdentityIdInternal(IVssRequestContext requestContext) => requestContext.GetUserId();

    private int GetProcessCountLimit(IVssRequestContext requestContext) => (requestContext.IsFeatureEnabled("WebAccess.Process.Hierarchy") ? 1 : (requestContext.IsFeatureEnabled("WebAccess.Process.ProcessUpload") ? 1 : 0)) == 0 || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? int.MaxValue : requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Process/Settings/ProcessLimit", 128);

    private void CheckDeploymentLevelNameCollisions(
      IVssRequestContext requestContext,
      string name,
      string referenceName)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IReadOnlyCollection<ProcessDescriptor> processDescriptors = vssRequestContext.GetService<ITeamFoundationProcessService>().GetProcessDescriptors(vssRequestContext, false);
      if (!string.IsNullOrEmpty(name) && processDescriptors.Any<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (t => TFStringComparer.ProcessName.Equals(t.Name, name))))
        throw new ProcessNameConflictException(name);
      if (!string.IsNullOrEmpty(referenceName) && processDescriptors.Any<ProcessDescriptor>((Func<ProcessDescriptor, bool>) (t => TFStringComparer.ProcessName.Equals(t.ReferenceName, referenceName))))
        throw new ProcessNameConflictException(referenceName);
    }

    private void CheckMaxTemplateFileSizeLimitRespected(
      IVssRequestContext requestContext,
      Stream zipContentStream)
    {
      requestContext.GetService<IProcessTemplateValidatorService>().ValidateTemplateFileSizeLimit(requestContext, zipContentStream);
    }

    private ServiceLevel GetServiceLevel(IVssRequestContext requestContext)
    {
      if (!requestContext.IsServicingContext)
      {
        string serviceLevel = requestContext.ServiceHost.ServiceHostInternal().ServiceLevel;
        Dictionary<string, ServiceLevel> serviceLevelMap = ServiceLevel.CreateServiceLevelMap(requestContext, serviceLevel);
        if (serviceLevelMap.Any<KeyValuePair<string, ServiceLevel>>())
          return serviceLevelMap.Values.Max<ServiceLevel>();
      }
      return (ServiceLevel) null;
    }

    private ProcessTemplateComponent CreateComponent(IVssRequestContext requestContext)
    {
      ProcessTemplateComponent component = requestContext.CreateComponent<ProcessTemplateComponent>();
      component.NotificationAuthor = this.m_notificationAuthor;
      return component;
    }
  }
}
