// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectNotifications
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal static class ProjectNotifications
  {
    public static readonly Guid TeamProjectChanged = new Guid("A18D6062-1C4B-40FC-8401-D351F4DDC34B");
    public static readonly Guid TeamProjectDeleted = new Guid("8DFF6916-1FB2-42AE-B348-B6F7096BDAEA");
    public static readonly Guid ProjectUpdateJob = new Guid("51E260B7-5405-4061-8F4B-2566C00D7AD1");
  }
}
