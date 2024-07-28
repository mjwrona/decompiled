// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspaceCache
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class WorkspaceCache : VersionControlWebCache<WorkspaceInternal>
  {
    private ITeamFoundationSqlNotificationService m_sqlNotificationService;

    public WorkspaceCache(VersionControlRequestContext vcRequestContext)
      : base(vcRequestContext.VersionControlService)
    {
      this.m_sqlNotificationService = vcRequestContext.Elevate().RequestContext.GetService<ITeamFoundationSqlNotificationService>();
      ArgumentUtility.CheckForNull<ITeamFoundationSqlNotificationService>(this.m_sqlNotificationService, "versionControlService.SystemRequestContext.RequestContext.GetService<ITeamFoundationSqlNotificationService>()");
      this.RegisterNotification(vcRequestContext);
    }

    public void Unload(IVssRequestContext systemRequestContext) => this.UnRegisterNotification(systemRequestContext);

    protected override string FullyQualifyKey(string providedKey) => base.FullyQualifyKey("WorkspaceCache, " + providedKey);

    private void RegisterNotification(VersionControlRequestContext vcRequestContext) => this.m_sqlNotificationService.RegisterNotification(vcRequestContext.RequestContext, DatabaseCategories.VersionControl, SqlNotificationEventClasses.WorkspaceChanged, new SqlNotificationCallback(this.OnWorkspaceChanged), true);

    private void UnRegisterNotification(IVssRequestContext requestContext) => this.m_sqlNotificationService.UnregisterNotification(requestContext.Elevate(), DatabaseCategories.VersionControl, SqlNotificationEventClasses.WorkspaceChanged, new SqlNotificationCallback(this.OnWorkspaceChanged), false);

    private void OnWorkspaceChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventDataXml)
    {
      try
      {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        using (StringReader input = new StringReader(eventDataXml))
        {
          using (XmlReader xmlReader = XmlReader.Create((TextReader) input, settings))
          {
            if (!xmlReader.Read())
              return;
            string attribute1 = xmlReader.GetAttribute("ownerId");
            string attribute2 = xmlReader.GetAttribute("oldName");
            Guid ownerId = new Guid(attribute1);
            WorkspaceInternal.RemoveFromCache(new VersionControlRequestContext(requestContext), ownerId, attribute2);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(700203, TraceArea.WorkspaceCache, TraceLayer.BusinessLogic, ex);
      }
    }
  }
}
