using System;
using System.Linq;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("🚀 HTML Serializer & Query - Starting...\n");

            try
            {
                // יצירת מופע של הסריאליזר
                var serializer = new HtmlSerializer();

                // דוגמה 1: קריאת HTML מכתובת אינטרנט
                Console.WriteLine("📥 Loading HTML from URL...");
                string url = "https://www.w3schools.com/html/html_examples.asp"; // ניתן להחליף לכל כתובת אחרת

                var root = await serializer.SerializeAsync(url);
                Console.WriteLine("✅ HTML loaded and parsed successfully!\n");

                // דוגמה 2: חיפוש אלמנטים
                Console.WriteLine("🔍 Running queries:\n");

                // חיפוש כל ה-div elements
                var divs = root.FindByTagName("div");
                Console.WriteLine($"Found {divs.Count()} div elements");

                // חיפוש לפי ID
                var elementById = root.FindById("myId");
                if (elementById != null)
                {
                    Console.WriteLine($"Found element with id 'myId': <{elementById.Name}>");
                }

                // חיפוש לפי class
                var elementsByClass = root.FindByClass("container");
                Console.WriteLine($"Found {elementsByClass.Count()} elements with class 'container'");

                // חיפוש מורכב לפי selector
                var complexQuery = root.FindElements("div p.highlight");
                Console.WriteLine($"Found {complexQuery.Count()} elements matching 'div p.highlight'");

                // דוגמה 3: מעבר על צאצאים
                Console.WriteLine("\n📊 Tree structure:");
                var body = root.FindByTagName("body").FirstOrDefault();
                if (body != null)
                {
                    Console.WriteLine($"Body element has {body.Children.Count} direct children");
                    Console.WriteLine($"Body element has {body.Descendants().Count()} total descendants");
                }

                Console.WriteLine("\n✅ All operations completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}