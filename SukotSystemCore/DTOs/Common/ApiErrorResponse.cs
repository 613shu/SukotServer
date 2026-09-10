namespace SukotSystemCore.DTOs.Common
{
    // The single, uniform shape every error response from the API returns.
    // Core defines it (it's a plain DTO, no EF/ASP.NET dependency) so both
    // the middleware (API) and, later, Services that want to signal an error
    // can agree on the same contract.
    public class ApiErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        // Lets the user (or support) match this exact response to the matching
        // line in the server logs, produced by CorrelationIdMiddleware.
        public string CorrelationId { get; set; } = string.Empty;
    }
}
