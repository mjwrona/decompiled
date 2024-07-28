// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.ChangeProcessing.CargoExceptionMarkFactory
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.ChangeProcessing
{
  public class CargoExceptionMarkFactory : IShouldMarkFactory
  {
    public IShouldMark Create(bool forceMode) => forceMode ? (IShouldMark) new TransientNetworkExceptionListMarker() : (IShouldMark) new ExceptionBlackListMarker((IReadOnlyList<ExceptionFilter>) new List<ExceptionFilter>());
  }
}
