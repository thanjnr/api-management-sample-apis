param name string
param location string
param tags object = {}
param appSettings object = {}
param connectionStrings object = {}

param applicationInsightsName string = ''
param appServicePlanId string

module apiService '../core/host/appservice.bicep' = {
  name: name
  params: {
    name: name
    location: location
    tags: tags
    appCommandLine: ''
    applicationInsightsName: applicationInsightsName
    appServicePlanId: appServicePlanId
    appSettings: appSettings
    connectionStrings: connectionStrings
    runtimeName: 'dotnetcore'
    runtimeVersion: '9.0'
    scmDoBuildDuringDeployment: false
    useManagedIdentity: true
  }
}

output serviceUri string = apiService.outputs.uri
output servicePrincipalId string = apiService.outputs.servicePrincipalId
