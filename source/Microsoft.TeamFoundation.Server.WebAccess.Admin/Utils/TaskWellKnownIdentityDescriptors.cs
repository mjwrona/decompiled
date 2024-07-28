// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Utils.TaskWellKnownIdentityDescriptors
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Utils
{
  internal static class TaskWellKnownIdentityDescriptors
  {
    public static readonly IdentityDescriptor EndpointAdministratorsDescriptor = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(TaskWellKnownSecurityIds.EndpointAdministratorsGroupId);
    public static readonly IdentityDescriptor EndpointCreatorsDescriptors = IdentityHelper.CreateReadOnlyTeamFoundationDescriptor(TaskWellKnownSecurityIds.ÈndpointCreatorsGroupId);
  }
}
