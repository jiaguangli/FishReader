using FishReader.Json;

namespace FishReader;

public class Setting
{
    /// <summary>
    /// 透明度
    /// </summary>
    public int BackGroundTransparency { get; set;}

    public int FontSize { get; set; } = 12;
    public int FontTransparency { get; set; }
    public int FontNumber { get; set; } = 200;
    
    
    public static Setting GetSetting()
    {
        var obj =  JsonHelper.ReadFromJsonFile<Config>();
        return obj.Setting; 
    }
}