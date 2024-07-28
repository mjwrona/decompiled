// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.ReleaseHelper
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common
{
  public class ReleaseHelper
  {
    private const string c_webUriKey = "web";
    private const string c_hrefUriKey = "href";
    public const string deploymentConstant = "deployment";
    public const string environmentConstant = "environment";
    public const string ownerConstant = "owner";
    public const string uniqueNameConstant = "uniqueName";
    public const string displayNameConstant = "displayName";
    public const string nameConstant = "name";
    public const string deploymentStatusConstant = "deploymentStatus";
    public const string completedOnConstant = "completedOn";
    public const string startedOnConstant = "startedOn";
    public const string releaseConstant = "release";
    public const string releaseEnvironmentConstant = "releaseEnvironment";
    public const string linksConstant = "_links";

    public static string GetReleasePropertyNameValueFromDeployment(
      JToken deployment,
      string propertyConstant)
    {
      JToken childTokenFromJtoken = ReleaseHelper.GetChildTokenFromJToken(deployment, propertyConstant);
      return !childTokenFromJtoken.HasValues ? (string) null : ReleaseHelper.GetStringFromJToken(childTokenFromJtoken, "name");
    }

    public static string GetReleasePropertyLinkValueFromDeployment(
      JToken deployment,
      string propertyConstant)
    {
      JToken childTokenFromJtoken = ReleaseHelper.GetChildTokenFromJToken(deployment, propertyConstant);
      if (!childTokenFromJtoken.HasValues)
        return (string) null;
      string stringFromJtoken = ReleaseHelper.GetStringFromJToken(childTokenFromJtoken, "name");
      return ReleaseHelper.BuildReleaseTextLink(ReleaseHelper.GetStringFromJToken(ReleaseHelper.GetChildTokenFromJToken(ReleaseHelper.GetChildTokenFromJToken(childTokenFromJtoken, "_links"), "web"), "href"), stringFromJtoken);
    }

    public static string BuildReleaseTextLink(string url, string name) => string.Format(CommonConsumerResources.ReleaseHelper_TextLinkFormat, (object) url, (object) name);

    public static string GetDeploymentStatus(JToken deployment) => ReleaseHelper.GetStringFromJToken(deployment, "deploymentStatus");

    public static string GetOwnerUniqueName(JToken owner) => ReleaseHelper.GetStringFromJToken(owner, "uniqueName");

    public static string GetOwnerDisplayName(JToken owner) => ReleaseHelper.GetStringFromJToken(owner, "displayName");

    public static JToken GetJTokenFromJObject(JObject resource, string propertyName)
    {
      try
      {
        return resource[propertyName];
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format(CommonConsumerResources.ReleaseHelper_ErrorWhileExtractingJToken, (object) propertyName), ex);
      }
    }

    public static JToken GetChildTokenFromJToken(JToken resource, string propertyName)
    {
      try
      {
        return resource[(object) propertyName];
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format(CommonConsumerResources.ReleaseHelper_ErrorWhileExtractingJToken, (object) propertyName), ex);
      }
    }

    public static string GetStringFromJToken(JToken resource, string propertyName)
    {
      try
      {
        return (string) resource[(object) propertyName];
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format(CommonConsumerResources.ReleaseHelper_ErrorWhileExtractingJToken, (object) propertyName), ex);
      }
    }
  }
}
