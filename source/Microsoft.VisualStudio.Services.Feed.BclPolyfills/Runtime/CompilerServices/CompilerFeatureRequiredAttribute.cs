// Decompiled with JetBrains decompiler
// Type: System.Runtime.CompilerServices.CompilerFeatureRequiredAttribute
// Assembly: Microsoft.VisualStudio.Services.Feed.BclPolyfills, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B1AF8C-B841-48B3-9E2E-229EAA8AFCE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.BclPolyfills.dll


#nullable enable
namespace System.Runtime.CompilerServices
{
  [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
  public sealed class CompilerFeatureRequiredAttribute : Attribute
  {
    public CompilerFeatureRequiredAttribute(string featureName)
    {
      this.FeatureName = featureName;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public string FeatureName { get; }

    public bool IsOptional { get; init; }
  }
}
