namespace ZestPost.Base.Controller
{
    public class JsonConfigBuilder
    {
        private readonly string _defaultJsonPath;
        private readonly JObject _json;
        public JObject json;

        public JsonConfigBuilder()
        {
            _json = new JObject();
            _defaultJsonPath = Path.Combine(AppContext.BaseDirectory, "settings");
        }
        public JsonConfigBuilder(string jsonPathOrString, bool isString = false, bool schedule_mode = false, string identity = "")
        {
            try
            {
                if (jsonPathOrString.EndsWith(".json")) jsonPathOrString = jsonPathOrString.Substring(0, jsonPathOrString.Length - ".json".Length);
                if (isString)
                {
                    jsonPathOrString = jsonPathOrString.IfEmpty("{}");
                    _json = JObject.Parse(jsonPathOrString);
                }
                else
                {
                    if (jsonPathOrString.Contains(AppContext.BaseDirectory)) _defaultJsonPath = jsonPathOrString;
                    else if (schedule_mode)
                    {
                        _defaultJsonPath = $"{AppContext.BaseDirectory}settings\\Schedule\\{identity}\\{jsonPathOrString}.json";
                        HelperStringSync.CreateDirectoryIfNotExists(_defaultJsonPath);
                    }
                    else
                    {
                        _defaultJsonPath = $"{AppContext.BaseDirectory}\\settings\\{jsonPathOrString}.json";
                        HelperStringSync.CreateDirectoryIfNotExists(_defaultJsonPath);
                    }

                    if (!File.Exists(_defaultJsonPath))
                    {
                        File.WriteAllText(_defaultJsonPath, "{}");
                    }
                    _json = JObject.Parse(File.ReadAllText(_defaultJsonPath));
                }
            }
            catch
            {
                _json = new JObject();
            }
        }

        public int GetValueInt(string key, int defaultValue = 0)
        {
            try
            {
                return int.TryParse(GetValue(key), out var result) ? result : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        public bool GetValueBool(string key, bool defaultValue = false)
        {
            try
            {
                return bool.TryParse(GetValue(key), out var result) ? result : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        public string GetValue(string key, string defaultValue = "")
        {
            try
            {
                return _json.TryGetValue(key, out var value) ? value.ToString() : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        public string GetFullString()
        {
            return _json.ToString(Newtonsoft.Json.Formatting.None);
        }
        public DateTime GetValueDateTime(string key, DateTime? defaultValue = null)
        {
            defaultValue ??= DateTime.Now;
            try
            {
                return DateTime.TryParse(GetValue(key), out var result) ? result : defaultValue.Value;
            }
            catch
            {
                return defaultValue.Value;
            }
        }
        public T GetValueObject<T>(string key, T value)
        {
            T result = value;
            try
            {
                result = _json.GetValue(key) != null ? JsonConvert.DeserializeObject<T>(_json.GetValue(key).ToString()) : value;
            }
            catch { }
            return result;
        }

        //public List<string> GetValueList(string key, int typeSplitString = 0)
        //{
        //    try
        //    {
        //        var value = _json[key]?.ToString() ?? "";
        //        var splitPattern = typeSplitString == 0 ? new[] { "\n", "\r" } : new[] { "\n|\n" };
        //        return value.Split(splitPattern, StringSplitOptions.RemoveEmptyEntries).AsList();
        //    }
        //    catch
        //    {
        //        return new List<string>();
        //    }
        //}
        public void AddOrUpdate(string key, object value)
        {
            try
            {
                if (value is List<string> lst)
                {
                    _json[key] = string.Join("\n", lst);
                }
                else
                {
                    _json[key] = JToken.FromObject(value);
                }
            }
            catch (Exception ex)
            {
                Log4NetSyncController.LogException(ex, $"Lỗi khi thêm hoặc sửa key '{key}': {ex.Message}");
            }
        }
        public void Remove(string key)
        {
            _json.Remove(key);
        }
        public void Save(string pathFileSetting = "")
        {
            try
            {
                if (pathFileSetting.IsEmpty())
                {
                    pathFileSetting = _defaultJsonPath;
                }
                File.WriteAllText(pathFileSetting, _json.ToString());
            }
            catch { }
        }
        public static Dictionary<string, object> ConvertToDictionary(JObject jObject)
        {
            try
            {
                return jObject.ToObject<Dictionary<string, object>>();
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }

    }
}
