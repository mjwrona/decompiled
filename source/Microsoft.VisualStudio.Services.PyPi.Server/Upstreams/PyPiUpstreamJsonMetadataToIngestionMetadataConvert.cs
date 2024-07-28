// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.PyPiUpstreamJsonMetadataToIngestionMetadataConverter
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public class PyPiUpstreamJsonMetadataToIngestionMetadataConverter : 
    IConverter<PyPiUpstreamJsonMetadataPackageFileMetadataRequest, IReadOnlyDictionary<string, string[]>>,
    IHaveInputType<PyPiUpstreamJsonMetadataPackageFileMetadataRequest>,
    IHaveOutputType<IReadOnlyDictionary<string, string[]>>
  {
    private static readonly IReadOnlyDictionary<string, string> WellKnownJsonMetadataPropertiesToIngestMetadataMap = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "author",
        "author"
      },
      {
        "author_email",
        "author_email"
      },
      {
        "comment_text",
        "comment"
      },
      {
        "content",
        "content"
      },
      {
        "description",
        "description"
      },
      {
        "description_content_type",
        "description_content_type"
      },
      {
        "classifiers",
        "classifiers"
      },
      {
        "download_url",
        "download_url"
      },
      {
        "filename",
        "filename"
      },
      {
        "packagetype",
        "filetype"
      },
      {
        "gpg_signature",
        "gpg_signature"
      },
      {
        "home_page",
        "home_page"
      },
      {
        "keywords",
        "keywords"
      },
      {
        "license",
        "license"
      },
      {
        "maintainer",
        "maintainer"
      },
      {
        "maintainer_email",
        "maintainer_email"
      },
      {
        "md5_digest",
        "md5_digest"
      },
      {
        "name",
        "name"
      },
      {
        "obsoletes_dist",
        "obsoletes_dist"
      },
      {
        "platform",
        "platform"
      },
      {
        "project_urls",
        "project_urls"
      },
      {
        "provides_dist",
        "provides_dist"
      },
      {
        "provides_extras",
        "provides_extras"
      },
      {
        "python_version",
        "pyversion"
      },
      {
        "requires_dist",
        "requires_dist"
      },
      {
        "requires_external",
        "requires_external"
      },
      {
        "requires_python",
        "requires_python"
      },
      {
        "summary",
        "summary"
      },
      {
        "supported_platform",
        "supported_platform"
      },
      {
        "variant",
        "variant"
      },
      {
        "version",
        "version"
      }
    };

    public IReadOnlyDictionary<string, string[]> Convert(
      PyPiUpstreamJsonMetadataPackageFileMetadataRequest upstreamPackageFileMetadataRequest)
    {
      Dictionary<string, string[]> metaData = new Dictionary<string, string[]>();
      JObject json = JObject.Parse(upstreamPackageFileMetadataRequest.UpstreamJsonMetadata);
      Dictionary<string, string[]> collection = json["info"] != null && json["urls"] != null ? PyPiUpstreamJsonMetadataUtility.GetPackageLevelMetadataDictionary((JToken) json) : throw new PyPiUpstreamMetadataParsingException();
      JToken fileMetadataNode;
      Dictionary<string, string[]> metadataDictionary = PyPiUpstreamJsonMetadataUtility.GetFileLevelMetadataDictionary((JToken) json, upstreamPackageFileMetadataRequest.UpstreamPackageFileName, out fileMetadataNode);
      if (metadataDictionary == null)
        return (IReadOnlyDictionary<string, string[]>) null;
      collection.ForEach<KeyValuePair<string, string[]>>((Action<KeyValuePair<string, string[]>>) (packageLevelMetaDataItem => this.TransformAndAddMetadataItem(packageLevelMetaDataItem, metaData)));
      metadataDictionary.ForEach<KeyValuePair<string, string[]>>((Action<KeyValuePair<string, string[]>>) (fileLevelMetadataItem => this.TransformAndAddMetadataItem(fileLevelMetadataItem, metaData)));
      this.ParseAndAddDigests(fileMetadataNode, metaData);
      metaData.Add("protocol_version", new string[1]{ "1" });
      metaData.Add("metadata_version", new string[1]
      {
        "2.1"
      });
      this.ParseAndAddUrl(fileMetadataNode, metaData);
      return (IReadOnlyDictionary<string, string[]>) metaData;
    }

    private void TransformAndAddMetadataItem(
      KeyValuePair<string, string[]> metaDataItem,
      Dictionary<string, string[]> metaData)
    {
      if (!PyPiUpstreamJsonMetadataToIngestionMetadataConverter.WellKnownJsonMetadataPropertiesToIngestMetadataMap.ContainsKey(metaDataItem.Key))
        return;
      metaData[PyPiUpstreamJsonMetadataToIngestionMetadataConverter.WellKnownJsonMetadataPropertiesToIngestMetadataMap[metaDataItem.Key]] = metaDataItem.Value;
    }

    private void ParseAndAddDigests(JToken fileMetadataNode, Dictionary<string, string[]> metaData)
    {
      if (fileMetadataNode[(object) "digests"] == null || fileMetadataNode[(object) "digests"][(object) "sha256"] == null)
        return;
      metaData.Add("sha256_digest", new string[1]
      {
        fileMetadataNode[(object) "digests"][(object) "sha256"].ToString()
      });
    }

    private void ParseAndAddUrl(JToken fileMetadataNode, Dictionary<string, string[]> metaData)
    {
      if (fileMetadataNode[(object) "url"] == null)
        throw new PyPiUpstreamMetadataParsingException();
      metaData.Add("url", new string[1]
      {
        fileMetadataNode[(object) "url"].ToString()
      });
    }
  }
}
