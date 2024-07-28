// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.WidgetSettingsVersionInvalidException
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [Serializable]
  public class WidgetSettingsVersionInvalidException : TeamFoundationServerException
  {
    public WidgetSettingsVersionInvalidException()
    {
    }

    public WidgetSettingsVersionInvalidException(string version)
      : base(string.Format(DashboardResources.ErrorWidgetSettingsVersionInvalid, (object) version))
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public WidgetSettingsVersionInvalidException(WrappedException wrappedException)
      : base(wrappedException.Message, wrappedException.UnwrappedInnerException)
    {
    }

    protected WidgetSettingsVersionInvalidException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
