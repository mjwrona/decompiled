// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.PyPiMetadataUtils
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public class PyPiMetadataUtils
  {
    public static List<string> GetOptionalMetadataFieldList(
      string key,
      IReadOnlyDictionary<string, string[]> metadataFields)
    {
      if (!metadataFields.ContainsKey(key))
        return (List<string>) null;
      return ((IEnumerable<string>) metadataFields[key]).IsNullOrEmpty<string>() ? (List<string>) null : ((IEnumerable<string>) metadataFields[key]).ToList<string>();
    }

    public static string GetOptionalMetadataField(
      string key,
      IReadOnlyDictionary<string, string[]> metadataFields)
    {
      if (!metadataFields.ContainsKey(key))
        return (string) null;
      if (((IEnumerable<string>) metadataFields[key]).IsNullOrEmpty<string>())
        return (string) null;
      return ((IEnumerable<string>) metadataFields[key]).FirstOrDefault<string>().IsNullOrEmpty<char>() ? (string) null : ((IEnumerable<string>) metadataFields[key]).FirstOrDefault<string>();
    }

    public static string GetRequiredMetadataField(
      string key,
      IReadOnlyDictionary<string, string[]> metadataFields)
    {
      return PyPiMetadataUtils.GetOptionalMetadataField(key, metadataFields) ?? throw new InvalidPackageException(Resources.Error_MissingIngestionMetadata((object) key));
    }

    public static T GetRequiredProperty<T>(string propertyName, JObject metadataDoc) => (metadataDoc[propertyName] ?? throw new InvalidPackageException(Resources.Error_MissingIngestionMetadata((object) propertyName))).Value<T>() ?? throw new InvalidPackageException(Resources.Error_MissingIngestionMetadata((object) propertyName));
  }
}
