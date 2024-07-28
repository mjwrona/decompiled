// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryInstallationTargets
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal static class GalleryInstallationTargets
  {
    public static readonly HashSet<InstallationTarget> VstsInstallationTargets = new HashSet<InstallationTarget>((IEqualityComparer<InstallationTarget>) new GalleryInstallationTargets.InstallationTargetComparer())
    {
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Services"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Services.Resource.Cloud"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Services.Cloud"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.TeamFoundation.Server"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Services.Integration"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Services.Cloud.Integration"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.TeamFoundation.Server.Integration"
      }
    };
    public static readonly HashSet<InstallationTarget> VsCodeInstallationTargets = new HashSet<InstallationTarget>((IEqualityComparer<InstallationTarget>) new GalleryInstallationTargets.InstallationTargetComparer())
    {
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Code"
      }
    };
    public static readonly HashSet<InstallationTarget> VsInstallationTargets = new HashSet<InstallationTarget>((IEqualityComparer<InstallationTarget>) new GalleryInstallationTargets.InstallationTargetComparer())
    {
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Ide"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.VSWinDesktopExpress"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.VSWinExpress"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.VWDExpress"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Community"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Pro"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Enterprise"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.IntegratedShell"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Isolated"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Test"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Ultimate"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Premium"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.VST_All"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.VSLS"
      },
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.VPDExpress"
      }
    };
    public static readonly HashSet<InstallationTarget> VsForMacInstallationTargets = new HashSet<InstallationTarget>((IEqualityComparer<InstallationTarget>) new GalleryInstallationTargets.InstallationTargetComparer())
    {
      new InstallationTarget()
      {
        Target = "Microsoft.VisualStudio.Mac"
      }
    };

    internal class InstallationTargetComparer : IEqualityComparer<InstallationTarget>
    {
      public bool Equals(InstallationTarget target1, InstallationTarget target2) => string.Equals(target1.Target, target2.Target, StringComparison.OrdinalIgnoreCase);

      public int GetHashCode(InstallationTarget target) => target.Target.ToLower().GetHashCode();
    }
  }
}
