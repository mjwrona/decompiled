// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationJobDefinitionTemplate
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public interface ITeamFoundationJobDefinitionTemplate
  {
    TeamFoundationHostType HostType { get; }

    Guid JobId { get; }

    string JobName { get; }

    string PluginType { get; }

    XmlNode JobData { get; }

    TeamFoundationJobEnabledState EnabledState { get; }

    TeamFoundationJobDefinitionTemplateFlags Flags { get; }

    JobPriorityClass PriorityClass { get; }

    IEnumerable<ITeamFoundationJobScheduleTemplate> Schedules { get; }
  }
}
