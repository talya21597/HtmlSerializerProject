using System;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace HtmlSerializer
{
    // Singleton Pattern - רק מופע אחד של המחלקה
    public class HtmlHelper
    {
        private static HtmlHelper _instance;
        private static readonly object _lock = new object();

        public string[] AllTags { get; private set; }
        public string[] SelfClosingTags { get; private set; }

        // Constructor פרטי - מונע יצירת מופעים מבחוץ
        private HtmlHelper()
        {
            // טעינת קבצי JSON
            try
            {
                string tagsJson = File.ReadAllText("HtmlTags.json");
                AllTags = JsonSerializer.Deserialize<string[]>(tagsJson);

                string voidTagsJson = File.ReadAllText("HtmlVoidTags.json");
                SelfClosingTags = JsonSerializer.Deserialize<string[]>(voidTagsJson);

                Console.WriteLine($"✅ HtmlHelper loaded: {AllTags.Length} tags, {SelfClosingTags.Length} void tags");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading JSON files: {ex.Message}");
                Console.WriteLine("Make sure HtmlTags.json and HtmlVoidTags.json are in the output directory");
                AllTags = new string[0];
                SelfClosingTags = new string[0];
            }
        }

        // גישה למופע היחיד - Thread Safe (Double-Check Locking)
        public static HtmlHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new HtmlHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        // בדיקה אם תגית היא תגית HTML תקינה
        public bool IsValidTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            return AllTags.Contains(tag.ToLower());
        }

        // בדיקה אם תגית היא self-closing
        public bool IsSelfClosing(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            return SelfClosingTags.Contains(tag.ToLower());
        }
    }
}