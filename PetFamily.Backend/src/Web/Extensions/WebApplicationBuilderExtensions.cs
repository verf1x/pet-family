namespace Web.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static string GetSeqConnectionString(this WebApplicationBuilder builder)
    {
        return builder.Configuration.GetConnectionString("Seq")
               ?? throw new ArgumentNullException("Seq");
    }
}