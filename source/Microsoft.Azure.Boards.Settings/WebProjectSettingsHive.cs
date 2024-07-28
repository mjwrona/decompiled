// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Settings.WebProjectSettingsHive
// Assembly: Microsoft.Azure.Boards.Settings, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2AC3574E-9414-4605-BAB7-1F6B28A75804
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.Settings.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Boards.Settings
{
  public class WebProjectSettingsHive : WebGlobalSettingsHive
  {
    private Guid m_projectId;

    public WebProjectSettingsHive(IVssRequestContext requestContext, Guid projectId)
      : this(requestContext, (string) null, projectId)
    {
    }

    public WebProjectSettingsHive(
      IVssRequestContext requestContext,
      string cachePattern,
      Guid projectId)
      : base(requestContext, cachePattern)
    {
      this.m_projectId = projectId;
    }

    protected override string Prefix => "/Project/" + this.m_projectId.ToString() + "/WebAccess";
  }
}
