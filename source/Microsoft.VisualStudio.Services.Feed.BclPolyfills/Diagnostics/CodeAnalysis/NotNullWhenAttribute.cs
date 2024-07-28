// Decompiled with JetBrains decompiler
// Type: System.Diagnostics.CodeAnalysis.NotNullWhenAttribute
// Assembly: Microsoft.VisualStudio.Services.Feed.BclPolyfills, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B1AF8C-B841-48B3-9E2E-229EAA8AFCE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.BclPolyfills.dll

namespace System.Diagnostics.CodeAnalysis
{
  [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
  public class NotNullWhenAttribute : Attribute
  {
    public bool ReturnValue { get; }

    public NotNullWhenAttribute(bool returnValue) => this.ReturnValue = returnValue;
  }
}
