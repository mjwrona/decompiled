// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Xaml.XamlProjectInfoExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Xaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 48A241AC-D20F-49E0-A581-C219E1ED7760
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Xaml.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.Xaml
{
  public static class XamlProjectInfoExtensions
  {
    public static Guid SafeGetId(this ProjectInfo projectInfo) => projectInfo == null ? Guid.Empty : projectInfo.Id;

    public static string SafeGetName(this ProjectInfo projectInfo, string defaultValue = null) => projectInfo == null ? defaultValue : projectInfo.Name;
  }
}
