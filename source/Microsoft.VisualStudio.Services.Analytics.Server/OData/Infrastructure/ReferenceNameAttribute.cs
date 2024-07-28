// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ReferenceNameAttribute
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = true)]
  public class ReferenceNameAttribute : VersionedModifierAttribute
  {
    public ReferenceNameAttribute(string referenceName)
      : this(int.MinValue, int.MaxValue, referenceName)
    {
    }

    public ReferenceNameAttribute(
      int inclusiveMinVersion,
      int inclusiveMaxVersion,
      string referenceName)
      : base(inclusiveMinVersion, inclusiveMaxVersion)
    {
      this.ReferenceName = referenceName;
    }

    public virtual string ReferenceName { get; }

    protected string ReferenceNameValue { get; set; }
  }
}
