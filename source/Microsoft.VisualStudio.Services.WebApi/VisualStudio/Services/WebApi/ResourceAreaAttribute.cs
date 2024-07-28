// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ResourceAreaAttribute
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
  public sealed class ResourceAreaAttribute : Attribute
  {
    public readonly Guid AreaId;

    public ResourceAreaAttribute(string areaId) => this.AreaId = new Guid(areaId);
  }
}
