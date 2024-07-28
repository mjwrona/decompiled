// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.TeamFoundationJobReferences
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class TeamFoundationJobReferences
  {
    public static readonly TeamFoundationJobReference IdentitySyncNow = new TeamFoundationJobReference(new Guid("68D12C31-4894-49c3-8E12-4D3E954C98E7"), JobPriorityClass.High);
  }
}
