// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitItemDescriptor
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitItemDescriptor : VersionControlSecuredObject
  {
    [DataMember]
    public string Path;
    [DataMember]
    public string Version;
    [DataMember]
    public GitVersionType VersionType;
    [DataMember]
    public GitVersionOptions VersionOptions;

    public GitItemDescriptor()
    {
    }

    public GitItemDescriptor(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public VersionControlRecursionType RecursionLevel { get; set; }
  }
}
