// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Exceptions.CargoExceptionMappings
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Exceptions
{
  [ExcludeFromCodeCoverage]
  public static class CargoExceptionMappings
  {
    public static Dictionary<Type, HttpStatusCode> Mappings = PackagingExceptionMappings.WithOverrides((IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()).ToDictionary<KeyValuePair<Type, HttpStatusCode>, Type, HttpStatusCode>((Func<KeyValuePair<Type, HttpStatusCode>, Type>) (kv => kv.Key), (Func<KeyValuePair<Type, HttpStatusCode>, HttpStatusCode>) (kv => kv.Value));
  }
}
