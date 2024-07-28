// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.OfferTypeResolver
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

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
