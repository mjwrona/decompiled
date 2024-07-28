// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ArtifactSpec
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class ArtifactSpec : ICacheable
  {
    private const int c_maxMonikerLengthInChars = 440;
    private const int c_maxArtifactIdSize = 64;

    internal ArtifactSpec()
    {
    }

    public ArtifactSpec(Guid kind, string moniker, int version)
      : this(kind, moniker, version, Guid.Empty)
    {
    }

    public ArtifactSpec(Guid kind, string moniker, int version, Guid dataspaceIdentifier)
    {
      this.Moniker = moniker;
      this.Kind = kind;
      this.Version = version;
      this.DataspaceIdentifier = dataspaceIdentifier;
    }

    public ArtifactSpec(Guid kind, int id, int version, Guid dataspaceIdentifier)
      : this(kind, new byte[4]
      {
        (byte) (((long) id & 4278190080L) >> 24),
        (byte) ((id & 16711680) >> 16),
        (byte) ((id & 65280) >> 8),
        (byte) (id & (int) byte.MaxValue)
      }, version, dataspaceIdentifier)
    {
    }

    public ArtifactSpec(Guid kind, int id, int version)
      : this(kind, id, version, Guid.Empty)
    {
    }

    public ArtifactSpec(Guid kind, byte[] id, int version)
      : this(kind, id, version, Guid.Empty)
    {
    }

    public ArtifactSpec(Guid kind, byte[] id, int version, Guid dataspaceIdentifier)
    {
      this.Id = id;
      this.Kind = kind;
      this.Version = version;
      this.DataspaceIdentifier = dataspaceIdentifier;
    }

    public int GetCachedSize() => string.IsNullOrEmpty(this.Moniker) ? 24 : (this.Moniker.Length << 1) + 24;

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public byte[] Id { get; set; }

    [XmlAttribute("item")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string Moniker { get; set; }

    [XmlAttribute("ver")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public int Version { get; set; }

    [XmlAttribute("k")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public Guid Kind { get; set; }

    [XmlAttribute("ds")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public Guid DataspaceIdentifier { get; set; }

    [XmlIgnore]
    internal int Size => this.GetCachedSize();

    internal void Validate(PropertiesOptions options)
    {
      if ((options & PropertiesOptions.AllVersions) == PropertiesOptions.AllVersions && this.Version != 0)
        throw new TeamFoundationValidationException(FrameworkResources.InvalidArtifactVersion((object) this.Version, (object) "AllVersions"), "Version");
      if (!string.IsNullOrEmpty(this.Moniker))
      {
        ArgumentUtility.CheckStringForInvalidCharacters(this.Moniker, "Moniker");
        if (this.Moniker.Length > 440)
          throw new TeamFoundationValidationException(TFCommonResources.PropertyArgumentExceededMaximumSizeAllowed((object) "Moniker", (object) 440), "Moniker");
      }
      else
      {
        ArgumentUtility.CheckForNull<byte[]>(this.Id, "Id");
        if (this.Id.Length > 64)
          throw new TeamFoundationValidationException(TFCommonResources.PropertyArgumentExceededMaximumSizeAllowed((object) "ArtifactId", (object) 64), "ArtifactId");
      }
    }

    internal static ArtifactSpec TranslateSqlWildcards(
      ArtifactSpec artifactSpec,
      out bool containsWildcards)
    {
      bool wasTranslated;
      string moniker = ArtifactSpec.TranslateSqlWildcards(artifactSpec.Moniker, out wasTranslated, out containsWildcards);
      return !wasTranslated ? artifactSpec : new ArtifactSpec(artifactSpec.Kind, moniker, artifactSpec.Version);
    }

    internal static string TranslateSqlWildcards(
      string wildcardPattern,
      out bool wasTranslated,
      out bool containsWildcards)
    {
      containsWildcards = false;
      wasTranslated = false;
      if (string.IsNullOrEmpty(wildcardPattern))
        return wildcardPattern;
      bool flag = false;
      int length = wildcardPattern.Length;
      for (int index = 0; index < length; ++index)
      {
        switch (wildcardPattern[index])
        {
          case '%':
          case '[':
          case '_':
            containsWildcards = true;
            break;
          case '*':
          case '?':
            containsWildcards = true;
            break;
          case '\\':
            flag = true;
            ++index;
            break;
        }
      }
      if (!containsWildcards && !flag)
        return wildcardPattern;
      StringBuilder stringBuilder = new StringBuilder(length << 1);
      wasTranslated = true;
      for (int index = 0; index < length; ++index)
      {
        char ch1 = wildcardPattern[index];
        switch (ch1)
        {
          case '%':
          case '[':
          case '_':
            if (containsWildcards)
            {
              stringBuilder.Append('[');
              stringBuilder.Append(ch1);
              stringBuilder.Append(']');
              break;
            }
            stringBuilder.Append(ch1);
            break;
          case '*':
            stringBuilder.Append('%');
            break;
          case '?':
            stringBuilder.Append('_');
            break;
          case '\\':
            char ch2 = index != length - 1 ? wildcardPattern[++index] : throw new TeamFoundationValidationException(FrameworkResources.WildcardPatternIsInvalidError((object) wildcardPattern), nameof (wildcardPattern));
            switch (ch2)
            {
              case '*':
              case '?':
              case '\\':
                stringBuilder.Append(ch2);
                continue;
              default:
                throw new TeamFoundationValidationException(FrameworkResources.WildcardPatternIsInvalidError((object) wildcardPattern), nameof (wildcardPattern));
            }
          default:
            stringBuilder.Append(ch1);
            break;
        }
      }
      return stringBuilder.ToString();
    }
  }
}
