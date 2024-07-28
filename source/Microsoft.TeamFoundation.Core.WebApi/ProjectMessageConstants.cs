// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.ProjectMessageConstants
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  public class ProjectMessageConstants
  {
    public static string MessageTopic = "Microsoft.TeamFoundation.Project.Server";
    public static Guid PublishProjectMessageJob = new Guid("be2076a4-7261-4dc1-b576-e7ea60ef4b45");
    public static readonly string Root = "/Configuration/Project";
    public static readonly string PublishDeletedWatermark = ProjectMessageConstants.Root + "/DeletedWatermark";
    public static readonly string PublishModifiedWatermark = ProjectMessageConstants.Root + "/ModifiedWatermark";
  }
}
