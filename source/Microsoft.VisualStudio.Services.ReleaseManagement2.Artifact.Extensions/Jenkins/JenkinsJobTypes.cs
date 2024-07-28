// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Jenkins.JenkinsJobTypes
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Jenkins
{
  public static class JenkinsJobTypes
  {
    public const string Folder = "com.cloudbees.hudson.plugins.folder.Folder";
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Its a known term in Jenkins")]
    public const string MultiBranchPipeline = "org.jenkinsci.plugins.workflow.multibranch.WorkflowMultiBranchProject";
  }
}
