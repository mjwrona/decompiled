// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.RemoteLinkConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public static class RemoteLinkConstants
  {
    private const string RegistryPathRemoteLinkingRoot = "/Service/WorkItemTracking/RemoteLinking";
    public static readonly string RegistryPathRemoteDeletedRoot = "/Service/WorkItemTracking/RemoteLinking/RemoteDeletedProjects";
    public static readonly string RegistryPathLocalDeletedRoot = "/Service/WorkItemTracking/RemoteLinking/LocalDeletedProjects";
    public static readonly string RegistryPathRemoteDeletedAll = RemoteLinkConstants.RegistryPathRemoteDeletedRoot + "/...";
    public static readonly string RegistryPathLocalDeletedAll = RemoteLinkConstants.RegistryPathLocalDeletedRoot + "/...";
  }
}
