// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.InvalidProjectSettingsException
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [Serializable]
  internal sealed class InvalidProjectSettingsException : ProjectSettingsException
  {
    private IEnumerable<string> m_errors;

    public InvalidProjectSettingsException(IEnumerable<string> errors, Type sourceType)
      : this(errors, sourceType, (Exception) null)
    {
    }

    public InvalidProjectSettingsException(
      IEnumerable<string> errors,
      Type sourceType,
      Exception innerException)
      : base(string.Empty, innerException)
    {
      this.m_errors = (IEnumerable<string>) errors.ToArray<string>();
      this.SourceType = sourceType;
    }

    public override string Message => string.Join(Environment.NewLine, this.GetErrors());

    public Type SourceType { get; set; }

    public IEnumerable<string> GetErrors() => this.m_errors ?? (IEnumerable<string>) new string[0];

    private InvalidProjectSettingsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
