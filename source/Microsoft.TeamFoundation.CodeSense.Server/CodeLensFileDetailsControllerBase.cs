// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.WebAPI.CodeLensFileDetailsControllerBase
// Assembly: Microsoft.TeamFoundation.CodeSense.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FD810605-AAA9-4CE8-B2C6-6B28A5D994C5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.dll

using Microsoft.TeamFoundation.CodeSense.Server.Services;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;

namespace Microsoft.TeamFoundation.CodeSense.Server.WebAPI
{
  public class CodeLensFileDetailsControllerBase : CodeLensFilesController
  {
    public VersionedContent ReadFileDetails(CodeSenseResourceVersion targetResourceVersion = CodeSenseResourceVersion.Dev12Update3)
    {
      VersionedContent versionedContent = (VersionedContent) null;
      ICodeSenseService service = this.TfsRequestContext.GetService<ICodeSenseService>();
      try
      {
        int num = (int) this.TfsRequestContext.Items["codelensVersion"];
        string path = (string) this.TfsRequestContext.Items["codelensServerPath"];
        versionedContent = service.GetDetails(this.TfsRequestContext, path, targetResourceVersion);
      }
      catch (ItemNotFoundException ex)
      {
      }
      catch (ArgumentException ex)
      {
      }
      return versionedContent;
    }
  }
}
