// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.ICodeSenseService
// Assembly: Microsoft.TeamFoundation.CodeSense.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FD810605-AAA9-4CE8-B2C6-6B28A5D994C5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.dll

using Microsoft.TeamFoundation.CodeSense.Server.Services;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  [DefaultServiceImplementation(typeof (CodeSenseService))]
  public interface ICodeSenseService : IVssFrameworkService
  {
    VersionedContent GetDetails(
      IVssRequestContext requestContext,
      string path,
      CodeSenseResourceVersion targetResourceVersion = CodeSenseResourceVersion.Dev12Update3);

    VersionedContent GetSummary(
      IVssRequestContext requestContext,
      string path,
      CodeSenseResourceVersion targetResourceVersion = CodeSenseResourceVersion.Dev12Update3);
  }
}
