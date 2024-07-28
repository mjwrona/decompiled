// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.LoginTenantBranding
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [JsonObject(MemberSerialization.OptIn)]
  [Entity("loginTenantBrandings", new string[] {"Microsoft.WindowsAzure.ActiveDirectory.LoginTenantBranding", "Microsoft.DirectoryServices.LoginTenantBranding"})]
  [ExcludeFromCodeCoverage]
  public class LoginTenantBranding : GraphObject
  {
    private string _backgroundColor;
    private Stream _bannerLogo;
    private string _bannerLogoUrl;
    private string _boilerPlateText;
    private Stream _illustration;
    private string _illustrationUrl;
    private bool? _keepMeSignedInDisabled;
    private string _locale;
    private string _metadataUrl;
    private Stream _tileLogo;
    private string _tileLogoUrl;
    private string _userIdLabel;

    [JsonProperty("backgroundColor")]
    public string BackgroundColor
    {
      get => this._backgroundColor;
      set
      {
        this._backgroundColor = value;
        this.ChangedProperties.Add(nameof (BackgroundColor));
      }
    }

    [JsonProperty("bannerLogo")]
    public Stream BannerLogo
    {
      get => this._bannerLogo;
      set
      {
        this._bannerLogo = value;
        this.ChangedProperties.Add(nameof (BannerLogo));
      }
    }

    [JsonProperty("bannerLogoUrl")]
    public string BannerLogoUrl
    {
      get => this._bannerLogoUrl;
      set
      {
        this._bannerLogoUrl = value;
        this.ChangedProperties.Add(nameof (BannerLogoUrl));
      }
    }

    [JsonProperty("boilerPlateText")]
    public string BoilerPlateText
    {
      get => this._boilerPlateText;
      set
      {
        this._boilerPlateText = value;
        this.ChangedProperties.Add(nameof (BoilerPlateText));
      }
    }

    [JsonProperty("illustration")]
    public Stream Illustration
    {
      get => this._illustration;
      set
      {
        this._illustration = value;
        this.ChangedProperties.Add(nameof (Illustration));
      }
    }

    [JsonProperty("illustrationUrl")]
    public string IllustrationUrl
    {
      get => this._illustrationUrl;
      set
      {
        this._illustrationUrl = value;
        this.ChangedProperties.Add(nameof (IllustrationUrl));
      }
    }

    [JsonProperty("keepMeSignedInDisabled")]
    public bool? KeepMeSignedInDisabled
    {
      get => this._keepMeSignedInDisabled;
      set
      {
        this._keepMeSignedInDisabled = value;
        this.ChangedProperties.Add(nameof (KeepMeSignedInDisabled));
      }
    }

    [JsonProperty("locale")]
    [Key(true)]
    public string Locale
    {
      get => this._locale;
      set
      {
        this._locale = value;
        this.ChangedProperties.Add(nameof (Locale));
      }
    }

    [JsonProperty("metadataUrl")]
    public string MetadataUrl
    {
      get => this._metadataUrl;
      set
      {
        this._metadataUrl = value;
        this.ChangedProperties.Add(nameof (MetadataUrl));
      }
    }

    [JsonProperty("tileLogo")]
    public Stream TileLogo
    {
      get => this._tileLogo;
      set
      {
        this._tileLogo = value;
        this.ChangedProperties.Add(nameof (TileLogo));
      }
    }

    [JsonProperty("tileLogoUrl")]
    public string TileLogoUrl
    {
      get => this._tileLogoUrl;
      set
      {
        this._tileLogoUrl = value;
        this.ChangedProperties.Add(nameof (TileLogoUrl));
      }
    }

    [JsonProperty("userIdLabel")]
    public string UserIdLabel
    {
      get => this._userIdLabel;
      set
      {
        this._userIdLabel = value;
        this.ChangedProperties.Add(nameof (UserIdLabel));
      }
    }
  }
}
