namespace PromoCodeFactory.BlazorClient.Models;

public class Customer
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Имя.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Фамилия.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Адрес электронной почты.
    /// </summary>
    public string? Email { get; set; }

}
