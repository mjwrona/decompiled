// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.AgileInvalidIterationPathException
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  [Serializable]
  internal class AgileInvalidIterationPathException : TeamFoundationServerException
  {
    public AgileInvalidIterationPathException(string path)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AgileServerResources.SiteError_InvalidIterationPath, (object) path))
    {
    }

    public AgileInvalidIterationPathException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected AgileInvalidIterationPathException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
