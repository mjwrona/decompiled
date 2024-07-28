// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.DefaultServiceImplementationAttribute
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using System;

namespace Microsoft.VisualStudio.Services.Client
{
  [Obsolete("Please use class Microsoft.VisualStudio.Services.WebApi.VssClientServiceImplementationAttribute instead.")]
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
  public sealed class DefaultServiceImplementationAttribute : Attribute
  {
    public DefaultServiceImplementationAttribute(Type type) => this.Type = type;

    public DefaultServiceImplementationAttribute(string typeName) => this.TypeName = typeName;

    public Type Type { get; set; }

    public string TypeName { get; set; }
  }
}
