// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildProcess
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  [KnownType(typeof (DesignerProcess))]
  [KnownType(typeof (YamlProcess))]
  [KnownType(typeof (DockerProcess))]
  [KnownType(typeof (JustInTimeProcess))]
  [JsonConverter(typeof (BuildProcessJsonConverter))]
  public class BuildProcess
  {
    internal BuildProcess()
    {
    }

    protected BuildProcess(int type) => this.Type = type;

    [DataMember(Name = "Type")]
    public int Type { get; set; }

    [DataMember(Name = "Version")]
    public int Version { get; internal set; }
  }
}
