// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification.IContentValidationFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentVerification
{
  public interface IContentValidationFacade
  {
    Task<NullResult> SubmitFilesAsync(
      FeedCore feed,
      IEnumerable<ContentValidationKey> toScan,
      string userId,
      string creatorIpAddress);

    Task<NullResult> ScanBase64ContentInStream(
      FeedCore feed,
      string userId,
      string creatorIpAddress,
      string content,
      string fileName);
  }
}
