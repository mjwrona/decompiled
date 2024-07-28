// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.WorkItemTrackingWebConstants
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using System;

namespace Microsoft.Azure.Boards.WebApi.Common
{
  public class WorkItemTrackingWebConstants
  {
    public const string RestArea = "wit";
    public static readonly Guid WorkItemArtifactKind = new Guid("E7626DBD-6075-416C-A31E-DFD48FE3CFDE");
    public const string JsonPatchMediaType = "application/json-patch+json";
    public const string ApiResourceVersion = "WitApiResourceVersion";
    public const string ProcessesArea = "processes";
    public const string ProcessDefinitionsArea = "processDefinitions";
    public const string ProcessAdminArea = "processAdmin";

    public static string RestAreaId => "5264459E-E5E0-4BD8-B118-0985E68A4EC5";
  }
}
