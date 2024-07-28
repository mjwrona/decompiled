// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.DataImportExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  public static class DataImportExtensions
  {
    public const int ValidationChecksumVersion1 = 1;
    private static readonly byte[] ChecksumVersion1HaskKeyBytes = Encoding.UTF8.GetBytes("ValidationDataHashKey");

    public static int LatestChecksumVersion => 1;

    public static string CreateValidationDataChecksum(this DataImportPackage package)
    {
      ArgumentUtility.CheckForNull<DataImportPackage>(package, nameof (package));
      ArgumentUtility.CheckForNull<PropertiesCollection>(package.ValidationData, "ValidationData");
      PropertiesCollection validationData = new PropertiesCollection((IDictionary<string, object>) package.ValidationData);
      int num = validationData.GetValue<int>("ValidationChecksumVersion", DataImportExtensions.LatestChecksumVersion);
      validationData.Remove("ValidationChecksum");
      validationData.Remove("ValidationChecksumVersion");
      if (num == 1)
        return DataImportExtensions.GetSHA512HashBase64Encoded(validationData);
      throw new InvalidDataImportPropertyValueException(string.Format("Checksum version {0} not supported {1}", (object) num, (object) "ValidationChecksumVersion"));
    }

    private static string GetSHA512HashBase64Encoded(PropertiesCollection validationData)
    {
      using (HMACSHA512Hash hmacshA512Hash = new HMACSHA512Hash(JsonConvert.SerializeObject((object) validationData, Formatting.None, (JsonConverter[]) new DataImportPackage.PropertiesCollectionConverter[1]
      {
        new DataImportPackage.PropertiesCollectionConverter()
      }), DataImportExtensions.ChecksumVersion1HaskKeyBytes))
        return hmacshA512Hash.HashBase64Encoded;
    }
  }
}
