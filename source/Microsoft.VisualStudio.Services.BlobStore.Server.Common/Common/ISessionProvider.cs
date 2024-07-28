// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.ISessionProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public interface ISessionProvider
  {
    bool ProviderRequireVss { get; }

    Task InitializeAsync(VssRequestPump.Processor processor);

    Task CreateSessionAsync(
      VssRequestPump.Processor processor,
      Guid sessionId,
      Guid owner,
      SessionState status,
      DateTime expiration,
      DedupIdentifier contentId,
      List<Part> parts);

    Task<Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Session> GetSessionAsync(
      VssRequestPump.Processor processor,
      Guid sessionId);

    Task UpdateSessionAsync(
      VssRequestPump.Processor processor,
      Guid sessionId,
      Guid owner,
      SessionState status,
      DateTime expiration,
      DedupIdentifier contentId,
      List<Part> parts);

    Task DeleteSessionAsync(VssRequestPump.Processor processor, Guid sessionId);
  }
}
