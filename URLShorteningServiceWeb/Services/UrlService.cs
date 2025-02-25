using MudBlazor;
using System.Text.Json;
using System.Text;
using System;
using UrlShorteningServiceWeb.Models;

namespace UrlShorteningServiceWeb.Services
{
    public class UrlService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ValidatorUrlService _validatorUrlService;

        public UrlService(IConfiguration config, IHttpClientFactory clientFactory, ValidatorUrlService validatorUrlService)
        {
            _configuration = config;
            _clientFactory = clientFactory;
            _validatorUrlService = validatorUrlService;
        }

        public string HostBaseUrl()
        {
            return _configuration["Host:BaseUrl"] ?? "";
        }

        public async Task<Result?> ShortenUrl(string url)
        {
            var result = new Result
            {
                message = "Unexpected error.",
                type = TypeResult.Error
            };

            var isValidUrl = _validatorUrlService.ValidateUrlWithRegex(url);

            if (!isValidUrl)
            {
               return result = new Result
               {
                   message = "Invalid URL",
                   type = TypeResult.Error
               };
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{_configuration["Api:BaseUrl"]}/shorten");

                var data = new UrlString { url = url };
                var json = JsonSerializer.Serialize(data);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    var resultApi = await JsonSerializer.DeserializeAsync<UrlGet>(responseStream);
                    return result = new Result
                    {
                        message = "Shortened URL.",
                        url = $"{HostBaseUrl()}/{resultApi.shortCode}",
                        type = TypeResult.Success
                    };
                }
                else
                {
                    return result = new Result {
                        message = "Failed to shorten URL.",
                        type = TypeResult.Warning
                    };  
                }
            }
            catch (Exception ex)
            {
                return result = new Result
                {
                    message = $"Error: {ex.Message}",
                    type = TypeResult.Error
                };
            }
        }

        public async Task<Result?> OriginalUrl(string shortCode)
        {
            var result = new Result
            {
                message = "Unexpected error.",
                type = TypeResult.Error
            };

            if(string.IsNullOrEmpty(shortCode))
            {
                return result = new Result
                {
                    message = "Invalid short code.",
                    type = TypeResult.Error
                };
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["Api:BaseUrl"]}/shorten/{shortCode}");
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    var resultApi = await JsonSerializer.DeserializeAsync<UrlStatics>(responseStream);
                    return result = new Result
                    {
                        message = $"{resultApi.url}",
                        type = TypeResult.Success
                    };
                }
                else
                {
                    return result = new Result
                    {
                        message = "Failed to find URL.",
                        type = TypeResult.Warning
                    };
                }
            }
            catch (Exception ex)
            {
                return result = new Result
                {
                    message = $"Error: {ex.Message}",
                    type = TypeResult.Error
                };
            }
        }

        public async Task<Result?> StatsUrl(string shortCode)
        {
            var result = new Result
            {
                message = "Unexpected error.",
                type = TypeResult.Error
            };

            if (string.IsNullOrEmpty(shortCode))
            {
                return result = new Result
                {
                    message = "Invalid short code.",
                    type = TypeResult.Error
                };
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_configuration["Api:BaseUrl"]}/shorten/{shortCode}/stats");
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    var resultApi = await JsonSerializer.DeserializeAsync<UrlStatics>(responseStream);
                    return result = new Result
                    {
                        message = $"Total clicks: {resultApi.accessCount}",
                        type = TypeResult.Info
                    };
                }
                else
                {
                    return result = new Result
                    {
                        message = "Failed to find URL.",
                        type = TypeResult.Warning
                    };
                }
            }
            catch (Exception ex)
            {
                return result = new Result
                {
                    message = $"Error: {ex.Message}",
                    type = TypeResult.Error
                };
            }
        }
    }
}
