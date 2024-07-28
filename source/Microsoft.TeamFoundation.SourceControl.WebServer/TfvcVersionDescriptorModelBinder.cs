// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcVersionDescriptorModelBinder
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class TfvcVersionDescriptorModelBinder : VersionDescriptorModelBinder<TfvcVersionDescriptor>
  {
    protected override string ExpectedServiceArea => "Version Control";

    protected override TfvcVersionDescriptor CreateVersionDescriptor(
      ModelBindingContext bindingContext)
    {
      TfvcVersionDescriptor versionDescriptor = new TfvcVersionDescriptor();
      string result1;
      if (this.TryGetVersion(bindingContext, out result1))
        versionDescriptor.Version = result1;
      TfvcVersionType result2;
      if (this.TryGetVersionType<TfvcVersionType>(bindingContext, out result2))
        versionDescriptor.VersionType = result2;
      TfvcVersionOption result3;
      if (this.TryGetVersionOptions<TfvcVersionOption>(bindingContext, "versionOption", out result3))
        versionDescriptor.VersionOption = result3;
      return versionDescriptor;
    }
  }
}
