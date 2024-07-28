// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingUtility
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class LicensingUtility
  {
    public static IClientRightsEnvelope ParseClientRightsToken(
      string clientRightsToken,
      X509Certificate2 certificate)
    {
      ArgumentUtility.CheckForNull<string>(clientRightsToken, nameof (clientRightsToken));
      ArgumentUtility.CheckForNull<X509Certificate2>(certificate, nameof (certificate));
      return LicensingUtility.ParseJwt(JsonWebToken.Create(clientRightsToken), new JsonWebTokenValidationParameters()
      {
        SigningCredentials = VssSigningCredentials.Create(certificate),
        ValidateActor = false,
        ValidateAudience = false,
        ValidateExpiration = false,
        ValidateIssuer = false,
        ValidateNotBefore = false,
        ValidateSignature = true
      });
    }

    internal static IClientRightsEnvelope ParseJwt(
      JsonWebToken jwt,
      JsonWebTokenValidationParameters validationParameters)
    {
      Dictionary<string, string> dictionary = jwt.ValidateToken(validationParameters).Claims.ToDictionary<Claim, string, string>((Func<Claim, string>) (claim => claim.Type), (Func<Claim, string>) (claim => claim.Value));
      return (IClientRightsEnvelope) new ClientRightsEnvelope((IList<IClientRight>) JsonConvert.DeserializeObject<ClientRight[]>(LicensingUtility.GetRequiredClientRightsAttribute(dictionary, "Rights"), (JsonConverter) new LicensingUtility.VersionConverter()))
      {
        EnvelopeVersion = new Version(LicensingUtility.GetRequiredClientRightsAttribute(dictionary, "EnvelopeVersion")),
        UserId = new Guid(LicensingUtility.GetRequiredClientRightsAttribute(dictionary, "UserId")),
        UserName = LicensingUtility.GetOptionalClientRightsAttribute(dictionary, "UserName", string.Empty),
        RefreshInterval = TimeSpan.FromSeconds((double) int.Parse(LicensingUtility.GetRequiredClientRightsAttribute(dictionary, "PollS"))),
        ActivityId = new Guid(LicensingUtility.GetRequiredClientRightsAttribute(dictionary, "ActivityId")),
        CreationDate = (DateTimeOffset) jwt.ValidFrom.ToUniversalTime(),
        ExpirationDate = (DateTimeOffset) jwt.ValidTo.ToUniversalTime(),
        Canary = LicensingUtility.GetOptionalClientRightsAttribute(dictionary, "Canary", string.Empty)
      };
    }

    private static string GetRequiredClientRightsAttribute(
      Dictionary<string, string> claims,
      string name)
    {
      return claims[name];
    }

    private static string GetOptionalClientRightsAttribute(
      Dictionary<string, string> claims,
      string name,
      string defaultValue)
    {
      string str = (string) null;
      return !claims.TryGetValue(name, out str) ? defaultValue : str;
    }

    private class VersionConverter : JsonConverter
    {
      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => serializer.Serialize(writer, value);

      public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        Dictionary<string, int> dictionary = serializer.Deserialize<Dictionary<string, int>>(reader);
        return (object) new Version(dictionary["Major"], dictionary["Minor"], dictionary["Build"], dictionary["Revision"]);
      }

      public override bool CanConvert(Type objectType) => objectType == typeof (Version);
    }
  }
}
