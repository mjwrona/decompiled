// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.ResourceHelper
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ResourceHelper
  {
    private const string c_resourcesSuffix = ".resources";

    public static ResourceManager GetResourceManager(string resourcesName)
    {
      Assembly assembly = typeof (ResourceHelper).Assembly;
      return new ResourceManager(ResourceHelper.RemoveResourcesSuffix(resourcesName), assembly);
    }

    public static string GetResourceStreamAsString(string resourceName, bool isBinaryContent = false) => ResourceHelper.GetResourceStreamAsString(typeof (ResourceHelper).Assembly, resourceName, isBinaryContent);

    public static string GetResourceStreamAsString(
      Assembly assembly,
      string resourceName,
      bool isBinaryContent)
    {
      using (Stream resourceStream = ResourceHelper.GetResourceStream(assembly, resourceName))
      {
        if (resourceStream == null)
          return string.Empty;
        if (isBinaryContent)
          return ResourceHelper.ReadAllAsBase64String(resourceStream);
        using (StreamReader streamReader = new StreamReader(resourceStream))
          return streamReader.ReadToEnd();
      }
    }

    public static Stream GetResourceStream(Assembly assembly, string resourceName) => assembly.GetManifestResourceStream(resourceName);

    public static string RemoveResourcesSuffix(string resourceName) => resourceName.RemoveSuffix(".resources");

    private static string ReadAllAsBase64String(Stream stream)
    {
      byte[] numArray = new byte[stream.Length];
      return (long) stream.Read(numArray, 0, numArray.Length) < stream.Length ? string.Empty : Convert.ToBase64String(numArray);
    }
  }
}
