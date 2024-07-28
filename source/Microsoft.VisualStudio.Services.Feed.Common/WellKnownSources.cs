// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.WellKnownSources
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public static class WellKnownSources
  {
    public static WellKnownUpstreamSource Npmjs { get; } = new WellKnownUpstreamSource("npm", WellKnownSources.Tag.NpmJs, "npmjs", new Uri("https://registry.npmjs.org/"));

    public static WellKnownUpstreamSource NugetOrg { get; } = new WellKnownUpstreamSource("nuget", WellKnownSources.Tag.NuGetOrg, "NuGet Gallery", new Uri("https://api.nuget.org/v3/index.json"));

    public static WellKnownUpstreamSource PowerShellGallery { get; } = new WellKnownUpstreamSource("nuget", WellKnownSources.Tag.PSGallery, "PowerShell Gallery", new Uri("https://www.powershellgallery.com/api/v2/"));

    public static WellKnownUpstreamSource PyPiOrg { get; } = new WellKnownUpstreamSource("pypi", WellKnownSources.Tag.PyPI, "PyPI", new Uri("https://pypi.org/"));

    public static WellKnownUpstreamSource MavenCentralOrg { get; } = new WellKnownUpstreamSource("Maven", WellKnownSources.Tag.MavenCentral, "Maven Central", new Uri("https://repo.maven.apache.org/maven2/"));

    public static WellKnownUpstreamSource MavenGoogleRepo { get; } = new WellKnownUpstreamSource("Maven", WellKnownSources.Tag.GoogleMaven, "Google Maven Repository", new Uri("https://dl.google.com/android/maven2/"));

    public static WellKnownUpstreamSource MavenJitPackRepo { get; } = new WellKnownUpstreamSource("Maven", WellKnownSources.Tag.JitPack, "JitPack", new Uri("https://jitpack.io/"));

    public static WellKnownUpstreamSource MavenGradlePluginRepo { get; } = new WellKnownUpstreamSource("Maven", WellKnownSources.Tag.GradlePlugins, "Gradle Plugins", new Uri("https://plugins.gradle.org/m2/"));

    public static WellKnownUpstreamSource CargoCrates { get; } = new WellKnownUpstreamSource("Cargo", WellKnownSources.Tag.CratesIo, "crates.io", new Uri("https://index.crates.io/"));

    public static WellKnownUpstreamSource AnacondaCom { get; } = new WellKnownUpstreamSource("Conda", WellKnownSources.Tag.AnacondaCom, "Anaconda.com", new Uri("https://repo.anaconda.com/"));

    public static WellKnownUpstreamSource AnacondaOrg { get; } = new WellKnownUpstreamSource("Conda", WellKnownSources.Tag.AnacondaOrg, "Anaconda.org", new Uri("https://conda.anaconda.org/"));

    internal static IReadOnlyList<WellKnownUpstreamSource> All { get; } = (IReadOnlyList<WellKnownUpstreamSource>) new WellKnownUpstreamSource[11]
    {
      WellKnownSources.Npmjs,
      WellKnownSources.NugetOrg,
      WellKnownSources.PowerShellGallery,
      WellKnownSources.PyPiOrg,
      WellKnownSources.MavenCentralOrg,
      WellKnownSources.MavenGoogleRepo,
      WellKnownSources.MavenJitPackRepo,
      WellKnownSources.MavenGradlePluginRepo,
      WellKnownSources.CargoCrates,
      WellKnownSources.AnacondaCom,
      WellKnownSources.AnacondaOrg
    };

    public enum Tag : byte
    {
      Invalid,
      NpmJs,
      NuGetOrg,
      PSGallery,
      PyPI,
      MavenCentral,
      GoogleMaven,
      JitPack,
      GradlePlugins,
      CratesIo,
      AnacondaCom,
      AnacondaOrg,
    }
  }
}
