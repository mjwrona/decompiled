// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GvfsAllowedVersionRange
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DataContract]
  public class GvfsAllowedVersionRange
  {
    public GvfsAllowedVersionRange()
    {
    }

    [JsonConstructor]
    public GvfsAllowedVersionRange(Version min, Version max)
    {
      this.Min = GvfsAllowedVersionRange.FromVersion(min);
      this.Max = GvfsAllowedVersionRange.FromVersion(max);
    }

    [DataMember]
    public GvfsAllowedVersion Min { get; set; }

    [DataMember]
    public GvfsAllowedVersion Max { get; set; }

    private static GvfsAllowedVersion FromVersion(Version version)
    {
      if (version == (Version) null)
        return (GvfsAllowedVersion) null;
      return new GvfsAllowedVersion()
      {
        Major = version.Major,
        Minor = version.Minor,
        Build = version.Build,
        Revision = version.Revision
      };
    }
  }
}
