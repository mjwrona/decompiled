// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.TeamFoundationPropertyValidationException
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [Obsolete("Please use Microsoft.VisualStudio.Services.Common.VssPropertyValidationException instead.")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [ExceptionMapping("0.0", "3.0", "TeamFoundationPropertyValidationException", "Microsoft.TeamFoundation.Framework.Common.TeamFoundationPropertyValidationException, Microsoft.TeamFoundation.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class TeamFoundationPropertyValidationException : Exception
  {
    public TeamFoundationPropertyValidationException(string propertyName, string message)
      : base(message)
    {
      this.PropertyName = propertyName;
    }

    public TeamFoundationPropertyValidationException(
      string propertyName,
      string message,
      Exception innerException)
      : base(message, innerException)
    {
      this.PropertyName = propertyName;
    }

    public string PropertyName { get; set; }
  }
}
