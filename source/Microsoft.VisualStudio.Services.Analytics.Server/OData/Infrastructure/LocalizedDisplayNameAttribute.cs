// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.LocalizedDisplayNameAttribute
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
  public class LocalizedDisplayNameAttribute : DisplayNameAttribute
  {
    private string _key;

    public bool Force { get; set; }

    public LocalizedDisplayNameAttribute(string key, bool force = false)
      : base(AnalyticsResources.Manager.GetString(key))
    {
      this._key = key;
      this.Force = force;
    }
  }
}
