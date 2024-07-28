// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionFilterConverter
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  public class SubscriptionFilterConverter : TypeStringLookupConverter
  {
    private static Dictionary<string, Type> s_supportedFilters = new Dictionary<string, Type>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Expression",
        typeof (ExpressionFilter)
      },
      {
        "Artifact",
        typeof (ArtifactFilter)
      },
      {
        "Actor",
        typeof (ActorFilter)
      }
    };

    protected override Dictionary<string, Type> TypeMap => SubscriptionFilterConverter.s_supportedFilters;

    protected override Type BaseType => typeof (ISubscriptionFilter);

    protected override string TypeFieldName => "type";

    protected override object CreateUnsupportedTypeObject(string typeName) => (object) new UnsupportedFilter(!string.IsNullOrEmpty(typeName) ? typeName : "Invalid");
  }
}
