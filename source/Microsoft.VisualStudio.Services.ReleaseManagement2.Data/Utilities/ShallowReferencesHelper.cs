// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ShallowReferencesHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ShallowReferencesHelper
  {
    public static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.ReleaseDefinitionShallowReference CreateDefinitionShallowReference(
      IVssRequestContext context,
      Guid projectId,
      int id,
      string name,
      string path)
    {
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference()
      {
        Id = id,
        Name = name,
        Url = WebAccessUrlBuilder.GetReleaseDefinitionRestUrl(context, projectId, id)
      }.ToDefinitionShallowReference(path);
    }

    public static ReleaseShallowReference CreateReleaseShallowReference(
      IVssRequestContext context,
      Guid projectId,
      int id,
      string name)
    {
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference()
      {
        Id = id,
        Name = name,
        Url = WebAccessUrlBuilder.GetReleaseRestUrl(context, projectId, id)
      }.ToReleaseShallowReference();
    }

    public static ReleaseEnvironmentShallowReference CreateReleaseEnvironmentShallowReference(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      int id,
      string name)
    {
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ShallowReference()
      {
        Id = id,
        Name = name,
        Url = WebAccessUrlBuilder.GetReleaseEnvironmentRestUri(context, projectId, releaseId, id)
      }.ToReleaseEnvironmentShallowReference();
    }
  }
}
