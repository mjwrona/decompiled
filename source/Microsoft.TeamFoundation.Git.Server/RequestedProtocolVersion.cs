// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RequestedProtocolVersion
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal readonly struct RequestedProtocolVersion
  {
    public RequestedProtocolVersion(string inputString)
    {
      this.RequestedVersion = inputString;
      switch (inputString)
      {
        case "0":
          this.RecognizedVersion = GitProtocolVersion.Unknown;
          break;
        case "1":
          this.RecognizedVersion = GitProtocolVersion.One;
          break;
        case "2":
          this.RecognizedVersion = GitProtocolVersion.Two;
          break;
        default:
          if (int.TryParse(inputString, out int _))
          {
            this.RecognizedVersion = Enum.GetValues(typeof (GitProtocolVersion)).Cast<GitProtocolVersion>().Max<GitProtocolVersion>();
            break;
          }
          this.RecognizedVersion = GitProtocolVersion.Unknown;
          break;
      }
    }

    public GitProtocolVersion RecognizedVersion { get; }

    public string RequestedVersion { get; }
  }
}
