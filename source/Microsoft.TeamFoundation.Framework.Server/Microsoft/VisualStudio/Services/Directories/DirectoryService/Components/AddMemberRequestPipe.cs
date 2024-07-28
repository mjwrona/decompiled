// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryService.Components.AddMemberRequestPipe
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Directories.DirectoryService.Components
{
  public class AddMemberRequestPipe
  {
    public DirectoryMemberRequestStage Prepare { get; } = new DirectoryMemberRequestStage(nameof (Prepare));

    public DirectoryMemberRequestStage Discover { get; } = new DirectoryMemberRequestStage(nameof (Discover));

    public DirectoryMemberRequestStage Validate { get; } = new DirectoryMemberRequestStage(nameof (Validate));

    public DirectoryMemberRequestStage Build { get; } = new DirectoryMemberRequestStage(nameof (Build));

    public DirectoryMemberRequestStage Modify { get; } = new DirectoryMemberRequestStage(nameof (Modify));

    public DirectoryMemberRequestStage Persist { get; } = new DirectoryMemberRequestStage(nameof (Persist));

    public DirectoryMemberRequestStage Provision { get; } = new DirectoryMemberRequestStage(nameof (Provision));

    public void Process(IVssRequestContext requestContext, DirectoryMemberRequest request)
    {
      this.Prepare.Process(requestContext, request);
      this.Discover.Process(requestContext, request);
      this.Validate.Process(requestContext, request);
      this.Build.Process(requestContext, request);
      this.Modify.Process(requestContext, request);
      this.Persist.Process(requestContext, request);
      this.Provision.Process(requestContext, request);
    }
  }
}
