// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.IdentityRefExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal static class IdentityRefExtensions
  {
    public static IdentityRef Clone(this IdentityRef source)
    {
      if (source == null)
        return (IdentityRef) null;
      return new IdentityRef()
      {
        DisplayName = source.DisplayName,
        Id = source.Id,
        ImageUrl = source.ImageUrl,
        IsAadIdentity = source.IsAadIdentity,
        IsContainer = source.IsContainer,
        ProfileUrl = source.ProfileUrl,
        UniqueName = source.UniqueName,
        Url = source.Url
      };
    }
  }
}
