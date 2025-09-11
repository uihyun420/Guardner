using System.Collections.Generic;
using UnityEngine;

public class StringTable : DataTable
{
    private readonly string Error = "¿¡·¯";

    private readonly Dictionary<string, string> dictionary = new Dictionary<string, string>(); 
    public class StringData
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }
    public override void Load(string filename)
    {
        dictionary.Clear();

        var path = string.Format(FormatPath, filename);
        var textAsset = Resources.Load<TextAsset>(path);
        var list = LoadCSV<StringData>(textAsset.text);
        foreach(var text in list)
        {
            if(!dictionary.ContainsKey(text.Id))
            {
                dictionary.Add(text.Id, text.Text);
            }
        }
    }
    public string Get(string key)
    {
        if(!dictionary.ContainsKey(key))
        {
            return Error;
        }
        return dictionary[key];
    }

}
