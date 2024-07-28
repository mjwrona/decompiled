// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.VersionedModifierAttribute
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
  public abstract class VersionedModifierAttribute : Attribute
  {
    public int MinVersion { get; }

    public int MaxVersion { get; }

    public VersionedModifierAttribute(int inclusiveMinVersion, int inclusiveMaxVersion)
    {
      this.MinVersion = inclusiveMinVersion <= inclusiveMaxVersion ? inclusiveMinVersion : throw new ArgumentException(AnalyticsResources.MINVERSION_GREATER_THAN_MAXVERSION((object) nameof (inclusiveMinVersion), (object) nameof (inclusiveMaxVersion)));
      this.MaxVersion = inclusiveMaxVersion;
    }

    public bool ShouldApply(int currentVersion) => this.MinVersion <= currentVersion && currentVersion <= this.MaxVersion;
  }
}
