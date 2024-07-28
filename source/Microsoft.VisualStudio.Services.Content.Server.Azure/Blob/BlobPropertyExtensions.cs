// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.Blob.BlobPropertyExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure.Blob
{
  public static class BlobPropertyExtensions
  {
    private static readonly BlobProperties SampleBlobProperty = new BlobProperties();
    private static readonly List<PropertyInfo> ImplementedProperties = new List<PropertyInfo>()
    {
      typeof (BlobProperties).GetProperty("ETag", BindingFlags.Instance | BindingFlags.Public),
      typeof (BlobProperties).GetProperty("ContentMD5", BindingFlags.Instance | BindingFlags.Public),
      typeof (BlobProperties).GetProperty("ContentEncoding", BindingFlags.Instance | BindingFlags.Public)
    };
    private static List<PropertyInfo> UnimplementedProperties = new List<PropertyInfo>();

    public static void SetBlobEtag(this BlobProperties blobProperties, string eTag) => typeof (BlobProperties).GetProperty("ETag", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue((object) blobProperties, (object) eTag);

    public static void InitBlobProperties() => BlobPropertyExtensions.UnimplementedProperties = ((IEnumerable<PropertyInfo>) typeof (BlobProperties).GetProperties(BindingFlags.Instance | BindingFlags.Public)).Where<PropertyInfo>((Func<PropertyInfo, bool>) (propInfo => propInfo.CanWrite && propInfo.GetSetMethod(true).IsPublic)).Except<PropertyInfo>((IEnumerable<PropertyInfo>) BlobPropertyExtensions.ImplementedProperties).ToList<PropertyInfo>();

    public static bool ArePropertiesValid(BlobProperties blobProperties, out string propertyName)
    {
      IEnumerable<string> strings = BlobPropertyExtensions.UnimplementedProperties.Select<PropertyInfo, string>((Func<PropertyInfo, string>) (x => x.Name));
      propertyName = string.Empty;
      foreach (string name in strings)
      {
        object obj1 = typeof (BlobProperties).GetProperty(name, BindingFlags.Instance | BindingFlags.Public).GetValue((object) blobProperties, (object[]) null);
        object obj2 = typeof (BlobProperties).GetProperty(name, BindingFlags.Instance | BindingFlags.Public).GetValue((object) BlobPropertyExtensions.SampleBlobProperty, (object[]) null);
        if (obj1 == null && obj2 == null)
          return true;
        if (obj1 == null && obj2 != null || obj1 != null && obj2 == null)
        {
          propertyName = name;
          return false;
        }
        if (!obj1.Equals(obj2))
        {
          propertyName = name;
          return false;
        }
      }
      return true;
    }
  }
}
