// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ContainerPropertyNames
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ContainerPropertyNames
  {
    public const string Env = "env";
    public const string Image = "image";
    public const string Options = "options";
    public const string Volumes = "volumes";
    public const string Ports = "ports";
    public const string MapDockerSocket = "mapDockerSocket";
    public const string ReadOnlyMounts = "readOnlyMounts";
  }
}
