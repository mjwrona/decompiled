// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.WellKnownArtifactResourceTypes
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [Obsolete("Use ArtifactResourceTypes instead.")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class WellKnownArtifactResourceTypes
  {
    public const string FilePath = "FilePath";
    public const string SymbolStore = "SymbolStore";
    public const string VersionControl = "VersionControl";
    public const string Container = "Container";
    public const string GitRef = "GitRef";
    public const string TfvcLabel = "TfvcLabel";
    public const string SymbolRequest = "SymbolRequest";
  }
}
