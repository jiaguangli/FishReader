using System.IO;
using System.Text.Json;

namespace FishReader.Json
{
    public static class JsonHelper
    {
        public static string JsonName()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        }
        
        /// <summary>
        /// 从指定路径读取 JSON 文件并反序列化为指定类型对象。
        /// </summary>
        /// <typeparam name="T">要反序列化的对象类型</typeparam>
        /// <param name="filePath">JSON 文件的路径</param>
        /// <returns>反序列化后的对象</returns>
        public static T ReadFromJsonFile<T>()
        {
            var filePath = JsonName();
            
            try
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("文件未找到", filePath);
                }

                string jsonString = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<T>(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"读取 JSON 文件时出错: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 将对象序列化为 JSON 并写入到指定路径的文件中。
        /// </summary>
        /// <typeparam name="T">要序列化的对象类型</typeparam>
        /// <param name="data">要序列化的对象</param>
        public static void WriteToJsonFile<T>(T data)
        {
            var filePath = JsonName();
            try
            {
                string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true // 格式化输出
                });

                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"写入 JSON 文件时出错: {ex.Message}");
                throw;
            }
        }
    }
}