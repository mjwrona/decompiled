// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitVersionDescriptorModelBinder
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class GitVersionDescriptorModelBinder : VersionDescriptorModelBinder<GitVersionDescriptor>
  {
    protected override string ExpectedServiceArea => "git";

    protected override GitVersionDescriptor CreateVersionDescriptor(
      ModelBindingContext bindingContext)
    {
      GitVersionDescriptor descriptor = new GitVersionDescriptor();
      string result1;
      if (this.TryGetVersion(bindingContext, out result1))
        descriptor.Version = result1;
      GitVersionType result2;
      if (this.TryGetVersionType<GitVersionType>(bindingContext, out result2))
        descriptor.VersionType = result2;
      GitVersionOptions result3;
      if (this.TryGetVersionOptions<GitVersionOptions>(bindingContext, "versionOptions", out result3))
        descriptor.VersionOptions = result3;
      this.ValidateGitVersionDescriptor(descriptor);
      return descriptor;
    }

    public void ValidateGitVersionDescriptor(GitVersionDescriptor descriptor)
    {
      if (descriptor.VersionType != GitVersionType.Branch && string.IsNullOrEmpty(descriptor.Version))
        throw new ArgumentException(Resources.Get("AmbiguousVersion")).Expected("git");
    }
  }
}
