// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Utility.ShallowReferenceConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Utility
{
  public static class ShallowReferenceConverter
  {
    public static ReleaseShallowReference ToReleaseShallowReference(
      this ShallowReference shallowReference)
    {
      if (shallowReference == null)
        return (ReleaseShallowReference) null;
      return new ReleaseShallowReference()
      {
        Id = shallowReference.Id,
        Name = shallowReference.Name,
        Url = shallowReference.Url,
        Links = shallowReference.Links
      };
    }

    public static ReleaseEnvironmentShallowReference ToReleaseEnvironmentShallowReference(
      this ShallowReference shallowReference)
    {
      if (shallowReference == null)
        return (ReleaseEnvironmentShallowReference) null;
      return new ReleaseEnvironmentShallowReference()
      {
        Id = shallowReference.Id,
        Name = shallowReference.Name,
        Url = shallowReference.Url,
        Links = shallowReference.Links
      };
    }

    public static ReleaseDefinitionShallowReference ToReleaseDefinitionShallowReference(
      this ShallowReference shallowReference)
    {
      if (shallowReference == null)
        return (ReleaseDefinitionShallowReference) null;
      return new ReleaseDefinitionShallowReference()
      {
        Id = shallowReference.Id,
        Name = shallowReference.Name,
        Url = shallowReference.Url,
        Links = shallowReference.Links
      };
    }
  }
}
