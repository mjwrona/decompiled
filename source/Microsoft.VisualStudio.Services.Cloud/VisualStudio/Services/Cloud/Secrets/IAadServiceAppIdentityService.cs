// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.Secrets.IAadServiceAppIdentityService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Framework.Server;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.Secrets
{
  [DefaultServiceImplementation(typeof (AadServiceAppIdentityService))]
  public interface IAadServiceAppIdentityService : IVssFrameworkService
  {
    Task<AuthenticationResult> GetAuthResultAsync(string resource, string authority = null);

    AuthenticationResult GetAuthResult(string resource, string authority = null);
  }
}
