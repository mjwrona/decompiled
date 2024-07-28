// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.IResourceTaggingService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DefaultServiceImplementation(typeof (ResourceTaggingService))]
  internal interface IResourceTaggingService : IVssFrameworkService
  {
    void SaveTags(
      IVssRequestContext requestContext,
      string resourceName,
      Dictionary<string, string> tags);

    Dictionary<string, string> GetTags(IVssRequestContext requestContext, string resourceName);

    void DeleteTags(IVssRequestContext requestContext, string resourceName);
  }
}
