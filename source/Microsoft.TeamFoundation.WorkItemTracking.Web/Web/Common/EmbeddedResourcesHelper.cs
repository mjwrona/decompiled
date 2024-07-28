// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.EmbeddedResourcesHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using System.IO;
using System.Reflection;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common
{
  internal class EmbeddedResourcesHelper
  {
    internal Stream GetEmbeddedResource(string resourceName)
    {
      Assembly assembly = this.GetType().Assembly;
      return assembly.GetManifestResourceStream(assembly.GetName().Name + "." + resourceName);
    }
  }
}
