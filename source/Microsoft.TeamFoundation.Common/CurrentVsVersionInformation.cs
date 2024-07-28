// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CurrentVsVersionInformation
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation
{
  public static class CurrentVsVersionInformation
  {
    public const string AssemblyVersion = "19.0.0.0";
    public const string Version = "17.0";
    public const string ProgId = "VisualStudio.17.0";
    internal const string TfsVersion = "11.0";
    public const string RegistryKeyPath = "Software\\Microsoft\\VisualStudio\\17.0";
    public const string AppDataPath = "Microsoft\\VisualStudio\\17.0";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string AppId = "VisualStudio";
    [Obsolete("This constant will be removed in TFS 2018.")]
    public const string TFSRegistryKeyPath = "Software\\Microsoft\\TeamFoundationServer\\17.0";
    public const string TFSCommonFilesPath = "Microsoft Shared\\Team Foundation Server\\17.0";
  }
}
