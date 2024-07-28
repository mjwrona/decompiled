// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.ILicensingAdapterFactory
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public interface ILicensingAdapterFactory
  {
    void Start(
      IVssRequestContext requestContext,
      LicensingServiceConfiguration serviceSettings,
      IVssDateTimeProvider dateTimeProvider);

    void Stop(IVssRequestContext requestContext);

    IList<ILicensingAdapter> GetClientAdapters();

    IList<ILicensingAdapter> GetClientAdapters(string rightName);

    IList<ILicensingAdapter> GetServiceAdapters();

    IList<ILicensingAdapter> GetServiceAdapters(string rightName);

    T GetAdapter<T>() where T : ILicensingAdapter;
  }
}
