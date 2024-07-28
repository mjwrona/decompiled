// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.Client.ResourceType
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.PermissionsReport.Client
{
  [DataContract]
  public enum ResourceType
  {
    [ClientInternalUseOnly(false), EnumMember(Value = "unspecified")] Unspecified,
    [ClientInternalUseOnly(false), EnumMember(Value = "collection")] Collection,
    [ClientInternalUseOnly(false), EnumMember(Value = "project")] Project,
    [EnumMember(Value = "repo")] Repo,
    [EnumMember(Value = "ref")] Ref,
    [EnumMember(Value = "projectGit")] ProjectGit,
    [EnumMember(Value = "release")] Release,
    [EnumMember(Value = "tfvc")] Tfvc,
  }
}
