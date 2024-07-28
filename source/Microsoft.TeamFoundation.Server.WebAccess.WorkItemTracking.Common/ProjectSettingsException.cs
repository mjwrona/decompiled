// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectSettingsException
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [Serializable]
  internal abstract class ProjectSettingsException : SettingsException
  {
    protected ProjectSettingsException(string message)
      : base(message)
    {
    }

    protected ProjectSettingsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ProjectSettingsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
