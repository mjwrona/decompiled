// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Annotation
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class Annotation
  {
    internal int identityId;
    private string m_annotatedItem;
    private int m_version;
    private string m_annotationName;
    private string m_annotationValue;
    private string m_comment;
    private DateTime m_lastModifiedDate;

    public Annotation()
    {
    }

    internal Annotation(
      string annotatedItem,
      string annotationName,
      string annotationValue,
      string comment,
      DateTime lastModifiedDate)
    {
      this.m_annotatedItem = annotatedItem;
      this.m_annotationName = annotationName;
      this.m_annotationValue = annotationValue;
      this.m_comment = comment;
      this.m_lastModifiedDate = lastModifiedDate;
    }

    [XmlAttribute("item")]
    public string AnnotatedItem
    {
      get => this.m_annotatedItem;
      set => this.m_annotatedItem = value;
    }

    [XmlAttribute("v")]
    public int Version
    {
      get => this.m_version;
      set => this.m_version = value;
    }

    [XmlAttribute("name")]
    public string AnnotationName
    {
      get => this.m_annotationName;
      set => this.m_annotationName = value;
    }

    [XmlAttribute("value")]
    public string AnnotationValue
    {
      get => this.m_annotationValue;
      set => this.m_annotationValue = value;
    }

    public string Comment
    {
      get => this.m_comment;
      set => this.m_comment = value;
    }

    [XmlAttribute("date")]
    public DateTime LastModifiedDate
    {
      get => this.m_lastModifiedDate;
      set => this.m_lastModifiedDate = value;
    }

    internal static void CreateAnnotation(
      VersionControlRequestContext versionControlRequestContext,
      string annotationName,
      string annotatedItem,
      int version,
      string annotationValue,
      string comment,
      bool overwrite)
    {
      if (annotatedItem == null && version == 0)
        throw new ArgumentException(Resources.Get("GlobalAnnotationsNotSupported"));
      if (annotationName != null && annotationName.Equals("CheckinPolicies", StringComparison.OrdinalIgnoreCase))
      {
        if (versionControlRequestContext.RequestContext.IsFeatureEnabled("SourceControl.DisableOldTfvcCheckinPolicies"))
        {
          versionControlRequestContext.RequestContext.TraceAlways(700357, TraceLevel.Info, TraceArea.General, TraceLayer.BusinessLogic, "{0} annotation cannot be changed because it has been globally disabled", (object) annotationName);
          throw new ArgumentException(Resources.Get("OldCheckinPoliciesDisabled"));
        }
        if (versionControlRequestContext.RequestContext.IsFeatureEnabled("SourceControl.NewToggleToSwitchOldCheckinPoliciesSaving"))
        {
          Guid projectIdFromPath = ProjectUtility.GetProjectIdFromPath(versionControlRequestContext.RequestContext, annotatedItem);
          if (projectIdFromPath == Guid.Empty)
            throw new TeamProjectNotFoundException(annotatedItem);
          if (VersionControlSettingService.ReadDisableOldTfvcCheckinPolicies(versionControlRequestContext.RequestContext, projectIdFromPath))
          {
            versionControlRequestContext.RequestContext.TraceAlways(700358, TraceLevel.Info, TraceArea.General, TraceLayer.BusinessLogic, "{0} annotation cannot be changed because it has been disabled for {1} project", (object) annotationName, (object) annotatedItem);
            throw new ArgumentException(Resources.Get("OldCheckinPoliciesDisabled"));
          }
        }
      }
      if (versionControlRequestContext.VersionControlService.GetAllowAsynchronousCheckoutInServerWorkspaces(versionControlRequestContext) && string.Equals(annotationValue, bool.TrueString, StringComparison.OrdinalIgnoreCase))
      {
        if (annotationName.Equals("GetLatestOnCheckout", StringComparison.OrdinalIgnoreCase))
          throw new GetLatestOnCheckoutDisabledException();
        if (annotationName.Equals("ExclusiveCheckout", StringComparison.OrdinalIgnoreCase))
          throw new CheckoutLocksDisabledException();
      }
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.AdminProjectRights, annotatedItem);
      using (CommandSetAnnotationProperty annotationProperty = new CommandSetAnnotationProperty(versionControlRequestContext))
        annotationProperty.Execute(annotatedItem, version, annotationName, annotationValue, comment, new DateTime?(DateTime.Now.ToUniversalTime()));
      versionControlRequestContext.RequestContext.TraceAlways(700355, TraceLevel.Info, TraceArea.General, TraceLayer.BusinessLogic, "{0} annotation for {1} has been created", (object) annotationName, (object) annotatedItem);
      if (annotatedItem == null || annotationName == null || !annotationName.Equals("GetLatestOnCheckout", StringComparison.OrdinalIgnoreCase) && !annotationName.Equals("ExclusiveCheckout", StringComparison.OrdinalIgnoreCase) || !VersionControlPath.IsServerItem(annotatedItem) || VersionControlPath.GetFolderDepth(annotatedItem) != 1)
        return;
      versionControlRequestContext.VersionControlService.GetTeamProjectFolder(versionControlRequestContext).RefreshTeamProjectCache(versionControlRequestContext);
    }

    internal static void DeleteAnnotation(
      VersionControlRequestContext versionControlRequestContext,
      string annotationName,
      string annotatedItem,
      int version,
      string annotationValue)
    {
      if (annotatedItem == null && version == 0)
        throw new ArgumentException(Resources.Get("GlobalAnnotationsNotSupported"));
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.AdminProjectRights, annotatedItem);
      using (CommandSetAnnotationProperty annotationProperty = new CommandSetAnnotationProperty(versionControlRequestContext))
        annotationProperty.Execute(annotatedItem, version, annotationName, (string) null, (string) null, new DateTime?());
      if (annotatedItem == null || annotationName == null || !annotationName.Equals("GetLatestOnCheckout", StringComparison.OrdinalIgnoreCase) && !annotationName.Equals("ExclusiveCheckout", StringComparison.OrdinalIgnoreCase) || !VersionControlPath.IsServerItem(annotatedItem) || VersionControlPath.GetFolderDepth(annotatedItem) != 1)
        return;
      versionControlRequestContext.VersionControlService.GetTeamProjectFolder(versionControlRequestContext).RefreshTeamProjectCache(versionControlRequestContext);
    }

    internal static List<Annotation> QueryAnnotation(
      VersionControlRequestContext versionControlRequestContext,
      string annotationName,
      string annotatedItem,
      int version)
    {
      using (CommandGetAnnotationProperty annotationProperty = new CommandGetAnnotationProperty(versionControlRequestContext))
      {
        annotationProperty.Execute(annotationName, annotatedItem, version);
        versionControlRequestContext.RequestContext.TraceAlways(700356, TraceLevel.Info, TraceArea.General, TraceLayer.BusinessLogic, "{0} annotation for {1} has been queried", (object) annotationName, (object) annotatedItem);
        return annotationProperty.Result;
      }
    }
  }
}
