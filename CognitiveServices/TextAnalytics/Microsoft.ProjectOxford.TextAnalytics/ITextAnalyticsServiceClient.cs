using System;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.TextAnalytics.Contract.LanguageResponse;

namespace Microsoft.ProjectOxford.TextAnalytics
{
    internal interface ITextAnalyticsServiceClient
    {
        Task<Text> AnalyzeLanguageAsync(string text);
        Task<Contract.SentimentResponse.Sentiment> AnalyzeSentimentAsync(string text);
    }
}
