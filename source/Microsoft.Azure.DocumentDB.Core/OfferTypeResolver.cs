// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.OfferTypeResolver
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal sealed class OfferTypeResolver : ITypeResolver<Offer>
  {
    public static readonly ITypeResolver<Offer> RequestOfferTypeResolver = (ITypeResolver<Offer>) new OfferTypeResolver(false);
    public static readonly ITypeResolver<Offer> ResponseOfferTypeResolver = (ITypeResolver<Offer>) new OfferTypeResolver(true);
    private readonly bool isResponse;

    private OfferTypeResolver(bool isResponse) => this.isResponse = isResponse;

    Offer ITypeResolver<Offer>.Resolve(JObject propertyBag)
    {
      Offer offer;
      if (propertyBag != null)
      {
        offer = new Offer();
        offer.propertyBag = propertyBag;
        switch (offer.OfferVersion ?? string.Empty)
        {
          case "V1":
          case "":
            break;
          case "V2":
            offer = (Offer) new OfferV2();
            offer.propertyBag = propertyBag;
            break;
          default:
            DefaultTrace.TraceCritical("Unexpected offer version {0}", (object) offer.OfferVersion);
            if (!this.isResponse)
              throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.UnsupportedOfferVersion, (object) offer.OfferVersion));
            break;
        }
      }
      else
        offer = (Offer) null;
      return offer;
    }
  }
}
