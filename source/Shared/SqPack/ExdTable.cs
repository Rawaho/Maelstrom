using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Shared.SqPack
{
    public class ExdTable<T> where T : Exd.Entry, new()
    {
        private readonly Dictionary<ExdLanguage, Dictionary<uint, T>> entries = new Dictionary<ExdLanguage, Dictionary<uint, T>>();

        public ExdTable(Exh header)
        {
            foreach (ExdLanguage language in header.Languages)
                entries.Add(language, new Dictionary<uint, T>((int)header.Entries));
        }

        public void Add(ExdLanguage language, Exd data)
        {
            foreach (T entry in data.Entries)
                entries[language].Add(entry.Index, entry);
        }

        public bool Contains(uint id, ExdLanguage language)
        {
            return entries.ContainsKey(language) && entries[language].ContainsKey(id);
        }

        public bool TryGetValue(uint id, ExdLanguage language, out T entry)
        {
            entry = null;
            return entries.ContainsKey(language) && entries[language].TryGetValue(id, out entry);
        }

        public T GetValue(uint id, ExdLanguage language)
        {
            return entries[language][id];
        }

        public ReadOnlyCollection<T> GetValues(ExdLanguage language)
        {
            return entries.ContainsKey(language) ? new ReadOnlyCollection<T>(entries[language].Values.ToList()) : null;
        }

        public static ExdTable<T> Load(string name)
        {
            var header   = new Exh(name);
            var exdTable = new ExdTable<T>(header);

            foreach (ExdLanguage language in header.Languages)
            {
                for (uint i = 0u; i < header.Pages.Length; i++)
                {
                    string exdPath = Path.Combine(Path.GetDirectoryName(name) ?? "", $"{Path.GetFileNameWithoutExtension(name)}_{header.Pages[i].Id}");
                    if (language != ExdLanguage.None)
                        exdPath += $"_{language.ToString().ToLower()}";

                    exdPath = Path.ChangeExtension(exdPath, ".exd");

                    // some EXH files contain language codes but don't have the associated EXD files
                    if (!File.Exists(exdPath))
                        continue;

                    var exd = new Exd();
                    exd.Load<T>(exdPath, header, i);
                    exdTable.Add(language, exd);
                }
            }

            return exdTable;
        }
    }
}
