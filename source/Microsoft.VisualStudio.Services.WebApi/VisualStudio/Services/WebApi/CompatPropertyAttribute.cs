// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.CompatPropertyAttribute
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
  public sealed class CompatPropertyAttribute : Attribute
  {
    public CompatPropertyAttribute(string oldName, int majorApiVersion, int minorApiVersion = 0)
    {
      this.OldName = oldName;
      this.MaxApiVersion = new Version(majorApiVersion, minorApiVersion);
    }

    public string OldName { get; private set; }

    public Version MaxApiVersion { get; private set; }
  }
}
