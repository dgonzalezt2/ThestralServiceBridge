namespace ThestralServiceBridge.Domain.User;

using System.Text.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

public class UserInfoAuthDto
{
    [Required]
    [JsonPropertyName("user_id")]
    public string? UserId { get; set; }
    [Required]
    [JsonPropertyName("email")]
    public required string Email { get; set; }

    public static UserInfoAuthDto? DecodeUserInfoAuth(string base64String)
    {
        var decodedString = Decode64(base64String);
        return JsonSerializer.Deserialize<UserInfoAuthDto>(decodedString);
    }

    private static string Decode64(string text)
    {
        text = text.Replace('_', '/').Replace('-', '+');
        switch (text.Length % 4)
        {
            case 2:
                text += "==";
                break;
            case 3:
                text += "=";
                break;
        }
        return Encoding.UTF8.GetString(Convert.FromBase64String(text));
    }

}
