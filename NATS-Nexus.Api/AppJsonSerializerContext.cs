using NATS_Nexus.Models;
using System.Text.Json.Serialization;

[JsonSerializable(typeof(Publisher))]
[JsonSerializable(typeof(Consumer))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
