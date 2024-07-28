// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.ForwardedSshCommandParsingException
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  internal class ForwardedSshCommandParsingException : VssServiceException
  {
    public static ForwardedSshCommandParsingException GetNewException(
      string actualDestination,
      ForwardedSshCommandParsingException.FieldType fieldType)
    {
      if (actualDestination == null)
        actualDestination = "Null String";
      if (actualDestination.Equals(string.Empty))
        actualDestination = "Empty String";
      return new ForwardedSshCommandParsingException("The '" + Enum.GetName(typeof (ForwardedSshCommandParsingException.FieldType), (object) fieldType) + "': '" + actualDestination + "' is invalid.");
    }

    private ForwardedSshCommandParsingException(string message)
      : base(message)
    {
    }

    public enum FieldType
    {
      ProxyData,
      Collection,
      TargetAudience,
    }
  }
}
