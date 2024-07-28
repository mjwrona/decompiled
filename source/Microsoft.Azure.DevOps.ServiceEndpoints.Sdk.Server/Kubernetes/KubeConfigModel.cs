// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes.KubeConfigModel
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System;
using System.IO;
using YamlDotNet.RepresentationModel;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Kubernetes
{
  internal class KubeConfigModel
  {
    internal KubernetesAuthorizationParameters kubernetesAuthorizationParameters;
    private string CurrentClusterName;
    private YamlStream kubeconfigYaml;
    private const string CurrentContextConstant = "current-context";
    private const string ContextsConstant = "contexts";
    private const string ClustersConstant = "clusters";
    private const string ClusterConstant = "cluster";
    private const string CertAuthorityConstant = "certificate-authority-data";
    private const string ClientCertConstant = "client-certificate-data";
    private const string ClientKeyConstant = "client-key-data";
    private const string TokenConstant = "token";
    private const string NameConstant = "name";
    private const string ContextConstant = "context";
    private const string UserConstant = "user";
    private const string UsersConstant = "users";
    private const string UserNameConstant = "username";
    private const string PasswordConstant = "password";

    internal string CertificateAuthorityData => this.CertificateAuthData;

    public KubeConfigModel(string kubeConfig, string clusterContext)
    {
      try
      {
        YamlStream yamlStream = new YamlStream();
        yamlStream.Load((TextReader) new StringReader(kubeConfig));
        this.kubeconfigYaml = yamlStream;
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(ServiceEndpointSdkResources.CouldNotParseKubeConfig((object) ex.Message));
      }
      this.initialize(clusterContext);
    }

    private void initialize(string clusterContext)
    {
      try
      {
        YamlMappingNode rootNode = (YamlMappingNode) this.kubeconfigYaml.Documents[0].RootNode;
        string empty = string.Empty;
        string str = string.IsNullOrWhiteSpace(clusterContext) ? this.GetStringValue("current-context", rootNode) : clusterContext;
        string b = !string.IsNullOrWhiteSpace(str) ? this.GetUserName(str, rootNode) : throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesKubeconfigFieldNotFound((object) "current-context"));
        if (string.IsNullOrWhiteSpace(b))
          throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesKubeconfigFieldNotFound((object) "user"));
        YamlSequenceNode sequenceNode = this.GetSequenceNode("users", rootNode);
        bool flag = false;
        foreach (YamlMappingNode yamlMappingNode in sequenceNode)
        {
          if (string.Equals(this.GetStringValue("name", yamlMappingNode), b, StringComparison.OrdinalIgnoreCase))
          {
            flag = true;
            this.SetAuthorizationParameters(yamlMappingNode);
            break;
          }
        }
        if (!flag)
          throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesNoUserDataFound((object) str));
        foreach (YamlMappingNode mappingNode1 in this.GetSequenceNode("clusters", rootNode))
        {
          YamlMappingNode mappingNode2 = this.GetMappingNode("cluster", mappingNode1);
          if (string.Equals(this.GetStringValue("name", mappingNode1), str, StringComparison.OrdinalIgnoreCase))
          {
            this.CertificateAuthData = this.TryGetStringValue("certificate-authority-data", mappingNode2);
            break;
          }
        }
        this.CurrentClusterName = str;
      }
      catch (InvalidOperationException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesInvalidKubeconfig((object) ex.Message), ex);
      }
    }

    private void SetAuthorizationParameters(YamlMappingNode userNode)
    {
      YamlMappingNode mappingNode = this.GetMappingNode("user", userNode);
      string stringValue1 = this.TryGetStringValue("client-certificate-data", mappingNode);
      string stringValue2 = this.TryGetStringValue("client-key-data", mappingNode);
      string stringValue3 = this.TryGetStringValue("token", mappingNode);
      string stringValue4 = this.TryGetStringValue("username", mappingNode);
      string stringValue5 = this.TryGetStringValue("password", mappingNode);
      if (!string.IsNullOrEmpty(stringValue1) && !string.IsNullOrEmpty(stringValue2))
      {
        KubernetesClientCertAuthParameters certAuthParameters = new KubernetesClientCertAuthParameters();
        certAuthParameters.SetAuthorizationParameters(stringValue1, stringValue2);
        this.kubernetesAuthorizationParameters = (KubernetesAuthorizationParameters) certAuthParameters;
      }
      else if (!string.IsNullOrEmpty(stringValue3))
      {
        KubernetesTokenBasedAuthParameters basedAuthParameters = new KubernetesTokenBasedAuthParameters();
        basedAuthParameters.SetAuthorizationParameters(stringValue3);
        this.kubernetesAuthorizationParameters = (KubernetesAuthorizationParameters) basedAuthParameters;
      }
      else
      {
        if (string.IsNullOrEmpty(stringValue4) || string.IsNullOrEmpty(stringValue5))
          throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesNoUserDataFound((object) this.CurrentClusterName));
        KubernetesBasicAuthParameters basicAuthParameters = new KubernetesBasicAuthParameters();
        basicAuthParameters.SetAuthorizationParameters(stringValue4, stringValue5);
        this.kubernetesAuthorizationParameters = (KubernetesAuthorizationParameters) basicAuthParameters;
      }
    }

    private string GetUserName(string clusterName, YamlMappingNode rootNode)
    {
      foreach (YamlMappingNode mappingNode1 in this.GetSequenceNode("contexts", rootNode))
      {
        string stringValue = this.GetStringValue("name", mappingNode1);
        YamlMappingNode mappingNode2 = this.GetMappingNode("context", mappingNode1);
        string b = clusterName;
        if (string.Equals(stringValue, b, StringComparison.CurrentCultureIgnoreCase))
          return this.GetStringValue("user", mappingNode2);
      }
      throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesNoUserDataFound((object) clusterName));
    }

    private string GetStringValue(string nodeName, YamlMappingNode mappingNode)
    {
      try
      {
        YamlScalarNode key = new YamlScalarNode(nodeName);
        return mappingNode.Children[(YamlNode) key].ToString();
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesKubeconfigErrorWhileExtractingNode((object) nodeName), ex);
      }
    }

    private string TryGetStringValue(string nodeName, YamlMappingNode mappingNode)
    {
      YamlScalarNode key = new YamlScalarNode(nodeName);
      YamlNode yamlNode;
      return mappingNode.Children.TryGetValue((YamlNode) key, out yamlNode) ? yamlNode.ToString() : (string) null;
    }

    private YamlSequenceNode GetSequenceNode(string nodeName, YamlMappingNode mappingNode)
    {
      try
      {
        YamlScalarNode key = new YamlScalarNode(nodeName);
        return (YamlSequenceNode) mappingNode.Children[(YamlNode) key];
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesKubeconfigErrorWhileExtractingNode((object) nodeName), ex);
      }
    }

    private YamlMappingNode GetMappingNode(string nodeName, YamlMappingNode mappingNode)
    {
      try
      {
        YamlScalarNode key = new YamlScalarNode(nodeName);
        return (YamlMappingNode) mappingNode.Children[(YamlNode) key];
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(ServiceEndpointSdkResources.KubernetesKubeconfigErrorWhileExtractingNode((object) nodeName), ex);
      }
    }

    private string CertificateAuthData { get; set; }
  }
}
