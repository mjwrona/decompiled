// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.ProjectState
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [GenerateAllConstants(null)]
  public enum ProjectState
  {
    [EnumMember] Unchanged = -2, // 0xFFFFFFFE
    [EnumMember] All = -1, // 0xFFFFFFFF
    [EnumMember] New = 0,
    [EnumMember] WellFormed = 1,
    [EnumMember] Deleting = 2,
    [EnumMember] CreatePending = 3,
    [EnumMember] Deleted = 4,
  }
}
