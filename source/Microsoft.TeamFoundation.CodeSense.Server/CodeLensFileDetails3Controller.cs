// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.WebAPI.CodeLensFileDetails3Controller
// Assembly: Microsoft.TeamFoundation.CodeSense.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FD810605-AAA9-4CE8-B2C6-6B28A5D994C5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Net.Http;

namespace Microsoft.TeamFoundation.CodeSense.Server.WebAPI
{
  public sealed class CodeLensFileDetails3Controller : CodeLensFileDetailsControllerBase
  {
    [ValidateCodeLensInput]
    [TraceFilter(1023030, 1023040)]
    public HttpResponseMessage GetFileDetails(string pathVersion, string timeStamp = "") => this.CreateResponseFromResult(this.ReadFileDetails(), true);
  }
}
