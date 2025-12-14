using Microsoft.AspNetCore.Routing;

namespace Auth.Api.Extentions.MinimalApi
{
    public static class EndpointMetadataExtensions
    {
        public static TBuilder HasDescription<TBuilder>(this TBuilder builder, string description)
           where TBuilder : IEndpointConventionBuilder
        {
            builder.WithMetadata(new EndpointMetadataResponse { Description = description });
            return builder;
        }


    }
}
