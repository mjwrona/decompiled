// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.FileTypeManager
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class FileTypeManager
  {
    private ITeamFoundationSqlNotificationService m_sqlNotificationService;
    private Dictionary<string, bool> m_extensions;
    private ReaderWriterLock m_rwLock = new ReaderWriterLock();
    private bool m_enableExclusiveCheckout = true;

    internal FileTypeManager(
      VersionControlRequestContext versionControlRequestContext)
    {
      ArgumentUtility.CheckForNull<VersionControlRequestContext>(versionControlRequestContext, nameof (versionControlRequestContext));
      this.m_rwLock.AcquireWriterLock(-1);
      try
      {
        this.RefreshExtensionCache(versionControlRequestContext);
      }
      finally
      {
        this.m_rwLock.ReleaseWriterLock();
      }
      this.m_sqlNotificationService = versionControlRequestContext.Elevate().RequestContext.GetService<ITeamFoundationSqlNotificationService>();
      ArgumentUtility.CheckForNull<ITeamFoundationSqlNotificationService>(this.m_sqlNotificationService, "versionControlService.SystemRequestContext.RequestContext.GetService<ITeamFoundationSqlNotificationService>()");
      this.RegisterNotification(versionControlRequestContext);
    }

    public void Unload(IVssRequestContext requestContext) => this.UnregisterNotification(requestContext);

    internal void SetFileTypes(
      VersionControlRequestContext versionControlRequestContext,
      FileType[] fileTypes)
    {
      versionControlRequestContext.RequestContext.TraceEnter(700075, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, nameof (SetFileTypes));
      try
      {
        versionControlRequestContext.VersionControlService.SecurityWrapper.CheckGlobalPermission(versionControlRequestContext, GlobalPermissions.AdminConfiguration);
        Dictionary<string, FileType> dictionary1 = new Dictionary<string, FileType>((IEqualityComparer<string>) FileSpec.PartialPathComparer);
        Dictionary<string, FileType> dictionary2 = new Dictionary<string, FileType>((IEqualityComparer<string>) TFStringComparer.FileType);
        bool serverWorkspaces = versionControlRequestContext.VersionControlService.GetAllowAsynchronousCheckoutInServerWorkspaces(versionControlRequestContext);
        foreach (FileType fileType1 in fileTypes)
        {
          versionControlRequestContext.RequestContext.Trace(700076, TraceLevel.Verbose, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, "FileType: {0}", (object) fileType1);
          if (serverWorkspaces && !fileType1.AllowMultipleCheckout)
            throw new CheckoutLocksDisabledException();
          if (dictionary2.ContainsKey(fileType1.Name))
            throw new DuplicateFileTypeException(fileType1.Name);
          dictionary2.Add(fileType1.Name, fileType1);
          for (int index = 0; index < fileType1.Extensions.Count; ++index)
          {
            string extension = fileType1.Extensions[index];
            string str = FileSpec.IsLegalNtfsName(extension, 256) && extension.Length < 256 ? extension.TrimStart('.') : throw new InvalidExtensionException(extension);
            fileType1.Extensions[index] = str;
            FileType fileType2;
            if (dictionary1.TryGetValue(str, out fileType2))
              throw new DuplicateExtensionException(str, fileType2.Name, fileType1.Name);
            dictionary1.Add(str, fileType1);
          }
        }
        List<FileType> fileTypeList;
        using (ConfigurationResourceComponent resourceComponent = versionControlRequestContext.VersionControlService.GetConfigurationResourceComponent(versionControlRequestContext))
          fileTypeList = resourceComponent.QueryFileTypes();
        Dictionary<string, FileType> dictionary3 = new Dictionary<string, FileType>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
        foreach (FileType fileType in fileTypeList)
        {
          if (fileType.Id < 50000)
          {
            string key = Resources.Get(fileType.Name);
            if (!string.IsNullOrEmpty(key))
              dictionary3.Add(key, fileType);
          }
        }
        versionControlRequestContext.RequestContext.Trace(700077, TraceLevel.Verbose, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, "Match incoming file types with existing ones.");
        int num = 50000;
        foreach (FileType fileType3 in fileTypes)
        {
          FileType fileType4;
          if (dictionary3.TryGetValue(fileType3.Name, out fileType4))
          {
            fileType3.Name = fileType4.Name;
            fileType3.Id = fileType4.Id;
          }
          else
            fileType3.Id = num++;
          versionControlRequestContext.RequestContext.Trace(700078, TraceLevel.Verbose, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, "Result FileType: {0}", (object) fileType3);
        }
        this.m_rwLock.AcquireWriterLock(-1);
        try
        {
          using (ConfigurationResourceComponent resourceComponent = versionControlRequestContext.VersionControlService.GetConfigurationResourceComponent(versionControlRequestContext))
            resourceComponent.SetFileTypes(fileTypes);
          this.RefreshExtensionCache(versionControlRequestContext);
        }
        finally
        {
          this.m_rwLock.ReleaseWriterLock();
        }
      }
      finally
      {
        versionControlRequestContext.RequestContext.TraceLeave(700079, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, nameof (SetFileTypes));
      }
    }

    internal List<FileType> Query(
      VersionControlRequestContext versionControlRequestContext)
    {
      using (ConfigurationResourceComponent resourceComponent = versionControlRequestContext.VersionControlService.GetConfigurationResourceComponent(versionControlRequestContext))
      {
        List<FileType> fileTypeList = resourceComponent.QueryFileTypes();
        foreach (FileType fileType in fileTypeList)
        {
          if (fileType.Id < 50000)
          {
            string str = Resources.Get(fileType.Name);
            if (!string.IsNullOrEmpty(str))
              fileType.Name = str;
          }
          if (versionControlRequestContext.VersionControlService.GetAllowAsynchronousCheckoutInServerWorkspaces(versionControlRequestContext))
            fileType.AllowMultipleCheckout = true;
        }
        return fileTypeList;
      }
    }

    internal void EnableExclusiveCheckout(bool enableExclusiveCheckout) => this.m_enableExclusiveCheckout = enableExclusiveCheckout;

    internal bool IsExclusiveCheckout(IVssRequestContext requestContext, string serverItem)
    {
      requestContext.TraceEnter(700080, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, nameof (IsExclusiveCheckout));
      try
      {
        if (serverItem == null)
          return false;
        requestContext.Trace(700081, TraceLevel.Verbose, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, "Entering {0}: {1}", (object) nameof (IsExclusiveCheckout), (object) serverItem);
        if (!this.m_enableExclusiveCheckout)
        {
          requestContext.Trace(700082, TraceLevel.Verbose, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (IsExclusiveCheckout), (object) "!m_enableExclusiveCheckout");
          return false;
        }
        int startIndex = serverItem.LastIndexOf('/');
        if (startIndex < 0)
        {
          requestContext.Trace(700083, TraceLevel.Verbose, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, "Leaving {0}: {1}", (object) nameof (IsExclusiveCheckout), (object) "startIndex < 0");
          return false;
        }
        this.m_rwLock.AcquireReaderLock(-1);
        try
        {
          while ((startIndex = serverItem.IndexOf('.', startIndex) + 1) > 0)
          {
            string key = serverItem.Substring(startIndex);
            bool flag;
            if (this.m_extensions.TryGetValue(key, out flag))
            {
              requestContext.Trace(700084, TraceLevel.Verbose, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, "Leaving {0}: Matched extension {1}: {2}", (object) nameof (IsExclusiveCheckout), (object) key, (object) flag);
              return flag;
            }
          }
        }
        finally
        {
          this.m_rwLock.ReleaseReaderLock();
        }
        return false;
      }
      finally
      {
        requestContext.TraceLeave(700085, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, nameof (IsExclusiveCheckout));
      }
    }

    private void RefreshExtensionCache(
      VersionControlRequestContext versionControlRequestContext)
    {
      versionControlRequestContext.RequestContext.TraceEnter(700086, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, nameof (RefreshExtensionCache));
      try
      {
        this.m_extensions = new Dictionary<string, bool>((IEqualityComparer<string>) FileSpec.PartialPathComparer);
        using (ConfigurationResourceComponent resourceComponent = versionControlRequestContext.VersionControlService.GetConfigurationResourceComponent(versionControlRequestContext.Elevate()))
        {
          foreach (FileType queryFileType in resourceComponent.QueryFileTypes())
          {
            foreach (string extension in queryFileType.Extensions)
            {
              versionControlRequestContext.RequestContext.Trace(700087, TraceLevel.Verbose, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, "Loading extension: {0} {1}", (object) extension, (object) !queryFileType.AllowMultipleCheckout);
              this.m_extensions.Add(extension, !queryFileType.AllowMultipleCheckout);
            }
          }
        }
      }
      finally
      {
        versionControlRequestContext.RequestContext.TraceLeave(700088, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, nameof (RefreshExtensionCache));
      }
    }

    private void RegisterNotification(
      VersionControlRequestContext versionControlRequestContext)
    {
      this.m_sqlNotificationService.RegisterNotification(versionControlRequestContext.RequestContext, DatabaseCategories.VersionControl, SqlNotificationEventClasses.FileTypeChanged, new SqlNotificationCallback(this.OnFileTypeChanged), true);
    }

    private void UnregisterNotification(IVssRequestContext requestContext)
    {
      if (this.m_sqlNotificationService == null)
        return;
      this.m_sqlNotificationService.UnregisterNotification(requestContext, DatabaseCategories.VersionControl, SqlNotificationEventClasses.FileTypeChanged, new SqlNotificationCallback(this.OnFileTypeChanged), false);
      this.m_sqlNotificationService = (ITeamFoundationSqlNotificationService) null;
    }

    private void OnFileTypeChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(700089, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, nameof (OnFileTypeChanged));
      try
      {
        requestContext.Trace(700090, TraceLevel.Verbose, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, "Entering {0}: {1} {2}", (object) "OnSettingsChanged", (object) eventClass, (object) eventData);
        this.m_rwLock.AcquireWriterLock(-1);
        try
        {
          this.RefreshExtensionCache(new VersionControlRequestContext(requestContext.Elevate()));
        }
        finally
        {
          this.m_rwLock.ReleaseWriterLock();
        }
      }
      finally
      {
        requestContext.TraceLeave(700091, TraceArea.FileTypeManager, TraceLayer.BusinessLogic, nameof (OnFileTypeChanged));
      }
    }
  }
}
