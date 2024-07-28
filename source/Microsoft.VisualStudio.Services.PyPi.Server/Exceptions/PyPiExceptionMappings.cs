// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Exceptions.PyPiExceptionMappings
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Exceptions
{
  public static class PyPiExceptionMappings
  {
    public static Dictionary<Type, HttpStatusCode> Mappings = PackagingExceptionMappings.WithOverrides((IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()).ToDictionary<KeyValuePair<Type, HttpStatusCode>, Type, HttpStatusCode>((Func<KeyValuePair<Type, HttpStatusCode>, Type>) (kv => kv.Key), (Func<KeyValuePair<Type, HttpStatusCode>, HttpStatusCode>) (kv => kv.Value));
  }
}
