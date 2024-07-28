// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.BuildTypeConstants
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class BuildTypeConstants
  {
    public static readonly string AvailableConfigurations = "Debug,Release";
    public static readonly string AnyCpu = "Any CPU";
    public static readonly string AvailablePlatforms = "x86,x64,Itanium,Win32,Any CPU,.NET,Mixed Platforms,ARM";
    public static readonly string WorkspaceTemplateFileName = "WorkspaceMapping.xml";
  }
}
