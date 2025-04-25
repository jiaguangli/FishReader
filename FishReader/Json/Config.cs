namespace FishReader.Json;

public class Config
{
    public Setting Setting { get; set; } = new Setting();
    public Dictionary<string, int> Path { get; set; } = [];


    public void Save()
    {
        JsonHelper.WriteToJsonFile(this);
    }

    public static Config GetConfig()
    {
        return JsonHelper.ReadFromJsonFile<Config>();
    }
}