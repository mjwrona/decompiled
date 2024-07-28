// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.PersistentSharedPropertyProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal class PersistentSharedPropertyProvider : IPropertyProvider
  {
    private IPersistentPropertyBag persistedSessionProperties;

    public PersistentSharedPropertyProvider(IPersistentPropertyBag persistentPropertyBag)
    {
      persistentPropertyBag.RequiresArgumentNotNull<IPersistentPropertyBag>(nameof (persistentPropertyBag));
      this.persistedSessionProperties = persistentPropertyBag;
    }

    public void AddSharedProperties(
      List<KeyValuePair<string, object>> sharedProperties,
      TelemetryContext telemetryContext)
    {
      IEnumerable<KeyValuePair<string, object>> allProperties = this.persistedSessionProperties.GetAllProperties();
      sharedProperties.AddRange(allProperties);
    }

    public void PostProperties(TelemetryContext telemetryContext, CancellationToken token)
    {
    }
  }
}
