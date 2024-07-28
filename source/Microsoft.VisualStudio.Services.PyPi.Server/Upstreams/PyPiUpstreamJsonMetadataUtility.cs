// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.PyPiUpstreamJsonMetadataUtility
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public class PyPiUpstreamJsonMetadataUtility
  {
    public static Dictionary<string, string[]> ConvertJsonMetadataToDictionary(JToken json)
    {
      NameValueCollection nameValueCollection = new NameValueCollection();
      PyPiUpstreamJsonMetadataUtility.FillNameValueCollectionFromJson(nameValueCollection, json);
      return ((IEnumerable<string>) nameValueCollection.AllKeys).ToDictionary<string, string, string[]>((Func<string, string>) (key => key), new Func<string, string[]>(nameValueCollection.GetValues));
    }

    public static Dictionary<string, string[]> GetPackageLevelMetadataDictionary(JToken json) => PyPiUpstreamJsonMetadataUtility.ConvertJsonMetadataToDictionary(json[(object) "info"]);

    public static Dictionary<string, string[]> GetFileLevelMetadataDictionary(
      JToken json,
      string fileName,
      out JToken fileMetadataNode)
    {
      fileMetadataNode = json[(object) "urls"].Children().Where<JToken>((Func<JToken, bool>) (fileMetadata => PyPiUpstreamJsonMetadataUtility.IsRequestedFileMetadata(fileName, fileMetadata))).FirstOrDefault<JToken>();
      return fileMetadataNode != null ? PyPiUpstreamJsonMetadataUtility.ConvertJsonMetadataToDictionary(fileMetadataNode) : (Dictionary<string, string[]>) null;
    }

    private static bool IsRequestedFileMetadata(string fileName, JToken fileMetadata) => fileMetadata[(object) "filename"].ToString().Equals(fileName, StringComparison.Ordinal);

    private static void FillNameValueCollectionFromJson(
      NameValueCollection nameValueCollection,
      JToken upstreamJson)
    {
      foreach (JToken jtoken1 in (IEnumerable<JToken>) upstreamJson)
      {
        string name = ((JProperty) jtoken1).Name;
        JToken jtoken2 = ((JProperty) jtoken1).Value;
        switch (jtoken2.Type)
        {
          case JTokenType.Object:
            using (IEnumerator<JProperty> enumerator = jtoken2.Children<JProperty>().GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                JProperty current = enumerator.Current;
                nameValueCollection.Add(name, current.Name + "," + current.Value.ToString());
              }
              continue;
            }
          case JTokenType.Array:
            using (IEnumerator<JToken> enumerator = jtoken2.Children().GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                JToken current = enumerator.Current;
                nameValueCollection.Add(name, current.ToString());
              }
              continue;
            }
          case JTokenType.Integer:
          case JTokenType.Float:
          case JTokenType.String:
          case JTokenType.Boolean:
          case JTokenType.Null:
          case JTokenType.Date:
          case JTokenType.Uri:
          case JTokenType.TimeSpan:
            JProperty jproperty = (JProperty) jtoken1;
            nameValueCollection.Add(name, jproperty.Value.ToString());
            continue;
          default:
            throw new PyPiUpstreamMetadataParsingException();
        }
      }
    }
  }
}
