// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Partitioning.PartitionResolverUtils
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Partitioning
{
  [Obsolete("Support for classes used with IPartitionResolver is now obsolete.")]
  internal static class PartitionResolverUtils
  {
    public static object ExtractPartitionKeyFromDocument(object document, string propertyName)
    {
      try
      {
        return document is Document ? ((Resource) document).GetPropertyValue<object>(propertyName) : CustomTypeExtensions.GetProperty(document.GetType(), propertyName).GetValue(document, (object[]) null);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(ClientResources.PartitionPropertyNotFound, ex);
      }
    }
  }
}
