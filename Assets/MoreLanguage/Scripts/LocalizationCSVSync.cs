using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Settings;
using System.Collections;
using System.Linq;

public class LocalizationCSVSync : MonoBehaviour
{
    public string csvUrl;

    IEnumerator Start()
    {
        yield return DownloadAndApply();
    }

    IEnumerator DownloadAndApply()
    {
        UnityWebRequest req = UnityWebRequest.Get(csvUrl);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(req.error);
            yield break;
        }

        ApplyCSV(req.downloadHandler.text);
        LocalizationSettings.Instance.SetSelectedLocale(
            LocalizationSettings.SelectedLocale
        );
    }

    void ApplyCSV(string csv)
    {
        var lines = csv.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        var headers = lines[0].Split(',');

        var table = LocalizationSettings.StringDatabase.GetTable("StringTable");

        for (int i = 1; i < lines.Length; i++)
        {
            var row = lines[i].Split(',');
            if (row.Length < headers.Length) continue;

            string key = row[0].Trim();

            var entry = table.GetEntry(key);
            if (entry == null)
                entry = table.AddEntry(key, "");


            for (int j = 1; j < headers.Length; j++)
            {
                string localeCode = headers[j].Trim();
                string value = row[j].Trim();

                var locale = LocalizationSettings.AvailableLocales
                    .Locales
                    .FirstOrDefault(l => l.Identifier.Code == localeCode);

                if (locale == null)
                {
                    Debug.LogWarning("Locale not found: " + localeCode);
                    continue;
                }

                var stringTable = LocalizationSettings.StringDatabase
                    .GetTable(table.TableCollectionName, locale);

                if (stringTable == null)
                {
                    Debug.LogWarning("Table not found for locale: " + localeCode);
                    continue;
                }

                var localeEntry = stringTable.GetEntry(key);

                if (localeEntry == null)
                    stringTable.AddEntry(key, value);
                else
                    localeEntry.Value = value;
            }
        }

        Debug.Log("Localization Updated");
    }
}