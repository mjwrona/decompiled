// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ResourceAccessException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class ResourceAccessException : SecurityException
  {
    internal string m_identityName;
    internal string m_permissionName;
    internal string m_resourceName;

    public ResourceAccessException(
      string customExceptionMessage,
      string identityName,
      string permissionName,
      string resourceName)
      : base(string.Format((IFormatProvider) CultureInfo.InvariantCulture, customExceptionMessage, (object) identityName, (object) permissionName, (object) resourceName))
    {
      this.m_identityName = identityName;
      this.m_permissionName = permissionName;
      this.m_resourceName = resourceName;
      this.EventId = TeamFoundationEventId.ResourceAccessException;
      this.LogLevel = EventLogEntryType.Warning;
    }

    public ResourceAccessException(string identityName, string permissionName, string resourceName)
      : base(Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Format(nameof (ResourceAccessException), (object) identityName, (object) permissionName, (object) resourceName))
    {
      this.m_identityName = identityName;
      this.m_permissionName = permissionName;
      this.m_resourceName = resourceName;
      this.EventId = TeamFoundationEventId.ResourceAccessException;
      this.LogLevel = EventLogEntryType.Warning;
    }

    public ResourceAccessException(string identityName, string permissionName)
      : base(Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Format("GlobalResourceAccessException", (object) identityName, (object) permissionName))
    {
      this.m_identityName = identityName;
      this.m_permissionName = permissionName;
      this.EventId = TeamFoundationEventId.ResourceAccessException;
      this.LogLevel = EventLogEntryType.Warning;
    }

    public string Identity => this.m_identityName;

    public string Permission => this.m_permissionName;

    public string Resource => this.m_resourceName;

    public override void SetFailureInfo(Failure failure)
    {
      base.SetFailureInfo(failure);
      failure.IdentityName = this.m_identityName;
      failure.ResourceName = this.m_resourceName;
    }
  }
}
