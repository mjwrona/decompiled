// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ShallowReferenceExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ShallowReferenceExtensions
  {
    public static ReleaseShallowReference ToReleaseShallowReference(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference shallowReference)
    {
      if (shallowReference == null)
        return (ReleaseShallowReference) null;
      return new ReleaseShallowReference()
      {
        Id = shallowReference.Id,
        Name = shallowReference.Name,
        Url = shallowReference.Url
      };
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference ToDefinitionShallowReference(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference shallowReference,
      string path)
    {
      if (shallowReference == null)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference) null;
      return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference()
      {
        Id = shallowReference.Id,
        Name = shallowReference.Name,
        Url = shallowReference.Url,
        Path = path
      };
    }

    public static ReleaseEnvironmentShallowReference ToReleaseEnvironmentShallowReference(
      this Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference shallowReference)
    {
      if (shallowReference == null)
        return (ReleaseEnvironmentShallowReference) null;
      return new ReleaseEnvironmentShallowReference()
      {
        Id = shallowReference.Id,
        Name = shallowReference.Name,
        Url = shallowReference.Url
      };
    }
  }
}
