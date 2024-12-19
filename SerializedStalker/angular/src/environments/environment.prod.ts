import { Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44397/',
  redirectUri: baseUrl,
  clientId: 'SerializedStalker_App',
  responseType: 'code',
  scope: 'offline_access SerializedStalker',
  requireHttps: true,
};

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'SerializedStalker',
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44397',
      rootNamespace: 'SerializedStalker',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
  remoteEnv: {
    url: '/getEnvConfig',
    mergeStrategy: 'deepmerge'
  }
} as Environment;
