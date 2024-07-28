// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.GitHubDirectoryEntityConverter
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class GitHubDirectoryEntityConverter
  {
    internal static IDirectoryUser ConvertUser(
      IVssRequestContext context,
      GitHubData.V3.User user,
      IEnumerable<string> properties)
    {
      properties = (IEnumerable<string>) ((object) properties ?? (object) Array.Empty<string>());
      string entityId = new DirectoryEntityIdentifierV1("ghb", nameof (user), user.Id).Encode();
      return DirectoryEntityBuilder.BuildEntity<IDirectoryUser>("ghb", user.Id, entityId, properties: (IDictionary<string, object>) properties.ToDictionary<string, string, object>((Func<string, string>) (property => property), (Func<string, object>) (property =>
      {
        switch (property)
        {
          case "DisplayName":
            return (object) user.Name;
          case "Mail":
            return (object) user.Email;
          case "MailNickname":
            return (object) user.Login;
          case "PrincipalName":
            return (object) user.Login;
          case "SignInAddress":
            return (object) user.Login;
          default:
            return (object) null;
        }
      })));
    }
  }
}
