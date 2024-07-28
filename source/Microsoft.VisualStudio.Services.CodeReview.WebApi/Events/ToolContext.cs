// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Events.ToolContext
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi.Events
{
  public class ToolContext
  {
    public string Caption { get; set; }

    public string Id { get; set; }

    public string Context { get; set; }

    public string ArtifactType { get; set; }

    public string ArtifactScope { get; set; }

    public UserAction Action { get; set; }

    public UserAction SecondaryAction { get; set; }
  }
}
