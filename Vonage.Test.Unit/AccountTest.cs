﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace Vonage.Test.Unit
{
    public class AccountTest : TestBase
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async void GetAccountBalanceAsync(bool passCreds, bool asyncTest)
        {
            //ARRANGE
            var expectedUri = $"{RestUrl}/account/get-balance?api_key={ApiKey}&api_secret={ApiSecret}&";
            var expectedResponseContent = @"{""value"": 3.14159, ""autoReload"": false }";
            Setup(uri: expectedUri, responseContent: expectedResponseContent);

            var creds = Request.Credentials.FromApiKeyAndSecret(ApiKey, ApiSecret);
            var client = new VonageClient(creds);
            Accounts.Balance balance;
            if (asyncTest)
            {
                if (passCreds)
                {
                    balance = await client.AccountClient.GetAccountBalanceAsync(creds);
                }
                else
                {
                    balance = await client.AccountClient.GetAccountBalanceAsync();
                }
            }
            else
            {
                balance = client.AccountClient.GetAccountBalance();
            }

            //ASSERT
            Assert.Equal(3.14159m, balance.Value);
            Assert.False(balance.AutoReload);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async void SetSettings(bool passCreds, bool asyncTest)
        {
            //ARRANGE
            var expectedUri = $"{RestUrl}/account/settings";
            var expectedRequestContents = $"moCallBackUrl={HttpUtility.UrlEncode("https://example.com/webhooks/inbound-sms")}&drCallBackUrl={HttpUtility.UrlEncode("https://example.com/webhooks/delivery-receipt")}&api_key={ApiKey}&api_secret={ApiSecret}&";
            var expectedResponseContent = @"{""mo-callback-url"": ""https://example.com/webhooks/inbound-sms"",""dr-callback-url"": ""https://example.com/webhooks/delivery-receipt"",""max-outbound-request"": 15,""max-inbound-request"": 30,""max-calls-per-second"": 4}";
            Setup(uri: expectedUri, responseContent: expectedResponseContent, requestContent: expectedRequestContents);

            //ACT
            var creds = Request.Credentials.FromApiKeyAndSecret(ApiKey, ApiSecret);
            var client = new VonageClient(creds);
            Accounts.AccountSettingsResult result;
            if (asyncTest)
            {
                if (passCreds)
                {
                    result = await client.AccountClient.ChangeAccountSettingsAsync(new Accounts.AccountSettingsRequest { MoCallBackUrl = "https://example.com/webhooks/inbound-sms", DrCallBackUrl = "https://example.com/webhooks/delivery-receipt" }, creds);
                }
                else
                {
                    result = await client.AccountClient.ChangeAccountSettingsAsync(new Accounts.AccountSettingsRequest { MoCallBackUrl = "https://example.com/webhooks/inbound-sms", DrCallBackUrl = "https://example.com/webhooks/delivery-receipt" });
                }
            }
            else
            {
                result = client.AccountClient.ChangeAccountSettings(new Accounts.AccountSettingsRequest { MoCallBackUrl = "https://example.com/webhooks/inbound-sms", DrCallBackUrl = "https://example.com/webhooks/delivery-receipt" });
            }

            //ASSERT
            Assert.Equal("https://example.com/webhooks/delivery-receipt", result.DrCallbackurl);
            Assert.Equal("https://example.com/webhooks/inbound-sms", result.MoCallbackUrl);
            Assert.Equal(4, result.MaxCallsPerSecond);
            Assert.Equal(30, result.MaxInboundRequest);
            Assert.Equal(15, result.MaxOutboundRequest);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async void TopUp(bool passCreds, bool asyncTest)
        {
            //ARRANGE            
            var expectedUri = $"{RestUrl}/account/top-up?trx=00X123456Y7890123Z&api_key={ApiKey}&api_secret={ApiSecret}&";
            var expectedResponseContent = @"{""response"":""abc123""}";
            Setup(uri: expectedUri, responseContent: expectedResponseContent);

            var creds = Request.Credentials.FromApiKeyAndSecret(ApiKey, ApiSecret);
            //Act
            var client = new VonageClient(creds);
            Accounts.TopUpResult response;
            if (asyncTest) 
            {
                if (passCreds)
                {
                    response = await client.AccountClient.TopUpAccountBalanceAsync(new Accounts.TopUpRequest { Trx = "00X123456Y7890123Z" }, creds);
                }
                else
                {
                    response = await client.AccountClient.TopUpAccountBalanceAsync(new Accounts.TopUpRequest { Trx = "00X123456Y7890123Z" });
                }
            }
            else
            {
                response = client.AccountClient.TopUpAccountBalance(new Accounts.TopUpRequest { Trx = "00X123456Y7890123Z" });
            }
            Assert.Equal("abc123",response.Response);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async void GetNumbers(bool passCreds, bool asyncTest)
        {
            //ARRANGE
            var expectedUri = $"{RestUrl}/account/numbers?api_key={ApiKey}&api_secret={ApiSecret}&";
            var expectedResponseContent = @"{""count"":1,""numbers"":[{""country"":""US"",""msisdn"":""17775551212"",""type"":""mobile-lvn"",""features"":[""VOICE"",""SMS""]}]}";
            Setup(uri: expectedUri, responseContent: expectedResponseContent);

            //Act
            var creds = Request.Credentials.FromApiKeyAndSecret(ApiKey, ApiSecret);
            var client = new VonageClient(creds);
            Numbers.NumbersSearchResponse numbers;
            if (asyncTest)
            {
                if (passCreds)
                {
                    numbers = await client.NumbersClient.GetOwnedNumbersAsync(new Numbers.NumberSearchRequest(), creds);
                }
                else
                {
                    numbers = await client.NumbersClient.GetOwnedNumbersAsync(new Numbers.NumberSearchRequest());
                } 
            }
            else
            {
                numbers = client.NumbersClient.GetOwnedNumbers(new Numbers.NumberSearchRequest());
            }

            //ASSERT
            Assert.Equal(1, numbers.Count);
            Assert.Equal("17775551212", numbers.Numbers[0].Msisdn);
            Assert.Equal("US", numbers.Numbers[0].Country);
            Assert.Equal("mobile-lvn", numbers.Numbers[0].Type);
            Assert.Equal("VOICE", numbers.Numbers[0].Features.First());
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async void RetrieveApiSecrets(bool passCreds, bool asyncTest)
        {
            //ARRANGE            
            var expectedResponse = @"{
                  ""_links"": {
                    ""self"": {
                        ""href"": ""abc123""
                      }
                  },
                  ""_embedded"": {
                    ""secrets"": [
                      {
                        ""_links"": {
                          ""self"": {
                            ""href"": ""abc123""
                          }
                        },
                        ""id"": ""ad6dc56f-07b5-46e1-a527-85530e625800"",
                        ""created_at"": ""2017-03-02T16:34:49Z""
                      }
                    ]
                  }
                }";
            var expectedUri = $"https://api.nexmo.com/accounts/{ApiKey}/secrets";
            Setup(expectedUri, expectedResponse);

            //ACT
            var creds = Request.Credentials.FromApiKeyAndSecret(ApiKey, ApiSecret);
            var client = new VonageClient(creds);
            Accounts.SecretsRequestResult secrets;
            if (asyncTest)
            {
                if (passCreds)
                {
                    secrets = await client.AccountClient.RetrieveApiSecretsAsync(ApiKey, creds);
                }
                else
                {
                    secrets = await client.AccountClient.RetrieveApiSecretsAsync(ApiKey);
                } 
            }
            else
            {
                secrets = client.AccountClient.RetrieveApiSecrets(ApiKey);
            }
            

            //ASSERT
            Assert.Equal("ad6dc56f-07b5-46e1-a527-85530e625800", secrets.Embedded.Secrets[0].Id);
            Assert.Equal("2017-03-02T16:34:49Z", secrets.Embedded.Secrets[0].CreatedAt);
            Assert.Equal("abc123", secrets.Embedded.Secrets[0].Links.Self.Href);
            Assert.Equal("abc123", secrets.Links.Self.Href);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async void CreateApiSecret(bool passCreds, bool asyncTest)
        {            
            //ARRANGE            
            var expectedUri = $"https://api.nexmo.com/accounts/{ApiKey}/secrets";
            var expectedResponse = @"{
                  ""_links"": {
                    ""self"": {
                           ""href"": ""abc123""
                      }
                    },
                  ""id"": ""ad6dc56f-07b5-46e1-a527-85530e625800"",
                  ""created_at"": ""2017-03-02T16:34:49Z""
                }";
            Setup(expectedUri, expectedResponse);

            //ACT
            var creds = Request.Credentials.FromApiKeyAndSecret(ApiKey, ApiSecret);
            var client = new VonageClient(creds);
            Accounts.Secret secret;
            if (asyncTest)
            {
                if (passCreds)
                {
                    secret = await client.AccountClient.CreateApiSecretAsync(new Accounts.CreateSecretRequest { Secret = "password" }, ApiKey, creds);
                }
                else
                {
                    secret = await client.AccountClient.CreateApiSecretAsync(new Accounts.CreateSecretRequest { Secret = "password" }, ApiKey);
                } 
            }
            else
            {
                secret = client.AccountClient.CreateApiSecret(new Accounts.CreateSecretRequest { Secret = "password" }, ApiKey, creds);
            }
            

            //ASSERT
            Assert.Equal("ad6dc56f-07b5-46e1-a527-85530e625800", secret.Id);
            Assert.Equal("2017-03-02T16:34:49Z", secret.CreatedAt);
            Assert.Equal("abc123", secret.Links.Self.Href);            
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async void RetrieveSecret(bool passCreds, bool asyncTest)
        {

            //ARRANGE            
            var secretId = "ad6dc56f-07b5-46e1-a527-85530e625800";
            var expectedUri = $"https://api.nexmo.com/accounts/{ApiKey}/secrets/{secretId}";
            var expectedResponse = @"{
                  ""_links"": {
                    ""self"": {
                           ""href"": ""abc123""
                      }
                    },
                  ""id"": ""ad6dc56f-07b5-46e1-a527-85530e625800"",
                  ""created_at"": ""2017-03-02T16:34:49Z""
                }";
            Setup(expectedUri, expectedResponse);

            //ACT
            var creds = Request.Credentials.FromApiKeyAndSecret(ApiKey, ApiSecret);

            var client = new VonageClient(creds);
            Accounts.Secret secret;
            if (asyncTest)
            {
                if (passCreds)
                {
                    secret = await client.AccountClient.RetrieveApiSecretAsync(secretId, ApiKey, creds);
                }
                else
                {
                    secret = await client.AccountClient.RetrieveApiSecretAsync(secretId, ApiKey);
                } 
            }
            else
            {
                secret = client.AccountClient.RetrieveApiSecret(secretId, ApiKey);
            }
            

            //ASSERT
            Assert.Equal(secretId, secret.Id);
            Assert.Equal("2017-03-02T16:34:49Z", secret.CreatedAt);
            Assert.Equal("abc123", secret.Links.Self.Href);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public async void RevokeSecret(bool passCreds, bool asyncTest)
        {
            //ARRANGE            
            var secretId = "ad6dc56f-07b5-46e1-a527-85530e625800";
            var expectedUri = $"https://api.nexmo.com/accounts/{ApiKey}/secrets/{secretId}";
            var expectedResponse = @"";
            Setup(expectedUri, expectedResponse);

            //ACT
            var creds = Request.Credentials.FromApiKeyAndSecret(ApiKey, ApiSecret);
            var client = new VonageClient(creds);
            bool response;
            if (asyncTest)
            {
                if (passCreds)
                {
                    response = await client.AccountClient.RevokeApiSecretAsync(secretId, ApiKey, creds);
                }
                else
                {
                    response = await client.AccountClient.RevokeApiSecretAsync(secretId, ApiKey);
                } 
            }
            else
            {
                response = client.AccountClient.RevokeApiSecret(secretId, ApiKey);
            }

            //ASSERT
            Assert.True(response);
        }
    }
}
